namespace UserAutherization
{
    partial class frmViwerAdjustmetnsListPrint
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmViwerAdjustmetnsListPrint));
            this.CRVAdjustmentList = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // CRVAdjustmentList
            // 
            this.CRVAdjustmentList.ActiveViewIndex = -1;
            this.CRVAdjustmentList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CRVAdjustmentList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CRVAdjustmentList.Location = new System.Drawing.Point(0, 0);
            this.CRVAdjustmentList.Name = "CRVAdjustmentList";
            this.CRVAdjustmentList.Size = new System.Drawing.Size(826, 592);
            this.CRVAdjustmentList.TabIndex = 0;
            // 
            // frmViwerAdjustmetnsListPrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(826, 592);
            this.Controls.Add(this.CRVAdjustmentList);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmViwerAdjustmetnsListPrint";
            this.Text = "Inventory Adjustmnets List";
            this.Load += new System.EventHandler(this.frmViwerAdjustmetnsListPrint_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer CRVAdjustmentList;
    }
}