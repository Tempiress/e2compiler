using System;
using System.IO;
using NUnit.Framework;
using SimpleParser;
using SimpleLexer;

[assembly: Property("Task", "DESCENT_PARSER_EXPRESSION2")]

namespace Tests
{
    [TestFixture, Property("Score", 9)]
    [Property("Job", "ParserTests")]
    public class TestParser
    {
        private bool Parse(string text)
        {
            TextReader inputReader = new StringReader(text);
            Lexer l = new Lexer(inputReader);
            Parser p = new Parser(l);
            
            p.Progr();
            if (l.LexKind == Tok.EOF)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        [Test, Property("ScorePercentage", 33)]
        public void TestMy() 
        {
            Assert.IsTrue(Parse(@"{while(true){}}"));
        }

        [Test, Property("ScorePercentage", 33)]
        public void TestLoops()
        {
            Assert.IsTrue(Parse(@"while(5){ a=2}"));

            Assert.IsTrue(Parse(@"{ 
                                     while (5){ 
                                       
                                       a=2 
                                     }
                                  }"));

            Assert.IsTrue(Parse(@"{while(5){
                                    while(6){
                                    b = 4 
                                    while(2){a = 5}}}}"));
            
            //Assert.IsTrue(Parse(@"{while (5){                                      
            //                           while (6){ 
            //                            a=2 
            //                           while (7){ 
            //                             a=3
            //                             c=4
            //                           }
            //                         }
            //                      }}"));

            //Assert.IsTrue(Parse(@"{ 
            //                         for (a = 1, 5, 1){ 
                                      
            //                           b=1
            //                         }
            //                      }"));

            //Assert.IsTrue(Parse(@"{ 
            //                         for (a=1, 5, 1) 
            //                          {
            //                           for (i = 1, 6, 1)
            //                           c:=1
            //                           b:=1 
            //                         }
            //                      }"));


        }


        [Test, Property("ScorePercentage", 33)]
        public void TestConditionals1()
        {
            Assert.IsTrue(Parse(@"if(2){a=2}else{b=2}"));

        }



        [Test, Property("ScorePercentage", 33)]
        public void TestConditionals()
        {
            Assert.IsTrue(Parse(@" 
                                     if (2){  
                                        a=2}
                                     else{ 
                                        b=2}

                                     if (3)
                                     {
                                        if (c)
                                        {
                                            c=4
                                        }
                                         else{
                                            m=1}
                                      }
                                     else{
                                        v=8}   
                                    
                                     if (4) {
                                       if (4){
                                         if (6){
                                            m=0}}}
                                  "));

        }

        [Test, Property("ScorePercentage", 34)]
        public void TestExpressions()
        {
            Assert.IsTrue(Parse(@" 
            if (2 + 2 * (c - d / 3))
            {
            a = 2
            while (2 - 3 + f) { c = c * 2 }
            }
            else
            {
                b = 2 - 3 * (c - d / f * 3)
            }

            for (i = 2 - 3 * (s - d), (c - 3))
            {
                a = (a - (3 - 3))
            }

            if (3)
            {
                if (c - 3)
                {
                    c = 4 + 2
                }
                else
                {
                    m = 1
                }
                else
                {
                    v = (8 + 2)
                }
            }
                                  "));
        }


        [Test, Property("ScorePercentage", 34)]
        public void TestExpressions2()
        {
            Assert.IsTrue(Parse(@" 
            if (2 + 2 * (c - d / 3))
            {
            a = 2
            }
                                  "));
        }
    }











}