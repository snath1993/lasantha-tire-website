namespace UserAutherization
{
    partial class frmPOPrintSalesItemRequest
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
            this.crvsalesrequest = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // crvsalesrequest
            // 
            this.crvsalesrequest.ActiveViewIndex = -1;
            this.crvsalesrequest.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crvsalesrequest.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.crvsalesrequest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crvsalesrequest.Location = new System.Drawing.Point(0, 0);
            this.crvsalesrequest.Name = "crvsalesrequest";
            this.crvsalesrequest.SelectionFormula = "";
            this.crvsalesrequest.Size = new System.Drawing.Size(860, 528);
            this.crvsalesrequest.TabIndex = 2;
            this.crvsalesrequest.ViewTimeSelectionFormula = "";
            // 
            // frmPrintSalesItemRequest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 528);
            this.Controls.Add(this.crvsalesrequest);
            this.Name = "frmPrintSalesItemRequest";
            this.Text = "frmPrintSalesItemRequest";
            this.Load += new System.EventHandler(this.frmPrintSalesItemRequest_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer crvsalesrequest;
    }
}