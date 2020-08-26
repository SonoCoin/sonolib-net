using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Sonolib.Crypto;
using Sonolib.Dtos.Extended;
using Sonolib.Extensions;
using Sonolib.Helpers;
using Sonolib.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NBitcoin;
using Sonolib.Dtos;
using Xunit;
using Xunit.Abstractions;
using Key = Sonolib.Crypto.Key;

namespace Sonolib.Tests
{
    public class KeyTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IConfiguration _configuration;
        private readonly IHttpService _httpService;
        private readonly IService _service;
        private const string Network = "MainNet";

        public KeyTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Settings.Test.json")
                .Build();

            var clientSettings =
                _configuration.GetSection($"CryptoServices:SONO:TestNet");
            var mockFactory = new Mock<IHttpClientFactory>();
            var client = new HttpClient
            {
                BaseAddress = new Uri(clientSettings["Url"]),
                MaxResponseContentBufferSize = clientSettings["HttpBufferSizeKb"].ToInt(64) * 1024,
                Timeout = clientSettings["HttpTimeoutSec"].ToSeconds(60),
            };

            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var factory = mockFactory.Object;
            _httpService = new HttpService(factory, new NullLogger<HttpService>(), _configuration);
            _service = new Service(_httpService);
        }

        [Fact]
        public void Sha256Test()
        {
            var msg = Encoding.UTF8.GetBytes("Hello World.");
            var expectedHash = "f4bb1975bf1f81f76ce824f7536c1e101a8060a632a52289d530a6f600d52c92";

            var hash = msg.ToSha256();
            _testOutputHelper.WriteLine(hash.ToHex());

            Assert.Equal(expectedHash, hash.ToHex());
        }

        [Fact]
        public void KeysTest()
        {
            const string seed =
                "68bc96f22b3a447cdb1f82b9935396ea4dca90831c385e24970c70e3b17480e3460830a1b14db6f2eb021bf8628c8502e992ad74f6babd3fabbe164dd97e8c35";

            var key = new Key(seed.HexDecode(), 0);

            _testOutputHelper.WriteLine(key.PrivateKey.ToHex());
        }

        [Fact]
        public void WalletAddressTest()
        {
            var w = _service.CreateWallet();
            _testOutputHelper.WriteLine(w.Mnemonic);
            _testOutputHelper.WriteLine(w.Seed);
            _testOutputHelper.WriteLine(w.PrivateKey);
            _testOutputHelper.WriteLine(w.PublicKey);
            _testOutputHelper.WriteLine(w.Address);

            var key = new Key(w.Seed.HexDecode(), 0);
            var walletKeys = key.ToWallet();

            var key2 = new Key(w.PrivateKey.HexDecode());
            var walletKeys2 = key2.ToWallet();

            Assert.Equal(w.PrivateKey, key.PrivateKey.ToHex());

            Assert.Equal(w.Address, walletKeys.Base58Address);
            Assert.Equal(w.Address, walletKeys2.Base58Address);

            var pk = "02fbeb29c5dbbebe7a64d3bf441878ce8c4eb12f379ca41eb9237c362ca2431f8f";
            var expectedAddress = "SC8Ce9JAA9GuzBhrCQE8Mx4fMuNcsHJVB7w7";

            var wallet = new Wallet(pk.HexDecode());
            Assert.Equal(expectedAddress, wallet.Base58Address);
        }


        [Fact]
        public void VerifySignTest()
        {
            var pk = "02fbeb29c5dbbebe7a64d3bf441878ce8c4eb12f379ca41eb9237c362ca2431f8f".HexDecode();
            var sign =
                "73b306abb83df6d1533e16c62b23ce7c1e62f9ab648e0db4fe18326b9233340719b22197a1ceb8aeddf1bf73aa7da782b816205bfa01b75091f0f297ae0fffd8"
                    .HexDecode();
            var msg = Encoding.UTF8.GetBytes("Hello World.");

            Assert.True(Key.Verify(sign, msg, pk));
        }

        [Fact]
        public async Task GetBlockTest()
        {
            const string blockHash = "ae748aeb9e818152dbecf01ecc76512b1c6196914fe23b87da2d4544d9ead2bf";
            var block = await _httpService.GetBlock("Main", blockHash);

            var json = block.SerializeToJson(null, true);
            _testOutputHelper.WriteLine(json);
        }

        [Fact]
        public async Task TestManualTx()
        {
            const string receiver = "SCpk7rqhA56LRzv3PKHfr31ptoSsCSeC5xn";
            const string privateKey = " ";
            const decimal amount = 1000m;

            const ulong gasPrice = 3;
            const ulong gas = Constants.Commission;
            const ulong commission = gas * gasPrice;

            var key = new Key(privateKey.HexDecode());

            var wallet = key.ToWallet();

            var balance = await _httpService.GetBalance(Network, wallet.Base58Address);
            _testOutputHelper.WriteLine($"confirmed balance: {balance.ConfirmedAmount}");
            _testOutputHelper.WriteLine($"unconfirmed balance: {balance.UnconfirmedAmount}");

            var nonce = await _httpService.GetNonce(Network, wallet.Base58Address);

            const ulong txAmount = (ulong) (amount * (decimal) MoneyUnit.BTC);

            var tx = new TransactionRequest()
                .AddCommission(gasPrice, gas)
                .AddSender(wallet.Base58Address, key, txAmount + commission, nonce.ConfirmedNonce)
                .AddTransfer(receiver, txAmount)
                .Sign();

            // _testOutputHelper.WriteLine(tx.ToJson());

            var response = await _httpService.Send(Network, tx);

            _testOutputHelper.WriteLine(response.Result);
        }

    }
}