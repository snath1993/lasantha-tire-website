namespace UserAutherization
{
    partial class frmBeginingBalReport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBeginingBalReport));
            this.crvBegBal = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.cmbSelectWarehouse = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnView = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // crvBegBal
            // 
            this.crvBegBal.ActiveViewIndex = -1;
            this.crvBegBal.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.crvBegBal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crvBegBal.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.crvBegBal.Location = new System.Drawing.Point(0, 53);
            this.crvBegBal.Name = "crvBegBal";
            this.crvBegBal.Size = new System.Drawing.Size(894, 507);
            this.crvBegBal.TabIndex = 0;
            // 
            // cmbSelectWarehouse
            // 
            this.cmbSelectWarehouse.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbSelectWarehouse.FormattingEnabled = true;
            this.cmbSelectWarehouse.Location = new System.Drawing.Point(122, 9);
            this.cmbSelectWarehouse.Name = "cmbSelectWarehouse";
            this.cmbSelectWarehouse.Size = new System.Drawing.Size(208, 24);
            this.cmbSelectWarehouse.TabIndex = 1;
            this.cmbSelectWarehouse.Text = "All";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(10, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Warehouse ";
            // 
            // btnView
            // 
            this.btnView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnView.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnView.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnView.Location = new System.Drawing.Point(336, 10);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(75, 23);
            this.btnView.TabIndex = 3;
            this.btnView.Text = "View";
            this.btnView.UseVisualStyleBackColor = false;
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            // 
            // frmBeginingBalReport
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(894, 560);
            this.Controls.Add(this.btnView);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbSelectWarehouse);
            this.Controls.Add(this.crvBegBal);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmBeginingBalReport";
            this.Text = "Begining Balances ";
            this.Load += new System.EventHandler(this.frmBeginingBalReport_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer crvBegBal;
        private System.Windows.Forms.ComboBox cmbSelectWarehouse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnView;



    }
}