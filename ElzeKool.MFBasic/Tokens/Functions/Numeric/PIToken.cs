using System;

using ElzeKool.MFBasic.Tokens;
using ElzeKool.MFBasic.Tokens.Operator;
using ElzeKool.MFBasic.Tokens.SpecialChars;
using ElzeKool.MFBasic.Tokens.Functions;

using ElzeKool.MFBasic.Utilities;

namespace ElzeKool.MFBasic.Tokens.Functions.Numeric
{
    public sealed class PIToken : NumericFunctionToken
    {
        public PIToken(CodeParser Parser, String Content) : base(Parser, Content) { }

        public override double GetNumericResult(Basic BasicIntrepeter)
        {
            BasicIntrepeter.CodeParser.Expect(typeof(PIToken));
            BasicIntrepeter.CodeParser.Expect(typeof(LeftParenToken));
            BasicIntrepeter.CodeParser.Expect(typeof(RightParenToken));
            return MFToolkit.MicroUtilities.exMath.PI;
        }
    }
}