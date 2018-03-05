using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dime.AutoMapper.Tests
{
    /// <summary>
    ///
    /// </summary>
    [TestClass]
    public class AutoMapperFactoryTests
    {
        /// <summary>
        ///
        /// </summary>
        public AutoMapperFactoryTests()
        {
        }

        [TestMethod]
        [TestCategory("AutoMapper")]
        public void TestAutoMapperFactoryScanning()
        {
            IMapper mapper = AutoMapperFactory.Create();
            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [TestMethod]
        [TestCategory("AutoMapper")]
        public void TestStaticAutoMapperFactoryScanning()
        {
            AutoMapperFactory.Initialize();
        }
    }
}