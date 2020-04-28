using System;
using AutoMapper;

namespace Dime.AutoMapper.Tests.Models
{
    public class Mapping : IAutoMapper
    {
        public Action<IMapperConfigurationExpression> Configure()
        {
            return (x) =>
            {
                x.CreateMap<TestClassThree, TestClassFour>();
                x.CreateMap<TestClassFour, TestClassThree>();
            };
        }
    }
}