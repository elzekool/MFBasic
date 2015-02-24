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
using ElzeKool.MFBasic.Tokens.Functions;
using ElzeKool.MFBasic.Tokens.Operator;
using ElzeKool.MFBasic.Tokens.Routines;
using ElzeKool.MFBasic.Tokens.RoutineHelpers;
using ElzeKool.MFBasic.Tokens.SpecialChars;

using ElzeKool.MFBasic.Utilities;

namespace ElzeKool.MFBasic.Tokens.Routines
{
    public sealed class IfToken : GeneralStatementToken
    {
        public IfToken(CodeParser Parser, String Content) : base(Parser, Content) { }


        /// <summary>
        /// Run Statement
        /// </summary>
        /// <param name="BasicIntrepeter">Basic Intrepeter that called the Statement</param>
        public override void ExcecuteStatement(Basic BasicIntrepeter)
        {
            bool openParenthis = false;
            bool MultiLineIF = false;
            double ComparisonResult;

            BasicIntrepeter.CodeParser.Expect(typeof(IfToken));

            // Alow the usage of parenthis like C language
            if (typeof(LeftParenToken).Equals(BasicIntrepeter.CodeParser.CurrentToken.GetType()))
            {
                BasicIntrepeter.CodeParser.Expect(typeof(LeftParenToken));
                openParenthis = true;
            }

            // Get comparison result
            ComparisonResult = BasicIntrepeter.NumericModifier.ExpressionLevel3();

            // If we had an opening parenthis we expect a closing one
            if (openParenthis)
                BasicIntrepeter.CodeParser.Expect(typeof(RightParenToken));


            // Expect THEN
            Parser.Expect(typeof(ThenToken));


            // Check if multiline 
            if (Parser.CurrentToken.GetType().Equals(typeof(EndOfLineToken)))
            {
                MultiLineIF = true;
                BasicIntrepeter.CodeParser.Next();
            }


            if (MultiLineIF)
            {

                ///////////////////////////////////////////////////////////////////
                //
                //                            MULTILINE IF
                //
                ///////////////////////////////////////////////////////////////////
                if (ComparisonResult >= 1)
                {

                    // 1. Excecute Statements until ENDIF or ELSE
                    // ------------------------------------------------------------
                    while (true)
                    {
                        if (
                            (BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(EndIfToken))) ||
                            (BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(ElseToken))) ||
                            (BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(EndOfInputToken)))
                        )
                            break;

                        // If NewLine skip it. 
                        // So we can be sure that all ELSE and ENDIF tokens are catched!
                        if (BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(EndOfLineToken)))
                        {
                            BasicIntrepeter.CodeParser.Next();
                            continue;
                        }

                        if (!BasicIntrepeter.DontExectuteIF) BasicIntrepeter.ProcessNextStatement();
                    }

                    // 2. If ELSE then Skip until ENDIF
                    // ------------------------------------------------------------
                    if (BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(ElseToken)))
                    {
                        bool BackupDontExectuteIF = BasicIntrepeter.DontExectuteIF;
                        BasicIntrepeter.DontExectuteIF = true;

                        while (
                            (!BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(EndIfToken))) &&
                            (!BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(EndOfInputToken)))
                        )
                        {
                            if (BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(IfToken))) BasicIntrepeter.ProcessNextStatement();
                            BasicIntrepeter.CodeParser.Next();
                        }

                        BasicIntrepeter.DontExectuteIF = BackupDontExectuteIF;
                    }

                    // 3. If ENDOFINPUT then throw error
                    // ------------------------------------------------------------
                    if (BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(EndOfInputToken)))
                    {
                        throw new MFBasic.Exceptions.BasicLanguageException(MFBasic.Exceptions.BasicLanguageException.MULTILINE_IF_WITHOUT_ENDIF);
                    }

                    // Skip EndIf Token
                    BasicIntrepeter.CodeParser.Next();

                }
                else
                {

                    // 1. Skip Tokens until ENDIF or ELSE
                    // ------------------------------------------------------------
                    bool BackupDontExectuteIF = BasicIntrepeter.DontExectuteIF;
                    BasicIntrepeter.DontExectuteIF = true;
                    while (
                        (!BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(EndIfToken))) &
                        (!BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(ElseToken))) &
                        (!BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(EndOfInputToken)))
                    )
                    {
                        if (BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(IfToken))) BasicIntrepeter.ProcessNextStatement();
                        BasicIntrepeter.CodeParser.Next();
                    }
                    BasicIntrepeter.DontExectuteIF = BackupDontExectuteIF;


                    // 2. If ENDOFINPUT then throw error
                    // ------------------------------------------------------------
                    if (BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(EndOfInputToken)))
                    {
                        throw new MFBasic.Exceptions.BasicLanguageException(MFBasic.Exceptions.BasicLanguageException.MULTILINE_IF_WITHOUT_ENDIF);
                    }


                    // 3. If ELSE then Execute until ENDIF
                    // ------------------------------------------------------------
                    if (BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(ElseToken)))
                    {
                        // Skip ELSE
                        BasicIntrepeter.CodeParser.Next();

                        while (
                            (!BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(EndIfToken))) &&
                            (!BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(EndOfInputToken)))
                        )
                        {

                            // If NewLine skip it. 
                            // So we can be sure that all ENDIF tokens are catched!
                            if (BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(EndOfLineToken)))
                            {
                                BasicIntrepeter.CodeParser.Next();
                                continue;
                            }

                            if (!BasicIntrepeter.DontExectuteIF) BasicIntrepeter.ProcessNextStatement();

                        }
                    }

                    // 4. If ENDOFINPUT then throw error
                    // ------------------------------------------------------------
                    if (BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(EndOfInputToken)))
                    {
                        throw new MFBasic.Exceptions.BasicLanguageException(MFBasic.Exceptions.BasicLanguageException.MULTILINE_IF_WITHOUT_ENDIF);
                    }

                    // 5. Skip ENDIF
                    // ------------------------------------------------------------
                    BasicIntrepeter.CodeParser.Expect(typeof(EndIfToken));

                }
            }
            else
            {
                ///////////////////////////////////////////////////////////////////
                //
                //                          SINGLE LINE IF
                //
                ///////////////////////////////////////////////////////////////////

                // If outcome is true 
                if (ComparisonResult >= 1)
                {
                    // 1. Process statement
                    // ------------------------------------------------------------
                    if (!BasicIntrepeter.DontExectuteIF) BasicIntrepeter.ProcessNextStatement();

                    // 2. If ELSE Skip until NEWLINE/ENDOFINPUT
                    // ------------------------------------------------------------
                    while ((!BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(EndOfLineToken))) &&
                           (!BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(EndOfInputToken)))
                    )
                    {
                        if (BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(IfToken))) BasicIntrepeter.ProcessNextStatement();
                        BasicIntrepeter.CodeParser.Next();
                    }

                    // 3. If EndIfToken and Dont Execute is true go back one program position
                    if ((BasicIntrepeter.DontExectuteIF) && (BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(EndIfToken))))
                        BasicIntrepeter.CodeParser.ProgramPosition--;

                }
                else
                {
                    // 1. Skip until ELSE/NEWLINE/ENDOFINPUT
                    // ------------------------------------------------------------
                    bool BackupDontExectuteIF = BasicIntrepeter.DontExectuteIF;
                    BasicIntrepeter.DontExectuteIF = true;
                    while ((!BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(EndOfLineToken))) &&
                           (!BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(ElseToken))) &&
                           (!BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(EndOfInputToken)))
                    )
                    {
                        if (BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(IfToken))) BasicIntrepeter.ProcessNextStatement();
                        BasicIntrepeter.CodeParser.Next();
                    }
                    BasicIntrepeter.DontExectuteIF = BackupDontExectuteIF;

                    // 2. If ELSE excecute statement
                    // ------------------------------------------------------------
                    if ((BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(ElseToken))))
                    {
                        // Skip Next
                        BasicIntrepeter.CodeParser.Next();

                        // Execute Statement
                        // Don't execute if searching for ELSE/ENDIF
                        if (!BasicIntrepeter.DontExectuteIF) BasicIntrepeter.ProcessNextStatement();

                    }

                    // 3. If EndIfToken and Dont Execute is true go back one program position
                    if ((BasicIntrepeter.DontExectuteIF) && (BasicIntrepeter.CodeParser.CurrentToken.GetType().Equals(typeof(EndIfToken))))
                        BasicIntrepeter.CodeParser.ProgramPosition--;

                }
            }
        }

    }
}
