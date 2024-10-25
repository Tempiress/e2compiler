using System;
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
            Statement();
        }

        public void Expr()
        {
            if (l.LexKind == Tok.ID || l.LexKind == Tok.INUM)
            {
                l.NextLexem();
            }
            else
            {
                SyntaxError("expression expected");
            }
        }

        public void Assign()
        {
            l.NextLexem();  // ������� id
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
            Statement();
            while (l.LexKind == Tok.SEMICOLON)
            {
                l.NextLexem();
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

                default:
                    {
                        SyntaxError("Operator expected");
                        break;
                    }
            }
        }

        public void Block()
        {
            l.NextLexem();    // ������� begin
            StatementList();
            if (l.LexKind == Tok.END)
            {
                l.NextLexem();
            }
            else
            {
                SyntaxError("end expected");
            }

        }

        public void WhileStatement()
        {
            l.NextLexem(); //������� 'while'
            if (l.LexKind == Tok.LEFT_BRACKET)
            {
                l.NextLexem(); // ������� '('
                Expr();

                if (l.LexKind == Tok.RIGHT_BRACKET)
                {
                    l.NextLexem(); //������� ')'

                    if (l.LexKind == Tok.BEGIN) // ��������� '{'
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

            l.NextLexem(); //������� 'if'
            if (l.LexKind == Tok.LEFT_BRACKET) 
            {
                l.NextLexem(); // ������� '('
                Expr();

                if (l.LexKind == Tok.RIGHT_BRACKET) 
                {
                    l.NextLexem();
                    if (l.LexKind == Tok.BEGIN)
                    {
                        Block();
                    }
                    else
                    {
                        SyntaxError("begin expected");
                    }

                    if (l.LexKind != Tok.EOF)
                        Statement();                    
                }
                if (l.LexKind == Tok.LEFT_BRACKET) 
                {
                    Block();
                }
            }
        }

        public void elseStatement() 
        {
            l.NextLexem(); // ������� 'else'
            if (l.LexKind == Tok.BEGIN)
            {
                Block();
            }
            else 
            {
                SyntaxError("begin expected");
            }

            if (l.LexKind != Tok.EOF)
                Statement();
        }

        public void ForStatement() 
        {
            l.NextLexem(); // ������� 'for'
            if (l.LexKind == Tok.LEFT_BRACKET)
            {
                l.NextLexem(); //������� '('
                Assign(); //������ ������������� ����������

                if (l.LexKind == Tok.COMMA)
                {
                    l.NextLexem();// ������� ','
                    Expr();
                    if (l.LexKind == Tok.RIGHT_BRACKET)
                    {
                        l.NextLexem(); // ������� ')'
                        Statement(); // ������ ���� �����                        
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

