using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

using Nini.Config;

namespace MPC
{
    /// <summary>
    /// Contains static methods for FILE I/O within MPC
    /// </summary>
    public class FileIO
    {
        /// <summary>
        /// Loads in Config.ini to MPC
        /// </summary>
        /// <returns>A filled configuration file, or null if there was a problem</returns>
        public static Configuration LoadConfig()
        {
            Configuration cfg = new Configuration();

            try
            {

                IniConfigSource s = new IniConfigSource(Configuration.CONFIG_FILE);

                //Template Sources
                cfg.HeaderTemplate = s.Configs["Templates"].GetString("Header");
                cfg.FooterTemplate = s.Configs["Templates"].GetString("Footer");
                cfg.NavTemplate = s.Configs["Templates"].GetString("NavBar");
                cfg.ModalTemplate = s.Configs["Templates"].GetString("Modal");

                //Where to Publish finished products
                cfg.PublishLocation = s.Configs["Sources"].GetString("Publish");

                //Get all the pages to fix & where they are stored
                cfg.Pages = s.Configs["Sources"].GetString("Pages").Split(',');
                cfg.SourceLocation = s.Configs["Sources"].GetString("Source");

                Console.WriteLine(">> Configuration loaded!");

                return cfg;
            }
            catch (Exception e) { Console.WriteLine("MPC>> * Configuration file error *\nMessage: {0} \n\nTrace: {1}", e.Message, e.StackTrace); return null; }
        }

        /// <summary>
        /// Load and read in data from given HTML file as one long line
        /// </summary>
        /// <param name="Filename">HTML file to load</param>
        /// <returns>Data from HTML Line on one line or exits if error occured</returns>
        public static string LoadHTML(string Filename)
        {
            try
            {
                string HTML = "";
                string[] Buffer = File.ReadAllLines(Filename);

                foreach (string Line in Buffer) { HTML += Line; }

                return HTML;
            }
            catch (Exception e)
            {
                Console.WriteLine("MPC>> * Error loading in HTML Document {0} *\nMessage: {1} \n\nTrace: {2}", Filename, e.Message, e.StackTrace);
                Console.WriteLine("\n\n>> Exited with error code 0x4"); System.Environment.Exit(4);
                return null;
            } 
        }
    }
}
