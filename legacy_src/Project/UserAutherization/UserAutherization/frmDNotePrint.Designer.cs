namespace UserAutherization
{
    partial class frmDNotePrint
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
            this.crvDeliveryNote = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // crvDeliveryNote
            // 
            this.crvDeliveryNote.ActiveViewIndex = -1;
            this.crvDeliveryNote.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crvDeliveryNote.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crvDeliveryNote.Location = new System.Drawing.Point(0, 0);
            this.crvDeliveryNote.Name = "crvDeliveryNote";
            this.crvDeliveryNote.SelectionFormula = "";
            this.crvDeliveryNote.Size = new System.Drawing.Size(632, 527);
            this.crvDeliveryNote.TabIndex = 0;
            this.crvDeliveryNote.ViewTimeSelectionFormula = "";
            this.crvDeliveryNote.Load += new System.EventHandler(this.crvDeliveryNote_Load);
            // 
            // frmDNotePrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 527);
            this.Controls.Add(this.crvDeliveryNote);
            this.Name = "frmDNotePrint";
            this.Text = "frmDeiveryNotePrint";
            this.Load += new System.EventHandler(this.frmDeiveryNotePrint_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer crvDeliveryNote;
    }
}