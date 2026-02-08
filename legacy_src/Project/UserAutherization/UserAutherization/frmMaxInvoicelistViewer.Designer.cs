namespace UserAutherization
{
    partial class frmMaxInvoicelistViewer
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
            this.crvMaxViewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // crvMaxViewer
            // 
            this.crvMaxViewer.ActiveViewIndex = -1;
            this.crvMaxViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.crvMaxViewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crvMaxViewer.Location = new System.Drawing.Point(0, 0);
            this.crvMaxViewer.Name = "crvMaxViewer";
            this.crvMaxViewer.SelectionFormula = "";
            this.crvMaxViewer.Size = new System.Drawing.Size(887, 459);
            this.crvMaxViewer.TabIndex = 0;
            this.crvMaxViewer.ViewTimeSelectionFormula = "";
            // 
            // frmMaxInvoicelistViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(892, 462);
            this.Controls.Add(this.crvMaxViewer);
            this.Name = "frmMaxInvoicelistViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Invoice List";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmMaxInvoicelistViewer_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer crvMaxViewer;
    }
}