namespace UserAutherization
{
    partial class frmPationReportPrint
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPationReportPrint));
            this.crvInvoice = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // crvInvoice
            // 
            this.crvInvoice.ActiveViewIndex = -1;
            this.crvInvoice.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crvInvoice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crvInvoice.Location = new System.Drawing.Point(0, 0);
            this.crvInvoice.Margin = new System.Windows.Forms.Padding(4);
            this.crvInvoice.Name = "crvInvoice";
            this.crvInvoice.Size = new System.Drawing.Size(780, 667);
            this.crvInvoice.TabIndex = 0;
            this.crvInvoice.ToolPanelWidth = 267;
            // 
            // frmPationReportPrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(780, 667);
            this.Controls.Add(this.crvInvoice);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmPationReportPrint";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmPationReportPrint";
            this.Load += new System.EventHandler(this.frmPationReportPrint_Load);
            this.ResumeLayout(false);

        }
        private CrystalDecisions.Windows.Forms.CrystalReportViewer crvInvoice;
        #endregion
    }
}