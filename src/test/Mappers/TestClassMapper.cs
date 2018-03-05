using AutoMapper;
using Dime.AutoMapper.Tests.Models;
using System;

namespace Dime.AutoMapper.Tests.Mappers
{
    public class TestClassMapper : IAutoMapper
    {
        public Action<IMapperConfigurationExpression> Configure()
        {
            return (x) =>
            {
                x.CreateMap<TestClassOne, TestClassTwo>();
            };
        }
    }
}