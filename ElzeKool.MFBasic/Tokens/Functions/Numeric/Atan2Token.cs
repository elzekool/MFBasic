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
    public sealed class ATan2Token : NumericFunctionToken
    {
        public ATan2Token(CodeParser Parser, String Content) : base(Parser, Content) { }

        public override double GetNumericResult(Basic BasicIntrepeter)
        {
            double Input1 = 0F;
            double Input2 = 0F;

            // Expect function token
            BasicIntrepeter.CodeParser.Expect(typeof(ATan2Token));

            // Expect left token
            BasicIntrepeter.CodeParser.Expect(typeof(LeftParenToken));

            // Expect number 1
            if (BasicIntrepeter.NumericModifier.isNumeric())
            {
                Input1 = BasicIntrepeter.NumericModifier.Value();
            }
            else
            {
                BasicIntrepeter.CodeParser.Expect(typeof(NumericToken));
            }

            // Expect comma ,
            BasicIntrepeter.CodeParser.Expect(typeof(CommaToken));

            // Expect number 2
            if (BasicIntrepeter.NumericModifier.isNumeric())
            {
                Input2 = BasicIntrepeter.NumericModifier.Value();
            }
            else
            {
                BasicIntrepeter.CodeParser.Expect(typeof(NumericToken));
            }

            // Expect right token
            BasicIntrepeter.CodeParser.Expect(typeof(RightParenToken));

            return MFToolkit.MicroUtilities.exMath.Atan2(Input1, Input2);
        }
    }
}
