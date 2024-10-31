using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;
using SimpleLexer;

namespace SimpleParser
{
    public class ParserException : System.Exception
    {
        public ParserException(string msg)
            : base(msg)
        {
        }

    }

    public class Parser
    {
        private SimpleLexer.Lexer l;

        public Parser(SimpleLexer.Lexer lexer)
        {
            l = lexer;
        }

        public void Progr()
        {
            StatementList();
        }

        public void Expr()
        {
            if (l.LexKind == Tok.LEFT_BRACKET)
            {
                l.NextLexem();
                Expr();

                if (l.LexKind == Tok.RIGHT_BRACKET)
                {
                    l.NextLexem();
                }
                else
                {
                    SyntaxError("')' expected");
                }
            }

            else if (l.LexKind == Tok.ID || l.LexKind == Tok.INUM)
            {
                l.NextLexem();

                while (l.LexKind == Tok.PLUS || l.LexKind == Tok.MINUS || l.LexKind == Tok.MULT || l.LexKind == Tok.DIVISION)
                {
                    l.NextLexem();
                    Expr();
                }
            }
            else 
            {
                SyntaxError("expression expected");
            }

        }

        public void Assign()
        {
            l.NextLexem();  // пропуск id
            if (l.LexKind == Tok.ASSIGN)
            {
                l.NextLexem();
                Expr();
            }
            else
            {
                SyntaxError("= expected");
            }
            //Expr();
        }

        public void StatementList()
        {
           
            while (l.LexKind != Tok.EOF && l.LexKind != Tok.END)
            {
                
                Statement();
            }
        }

        public void Statement()
        {
            switch (l.LexKind)
            {
                case Tok.BEGIN:
                    {
                        Block();
                        break;
                    }
                case Tok.ID:
                    {
                        Assign();
                        break;
                    }
                case Tok.WHILE:
                    {
                        WhileStatement();
                        break;
                    }
                case Tok.FOR:
                    {
                        ForStatement();
                        break;
                    }
                case Tok.END:
                    {
                        break;
                    }
                case Tok.IF:
                    {
                        ifStatement();
                        break;

                    }
                case Tok.ELSE: 
                    {
                        elseStatement();
                        break;
                    }
                case Tok.BREAK:
                case Tok.CONTINUE:
                    LoopControlStatement();
                    break;
                case Tok.TRY:
                    tryStatement();
                    break;
                case Tok.CATCH:
                    catchStatement();
                    break;
                case Tok.PRINT:
                    printStatement();
                    break;

                default:
                    {
                        SyntaxError("Operator expected");
                        break;
                    }
            }
        }

        public void Block()
        {
            if (l.LexKind == Tok.BEGIN)
            {
                l.NextLexem();    // пропуск begin
                StatementList();
                if (l.LexKind == Tok.END)
                {
                    l.NextLexem();
                }
            }
            else 
            {
                Statement(); // один оператор
            }

        }

        public void WhileStatement()
        {
            l.NextLexem(); //Пропуск 'while'
            if (l.LexKind == Tok.LEFT_BRACKET)
            {
                l.NextLexem(); // Пропуск '('
                Expr();

                if (l.LexKind == Tok.RIGHT_BRACKET)
                {
                    l.NextLexem(); //Пропуск ')'

                    if (l.LexKind == Tok.BEGIN) // Ожидается '{'
                    {
                        Block();
                    }
                    else
                    {
                        Statement();
                    }
                }
                else
                {
                    SyntaxError("')' Not found");
                }
            }
            else
            {
                SyntaxError("'(' Not found");
            }
        }

        public void ifStatement() 
        {

            l.NextLexem(); //Пропуск 'if'
            if (l.LexKind == Tok.LEFT_BRACKET)
            {
                l.NextLexem(); // Пропуск '('

                while (l.LexKind != Tok.RIGHT_BRACKET)
                {

                    if (l.LexKind == Tok.RIGHT_BRACKET)
                    {
                        ifStatement();
                    }
                    else
                    {
                        Expr();
                    }

                }

                if (l.LexKind == Tok.RIGHT_BRACKET)
                {
                    l.NextLexem();
                    if (l.LexKind == Tok.BEGIN)
                    {
                        Block();
                    }
                }
                
                   StatementList();

            }
        }

        public void elseStatement() 
        {
            l.NextLexem(); // Пропуск 'else'
            if (l.LexKind == Tok.BEGIN)
            {
                Block();
            }
            else 
            {
                SyntaxError("begin expected");
            }
          
            StatementList();
        }

        public void ForStatement() 
        {
            l.NextLexem(); // Пропуск 'for'
            if (l.LexKind == Tok.LEFT_BRACKET)
            {
                l.NextLexem(); //Пропуск '('
                Assign(); //разбор инициализации переменной

                if (l.LexKind == Tok.COMMA)
                {
                    l.NextLexem();// Пропуск ','
                    Expr();

                    if (l.LexKind == Tok.COMMA) 
                    {
                        l.NextLexem();
                        Expr();
                    }

                    if (l.LexKind == Tok.RIGHT_BRACKET)
                    {
                        l.NextLexem(); // Пропуск ')'

                        if (l.LexKind == Tok.BEGIN) 
                        {
                            Block();
                        }
                       
                        StatementList();                                                                    
                    }

                    else
                    {
                        SyntaxError("')' Not found");
                    }
                }
                else
                {
                    SyntaxError("',' Not found");
                }
            }
            else 
            {
                SyntaxError("'(' Not found");
            }
        }

        public void LoopControlStatement() 
        {
            if (l.LexKind == Tok.BREAK || l.LexKind == Tok.CONTINUE)
            {
                l.NextLexem();

            }
            else 
            {
                SyntaxError("'break' or 'continue' expected");
            }
        }

        public void printStatement()
        {
            l.NextLexem();  

            if (l.LexKind == Tok.LEFT_BRACKET)
            {
                l.NextLexem(); 

                Expr();  // Разбор аргумента для print

                if (l.LexKind == Tok.RIGHT_BRACKET)
                {
                    l.NextLexem();  
                }
                else
                {
                    SyntaxError("')' expected after print argument");
                }
            }
            else
            {
                SyntaxError("'(' expected after print");
            }
        }

        public void tryStatement() 
        {
            if (l.LexKind == Tok.TRY) 
            {
                l.NextLexem();

                if (l.LexKind == Tok.BEGIN) 
                {
                    Block();
                }
            }
            else
            {
                SyntaxError("try expected");
            }
        }

        public void catchStatement() 
        {
            if (l.LexKind == Tok.CATCH)
            {
                l.NextLexem();
                if (l.LexKind == Tok.LEFT_BRACKET)
                {
                    Expr();
                }
                if (l.LexKind == Tok.RIGHT_BRACKET)
                {
                    l.NextLexem();
                    if (l.LexKind == Tok.BEGIN)
                    {
                        Block();
                    }
                }
            }
            else 
            {
                SyntaxError("catch expected");
            }
        }




        public void SyntaxError(string message)
        {
            var errorMessage = "Syntax error in line " + l.LexRow.ToString() + ":\n";
            errorMessage += l.FinishCurrentLine() + "\n";
            errorMessage += new String(' ', l.LexCol - 1) + "^\n";
            if (message != "")
            {
                errorMessage += message;
            }
            throw new ParserException(errorMessage);
        }

    }

}

