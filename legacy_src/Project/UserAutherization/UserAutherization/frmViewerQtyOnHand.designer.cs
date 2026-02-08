namespace UserAutherization
{
    partial class frmViewerQtyOnHand
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
            this.crvValuation = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // crvValuation
            // 
            this.crvValuation.ActiveViewIndex = -1;
            this.crvValuation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crvValuation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crvValuation.Location = new System.Drawing.Point(0, 0);
            this.crvValuation.Name = "crvValuation";
            this.crvValuation.SelectionFormula = "";
            this.crvValuation.Size = new System.Drawing.Size(996, 470);
            this.crvValuation.TabIndex = 0;
            this.crvValuation.ViewTimeSelectionFormula = "";
            // 
            // frmViewerQtyOnHand
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(996, 470);
            this.Controls.Add(this.crvValuation);
            this.Name = "frmViewerQtyOnHand";
            this.Text = "Quntity On Hand";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmValuations_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer crvValuation;
    }
}