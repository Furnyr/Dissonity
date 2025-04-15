
using System;

namespace Dissonity
{
    /// <summary>
    /// This exception is thrown by the <c> Api.Initialize </c> method when the hiRPC handshake
    /// is an error payload. <br/> <br/>
    /// 
    /// This usually means something failed during authentication. Read the Discord console.
    /// </summary>
    /// 
    public class ExternalException : Exception
    {
        public ExternalException() : base() {}
        public ExternalException(string message) : base(message) {}
    }
}