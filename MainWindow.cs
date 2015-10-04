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

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            labelDescription.Top = 0;
            labelDescription.Left = 0;
            labelDescription.Width = ClientRectangle.Width;
            labelDescription.Height = ClientRectangle.Height;
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            MainWindow_Resize(sender, e);

            labelDescription.Text =
                @"

This app improves Alt Tab. It basically hooks on Alt + ` [the key left to 1 and above Tab] 
and switches the focus of apps from the same executable.

It work great to switch between command prompts, terminals, browsers, word processors, etc.... 

You can of course hit Shift to switch the focus on the opposite order.

The app is open source: https://github.com/marcosdiez/alt_tab_helper

";

            HookManager.EnsureSubscribedToGlobalKeyboardEvents();

        }

        private void labelDescription_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/marcosdiez/alt_tab_helper");
        }
    }
}
