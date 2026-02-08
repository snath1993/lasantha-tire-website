namespace UserAutherization
{
    partial class ExportItemMaster
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportItemMaster));
            this.btnInventory = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnInventory
            // 
            this.btnInventory.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnInventory.Image = ((System.Drawing.Image)(resources.GetObject("btnInventory.Image")));
            this.btnInventory.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnInventory.Location = new System.Drawing.Point(176, 75);
            this.btnInventory.Name = "btnInventory";
            this.btnInventory.Size = new System.Drawing.Size(134, 35);
            this.btnInventory.TabIndex = 229;
            this.btnInventory.Text = "Import Inventory";
            this.btnInventory.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnInventory.UseVisualStyleBackColor = false;
            this.btnInventory.Click += new System.EventHandler(this.btnInventory_Click);
            // 
            // ExportItemMaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(489, 195);
            this.Controls.Add(this.btnInventory);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ExportItemMaster";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ExportItemMaster";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnInventory;
    }
}