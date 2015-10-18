using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AltTabHelperV2
{
    public partial class MainWindow : Form
    {

        public static MainWindow self;

        public MainWindow()
        {
            InitializeComponent();
            self = this;
            this.WindowState = FormWindowState.Minimized;
            MimimizeMe();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            labelDescription.Text =
                @"

This app improves Alt Tab. It basically hooks on Alt + ` [the key left to 1 and above Tab] 
and switches the focus of apps from the same executable.

It work great to switch between command prompts, terminals, browsers, word processors, etc.... 

You can of course hit Shift to switch the focus on the opposite order.

The app is open source: https://github.com/marcosdiez/alt_tab_helper

Unfortunatelly, due to some restrictions on Windows, this app just work properly if started by a debugger.
Therefore we recomend it being launched via LauncherV2.exe instead of directly.

";

            HookManager.EnsureSubscribedToGlobalKeyboardEvents();

        }

        private void labelDescription_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/marcosdiez/alt_tab_helper");
        }

        private void myNotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            RestoreMe();
        }

        public void RestoreMe()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        public void MimimizeMe()
        {
            myNotifyIcon.Visible = true;
            this.ShowInTaskbar = false;
            this.Hide();
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            myNotifyIcon.Visible = false;
            Environment.Exit(0);
        }

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                MimimizeMe();
            }

            else if (FormWindowState.Normal == this.WindowState)
            {

                labelDescription.Top = 0;
                labelDescription.Left = 0;
                labelDescription.Width = ClientRectangle.Width;
                labelDescription.Height = ClientRectangle.Height;

                myNotifyIcon.Visible = false;
                this.ShowInTaskbar = true;
            }
        }


    }
}
