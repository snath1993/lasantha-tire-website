namespace UserAutherization
{
    partial class frmPrintTotalDailyCollection
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
            this.CryDaillyColView = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // CryDaillyColView
            // 
            this.CryDaillyColView.ActiveViewIndex = -1;
            this.CryDaillyColView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CryDaillyColView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CryDaillyColView.Location = new System.Drawing.Point(0, 0);
            this.CryDaillyColView.Name = "CryDaillyColView";
            this.CryDaillyColView.Size = new System.Drawing.Size(848, 560);
            this.CryDaillyColView.TabIndex = 0;
            this.CryDaillyColView.Load += new System.EventHandler(this.CryDaillyColView_Load);
            // 
            // frmPrintTotalDailyCollection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(848, 560);
            this.Controls.Add(this.CryDaillyColView);
            this.Name = "frmPrintTotalDailyCollection";
            this.Text = "frmPrintTotalDailyCollection";
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer CryDaillyColView;
    }
}