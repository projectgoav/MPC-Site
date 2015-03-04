using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

using Nini.Config;

namespace MPC
{
    /// <summary>
    /// MPC helper program for use when developing the Website
    /// </summary>
    class Program
    {
        public const string CONFIG_FILE = "Config.ini";

        //Configuration
        private Configuration Config;

        private string[] Commands = { "compile", "publish", "clean" };


        /// <summary>
        /// Entry point to application
        /// </summary>
        static void Main(string[] args)
        {
            Program me = new Program();
            me.Run();
            Console.WriteLine("> Bye!");
        }


        /// <summary>
        /// Main Program loop which deals with input and dispatches to tasks
        /// </summary>
        private void Run()
        {
            //Load configuration settings and close if they are equal to null
            Config = FileIO.LoadConfig();
            if (Config == null) { Console.WriteLine("\n\n>> Exited with error code 0x2"); System.Environment.Exit(2); } 


            //Enter Main Command Loop and get input from user
            Console.Write("MPC> ");
            string[] cmd = Console.ReadLine().ToLower().Split(' ');

            //Just loop until we get a quit
            while (cmd[0] != "quit")
            {
                if (Commands.Contains(cmd[0]) == false) { Console.WriteLine(">> Invalid command"); }
                else
                {
                    switch (cmd[0])
                    {
                        case "compile": { Compile(null); break; }
                        case "clean": { Clean(cmd); break; }
                        case "publish":
                            {
                                FTP ftp = new FTP(Config.Username);
                                ftp.Publish(Config.PublishLocation, Config.Pages.Length);
                                break;
                            }
                    }
                }

                //Get next input
                Console.Write("MPC> ");
                cmd = Console.ReadLine().ToLower().Split(' ');
            }

        }

        //Compiles pages with Headers and footers
        private void Compile(string[] cmd)
        {
            DateTime First = DateTime.Now;

            //Load in Template Text
            string Header = FileIO.LoadHTML(Config.SourceLocation + Config.HeaderTemplate);
            string Footer = FileIO.LoadHTML(Config.SourceLocation + Config.FooterTemplate);
            string NavBar = FileIO.LoadHTML(Config.SourceLocation + Config.NavTemplate);
            string Modals = FileIO.LoadHTML(Config.SourceLocation + Config.ModalTemplate);

            Console.WriteLine("> Loaded templates");


            for (int i = 0; i < Config.Pages.Length; ++i)
            {
                Console.Write("> (working) Page: {0}", Config.Pages[i]);

                string PageTitle = Config.Pages[i].Replace(".html", "");
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
                PageContent += FileIO.LoadHTML(Config.SourceLocation + Config.Pages[i]);
                PageContent += Footer;

                File.WriteAllText(Config.PublishLocation + Config.Pages[i], PageContent);
                Console.Write("\r> (DONE) Page: {0}                          \n", Config.Pages[i]);
            }

            DateTime Second = DateTime.Now;
            TimeSpan TimeTaken = Second - First;

            Console.WriteLine("> Compiled {0} page(s) in {1}ms\n", Config.Pages.Length, TimeTaken.TotalMilliseconds);
        }


        private static void Clean(string[] cmd)
        {
            Console.WriteLine(">> Nothing to clean.");
        }

    }
}
