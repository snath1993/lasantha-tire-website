namespace UserAutherization
{
    partial class frmLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLogin));
            this.lblUserName = new System.Windows.Forms.Label();
            this.lblPassWord = new System.Windows.Forms.Label();
            this.txtPassWord = new System.Windows.Forms.TextBox();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAddUser = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.monthCalendar1 = new System.Windows.Forms.MonthCalendar();
            this.dtplogindate = new System.Windows.Forms.DateTimePicker();
            this.rdobtnSpecificDate = new System.Windows.Forms.RadioButton();
            this.rdobtnUseCurrentDate = new System.Windows.Forms.RadioButton();
            this.lnkReg = new System.Windows.Forms.LinkLabel();
            this.gradientWaitingBar2 = new KDHLib.Controls.GradientWaitingBar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.BackColor = System.Drawing.Color.White;
            this.lblUserName.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserName.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblUserName.Location = new System.Drawing.Point(7, 15);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(73, 16);
            this.lblUserName.TabIndex = 0;
            this.lblUserName.Text = "User Name";
            // 
            // lblPassWord
            // 
            this.lblPassWord.AutoSize = true;
            this.lblPassWord.BackColor = System.Drawing.Color.White;
            this.lblPassWord.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPassWord.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblPassWord.Location = new System.Drawing.Point(8, 44);
            this.lblPassWord.Name = "lblPassWord";
            this.lblPassWord.Size = new System.Drawing.Size(73, 16);
            this.lblPassWord.TabIndex = 0;
            this.lblPassWord.Text = "Password  ";
            // 
            // txtPassWord
            // 
            this.txtPassWord.BackColor = System.Drawing.Color.White;
            this.txtPassWord.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPassWord.Location = new System.Drawing.Point(148, 41);
            this.txtPassWord.Name = "txtPassWord";
            this.txtPassWord.PasswordChar = '*';
            this.txtPassWord.Size = new System.Drawing.Size(248, 22);
            this.txtPassWord.TabIndex = 1;
            this.txtPassWord.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPassWord_KeyDown);
            // 
            // txtUserName
            // 
            this.txtUserName.BackColor = System.Drawing.Color.White;
            this.txtUserName.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUserName.Location = new System.Drawing.Point(148, 15);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(248, 22);
            this.txtUserName.TabIndex = 0;
            this.txtUserName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtUserName_KeyDown);
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = System.Drawing.Color.Gray;
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnLogin.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLogin.ForeColor = System.Drawing.Color.White;
            this.btnLogin.Location = new System.Drawing.Point(241, 69);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(75, 27);
            this.btnLogin.TabIndex = 2;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Gray;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancel.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(322, 69);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(74, 27);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAddUser
            // 
            this.btnAddUser.BackColor = System.Drawing.Color.GhostWhite;
            this.btnAddUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddUser.Location = new System.Drawing.Point(6, 69);
            this.btnAddUser.Name = "btnAddUser";
            this.btnAddUser.Size = new System.Drawing.Size(86, 27);
            this.btnAddUser.TabIndex = 1;
            this.btnAddUser.Text = "ADD USER";
            this.btnAddUser.UseVisualStyleBackColor = false;
            this.btnAddUser.Visible = false;
            this.btnAddUser.Click += new System.EventHandler(this.btnAddUser_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Controls.Add(this.monthCalendar1);
            this.groupBox1.Controls.Add(this.dtplogindate);
            this.groupBox1.Controls.Add(this.rdobtnSpecificDate);
            this.groupBox1.Controls.Add(this.rdobtnUseCurrentDate);
            this.groupBox1.Location = new System.Drawing.Point(6, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(403, 205);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Default Transaction Date";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(11, 39);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(147, 127);
            this.pictureBox1.TabIndex = 22;
            this.pictureBox1.TabStop = false;
            // 
            // monthCalendar1
            // 
            this.monthCalendar1.BackColor = System.Drawing.SystemColors.MenuBar;
            this.monthCalendar1.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.monthCalendar1.Location = new System.Drawing.Point(170, 14);
            this.monthCalendar1.Name = "monthCalendar1";
            this.monthCalendar1.ShowTodayCircle = false;
            this.monthCalendar1.TabIndex = 3;
            this.monthCalendar1.TitleBackColor = System.Drawing.SystemColors.AppWorkspace;
            this.monthCalendar1.TitleForeColor = System.Drawing.SystemColors.ActiveBorder;
            this.monthCalendar1.TrailingForeColor = System.Drawing.SystemColors.ControlLightLight;
            // 
            // dtplogindate
            // 
            this.dtplogindate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtplogindate.Location = new System.Drawing.Point(170, 180);
            this.dtplogindate.Name = "dtplogindate";
            this.dtplogindate.Size = new System.Drawing.Size(227, 20);
            this.dtplogindate.TabIndex = 2;
            // 
            // rdobtnSpecificDate
            // 
            this.rdobtnSpecificDate.AutoSize = true;
            this.rdobtnSpecificDate.Location = new System.Drawing.Point(10, 183);
            this.rdobtnSpecificDate.Name = "rdobtnSpecificDate";
            this.rdobtnSpecificDate.Size = new System.Drawing.Size(111, 17);
            this.rdobtnSpecificDate.TabIndex = 1;
            this.rdobtnSpecificDate.Text = "Use Specific Date";
            this.rdobtnSpecificDate.UseVisualStyleBackColor = true;
            this.rdobtnSpecificDate.CheckedChanged += new System.EventHandler(this.rdobtnSpecificDate_CheckedChanged);
            // 
            // rdobtnUseCurrentDate
            // 
            this.rdobtnUseCurrentDate.AutoSize = true;
            this.rdobtnUseCurrentDate.Checked = true;
            this.rdobtnUseCurrentDate.Location = new System.Drawing.Point(10, 16);
            this.rdobtnUseCurrentDate.Name = "rdobtnUseCurrentDate";
            this.rdobtnUseCurrentDate.Size = new System.Drawing.Size(144, 17);
            this.rdobtnUseCurrentDate.TabIndex = 0;
            this.rdobtnUseCurrentDate.TabStop = true;
            this.rdobtnUseCurrentDate.Text = "Use Current System Date";
            this.rdobtnUseCurrentDate.UseVisualStyleBackColor = true;
            // 
            // lnkReg
            // 
            this.lnkReg.AutoSize = true;
            this.lnkReg.Location = new System.Drawing.Point(311, 333);
            this.lnkReg.Name = "lnkReg";
            this.lnkReg.Size = new System.Drawing.Size(98, 13);
            this.lnkReg.TabIndex = 4;
            this.lnkReg.TabStop = true;
            this.lnkReg.Text = "Database Manager";
            this.lnkReg.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkReg_LinkClicked);
            // 
            // gradientWaitingBar2
            // 
            this.gradientWaitingBar2.BackColor = System.Drawing.Color.White;
            this.gradientWaitingBar2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gradientWaitingBar2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.gradientWaitingBar2.GradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.gradientWaitingBar2.GradientColor2 = System.Drawing.Color.White;
            this.gradientWaitingBar2.Interval = 10;
            this.gradientWaitingBar2.Location = new System.Drawing.Point(4, 360);
            this.gradientWaitingBar2.Name = "gradientWaitingBar2";
            this.gradientWaitingBar2.ScrollWAY = KDHLib.Controls.GradientWaitingBar.SCROLLGRADIENTALIGN.HORIZONTAL;
            this.gradientWaitingBar2.Size = new System.Drawing.Size(409, 6);
            this.gradientWaitingBar2.Speed = 1;
            this.gradientWaitingBar2.TabIndex = 20;
            this.gradientWaitingBar2.Text = "aaaaaa";
            this.gradientWaitingBar2.Click += new System.EventHandler(this.gradientWaitingBar2_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.White;
            this.groupBox2.Controls.Add(this.txtUserName);
            this.groupBox2.Controls.Add(this.txtPassWord);
            this.groupBox2.Controls.Add(this.lblUserName);
            this.groupBox2.Controls.Add(this.lblPassWord);
            this.groupBox2.Controls.Add(this.btnAddUser);
            this.groupBox2.Controls.Add(this.btnLogin);
            this.groupBox2.Controls.Add(this.btnCancel);
            this.groupBox2.Location = new System.Drawing.Point(6, 218);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(403, 107);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Login";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(135, 328);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(150, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Version 2024.11.15.0001";
            // 
            // frmLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(414, 379);
            this.ControlBox = false;
            this.Controls.Add(this.lnkReg);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.gradientWaitingBar2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.Load += new System.EventHandler(this.frmLogin_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Label lblPassWord;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.TextBox txtPassWord;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnAddUser;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdobtnSpecificDate;
        private System.Windows.Forms.RadioButton rdobtnUseCurrentDate;
        private System.Windows.Forms.MonthCalendar monthCalendar1;
        private System.Windows.Forms.DateTimePicker dtplogindate;
        private KDHLib.Controls.GradientWaitingBar gradientWaitingBar2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.LinkLabel lnkReg;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
    }
}

