namespace CxFinder
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label3;
            this.niComboBox = new System.Windows.Forms.ComboBox();
            this.ethCheckBox = new System.Windows.Forms.CheckBox();
            this.wlanCheckBox = new System.Windows.Forms.CheckBox();
            this.lbCheckBox = new System.Windows.Forms.CheckBox();
            this.searchButton = new System.Windows.Forms.Button();
            this.cxListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mainStatusStrip = new System.Windows.Forms.StatusStrip();
            this.statusStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.searchStatusProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.searchProgressTimer = new System.Windows.Forms.Timer(this.components);
            this.timeoutNumericUpDown = new System.Windows.Forms.NumericUpDown();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            this.mainStatusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timeoutNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 17);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(95, 13);
            label1.TabIndex = 0;
            label1.Text = "Network Interface:";
            // 
            // niComboBox
            // 
            this.niComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.niComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.niComboBox.FormattingEnabled = true;
            this.niComboBox.Location = new System.Drawing.Point(113, 14);
            this.niComboBox.Name = "niComboBox";
            this.niComboBox.Size = new System.Drawing.Size(339, 21);
            this.niComboBox.TabIndex = 1;
            // 
            // ethCheckBox
            // 
            this.ethCheckBox.AutoSize = true;
            this.ethCheckBox.Checked = true;
            this.ethCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ethCheckBox.Location = new System.Drawing.Point(113, 41);
            this.ethCheckBox.Name = "ethCheckBox";
            this.ethCheckBox.Size = new System.Drawing.Size(66, 17);
            this.ethCheckBox.TabIndex = 2;
            this.ethCheckBox.Text = "Ethernet";
            this.ethCheckBox.UseVisualStyleBackColor = true;
            this.ethCheckBox.CheckedChanged += new System.EventHandler(this.NiCheckBoxes_CheckedChanged);
            // 
            // wlanCheckBox
            // 
            this.wlanCheckBox.AutoSize = true;
            this.wlanCheckBox.Checked = true;
            this.wlanCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.wlanCheckBox.Location = new System.Drawing.Point(185, 41);
            this.wlanCheckBox.Name = "wlanCheckBox";
            this.wlanCheckBox.Size = new System.Drawing.Size(66, 17);
            this.wlanCheckBox.TabIndex = 2;
            this.wlanCheckBox.Text = "Wireless";
            this.wlanCheckBox.UseVisualStyleBackColor = true;
            this.wlanCheckBox.CheckedChanged += new System.EventHandler(this.NiCheckBoxes_CheckedChanged);
            // 
            // lbCheckBox
            // 
            this.lbCheckBox.AutoSize = true;
            this.lbCheckBox.Location = new System.Drawing.Point(257, 41);
            this.lbCheckBox.Name = "lbCheckBox";
            this.lbCheckBox.Size = new System.Drawing.Size(74, 17);
            this.lbCheckBox.TabIndex = 3;
            this.lbCheckBox.Text = "Loopback";
            this.lbCheckBox.UseVisualStyleBackColor = true;
            this.lbCheckBox.CheckedChanged += new System.EventHandler(this.NiCheckBoxes_CheckedChanged);
            // 
            // searchButton
            // 
            this.searchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.searchButton.Location = new System.Drawing.Point(458, 12);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(126, 23);
            this.searchButton.TabIndex = 4;
            this.searchButton.Text = "Broadcast Search";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // cxListView
            // 
            this.cxListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cxListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7});
            this.cxListView.FullRowSelect = true;
            this.cxListView.Location = new System.Drawing.Point(15, 64);
            this.cxListView.Name = "cxListView";
            this.cxListView.Size = new System.Drawing.Size(437, 206);
            this.cxListView.TabIndex = 5;
            this.cxListView.UseCompatibleStateImageBehavior = false;
            this.cxListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 110;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "IP Address";
            this.columnHeader2.Width = 110;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "AMS NetId";
            this.columnHeader3.Width = 110;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "TwinCAT";
            this.columnHeader5.Width = 110;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "OS Version";
            this.columnHeader6.Width = 110;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Comment";
            this.columnHeader7.Width = 200;
            // 
            // mainStatusStrip
            // 
            this.mainStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusStatusLabel,
            this.searchStatusProgressBar});
            this.mainStatusStrip.Location = new System.Drawing.Point(0, 285);
            this.mainStatusStrip.Name = "mainStatusStrip";
            this.mainStatusStrip.Size = new System.Drawing.Size(596, 25);
            this.mainStatusStrip.TabIndex = 6;
            this.mainStatusStrip.Text = "statusStrip1";
            // 
            // statusStatusLabel
            // 
            this.statusStatusLabel.Name = "statusStatusLabel";
            this.statusStatusLabel.Size = new System.Drawing.Size(50, 20);
            this.statusStatusLabel.Text = "Ready";
            // 
            // searchStatusProgressBar
            // 
            this.searchStatusProgressBar.Name = "searchStatusProgressBar";
            this.searchStatusProgressBar.Size = new System.Drawing.Size(300, 19);
            this.searchStatusProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // searchProgressTimer
            // 
            this.searchProgressTimer.Interval = 1000;
            this.searchProgressTimer.Tick += new System.EventHandler(this.searchProgressTimer_Tick);
            // 
            // label2
            // 
            label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(455, 43);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(45, 13);
            label2.TabIndex = 7;
            label2.Text = "Timeout";
            // 
            // timeoutNumericUpDown
            // 
            this.timeoutNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.timeoutNumericUpDown.Location = new System.Drawing.Point(506, 41);
            this.timeoutNumericUpDown.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.timeoutNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.timeoutNumericUpDown.Name = "timeoutNumericUpDown";
            this.timeoutNumericUpDown.Size = new System.Drawing.Size(60, 20);
            this.timeoutNumericUpDown.TabIndex = 8;
            this.timeoutNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label3
            // 
            label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(572, 43);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(12, 13);
            label3.TabIndex = 7;
            label3.Text = "s";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(596, 310);
            this.Controls.Add(this.timeoutNumericUpDown);
            this.Controls.Add(label3);
            this.Controls.Add(label2);
            this.Controls.Add(this.mainStatusStrip);
            this.Controls.Add(this.cxListView);
            this.Controls.Add(this.searchButton);
            this.Controls.Add(this.lbCheckBox);
            this.Controls.Add(this.wlanCheckBox);
            this.Controls.Add(this.ethCheckBox);
            this.Controls.Add(this.niComboBox);
            this.Controls.Add(label1);
            this.MinimumSize = new System.Drawing.Size(600, 300);
            this.Name = "MainForm";
            this.Text = "CxFinder";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.mainStatusStrip.ResumeLayout(false);
            this.mainStatusStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timeoutNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox niComboBox;
        private System.Windows.Forms.CheckBox ethCheckBox;
        private System.Windows.Forms.CheckBox wlanCheckBox;
        private System.Windows.Forms.CheckBox lbCheckBox;
        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.ListView cxListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ToolStripStatusLabel statusStatusLabel;
        private System.Windows.Forms.ToolStripProgressBar searchStatusProgressBar;
        private System.Windows.Forms.StatusStrip mainStatusStrip;
        private System.Windows.Forms.Timer searchProgressTimer;
        private System.Windows.Forms.NumericUpDown timeoutNumericUpDown;
    }
}

