namespace UserAutherization
{
    partial class FrmUserwiseSummery
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
            this.cmbUser = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.SuspendLayout();
            // 
            // cmbUser
            // 
            this.cmbUser.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cmbUser.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmbUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbUser.FormattingEnabled = true;
            this.cmbUser.Location = new System.Drawing.Point(181, 47);
            this.cmbUser.Name = "cmbUser";
            this.cmbUser.Size = new System.Drawing.Size(187, 21);
            this.cmbUser.TabIndex = 318;
            this.cmbUser.Text = "SELECT USER";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(85, 74);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(283, 34);
            this.button1.TabIndex = 317;
            this.button1.Text = "Print";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker2.Location = new System.Drawing.Point(260, 15);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(108, 20);
            this.dateTimePicker2.TabIndex = 316;
            this.dateTimePicker2.Value = new System.DateTime(2013, 11, 7, 0, 0, 0, 0);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(199, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 17);
            this.label1.TabIndex = 315;
            this.label1.Text = "Date To";
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label39.Location = new System.Drawing.Point(9, 15);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(72, 17);
            this.label39.TabIndex = 314;
            this.label39.Text = "Date From";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker1.Location = new System.Drawing.Point(85, 15);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(108, 20);
            this.dateTimePicker1.TabIndex = 313;
            this.dateTimePicker1.Value = new System.DateTime(2013, 11, 7, 0, 0, 0, 0);
            // 
            // FrmUserwiseSummery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(382, 124);
            this.Controls.Add(this.cmbUser);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dateTimePicker2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label39);
            this.Controls.Add(this.dateTimePicker1);
            this.Name = "FrmUserwiseSummery";
            this.Text = "Daily Collection";
            this.Load += new System.EventHandler(this.FrmUserwiseSummery_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbUser;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
    }
}