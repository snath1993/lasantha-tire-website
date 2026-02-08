namespace UserAutherization
{
    partial class frmViewerIsueNoteListprint
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmViewerIsueNoteListprint));
            this.CRVIssueNoteList = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // CRVIssueNoteList
            // 
            this.CRVIssueNoteList.ActiveViewIndex = -1;
            this.CRVIssueNoteList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CRVIssueNoteList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CRVIssueNoteList.Location = new System.Drawing.Point(0, 0);
            this.CRVIssueNoteList.Name = "CRVIssueNoteList";
            this.CRVIssueNoteList.Size = new System.Drawing.Size(812, 508);
            this.CRVIssueNoteList.TabIndex = 0;
            // 
            // frmViewerIsueNoteListprint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(812, 508);
            this.Controls.Add(this.CRVIssueNoteList);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmViewerIsueNoteListprint";
            this.Text = "Issue Notes List";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ViewerIsueNoteListprint_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer CRVIssueNoteList;
    }
}