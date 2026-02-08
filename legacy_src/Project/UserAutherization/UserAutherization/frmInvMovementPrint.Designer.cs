namespace UserAutherization
{
    partial class frmInvMovementPrint
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
            this.crvInvmovement = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // crvInvmovement
            // 
            this.crvInvmovement.ActiveViewIndex = -1;
            this.crvInvmovement.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crvInvmovement.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.crvInvmovement.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crvInvmovement.Location = new System.Drawing.Point(0, 0);
            this.crvInvmovement.Name = "crvInvmovement";
            this.crvInvmovement.Size = new System.Drawing.Size(830, 509);
            this.crvInvmovement.TabIndex = 0;
            // 
            // frmInvMovementPrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(723, 427);
            this.Controls.Add(this.crvInvmovement);
            this.Name = "frmInvMovementPrint";
            this.Text = "Inventory Movement Report Print";
            this.Load += new System.EventHandler(this.frmInvMovementPrint_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer crvInvmovement;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}