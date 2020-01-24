using System;
using System.Linq;
using System.Reflection;

namespace AutoMapper
{
    internal static class MapperExceptionFactory
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        internal static MapperException Throw(this ReflectionTypeLoadException ex)
        {
            string messages = ex.LoaderExceptions.Aggregate(string.Empty, (current, exception) => current + exception.Message + Environment.NewLine);
            return new MapperException($"Could not load these types: {messages} ", ex);
        }
    }
}