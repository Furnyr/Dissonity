
using System;

namespace Dissonity
{
    /// <summary>
    /// This exception is thrown by the <c> Api.Initialize </c> method when the user
    /// denies access to the OAuth2 scopes. <br/> <br/>
    /// 
    /// You usually want to <c> Api.Close </c> your app if you receive this exception.
    /// </summary>
    public class AuthorizationException : Exception
    {
        public AuthorizationException() : base() {}
        public AuthorizationException(string message) : base(message) {}
    }
}