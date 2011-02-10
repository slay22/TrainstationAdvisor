using System;
using OpenNETCF.WindowsCE;
namespace TrainStationAdvisor
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

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

            DeviceManagement.ACPowerApplied -= DeviceManagement_ACPowerApplied;
            DeviceManagement.ACPowerRemoved -= DeviceManagement_ACPowerRemoved;

            if (null != m_SMSSender)
            {
                m_SMSSender.SMSCompleted -= SMSSender_SMSCompleted;
                m_SMSSender = null;

                Logger.Log("SMSSender disposed");
            }

            if (null != m_CellIdCollector)
            {
                m_CellIdCollector.Stop();
                m_CellIdCollector.CellIdFound -= CellIdCollector_CellIdFound;
                m_CellIdCollector.Dispose();
                m_CellIdCollector = null;

                Logger.Log("CellIdCollector disposed");
            }

            if (null != m_Gps)
            {
                //m_Gps.Close();
                m_Gps.LocationChanged -= Gps_LocationChanged;
                m_Gps.Dispose();
                m_Gps = null;

                Logger.Log("GPS disposed");
            }

            if (null != m_Options)
            {
                m_Options.UseGps -= chkUseGps_Click;
                m_Options.RouteChanged -= Options_RouteChanged;
                m_Options.DebugChanged -= Options_DebugChanged;
                m_Options.Dispose();
                m_Options = null;

                Logger.Log("Options Form disposed");
            }

            //m_ItemsControl.Dispose();
            //WindowlessHost.Dispose();

            //if (null != m_Vibrate)
            //{
            //    m_Vibrate = null;
            //}

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.mnuOptions = new System.Windows.Forms.MenuItem();
            this.mnuAbout = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.WindowlessHost = new WindowlessControls.VerticalStackPanelHost();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItem1);
            this.mainMenu1.MenuItems.Add(this.menuItem2);
            // 
            // menuItem1
            // 
            this.menuItem1.Text = "Start";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.MenuItems.Add(this.mnuOptions);
            this.menuItem2.MenuItems.Add(this.mnuAbout);
            this.menuItem2.MenuItems.Add(this.menuItem4);
            this.menuItem2.MenuItems.Add(this.menuItem3);
            this.menuItem2.Text = "Menu";
            // 
            // mnuOptions
            // 
            this.mnuOptions.Text = "Options...";
            this.mnuOptions.Click += new System.EventHandler(this.mnuOptions_Click);
            // 
            // mnuAbout
            // 
            this.mnuAbout.Text = "About";
            this.mnuAbout.Click += new System.EventHandler(this.mnuAbout_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Text = "-";
            // 
            // menuItem3
            // 
            this.menuItem3.Text = "Exit";
            this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click);
            // 
            // txtStatus
            // 
            this.txtStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtStatus.Location = new System.Drawing.Point(0, 223);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Size = new System.Drawing.Size(240, 45);
            this.txtStatus.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(240, 223);
            // 
            // WindowlessHost
            // 
            this.WindowlessHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WindowlessHost.Location = new System.Drawing.Point(0, 0);
            this.WindowlessHost.Name = "WindowlessHost";
            this.WindowlessHost.Size = new System.Drawing.Size(240, 223);
            this.WindowlessHost.TabIndex = 0;
            this.WindowlessHost.Text = "verticalStackPanelHost1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.WindowlessHost);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtStatus);
            this.Menu = this.mainMenu1;
            this.Name = "MainForm";
            this.Text = "Train Station Advisor";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
            this.ResumeLayout(false);

        }

        #endregion

        private WindowlessControls.VerticalStackPanelHost WindowlessHost;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.MenuItem mnuOptions;
        private System.Windows.Forms.MenuItem mnuAbout;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem menuItem4;
    }
}

