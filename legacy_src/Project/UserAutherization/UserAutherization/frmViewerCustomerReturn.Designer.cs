namespace UserAutherization
{
    partial class frmViewerCustomerReturn
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
            this.crvCustomerReturn = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // crvCustomerReturn
            // 
            this.crvCustomerReturn.ActiveViewIndex = -1;
            this.crvCustomerReturn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crvCustomerReturn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crvCustomerReturn.Location = new System.Drawing.Point(0, 0);
            this.crvCustomerReturn.Name = "crvCustomerReturn";
            this.crvCustomerReturn.SelectionFormula = "";
            this.crvCustomerReturn.Size = new System.Drawing.Size(787, 556);
            this.crvCustomerReturn.TabIndex = 0;
            this.crvCustomerReturn.ViewTimeSelectionFormula = "";
            // 
            // frmViewerCustomerReturn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(787, 556);
            this.Controls.Add(this.crvCustomerReturn);
            this.Name = "frmViewerCustomerReturn";
            this.Text = "Customer Return";
            this.Load += new System.EventHandler(this.frmViewerCustomerReturn_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer crvCustomerReturn;
    }
}