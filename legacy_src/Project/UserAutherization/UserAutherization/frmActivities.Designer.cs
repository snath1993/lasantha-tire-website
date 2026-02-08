namespace UserAutherization
{
    partial class frmActivities
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmActivities));
            this.lblActivityNo = new System.Windows.Forms.Label();
            this.lblActivityName = new System.Windows.Forms.Label();
            this.txtActivityNo = new System.Windows.Forms.TextBox();
            this.txtActivityName = new System.Windows.Forms.TextBox();
            this.grpActivities = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSystemName = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnAll = new System.Windows.Forms.Button();
            this.grpActivities.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblActivityNo
            // 
            this.lblActivityNo.AutoSize = true;
            this.lblActivityNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblActivityNo.Location = new System.Drawing.Point(31, 28);
            this.lblActivityNo.Name = "lblActivityNo";
            this.lblActivityNo.Size = new System.Drawing.Size(69, 13);
            this.lblActivityNo.TabIndex = 0;
            this.lblActivityNo.Text = "Activity No";
            // 
            // lblActivityName
            // 
            this.lblActivityName.AutoSize = true;
            this.lblActivityName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblActivityName.Location = new System.Drawing.Point(31, 60);
            this.lblActivityName.Name = "lblActivityName";
            this.lblActivityName.Size = new System.Drawing.Size(85, 13);
            this.lblActivityName.TabIndex = 0;
            this.lblActivityName.Text = "Activity Name";
            // 
            // txtActivityNo
            // 
            this.txtActivityNo.BackColor = System.Drawing.Color.White;
            this.txtActivityNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtActivityNo.Location = new System.Drawing.Point(177, 26);
            this.txtActivityNo.Name = "txtActivityNo";
            this.txtActivityNo.Size = new System.Drawing.Size(205, 20);
            this.txtActivityNo.TabIndex = 1;
            // 
            // txtActivityName
            // 
            this.txtActivityName.BackColor = System.Drawing.Color.White;
            this.txtActivityName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtActivityName.Location = new System.Drawing.Point(177, 55);
            this.txtActivityName.Name = "txtActivityName";
            this.txtActivityName.Size = new System.Drawing.Size(205, 20);
            this.txtActivityName.TabIndex = 2;
            // 
            // grpActivities
            // 
            this.grpActivities.BackColor = System.Drawing.Color.WhiteSmoke;
            this.grpActivities.Controls.Add(this.lblActivityNo);
            this.grpActivities.Controls.Add(this.label1);
            this.grpActivities.Controls.Add(this.txtSystemName);
            this.grpActivities.Controls.Add(this.lblActivityName);
            this.grpActivities.Controls.Add(this.txtActivityName);
            this.grpActivities.Controls.Add(this.txtActivityNo);
            this.grpActivities.Location = new System.Drawing.Point(5, 0);
            this.grpActivities.Name = "grpActivities";
            this.grpActivities.Size = new System.Drawing.Size(419, 120);
            this.grpActivities.TabIndex = 0;
            this.grpActivities.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(31, 91);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "System Name";
            // 
            // txtSystemName
            // 
            this.txtSystemName.BackColor = System.Drawing.Color.White;
            this.txtSystemName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSystemName.Location = new System.Drawing.Point(177, 83);
            this.txtSystemName.Name = "txtSystemName";
            this.txtSystemName.Size = new System.Drawing.Size(205, 20);
            this.txtSystemName.TabIndex = 2;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.DarkGray;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(355, 129);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(69, 27);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.DarkGray;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(154, 129);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(80, 27);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnNew
            // 
            this.btnNew.BackColor = System.Drawing.Color.DarkGray;
            this.btnNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNew.Location = new System.Drawing.Point(46, 129);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(84, 27);
            this.btnNew.TabIndex = 1;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = false;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnAll
            // 
            this.btnAll.BackColor = System.Drawing.Color.DarkGray;
            this.btnAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAll.Location = new System.Drawing.Point(261, 129);
            this.btnAll.Name = "btnAll";
            this.btnAll.Size = new System.Drawing.Size(72, 27);
            this.btnAll.TabIndex = 3;
            this.btnAll.Text = "View All";
            this.btnAll.UseVisualStyleBackColor = false;
            this.btnAll.Click += new System.EventHandler(this.btnAll_Click);
            // 
            // frmActivities
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(429, 169);
            this.ControlBox = false;
            this.Controls.Add(this.btnAll);
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.grpActivities);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(15, 50);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmActivities";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Activities";
            this.Load += new System.EventHandler(this.frmActivities_Load);
            this.grpActivities.ResumeLayout(false);
            this.grpActivities.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblActivityNo;
        private System.Windows.Forms.Label lblActivityName;
        private System.Windows.Forms.TextBox txtActivityNo;
        private System.Windows.Forms.TextBox txtActivityName;
        private System.Windows.Forms.GroupBox grpActivities;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnAll;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSystemName;
    }
}