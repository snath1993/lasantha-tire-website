namespace UserAutherization
{
    partial class frmImportItemMaster
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmImportItemMaster));
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblDate = new System.Windows.Forms.Label();
            this.dtpTranDate = new System.Windows.Forms.DateTimePicker();
            this.btnFillWarehouse = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnInventory = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.lblDate);
            this.panel1.Controls.Add(this.dtpTranDate);
            this.panel1.Controls.Add(this.btnFillWarehouse);
            this.panel1.Controls.Add(this.btnImport);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Location = new System.Drawing.Point(314, 91);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(127, 53);
            this.panel1.TabIndex = 2;
            this.panel1.Visible = false;
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Location = new System.Drawing.Point(8, 8);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(31, 13);
            this.lblDate.TabIndex = 2;
            this.lblDate.Text = "Date";
            // 
            // dtpTranDate
            // 
            this.dtpTranDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTranDate.Location = new System.Drawing.Point(44, 8);
            this.dtpTranDate.Name = "dtpTranDate";
            this.dtpTranDate.Size = new System.Drawing.Size(100, 22);
            this.dtpTranDate.TabIndex = 1;
            // 
            // btnFillWarehouse
            // 
            this.btnFillWarehouse.BackColor = System.Drawing.Color.White;
            this.btnFillWarehouse.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnFillWarehouse.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFillWarehouse.Location = new System.Drawing.Point(150, 8);
            this.btnFillWarehouse.Name = "btnFillWarehouse";
            this.btnFillWarehouse.Size = new System.Drawing.Size(118, 36);
            this.btnFillWarehouse.TabIndex = 0;
            this.btnFillWarehouse.Text = "Fill Warehouse";
            this.btnFillWarehouse.UseVisualStyleBackColor = false;
            this.btnFillWarehouse.Click += new System.EventHandler(this.btnFillWarehouse_Click);
            // 
            // btnImport
            // 
            this.btnImport.BackColor = System.Drawing.Color.White;
            this.btnImport.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnImport.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImport.Location = new System.Drawing.Point(44, 55);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(100, 36);
            this.btnImport.TabIndex = 0;
            this.btnImport.Text = "Yes";
            this.btnImport.UseVisualStyleBackColor = false;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.White;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(150, 55);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(118, 36);
            this.button1.TabIndex = 0;
            this.button1.Text = "Close";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(271, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(165, 50);
            this.pictureBox1.TabIndex = 227;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Navy;
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(253, 25);
            this.label1.TabIndex = 226;
            this.label1.Text = "Import Peachtree Data";
            // 
            // btnInventory
            // 
            this.btnInventory.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnInventory.Image = ((System.Drawing.Image)(resources.GetObject("btnInventory.Image")));
            this.btnInventory.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnInventory.Location = new System.Drawing.Point(45, 88);
            this.btnInventory.Name = "btnInventory";
            this.btnInventory.Size = new System.Drawing.Size(134, 35);
            this.btnInventory.TabIndex = 228;
            this.btnInventory.Text = "Import Inventory";
            this.btnInventory.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnInventory.UseVisualStyleBackColor = false;
            this.btnInventory.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // frmImportItemMaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(453, 166);
            this.Controls.Add(this.btnInventory);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmImportItemMaster";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Import Item Master";
            this.Load += new System.EventHandler(this.frmImportItemMaster_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnFillWarehouse;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.DateTimePicker dtpTranDate;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnInventory;
    }
}