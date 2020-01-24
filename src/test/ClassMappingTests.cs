using AutoMapper;
using Dime.AutoMapper.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dime.AutoMapper.Tests
{
    [TestClass]
    public class ClassMappingTests
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
        }

        [TestInitialize]
        public void Init()
        {
        }

        [TestMethod]
        [TestCategory("AutoMapper")]
        public void TestClassModelUsingInstance()
        {
            IMapper mapper = AutoMapperFactory.Create();
            TestClassOne instance1 = new TestClassOne { Id = 2 };
            TestClassTwo instance2 = mapper.Map<TestClassTwo>(instance1);

            Assert.IsNotNull(instance2);
            Assert.AreEqual(instance2.Id, 2);
        }

        [TestMethod]
        [TestCategory("AutoMapper")]
        public void TestClassModelOtherAssemblyUsingInstance()
        {
            IMapper mapper = AutoMapperFactory.Create();
            TestClassFour instance1 = new TestClassFour { Id = 2 };
            TestClassThree instance2 = mapper.Map<TestClassThree>(instance1);

            Assert.IsNotNull(instance2);
            Assert.AreEqual(instance2.Id, 2);
        }
    }
}