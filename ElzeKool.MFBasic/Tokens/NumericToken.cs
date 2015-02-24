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
    public sealed class NumericToken : Token
    {
        public NumericToken(CodeParser Parser, String Content) : base(Parser, Content) { this.Content = Content;  }

        private bool ValueCalculated = false;
        private double CalculatedValue;

        /// <summary>
        /// Return value of current token
        /// </summary>
        /// <returns>Current Token value</returns>
        public override double asNumber()
        {
            // Prevent another calculation
            if (ValueCalculated)
                return CalculatedValue;

            double r = 0F;
            double m = 0.1F;
            bool afterDot = false;

            foreach (char c in this.Content)
            {
                // Stop when character is not a number
                if ("0123456789.".IndexOf(c) == -1) break;

                // Check for Dot
                if (c == '.')
                {
                    afterDot = true;
                    continue;
                }

                if (!afterDot)
                {
                    r *= 10F;
                    r += (double)(c - '0');
                }
                else
                {
                    r += ((double)(c - '0')) * m;
                    m /= 10F;
                }
            }

            // Store calculation
            ValueCalculated = true;
            CalculatedValue = r;

            return r;
        }

    }
}
