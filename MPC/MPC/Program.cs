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

        static string HeaderTemplate = null;
        static string NavTemplate = null;
        static string FooterTemplate = null;

        static string HeaderTag = null;
        static string FooterTag = null;
        static string LinkTag = null;

        static string[] Pages = null;



        static void Main(string[] args)
        {   
            string[] Commands = { "publish",
                                  "clean",
                                };

            LoadConfig();


            Console.Write("> ");
            string[] cmd = Console.ReadLine().ToLower().Split(' ');
         


            while (cmd[0] != "quit")
            {
                if (Commands.Contains(cmd[0]) == false) { Console.WriteLine("Invalid command"); }
                else
                {
                    switch(cmd[0])
                    {
                        case "publish": { Publish(cmd); break; }
                        case "clean": { Clean(cmd); break; }
                    }
                }



                Console.Write("> ");
                cmd = Console.ReadLine().ToLower().Split(' ');
            }

        }

        //Compiles pages with Headers and footers

        //TODO - Remove heading from original pages
        //TODO - Fix NavBar styling!
        private static void Publish(string[] cmd)
        {
            //Load in Template Text
            string Header = LoadTemplate(SourceLocation + HeaderTemplate);
            string Footer = LoadTemplate(SourceLocation + FooterTemplate);
            string NavBar = LoadTemplate(SourceLocation + NavTemplate);

            Console.WriteLine("\t> Loaded templates");


            for (int i = 0; i < Pages.Length; ++i)
            {
                Console.WriteLine("\t> Working with: {0}", Pages[i]);

                string PageTitle = Pages[i].Replace(".html", "");
                string PageNav = NavBar;
                string PageContent = null;

                switch (PageTitle)
                {
                    case "index": { PageNav.Replace("id=\"Home\"", "id=\"Home_A\""); break; }
                    case "worship": { PageNav.Replace("id=\"Worship\"", "id=\"Worship_A\""); break; }
                    case "people": { PageNav.Replace("id=\"People\"", "id=\"People_A\""); break; }
                    case "find": { PageNav.Replace("id=\"Contact\"", "id=\"Contact_A\""); break; }
                    case "groups": { PageNav.Replace("id=\"Groups\"", "id=\"Groups_A\""); break; }
                    case "future": { PageNav.Replace("id=\"Future\"", "id=\"Future_A\""); break; }
                    default: break;
                }

                Console.WriteLine("\t \t> NavBar: DONE");

                PageContent = Header + PageNav;
                PageContent += LoadPage(SourceLocation + Pages[i]);
                Console.WriteLine("\t \t> Page Content: DONE");

                File.WriteAllText(PublishLocation + Pages[i], PageContent);
                Console.WriteLine("\t \t> Page Complete!\n");
            }

            //For each Page - format Nav with required ActiveLink
                //Add Nav to Head
                //Add Head to page
                //Add footer to page
                //Copy to publish folder

        }



        //TODO - Do something
        private static void Clean(string[] cmd)
        {
            Console.WriteLine("Nothing to clean.");
        }


        private static void LoadConfig()
        {
            IniConfigSource s = new IniConfigSource(CONFIG_FILE);

            //Template Sources
            HeaderTemplate = s.Configs["Templates"].GetString("Header");
            FooterTemplate = s.Configs["Templates"].GetString("Footer");
            NavTemplate = s.Configs["Templates"].GetString("NavBar");

            //Where to Publish finished products
            PublishLocation = s.Configs["Sources"].GetString("Publish");

            //Get all the pages to fix & where they are stored
            Pages = s.Configs["Sources"].GetString("Pages").Split(',');
            SourceLocation = s.Configs["Sources"].GetString("Source");

            //Formatting tag
            HeaderTag = s.Configs["Tags"].GetString("Head");
            FooterTag = s.Configs["Tags"].GetString("Foot");
            LinkTag = s.Configs["Tags"].GetString("Link");

            Console.WriteLine("Configuration loaded!");
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
