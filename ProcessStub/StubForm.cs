namespace NetStub
{
    using System;
    using System.Drawing;
    using System.Net;
    using System.Net.Sockets;
    using System.Windows.Forms;
    using RTCV.Common;
    using RTCV.NetCore;
    using RTCV.UI;
    using Vanguard;

    public partial class StubForm : Form
    {
        private Point originalLbTargetLocation;

        private Size originalLbTargetSize;

        public string localIP;
        public int localPort;
        public StubForm()
        {
            InitializeComponent();
            //thx https://stackoverflow.com/questions/6803073/get-local-ip-address
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
                localPort = endPoint.Port;
                //socket.Disconnect(true);
            }

            SyncObjectSingleton.SyncObject = this;
            label7.Text = localIP;
            
            Text += " " + Hook.NetStubVersion;

            if (!Params.IsParamSet("DISCLAIMERREAD"))
            {
                var disclaimer = $@"Welcome to Net Stub
Version {Hook.NetStubVersion}.

Read the enclosed instruction book... readme to see how to use this thing.

Disclaimer:
This program comes with absolutely ZERO warranty.
You may use it at your own risk.
Be EXTREMELY careful with what you choose to corrupt.
Be aware there is always the chance of damage.

This program inserts random data in hooked processes. There is no way to accurately predict what can happen out of this.
The developers of this software will not be held responsible for any damage caused
as the result of use of this software.

By clicking 'Yes' you agree that you have read this warning in full and are aware of any potential consequences of use of the program. If you do not agree, click 'No' to exit this software.";
                if (MessageBox.Show(disclaimer, "Net Stub", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    Environment.Exit(0);

                Params.SetParam("DISCLAIMERREAD");
            }
        }

        private void StubForm_Load(object sender, EventArgs e)
        {
            Colors.SetRTCColor(Color.FromArgb(149, 120, 161), this);

            Focus();

        }

        //public void RunProgressBar(string progressLabel, int maxProgress, Action<object, EventArgs> action, Action<object, EventArgs> postAction = null)
        //{
        //    if (ProcessWatch.progressForm != null)
        //    {
        //        ProcessWatch.progressForm.Close();
        //        Controls.Remove(ProcessWatch.progressForm);
        //        ProcessWatch.progressForm = null;
        //    }

        //    ProcessWatch.progressForm = new ProgressForm(progressLabel, maxProgress, action, postAction);
        //    ProcessWatch.progressForm.Run();
        //}

        //public void EnableTargetInterface()
        //{
        //    var diff = lbTarget.Location.X - btnBrowseTarget.Location.X;
        //    originalLbTargetLocation = lbTarget.Location;
        //    lbTarget.Location = btnBrowseTarget.Location;
        //    lbTarget.Visible = true;

        //    btnBrowseTarget.Visible = false;
        //    originalLbTargetSize = lbTarget.Size;
        //    lbTarget.Size = new Size(lbTarget.Size.Width + diff, lbTarget.Size.Height);
        //    btnUnloadTarget.Visible = true;
        //    btnRefreshDomains.Visible = true;

        //    ProcessWatch.EnableInterface();
        //}

        //public void DisableTargetInterface()
        //{
        //    btnUnloadTarget.Visible = false;
        //    btnRefreshDomains.Visible = false;
        //    btnBrowseTarget.Visible = true;

        //    lbTarget.Size = originalLbTargetSize;
        //    lbTarget.Location = originalLbTargetLocation;
        //    lbTarget.Visible = false;

        //    lbTarget.Text = "No target selected";
        //    lbTargetStatus.Text = "No target selected";
        //}

        //private void BtnBrowseTarget_Click(object sender, EventArgs e)
        //{
        //    if (!ProcessWatch.LoadTarget())
        //        return;

        //    if (!VanguardCore.vanguardConnected)
        //        VanguardCore.Start();

        //    EnableTargetInterface();
        //}

        //private void BtnReleaseTarget_Click(object sender, EventArgs e)
        //{
        //    if (!ProcessWatch.CloseTarget())
        //        return;
        //    DisableTargetInterface();
        //}

        //private void StubForm_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    if (!ProcessWatch.CloseTarget(false))
        //        e.Cancel = true;
        //}

        //private void BtnTargetSettings_MouseDown(object sender, MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Left)
        //    {
        //        Control c = (Control)sender;
        //        Point locate = new Point(c.Location.X + e.Location.X, ((Control)sender).Location.Y + e.Location.Y);

        //        ContextMenuStrip columnsMenu = new ContextMenuStrip();

        //        ((ToolStripMenuItem)columnsMenu.Items.Add("Use AutoHook", null, (ob, ev) =>
        //        {
        //            ProcessWatch.AutoHookTimer.Enabled = !ProcessWatch.AutoHookTimer.Enabled;
        //            tbAutoAttach.Enabled = ProcessWatch.AutoHookTimer.Enabled;
        //        })).Checked = ProcessWatch.AutoHookTimer.Enabled;

        //        ((ToolStripMenuItem)columnsMenu.Items.Add("Use Filtering", null, (ob, ev) =>
        //        {
        //            ProcessWatch.UseFiltering = !ProcessWatch.UseFiltering;
        //            Params.SetParam("USEFILTERING", ProcessWatch.UseFiltering.ToString());

        //            if (VanguardCore.vanguardConnected)
        //                ProcessWatch.UpdateDomains();
        //        })).Checked = ProcessWatch.UseFiltering;

        //        /*
        //        ((ToolStripMenuItem)columnsMenu.Items.Add("Use Exception Handler Override", null, (ob, ev) =>
        //        {

        //            ProcessWatch.UseExceptionHandler = !ProcessWatch.UseExceptionHandler;
        //            Params.SetParam("USEEXCEPTIONHANDLER", ProcessWatch.UseExceptionHandler.ToString());


        //        })).Checked = ProcessWatch.UseExceptionHandler;
        //        */
        //        ((ToolStripMenuItem)columnsMenu.Items.Add("Use Blacklist", null, (ob, ev) =>
        //        {
        //            ProcessWatch.UseBlacklist = !ProcessWatch.UseBlacklist;
        //            Params.SetParam("USEBLACKLIST", ProcessWatch.UseBlacklist.ToString());
        //        })).Checked = ProcessWatch.UseBlacklist;
        //        ((ToolStripMenuItem)columnsMenu.Items.Add("Suspend Process on Corrupt", null, (ob, ev) =>
        //        {
        //            ProcessWatch.SuspendProcess = !ProcessWatch.SuspendProcess;
        //            Params.SetParam("SUSPENDPROCESS", ProcessWatch.SuspendProcess.ToString());
        //        })).Checked = ProcessWatch.SuspendProcess;

        //        columnsMenu.Items.Add(new ToolStripSeparator());
        //        columnsMenu.Items.Add("Select Memory Protection Modes to Corrupt", null, (ob, ev) =>
        //        {
        //            S.GET<MemoryProtectionSelector>().ShowDialog();
        //        });

        //        columnsMenu.Show(this, locate);
        //    }
        //}

        private void BtnRefreshDomains_Click(object sender, EventArgs e)
        {
            if (VanguardCore.vanguardConnected)
                Hook.UpdateDomains();
        }

        private void connect_Click(object sender, EventArgs e)
        {
        }

        private void tbClientAddr_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbClientPort_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            Connector.InitializeConnector();
            btnStartClient.Visible = true;

        }

        private void label7_Click(object sender, EventArgs e)
        {
            Connector.SendMessage("test");
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Hook.Start();
            VanguardCore.Start();
        }

        private void btnStartClient_Click(object sender, EventArgs e)
        {
            Hook.Start();
            VanguardCore.Start();
            btnRefreshDomains.Visible = true;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void placeholderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (placeholderComboBox.SelectedItem.Equals("Linux machine"))
            {
                btnConnect.Visible = true;
                label3.Visible = true;
                tbClientAddr.Visible = true;
                Hook.NetStubMode = "Linux";
            }

            if (placeholderComboBox.SelectedItem.Equals("PS3 (WEBMAN MOD)"))
            {
                btnConnect.Visible = true;
                label3.Visible = true;
                tbClientAddr.Visible = true;
                ps3_ProcCheckBox.Visible = true;
                Hook.NetStubMode = "PS3";
            }

        }

        private void StubForm_Load_1(object sender, EventArgs e)
        {

        }

        private void ps3_ProcCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ps3_ProcCheckBox.Checked == true)
                Hook.PS3_AccessProcess = true;
            else Hook.PS3_AccessProcess = false;
        }


        //private void tbAutoAttach_TextChanged(object sender, EventArgs e)
        //{

        //}

        //private void label3_Click(object sender, EventArgs e)
        //{

        //}

        //private void btnTargetSettings_Click(object sender, EventArgs e)
        //{

        //}

        //private void label4_Click(object sender, EventArgs e)
        //{

        //}
    }
}
