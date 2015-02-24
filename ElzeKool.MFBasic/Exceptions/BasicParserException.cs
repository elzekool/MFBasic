using System;
using Microsoft.SPOT;

namespace ElzeKool.MFBasic.Exceptions
{
    public sealed class BasicParserException : Exception
    {
        public const String UNEXPECTED_END_OF_LINE = "Unexpected End of Line";
        public const String NUMBER_TO_LONG = "Number is to long";
        public const String VARIABLENAME_UNEXPECTED_END = "Unexpected end of Variable name";
        public const String UNKNOWN_ERROR = "Unknown error while parsing";


        public BasicParserException() : base() { }
        public BasicParserException(String Message) : base(Message) { }
        public BasicParserException(String Message, Exception innerException) : base(Message, innerException) { }
    }
}
