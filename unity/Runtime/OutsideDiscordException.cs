
using System;

namespace Dissonity
{
    /// <summary>
    /// This exception is thrown by the <c> Api.Initialize </c> method when the application runs
    /// outside the Discord client. <br/> <br/>
    /// 
    /// You may use it to detect whether the app is running inside Discord or
    /// in a browser.
    /// </summary>
    public class OutsideDiscordException : Exception
    {
        public OutsideDiscordException() : base() {}
        public OutsideDiscordException(string message) : base(message) {}
    }
}