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
using ElzeKool.MFBasic.Tokens.Functions;

namespace ElzeKool.MFBasic.Utilities
{
    public class StringModifiers
    {
        private Basic BasicIntrepeter;

        public StringModifiers(Basic BasicIntrepeter)
        {
            this.BasicIntrepeter = BasicIntrepeter;
        }

        /// <summary>
        /// Test if next argument is a String
        /// </summary>
        /// <returns></returns>
        public bool isString()
        {
            if (BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(StringToken)) ||
                BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(StringVariableToken)) ||
                BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(StringFunctionToken)) ||
                BasicIntrepeter.CodeParser.CurrentToken.GetType().IsSubclassOf(typeof(StringFunctionToken))
                )
                return true;
            else
                return false;
        }

        public int Compare()
        {
            // Get String 1
            String Varible1 = Value();

            Type Operator = BasicIntrepeter.CodeParser.CurrentToken.GetType();

            if (Operator.Equals(typeof(EqualToken)) ||
                Operator.Equals(typeof(NotEqualToken)))
            {
                BasicIntrepeter.CodeParser.Next();
                String Varible2 = Value();

                if (Operator.Equals(typeof(EqualToken)))
                {
                    if (Varible1 == Varible2) return 1; else return 0;
                }
                else if (Operator.Equals(typeof(NotEqualToken)))
                {
                    if (Varible1 != Varible2) return 1; else return 0;
                }
            }

            throw new MFBasic.Exceptions.BasicLanguageException(MFBasic.Exceptions.BasicLanguageException.EXPECTED_COMPARE_OPERATOR);

        }


        public String Value()
        {
            String retValue = null;

            if (BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(StringToken)))
            {
                retValue = BasicIntrepeter.CodeParser.CurrentToken.asString();
                BasicIntrepeter.CodeParser.Expect(typeof(StringToken));
            }
            else if (BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(StringVariableToken)))
            {
                retValue = ((Basic.StringBasicVariable )BasicIntrepeter.StringVariables[BasicIntrepeter.CodeParser.CurrentToken.asVariableID(BasicIntrepeter)]).Value;
                BasicIntrepeter.CodeParser.Expect(typeof(StringVariableToken));
            }
            else if (BasicIntrepeter.CodeParser.CurrentToken.GetType().IsSubclassOf(typeof(StringFunctionToken)))
            {
                retValue = BasicIntrepeter.CodeParser.CurrentToken.GetStringResult(BasicIntrepeter);
            }

            // Error 
            if (retValue == null)
            {
                BasicIntrepeter.CodeParser.Expect(typeof(StringToken));
                return null;
            }

            // + Modifier 
            if (BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(PlusToken)))
            {
                BasicIntrepeter.CodeParser.Expect(typeof(PlusToken));
                retValue = retValue + Value();
            }

            return retValue;

        }


    }
}