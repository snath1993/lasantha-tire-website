namespace WarehouseTransfer
{
    partial class frmFGTransferPrint
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
            this.crvReturnNoteprint = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // crvReturnNoteprint
            // 
            this.crvReturnNoteprint.ActiveViewIndex = -1;
            this.crvReturnNoteprint.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crvReturnNoteprint.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.crvReturnNoteprint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crvReturnNoteprint.Location = new System.Drawing.Point(0, 0);
            this.crvReturnNoteprint.Name = "crvReturnNoteprint";
            this.crvReturnNoteprint.SelectionFormula = "";
            this.crvReturnNoteprint.Size = new System.Drawing.Size(800, 439);
            this.crvReturnNoteprint.TabIndex = 0;
            this.crvReturnNoteprint.ViewTimeSelectionFormula = "";
            // 
            // frmPrintReturnNote
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 439);
            this.Controls.Add(this.crvReturnNoteprint);
            this.Name = "frmPrintReturnNote";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Issue Note";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmPrintReturnNote_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer crvReturnNoteprint;
    }
}