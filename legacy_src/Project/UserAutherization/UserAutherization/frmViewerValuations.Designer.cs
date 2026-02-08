namespace UserAutherization
{
    partial class frmViewerValuations
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
            this.SuspendLayout();
            // 
            // crvInvmovement
            // 
            this.crvInvmovement.ActiveViewIndex = -1;
            this.crvInvmovement.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crvInvmovement.Cursor = System.Windows.Forms.Cursors.Default;
            this.crvInvmovement.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crvInvmovement.Location = new System.Drawing.Point(0, 0);
            this.crvInvmovement.Name = "crvInvmovement";
            this.crvInvmovement.Size = new System.Drawing.Size(785, 396);
            this.crvInvmovement.TabIndex = 1;
            // 
            // frmViewerValuations
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(785, 396);
            this.Controls.Add(this.crvInvmovement);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frmViewerValuations";
            this.Text = "Inventory On Hand";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmValuations_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer crvInvmovement;
    }
}