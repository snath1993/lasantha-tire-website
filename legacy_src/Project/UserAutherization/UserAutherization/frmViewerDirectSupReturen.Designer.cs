namespace UserAutherization
{
    partial class frmViewerDirectSupReturen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmViewerDirectSupReturen));
            this.CRVDirectsupReturn = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // CRVDirectsupReturn
            // 
            this.CRVDirectsupReturn.ActiveViewIndex = -1;
            this.CRVDirectsupReturn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CRVDirectsupReturn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CRVDirectsupReturn.Location = new System.Drawing.Point(0, 0);
            this.CRVDirectsupReturn.Name = "CRVDirectsupReturn";
            this.CRVDirectsupReturn.Size = new System.Drawing.Size(712, 399);
            this.CRVDirectsupReturn.TabIndex = 0;
            this.CRVDirectsupReturn.Load += new System.EventHandler(this.CRVDirectsupReturn_Load);
            // 
            // frmViewerDirectSupReturen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(712, 399);
            this.Controls.Add(this.CRVDirectsupReturn);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmViewerDirectSupReturen";
            this.Text = "Direct Suppllier Return";
            this.Load += new System.EventHandler(this.frmViewerDirectSupReturen_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer CRVDirectsupReturn;
    }
}