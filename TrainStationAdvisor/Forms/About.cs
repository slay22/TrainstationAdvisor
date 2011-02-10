using System;
using System.Windows.Forms;

namespace TrainStationAdvisor
{
    public partial class About : Form
    {
        string applicationName = "";
        string version = "";

        string applicationDescription = "";
        string author = "";
        string contactInfo = "";

        public About()
        {
            InitializeComponent();
        }

        public string ApplicationName
        {
            set { applicationName = value; }
        }

        public string Version
        {
            set { version = value; }
        }

        public string ApplicationDescription
        {
            set { applicationDescription = value; }
        }

        public string Author
        {
            set { author = value; }
        }

        public string ContactInfo
        {
            set { contactInfo = value; }
        }

        private void About_KeyDown(object sender, KeyEventArgs e)
        {   
            if ((e.KeyCode == Keys.Enter))
            {
                Close();
            }
        }

        private void About_Load(object sender, EventArgs e)
        {
            appNameLabel.Text = applicationName;
            versionLabel.Text = version;

            descriptionTextBox.Text = string.Format("{0} \r\n Written by {1} ({2})",
                applicationDescription, author, contactInfo);

            Size = Screen.PrimaryScreen.WorkingArea.Size;
        }

        private void okMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}