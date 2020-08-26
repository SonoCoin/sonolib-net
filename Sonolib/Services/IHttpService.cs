using System.Collections.Generic;
using System.Threading.Tasks;
using Sonolib.Dtos;
using Sonolib.Dtos.Extended;

namespace Sonolib.Services
{
    
    /// <summary>
    /// Coin service interface
    /// </summary>
    public interface IHttpService
    {
        Task<BalanceDto> GetBalance(string network, string address);
        Task<BalanceListDto> GetBulkWalletBalances(string network, List<string> walletAddresses);
        
        Task<TransactionResultDto> Send(string network, TransactionRequest model);

        Task<NonceDto> GetNonce(string network, string address);
        Task<NonceDto> GetAllowanceNonce(string network, string address);
        
        Task<List<BlockHeaderDto>> GetHeaders(string network, int offset = 0, int limit = 0);
        Task<List<BlockHeaderDto>> GetHeadersFromHeight(string network, int height = 1, int limit = 1);
        Task<BlockHeaderDto> GetHeader(string network, int height);
        Task<List<BlockHeaderDto>> GetBlockHeaders(HeadersRequestDto model);

        Task<BlockDto> GetBlock(string network, string hash);
        Task<List<BlockDto>> GetBlocks(string network, List<string> hashes);
        Task<List<BlockDto>> GetBlocks(string network, int offset = 0, int limit = 0);
        Task<List<TransactionExtendedDto>> GetTransListByHashList(TxRequest model);
        Task<List<string>> GetTxHashesByAccount(string network, string address);
        Task<ContractStaticCallResponseDto> ContractStaticCall(string network, ContractStaticCallRequestDto model);
        Task<ContractConsumedFeeResponseDto> ContractConsumedFee(string network, ContractConsumedFeeRequestDto model);
    }
}
