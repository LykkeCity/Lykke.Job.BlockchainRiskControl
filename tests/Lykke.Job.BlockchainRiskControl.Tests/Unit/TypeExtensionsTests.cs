using Lykke.Job.BlockchainRiskControl.DomainServices;
using System.ComponentModel;
using Xunit;

namespace Lykke.Job.BlockchainRiskControl.Tests.Unit
{
    public class TypeExtensionsTests
    {
        [DisplayName("TestDisplayName")]
        private class TestClass1
        {
        }

        private class TestClass2
        {
        }

        [Fact]
        public void Test_that_DisplayName_attribute_works()
        {
            var displayName = typeof(TestClass1).GetDisplayName();

            Assert.Equal("TestDisplayName", displayName);
        }

        [Fact]
        public void Test_that_plain_class_works()
        {
            var displayName = typeof(TestClass2).GetDisplayName();

            Assert.Equal("TestClass2", displayName);
        }
    }
}
