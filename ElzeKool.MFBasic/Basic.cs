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

using ElzeKool.MFBasic.Utilities;

using ElzeKool.MFBasic.Tokens;
using ElzeKool.MFBasic.Tokens.Operator;
using ElzeKool.MFBasic.Tokens.SpecialChars;
using ElzeKool.MFBasic.Tokens.Routines;

namespace ElzeKool.MFBasic
{

    /// <summary>
    /// BASIC Intrepreter
    /// </summary>
    public class Basic
    {

        /// <summary>
        /// Numeric Variable
        /// </summary>
        [Serializable]
        public class NumericBasicVariable
        {
            public readonly String Name;
            public double Value;

            public NumericBasicVariable(String Name, double Value)
            {
                this.Name = Name;
                this.Value = Value;
            }
        }

        /// <summary>
        /// Storage for numeric variables
        /// </summary>
        public System.Collections.ArrayList NumericVariables = new System.Collections.ArrayList();
  
        /// <summary>
        /// Numeric Modifier Instance for BASIC Language
        /// </summary>
        public NumericModifiers NumericModifier;



        /// <summary>
        /// String Variable
        /// </summary>
        [Serializable]
        public class StringBasicVariable
        {
            public readonly String Name;
            public String Value;

            public StringBasicVariable(String Name, String Value)
            {
                this.Name = Name;
                this.Value = Value;
            }
        }


        /// <summary>
        /// Storage for string variables
        /// </summary>
        public System.Collections.ArrayList StringVariables = new System.Collections.ArrayList();

        /// <summary>
        /// Numeric Modifier Instance for BASIC Language
        /// </summary>
        public StringModifiers StringModifier;

        #region Variables for WHILE and WEND statements

        /// <summary>
        /// Return line numbers for WHILE/WEND Calls
        /// </summary>
        public int[] WHILEStack = new int[10];

        /// <summary>
        /// Current Stack Depth for WHILE
        /// </summary>
        public int WHILEStackPosition = 0;

        #endregion


        #region Variables for GOSUB and RETURN statements

        /// <summary>
        /// Return line numbers for GOSUB Calls
        /// </summary>
        public int[] GOSUBStack = new int[10];

        /// <summary>
        /// Current Stack Depth for GOSUB Stack
        /// </summary>
        public int GOSUBStackPosition;

        #endregion

        #region Variables for FOR and NEXT statements

        /// <summary>
        /// Stack for FOR Loops
        /// </summary>
        public ForToken.ForStackItem[] FORStack = new ForToken.ForStackItem[5];

        /// <summary>
        /// Current Stack Depth for FOR Stack
        /// </summary>
        public int FORStackPosition;

        #endregion


        /// <summary>
        /// SourceCode for Program
        /// </summary>
        private readonly String SourceCode;

        /// <summary>
        /// Parser for BASIC Language
        /// </summary>
        public CodeParser CodeParser;

        /// <summary>
        /// True when program has ended
        /// </summary>
        public bool Ended = false;

        /// <summary>
        /// Create a new BASIC Interpreter Instance
        /// </summary>
        /// <param name="Program">Program to excecute</param>
        public Basic(String SourceCode) : this(SourceCode, null) { }

        /// <summary>
        /// Used for processing IF statements
        /// It aids the search of ELSE and ENDIF statements
        /// </summary>
        public bool DontExectuteIF = false;

        /// <summary>
        /// Reset Basic to initial state
        /// </summary>
        public void Reset()
        {
            this.GOSUBStackPosition = 0;
            this.FORStackPosition = 0;
            this.WHILEStackPosition = 0;
            this.Ended = false;
            this.DontExectuteIF = false;
            this.CodeParser.Reset();
            this.NumericModifier = new NumericModifiers(this);
            this.StringModifier = new StringModifiers(this);
        }

        /// <summary>
        /// Create a new BASIC Interpreter Instance
        /// </summary>
        /// <param name="Program">Program to excecute</param>
        /// <param name="CustomTokens">Custom Tokens</param>
        public Basic(String SourceCode, CodeParser.KeywordTokenConstructor[] CustomTokens)
        {
            for (byte i = 0; i < 26; i++)
            {
                NumericVariables.Add(new NumericBasicVariable(((char) (((char)i) + 'a')).ToString(), 0));
            }

            // Remove carrage return from sourcecode
            this.SourceCode = "";
            foreach (char c in SourceCode)
            {
                if (c != '\r')
                    this.SourceCode += c;
            }
            
            this.GOSUBStackPosition = 0;
            this.FORStackPosition = 0;
            this.WHILEStackPosition = 0;
            this.Ended = false;
            this.DontExectuteIF = false;
            this.CodeParser = new CodeParser(this.SourceCode, CustomTokens);
            this.NumericModifier = new NumericModifiers(this);
            this.StringModifier = new StringModifiers(this);

        }



        /// <summary>
        /// Move Parser to given line
        /// </summary>
        /// <param name="LineNumber">Line number to seek</param>
        public void JumpToLine(double LineNumber)
        {
            for (int x = 0; x < CodeParser.Program.Length; x++)
            {
                if (typeof(LineNumberToken).Equals(CodeParser.Program[x].GetType()))
                {
                    if (((LineNumberToken)CodeParser.Program[x]).asNumber() == LineNumber)
                    {
                        CodeParser.ProgramPosition = x + 1;
                        CodeParser.Next();
                        return;
                    }
                }
            }
            throw new MFBasic.Exceptions.BasicLanguageException(MFBasic.Exceptions.BasicLanguageException.JUMP_TO_LABEL_FAILED);

        }

        /// <summary>
        /// Move Parser to given Label
        /// </summary>
        /// <param name="Label">Label number to seek</param>
        public void JumpToLabel(String Label)
        {
            for (int x = 0; x < CodeParser.Program.Length; x++)
            {
                if (typeof(LabelToken).Equals(CodeParser.Program[x].GetType()))
                {
                    if (((LabelToken)CodeParser.Program[x]).Content == Label)
                    {
                        CodeParser.ProgramPosition = x + 1;
                        CodeParser.Next();
                        return;
                    }
                }
            }

            throw new MFBasic.Exceptions.BasicLanguageException(MFBasic.Exceptions.BasicLanguageException.JUMP_TO_LABEL_FAILED);

        }

        /// <summary>
        /// Processes next BASIC Statement
        /// </summary>
        public void ProcessNextStatement()
        {
            // Give it some time
            System.Threading.Thread.Sleep(10);

            while (typeof(EndOfLineToken).Equals(CodeParser.CurrentToken.GetType()))
                CodeParser.Next();

            if (CodeParser.CurrentToken.GetType().IsSubclassOf(typeof(GeneralStatementToken)))
                CodeParser.CurrentToken.ExcecuteStatement(this);
            else
                throw new MFBasic.Exceptions.BasicUnkownStatementException("Statement not known (" + CodeParser.CurrentToken.GetType().FullName + ") !");

        }

        /// <summary>
        /// Process Line number
        /// </summary>
        /// <returns></returns>
        private void LineStatement()
        {
            while (typeof(EndOfLineToken).Equals(CodeParser.CurrentToken.GetType()))
                CodeParser.Next();

            if (typeof(LineNumberToken).Equals(CodeParser.CurrentToken.GetType()))
                CodeParser.Expect(typeof(LineNumberToken));

            if (typeof(LabelToken).Equals(CodeParser.CurrentToken.GetType()))
            {
                CodeParser.Expect(typeof(LabelToken));
            }

            ProcessNextStatement();
        }

        /// <summary>
        /// Run next piece of Basic Code
        /// </summary>
        public void Run()
        {
            if (CodeParser.Finished()) return;
            
            LineStatement();
        }

        public StateStorage SaveCurrentState()
        {
            StateStorage currentState = new StateStorage();

            // Store source
            currentState.SourceCode = this.SourceCode;

            // Store general state
            currentState.Ended = this.Ended;
            currentState.DontExectuteIF = this.DontExectuteIF;
            currentState.ProgramPosition = this.CodeParser.ProgramPosition;

            // Store variables
            currentState.NumericVariables = (System.Collections.ArrayList)this.NumericVariables.Clone();
            currentState.StringVariables = (System.Collections.ArrayList)this.StringVariables.Clone();

            // Store WHILE-WEND state
            currentState.WHILEStack = (int[]) this.WHILEStack.Clone();
            currentState.WHILEStackPosition = this.WHILEStackPosition;

            // Store GOSUB-RETURN state
            currentState.GOSUBStack = (int[])this.GOSUBStack.Clone();
            currentState.GOSUBStackPosition = this.GOSUBStackPosition;

            // Store FOR-NEXT state
            currentState.FORStack = (ForToken.ForStackItem[]) this.FORStack.Clone();
            currentState.FORStackPosition = this.FORStackPosition;


            return currentState;
        }

    }
}
