using System;
using Microsoft.SPOT;

using ElzeKool.MFBasic.Tokens;
using ElzeKool.MFBasic.Tokens.Operator;
using ElzeKool.MFBasic.Tokens.SpecialChars;
using ElzeKool.MFBasic.Tokens.Functions;

namespace ElzeKool.MFBasic.Tokens.Functions.Strings
{
    public sealed class FormatNumber__Token : StringFunctionToken
    {
        public FormatNumber__Token(CodeParser Parser, String Content) : base(Parser, Content) { }

        public override string GetStringResult(Basic BasicIntrepeter)
        {
            String Formatter = "";
            Double NumberToFormat = 0F;

            // Expect this function
            BasicIntrepeter.CodeParser.Expect(typeof(FormatNumber__Token));

            // Expect Left Parenthis (
            BasicIntrepeter.CodeParser.Expect(typeof(LeftParenToken));

            // Expect string 
            if (BasicIntrepeter.StringModifier.isString())
            {
                Formatter = BasicIntrepeter.StringModifier.Value();
            }
            else
            {
                BasicIntrepeter.CodeParser.Expect(typeof(StringToken));
            }

            // Expect comma ,
            BasicIntrepeter.CodeParser.Expect(typeof(CommaToken));

            // Expect number
            if (BasicIntrepeter.NumericModifier.isNumeric())
            {
                NumberToFormat = BasicIntrepeter.NumericModifier.ExpressionLevel3();
            }
            else
            {
                BasicIntrepeter.CodeParser.Expect(typeof(NumericToken));
            }

            // Expect Right Parenthis )
            BasicIntrepeter.CodeParser.Expect(typeof(RightParenToken));

            // Return formatted number
            return MFToolkit.MicroUtilities.NumberFormatter.FormatNumber(Formatter, NumberToFormat);

        }
    }
}