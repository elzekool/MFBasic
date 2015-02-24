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
 * NOTE: This file isn't ready yet!
 * 
 **/

using System;
using System.Collections;
using Microsoft.SPOT;
using ElzeKool.MFBasic.Tokens.Routines;

namespace ElzeKool.MFBasic
{
    /// <summary>
    /// Class used to store current MFBasic state. 
    /// </summary>
    [Serializable]
    public class StateStorage
    {
        /// <summary>
        /// Sourcecode 
        /// </summary>
        public String SourceCode;

        // WHILE-WEND Status
        public int[] WHILEStack;
        public int WHILEStackPosition;

        // GOSUB-RETURN Status
        public int[] GOSUBStack;
        public int GOSUBStackPosition;

        // FOR-NEXT Status
        public ForToken.ForStackItem[] FORStack;
        public int FORStackPosition;

        public bool Ended;
        public bool DontExectuteIF;
        public int ProgramPosition;

        public ArrayList NumericVariables;
        public ArrayList StringVariables;
        
    }
}
