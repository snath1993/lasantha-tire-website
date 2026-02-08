namespace UserAutherization
{
    partial class frmRegSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRegSettings));
            this.txtPWD = new System.Windows.Forms.TextBox();
            this.txtUserID = new System.Windows.Forms.TextBox();
            this.cmbDB = new System.Windows.Forms.ComboBox();
            this.cmbServer = new System.Windows.Forms.ComboBox();
            this.btnReg = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtBU = new System.Windows.Forms.TextBox();
            this.txtDB = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.rdoWindowsAuth = new System.Windows.Forms.RadioButton();
            this.rdoSQLAuthentication = new System.Windows.Forms.RadioButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnmdfbrows = new System.Windows.Forms.Button();
            this.txtldfPath = new System.Windows.Forms.TextBox();
            this.txtmdfPath = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.OFD = new System.Windows.Forms.OpenFileDialog();
            this.btnfinished = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtPWD
            // 
            this.txtPWD.Location = new System.Drawing.Point(155, 159);
            this.txtPWD.Name = "txtPWD";
            this.txtPWD.Size = new System.Drawing.Size(223, 21);
            this.txtPWD.TabIndex = 8;
            this.txtPWD.UseSystemPasswordChar = true;
            this.txtPWD.Leave += new System.EventHandler(this.txtPWD_Leave);
            // 
            // txtUserID
            // 
            this.txtUserID.Location = new System.Drawing.Point(155, 132);
            this.txtUserID.Name = "txtUserID";
            this.txtUserID.Size = new System.Drawing.Size(223, 21);
            this.txtUserID.TabIndex = 7;
            // 
            // cmbDB
            // 
            this.cmbDB.FormattingEnabled = true;
            this.cmbDB.Location = new System.Drawing.Point(155, 250);
            this.cmbDB.Name = "cmbDB";
            this.cmbDB.Size = new System.Drawing.Size(223, 21);
            this.cmbDB.TabIndex = 6;
            // 
            // cmbServer
            // 
            this.cmbServer.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbServer.FormattingEnabled = true;
            this.cmbServer.Location = new System.Drawing.Point(155, 20);
            this.cmbServer.Name = "cmbServer";
            this.cmbServer.Size = new System.Drawing.Size(223, 21);
            this.cmbServer.TabIndex = 5;
            this.cmbServer.SelectedIndexChanged += new System.EventHandler(this.cmbServer_SelectedIndexChanged);
            // 
            // btnReg
            // 
            this.btnReg.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnReg.Location = new System.Drawing.Point(384, 252);
            this.btnReg.Name = "btnReg";
            this.btnReg.Size = new System.Drawing.Size(194, 23);
            this.btnReg.TabIndex = 4;
            this.btnReg.Text = "Create a Connection to the DB";
            this.btnReg.UseVisualStyleBackColor = true;
            this.btnReg.Click += new System.EventHandler(this.btnReg_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 255);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Database";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 166);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Password";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 135);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "User Name";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "SQL Server Name";
            // 
            // txtBU
            // 
            this.txtBU.Enabled = false;
            this.txtBU.Location = new System.Drawing.Point(319, 116);
            this.txtBU.Name = "txtBU";
            this.txtBU.Size = new System.Drawing.Size(160, 21);
            this.txtBU.TabIndex = 13;
            this.txtBU.Text = "SampleDistributionDB";
            this.txtBU.Visible = false;
            this.txtBU.TextChanged += new System.EventHandler(this.txtBU_TextChanged);
            // 
            // txtDB
            // 
            this.txtDB.Enabled = false;
            this.txtDB.Location = new System.Drawing.Point(155, 223);
            this.txtDB.Name = "txtDB";
            this.txtDB.Size = new System.Drawing.Size(223, 21);
            this.txtDB.TabIndex = 14;
            this.txtDB.Text = "SampleDistributionDB";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(227, 109);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(86, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Backup Name";
            this.label6.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 227);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(98, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Database Name";
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Location = new System.Drawing.Point(384, 222);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(194, 23);
            this.button1.TabIndex = 17;
            this.button1.Text = "Create a new Database";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(210, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(176, 16);
            this.label5.TabIndex = 19;
            this.label5.Text = "SQL Server && Database";
            // 
            // rdoWindowsAuth
            // 
            this.rdoWindowsAuth.AutoSize = true;
            this.rdoWindowsAuth.Location = new System.Drawing.Point(16, 61);
            this.rdoWindowsAuth.Name = "rdoWindowsAuth";
            this.rdoWindowsAuth.Size = new System.Drawing.Size(207, 17);
            this.rdoWindowsAuth.TabIndex = 20;
            this.rdoWindowsAuth.TabStop = true;
            this.rdoWindowsAuth.Text = "Use Windows  NT authentication";
            this.rdoWindowsAuth.UseVisualStyleBackColor = true;
            this.rdoWindowsAuth.CheckedChanged += new System.EventHandler(this.rdoWindowsAuth_CheckedChanged);
            // 
            // rdoSQLAuthentication
            // 
            this.rdoSQLAuthentication.AutoSize = true;
            this.rdoSQLAuthentication.Location = new System.Drawing.Point(16, 85);
            this.rdoSQLAuthentication.Name = "rdoSQLAuthentication";
            this.rdoSQLAuthentication.Size = new System.Drawing.Size(179, 17);
            this.rdoSQLAuthentication.TabIndex = 21;
            this.rdoSQLAuthentication.TabStop = true;
            this.rdoSQLAuthentication.Text = "SQL  Server authentication";
            this.rdoSQLAuthentication.UseVisualStyleBackColor = true;
            this.rdoSQLAuthentication.CheckedChanged += new System.EventHandler(this.rdoSQLAuthentication_CheckedChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::UserAutherization.Properties.Resources.DbConfig;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.ErrorImage = null;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(10, 9);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(186, 315);
            this.pictureBox1.TabIndex = 18;
            this.pictureBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnfinished);
            this.groupBox2.Controls.Add(this.cmbDB);
            this.groupBox2.Controls.Add(this.btnmdfbrows);
            this.groupBox2.Controls.Add(this.rdoSQLAuthentication);
            this.groupBox2.Controls.Add(this.txtldfPath);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.txtmdfPath);
            this.groupBox2.Controls.Add(this.rdoWindowsAuth);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.btnReg);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txtDB);
            this.groupBox2.Controls.Add(this.cmbServer);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.txtUserID);
            this.groupBox2.Controls.Add(this.txtPWD);
            this.groupBox2.Location = new System.Drawing.Point(204, 32);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(674, 290);
            this.groupBox2.TabIndex = 23;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Server Configaration && Locate the Database";
            // 
            // btnmdfbrows
            // 
            this.btnmdfbrows.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnmdfbrows.Location = new System.Drawing.Point(636, 194);
            this.btnmdfbrows.Name = "btnmdfbrows";
            this.btnmdfbrows.Size = new System.Drawing.Size(25, 23);
            this.btnmdfbrows.TabIndex = 22;
            this.btnmdfbrows.Text = "...";
            this.btnmdfbrows.UseVisualStyleBackColor = true;
            this.btnmdfbrows.Click += new System.EventHandler(this.btnmdfbrows_Click);
            // 
            // txtldfPath
            // 
            this.txtldfPath.Location = new System.Drawing.Point(490, 24);
            this.txtldfPath.Name = "txtldfPath";
            this.txtldfPath.Size = new System.Drawing.Size(98, 21);
            this.txtldfPath.TabIndex = 21;
            this.txtldfPath.Visible = false;
            // 
            // txtmdfPath
            // 
            this.txtmdfPath.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtmdfPath.Location = new System.Drawing.Point(155, 195);
            this.txtmdfPath.Name = "txtmdfPath";
            this.txtmdfPath.Size = new System.Drawing.Size(479, 20);
            this.txtmdfPath.TabIndex = 20;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(11, 199);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(55, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "File Path";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(400, 27);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(75, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = ".ldf file path";
            this.label8.Visible = false;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // OFD
            // 
            this.OFD.FileName = "openFileDialog1";
            // 
            // btnfinished
            // 
            this.btnfinished.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnfinished.Location = new System.Drawing.Point(588, 250);
            this.btnfinished.Name = "btnfinished";
            this.btnfinished.Size = new System.Drawing.Size(75, 23);
            this.btnfinished.TabIndex = 23;
            this.btnfinished.Text = "Finish";
            this.btnfinished.UseVisualStyleBackColor = true;
            this.btnfinished.Click += new System.EventHandler(this.btnfinished_Click);
            // 
            // frmRegSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(882, 335);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.txtBU);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.pictureBox1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmRegSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Database Configuration";
            this.Load += new System.EventHandler(this.frmLogin_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtPWD;
        private System.Windows.Forms.TextBox txtUserID;
        private System.Windows.Forms.ComboBox cmbDB;
        private System.Windows.Forms.ComboBox cmbServer;
        private System.Windows.Forms.Button btnReg;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtBU;
        private System.Windows.Forms.TextBox txtDB;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton rdoWindowsAuth;
        private System.Windows.Forms.RadioButton rdoSQLAuthentication;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtldfPath;
        private System.Windows.Forms.TextBox txtmdfPath;
        private System.Windows.Forms.Button btnmdfbrows;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.OpenFileDialog OFD;
        private System.Windows.Forms.Button btnfinished;
    }
}

