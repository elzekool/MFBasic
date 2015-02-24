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
using ElzeKool.MFBasic.Tokens.Operator;
using ElzeKool.MFBasic.Tokens.SpecialChars;
using ElzeKool.MFBasic.Tokens.Functions;

namespace ElzeKool.MFBasic.Tokens.Functions.Strings
{
    public sealed class ToUpper__Token : StringFunctionToken
    {
        public ToUpper__Token(CodeParser Parser, String Content) : base(Parser, Content) { }

        public override string GetStringResult(Basic BasicIntrepeter)
        {
            String Input = "";
            BasicIntrepeter.CodeParser.Expect(typeof(ToUpper__Token));
            BasicIntrepeter.CodeParser.Expect(typeof(LeftParenToken));

            if (BasicIntrepeter.StringModifier.isString())
                Input = BasicIntrepeter.StringModifier.Value();
            else
                BasicIntrepeter.CodeParser.Expect(typeof(StringToken));

            BasicIntrepeter.CodeParser.Expect(typeof(RightParenToken));

            return Input.ToUpper();

        }
    }
}