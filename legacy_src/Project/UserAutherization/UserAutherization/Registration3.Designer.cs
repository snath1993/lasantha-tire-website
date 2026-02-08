namespace UserAutherization
{
    partial class Registration3
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Registration3));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.rdobtnLocalServer = new System.Windows.Forms.RadioButton();
            this.rdobtnWorkstation = new System.Windows.Forms.RadioButton();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Registration Type";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(367, 78);
            this.label2.TabIndex = 3;
            this.label2.Text = resources.GetString("label2.Text");
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // rdobtnLocalServer
            // 
            this.rdobtnLocalServer.AutoSize = true;
            this.rdobtnLocalServer.Location = new System.Drawing.Point(12, 176);
            this.rdobtnLocalServer.Name = "rdobtnLocalServer";
            this.rdobtnLocalServer.Size = new System.Drawing.Size(146, 17);
            this.rdobtnLocalServer.TabIndex = 4;
            this.rdobtnLocalServer.TabStop = true;
            this.rdobtnLocalServer.Text = "Local/Server Registration";
            this.rdobtnLocalServer.UseVisualStyleBackColor = true;
            // 
            // rdobtnWorkstation
            // 
            this.rdobtnWorkstation.AutoSize = true;
            this.rdobtnWorkstation.Location = new System.Drawing.Point(12, 199);
            this.rdobtnWorkstation.Name = "rdobtnWorkstation";
            this.rdobtnWorkstation.Size = new System.Drawing.Size(141, 17);
            this.rdobtnWorkstation.TabIndex = 5;
            this.rdobtnWorkstation.TabStop = true;
            this.rdobtnWorkstation.Text = "Workstation Registration";
            this.rdobtnWorkstation.UseVisualStyleBackColor = true;
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.Color.SteelBlue;
            this.btnBack.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnBack.Location = new System.Drawing.Point(300, 233);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 23);
            this.btnBack.TabIndex = 0;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnNext
            // 
            this.btnNext.BackColor = System.Drawing.Color.SteelBlue;
            this.btnNext.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnNext.Location = new System.Drawing.Point(394, 233);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 1;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = false;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.SteelBlue;
            this.btnCancel.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnCancel.Location = new System.Drawing.Point(480, 233);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // Registration3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(586, 268);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.rdobtnWorkstation);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.rdobtnLocalServer);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Registration3";
            this.Text = "Registration Assistant(Distribution System)";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.RadioButton rdobtnLocalServer;
        private System.Windows.Forms.RadioButton rdobtnWorkstation;
    }
}