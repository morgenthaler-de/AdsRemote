using AdsRemote;
using AdsRemote.Router;
using CxFinder.ui;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Windows.Forms;

namespace CxFinder
{
    public partial class MainForm : Form
    {
        const string APP_NAME = "CxFinder";
        const string STATUS_READY = "Ready";
        const string STATUS_SEARCHING = "Searching...";

        public MainForm()
        {
            InitializeComponent();
        }

        private string AppShortVersion
        {
            get
            {
                Version v = Assembly.GetExecutingAssembly().GetName().Version;
                return string.Concat(v.Major, ".", v.Minor); 
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Text = APP_NAME + AppShortVersion;

            FormNiCombo();
            statusStatusLabel.Text = STATUS_READY;
        }

        private void FormNiCombo()
        {
            searchButton.Enabled = false;

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (wlanCheckBox.Checked || ethCheckBox.Checked || lbCheckBox.Checked)
                {
                    if (!((ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) && wlanCheckBox.Checked ||
                        (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet) && ethCheckBox.Checked ||
                        (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) && lbCheckBox.Checked))
                    {
                        continue;
                    }
                }

                foreach (UnicastIPAddressInformation unicastInfo in ni.GetIPProperties().UnicastAddresses)
                {
                    if (unicastInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ComboBoxItem item = new ComboBoxItem();
                        item.Text = string.Concat(ni.Name, " (", unicastInfo.Address.ToString(), ")");
                        item.Value = unicastInfo.Address;
                        niComboBox.Items.Add(item);
                    }
                }
            }

            if (niComboBox.Items.Count > 0)
            {
                searchButton.Enabled = true;
                niComboBox.SelectedIndex = 0;
            }
        }

        private void NiCheckBoxes_CheckedChanged(object sender, EventArgs e)
        {
            niComboBox.Items.Clear();
            FormNiCombo();
        }

        private async void searchButton_Click(object sender, EventArgs e)
        {
            if (niComboBox.SelectedIndex < 0)
                return;

            statusStatusLabel.Text = STATUS_SEARCHING;

            int timeout = (int)timeoutNumericUpDown.Value;
            searchStatusProgressBar.Maximum = timeout;

            searchStatusProgressBar.Value = 0;
            searchStatusProgressBar.Visible = true;
            searchProgressTimer.Enabled = true;

            cxListView.Enabled = false;
            searchButton.Enabled = timeoutNumericUpDown.Enabled = false;
            niComboBox.Enabled = ethCheckBox.Enabled = wlanCheckBox.Enabled = lbCheckBox.Enabled = false;

            ComboBoxItem si = (ComboBoxItem)niComboBox.SelectedItem;            
            List<RemotePlcInfo> di =
                await AmsRouter.BroadcastSearchAsync(
                    (IPAddress)si.Value,
                    timeout * 1000);

            cxListView.Items.Clear();
            foreach(RemotePlcInfo info in di)
            {
                ListViewItem lvi =
                    new ListViewItem(
                        new string[]
                        {
                            info.Name,
                            info.Address.ToString(),
                            info.AmsNetId.ToString(),
                            string.Concat(
                                info.TcVersion.Version.ToString(), ".",
                                info.TcVersion.Revision.ToString(), ".",
                                info.TcVersion.Build.ToString()),
                            info.OsVersion,
                            info.Comment
                        });

                cxListView.Items.Add(lvi);
            }

            cxListView.Enabled = true;
            searchButton.Enabled = timeoutNumericUpDown.Enabled = true;
            niComboBox.Enabled = ethCheckBox.Enabled = wlanCheckBox.Enabled = lbCheckBox.Enabled = true;

            searchStatusProgressBar.Visible = false;
            searchProgressTimer.Enabled = false;

            statusStatusLabel.Text = STATUS_READY;
        }

        private void searchProgressTimer_Tick(object sender, EventArgs e)
        {
            if (searchStatusProgressBar.Value + 1 <= searchStatusProgressBar.Maximum)
                searchStatusProgressBar.Value++;
            else
                searchStatusProgressBar.Value = searchStatusProgressBar.Maximum;
        }
    }
}
