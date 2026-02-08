namespace UserAutherization
{
    partial class frmViwerIssueNoteprint
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
            this.CRVIssueNote = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // CRVIssueNote
            // 
            this.CRVIssueNote.ActiveViewIndex = -1;
            this.CRVIssueNote.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CRVIssueNote.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CRVIssueNote.Location = new System.Drawing.Point(0, 0);
            this.CRVIssueNote.Name = "CRVIssueNote";
            this.CRVIssueNote.Size = new System.Drawing.Size(811, 545);
            this.CRVIssueNote.TabIndex = 0;
            // 
            // frmViwerIssueNoteprint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(811, 545);
            this.Controls.Add(this.CRVIssueNote);
            this.Name = "frmViwerIssueNoteprint";
            this.Text = "frmViwerIssueNoteprint";
            this.Load += new System.EventHandler(this.frmViwerIssueNoteprint_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer CRVIssueNote;
    }
}