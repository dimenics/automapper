using System;

namespace AutoMapper
{
    /// <summary>
    ///
    /// </summary>
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