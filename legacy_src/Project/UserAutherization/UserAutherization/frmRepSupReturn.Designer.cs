namespace UserAutherization
{
    partial class frmRepSupReturn
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRepSupReturn));
            this.crvSupplierReturn = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // crvSupplierReturn
            // 
            this.crvSupplierReturn.ActiveViewIndex = -1;
            this.crvSupplierReturn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crvSupplierReturn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crvSupplierReturn.Location = new System.Drawing.Point(0, 0);
            this.crvSupplierReturn.Name = "crvSupplierReturn";
            this.crvSupplierReturn.SelectionFormula = "";
            this.crvSupplierReturn.Size = new System.Drawing.Size(811, 433);
            this.crvSupplierReturn.TabIndex = 0;
            this.crvSupplierReturn.ViewTimeSelectionFormula = "";
            this.crvSupplierReturn.Load += new System.EventHandler(this.crvSupplierReturn_Load);
            // 
            // frmRepSupReturn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(811, 433);
            this.Controls.Add(this.crvSupplierReturn);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmRepSupReturn";
            this.Text = "Supplier Return";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmRepSupReturn_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer crvSupplierReturn;
    }
}