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
using ElzeKool.MFBasic.Utilities;

using ElzeKool.MFBasic.Tokens;
using ElzeKool.MFBasic.Tokens.Operator;
using ElzeKool.MFBasic.Tokens.SpecialChars;
using ElzeKool.MFBasic.Tokens.Functions;


namespace ElzeKool.MFBasic.Utilities
{
    public sealed class NumericModifiers
    {
        private Basic BasicIntrepeter;

        public NumericModifiers(Basic BasicIntrepeter)
        {
            this.BasicIntrepeter = BasicIntrepeter;
        }


        /// <summary>
        /// Test if next argument is numeric
        /// </summary>
        /// <returns></returns>
        public bool isNumeric()
        {
            if (BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(NumericToken)) ||
                BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(LeftParenToken)) ||
                BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(NumericVariableToken)) || 
                BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(NumericFunctionToken)) ||
                BasicIntrepeter.CodeParser.CurrentToken.GetType().IsSubclassOf(typeof(NumericFunctionToken))
                )
                return true;
            else
                return false;
        }

        /// <summary>
        /// Get Calculate Factor from Variable
        /// </summary>
        /// <returns></returns>
        public double VariableValue()
        {
            // Get current variable number
            double r = ((Basic.NumericBasicVariable)BasicIntrepeter.NumericVariables[BasicIntrepeter.CodeParser.CurrentToken.asVariableID(BasicIntrepeter)]).Value;

            // Check Token Type
            BasicIntrepeter.CodeParser.Expect(typeof(NumericVariableToken));

            // Return Variable number
            return r;
        }


        /// <summary>
        /// Get next Calculation Value
        /// </summary>
        /// <returns></returns>
        public double Value()
        {
            double r;

            // Basic Number
            if (typeof(NumericToken).Equals(BasicIntrepeter.CodeParser.CurrentToken.GetType()))
            {
                r = BasicIntrepeter.CodeParser.CurrentToken.asNumber();
                BasicIntrepeter.CodeParser.Expect(typeof(NumericToken));
                return r;
            }

            // Expression
            else if (typeof(LeftParenToken).Equals(BasicIntrepeter.CodeParser.CurrentToken.GetType()))
            {
                BasicIntrepeter.CodeParser.Expect(typeof(LeftParenToken));
                r = ExpressionLevel3();
                BasicIntrepeter.CodeParser.Expect(typeof(RightParenToken));
                return r;
            }

            // Variable 
            else if (typeof(NumericVariableToken).Equals(BasicIntrepeter.CodeParser.CurrentToken.GetType()))
            {
                r = VariableValue();
                return r;
            }

            // Function
            else if (BasicIntrepeter.CodeParser.CurrentToken.GetType().IsSubclassOf(typeof(NumericFunctionToken)))
            {
                r = BasicIntrepeter.CodeParser.CurrentToken.GetNumericResult(BasicIntrepeter);
                return r;
            }

            throw new MFBasic.Exceptions.BasicLanguageException(MFBasic.Exceptions.BasicLanguageException.FAILED_TO_GET_VALUE);

        }


        /// <summary>
        /// First level expressions (Multipy, Divide, Modulus)
        /// </summary>
        /// <returns>Calculated Expression</returns>
        public double ExpressionsLevel1()
        {
            // Factors used 
            double Value1, Value2;

            // Operator for factor
            Type Operator;

            Value1 = Value();
            Operator = BasicIntrepeter.CodeParser.CurrentToken.GetType();

            while ((typeof(MultiplyToken).Equals(Operator)) || (typeof(DivideToken).Equals(Operator)) || (typeof(ModulusToken).Equals(Operator)))
            {
                BasicIntrepeter.CodeParser.Next();
                Value2 = Value();

                if (typeof(MultiplyToken).Equals(Operator))
                {
                    Value1 = Value1 * Value2;
                }
                else if (typeof(DivideToken).Equals(Operator))
                {
                    Value1 = Value1 / Value2;
                }
                else if (typeof(ModulusToken).Equals(Operator))
                {
                    Value1 = Value1 % Value2;
                }

                Operator = BasicIntrepeter.CodeParser.CurrentToken.GetType();
            }

            return Value1;
        }

        /// <summary>
        /// Second Level Expressions (Plus, Minus, And, Or)
        /// </summary>
        /// <returns>Calculated Expression</returns>
        public double ExpressionsLevel2()
        {
            double Value1, Value2;
            Type Operator;

            Value1 = ExpressionsLevel1();
            Operator = BasicIntrepeter.CodeParser.CurrentToken.GetType();

            while ((typeof(PlusToken).Equals(Operator)) || (typeof(MinusToken).Equals(Operator)) ||
                (typeof(AndToken).Equals(Operator)) || (typeof(OrToken).Equals(Operator)))
            {
                BasicIntrepeter.CodeParser.Next();
                Value2 = ExpressionsLevel1();

                if (typeof(PlusToken).Equals(Operator))
                {
                    Value1 = Value1 + Value2;
                }
                else if (typeof(MinusToken).Equals(Operator))
                {
                    Value1 = Value1 - Value2;
                }
                else if (typeof(AndToken).Equals(Operator))
                {
                    Value1 = (double)(((int)System.Math.Floor(Value2)) & ((int)System.Math.Floor(Value2)));
                }
                else if (typeof(OrToken).Equals(Operator))
                {
                    Value1 = (double)(((int)System.Math.Floor(Value2)) | ((int)System.Math.Floor(Value2)));
                }

                Operator = BasicIntrepeter.CodeParser.CurrentToken.GetType();
            }

            return Value1;
        }

        /// <summary>
        /// Compare two values (Less than, Greater than, Equal, etc)
        /// </summary>
        /// <returns>1 if comparion is true, 0 if comparison is false</returns>
        public double ExpressionLevel3()
        {
            double Value1, Value2;
            Type Operator;

            if (BasicIntrepeter.StringModifier.isString())
            {
                return BasicIntrepeter.StringModifier.Compare();
            }

            Value1 = ExpressionsLevel2();
            Operator = BasicIntrepeter.CodeParser.CurrentToken.GetType();

            while ( (typeof(LessThenToken).Equals(Operator)) || 
                    (typeof(GreaterThenToken).Equals(Operator)) ||
                    (typeof(LessOrEqualToken).Equals(Operator)) ||
                    (typeof(MoreOrEqualToken).Equals(Operator)) || 
                    (typeof(EqualToken).Equals(Operator)) || 
                    (typeof(NotEqualToken).Equals(Operator))
                  )
            {
                BasicIntrepeter.CodeParser.Next();
                Value2 = ExpressionsLevel2();

                if (typeof(LessThenToken).Equals(Operator))
                {
                    if (Value1 < Value2) Value1 = 1; else Value1 = 0;
                }
                else if (typeof(GreaterThenToken).Equals(Operator))
                {
                    if (Value1 > Value2) Value1 = 1; else Value1 = 0;
                }
                else if (typeof(LessOrEqualToken).Equals(Operator))
                {
                    if (Value1 <= Value2) Value1 = 1; else Value1 = 0;
                }
                else if (typeof(MoreOrEqualToken).Equals(Operator))
                {
                    if (Value1 >= Value2) Value1 = 1; else Value1 = 0;
                }
                else if (typeof(EqualToken).Equals(Operator))
                {
                    if (Value1 == Value2) Value1 = 1; else Value1 = 0;
                }
                else if (typeof(NotEqualToken).Equals(Operator))
                {

                    if (Value1 != Value2) Value1 = 1; else Value1 = 0;
                }

                Operator = BasicIntrepeter.CodeParser.CurrentToken.GetType();
            }

            return Value1;
        }



    }
}
