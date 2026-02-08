namespace UserAutherization
{
    partial class frmOPDCus
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
            this.crvopdcus = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // crvopdcus
            // 
            this.crvopdcus.ActiveViewIndex = -1;
            this.crvopdcus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crvopdcus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crvopdcus.Location = new System.Drawing.Point(0, 0);
            this.crvopdcus.Name = "crvopdcus";
            this.crvopdcus.SelectionFormula = "";
            this.crvopdcus.Size = new System.Drawing.Size(599, 501);
            this.crvopdcus.TabIndex = 0;
            this.crvopdcus.ViewTimeSelectionFormula = "";
            // 
            // frmOPDCus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(599, 501);
            this.Controls.Add(this.crvopdcus);
            this.Name = "frmOPDCus";
            this.Text = "frmOPDCus";
            this.Load += new System.EventHandler(this.frmOPDCus_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer crvopdcus;
    }
}