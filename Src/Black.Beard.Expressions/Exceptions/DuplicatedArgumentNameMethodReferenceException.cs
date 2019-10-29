using System;

namespace Bb.Exceptions
{
    [Serializable]
    public class DuplicatedArgumentNameMethodReferenceException : Exception
    {
        public DuplicatedArgumentNameMethodReferenceException() { }
        public DuplicatedArgumentNameMethodReferenceException(string message) : base(message) { }
        public DuplicatedArgumentNameMethodReferenceException(string message, Exception inner) : base(message, inner) { }
        protected DuplicatedArgumentNameMethodReferenceException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

}
