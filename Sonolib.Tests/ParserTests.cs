using System;
using System.Linq;
using Sonolib.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Sonolib.Tests
{
    public class ParserTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ParserTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void BigEndianTest()
        {
            const string hex = "0000000000008813";

            var buf = hex.HexDecode();
            var num = BitConverter.ToUInt64(buf.Reverse().ToArray());
            
            _testOutputHelper.WriteLine($"{num} - {hex}");
        }
    }
}