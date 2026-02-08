namespace UserAutherization
{
    partial class frmPOsalesorderprint
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
            this.crvsalesorder = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // crvsalesorder
            // 
            this.crvsalesorder.ActiveViewIndex = -1;
            this.crvsalesorder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crvsalesorder.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.crvsalesorder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crvsalesorder.Location = new System.Drawing.Point(0, 0);
            this.crvsalesorder.Name = "crvsalesorder";
            this.crvsalesorder.SelectionFormula = "";
            this.crvsalesorder.Size = new System.Drawing.Size(891, 561);
            this.crvsalesorder.TabIndex = 1;
            this.crvsalesorder.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            this.crvsalesorder.ToolPanelWidth = 0;
            this.crvsalesorder.ViewTimeSelectionFormula = "";
            // 
            // frmPOsalesorderprint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(891, 561);
            this.Controls.Add(this.crvsalesorder);
            this.Name = "frmPOsalesorderprint";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quotation";
            this.Load += new System.EventHandler(this.frmPOsalesorderprint_Load);
            this.QueryAccessibilityHelp += new System.Windows.Forms.QueryAccessibilityHelpEventHandler(this.frmPOsalesorderprint_QueryAccessibilityHelp);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer crvsalesorder;
    }
}