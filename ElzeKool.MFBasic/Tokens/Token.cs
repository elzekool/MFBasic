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

namespace ElzeKool.MFBasic.Tokens
{

    public class Token
    {
        /// <summary>
        /// Parent Parser Object
        /// </summary>
        protected CodeParser Parser;

        /// <summary>
        /// Contents of token
        /// </summary>
        public String Content;

        /// <summary>
        /// Return string value of current token
        /// </summary>
        /// <returns>Current Token value</returns>
        public virtual String asString()
        {
            throw new MFBasic.Exceptions.BasicLanguageException(MFBasic.Exceptions.BasicLanguageException.AS_STRING_NOT_SUPPORTED);
        }

        /// <summary>
        /// Return integer value of current token
        /// </summary>
        /// <returns>Current Token value</returns>
        public virtual double asNumber()
        {
            throw new MFBasic.Exceptions.BasicLanguageException(MFBasic.Exceptions.BasicLanguageException.AS_NUMBER_NOT_SUPPORTED);
        }


        /// <summary>
        /// Token Content as Variable number
        /// </summary>
        /// <returns>Variable number</returns>
        public virtual byte asVariableID(Basic BasicIntrepeter)
        {
            throw new MFBasic.Exceptions.BasicLanguageException(MFBasic.Exceptions.BasicLanguageException.AS_VARIABLEID_NOT_SUPPORTED);
        }

        /// <summary>
        /// Run Statement
        /// </summary>
        /// <param name="BasicIntrepeter">Basic Intrepeter that called the Statement</param>
        public virtual void ExcecuteStatement(Basic BasicIntrepeter)
        {
            throw new MFBasic.Exceptions.BasicLanguageException(MFBasic.Exceptions.BasicLanguageException.EXCECUTESTATEMENT_NOT_SUPPORTED);
        }

        /// <summary>
        /// Get Numeric Result
        /// Used for Numeric functions
        /// </summary>
        /// <param name="BasicIntrepeter">Basic Intrepeter that called the Statement</param>
        /// <returns>Function result</returns>
        public virtual double GetNumericResult(Basic BasicIntrepeter)
        {
            throw new MFBasic.Exceptions.BasicLanguageException(MFBasic.Exceptions.BasicLanguageException.GET_NUMERIC_RESULT_NOT_SUPPORTED);
        }

        /// <summary>
        /// Get String Result
        /// Used for String functions
        /// </summary>
        /// <param name="BasicIntrepeter">Basic Intrepeter that called the Statement</param>
        /// <returns>Function result</returns>
        public virtual String GetStringResult(Basic BasicIntrepeter)
        {
            throw new MFBasic.Exceptions.BasicLanguageException(MFBasic.Exceptions.BasicLanguageException.GET_STRING_RESULT_NOT_SUPPORTED);
        }

        /// <summary>
        /// Instantiate a new Token
        /// </summary>
        /// <param name="Type">Type of Token</param>
        /// <param name="Content">Contents of Token</param>
        public Token(CodeParser Parser, String Content)
        {
            this.Parser = Parser;
            //this.Content = Content;
        }

    }
}
