namespace UserAutherization
{
    partial class frmViewePurchaseOder
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
            this.CRVPurchaseOder = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // CRVPurchaseOder
            // 
            this.CRVPurchaseOder.ActiveViewIndex = -1;
            this.CRVPurchaseOder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CRVPurchaseOder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CRVPurchaseOder.Location = new System.Drawing.Point(0, 0);
            this.CRVPurchaseOder.Name = "CRVPurchaseOder";
            this.CRVPurchaseOder.Size = new System.Drawing.Size(712, 399);
            this.CRVPurchaseOder.TabIndex = 0;
            this.CRVPurchaseOder.Load += new System.EventHandler(this.CRVPurchaseOder_Load);
            // 
            // frmViewePurchaseOder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(712, 399);
            this.Controls.Add(this.CRVPurchaseOder);
            this.Name = "frmViewePurchaseOder";
            this.Text = "frmViewePurchaseOder";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmViewePurchaseOder_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer CRVPurchaseOder;
    }
}