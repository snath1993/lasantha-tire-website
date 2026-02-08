namespace UserAutherization
{
    partial class frmSystem
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
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSystem));
            this.ultraPanel1 = new Infragistics.Win.Misc.UltraPanel();
            this.ultraPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraPanel1
            // 
            appearance10.BackColor = System.Drawing.SystemColors.Window;
            appearance10.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance10.ImageBackground")));
            this.ultraPanel1.Appearance = appearance10;
            this.ultraPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraPanel1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraPanel1.Location = new System.Drawing.Point(0, 0);
            this.ultraPanel1.Name = "ultraPanel1";
            this.ultraPanel1.Size = new System.Drawing.Size(965, 664);
            this.ultraPanel1.TabIndex = 1;
            // 
            // frmSystem
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(965, 664);
            this.ControlBox = false;
            this.Controls.Add(this.ultraPanel1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "frmSystem";
            this.ShowInTaskbar = false;
            this.Text = "Transactions";
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmTransactions_Load);
            this.ultraPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraPanel ultraPanel1;
    }
}