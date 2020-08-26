using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Sonolib.Services;
using Xunit;
using Xunit.Abstractions;

namespace Sonolib.Tests
{
    public class ClientTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IConfiguration _configuration;
        private readonly IHttpService _httpService;
        private readonly IService _service;
        private const string Network = "TestNet";
        
        public ClientTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Settings.Test.json")
                .Build();

            var clientSettings = 
                _configuration.GetSection($"CryptoServices:SONO:MainNet");
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
        public async Task GetBalances()
        {
            var addresses = new List<string>
            {
                "SCg1YUnoZA6mwQr4vuXQhi1158JHGguotjY",
            };

            var balances = await _httpService.GetBulkWalletBalances(Network, addresses);

            foreach (var json in balances.Balances.Select(balance => JsonSerializer.Serialize(balance)))
            {
                _testOutputHelper.WriteLine(json);
            }
        }
    }
}