using System;
using Microsoft.SPOT;

using ElzeKool.MFBasic;

using ElzeKool.MFBasic.Tokens;
using ElzeKool.MFBasic.Tokens.Operator;
using ElzeKool.MFBasic.Tokens.SpecialChars;
using ElzeKool.MFBasic.Tokens.Functions;

using ElzeKool.MFBasic.Utilities;

namespace ElzeKool.MFBasic.Tokens.Functions.Numeric
{
    public sealed class CosToken : NumericFunctionToken
    {
        public CosToken(CodeParser Parser, String Content) : base(Parser, Content) { }

        public override double GetNumericResult(Basic BasicIntrepeter)
        {
            double Input = 0F;

            BasicIntrepeter.CodeParser.Expect(typeof(CosToken));

            if (BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(LeftParenToken)))
            {
                Input = BasicIntrepeter.NumericModifier.Value();
            }
            else
            {
                BasicIntrepeter.CodeParser.Expect(typeof(LeftParenToken));
            }

            return MFToolkit.MicroUtilities.exMath.Cos(Input);
        }
    }
}
