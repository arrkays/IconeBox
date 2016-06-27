using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IconeBox
{
    public partial class Box : Form
    {
        string I = @"\";
        private SystemIconsImageList sysIcons = new SystemIconsImageList();
        //Atribut
        public double coefX = 0.8;
        public double coefY = 0.8;
        public int widthBox = 0;
        public int heightBox = 0;
        public int topBox = 0;
        public int leftBox = 0;
        public Color colorTitle = Color.White;
        public Color backgroundTitle = Color.FromArgb(255, 20, 20, 20);
        public string titleBox = "";
        public int titleFontSize = 15;
        public Font titleFont = new Font("Arial", 15);
        public Color backgroundBox = Color.FromArgb(255, 20, 20, 20);
        Ico[] icon;
        //int sizeIco; 
        public String folderPath;//dossier contenant les icons a metre dans la box
        public int radiusBorder = 16; //pair number
        public int listX = 20; //padding left right
        public int listY = 10; //padding top bot
        public int borderThickness = 1;
        public int separatorWidth = 1;
        public Color borderColor = Color.White;
        public int titleHeight = 30;//margin-top
        bool isClosable = false;

        public Box()
        {
            InitializeComponent();
        }

        /*public Box(String folderPath)
        {
            InitializeComponent();
            this.folderPath = folderPath;
            displayList(folderPath);
        }*/

        public void init()
        {
            //folderPath = Program.app + I + folderPath;
            parseAtribuText();
            displayList(Program.app + I + folderPath);
        }


        private void Box_Shown(Object sender, EventArgs e)
        {
            drawBorder();
        }

        //display icon in lisView1 tag = target path of the shortcut
        //set title width folder name if no title
        private void displayList(string folderPath)
        {
            listView1.SmallImageList = sysIcons.SmallIconsImageList;
            listView1.LargeImageList = sysIcons.LargeIconsImageList;

            //define title
            if (titleBox.Equals(""))
            {
                titleBox = folderPath.Substring(folderPath.LastIndexOf('\\')+1);
            }

            //get folderFile into files array
            DirectoryInfo root = new DirectoryInfo(folderPath);
            FileInfo[] files = null;
            try
            {
                files = root.GetFiles();
            }
            catch
            {
                R.notif.setNotif(this.titleBox+" introuvable", 'w');
            }

            if (files != null)
            {
                this.icon = new Ico[files.Length];
                foreach (FileInfo f in files)
                {
                    if (f.Extension.EndsWith(".lnk"))//if shortcut
                    {
                        String targetPath = R.GetTargetPath(f.FullName);//shortcut target
                        if (File.Exists(targetPath))//si la sible exist on affiche l'item
                        {
                            ListViewItem item = new ListViewItem(f.Name.Substring(0, f.Name.Length - 4));
                            item.Tag = f.FullName;
                            item.ImageIndex = sysIcons.GetIconIndex(targetPath);
                            listView1.Items.Add(item);
                        }
                        else
                        {
                            //bug si target introuvable on essay de remplace Program Files (x86) par Program Files
                            targetPath = targetPath.Replace("Program Files (x86)", "Program Files");
                            if (File.Exists(targetPath))//si la sible exist on affiche l'item
                            {
                                ListViewItem item = new ListViewItem(f.Name.Substring(0, f.Name.Length - 4));
                                item.Tag = targetPath;
                                item.ImageIndex = sysIcons.GetIconIndex(targetPath);
                                listView1.Items.Add(item);
                            }
                            else
                            {
                                Debug.WriteLine("Fichier introuvable");
                                Debug.WriteLine("full -> " + f.FullName);
                                Debug.WriteLine("name -> " + f.Name.Substring(0, f.Name.Length - 4));
                                Debug.WriteLine("target> " + targetPath);
                                Debug.WriteLine("");
                            }
                        }
                    }
                    else if (f.Extension.EndsWith(".url"))//Raccourci Internet
                    {
                        ListViewItem item = new ListViewItem(f.Name.Substring(0, f.Name.Length - 4));
                        item.Tag = f.FullName;
                        item.ImageIndex = sysIcons.GetIconIndex(GetStandardBrowserPath());
                        listView1.Items.Add(item);
                    }
                    /*else if (f.Extension.EndsWith(".appref-ms"))
                    {

                    }*/
                }
            }
        }

        private static string GetStandardBrowserPath()
        {
            string browserPath = string.Empty;
            RegistryKey browserKey = null;

            try
            {
                //Read default browser path from Win XP registry key
                browserKey = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false);

                //If browser path wasn't found, try Win Vista (and newer) registry key
                if (browserKey == null)
                {
                    browserKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http", false); ;
                }

                //If browser path was found, clean it
                if (browserKey != null)
                {
                    //Remove quotation marks
                    browserPath = (browserKey.GetValue(null) as string).ToLower().Replace("\"", "");

                    //Cut off optional parameters
                    if (!browserPath.EndsWith("exe"))
                    {
                        browserPath = browserPath.Substring(0, browserPath.LastIndexOf(".exe") + 4);
                    }

                    //Close registry key
                    browserKey.Close();
                }
            }
            catch
            {
                //Return empty string, if no path was found
                return string.Empty;
            }
            //Return default browsers path
            return browserPath;
        }

        //getset
        public void setSize(int w, int h)
        {
            widthBox = w;
            heightBox = h;
            updateSize();
        }

        public void updateSize()
        {
            this.ClientSize = new System.Drawing.Size(widthBox, heightBox);
            this.listView1.Size = new System.Drawing.Size(widthBox-listX*2, (heightBox-titleHeight)-listY*2);

            //draw border
           
        }

        public void setLocation(int x, int y)
        {
            leftBox = x;
            topBox = y;
            updateLocation();
        }

        public void updateLocation()
        {
            this.Location = new System.Drawing.Point(leftBox, topBox);
        }

        //Event
        void onClicItem(object sender, System.EventArgs e)
        {
            lauchExe();
        }

        void lauchExe()
        {
            string path = listView1.SelectedItems[0].Tag.ToString();
            Process proc = new Process();
            proc.StartInfo.FileName = path;
            proc.Start();
        }

        public void makeExit()
        {
            Action test = exitBox;
            this.Invoke(test, null);
        }

        public void exitBox()
        {
            isClosable = true;
            this.Close();
        }

        //deny alt+F4 to close
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && !isClosable)
                e.Cancel = true;
        }

        //??
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        //hide in alt tab
        protected override CreateParams CreateParams
        {
            get
            {
                var Params = base.CreateParams;
                Params.ExStyle |= 0x80;
                return Params;
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
