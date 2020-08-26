using Sonolib.Crypto;
using Xunit;
using Xunit.Abstractions;

namespace Sonolib.Tests
{
    public class WalletTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public WalletTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void AddressTest()
        {
            const string invalid = "SC4677676767676767676766767565656656";
            Assert.False(Wallet.IsValidAddress(invalid));

            const string valid = "SCjRdq6w3QX8HWusFc6mUXbkMgbKB9shhZt";
            
            Assert.True(Wallet.IsValidAddress(valid));
        }
    }
}