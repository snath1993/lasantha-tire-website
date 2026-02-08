namespace UserAutherization
{
    partial class frmRepSupInvoice
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRepSupInvoice));
            this.crvSupInvoice = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // crvSupInvoice
            // 
            this.crvSupInvoice.ActiveViewIndex = -1;
            this.crvSupInvoice.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crvSupInvoice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crvSupInvoice.Location = new System.Drawing.Point(0, 0);
            this.crvSupInvoice.Name = "crvSupInvoice";
            this.crvSupInvoice.SelectionFormula = "";
            this.crvSupInvoice.Size = new System.Drawing.Size(532, 345);
            this.crvSupInvoice.TabIndex = 0;
            this.crvSupInvoice.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            this.crvSupInvoice.ToolPanelWidth = 0;
            this.crvSupInvoice.ViewTimeSelectionFormula = "";
            // 
            // frmRepSupInvoice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(532, 345);
            this.Controls.Add(this.crvSupInvoice);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmRepSupInvoice";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Supplier Invoice";
            this.Load += new System.EventHandler(this.frmRepSupInvoice_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer crvSupInvoice;
    }
}