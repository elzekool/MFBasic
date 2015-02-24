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
    public sealed class GosubToken : GeneralStatementToken
    {
        public GosubToken(CodeParser Parser, String Content) : base(Parser, Content) { }

        /// <summary>
        /// Run Statement
        /// </summary>
        /// <param name="BasicIntrepeter">Basic Intrepeter that called the Statement</param>
        public override void ExcecuteStatement(Basic BasicIntrepeter)
        {
            BasicIntrepeter.CodeParser.Expect(typeof(GosubToken));

            if (BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(NumericToken)))
            {
                double GotoLineNumber = BasicIntrepeter.CodeParser.CurrentToken.asNumber();
                BasicIntrepeter.CodeParser.Expect(typeof(NumericToken));
                
                // Find start of next line (Used for if then else loops)
                while (!BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(EndOfLineToken)) && !BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(EndOfInputToken)))
                    BasicIntrepeter.CodeParser.Next();
                
                if (BasicIntrepeter.GOSUBStackPosition < (BasicIntrepeter.GOSUBStack.Length - 1))
                {
                    BasicIntrepeter.GOSUBStack[BasicIntrepeter.GOSUBStackPosition] = BasicIntrepeter.CodeParser.ProgramPosition - 1;
                    BasicIntrepeter.GOSUBStackPosition++;
                    BasicIntrepeter.JumpToLine(GotoLineNumber);
                }
                else
                {
                    throw new MFBasic.Exceptions.BasicLanguageException(MFBasic.Exceptions.BasicLanguageException.GOSUB_STACK_EXHAUSTED);
                }
            }
            else if (BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(TextToken)))
            {
                String GotoLabel = BasicIntrepeter.CodeParser.CurrentToken.Content;
                BasicIntrepeter.CodeParser.Expect(typeof(TextToken));

                // Find start of next line (Used for if then else loops)
                while (!BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(EndOfLineToken)) && !BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(EndOfInputToken)))
                    BasicIntrepeter.CodeParser.Next();

                if (BasicIntrepeter.GOSUBStackPosition < (BasicIntrepeter.GOSUBStack.Length - 1))
                {
                    BasicIntrepeter.GOSUBStack[BasicIntrepeter.GOSUBStackPosition] = BasicIntrepeter.CodeParser.ProgramPosition - 1;
                    BasicIntrepeter.GOSUBStackPosition++;
                    BasicIntrepeter.JumpToLabel(GotoLabel);
                }
                else
                {
                    throw new MFBasic.Exceptions.BasicLanguageException(MFBasic.Exceptions.BasicLanguageException.GOSUB_STACK_EXHAUSTED);
                }
            }
        }

    }
}
