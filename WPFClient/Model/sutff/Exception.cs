using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WPFClient.Model.sutff
{
    internal class VIException : Exception
    {
        public VIException()
        {
        }

        public VIException(string? message) : base(message)
        {
        }

        public VIException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected VIException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
