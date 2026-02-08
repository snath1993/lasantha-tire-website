namespace UserAutherization
{
    partial class Registration4
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Registration4));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblSerialNO = new System.Windows.Forms.Label();
            this.lblCompanuName = new System.Windows.Forms.Label();
            this.txtBaseSerialNO = new System.Windows.Forms.TextBox();
            this.txtBaseCompanyName = new System.Windows.Forms.TextBox();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Registration Details";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(422, 26);
            this.label2.TabIndex = 2;
            this.label2.Text = "Please enter your company,s registered and base seriol number. The base seriol Nu" +
                "mber\r\ncan be found on your invoce or the back of the CD cover.";
            // 
            // lblSerialNO
            // 
            this.lblSerialNO.AutoSize = true;
            this.lblSerialNO.Location = new System.Drawing.Point(12, 140);
            this.lblSerialNO.Name = "lblSerialNO";
            this.lblSerialNO.Size = new System.Drawing.Size(72, 13);
            this.lblSerialNO.TabIndex = 3;
            this.lblSerialNO.Text = "Product Code";
            // 
            // lblCompanuName
            // 
            this.lblCompanuName.AutoSize = true;
            this.lblCompanuName.Location = new System.Drawing.Point(12, 163);
            this.lblCompanuName.Name = "lblCompanuName";
            this.lblCompanuName.Size = new System.Drawing.Size(131, 13);
            this.lblCompanuName.TabIndex = 4;
            this.lblCompanuName.Text = "Company Regitered Name";
            // 
            // txtBaseSerialNO
            // 
            this.txtBaseSerialNO.Location = new System.Drawing.Point(176, 137);
            this.txtBaseSerialNO.Name = "txtBaseSerialNO";
            this.txtBaseSerialNO.Size = new System.Drawing.Size(224, 20);
            this.txtBaseSerialNO.TabIndex = 5;
            // 
            // txtBaseCompanyName
            // 
            this.txtBaseCompanyName.Location = new System.Drawing.Point(176, 163);
            this.txtBaseCompanyName.Name = "txtBaseCompanyName";
            this.txtBaseCompanyName.Size = new System.Drawing.Size(396, 20);
            this.txtBaseCompanyName.TabIndex = 6;
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.Color.SteelBlue;
            this.btnBack.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnBack.Location = new System.Drawing.Point(307, 233);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 23);
            this.btnBack.TabIndex = 8;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnNext
            // 
            this.btnNext.BackColor = System.Drawing.Color.SteelBlue;
            this.btnNext.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnNext.Location = new System.Drawing.Point(404, 233);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 9;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = false;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.SteelBlue;
            this.btnCancel.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnCancel.Location = new System.Drawing.Point(496, 233);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // Registration4
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ClientSize = new System.Drawing.Size(584, 269);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.txtBaseCompanyName);
            this.Controls.Add(this.txtBaseSerialNO);
            this.Controls.Add(this.lblCompanuName);
            this.Controls.Add(this.lblSerialNO);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Registration4";
            this.Text = "Registration Assistant(Perfect Distribution)";
            this.Load += new System.EventHandler(this.Registration4_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblSerialNO;
        private System.Windows.Forms.Label lblCompanuName;
        private System.Windows.Forms.TextBox txtBaseSerialNO;
        private System.Windows.Forms.TextBox txtBaseCompanyName;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.Button btnNext;
    }
}