using System;
using Microsoft.SPOT;

using ElzeKool.MFBasic.Tokens;
using ElzeKool.MFBasic.Tokens.Operator;
using ElzeKool.MFBasic.Tokens.SpecialChars;
using ElzeKool.MFBasic.Tokens.Functions;

namespace ElzeKool.MFBasic.Tokens.Functions.Strings
{
    public sealed class FormatDateTime__Token : StringFunctionToken
    {
        public FormatDateTime__Token(CodeParser Parser, String Content) : base(Parser, Content) { }

        public override string GetStringResult(Basic BasicIntrepeter)
        {
            String Formatter = "";
            Double NumberToFormat = 0F;

            // Expect this function
            BasicIntrepeter.CodeParser.Expect(typeof(FormatDateTime__Token));

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
                NumberToFormat = BasicIntrepeter.NumericModifier.Value() * TimeSpan.TicksPerSecond;
            }
            else
            {
                BasicIntrepeter.CodeParser.Expect(typeof(NumericToken));
            }

            // Expect Right Parenthis )
            BasicIntrepeter.CodeParser.Expect(typeof(RightParenToken));

            // Return formatted date time
            return MFToolkit.MicroUtilities.DateFormatter.Format(Formatter, new DateTime((long) NumberToFormat));

        }
    }
}