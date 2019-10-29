using System;

namespace Bb.Workflows.Exceptions
{
    [Serializable]
    public class InvalidArgumentNameMethodReferenceException : Exception
    {
        public InvalidArgumentNameMethodReferenceException() { }
        public InvalidArgumentNameMethodReferenceException(string message) : base(message) { }
        public InvalidArgumentNameMethodReferenceException(string message, Exception inner) : base(message, inner) { }
        protected InvalidArgumentNameMethodReferenceException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

}
