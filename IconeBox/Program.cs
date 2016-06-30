using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IconeBox
{
    static class Program
    {
        public static Conf conf;
        public static String[] confInit;
        public static string confFile = "";

        //path
        public static string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string groupe;
        public static string PathServerIconBox;
        public static string appName = "iconBox";

        //tools
        public static string I = @"\";
        public static string app;

        /// <summary>
        /// arg1 = groupe
        /// arg2 = server path
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            //parametre
            try
            {
                groupe = args[0];
                PathServerIconBox = args[1];
            }
            catch { }
            //groupe = "cdi";
            //PathServerIconBox = @"\\DISKSTATION\public\logiciel\admin\iconBox";
            app = appdata + I + appName + I + groupe;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            R.screen = Screen.PrimaryScreen.WorkingArea;
            R.notif = new Notif();

            initSystemFolder();
            start();
        }

        public static void start()
        {
            if (init())
            {
                setBoxPosition(R.BOX, conf.colone);
            }
            else
            {

            }
            Application.Run(new MultiFormContext(toRun()));
        }

        //renvoi false si ~conf n'a pas été trouver
        public static bool init()
        {
            bool rez = true;
            //get confinit.txt
            StreamReader file = null;
            try
            {
                file = new StreamReader(app+I+"~conf_init.txt");
            }
            catch
            {
                Debug.WriteLine("conf_init.txt introuvable");
            }

            //TODO ajouter une signature dans conf_init
            if (file != null)//~conf_init.txt doight être présent
            {
                confFile = file.ReadToEnd();
                confInit = confFile.Split('|');

                //conf init
                if (!confInit[0].Equals(""))
                    conf = JsonConvert.DeserializeObject<Conf>(confInit[0]);
                else
                    conf = new Conf();

                //parse conf BOX
                if (!confInit[1].Equals("") && conf.enable)
                {
                    R.BOX = JsonConvert.DeserializeObject<Box[]>(confInit[1]);
                    foreach (Box b in R.BOX)
                        b.init();
                }
                else
                    R.BOX = new Box[0];
                file.Close();
            }
            else
            {
                //envoyer message dans dans notif pour dir conf local introuvable
                R.notif.setNotif("~conf_init.txt introuvable!", 'e');
                rez = false;
                R.BOX = new Box[0];
                conf = new Conf();
            }
            return rez;
        }

        //construi le system de dossioe dans appdata
        public static void initSystemFolder()
        {
            if (Directory.Exists(appdata + I + appName))//si iconbox exist dans appdata
            {
                if (Directory.Exists(appdata + I + appName+I+groupe))//si le groupe exist
                {
                    Debug.WriteLine("sys folder OK");
                }
                else//si le groupe n'existe pas
                {
                    R.log("sys folder : groupe inexistant");
                    R.cpd(PathServerIconBox + I + groupe, appdata + I + appName);
                }
            }
            else//si iconbox exist pas dans appdata
            {
                R.log("sys folder : premierdemarage, rien n'existe");
                Directory.CreateDirectory(appdata+I+appName);
                R.cpd(PathServerIconBox + I + groupe, appdata + I + appName);
            }
        }

        private static Form[] toRun()
        {
            Form[] f = new Form[R.BOX.Length + 1];
            f[0] = R.notif;
            for (int i = 1; i < R.BOX.Length+1; i++)
            {
                f[i] = R.BOX[i-1];
            }
            return f;
        }

        /**
         * definie les atribut width, height, top, left des box contenue dans le tableau
         *param name="boxs" tableau de box
         * param name="nbColonne" nombre de colone du layout
         **/
        static void setBoxPosition(Box[] boxs, int nbColonne)
        {
            if (boxs.Length > 0)
            {
                int nbLine = (boxs.Length % nbColonne == 0) ? (int)boxs.Length / nbColonne : (int)(boxs.Length / nbColonne) + 1;//definie le nb de line
                Rectangle screen = Screen.PrimaryScreen.WorkingArea;
                R.screen = screen;
                Size zonne = new Size((int)(screen.Width / nbColonne), (int)(screen.Height / nbLine));
                int currentLine = 0;
                int currentColonne = 0;

                for (int i = 0; i < boxs.Length; i++)
                {
                    if (currentColonne >= nbColonne)
                    {
                        currentColonne = 0;
                        currentLine++;
                    }

                    Box b = boxs[i];

                    b.widthBox = (int)(zonne.Width * b.coefX);
                    b.heightBox = (int)(zonne.Height * b.coefY);
                    b.leftBox = (int)(currentColonne * zonne.Width) + ((int)(zonne.Width / 2) - (int)(b.widthBox / 2));
                    b.topBox = (currentLine * zonne.Height) + ((zonne.Height / 2) - (b.heightBox / 2));

                    b.updateSize();
                    b.updateLocation();

                    currentColonne++;
                }
            }
        }
    }
}
