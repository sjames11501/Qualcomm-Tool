namespace QCOH
{
    partial class dlgSelectPartition
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
            this.label1 = new System.Windows.Forms.Label();
            this.cmbPartionName = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Partition Name:";
            // 
            // cmbPartionName
            // 
            this.cmbPartionName.FormattingEnabled = true;
            this.cmbPartionName.Location = new System.Drawing.Point(97, 12);
            this.cmbPartionName.Name = "cmbPartionName";
            this.cmbPartionName.Size = new System.Drawing.Size(377, 21);
            this.cmbPartionName.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(97, 39);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(377, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Sumbit";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dlgSelectPartition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(488, 76);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cmbPartionName);
            this.Controls.Add(this.label1);
            this.Name = "dlgSelectPartition";
            this.Text = "Select Partition";
            this.Load += new System.EventHandler(this.dlgSelectPartition_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox cmbPartionName;
        private System.Windows.Forms.Button button1;
    }
}