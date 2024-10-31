using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SimpleLexer
{

    public class LexerException : System.Exception
    {
        public LexerException(string msg)
            : base(msg)
        {
        }

    }

    public enum Tok
    {
        EOF,
        ID,
        INUM,
        COLON,
        SEMICOLON,
        ASSIGN,
        BEGIN,
        END,
        COMMA,
        PLUS,
        MINUS,
        MULT,
        DIVISION,
        MOD,
        DIV,
        AND,
        OR,
        NOT,
        MULTASSIGN,
        DIVASSIGN,
        PLUSASSIGN,
        MINUSASSIGN,
        LT,  //lesser
        GT,  //greater
        LEQ, //less or equal
        GEQ, //greater or equal
        EQ,  //equal
        NEQ, //not equal
        WHILE,
        DO,
        FOR,
        TO,
        IF,
        THEN,
        ELSE,
        ELSEIF,
        LEFT_BRACKET,
        RIGHT_BRACKET,
        ASSERT,
        BREAK,
        CASE,
        PRINT,
        RETURN,
        SWITCH,
        COMMENT,
        LET,
        CONST,
        TRY,
        CATCH,
        CONTINUE
    }

    public class Lexer
    {
        private int position;
        private char currentCh;                      // Текущий символ
        public int LexRow, LexCol;                  // Строка-столбец начала лексемы. Конец лексемы = LexCol+LexText.Length
        private int row, col;                        // текущие строка и столбец в файле
        private TextReader inputReader;
        private Dictionary<string, Tok> keywordsMap; // Словарь, сопоставляющий ключевым словам константы типа TLex. Инициализируется процедурой InitKeywords 
        public Tok LexKind;                         // Тип лексемы
        public string LexText;                      // Текст лексемы
        public int LexValue;                        // Целое значение, связанное с лексемой LexNum

        private string CurrentLineText;  // Накапливает символы текущей строки для сообщений об ошибках


        public Lexer(TextReader input)
        {
            CurrentLineText = "";
            inputReader = input;
            keywordsMap = new Dictionary<string, Tok>();
            InitKeywords();
            row = 1; col = 0;
            NextCh();       // Считать первый символ в ch
            NextLexem();    // Считать первую лексему, заполнив LexText, LexKind и, возможно, LexValue
        }

        public void Init()
        {

        }

        private void PassSpaces()
        {
            while (char.IsWhiteSpace(currentCh))
            {
                NextCh();
            }
        }

        private void InitKeywords()
        {
            keywordsMap["div"] = Tok.DIV;
            keywordsMap["mod"] = Tok.MOD;
            keywordsMap["and"] = Tok.AND;
            keywordsMap["or"] = Tok.OR;
            keywordsMap["not"] = Tok.NOT;
            keywordsMap["assert"] = Tok.ASSERT;
            keywordsMap["if"] = Tok.IF;
            keywordsMap["else"] = Tok.ELSE;
            keywordsMap["break"] = Tok.BREAK;
            keywordsMap["case"] = Tok.CASE;
            keywordsMap["do"] = Tok.DO;
            keywordsMap["print"] = Tok.PRINT;
            keywordsMap["return"] = Tok.RETURN;
            keywordsMap["while"] = Tok.WHILE;
            keywordsMap["for"] = Tok.FOR;
            keywordsMap["continue"] = Tok.CONTINUE;
            keywordsMap["swith"] = Tok.SWITCH;
            keywordsMap["try"] = Tok.TRY;
            keywordsMap["catch"] = Tok.CATCH;
            keywordsMap["let"] = Tok.LET;
            keywordsMap["const"] = Tok.CONST;
        }

        public string FinishCurrentLine()
        {
            return CurrentLineText + inputReader.ReadLine();
        }

        private void LexError(string message)
        {
            System.Text.StringBuilder errorDescription = new System.Text.StringBuilder();
            errorDescription.AppendFormat("Lexical error in line {0}:", row);
            errorDescription.Append("\n");
            errorDescription.Append(FinishCurrentLine());
            errorDescription.Append("\n");
            errorDescription.Append(new String(' ', col - 1) + '^');
            errorDescription.Append('\n');
            if (message != "")
            {
                errorDescription.Append(message);
            }
            throw new LexerException(errorDescription.ToString());
        }

        private void NextCh()
        {
            // В LexText накапливается предыдущий символ и считывается следующий символ
            LexText += currentCh;
            var nextChar = inputReader.Read();
            if (nextChar != -1)
            {
                currentCh = (char)nextChar;
                if (currentCh != '\n')
                {
                    col += 1;
                    CurrentLineText += currentCh;
                }
                else
                {
                    row += 1;
                    col = 0;
                    CurrentLineText = "";
                }
            }
            else
            {
                currentCh = (char)0; // если достигнут конец файла, то возвращается #0
            }
        }

        public void NextLexem()
        {
            PassSpaces();
            // R К этому моменту первый символ лексемы считан в ch
            LexText = "";
            LexRow = row;
            LexCol = col;
            // Тип лексемы определяется по ее первому символу
            // Для каждой лексемы строится синтаксическая диаграмма
            if (currentCh == '#')
            {
                NextCh();
                if (currentCh != '[')
                {
                    FinishCurrentLine();
                    NextCh();
                    NextLexem();
                }

                if (currentCh == '[')
                {
                    NextCh();
                    while (true)
                    {
                        if ((int)currentCh == 0)
                        {
                            throw new LexerException("#[ Is not closed");
                        }

                        if (currentCh == ']')
                        {
                            NextCh();
                            if (currentCh == '#')
                            {
                                NextCh();
                                break;
                            }

                        }
                        NextCh();
                    }
                    NextLexem();
                }

            }

            else if (currentCh == ';')
            {
                NextCh();
                LexKind = Tok.SEMICOLON;
            }

            else if (currentCh == ':')
            {
                NextCh();
                LexKind = Tok.COLON;
            }

            else if (currentCh == '=')
            {
                NextCh();
                if (currentCh == '=')
                {
                    NextCh();
                    LexKind = Tok.EQ;
                }
                else
                {

                    LexKind = Tok.ASSIGN;
                }

            }

            else if (currentCh == '(')
            {
                NextCh();
                LexKind = Tok.LEFT_BRACKET;
            }

            else if (currentCh == ')')
            {
                NextCh();
                LexKind = Tok.RIGHT_BRACKET;
            }
            else if (currentCh == ',')
            {
                NextCh();
                LexKind = Tok.COMMA;
            }

            else if (currentCh == '{')
            {
                NextCh();
                LexKind = Tok.BEGIN;
            }

            else if (currentCh == '}') 
            {
                NextCh();
                LexKind = Tok.END;
            }

            else if (currentCh == '+')
            {
                NextCh();
                LexKind = Tok.PLUS;
            }

            else if (currentCh == '-')
            {
                NextCh();
                LexKind = Tok.MINUS;
            }

            else if (currentCh == '*')
            {
                NextCh();
                LexKind = Tok.MULT;
            }


            else if (currentCh == '/')
            {
                NextCh();
                LexKind = Tok.DIVISION;
            }

            else if (currentCh == '%')
            {
                NextCh();
                LexKind = Tok.MOD;
            }

            else if (currentCh == '|')
            {
                NextCh();
                LexKind = Tok.OR;
            }

            else if (currentCh == '&')
            {
                NextCh();
                LexKind = Tok.AND;
            }

            else if (currentCh == '>')
            {
                NextCh();
                if (currentCh == '=')
                {
                    NextCh();
                    LexKind = Tok.GEQ;
                }
                else
                {

                    LexKind = Tok.GT;
                }

            }

            else if (currentCh == '<')
            {
                NextCh();

                if (currentCh == '=')
                {
                    NextCh();
                    LexKind = Tok.LEQ;
                }
                else
                {

                    LexKind = Tok.LT;
                }

            }

            else if (currentCh == '!')
            {
                NextCh();
                if (currentCh == '=')
                {
                    NextCh();
                    LexKind = Tok.NEQ;
                }
                else
                {

                    LexKind = Tok.NOT;
                }

            }

            else if (char.IsLetter(currentCh))
            {
                while (char.IsLetterOrDigit(currentCh))
                {
                    NextCh();
                }
                if (keywordsMap.ContainsKey(LexText))
                {
                    LexKind = keywordsMap[LexText];
                }

                else
                {
                    LexKind = Tok.ID;
                }
            }
            else if (char.IsDigit(currentCh))
            {
                while (char.IsDigit(currentCh))
                {
                    NextCh();
                }
                LexValue = Int32.Parse(LexText);
                LexKind = Tok.INUM;
            }
            else if ((int)currentCh == 0)
            {
                LexKind = Tok.EOF;
            }
            else
            {
                LexError("Incorrect symbol " + currentCh);
            }
        }

        public virtual void ParseToConsole()
        {
            do
            {
                Console.WriteLine(TokToString(LexKind));
                NextLexem();
            } while (LexKind != Tok.EOF);
        }

        public string TokToString(Tok t)
        {
            var result = t.ToString();
            switch (t)
            {
                case Tok.ID:
                    result += ' ' + LexText;
                    break;
                case Tok.INUM:
                    result += ' ' + LexValue.ToString();
                    break;
            }
            return result;
        }
    }
}