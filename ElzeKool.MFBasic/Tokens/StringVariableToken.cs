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
using ElzeKool.MFBasic.Utilities;
using ElzeKool.MFBasic.Tokens.Routines;

namespace ElzeKool.MFBasic.Tokens
{
    /// <summary>
    /// Indicates a Variable in the Sourcecode
    /// It is a derivative of LetToken, that allows the user to assign a value without using LET
    /// </summary>
    public sealed class StringVariableToken : LetToken
    {
        public StringVariableToken(CodeParser Parser, String Content) : base(Parser, Content) { }

        /// <summary>
        /// Token Content as Variable number
        /// </summary>
        /// <returns>Variable number</returns>
        public override byte asVariableID(Basic BasicIntrepeter)
        {
            for (byte i = 0; i < BasicIntrepeter.StringVariables.Count; i++)
            {
                if (((Basic.StringBasicVariable)BasicIntrepeter.StringVariables[i]).Name == this.Content)
                {
                    return i;
                }
            }

            return (byte)BasicIntrepeter.StringVariables.Add(new Basic.StringBasicVariable(this.Content,""));
        }


    }
}
