namespace UserAutherization
{
    partial class frmVehicleHistoryView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmVehicleHistoryView));
            this.crvMaxViewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // crvMaxViewer
            // 
            this.crvMaxViewer.ActiveViewIndex = -1;
            this.crvMaxViewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crvMaxViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crvMaxViewer.Location = new System.Drawing.Point(0, 0);
            this.crvMaxViewer.Name = "crvMaxViewer";
            this.crvMaxViewer.SelectionFormula = "";
            this.crvMaxViewer.Size = new System.Drawing.Size(576, 347);
            this.crvMaxViewer.TabIndex = 0;
            this.crvMaxViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            this.crvMaxViewer.ToolPanelWidth = 0;
            this.crvMaxViewer.ViewTimeSelectionFormula = "";
            // 
            // frmVehicleHistoryView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(576, 347);
            this.Controls.Add(this.crvMaxViewer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmVehicleHistoryView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VehicleHistoryView";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmSalesWiseReportView_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer crvMaxViewer;
    }
}