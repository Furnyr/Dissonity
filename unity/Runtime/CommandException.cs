
using System;

namespace Dissonity
{
    /// <summary>
    /// This exception is thrown by the <c> Api.Commands </c> and <c> Api.Subscribe </c> methods when the Discord client sends an error response for a command. <br/> <br/>
    /// 
    /// You can access the <c> Message </c> and <c> Code </c> fields for more information.
    /// </summary>
    public class CommandException : Exception
    {
        public int Code { get; }
        public CommandException(string message, int code) : base(message)
        {
            Code = code;
        }
    }
}