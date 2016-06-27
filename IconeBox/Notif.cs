using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IconeBox
{
    class Notif : Form
    {
        // private IContainer components;
        PictureBox updateImg;
        PictureBox warningImg;
        PictureBox errorImg;
        PictureBox itWorkImg;
        TextBox mdp;
        Label mdpLabel;
        Label message;
        OpenFileDialog openFile;
        NotifyIcon notif;
        //notif
        bool showError = false;
        bool showWarning = false;
        string msgString = "";
        bool isSearchingFile = false;
        private bool showItWork;

        public Notif()
        {
            displayIcon();
            this.Deactivate += unFocus;
            this.ShowInTaskbar = false;
            this.ClientSize = new Size(200, 40);
            this.StartPosition = FormStartPosition.Manual;
            this.FormBorderStyle = FormBorderStyle.None;
            //this.BackColor = Color.Gray;
            //add image
            updateImg = new PictureBox();
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream imageStream = assembly.GetManifestResourceStream("IconeBox.img.update.bmp");
            updateImg.Image = Image.FromStream(imageStream);
            updateImg.Size = new Size(50, 28);
            //updateImg.Location = new Point(ClientSize.Width / 2 - updateImg.Size.Width/2, ClientSize.Height - updateImg.Size.Height-5); //center
            updateImg.Location = new Point(ClientSize.Width - updateImg.Size.Width-6, ClientSize.Height - updateImg.Size.Height-5);
            updateImg.MouseHover += mouseoverImg;
            updateImg.MouseDown += mousedownImg;
            updateImg.MouseUp += mouseupImg;
            updateImg.MouseLeave += mouseLeaveImg;


            //add mdp field
            mdp = new TextBox();
            mdp.PasswordChar = ' ';
            mdp.Width = 150;
            mdp.Location = new Point(ClientSize.Width - mdp.Width - 5, 7);
            mdp.KeyDown += mdpHandeler;

            //add label mdp
            mdpLabel = new Label();
            mdpLabel.Text = "Login :";
            mdpLabel.Size = new Size(50, 20);
            mdpLabel.Location = new Point(5, 9);

            //add warning image
            warningImg = new PictureBox();
            assembly = Assembly.GetExecutingAssembly();
            imageStream = assembly.GetManifestResourceStream("IconeBox.img.warning.bmp");
            warningImg.Image = Image.FromStream(imageStream);
            warningImg.Size = new Size(30, 30);
            warningImg.Location = new Point(5, ClientSize.Height - warningImg.Height -6);
            warningImg.Visible = false;

            //add error image
            errorImg = new PictureBox();
            assembly = Assembly.GetExecutingAssembly();
            imageStream = assembly.GetManifestResourceStream("IconeBox.img.error.bmp");
            errorImg.Image = Image.FromStream(imageStream);
            errorImg.Size = new Size(30, 30);
            errorImg.Location = new Point(6, ClientSize.Height - errorImg.Height - 5);
            errorImg.Visible = false;

            //add message warning/error
            message = new Label();
            message.Size = new Size(100, 30);
            message.Location = new Point(43, ClientSize.Height-35);
            message.Text = "";
            message.Tag = "o"; // type ok par default

            // add it  work image
            itWorkImg = new PictureBox();
            assembly = Assembly.GetExecutingAssembly();
            imageStream = assembly.GetManifestResourceStream("IconeBox.img.itWork.bmp");
            itWorkImg.Image = Image.FromStream(imageStream);
            itWorkImg.Size = new Size(30, 30);
            itWorkImg.Location = new Point(6, ClientSize.Height - itWorkImg.Height - 5);
            Debug.WriteLine("------------------------------------------------5----------------------------------------------");

            //file explorer
            openFile = new OpenFileDialog();
            string defaultPath = @"\\DISKSTATION\public";
            //if (Directory.Exists(defaultPath))
                openFile.InitialDirectory = defaultPath;
            //openFile.DefaultExt = "conf_init.txt";

            Debug.WriteLine("------------------------------------------------6----------------------------------------------");


            //add componante
            this.Controls.Add(updateImg);
           // this.Controls.Add(mdp);
           // this.Controls.Add(mdpLabel);
            this.Controls.Add(warningImg);
            this.Controls.Add(errorImg);
            this.Controls.Add(message);
            this.Controls.Add(itWorkImg);

            //paint border
            this.Shown += show;
        }


        /*internal void resetNotif()
        {
            showError = false;
            showWarning = false;
            msgString = "";
            if (this.Visible)
            {
                Action a = showMessage;
                this.Invoke(a, null);
            }
        }*/

        /**
         *type = w for warning
         *type = e for error
         *type = o for ok
         */
        public void setNotif(string msg, char type)
        {
            
            if (type == 'w' && !showError)
            {
                showItWork = false;
                showItWork = false;
                showWarning = true;
                msgString = msg;
                message.Tag = "w";
            }
            else if (type == 'e')
            {
                showItWork = false;
                showWarning = false;
                showError = true;
                msgString = msg;
                message.Tag = "e";
            }
            else if(type == 'o')
            {
                showWarning = false;
                showError = false;
                showItWork = true;
                msgString = msg;
                message.Tag = "o";
                
            }

            if(this.Visible)
            {
                Action a = showMessage;
                this.Invoke(a, null);
            }
        }

        void showMessage()
        {
            if (showWarning)
            {
                warningImg.Visible = true;
                message.ForeColor = Color.Orange;
                message.Text = msgString;
                itWorkImg.Visible = false;
                errorImg.Visible = false;
            }
            else if (showError)
            {
                errorImg.Visible = true;
                message.ForeColor = Color.Red;
                message.Text = msgString;
                itWorkImg.Visible = false;
                warningImg.Visible = false;
            }
            else if(showItWork)
            {
                message.Text = msgString;
                errorImg.Visible = false;
                warningImg.Visible = false;
                itWorkImg.Visible = true;
            }
        }

        private void mouseLeaveImg(object sender, EventArgs e)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream imageStream = assembly.GetManifestResourceStream("IconeBox.img.update.bmp");
            updateImg.Image = Image.FromStream(imageStream);
        }

        private void mouseupImg(object sender, MouseEventArgs e)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream imageStream = assembly.GetManifestResourceStream("IconeBox.img.update.bmp");
            updateImg.Image = Image.FromStream(imageStream);

            //lauch update
            if ((ModifierKeys & Keys.Control) == Keys.Control)
                R.hardReboot = true;
            R.updateConf();
        }

        private void mousedownImg(object sender, MouseEventArgs e)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream imageStream = assembly.GetManifestResourceStream("IconeBox.img.updateCliced2.bmp");
            updateImg.Image = Image.FromStream(imageStream);
        }

        private void mouseoverImg(object sender, EventArgs e)
        {
            updateImg.Cursor = Cursors.Hand;
        }

        private void mdpHandeler(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (mdp.Text.Equals(Program.conf.pass))
                {
                    isSearchingFile = true;
                    if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        updateServerPath(openFile.FileName);
                    }
                    isSearchingFile = false;
                }
                else
                {
                    if(!mdp.Text.Equals(""))
                        setNotif("login incorrecte", 'w', 800);
                }
                mdp.Text = "";
                e.Handled = true;//enlève le ding
                e.SuppressKeyPress = true;//enlève le ding
            }
        }

        public void setNotif(string msg, char type, int time)
        {
            Thread t = new Thread(delegate ()
            {
                string s = (string)this.message.Tag;
                char oldType = s.ToCharArray()[0];
                string oldMessage = this.message.Text;

                setNotif(msg, type);
                Thread.Sleep(time);

                setNotif(oldMessage, oldType);
            }); t.Start();
        }

        private void updateServerPath(string fileName)
        {
            if (fileName.Substring(fileName.LastIndexOf('\\') + 1).Equals("conf_init.txt"))
            {
                File.WriteAllText("path_conf.txt", fileName);
                R.updateConf();
            }
            else
            {
                setNotif("fichier incompatible.", 'e');
            }
        }

        void setLocation(Point souris)
        {

            int x = souris.X - (ClientSize.Width / 2);
            if(x + ClientSize.Width > R.screen.Width)
                x = R.screen.Width - ClientSize.Width;
            int y;
            if (souris.Y > R.screen.Height)
                y = R.screen.Height - ClientSize.Height - 1;
            else
                y = souris.Y - ClientSize.Height-34;
            if (y < 0)
                y = 0;
            this.Location = new Point(x,y);
        }

        void unFocus(object sender, EventArgs e)
        {
            toggleVisibility();
        }



        //icon
        public void displayIcon()
        {
            //get image ico
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream imageStream = assembly.GetManifestResourceStream("IconeBox.ico6_64.ico");

            //set menu notif 
            Container containerIco = new Container();
            ContextMenu menuIco = new ContextMenu();
            MenuItem item1 = new MenuItem();
            MenuItem[] menuItems = { item1 };

            //init menu
            menuIco.MenuItems.AddRange(menuItems);

            //init item
            item1.Index = 0;
            item1.Text = "actualise";
            item1.Click += new EventHandler(updateAll);

            //init notifi
            notif = new NotifyIcon(containerIco);

            try
            {
                notif.Icon = new Icon(imageStream);
            }
            catch
            {
                Debug.WriteLine("Erreur creation icon");
            }

            //ajou du menu
            //notif.ContextMenu = menuIco;
            notif.Text = "Icone Box";
            notif.Visible = true;
            notif.Click += new System.EventHandler(showMenuNotif);
        }

        private void Notif_MouseDown(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("ddown");
        }

        void toggleVisibility()
        {
            if (this.Visible && !isSearchingFile)
                this.Visible = false;
            else
            {
                Debug.WriteLine("this.Visible = true;");
                //this.Show();
                this.Visible = true;
                Debug.WriteLine(this.Focus());
                this.Activate();
                drawBorder();
                showMessage();
            }
        }

        void showMenuNotif(object sender, EventArgs e)
        {
            setLocation(new Point(Cursor.Position.X,Cursor.Position.Y));
            toggleVisibility();
        }

        private void updateAll(object sender, EventArgs e)
        {
            R.updateConf();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Notif
            // 
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Name = "Notif";
            this.ResumeLayout(false);
        }

        void show(Object sender, EventArgs e)
        {
            Debug.WriteLine("show");
            drawBorder();
        }

        private void drawBorder()
        {
            int widthPen = 1;
            Pen pen = new Pen(Color.Gray);
            pen.Width = widthPen;

            Graphics g = this.CreateGraphics();
            g.DrawRectangle(pen, new Rectangle(0, 0, ClientSize.Width-widthPen, ClientSize.Height-widthPen));
            Debug.WriteLine("border drowed");
        }

    }
}
