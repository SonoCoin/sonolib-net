using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Sonolib.Dtos;
using Sonolib.Dtos.Extended;
using Sonolib.Extensions;
using Microsoft.Extensions.Logging;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Sonolib.Services
{
    public class HttpService : IHttpService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HttpService> _logger;
        private readonly IConfiguration _configuration;
        
        private static JsonSerializerOptions _jsonOpts => new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        };

        public HttpService(IHttpClientFactory httpClientFactory, 
            ILogger<HttpService> logger,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _configuration = configuration;
        }

        #region Get client
        
        private HttpClient GetClient(string network)
        {
            return  _httpClientFactory.CreateClient($"SONO-{network.ToNetwork()}");
        }
        
        #endregion
        
        #region Error handler
        
        private static async Task CheckErrors(string url, HttpResponseMessage response)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            BlockchainError errorResponse = null;
            
            try
            {
                // errorResponse = JsonConvert.DeserializeObject<BlockchainError>(jsonString);
                errorResponse = JsonSerializer.Deserialize<BlockchainError>(jsonString, _jsonOpts);
            }
            catch
            {
                // Ignore
            }

            if (!response.IsSuccessStatusCode)
            {
                var msg = new List<string>
                {
                    $"Url: {url}",
                    response.StatusCode.ToString(),
                };
                throw new Exception(errorResponse?.Message ?? string.Join(Environment.NewLine, msg));
            }

            if (!string.IsNullOrEmpty(errorResponse?.Message))
            {
                var msg = new List<string>
                {
                    $"Url: {url}",
                    errorResponse.Message
                };
                throw new Exception(errorResponse.Message ?? string.Join(Environment.NewLine, msg));
            }
        }
        
        #endregion
        
        #region Get, Post

        private async Task<T> Get<T>(string network, string url)
        {
            var client = GetClient(network);
            var uri = $"{client.BaseAddress.AbsolutePath}{url}";
            _logger.LogInformation($"GET {uri}");
            var response = await client.GetAsync(uri);
            var jsonString = await response.Content.ReadAsStringAsync();
            await CheckErrors(uri, response);
            // return JsonConvert.DeserializeObject<T>(jsonString, _jsonSettings);
            return JsonSerializer.Deserialize<T>(jsonString, _jsonOpts);
        }

        private async Task<T2> Post<T1, T2>(string network, string url, T1 model)
        {
            // var reqJson = JsonConvert.SerializeObject(model, _jsonSettings);
            var reqJson = JsonSerializer.Serialize(model, _jsonOpts);

            var client = GetClient(network);
            var uri = $"{client.BaseAddress.AbsolutePath}{url}";
            _logger.LogInformation($"POST {uri} (json): {reqJson}");

            var content = new StringContent(reqJson, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(uri, content);
            var jsonString = await response.Content.ReadAsStringAsync();
            await CheckErrors(uri, response);
            // return JsonConvert.DeserializeObject<T2>(jsonString, _jsonSettings);
            return JsonSerializer.Deserialize<T2>(jsonString, _jsonOpts);
        }
        
        #endregion

        #region Get balance

        public async Task<BalanceDto> GetBalance(string network, string address)
        {
            var url = $"/account/{address}/balance";
            var item = await Get<BalanceDto>(network, url);
            item.Address = address;
            return item;
        }

        public async Task<BalanceListDto> GetBulkWalletBalances(string network, List<string> walletAddresses)
        {
            const string url = "/account/balances";
            
            // var model = new BalancesRequest
            // {
            //     Addresses = walletAddresses
            // };

            return await Post<List<string>, BalanceListDto>(network, url, walletAddresses);
        }

        #endregion

        #region Send

        public async Task<TransactionResultDto> Send(string network, TransactionRequest model)
        {
            const string url = "/txs/publish";
            return await Post<TransactionRequest, TransactionResultDto>(network, url, model);
        }

        #endregion

        #region Get Nonce

        public async Task<NonceDto> GetNonce(string network, string address)
        {
            var url = $"/account/{address}/nonce";
            return await Get<NonceDto>(network, url);
        }

        public async Task<NonceDto> GetAllowanceNonce(string network, string address)
        {
            var url = $"/wallet/{address}/allowance_nonce";
            return await Get<NonceDto>(network, url);
        }
        
        #endregion

        #region Headers
        
        public async Task<List<BlockHeaderDto>> GetHeaders(string network, int offset = 0, int limit = 0)
        {
            var url = $"/headers?offset={offset}&limit={limit}";
            return await Get<List<BlockHeaderDto>>(network, url);
        }
        
        public async Task<List<BlockHeaderDto>> GetHeadersFromHeight(string network, int height = 1, int limit = 1)
        {
            var url = $"/headers/from_height/{height}?&limit={limit}";
            return await Get<List<BlockHeaderDto>>(network, url);
        }
        
        public async Task<BlockHeaderDto> GetHeader(string network, int height)
        {
            var url = $"/headers/height/{height}";
            return await Get<BlockHeaderDto>(network, url);
        }

        public async Task<List<BlockHeaderDto>> GetBlockHeaders(HeadersRequestDto model)
        {
            // @TODO maybe remove offset
            var url = $"/headers?offset={model.Offset}";
            return await Get<List<BlockHeaderDto>>(model.NetworkName, url);
        }      

        #endregion

        #region Blocks
        
        public async Task<BlockDto> GetBlock(string network, string hash)
        {
            var url = $"/blocks/{hash}";
            return await Get<BlockDto>(network, url);
        }

        public async Task<List<BlockDto>> GetBlocks(string network, List<string> hashes)
        {
            var model = new BlocksRequestDto {Hashes = hashes};
            var url = "/blocks/hashes";
            return await Post<BlocksRequestDto, List<BlockDto>>(network, url, model);
        }

        public async Task<List<BlockDto>> GetBlocks(string network, int offset = 0, int limit = 0)
        {
            var url = $"/blocks?offset={offset}&limit={limit}";
            return await Get<List<BlockDto>>(network, url);
        }

        #endregion

        #region Transactions

        public async Task<List<TransactionExtendedDto>> GetTransListByHashList(TxRequest model)
        {
            var reqJson = new TxRequestByHash {HashList = model.HashList};
            const string url = "/txs";
            return await Post<TxRequestByHash, List<TransactionExtendedDto>>(model.NetworkName, url, reqJson);
        }
        
        public async Task<List<string>> GetTxHashesByAccount(string network, string address)
        {
            var url = $"/account/{address}/txs";
            return await Get<List<string>>(network, url);
        }
        
        #endregion

        #region Contract static call

        public async Task<ContractStaticCallResponseDto> ContractStaticCall(string network, ContractStaticCallRequestDto model)
        {
            const string url = "/contract/static_call";
            return await Post<ContractStaticCallRequestDto, ContractStaticCallResponseDto>(network, url, model);
        }

        #endregion

        #region Contract consumed fee

        public async Task<ContractConsumedFeeResponseDto> ContractConsumedFee(string network, ContractConsumedFeeRequestDto model)
        {
            const string url = "/contract/consumed_fee";
            return await Post<ContractConsumedFeeRequestDto, ContractConsumedFeeResponseDto>(network, url, model);
        }

        #endregion
    }
}