namespace UserAutherization
{
    partial class frmSVATPrint
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSVATPrint));
            this.CRSVATPrint = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // CRSVATPrint
            // 
            this.CRSVATPrint.ActiveViewIndex = -1;
            this.CRSVATPrint.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CRSVATPrint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CRSVATPrint.Location = new System.Drawing.Point(0, 0);
            this.CRSVATPrint.Name = "CRSVATPrint";
            this.CRSVATPrint.Size = new System.Drawing.Size(844, 515);
            this.CRSVATPrint.TabIndex = 0;
            // 
            // frmSVATPrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(844, 515);
            this.Controls.Add(this.CRSVATPrint);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSVATPrint";
            this.Text = "SVATPrint";
            this.Load += new System.EventHandler(this.frmSVATPrint_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer CRSVATPrint;
    }
}