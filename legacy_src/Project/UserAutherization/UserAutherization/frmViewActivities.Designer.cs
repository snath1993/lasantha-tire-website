namespace UserAutherization
{
    partial class frmViewActivities
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
            this.dgvActivities = new System.Windows.Forms.DataGridView();
            this.ActivityNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ActivityName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvActivities)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvActivities
            // 
            this.dgvActivities.BackgroundColor = System.Drawing.Color.White;
            this.dgvActivities.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvActivities.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ActivityNo,
            this.ActivityName});
            this.dgvActivities.GridColor = System.Drawing.Color.DarkBlue;
            this.dgvActivities.Location = new System.Drawing.Point(9, 22);
            this.dgvActivities.Name = "dgvActivities";
            this.dgvActivities.ReadOnly = true;
            this.dgvActivities.RowHeadersVisible = false;
            this.dgvActivities.Size = new System.Drawing.Size(303, 347);
            this.dgvActivities.TabIndex = 0;
            // 
            // ActivityNo
            // 
            this.ActivityNo.HeaderText = "Activity No";
            this.ActivityNo.Name = "ActivityNo";
            this.ActivityNo.ReadOnly = true;
            // 
            // ActivityName
            // 
            this.ActivityName.HeaderText = "Activity Name";
            this.ActivityName.Name = "ActivityName";
            this.ActivityName.ReadOnly = true;
            this.ActivityName.Width = 200;
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.Location = new System.Drawing.Point(118, 377);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(91, 27);
            this.btnDelete.TabIndex = 1;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(216, 377);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(91, 27);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmViewActivities
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(321, 410);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.dgvActivities);
            this.Name = "frmViewActivities";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "View Activities";
            this.Load += new System.EventHandler(this.frmViewActivities_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvActivities)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvActivities;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DataGridViewTextBoxColumn ActivityNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn ActivityName;
    }
}