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
using ElzeKool.MFBasic.Tokens;
using ElzeKool.MFBasic.Tokens.Functions;
using ElzeKool.MFBasic.Tokens.Operator;
using ElzeKool.MFBasic.Tokens.Routines;
using ElzeKool.MFBasic.Tokens.RoutineHelpers;
using ElzeKool.MFBasic.Tokens.SpecialChars;

namespace ElzeKool.MFBasic.Tokens.Routines
{
    public class WhileToken : GeneralStatementToken
    {
        public WhileToken(CodeParser Parser, String Content) : base(Parser, Content) { }
            
        /// <summary>
        /// Searches for WEND
        /// </summary>
        internal void SearchForWEND()
        {
            // Repeat until we've hit WEND
            while (!Parser.CurrentToken.GetType().Equals(typeof(WendToken)))
            {
                // If we hit a WHILE then look for that WEND first
                if (Parser.CurrentToken.GetType().Equals(typeof(WhileToken)))
                {
                    Parser.Next();
                    SearchForWEND();
                }

                Parser.Next();
            }

            // Skip Wend
            Parser.Expect(typeof(WendToken));

        }

        /// <summary>
        /// Run Statement
        /// </summary>
        /// <param name="BasicIntrepeter">Basic Intrepeter that called the Statement</param>
        public override void ExcecuteStatement(Basic BasicIntrepeter)
        {
            double ComparisonResult = 0F;
            bool openParenthis = false;

            BasicIntrepeter.CodeParser.Expect(typeof(WhileToken));

            // Alow the usage of parenthis like C language
            if (typeof(LeftParenToken).Equals(BasicIntrepeter.CodeParser.CurrentToken.GetType()))
            {
                BasicIntrepeter.CodeParser.Expect(typeof(LeftParenToken));
                openParenthis = true;
            }

            // Get comparison result
            ComparisonResult = BasicIntrepeter.NumericModifier.ExpressionLevel3();

            // If we had an opening parenthis we expect a closing one
            if (openParenthis)
                BasicIntrepeter.CodeParser.Expect(typeof(RightParenToken));

            // Check comparison result
            if (ComparisonResult < 1.0F)
            {
                // False -> Goto WEND and continue processing from there
                SearchForWEND();
            }
            else
            {
                // True -> Add to WHILE Stack

                // Search for earlier WHILE statement
                int NegativeOffset = 0;
                while (true)
                {
                    if (Parser.Program[Parser.ProgramPosition - NegativeOffset].GetType().Equals(typeof(WhileToken)))
                        break;
                    NegativeOffset++;
                }

                // Store WHILE position on stack
                if (BasicIntrepeter.WHILEStackPosition < (BasicIntrepeter.WHILEStack.Length - 1))
                {
                    BasicIntrepeter.WHILEStack[BasicIntrepeter.WHILEStackPosition] = Parser.ProgramPosition - NegativeOffset;
                    BasicIntrepeter.WHILEStackPosition++;
                }
                else
                    throw new MFBasic.Exceptions.BasicLanguageException(MFBasic.Exceptions.BasicLanguageException.WHILE_STACK_EXHAUSTED);

                // Find start of next line 
                while (!BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(EndOfLineToken)) && !BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(EndOfInputToken)))
                    BasicIntrepeter.CodeParser.Next();

            }
        }
    }
}