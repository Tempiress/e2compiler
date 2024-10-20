using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Net;
using System.IO;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using System.Diagnostics;
using System.Runtime;
using static System.Net.Mime.MediaTypeNames;
using System.Threading.Tasks;
using NunitReport;


namespace Report
{
    class Program
    {
        static string findConfig()
        {
            string guess = "config.json";
            int nTries = 0;
            while(!File.Exists(guess) && nTries < 5) {
                guess = @"../" + guess;
                nTries++;
            }
            return guess;
        }

        // run as NUnitReport ${CI_PROJECT_DIR} ${GITLAB_PROJECT_NAME}
        // NUnitReport WORKDIR USERNAME
        static void Main(string[] args)
        {
            
            string filePath;
            string userName;
            string basePath = args[0];
            string configPath = findConfig();
            if (args.Length > 0)
            {
                filePath = basePath + @"/TestResult.xml";
                var nameparts = args[1].Split('/');
                string repoName = "";
                if (nameparts.Length > 1)
                {
                    repoName = nameparts[1];
                    // TODO: check if it's [1] or [-1]
                    userName = repoName;
                }
                else
                {
                    userName = args[1];
                }
            }
            else
            {
                userName = "Test";
                filePath = @"./TestResult.xml";
            }
            System.Console.WriteLine("user name: " + userName);
            System.Console.WriteLine("base path: " + basePath);
            

            ReportParser.ProcessReport(basePath, filePath, userName, configPath);
        }
    }

    class ReportNode
    {
        public string id;
        public string title;
        public string author;
    }
}