namespace UserAutherization
{
    partial class frmTransNoteListView
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
            this.crvTransferNotelist = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // crvTransferNotelist
            // 
            this.crvTransferNotelist.ActiveViewIndex = -1;
            this.crvTransferNotelist.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crvTransferNotelist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crvTransferNotelist.Location = new System.Drawing.Point(0, 0);
            this.crvTransferNotelist.Name = "crvTransferNotelist";
            this.crvTransferNotelist.SelectionFormula = "";
            this.crvTransferNotelist.Size = new System.Drawing.Size(729, 538);
            this.crvTransferNotelist.TabIndex = 0;
            this.crvTransferNotelist.ViewTimeSelectionFormula = "";
            // 
            // frmTransNoteListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(729, 538);
            this.Controls.Add(this.crvTransferNotelist);
            this.Name = "frmTransNoteListView";
            this.Text = "Transfer Note List";
            this.Load += new System.EventHandler(this.frmTransNoteListView_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer crvTransferNotelist;
    }
}