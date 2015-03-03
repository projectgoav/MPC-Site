using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

using Nini.Config;

namespace MPC
{
    class Program
    {
        const string CONFIG_FILE = "Config.ini";

        //Configuration options!
        static string PublishLocation = null;
        static string SourceLocation = null;

        //Template locations
        static string HeaderTemplate = null;
        static string NavTemplate = null;
        static string FooterTemplate = null;
        static string ModalTemplate = null;

        //List of pages to compile
        static string[] Pages = null;



        static void Main(string[] args)
        {   
            string[] Commands = { "compile",
                                  "clean",
                                };

            LoadConfig();   //Load configuration settings

            Console.Write("> ");
            string[] cmd = Console.ReadLine().ToLower().Split(' ');
         
            
            //Just loop until we get a quit
            while (cmd[0] != "quit")
            {
                if (Commands.Contains(cmd[0]) == false) { Console.WriteLine(">> Invalid command"); }
                else
                {
                    switch(cmd[0])
                    {
                        case "compile": { Compile(null); break; }
                        case "clean": { Clean(cmd); break; }
                    }
                }


                //Get next input
                Console.Write("> ");
                cmd = Console.ReadLine().ToLower().Split(' ');
            }

        }

        //Compiles pages with Headers and footers
        private static void Compile(string[] cmd)
        {
            //Load in Template Text
            string Header = LoadTemplate(SourceLocation + HeaderTemplate);
            string Footer = LoadTemplate(SourceLocation + FooterTemplate);
            string NavBar = LoadTemplate(SourceLocation + NavTemplate);
            string Modals = LoadTemplate(SourceLocation + ModalTemplate);

            Console.WriteLine("\t> Loaded templates");


            for (int i = 0; i < Pages.Length; ++i)
            {
                Console.Write("\t> Page: {0}     (working)", Pages[i]);

                string PageTitle = Pages[i].Replace(".html", "");
                string PageNav = NavBar;
                string PageContent = null;

                switch (PageTitle)
                {
                    case "index": { PageNav = PageNav.Replace("id=\"Home\"", "id=\"Home_A\""); break; }
                    case "worship": { PageNav = PageNav.Replace("id=\"Worship\"", "id=\"Worship_A\""); break; }
                    case "people": { PageNav = PageNav.Replace("id=\"People\"", "id=\"People_A\""); break; }
                    case "find": { PageNav = PageNav.Replace("id=\"Contact\"", "id=\"Contact_A\""); break; }
                   case "future": { PageNav = PageNav.Replace("id=\"Future\"", "id=\"Future_A\""); break; }

                    //We need to do something special here and load in our modals for the Groups
                    case "groups": { 
                        PageNav = PageNav.Replace("id=\"Groups\"", "id=\"Groups_A\"");
                        PageNav += Modals;
                        break; }

                    
                    default: break;
                }

                //Put the page together and save elsewhere
                PageContent = Header + PageNav;
                PageContent += LoadPage(SourceLocation + Pages[i]);
                PageContent += Footer;

                File.WriteAllText(PublishLocation + Pages[i], PageContent);
                Console.Write("\r \t> Page: {0}     (DONE)                     \n", Pages[i]);
            }
        }



        //TODO - Do something
        private static void Clean(string[] cmd)
        {
            Console.WriteLine(">> Nothing to clean.");
        }


        private static void LoadConfig()
        {
            IniConfigSource s = new IniConfigSource(CONFIG_FILE);

            //Template Sources
            HeaderTemplate = s.Configs["Templates"].GetString("Header");
            FooterTemplate = s.Configs["Templates"].GetString("Footer");
            NavTemplate = s.Configs["Templates"].GetString("NavBar");
            ModalTemplate = s.Configs["Templates"].GetString("Modal");

            //Where to Publish finished products
            PublishLocation = s.Configs["Sources"].GetString("Publish");

            //Get all the pages to fix & where they are stored
            Pages = s.Configs["Sources"].GetString("Pages").Split(',');
            SourceLocation = s.Configs["Sources"].GetString("Source");

            Console.WriteLine(">> Configuration loaded!");
        }

        //Returns a loaded Template from HTML
        private static string LoadTemplate(string Filename)
        {
            string Template = "";
            string[] Buffer = File.ReadAllLines(Filename);

            foreach (string line in Buffer) { Template += line; }

            return Template;
        }

        private static string LoadPage(string Filename)
        {
            string Page = "";
            string[] Buffer = File.ReadAllLines(Filename);

            foreach (string Line in Buffer) { Page += Line; }

            return Page;
        }

    }
}
