namespace UserAutherization
{
    partial class frmViewerJobVarience
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmViewerJobVarience));
            this.crvInvoice = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // crvInvoice
            // 
            this.crvInvoice.ActiveViewIndex = -1;
            this.crvInvoice.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crvInvoice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crvInvoice.Location = new System.Drawing.Point(0, 0);
            this.crvInvoice.Name = "crvInvoice";
            this.crvInvoice.Size = new System.Drawing.Size(585, 542);
            this.crvInvoice.TabIndex = 0;
            this.crvInvoice.Load += new System.EventHandler(this.crystalReportViewer1_Load);
            // 
            // frmViewerJobVarience
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(585, 542);
            this.Controls.Add(this.crvInvoice);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmViewerJobVarience";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Job Varience";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            //this.Load += new System.EventHandler(this.frmViewerJobVarience_Load_1);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer crvInvoice;
    }
}