using System;

namespace AutoMapper
{
    /// <summary>
    /// Interface used by Dime's mapping bootstrapper to detect all classes that perform Automapping
    /// </summary>
    public interface IAutoMapper
    {
        /// <summary>
        ///
        /// </summary>
        Action<IMapperConfigurationExpression> Configure();
    }
}