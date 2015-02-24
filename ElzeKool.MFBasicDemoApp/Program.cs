/**
 * MFBasic Basic Interpeter Demo Application 1
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

namespace ElzeKool.MFBasicDemoApp1
{
    /// <summary>
    /// Demo Application to show how to use MFBasic
    /// </summary>
    public class Program
    {
        public static void Main()
        {
            // Load Basic Program from Resources
            String BasicSource = Resources.GetString(Resources.StringResources.basicprogram);

            // Create new Basic Interpeter Instance 
            Basic MFBasic = new Basic(BasicSource);

            // Run MFBasic program until ended
            while (!MFBasic.Ended)
            {
                MFBasic.Run();
            }

            // Endless sleep
            System.Threading.Thread.Sleep(-1);

        }

    }
}
