namespace UserAutherization
{
    partial class frmAddUser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAddUser));
            this.lblUserName = new System.Windows.Forms.Label();
            this.lblConfirmPassWord = new System.Windows.Forms.Label();
            this.lblUserLevel = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.txtPassWord = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpAddUser = new System.Windows.Forms.GroupBox();
            this.cmbUserLevel = new System.Windows.Forms.ComboBox();
            this.lblPassWord = new System.Windows.Forms.Label();
            this.txtConfirmPassWord = new System.Windows.Forms.TextBox();
            this.btnAll = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnAuthentication = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.grpAddUser.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserName.Location = new System.Drawing.Point(18, 16);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(69, 13);
            this.lblUserName.TabIndex = 1;
            this.lblUserName.Text = "User Name";
            // 
            // lblConfirmPassWord
            // 
            this.lblConfirmPassWord.AutoSize = true;
            this.lblConfirmPassWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConfirmPassWord.Location = new System.Drawing.Point(18, 68);
            this.lblConfirmPassWord.Name = "lblConfirmPassWord";
            this.lblConfirmPassWord.Size = new System.Drawing.Size(107, 13);
            this.lblConfirmPassWord.TabIndex = 1;
            this.lblConfirmPassWord.Text = "Confirm Password";
            // 
            // lblUserLevel
            // 
            this.lblUserLevel.AutoSize = true;
            this.lblUserLevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserLevel.Location = new System.Drawing.Point(18, 94);
            this.lblUserLevel.Name = "lblUserLevel";
            this.lblUserLevel.Size = new System.Drawing.Size(68, 13);
            this.lblUserLevel.TabIndex = 1;
            this.lblUserLevel.Text = "User Level";
            // 
            // txtUserName
            // 
            this.txtUserName.BackColor = System.Drawing.Color.White;
            this.txtUserName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUserName.Location = new System.Drawing.Point(174, 16);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(188, 20);
            this.txtUserName.TabIndex = 1;
            // 
            // txtPassWord
            // 
            this.txtPassWord.BackColor = System.Drawing.Color.White;
            this.txtPassWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPassWord.Location = new System.Drawing.Point(174, 42);
            this.txtPassWord.Name = "txtPassWord";
            this.txtPassWord.PasswordChar = '*';
            this.txtPassWord.Size = new System.Drawing.Size(188, 20);
            this.txtPassWord.TabIndex = 2;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.Silver;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(206, 201);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(83, 35);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Silver;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(390, 198);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(109, 35);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grpAddUser
            // 
            this.grpAddUser.BackColor = System.Drawing.Color.WhiteSmoke;
            this.grpAddUser.Controls.Add(this.cmbUserLevel);
            this.grpAddUser.Controls.Add(this.lblUserName);
            this.grpAddUser.Controls.Add(this.lblPassWord);
            this.grpAddUser.Controls.Add(this.lblConfirmPassWord);
            this.grpAddUser.Controls.Add(this.lblUserLevel);
            this.grpAddUser.Controls.Add(this.txtUserName);
            this.grpAddUser.Controls.Add(this.txtConfirmPassWord);
            this.grpAddUser.Controls.Add(this.txtPassWord);
            this.grpAddUser.Location = new System.Drawing.Point(6, 60);
            this.grpAddUser.Name = "grpAddUser";
            this.grpAddUser.Size = new System.Drawing.Size(378, 129);
            this.grpAddUser.TabIndex = 0;
            this.grpAddUser.TabStop = false;
            // 
            // cmbUserLevel
            // 
            this.cmbUserLevel.BackColor = System.Drawing.Color.White;
            this.cmbUserLevel.FormattingEnabled = true;
            this.cmbUserLevel.Items.AddRange(new object[] {
            "1",
            "2",
            "3"});
            this.cmbUserLevel.Location = new System.Drawing.Point(174, 94);
            this.cmbUserLevel.Name = "cmbUserLevel";
            this.cmbUserLevel.Size = new System.Drawing.Size(188, 21);
            this.cmbUserLevel.TabIndex = 4;
            this.cmbUserLevel.Text = "1";
            // 
            // lblPassWord
            // 
            this.lblPassWord.AutoSize = true;
            this.lblPassWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPassWord.Location = new System.Drawing.Point(18, 42);
            this.lblPassWord.Name = "lblPassWord";
            this.lblPassWord.Size = new System.Drawing.Size(61, 13);
            this.lblPassWord.TabIndex = 1;
            this.lblPassWord.Text = "Password";
            // 
            // txtConfirmPassWord
            // 
            this.txtConfirmPassWord.BackColor = System.Drawing.Color.White;
            this.txtConfirmPassWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConfirmPassWord.Location = new System.Drawing.Point(174, 68);
            this.txtConfirmPassWord.Name = "txtConfirmPassWord";
            this.txtConfirmPassWord.PasswordChar = '*';
            this.txtConfirmPassWord.Size = new System.Drawing.Size(188, 20);
            this.txtConfirmPassWord.TabIndex = 3;
            // 
            // btnAll
            // 
            this.btnAll.BackColor = System.Drawing.Color.Silver;
            this.btnAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAll.Location = new System.Drawing.Point(390, 64);
            this.btnAll.Name = "btnAll";
            this.btnAll.Size = new System.Drawing.Size(108, 35);
            this.btnAll.TabIndex = 5;
            this.btnAll.Text = "View All";
            this.btnAll.UseVisualStyleBackColor = false;
            this.btnAll.Click += new System.EventHandler(this.btnAll_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.Silver;
            this.btnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearch.Location = new System.Drawing.Point(11, 20);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(91, 27);
            this.btnSearch.TabIndex = 8;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.BackColor = System.Drawing.Color.White;
            this.txtSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearch.Location = new System.Drawing.Point(118, 20);
            this.txtSearch.Multiline = true;
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(378, 25);
            this.txtSearch.TabIndex = 9;
            this.txtSearch.Tag = "";
            // 
            // btnEdit
            // 
            this.btnEdit.BackColor = System.Drawing.Color.Silver;
            this.btnEdit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEdit.Location = new System.Drawing.Point(294, 199);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(87, 35);
            this.btnEdit.TabIndex = 3;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnNew
            // 
            this.btnNew.BackColor = System.Drawing.Color.Silver;
            this.btnNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNew.Location = new System.Drawing.Point(111, 202);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(90, 35);
            this.btnNew.TabIndex = 1;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = false;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnAuthentication
            // 
            this.btnAuthentication.BackColor = System.Drawing.Color.Silver;
            this.btnAuthentication.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAuthentication.Location = new System.Drawing.Point(391, 151);
            this.btnAuthentication.Name = "btnAuthentication";
            this.btnAuthentication.Size = new System.Drawing.Size(108, 39);
            this.btnAuthentication.TabIndex = 6;
            this.btnAuthentication.Text = "Authentication";
            this.btnAuthentication.UseVisualStyleBackColor = false;
            this.btnAuthentication.Click += new System.EventHandler(this.btnAuthentication_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.BackColor = System.Drawing.Color.Silver;
            this.btnCopy.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCopy.Location = new System.Drawing.Point(390, 107);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(108, 36);
            this.btnCopy.TabIndex = 7;
            this.btnCopy.Text = "Copy";
            this.btnCopy.UseVisualStyleBackColor = false;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // frmAddUser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(508, 242);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.btnAuthentication);
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.btnAll);
            this.Controls.Add(this.grpAddUser);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(15, 50);
            this.Name = "frmAddUser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add User";
            this.Load += new System.EventHandler(this.frmAddUser_Load);
            this.grpAddUser.ResumeLayout(false);
            this.grpAddUser.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Label lblConfirmPassWord;
        private System.Windows.Forms.Label lblUserLevel;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.TextBox txtPassWord;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox grpAddUser;
        private System.Windows.Forms.Label lblPassWord;
        private System.Windows.Forms.TextBox txtConfirmPassWord;
        private System.Windows.Forms.ComboBox cmbUserLevel;
        private System.Windows.Forms.Button btnAll;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnAuthentication;
        private System.Windows.Forms.Button btnCopy;
    }
}