namespace UserAutherization
{
    partial class frmDeleteRecords
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDeleteRecords));
            this.lblPO = new System.Windows.Forms.Label();
            this.cmbPO = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnDeleteSO = new System.Windows.Forms.Button();
            this.btnPoDelete = new System.Windows.Forms.Button();
            this.cmbSalesOrder = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblPO
            // 
            this.lblPO.AutoSize = true;
            this.lblPO.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPO.Location = new System.Drawing.Point(12, 76);
            this.lblPO.Name = "lblPO";
            this.lblPO.Size = new System.Drawing.Size(120, 20);
            this.lblPO.TabIndex = 20;
            this.lblPO.Text = "Purchase Order";
            // 
            // cmbPO
            // 
            this.cmbPO.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbPO.FormattingEnabled = true;
            this.cmbPO.Location = new System.Drawing.Point(138, 73);
            this.cmbPO.Name = "cmbPO";
            this.cmbPO.Size = new System.Drawing.Size(167, 23);
            this.cmbPO.TabIndex = 19;
            this.cmbPO.SelectedIndexChanged += new System.EventHandler(this.cmbPO_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnDeleteSO);
            this.panel1.Controls.Add(this.btnPoDelete);
            this.panel1.Controls.Add(this.cmbSalesOrder);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.cmbPO);
            this.panel1.Controls.Add(this.lblPO);
            this.panel1.Location = new System.Drawing.Point(2, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(426, 192);
            this.panel1.TabIndex = 21;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(247, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(165, 50);
            this.pictureBox1.TabIndex = 225;
            this.pictureBox1.TabStop = false;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.White;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(337, 147);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 34);
            this.btnClose.TabIndex = 21;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnDeleteSO
            // 
            this.btnDeleteSO.BackColor = System.Drawing.Color.White;
            this.btnDeleteSO.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDeleteSO.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeleteSO.Location = new System.Drawing.Point(337, 106);
            this.btnDeleteSO.Name = "btnDeleteSO";
            this.btnDeleteSO.Size = new System.Drawing.Size(75, 35);
            this.btnDeleteSO.TabIndex = 21;
            this.btnDeleteSO.Text = "Delete";
            this.btnDeleteSO.UseVisualStyleBackColor = false;
            this.btnDeleteSO.Click += new System.EventHandler(this.btnDeleteSO_Click);
            // 
            // btnPoDelete
            // 
            this.btnPoDelete.BackColor = System.Drawing.Color.White;
            this.btnPoDelete.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPoDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPoDelete.Location = new System.Drawing.Point(336, 72);
            this.btnPoDelete.Name = "btnPoDelete";
            this.btnPoDelete.Size = new System.Drawing.Size(75, 28);
            this.btnPoDelete.TabIndex = 21;
            this.btnPoDelete.Text = "Delete";
            this.btnPoDelete.UseVisualStyleBackColor = false;
            this.btnPoDelete.Click += new System.EventHandler(this.btnPoDelete_Click);
            // 
            // cmbSalesOrder
            // 
            this.cmbSalesOrder.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbSalesOrder.FormattingEnabled = true;
            this.cmbSalesOrder.Location = new System.Drawing.Point(136, 107);
            this.cmbSalesOrder.Name = "cmbSalesOrder";
            this.cmbSalesOrder.Size = new System.Drawing.Size(167, 23);
            this.cmbSalesOrder.TabIndex = 19;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 110);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 20);
            this.label1.TabIndex = 20;
            this.label1.Text = "Sales Order";
            // 
            // frmDeleteRecords
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(435, 209);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmDeleteRecords";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Delete Records";
            this.Load += new System.EventHandler(this.frmDeleteRecords_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblPO;
        private System.Windows.Forms.ComboBox cmbPO;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnPoDelete;
        private System.Windows.Forms.Button btnDeleteSO;
        private System.Windows.Forms.ComboBox cmbSalesOrder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}