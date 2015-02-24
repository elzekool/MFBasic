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
using ElzeKool.MFBasic.Tokens.RoutineHelpers;

using ElzeKool.MFBasic.Utilities;

namespace ElzeKool.MFBasic.Tokens.Routines
{
    public sealed class ForToken : GeneralStatementToken
    {
        public ForToken(CodeParser Parser, String Content) : base(Parser, Content) { }

        /// <summary>
        /// Class used for adding For loops to the stack
        /// </summary>
        [Serializable]
        public class ForStackItem
        {
            public readonly int TokenAfterFor;
            public readonly byte ForVariable;
            public readonly double To;
            public readonly double Step;

            /// <summary>
            /// Instantiate a new For State
            /// </summary>
            /// <param name="LineAfterFor"></param>
            /// <param name="ForVariable"></param>
            /// <param name="To"></param>
            public ForStackItem(int TokenAfterFor, byte ForVariable, double To, double Step)
            {
                this.TokenAfterFor = TokenAfterFor;
                this.ForVariable = ForVariable;
                this.To = To;
                this.Step = Step;
            }
        }


        /// <summary>
        /// Run Statement
        /// </summary>
        /// <param name="BasicIntrepeter">Basic Intrepeter that called the Statement</param>
        public override void ExcecuteStatement(Basic BasicIntrepeter)
        {
            byte var;
            double to;
            double step = 1;

            BasicIntrepeter.CodeParser.Expect(typeof(ForToken));

            var = BasicIntrepeter.CodeParser.CurrentToken.asVariableID(BasicIntrepeter);
            BasicIntrepeter.CodeParser.Expect(typeof(NumericVariableToken));

            BasicIntrepeter.CodeParser.Expect(typeof(EqualToken));

            ((Basic.NumericBasicVariable)BasicIntrepeter.NumericVariables[var]).Value = BasicIntrepeter.NumericModifier.ExpressionsLevel2();
            
            BasicIntrepeter.CodeParser.Expect(typeof(ToToken));
            to = BasicIntrepeter.NumericModifier.ExpressionLevel3();

            // Check for Step
            if (BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(StepToken)))
            {
                BasicIntrepeter.CodeParser.Expect(typeof(StepToken));
                step = BasicIntrepeter.NumericModifier.ExpressionLevel3();

            }
            
            // Ok, can't be used on a single if then else line
            BasicIntrepeter.CodeParser.Expect(typeof(EndOfLineToken));

            if (BasicIntrepeter.FORStackPosition < (BasicIntrepeter.FORStack.Length - 1))
            {
                // Debug.Print(Parser.CurrentTokenValue().ToString());
                BasicIntrepeter.FORStack[BasicIntrepeter.FORStackPosition] = new ForStackItem(BasicIntrepeter.CodeParser.ProgramPosition - 1, (byte)var, to, step);
                BasicIntrepeter.FORStackPosition++;
            }
            else
            {
                throw new MFBasic.Exceptions.BasicLanguageException(MFBasic.Exceptions.BasicLanguageException.FOR_STACK_EXHAUSTED);
            }
        }


    }
}
