/**
 * MFBasic Basic Interpeter
 * 
 * Copyright (C) 2009 Elze F.R. Kool
 * http://www.microframework.nl
 * http://mfbasic.codeplex.com
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 * 
 **/


using System;
using Microsoft.SPOT;

using ElzeKool.MFBasic;

using ElzeKool.MFBasic.Tokens;
using ElzeKool.MFBasic.Tokens.Operator;
using ElzeKool.MFBasic.Tokens.SpecialChars;


using ElzeKool.MFBasic.Utilities;

namespace ElzeKool.MFBasic.Tokens.Routines
{
    public sealed class PrintToken : GeneralStatementToken
    {
        public PrintToken(CodeParser Parser, String Content) : base(Parser, Content) { }

        /// <summary>
        /// Run Statement
        /// </summary>
        /// <param name="BasicIntrepeter">Basic Intrepeter that called the Statement</param>
        public override void ExcecuteStatement(Basic BasicIntrepeter)
        {
            BasicIntrepeter.CodeParser.Expect(typeof(PrintToken));
            String PrintOutput = "";

            do
            {
                // Comma, Output space, Expect next value/expression/string
                if (typeof(CommaToken).Equals(BasicIntrepeter.CodeParser.CurrentToken.GetType()))
                {
                    PrintOutput += " ";
                    BasicIntrepeter.CodeParser.Next();
                    continue;
                }

                // Semicolom, Output nothing, Expect next value/expression/string
                if (typeof(SemicolonToken).Equals(BasicIntrepeter.CodeParser.CurrentToken.GetType()))
                {
                    BasicIntrepeter.CodeParser.Next();
                    continue;
                }

                // Numeric Function
                if (BasicIntrepeter.NumericModifier.isNumeric())
                {
                    PrintOutput += BasicIntrepeter.NumericModifier.ExpressionLevel3().ToString();
                    continue;
                }

                // String Function
                if (BasicIntrepeter.StringModifier.isString())
                {
                    PrintOutput += BasicIntrepeter.StringModifier.Value().ToString();
                    continue;
                }



                break;

            } while ((!typeof(EndOfLineToken).Equals(BasicIntrepeter.CodeParser.CurrentToken.GetType())) && (!typeof(EndOfInputToken).Equals(BasicIntrepeter.CodeParser.CurrentToken.GetType())));

            // TODO: Something usefull for Print output?!
            Debug.Print(PrintOutput);
            BasicIntrepeter.CodeParser.Next();

        }
    }
}
