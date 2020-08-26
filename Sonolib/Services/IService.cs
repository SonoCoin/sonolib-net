 using System.Threading.Tasks;
 using Sonolib.Contracts.Functions;
 using Sonolib.Dtos.Extended;

 namespace Sonolib.Services
{
    public interface IService
    {
        WalletDto CreateWallet(int index = 0);
        Task<BalanceDto> GetBalance(string network, string address);
        Task<NonceDto> GetNonce(string network, string address);
        // Task<string> Send(string network, string seed, int index, string receiver, decimal amount, ulong nonce);
        // Task<string> Send(string network, string seed, int index, string receiver, decimal amount, bool useUnconfirmed = false);
        Task<T> StaticCall<T>(string network, string address, ContractFunctionCall call);
    }
}