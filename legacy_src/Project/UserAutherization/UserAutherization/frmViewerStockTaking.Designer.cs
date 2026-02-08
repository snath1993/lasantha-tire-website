namespace UserAutherization
{
    partial class frmViewerStockTaking
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
            this.crvStockTaking = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // crvStockTaking
            // 
            this.crvStockTaking.ActiveViewIndex = -1;
            this.crvStockTaking.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crvStockTaking.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crvStockTaking.Location = new System.Drawing.Point(0, 0);
            this.crvStockTaking.Name = "crvStockTaking";
            this.crvStockTaking.SelectionFormula = "";
            this.crvStockTaking.Size = new System.Drawing.Size(292, 266);
            this.crvStockTaking.TabIndex = 0;
            this.crvStockTaking.ViewTimeSelectionFormula = "";
            // 
            // frmViewerStockTaking
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.crvStockTaking);
            this.Name = "frmViewerStockTaking";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Stock Taking";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmViewerStockTaking_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer crvStockTaking;
    }
}