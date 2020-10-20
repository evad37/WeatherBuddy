using System;
using System.Runtime.Serialization;

namespace WeatherBuddy.Models
{
    [Serializable]
    internal class BadResponseException : Exception
    {
        public BadResponseException()
        {
        }

        public BadResponseException(string message) : base(message)
        {
        }

        public BadResponseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BadResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}