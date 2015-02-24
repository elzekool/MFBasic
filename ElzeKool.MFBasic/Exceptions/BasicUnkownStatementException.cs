using System;
using Microsoft.SPOT;

namespace ElzeKool.MFBasic.Exceptions
{
    public sealed class BasicUnkownStatementException : Exception
    {
        public BasicUnkownStatementException() : base() { }
        public BasicUnkownStatementException(String Message) : base(Message) { }
        public BasicUnkownStatementException(String Message, Exception innerException) : base(Message, innerException) { }
    }
}


