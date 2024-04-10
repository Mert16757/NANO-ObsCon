namespace ASCOM.NANO.ObservingConditions
{
    partial class SetupDialogForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupDialogForm));
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.picASCOM = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkTrace = new System.Windows.Forms.CheckBox();
            this.comboBoxComPort = new System.Windows.Forms.ComboBox();
            this.FWHMtextBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.BatchtextBox1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Batchfile = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.LogfiletextBox1 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.LogfilecheckBox1 = new System.Windows.Forms.CheckBox();
            this.UpdatetextBox1 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.CorrtextBox1 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picASCOM)).BeginInit();
            this.SuspendLayout();
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(472, 322);
            this.cmdOK.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(79, 30);
            this.cmdOK.TabIndex = 0;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.CmdOK_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(472, 359);
            this.cmdCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(79, 31);
            this.cmdCancel.TabIndex = 1;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.CmdCancel_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(149, -2);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(164, 63);
            this.label1.TabIndex = 2;
            this.label1.Text = "Setup parameters for NANO ObsCon";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picASCOM
            // 
            this.picASCOM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picASCOM.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picASCOM.Image = ((System.Drawing.Image)(resources.GetObject("picASCOM.Image")));
            this.picASCOM.Location = new System.Drawing.Point(487, 11);
            this.picASCOM.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.picASCOM.Name = "picASCOM";
            this.picASCOM.Size = new System.Drawing.Size(48, 56);
            this.picASCOM.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picASCOM.TabIndex = 3;
            this.picASCOM.TabStop = false;
            this.picASCOM.Click += new System.EventHandler(this.BrowseToAscom);
            this.picASCOM.DoubleClick += new System.EventHandler(this.BrowseToAscom);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(209, 81);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "Comm Port";
            // 
            // chkTrace
            // 
            this.chkTrace.AutoSize = true;
            this.chkTrace.Location = new System.Drawing.Point(24, 113);
            this.chkTrace.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkTrace.Name = "chkTrace";
            this.chkTrace.Size = new System.Drawing.Size(80, 20);
            this.chkTrace.TabIndex = 6;
            this.chkTrace.Text = "Trace on";
            this.chkTrace.UseVisualStyleBackColor = true;
            this.chkTrace.CheckedChanged += new System.EventHandler(this.chkTrace_CheckedChanged);
            // 
            // comboBoxComPort
            // 
            this.comboBoxComPort.FormattingEnabled = true;
            this.comboBoxComPort.Location = new System.Drawing.Point(24, 78);
            this.comboBoxComPort.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.comboBoxComPort.Name = "comboBoxComPort";
            this.comboBoxComPort.Size = new System.Drawing.Size(176, 24);
            this.comboBoxComPort.TabIndex = 7;
            this.comboBoxComPort.SelectedIndexChanged += new System.EventHandler(this.comboBoxComPort_SelectedIndexChanged);
            // 
            // FWHMtextBox1
            // 
            this.FWHMtextBox1.Location = new System.Drawing.Point(24, 144);
            this.FWHMtextBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.FWHMtextBox1.Name = "FWHMtextBox1";
            this.FWHMtextBox1.Size = new System.Drawing.Size(279, 22);
            this.FWHMtextBox1.TabIndex = 8;
            this.FWHMtextBox1.TextChanged += new System.EventHandler(this.FWHMtextBox1_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(312, 146);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 16);
            this.label3.TabIndex = 9;
            this.label3.Text = "Path to FWHM frames";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // BatchtextBox1
            // 
            this.BatchtextBox1.Location = new System.Drawing.Point(24, 183);
            this.BatchtextBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BatchtextBox1.Name = "BatchtextBox1";
            this.BatchtextBox1.Size = new System.Drawing.Size(279, 22);
            this.BatchtextBox1.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(312, 186);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(152, 16);
            this.label4.TabIndex = 11;
            this.label4.Text = "Path to Batchfile / Logfile";
            // 
            // Batchfile
            // 
            this.Batchfile.Location = new System.Drawing.Point(24, 219);
            this.Batchfile.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Batchfile.Name = "Batchfile";
            this.Batchfile.Size = new System.Drawing.Size(176, 22);
            this.Batchfile.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(215, 223);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(112, 16);
            this.label5.TabIndex = 13;
            this.label5.Text = "Name of Batchfile";
            // 
            // LogfiletextBox1
            // 
            this.LogfiletextBox1.Location = new System.Drawing.Point(24, 257);
            this.LogfiletextBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.LogfiletextBox1.Name = "LogfiletextBox1";
            this.LogfiletextBox1.Size = new System.Drawing.Size(176, 22);
            this.LogfiletextBox1.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(215, 262);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(150, 16);
            this.label6.TabIndex = 15;
            this.label6.Text = "Name of Sensor  Logfile";
            // 
            // LogfilecheckBox1
            // 
            this.LogfilecheckBox1.AutoSize = true;
            this.LogfilecheckBox1.Location = new System.Drawing.Point(124, 113);
            this.LogfilecheckBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.LogfilecheckBox1.Name = "LogfilecheckBox1";
            this.LogfilecheckBox1.Size = new System.Drawing.Size(112, 20);
            this.LogfilecheckBox1.TabIndex = 16;
            this.LogfilecheckBox1.Text = "Sensor Logfile";
            this.LogfilecheckBox1.UseVisualStyleBackColor = true;
            this.LogfilecheckBox1.CheckedChanged += new System.EventHandler(this.LogfilecheckBox1_CheckedChanged);
            // 
            // UpdatetextBox1
            // 
            this.UpdatetextBox1.Location = new System.Drawing.Point(153, 293);
            this.UpdatetextBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.UpdatetextBox1.Name = "UpdatetextBox1";
            this.UpdatetextBox1.Size = new System.Drawing.Size(48, 22);
            this.UpdatetextBox1.TabIndex = 17;
            this.UpdatetextBox1.TextChanged += new System.EventHandler(this.UpdatetextBox1_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(216, 298);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(150, 16);
            this.label7.TabIndex = 18;
            this.label7.Text = "Sample time in seconds";
            // 
            // CorrtextBox1
            // 
            this.CorrtextBox1.Location = new System.Drawing.Point(152, 330);
            this.CorrtextBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.CorrtextBox1.Name = "CorrtextBox1";
            this.CorrtextBox1.Size = new System.Drawing.Size(48, 22);
            this.CorrtextBox1.TabIndex = 19;
            this.CorrtextBox1.TextChanged += new System.EventHandler(this.CorrtextBox1_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(219, 333);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(191, 16);
            this.label8.TabIndex = 20;
            this.label8.Text = "Correction factor FWHM-scope\r\n";
            // 
            // SetupDialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(564, 400);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.CorrtextBox1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.UpdatetextBox1);
            this.Controls.Add(this.LogfilecheckBox1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.LogfiletextBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.Batchfile);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.BatchtextBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.FWHMtextBox1);
            this.Controls.Add(this.comboBoxComPort);
            this.Controls.Add(this.chkTrace);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.picASCOM);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetupDialogForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NANO Setup";
            this.Load += new System.EventHandler(this.SetupDialogForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picASCOM)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox picASCOM;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkTrace;
        private System.Windows.Forms.ComboBox comboBoxComPort;
        private System.Windows.Forms.TextBox FWHMtextBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox BatchtextBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox Batchfile;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox LogfiletextBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox LogfilecheckBox1;
        private System.Windows.Forms.TextBox UpdatetextBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox CorrtextBox1;
        private System.Windows.Forms.Label label8;
    }
}