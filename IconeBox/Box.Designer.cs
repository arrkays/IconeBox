using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace IconeBox
{
    partial class Box
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listView1 = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(listX, listY + titleHeight);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(widthBox, heightBox);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.ForeColor = Color.White;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            this.listView1.DoubleClick += new System.EventHandler(onClicItem);
            this.listView1.MultiSelect = false;
            this.listView1.KeyDown += ListView1_KeyDown;
            this.listView1.BorderStyle = BorderStyle.None;

            // 
            // Box
            // 
            this.StartPosition = FormStartPosition.Manual;
            this.ClientSize = new System.Drawing.Size(widthBox, heightBox);
            this.Controls.Add(this.listView1);
            this.Name = "Box";
            this.Text = "Box";
            //--perso
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.Magenta;
            this.Shown += Box_Shown;
            this.Activated += Box_Activated;
            this.Load += Box_Load;
            this.TransparencyKey = Color.Magenta;
        }

        private void ListView1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                lauchExe();
            }
        }

        private void Box_Load(object sender, EventArgs e)
        {
            SetWindowPos(Handle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
        }

        private void Box_Activated(object sender, EventArgs e)
        {
            SetWindowPos(Handle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
        }



        [DllImport("user32.dll")]
        static extern bool SetWindowPos(System.IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 SWP_NOACTIVATE = 0x0010;

        //drawing border
        public void drawBorder()
        {
            SolidBrush myBrush = new SolidBrush(borderColor);
            Graphics g = this.CreateGraphics();

            //bordure
            //angle arondi
            g.FillEllipse(myBrush, new Rectangle(0, 0, radiusBorder, radiusBorder));
            g.FillEllipse(myBrush, new Rectangle(widthBox - radiusBorder, 0, radiusBorder, radiusBorder));
            g.FillEllipse(myBrush, new Rectangle(widthBox - radiusBorder, heightBox - radiusBorder, radiusBorder, radiusBorder));
            g.FillEllipse(myBrush, new Rectangle(0, heightBox - radiusBorder, radiusBorder, radiusBorder));

            //bord
            g.FillRectangle(myBrush, new Rectangle(radiusBorder / 2, 0, widthBox - radiusBorder, heightBox));
            g.FillRectangle(myBrush, new Rectangle(0, radiusBorder / 2, widthBox, heightBox - radiusBorder));

            myBrush.Color = backgroundBox;
            //background
            //angle arondi
            g.FillEllipse(myBrush, new Rectangle(borderThickness, borderThickness, radiusBorder, radiusBorder));
            g.FillEllipse(myBrush, new Rectangle(widthBox - radiusBorder - borderThickness, borderThickness, radiusBorder, radiusBorder));
            g.FillEllipse(myBrush, new Rectangle(widthBox - radiusBorder - borderThickness, heightBox - radiusBorder - borderThickness, radiusBorder, radiusBorder));
            g.FillEllipse(myBrush, new Rectangle(borderThickness, heightBox - radiusBorder - borderThickness, radiusBorder, radiusBorder));

            //bord
            //top/bot
            g.FillRectangle(myBrush, new Rectangle(radiusBorder / 2 + borderThickness, borderThickness, widthBox - radiusBorder - borderThickness * 2, heightBox - borderThickness * 2));
            //left/right
            g.FillRectangle(myBrush, new Rectangle(borderThickness, radiusBorder / 2 + borderThickness, widthBox - borderThickness * 2, heightBox - radiusBorder - borderThickness * 2));

            //titre --
            Pen pen = new Pen(borderColor);
            pen.Width = separatorWidth;

            //separateur
            if (separatorWidth > 0)
            {
                g.DrawLine(pen, 0, titleHeight, Width, titleHeight);
            }

            //draw string
            Font drawFont = titleFont;
            myBrush.Color = colorTitle;
            StringFormat drawFormat = new StringFormat();
            Size s = TextRenderer.MeasureText(titleBox, drawFont);
            g.DrawString(titleBox, drawFont, myBrush, new PointF(widthBox/2 - s.Width/2, titleHeight/2-s.Height/2));
            
            myBrush.Dispose();
            g.Dispose();
        }

        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            drawBorder();
        }

        private System.Windows.Forms.ListView listView1;
        //attribue forme text pour parse json
        public string colorTitleText;
        public string backgroundTitleText;
        public string titleFontText;
        public string backgroundBoxText;
        public string borderColorText;

        void parseAtribuText()
        {
            colorTitle = ColorTranslator.FromHtml(colorTitleText);
            backgroundTitle = ColorTranslator.FromHtml(backgroundTitleText);
            this.titleFont = new Font(titleFontText, (float)titleFontSize);
            backgroundBox = ColorTranslator.FromHtml(backgroundBoxText);
            borderColor = ColorTranslator.FromHtml(borderColorText);
            this.listView1.BackColor = backgroundBox;
        }

    }
}