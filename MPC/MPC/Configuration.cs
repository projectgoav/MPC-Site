using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPC
{
    public class Configuration
    {

        public const string CONFIG_FILE = "Config.ini";

        public string HeaderTemplate { get; set; }
        public string FooterTemplate { get; set; }
        public string NavTemplate { get; set; }
        public string ModalTemplate { get; set; }

        public string[] Pages { get; set; }
        public string[] Folders { get; set; } 

        public string SourceLocation { get; set; }
        public string DesignLocation { get; set; }
        public string PublishLocation { get; set; }

        public string Username { get; set; }
    }
}
