using System;

namespace AutoMapper
{
    /// <summary>
    ///
    /// </summary>
    [Obsolete("Switch to Automapper 9.x.x and use AutoMapper.Profile instead of IAutoMapper")]
    public class MapperException : Exception
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        public MapperException(string message) : base(message)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public MapperException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}