using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AltTabHelperV2
{
    public partial class MainWindow : Form
    {
        public static MainWindow theInstance;

        public MainWindow()
        {
            theInstance = this;
            InitializeComponent();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            listBoxApps.Width = ClientRectangle.Width;
            listBoxApps.Height = ClientRectangle.Height;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Form1_Resize(sender, e);
            PopulateListBox(ProgramManager.GetAppList());

            Hook.EnsureSubscribedToGlobalKeyboardEvents();

        }


        public void PopulateListBox(List<ProgramItem> theList)
        {
            var items = listBoxApps.Items;
            items.Clear();
            foreach (var item in theList)
            {
                items.Add(item);
            }
        }


        private void listBoxApps_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.listBoxApps.IndexFromPoint(e.Location);
            if (index != System.Windows.Forms.ListBox.NoMatches)
            {
                ((ProgramItem)listBoxApps.Items[index]).SetFocus();
                // MessageBox.Show(index.ToString());
            }
        }

        private void listBoxApps_Enter(object sender, EventArgs e)
        {
            PopulateListBox(ProgramManager.GetAppList());
        }

        private void MainWindow_Activated(object sender, EventArgs e)
        {
            PopulateListBox(ProgramManager.GetAppList());
        }
    }
}
