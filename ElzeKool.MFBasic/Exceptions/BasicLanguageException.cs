using System;
using Microsoft.SPOT;

namespace ElzeKool.MFBasic.Exceptions
{
    public sealed class BasicLanguageException : Exception
    {
        public const String AS_STRING_NOT_SUPPORTED = "asString() function only valid for String Tokens!";
        public const String AS_NUMBER_NOT_SUPPORTED = "asNumber() function only valid for String Tokens!";
        public const String AS_VARIABLEID_NOT_SUPPORTED = "asVariableID() function only valid for Variable Tokens!";
        public const String EXCECUTESTATEMENT_NOT_SUPPORTED = "ExcecuteStatement() called for a Token without a ExcecuteStatement() implementation";
        public const String GET_NUMERIC_RESULT_NOT_SUPPORTED = "GetNumericResult() called for a Token without a GetNumericResult() implementation";
        public const String GET_STRING_RESULT_NOT_SUPPORTED = "GetStringResult() called for a Token without a GetStringResult() implementation";

        public const String WEND_WITHOUT_WHILE = "WEND without WHILE in Stack";
        public const String WHILE_STACK_EXHAUSTED = "WHILE Stack exhausted";

        public const String RETURN_WITHOUT_GOSUB = "Return without GOSUB in Stack";
        public const String GOSUB_STACK_EXHAUSTED = "GOSUB Stack exhausted";
        public const String JUMP_TO_LINE_FAILED = "JUMP to Line Failed. End of program reached";
        public const String JUMP_TO_LABEL_FAILED = "JUMP to Label Failed. End of program reached";
        public const String NEXT_WITHOUT_FOR = "NEXT without previous FOR";
        public const String FOR_STACK_EXHAUSTED = "FOR Stack exhausted";

        public const String FAILED_TO_GET_VALUE = "Failed calculating value";
        public const String EXPECTED_COMPARE_OPERATOR = "Expected an compare operator";

        public const String MULTILINE_IF_WITHOUT_ENDIF = "IF without ENDIF";

        public BasicLanguageException() : base() { }
        public BasicLanguageException(String Message) : base(Message) { }
        public BasicLanguageException(String Message, Exception innerException) : base(Message, innerException) { }
    }
}


