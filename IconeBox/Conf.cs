using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IconeBox
{
    class Conf
    {
        public string pass = "doc";
        public int colone = 2;
        public string pathConf = "";
        public bool enable = true;

        public void setPathconf()
        {
            StreamReader file = null;
            try
            {
                file = new StreamReader("conf_init.txt");
                this.pathConf = file.ReadToEnd();
            }
            catch
            {
                Debug.WriteLine("path-conf.txt introuvable");
            }
        }
    }
}
