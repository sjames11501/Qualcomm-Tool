namespace QCOH
{
    partial class Main
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
            this.btnConnect = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.fhGroupBox = new System.Windows.Forms.GroupBox();
            this.btnRestart = new System.Windows.Forms.Button();
            this.btnRestore = new System.Windows.Forms.Button();
            this.btnSimLock = new System.Windows.Forms.Button();
            this.btnDumpPartition = new System.Windows.Forms.Button();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.cmbModel = new System.Windows.Forms.ComboBox();
            this.prgBar = new System.Windows.Forms.ProgressBar();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.ckbEraseST = new System.Windows.Forms.CheckBox();
            this.ckbDwnModem = new System.Windows.Forms.CheckBox();
            this.fhGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(605, 42);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(405, 43);
            this.btnConnect.TabIndex = 6;
            this.btnConnect.Text = "CONNECT / SEND FLASH";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // fhGroupBox
            // 
            this.fhGroupBox.Controls.Add(this.btnRestart);
            this.fhGroupBox.Controls.Add(this.btnRestore);
            this.fhGroupBox.Controls.Add(this.btnSimLock);
            this.fhGroupBox.Controls.Add(this.btnDumpPartition);
            this.fhGroupBox.Location = new System.Drawing.Point(605, 172);
            this.fhGroupBox.Name = "fhGroupBox";
            this.fhGroupBox.Size = new System.Drawing.Size(405, 254);
            this.fhGroupBox.TabIndex = 11;
            this.fhGroupBox.TabStop = false;
            this.fhGroupBox.Text = "Firehose Functions";
            // 
            // btnRestart
            // 
            this.btnRestart.Enabled = false;
            this.btnRestart.Location = new System.Drawing.Point(6, 174);
            this.btnRestart.Name = "btnRestart";
            this.btnRestart.Size = new System.Drawing.Size(393, 45);
            this.btnRestart.TabIndex = 4;
            this.btnRestart.Text = "Power Reset";
            this.btnRestart.UseVisualStyleBackColor = true;
            this.btnRestart.Click += new System.EventHandler(this.btnRestart_Click);
            // 
            // btnRestore
            // 
            this.btnRestore.Enabled = false;
            this.btnRestore.Location = new System.Drawing.Point(6, 123);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(393, 45);
            this.btnRestore.TabIndex = 3;
            this.btnRestore.Text = "Restore SIMLOCK";
            this.btnRestore.UseVisualStyleBackColor = true;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // btnSimLock
            // 
            this.btnSimLock.Enabled = false;
            this.btnSimLock.Location = new System.Drawing.Point(6, 72);
            this.btnSimLock.Name = "btnSimLock";
            this.btnSimLock.Size = new System.Drawing.Size(393, 45);
            this.btnSimLock.TabIndex = 2;
            this.btnSimLock.Text = "Write SIMLOCK";
            this.btnSimLock.UseVisualStyleBackColor = true;
            this.btnSimLock.Click += new System.EventHandler(this.btnSimLock_Click);
            // 
            // btnDumpPartition
            // 
            this.btnDumpPartition.Enabled = false;
            this.btnDumpPartition.Location = new System.Drawing.Point(6, 22);
            this.btnDumpPartition.Name = "btnDumpPartition";
            this.btnDumpPartition.Size = new System.Drawing.Size(393, 44);
            this.btnDumpPartition.TabIndex = 1;
            this.btnDumpPartition.Text = "Dump partition to file";
            this.btnDumpPartition.UseVisualStyleBackColor = true;
            this.btnDumpPartition.Click += new System.EventHandler(this.btnDumpPartition_Click);
            // 
            // rtbLog
            // 
            this.rtbLog.BackColor = System.Drawing.Color.White;
            this.rtbLog.Cursor = System.Windows.Forms.Cursors.Default;
            this.rtbLog.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.rtbLog.HideSelection = false;
            this.rtbLog.Location = new System.Drawing.Point(12, 12);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbLog.Size = new System.Drawing.Size(575, 373);
            this.rtbLog.TabIndex = 8;
            this.rtbLog.Text = "";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(68, 17);
            this.toolStripStatusLabel1.Text = "QCOH Beta";
            // 
            // cmbModel
            // 
            this.cmbModel.FormattingEnabled = true;
            this.cmbModel.Items.AddRange(new object[] {
            "Alcatel 4060A"});
            this.cmbModel.Location = new System.Drawing.Point(605, 12);
            this.cmbModel.Name = "cmbModel";
            this.cmbModel.Size = new System.Drawing.Size(405, 24);
            this.cmbModel.TabIndex = 13;
            // 
            // prgBar
            // 
            this.prgBar.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.prgBar.Location = new System.Drawing.Point(12, 391);
            this.prgBar.Name = "prgBar";
            this.prgBar.Size = new System.Drawing.Size(575, 22);
            this.prgBar.TabIndex = 2;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 416);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1028, 22);
            this.statusStrip1.TabIndex = 12;
            this.statusStrip1.Text = "statusStrip";
            // 
            // ckbEraseST
            // 
            this.ckbEraseST.AutoSize = true;
            this.ckbEraseST.Location = new System.Drawing.Point(605, 91);
            this.ckbEraseST.Name = "ckbEraseST";
            this.ckbEraseST.Size = new System.Drawing.Size(95, 20);
            this.ckbEraseST.TabIndex = 2;
            this.ckbEraseST.Text = "Clear EFS ";
            this.ckbEraseST.UseVisualStyleBackColor = true;
            // 
            // ckbDwnModem
            // 
            this.ckbDwnModem.AutoSize = true;
            this.ckbDwnModem.Location = new System.Drawing.Point(605, 117);
            this.ckbDwnModem.Name = "ckbDwnModem";
            this.ckbDwnModem.Size = new System.Drawing.Size(151, 20);
            this.ckbDwnModem.TabIndex = 14;
            this.ckbDwnModem.Text = "Downgrade Modem";
            this.ckbDwnModem.UseVisualStyleBackColor = true;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1028, 438);
            this.Controls.Add(this.ckbDwnModem);
            this.Controls.Add(this.ckbEraseST);
            this.Controls.Add(this.prgBar);
            this.Controls.Add(this.cmbModel);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.rtbLog);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.fhGroupBox);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Main";
            this.Text = "Qualcomm Tool";
            this.UseWaitCursor = true;
            this.Load += new System.EventHandler(this.Main_Load);
            this.fhGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.GroupBox fhGroupBox;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.Button btnDumpPartition;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ComboBox cmbModel;
        private System.Windows.Forms.ProgressBar prgBar;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.CheckBox ckbEraseST;
        private System.Windows.Forms.CheckBox ckbDwnModem;
        private System.Windows.Forms.Button btnSimLock;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.Button btnRestart;
    }
}

