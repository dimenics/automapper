using System;

namespace AutoMapper
{
    /// <summary>
    /// Interface used by Dime's mapping bootstrapper to detect all classes that execute auto mapping
    /// </summary>
    [Obsolete("Switch to Automapper 9.x.x and use AutoMapper.Profile instead of IAutoMapper")]
    public interface IAutoMapper
    {
        /// <summary>
        /// Configures the mapping
        /// </summary>
        Action<IMapperConfigurationExpression> Configure();
    }
}