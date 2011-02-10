using TrainStationAdvisor.ClassLibrary;

namespace TrainStationAdvisor
{
    partial class Options
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

            try
            {
                if (null != m_OutlookSession)
                {
                    m_OutlookSession.Dispose();
                    m_OutlookSession = null;
                }

                if (null != m_Storage)
                {
                    m_Storage.Dispose();
                    m_Storage = null;
                }

                Settings.Update();
            }
            catch { };

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.senseListCtrlOptions = new StedySoft.SenseSDK.SenseListControl();
            this.senseListCtrlAlert = new StedySoft.SenseSDK.SenseListControl();
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.chkAlwaysOn = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.chkSendSMS = new System.Windows.Forms.CheckBox();
            this.cboPhoneNr = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.checkTrackTraceActive = new System.Windows.Forms.CheckBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // senseListCtrlOptions
            // 
            this.senseListCtrlOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.senseListCtrlOptions.FocusedItem = null;
            this.senseListCtrlOptions.IsSecondaryScrollType = false;
            this.senseListCtrlOptions.Location = new System.Drawing.Point(0, 0);
            this.senseListCtrlOptions.MinimumMovement = 15;
            this.senseListCtrlOptions.Name = "senseListCtrlOptions";
            this.senseListCtrlOptions.ShowScrollIndicator = true;
            this.senseListCtrlOptions.Size = new System.Drawing.Size(240, 245);
            this.senseListCtrlOptions.Springback = 0.35F;
            this.senseListCtrlOptions.TabIndex = 29;
            this.senseListCtrlOptions.ThreadSleep = 100;
            this.senseListCtrlOptions.TopIndex = 0;
            this.senseListCtrlOptions.Velocity = 0.9F;
            // 
            // senseListCtrlAlert
            // 
            this.senseListCtrlAlert.Dock = System.Windows.Forms.DockStyle.Fill;
            this.senseListCtrlAlert.FocusedItem = null;
            this.senseListCtrlAlert.IsSecondaryScrollType = false;
            this.senseListCtrlAlert.Location = new System.Drawing.Point(0, 0);
            this.senseListCtrlAlert.MinimumMovement = 15;
            this.senseListCtrlAlert.Name = "senseListCtrlAlert";
            this.senseListCtrlAlert.ShowScrollIndicator = true;
            this.senseListCtrlAlert.Size = new System.Drawing.Size(240, 245);
            this.senseListCtrlAlert.Springback = 0.35F;
            this.senseListCtrlAlert.TabIndex = 21;
            this.senseListCtrlAlert.ThreadSleep = 100;
            this.senseListCtrlAlert.TopIndex = 0;
            this.senseListCtrlAlert.Velocity = 0.9F;
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItem1);
            // 
            // menuItem1
            // 
            this.menuItem1.Text = "Ok";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(240, 268);
            this.tabControl1.TabIndex = 15;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.senseListCtrlAlert);
            this.tabPage1.Location = new System.Drawing.Point(0, 0);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(240, 245);
            this.tabPage1.Text = "Alert";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.chkAlwaysOn);
            this.tabPage4.Controls.Add(this.senseListCtrlOptions);
            this.tabPage4.Location = new System.Drawing.Point(0, 0);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(240, 245);
            this.tabPage4.Text = "Options";
            // 
            // chkAlwaysOn
            // 
            this.chkAlwaysOn.Location = new System.Drawing.Point(3, 159);
            this.chkAlwaysOn.Name = "chkAlwaysOn";
            this.chkAlwaysOn.Size = new System.Drawing.Size(155, 15);
            this.chkAlwaysOn.TabIndex = 25;
            this.chkAlwaysOn.Text = "Always On";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.chkSendSMS);
            this.tabPage2.Controls.Add(this.cboPhoneNr);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Location = new System.Drawing.Point(0, 0);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(232, 242);
            this.tabPage2.Text = "Extended Alert";
            // 
            // chkSendSMS
            // 
            this.chkSendSMS.Location = new System.Drawing.Point(35, 44);
            this.chkSendSMS.Name = "chkSendSMS";
            this.chkSendSMS.Size = new System.Drawing.Size(155, 15);
            this.chkSendSMS.TabIndex = 20;
            this.chkSendSMS.Text = "Send SMS with Alert";
            this.chkSendSMS.Click += new System.EventHandler(this.chkSendSMS_Click);
            // 
            // cboPhoneNr
            // 
            this.cboPhoneNr.Location = new System.Drawing.Point(97, 7);
            this.cboPhoneNr.Name = "cboPhoneNr";
            this.cboPhoneNr.Size = new System.Drawing.Size(140, 22);
            this.cboPhoneNr.TabIndex = 18;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(4, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 21);
            this.label1.Text = "Phone Nr";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.checkTrackTraceActive);
            this.tabPage3.Controls.Add(this.txtPort);
            this.tabPage3.Controls.Add(this.label3);
            this.tabPage3.Controls.Add(this.txtIP);
            this.tabPage3.Controls.Add(this.label2);
            this.tabPage3.Location = new System.Drawing.Point(0, 0);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(232, 242);
            this.tabPage3.Text = "Server Options";
            // 
            // checkTrackTraceActive
            // 
            this.checkTrackTraceActive.Location = new System.Drawing.Point(7, 75);
            this.checkTrackTraceActive.Name = "checkTrackTraceActive";
            this.checkTrackTraceActive.Size = new System.Drawing.Size(226, 15);
            this.checkTrackTraceActive.TabIndex = 23;
            this.checkTrackTraceActive.Text = "Send Current Positions to Server";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(67, 36);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(157, 21);
            this.txtPort.TabIndex = 3;
            this.txtPort.Text = "1775";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(9, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 18);
            this.label3.Text = "Port";
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(67, 7);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(157, 21);
            this.txtIP.TabIndex = 1;
            this.txtIP.Text = "fliagutierrez.hopto.org";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(9, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 18);
            this.label2.Text = "IP";
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.tabControl1);
            this.Menu = this.mainMenu1;
            this.Name = "Options";
            this.Text = "Options";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private StedySoft.SenseSDK.SenseListControl senseListCtrlOptions;
        private StedySoft.SenseSDK.SenseListControl senseListCtrlAlert;

        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.CheckBox chkAlwaysOn;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckBox chkSendSMS;
        private System.Windows.Forms.ComboBox cboPhoneNr;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.CheckBox checkTrackTraceActive;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Label label2;
    }
}