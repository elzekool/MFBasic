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
using System.Collections;

using ElzeKool.MFBasic.Utilities;
using ElzeKool.MFBasic.Exceptions;

using ElzeKool.MFBasic.Tokens;
using ElzeKool.MFBasic.Tokens.Operator;
using ElzeKool.MFBasic.Tokens.SpecialChars;
using ElzeKool.MFBasic.Tokens.Routines;
using ElzeKool.MFBasic.Tokens.RoutineHelpers;
using ElzeKool.MFBasic.Tokens.Functions;
using ElzeKool.MFBasic.Tokens.Functions.Numeric;
using ElzeKool.MFBasic.Tokens.Functions.Strings;

namespace ElzeKool.MFBasic
{

    public sealed class CodeParser
    {

        #region Constants

        /// <summary>
        /// Maximum length of variable name
        /// </summary>
        private const int MAX_VARIABLENAME_LENGTH = 10;

        /// <summary>
        /// Maximum number length
        /// </summary>
        private const int MAX_NUM_LENGTH = 20;

        /// <summary>
        /// Used to check for number
        /// </summary>
        private const string CHECK_FOR_NUMBER = "0123456789.";

        /// <summary>
        /// Used to check for end of token name
        /// </summary>
        private const string CHECK_END_OF_TOKEN = "()\t\n :";

        /// <summary>
        /// Used to check for end of variable name
        /// </summary>
        private const string CHECK_END_OF_VARIABLE_NAME = "()\t\n =<>&|+-*/^:,!";

        #endregion

        #region Public classes and enums


        /// <summary>
        /// Used for finding keywords
        /// The parser searches for the keyword field, if found it runs TokenConstructor 
        /// </summary>
        public class KeywordTokenConstructor
        {
            /// <summary>
            /// Keyword string
            /// </summary>
            public readonly String Keyword;

            /// <summary>
            /// Token Constructor
            /// </summary>
            public readonly TokenConstructorDelegate TokenConstructor;

            /// <summary>
            /// Keyword TokenType
            /// </summary>
            public delegate Token TokenConstructorDelegate(CodeParser Parser, String Contents);

            /// <summary>
            /// Instantiate a new KeywordToken
            /// </summary>
            /// <param name="Keyword">String for keyword</param>
            /// <param name="TokenConstructor">Delegate that returns a new Token, It must implement ExcecuteStatement() </param>
            public KeywordTokenConstructor(String Keyword, TokenConstructorDelegate TokenConstructor)
            {
                if (TokenConstructor == null)
                    throw new NullReferenceException("TokenConstructor can't be NULL");

                this.Keyword = Keyword.ToLower();
                this.TokenConstructor = TokenConstructor;
            }

        };

        #endregion

        #region Private properties and functions

        /// <summary>
        /// Indicate if we expect a line number (At begin of program or after CR)
        /// </summary>
        private bool ExpectLineNumber = true;

        /// <summary>
        /// Indicates current position in source
        /// </summary>
        private int SourceCodePosition = 0;

        /// <summary>
        /// Indicates next position in source
        /// </summary>
        private int SourceCodeNextPosition = 0;

        /// <summary>
        /// Current program that is parsed
        /// </summary>
        private readonly String SourceCode;

        /// <summary>
        /// Lowercase version of program sourcecode
        /// </summary>
        private readonly String SourceCodeLower;

        /// <summary>
        /// Keyword token list
        /// </summary>
        private readonly ArrayList KeywordTokens;

        /// <summary>
        /// Found Variable Names
        /// </summary>
        private ArrayList VariableNames;

        #endregion

        #region Public properties 

        /// <summary>
        /// Current Token
        /// </summary>
        public Token CurrentToken = new Token(null,"");

        /// <summary>
        /// Parsed version of Program
        /// </summary>
        public readonly Token[] Program;

        /// <summary>
        /// Current Program Position
        /// </summary>
        public int ProgramPosition;

        #endregion


        /// <summary>
        /// Instantiate a new BASIC Parser
        /// </summary>
        /// <param name="Program">Program to parse</param>
        public CodeParser(String SourceCode) : this(SourceCode, null) {}

        /// <summary>
        /// Instantiate a new BASIC Parser
        /// </summary>
        /// <param name="Program">Program to parse</param>
        /// <param name="CustomTokens">Custom Tokens</param>
        public CodeParser(String SourceCode, KeywordTokenConstructor[] CustomTokens)
        {
            // Found variable names
            VariableNames = new ArrayList();

            // Keyword tokens to search for
            KeywordTokens = new ArrayList();

            // Let, Print
            KeywordTokens.Add(new KeywordTokenConstructor("let", delegate(CodeParser Parser, String Contents) { return new LetToken(Parser, Contents); } ));
            KeywordTokens.Add(new KeywordTokenConstructor("print", delegate(CodeParser Parser, String Contents) { return new PrintToken(Parser, Contents); }));

            // If-Then-Else
            KeywordTokens.Add(new KeywordTokenConstructor("if", delegate(CodeParser Parser, String Contents) { return new IfToken(Parser, Contents); }));
            KeywordTokens.Add(new KeywordTokenConstructor("then", delegate(CodeParser Parser, String Contents) { return new ThenToken(Parser, Contents); }));
            KeywordTokens.Add(new KeywordTokenConstructor("else", delegate(CodeParser Parser, String Contents) { return new ElseToken(Parser, Contents); }));
            KeywordTokens.Add(new KeywordTokenConstructor("endif", delegate(CodeParser Parser, String Contents) { return new EndIfToken(Parser, Contents); }));

            // While-Wend
            KeywordTokens.Add(new KeywordTokenConstructor("while", delegate(CodeParser Parser, String Contents) { return new WhileToken(Parser, Contents); }));
            KeywordTokens.Add(new KeywordTokenConstructor("wend", delegate(CodeParser Parser, String Contents) { return new WendToken(Parser, Contents); }));

            // For-Next
            KeywordTokens.Add(new KeywordTokenConstructor("for", delegate(CodeParser Parser, String Contents) { return new ForToken(Parser, Contents); }));
            KeywordTokens.Add(new KeywordTokenConstructor("to", delegate(CodeParser Parser, String Contents) { return new ToToken(Parser, Contents); }));
            KeywordTokens.Add(new KeywordTokenConstructor("step", delegate(CodeParser Parser, String Contents) { return new StepToken(Parser, Contents); }));
            KeywordTokens.Add(new KeywordTokenConstructor("next", delegate(CodeParser Parser, String Contents) { return new NextToken(Parser, Contents); }));
            
            // Goto, Gosub, Return
            KeywordTokens.Add(new KeywordTokenConstructor("goto", delegate(CodeParser Parser, String Contents) { return new GotoToken(Parser, Contents); }));
            KeywordTokens.Add(new KeywordTokenConstructor("gosub", delegate(CodeParser Parser, String Contents) { return new GosubToken(Parser, Contents); }));
            KeywordTokens.Add(new KeywordTokenConstructor("return", delegate(CodeParser Parser, String Contents) { return new ReturnToken(Parser, Contents); }));

            // End, Rem
            KeywordTokens.Add(new KeywordTokenConstructor("end", delegate(CodeParser Parser, String Contents) { return new EndToken(Parser, Contents); }));
            KeywordTokens.Add(new KeywordTokenConstructor("rem", delegate(CodeParser Parser, String Contents) { return new RemToken(Parser, Contents); }));

            // Sleep
            KeywordTokens.Add(new KeywordTokenConstructor("sleep", delegate(CodeParser Parser, String Contents) { return new SleepToken(Parser, Contents); }));

            // Numeric functions
            KeywordTokens.Add(new KeywordTokenConstructor("int", delegate(CodeParser Parser, String Contents) { return new IntToken(Parser, Contents); }));
            KeywordTokens.Add(new KeywordTokenConstructor("cint", delegate(CodeParser Parser, String Contents) { return new CIntToken(Parser, Contents); }));
            KeywordTokens.Add(new KeywordTokenConstructor("rnd", delegate(CodeParser Parser, String Contents) { return new RndToken(Parser, Contents); }));
            KeywordTokens.Add(new KeywordTokenConstructor("randomize", delegate(CodeParser Parser, String Contents) { return new RandomizeToken(Parser, Contents); }));
            KeywordTokens.Add(new KeywordTokenConstructor("timer", delegate(CodeParser Parser, String Contents) { return new TimerToken(Parser, Contents); }));
            KeywordTokens.Add(new KeywordTokenConstructor("now", delegate(CodeParser Parser, String Contents) { return new NowToken(Parser, Contents); }));
            KeywordTokens.Add(new KeywordTokenConstructor("strpos", delegate(CodeParser Parser, String Contents) { return new StrPosToken(Parser, Contents); }));

            KeywordTokens.Add(new KeywordTokenConstructor("cos", delegate(CodeParser Parser, String Contents) { return new CosToken(Parser, Contents); }));
            KeywordTokens.Add(new KeywordTokenConstructor("sin", delegate(CodeParser Parser, String Contents) { return new SinToken(Parser, Contents); }));
            KeywordTokens.Add(new KeywordTokenConstructor("tan", delegate(CodeParser Parser, String Contents) { return new TanToken(Parser, Contents); }));

            KeywordTokens.Add(new KeywordTokenConstructor("acos", delegate(CodeParser Parser, String Contents) { return new ACosToken(Parser, Contents); }));
            KeywordTokens.Add(new KeywordTokenConstructor("asin", delegate(CodeParser Parser, String Contents) { return new ASinToken(Parser, Contents); }));
            KeywordTokens.Add(new KeywordTokenConstructor("atan", delegate(CodeParser Parser, String Contents) { return new ATanToken(Parser, Contents); }));
            KeywordTokens.Add(new KeywordTokenConstructor("atan2", delegate(CodeParser Parser, String Contents) { return new ATan2Token(Parser, Contents); }));

            KeywordTokens.Add(new KeywordTokenConstructor("abs", delegate(CodeParser Parser, String Contents) { return new ATan2Token(Parser, Contents); }));
            KeywordTokens.Add(new KeywordTokenConstructor("pow", delegate(CodeParser Parser, String Contents) { return new ATan2Token(Parser, Contents); }));

            KeywordTokens.Add(new KeywordTokenConstructor("pi", delegate(CodeParser Parser, String Contents) { return new PIToken(Parser, Contents); }));


            // String Functions 
            KeywordTokens.Add(new KeywordTokenConstructor("chr$", delegate(CodeParser Parser, String Contents) { return new Chr__Token(Parser, Contents); }));
            KeywordTokens.Add(new KeywordTokenConstructor("toupper$", delegate(CodeParser Parser, String Contents) { return new ToUpper__Token(Parser, Contents); }));
            KeywordTokens.Add(new KeywordTokenConstructor("tolower$", delegate(CodeParser Parser, String Contents) { return new ToLower__Token(Parser, Contents); }));
            KeywordTokens.Add(new KeywordTokenConstructor("formatdatetime$", delegate(CodeParser Parser, String Contents) { return new FormatDateTime__Token(Parser, Contents); }));
            KeywordTokens.Add(new KeywordTokenConstructor("formatnumber$", delegate(CodeParser Parser, String Contents) { return new FormatNumber__Token(Parser, Contents); }));


            // Add custom tokens to token array
            if (CustomTokens != null)
            {
                foreach (KeywordTokenConstructor T in CustomTokens)
                {
                    KeywordTokens.Add(T);
                }
            }

            // Store Program
            this.SourceCode = SourceCode;
            this.SourceCodeLower = SourceCode.ToLower();

            // Tokenize the current program, increases speed
            ArrayList GetTokenizedProgram = new ArrayList();

            // Keep parsing until end of program
            while (true)
            {
                // Increase sourcecode position
                SourceCodePosition = SourceCodeNextPosition;

                // Check if we've reached the end of the program
                if (((SourceCodePosition >= SourceCode.Length) || (SourceCode[SourceCodePosition] == (char)0) || (typeof(EndOfInputToken).Equals(CurrentToken.GetType()))))
                    break;

                // Skip Whitespace
                while (((SourceCode[SourceCodePosition] == ' ') || (SourceCode[SourceCodePosition] == '\t')))
                {
                    SourceCodePosition++;

                    // Stop at the end of the program
                    if (SourceCodePosition == SourceCode.Length)
                    {
                        CurrentToken = new EndOfInputToken(this, "");
                        GetTokenizedProgram.Add(CurrentToken);
                        break;
                    }
                }

                // Fetch and Store Token 
                CurrentToken = FindNextToken(ref GetTokenizedProgram);
                GetTokenizedProgram.Add(CurrentToken);
            }

            // Store Tokenized Program
            GetTokenizedProgram.Add(new EndOfInputToken(this,""));
            Program = (Token[]) GetTokenizedProgram.ToArray(typeof(Token));
            Reset();
        }

        /// <summary>
        /// Reset Paser to inital state
        /// </summary>
        public void Reset()
        {
            this.CurrentToken = new Token(this, "");
            this.ProgramPosition = 0;
            Next();
        }

        /// <summary>
        /// Indicate end of program
        /// </summary>
        /// <returns>True if end of program is found</returns>
        public bool Finished()
        {
            return (ProgramPosition >= Program.Length);
        }

        /// <summary>
        /// Check if the current Token type is what we expect and proceed to next statement
        /// </summary>
        /// <param name="Token">Expected token</param>
        public void Expect(Type Token)
        {
            // Check if Token is what we expect
            if (Token != CurrentToken.GetType())
            {
                throw new BasicLanguageException("Unexpected: " + CurrentToken.GetType().FullName + " ,Expected: " + Token.FullName);
            }

            // Get next Token
            Next();
        }


        /// <summary>
        /// Go to next Token without checking the type
        /// </summary>
        public void Next()
        {
            if (Program != null)
            {
                // If Fininshed don't search
                if (Finished())
                    return;

                CurrentToken = Program[ProgramPosition];
                ProgramPosition++;

                return;
            }
        }


        /// <summary>
        /// Find next Token in Program
        /// </summary>
        /// <returns>Found Token Type</returns>
        private Token FindNextToken(ref ArrayList ProgramToTokenize)
        {
            /* Workings of method
             * 1. Search for End of Program
             * 2. Search for Numbers
             * 3. Search for Special characters
             * 4. Search for Quoted Strings
             * 5. Search for Keywords
             * 6. Search for Known variables
             * 7. Search for New variables
             */ 

            ///////////////////////////////////////////////////////////////////////////
            //
            //                      Search for End of Program
            //
            ///////////////////////////////////////////////////////////////////////////


            // Check for End of program
            if ((SourceCode[SourceCodePosition] == (char)0) || (SourceCodePosition > SourceCode.Length))
                return new EndOfInputToken(this, "");


            ///////////////////////////////////////////////////////////////////////////
            //
            //                      Search for Numbers
            //
            ///////////////////////////////////////////////////////////////////////////

            // Token is a Number
            if (CHECK_FOR_NUMBER.IndexOf(SourceCode[SourceCodePosition]) != -1)
            {
                // Retrieve number
                for (int i = 0; i < (MAX_NUM_LENGTH + 1); ++i)
                {
                    // Check if next char is numeric
                    if (CHECK_FOR_NUMBER.IndexOf(SourceCode[SourceCodePosition + i]) == -1)
                    {
                        // Check number length
                        if (i > 0)
                        {
                            // Incease Program Search Position
                            SourceCodeNextPosition = SourceCodePosition + i;

                            // If we expect a Line number return LineNumberToken else return NumberToken
                            if (ExpectLineNumber)
                            {
                                ExpectLineNumber = false;
                                return new LineNumberToken(this, SourceCode.Substring(SourceCodePosition, SourceCodeNextPosition - SourceCodePosition));
                            }
                            else
                            {
                                ExpectLineNumber = false;
                                return new NumericToken(this, SourceCode.Substring(SourceCodePosition, SourceCodeNextPosition - SourceCodePosition));
                            }
                        }
                        else
                        {
                            throw new BasicParserException(BasicParserException.UNKNOWN_ERROR);
                        }
                    }

                }
                throw new BasicParserException(BasicParserException.NUMBER_TO_LONG);
            }

            // Reset ExectLineNumber
            ExpectLineNumber = false;



            ///////////////////////////////////////////////////////////////////////////
            //
            //                      Search for Special characters
            //
            ///////////////////////////////////////////////////////////////////////////



            // Check for Single character tokens
            switch (SourceCode[SourceCodePosition])
            {

                // NewLine Character
                case '\n':

                    // Increase Program Search Position
                    SourceCodeNextPosition = SourceCodePosition + 1;

                    // After new line expect line number
                    ExpectLineNumber = true;

                    // A Single Text Token on a Line is a LabelToken
                    if (ProgramToTokenize.Count >= 2)
                    {
                        if (ProgramToTokenize[ProgramToTokenize.Count - 1].GetType().Equals(typeof(TextToken)) && ProgramToTokenize[ProgramToTokenize.Count - 2].GetType().Equals(typeof(EndOfLineToken)))
                            ProgramToTokenize[ProgramToTokenize.Count - 1] = new LabelToken(this, ((TextToken)ProgramToTokenize[ProgramToTokenize.Count - 1]).Content);
                    }
                    else if (ProgramToTokenize.Count == 1)
                    {
                        if (ProgramToTokenize[ProgramToTokenize.Count - 1].GetType().Equals(typeof(TextToken)))
                            ProgramToTokenize[ProgramToTokenize.Count - 1] = new LabelToken(this, ((TextToken)ProgramToTokenize[ProgramToTokenize.Count - 1]).Content);
                    }

                    // Return EndOfLine Token
                    return new EndOfLineToken(this, SourceCode.Substring(SourceCodePosition, SourceCodeNextPosition - SourceCodePosition));

                // NewLine within CodeLine
                case ':':

                    // Increase Program Search Position
                    SourceCodeNextPosition = SourceCodePosition + 1;

                    // After new line expect line number
                    ExpectLineNumber = true;

                    // A Single Text Token on a Line is a LabelToken
                    if (ProgramToTokenize.Count >= 2)
                    {
                        if (ProgramToTokenize[ProgramToTokenize.Count - 1].GetType().Equals(typeof(TextToken)) && ProgramToTokenize[ProgramToTokenize.Count - 2].GetType().Equals(typeof(EndOfLineToken)))
                            ProgramToTokenize[ProgramToTokenize.Count - 1] = new LabelToken(this, ((TextToken)ProgramToTokenize[ProgramToTokenize.Count - 1]).Content);
                    }
                    else if (ProgramToTokenize.Count == 1)
                    {
                        if (ProgramToTokenize[ProgramToTokenize.Count - 1].GetType().Equals(typeof(TextToken)))
                            ProgramToTokenize[ProgramToTokenize.Count - 1] = new LabelToken(this, ((TextToken)ProgramToTokenize[ProgramToTokenize.Count - 1]).Content);
                    }

                    // Return EndOfLine Token
                    return new EndOfLineToken(this, SourceCode.Substring(SourceCodePosition, SourceCodeNextPosition - SourceCodePosition));


                // Comma
                case ',':
                    SourceCodeNextPosition = SourceCodePosition + 1;
                    return new CommaToken(this, SourceCode.Substring(SourceCodePosition, SourceCodeNextPosition - SourceCodePosition));

                // Semicolon
                case ';':
                    SourceCodeNextPosition = SourceCodePosition + 1;
                    return new SemicolonToken(this, SourceCode.Substring(SourceCodePosition, SourceCodeNextPosition - SourceCodePosition));

                // Plus
                case '+':
                    SourceCodeNextPosition = SourceCodePosition + 1;
                    return new PlusToken(this, SourceCode.Substring(SourceCodePosition, SourceCodeNextPosition - SourceCodePosition));

                // Minus
                case '-':
                    SourceCodeNextPosition = SourceCodePosition + 1;
                    return new MinusToken(this, SourceCode.Substring(SourceCodePosition, SourceCodeNextPosition - SourceCodePosition));

                // Logical And
                case '&':
                    SourceCodeNextPosition = SourceCodePosition + 1;
                    return new AndToken(this, SourceCode.Substring(SourceCodePosition, SourceCodeNextPosition - SourceCodePosition));

                // Logical Or
                case '|':
                    SourceCodeNextPosition = SourceCodePosition + 1;
                    return new OrToken(this, SourceCode.Substring(SourceCodePosition, SourceCodeNextPosition - SourceCodePosition));

                // Multiply
                case '*':
                    SourceCodeNextPosition = SourceCodePosition + 1;
                    return new MultiplyToken(this, SourceCode.Substring(SourceCodePosition, SourceCodeNextPosition - SourceCodePosition));

                // Divide
                case '/':
                    SourceCodeNextPosition = SourceCodePosition + 1;
                    return new DivideToken(this, SourceCode.Substring(SourceCodePosition, SourceCodeNextPosition - SourceCodePosition));

                // Modulus
                case '%':
                    SourceCodeNextPosition = SourceCodePosition + 1;
                    return new ModulusToken(this, SourceCode.Substring(SourceCodePosition, SourceCodeNextPosition - SourceCodePosition));

                // Left Parenthis
                case '(':
                    SourceCodeNextPosition = SourceCodePosition + 1;
                    return new LeftParenToken(this, SourceCode.Substring(SourceCodePosition, SourceCodeNextPosition - SourceCodePosition));

                // Right Parenthis
                case ')':
                    SourceCodeNextPosition = SourceCodePosition + 1;
                    return new RightParenToken(this, SourceCode.Substring(SourceCodePosition, SourceCodeNextPosition - SourceCodePosition));

                // Less Than
                case '<':
                    SourceCodeNextPosition = SourceCodePosition + 1;

                    
                    if (SourceCodeNextPosition < SourceCode.Length)
                    {
                        // <> becomes NotEqual
                        if (SourceCode[SourceCodeNextPosition] == '>')
                        {
                            SourceCodeNextPosition = SourceCodeNextPosition + 1;
                            return new NotEqualToken(this, SourceCode.Substring(SourceCodePosition, SourceCodeNextPosition - SourceCodePosition));
                        }

                        // <= becomes LessOrEqual
                        if (SourceCode[SourceCodeNextPosition] == '=')
                        {
                            SourceCodeNextPosition = SourceCodeNextPosition + 1;
                            return new LessOrEqualToken(this, SourceCode.Substring(SourceCodePosition, SourceCodeNextPosition - SourceCodePosition));
                        }

                    }

                    // Return Less Than
                    return new LessThenToken(this, SourceCode.Substring(SourceCodePosition, SourceCodeNextPosition - SourceCodePosition));

                // In combination with = becomes NotEqual Token
                case '!':
                    SourceCodeNextPosition = SourceCodePosition + 1;
                    if (SourceCodeNextPosition < SourceCode.Length)
                    {
                        // Check if next Token is EqualToken
                        if (SourceCode[SourceCodeNextPosition] == '=')
                        {
                            SourceCodeNextPosition = SourceCodeNextPosition + 1;
                            return new NotEqualToken(this, SourceCode.Substring(SourceCodePosition, SourceCodeNextPosition - SourceCodePosition));
                        }
                    }

                    // If not != then ignore
                    break;

                // Greater Than
                case '>':
                    SourceCodeNextPosition = SourceCodePosition + 1;

                    if (SourceCodeNextPosition < SourceCode.Length)
                    {
                        // >= becomes MoreOrEqual
                        if (SourceCode[SourceCodeNextPosition] == '=')
                        {
                            SourceCodeNextPosition = SourceCodeNextPosition + 1;
                            return new MoreOrEqualToken(this, SourceCode.Substring(SourceCodePosition, SourceCodeNextPosition - SourceCodePosition));
                        }
                    }

                    // return GreaterThen
                    return new GreaterThenToken(this, SourceCode.Substring(SourceCodePosition, SourceCodeNextPosition - SourceCodePosition));

                // Equals
                case '=':
                    SourceCodeNextPosition = SourceCodePosition + 1;

                    // If previous token is an Text token make it a variables
                    // (Equal becomes assignment character)
                    if (ProgramToTokenize[ProgramToTokenize.Count - 1].GetType().Equals(typeof(TextToken)))
                    {
                        String VariableName = ((TextToken)ProgramToTokenize[ProgramToTokenize.Count - 1]).Content;
                        VariableNames.Add(VariableName);

                        // If $ then it is an String variable else Numeric
                        if (VariableName[VariableName.Length - 1] == '$')
                            ProgramToTokenize[ProgramToTokenize.Count - 1] = new StringVariableToken(this, VariableName);
                        else
                            ProgramToTokenize[ProgramToTokenize.Count - 1] = new NumericVariableToken(this, VariableName);
                    }

                    // Return Equals
                    return new EqualToken(this, SourceCode.Substring(SourceCodePosition, SourceCodeNextPosition - SourceCodePosition));

                // REM
                case '\'':
                    SourceCodeNextPosition = SourceCodePosition + 1;

                    // Skip source until EndOfLine or EndOfProgram
                    while (SourceCodeNextPosition < SourceCode.Length)
                    {
                        if (SourceCode[SourceCodeNextPosition] == '\n')
                            break;
                        SourceCodeNextPosition++;
                    }
                    return new RemToken(this, SourceCode.Substring(SourceCodePosition, SourceCodeNextPosition - SourceCodePosition));


            }

            ///////////////////////////////////////////////////////////////////////////
            //
            //                      Search for Quoted Strings
            //
            ///////////////////////////////////////////////////////////////////////////


            // Check for quote
            if (SourceCode[SourceCodePosition] == '\"')
            {
                SourceCodeNextPosition = SourceCodePosition;

                // Use extra string to check for backslashes
                String QuotedString = "";
                bool IgnoreNextQuote = false;

                // Search for end of string
                do
                {
                    // Check for backslash, Don't ignore a second backslash
                    if ((!IgnoreNextQuote) && (SourceCode[SourceCodeNextPosition] == '\\'))
                    {
                        // Backslash found, ignore next quote as string end
                        // Don't add to string
                        IgnoreNextQuote = true;
                    }
                    else
                    {
                        // Add to string
                        QuotedString += SourceCode[SourceCodeNextPosition];

                        // Reset quote search
                        IgnoreNextQuote = false;
                    }

                    SourceCodeNextPosition++;

                } while ((IgnoreNextQuote) || (SourceCode[SourceCodeNextPosition] != '\"'));

                SourceCodeNextPosition++;

                // Return String
                return new StringToken(this, QuotedString + "\"");
            }


            ///////////////////////////////////////////////////////////////////////////
            //
            //                      Search for Keywords
            //
            ///////////////////////////////////////////////////////////////////////////


            // Check for keywords
            foreach (KeywordTokenConstructor kt in KeywordTokens)
            {
                // Check if keyword is not longer then remaining program code
                if (kt.Keyword.Length <= (SourceCode.Length - SourceCodePosition))
                {
                    // Check for token
                    if (SourceCodeLower.Substring(SourceCodePosition, kt.Keyword.Length) == kt.Keyword)
                    {
                        // Check if it's the whole keyword that we've found
                        if ((SourceCode.Length - SourceCodePosition - kt.Keyword.Length) != 0)
                        {
                            if (CHECK_END_OF_TOKEN.IndexOf(SourceCode[SourceCodePosition + kt.Keyword.Length]) == -1)
                                continue;
                        }

                        // Return keyword if found
                        SourceCodeNextPosition = SourceCodePosition + kt.Keyword.Length;

                        if (kt.Keyword == "rem")
                        {
                            SourceCodeNextPosition = SourceCodePosition + 1;
                            while (SourceCodeNextPosition < SourceCode.Length)
                            {
                                if (SourceCode[SourceCodeNextPosition] == '\n')
                                    break;
                                SourceCodeNextPosition++;
                            }
                        }

                        return kt.TokenConstructor(this, SourceCode.Substring(SourceCodePosition, SourceCodeNextPosition - SourceCodePosition));
                    }

                }
            }

            ///////////////////////////////////////////////////////////////////////////
            //
            //                      Search for Known variables
            //
            ///////////////////////////////////////////////////////////////////////////


            // Check for variables
            foreach (String VarName in VariableNames)
            {


                // Check if variable name is not longer then remaining program code
                if (VarName.Length <= (SourceCode.Length - SourceCodePosition))
                {
                    // Check for token
                    if (SourceCodeLower.Substring(SourceCodePosition, VarName.Length) == VarName.ToLower())
                    {
                        // Check if it's the whole keyword that we've found
                        if ((SourceCode.Length - SourceCodePosition - VarName.Length) != 0)
                        {
                            if (CHECK_END_OF_VARIABLE_NAME.IndexOf(SourceCode[SourceCodePosition + VarName.Length]) == -1)
                                continue;
                        }

                        // Return keyword if found
                        SourceCodeNextPosition = SourceCodePosition + VarName.Length;

                        if (VarName[VarName.Length - 1] == '$')
                        {
                            return new StringVariableToken(this, SourceCode.Substring(SourceCodePosition, SourceCodeNextPosition - SourceCodePosition));
                        }
                        else
                        {
                            return new NumericVariableToken(this, SourceCode.Substring(SourceCodePosition, SourceCodeNextPosition - SourceCodePosition));
                        }

                    }
                }
            }

            ///////////////////////////////////////////////////////////////////////////
            //
            //                      Search for New variables
            //
            ///////////////////////////////////////////////////////////////////////////


            // Search for new Variable Names, If preceeded by LET or FOR then make it a new variable
            for (int i = 0; i < (MAX_VARIABLENAME_LENGTH + 1); ++i)
            {
                // Search for End of TextToken
                if ((CHECK_END_OF_VARIABLE_NAME.IndexOf(SourceCode[SourceCodePosition + i])) != -1)
                {
                    if (i > 0)
                    {
                        SourceCodeNextPosition = SourceCodePosition + i;
                        String TxtPart = SourceCode.Substring(SourceCodePosition, SourceCodeNextPosition - SourceCodePosition);
                        if (ProgramToTokenize.Count > 0)
                        {
                            if (ProgramToTokenize[ProgramToTokenize.Count - 1].GetType().Equals(typeof(ForToken)))
                            {
                                String VariableName = SourceCode.Substring(SourceCodePosition, SourceCodeNextPosition - SourceCodePosition);
                                VariableNames.Add(VariableName);

                                if (VariableName[VariableName.Length - 1] == '$')
                                    return new StringVariableToken(this, VariableName);
                                else
                                    return new NumericVariableToken(this, VariableName);
                            }

                            if (ProgramToTokenize[ProgramToTokenize.Count - 1].GetType().Equals(typeof(LetToken)))
                            {
                                String VariableName = SourceCode.Substring(SourceCodePosition, SourceCodeNextPosition - SourceCodePosition);
                                VariableNames.Add(VariableName);

                                if (VariableName[VariableName.Length - 1] == '$')
                                    return new StringVariableToken(this, VariableName);
                                else
                                    return new NumericVariableToken(this, VariableName);
                            }
                        }

                        // No FOR or LET make it a TextToken
                        return new TextToken(this, SourceCode.Substring(SourceCodePosition, SourceCodeNextPosition - SourceCodePosition));
                    }
                    else
                    {
                        throw new BasicParserException(BasicParserException.VARIABLENAME_UNEXPECTED_END);
                    }
                }
            }


            // Failed to find next token
            throw new BasicParserException(BasicParserException.UNKNOWN_ERROR);
        }


    }
}
