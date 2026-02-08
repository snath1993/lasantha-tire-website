namespace UserAutherization
{
    partial class frmVieverCreditNotelList
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
            this.CRVCreditNoteList = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // CRVCreditNoteList
            // 
            this.CRVCreditNoteList.ActiveViewIndex = -1;
            this.CRVCreditNoteList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CRVCreditNoteList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CRVCreditNoteList.Location = new System.Drawing.Point(0, 0);
            this.CRVCreditNoteList.Name = "CRVCreditNoteList";
            this.CRVCreditNoteList.Size = new System.Drawing.Size(848, 509);
            this.CRVCreditNoteList.TabIndex = 0;
            // 
            // frmVieverCreditNotelList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(848, 509);
            this.Controls.Add(this.CRVCreditNoteList);
            this.Name = "frmVieverCreditNotelList";
            this.Text = "Credit Note List";
            this.Load += new System.EventHandler(this.frmVieverCreditNotelList_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer CRVCreditNoteList;
    }
}