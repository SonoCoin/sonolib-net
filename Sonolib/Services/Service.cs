using System.Linq;
using System.Threading.Tasks;
using Sonolib.Dtos.Extended;
using Sonolib.Extensions;
using Sonolib.Helpers;
using NBitcoin;
using Sonolib.Contracts.Extensions;
using Sonolib.Contracts.Functions;
using Sonolib.Dtos;
using Key = Sonolib.Crypto.Key;

namespace Sonolib.Services
{
    public class Service : IService
    {
        private readonly IHttpService _httpService;
        private const decimal CurrencyDivider = 100000000;

        public Service(IHttpService httpService)
        {
            _httpService = httpService;
        }

        #region CreateWallet
        
        public WalletDto CreateWallet(byte[] seed, int index)
        {
            var key = new Key(seed, index);

            return new WalletDto
            {
                Seed = seed.ToHex(),
                PrivateKey = key.PrivateKey.ToHex(),
                PublicKey = key.PublicKey.ToHex(),
                Address = key.ToWallet().Base58Address,
            };
        }

        public WalletDto CreateWallet(int index)
        {
            var mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
            var seed = mnemonic.DeriveSeed();
            
            var key = new Key(seed, index);
            var words = string.Join(" ", mnemonic.Words);

            return new WalletDto
            {
                Mnemonic = words,
                Seed = seed.ToHex(),
                PrivateKey = key.PrivateKey.ToHex(),
                PublicKey = key.PublicKey.ToHex(),
                Address = key.ToWallet().Base58Address,
            };
        }
        
        #endregion
        
        #region Balance
        
        /// <summary>
        ///  return Balance in decimal format, already devided to 100000000
        /// </summary>
        /// <param name="network"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public async Task<BalanceDto> GetBalance(string network, string address)
        {
            var item = await _httpService.GetBalance(network, address);
            item.ConfirmedAmount /= CurrencyDivider;
            item.UnconfirmedAmount /= CurrencyDivider;
            return item;
        }
        
        #endregion

        #region Nonce

        public async Task<NonceDto> GetNonce(string network, string address)
        {
            return await _httpService.GetNonce(network, address);
        }

        #endregion

        // #region Send
        //
        // public async Task<string> Send(string network, string seed, int index, string receiver, decimal amount, ulong nonce)
        // {
        //     var key = new Key(seed.HexDecode(), index);
        //     var wallet = key.ToWallet();
        //     
        //     var txAmount = (ulong) (amount * (decimal) MoneyUnit.BTC);
        //     
        //     var tx = new TransactionRequest()
        //         .AddSender(wallet.Base58Address, key, txAmount + Constants.Commission, nonce)
        //         .AddTransfer(receiver, txAmount)
        //         .Sign();
        //     
        //     await _httpService.Send(network, tx);
        //     return tx.Hash;
        // }
        //
        // public async Task<string> Send(string network, string seed, int index, string receiver, decimal amount, bool useUnconfirmed = false)
        // {
        //     var key = new Key(seed.HexDecode(), index);
        //     var wallet = key.ToWallet();
        //
        //     var nonceDto = await _httpService.GetNonce(network, wallet.Base58Address);
        //     var nonce = nonceDto.ConfirmedNonce;
        //     if (useUnconfirmed)
        //     {
        //         nonce = nonceDto.UnconfirmedNonce;
        //     }
        //     var txAmount = (ulong) (amount * (decimal) MoneyUnit.BTC);
        //     
        //     var tx = new TransactionRequest()
        //         .AddSender(wallet.Base58Address, key, txAmount + Constants.Commission, nonce)
        //         .AddTransfer(receiver, txAmount)
        //         .Sign();
        //     
        //     await _httpService.Send(network, tx);
        //     return tx.Hash;
        // }
        //
        // #endregion
        
        #region Contracts

        public async Task<T> StaticCall<T>(string network, string address, ContractFunctionCall call)
        {
            var payload = call.ArgsCall.Aggregate(call.Function.NameHex, (current, argCall) => current + argCall.ToPayloadHex());

            var req = new ContractStaticCallRequestDto
            {
                Address = address,
                Payload = payload
            };
            var res = await _httpService.ContractStaticCall(network, req);

            return res.Result.ConvertTo<T>();
        }
        
        #endregion
    }
}