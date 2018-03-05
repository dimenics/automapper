using System;
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
        internal static MapperException Throw(ReflectionTypeLoadException ex)
        {
            string messages = string.Empty;
            foreach (Exception exception in ex.LoaderExceptions)
                messages += exception.Message + Environment.NewLine;

            return new MapperException(string.Format("Could not load these types: {0} ", messages), ex);
        }
    }
}