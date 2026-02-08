namespace UserAutherization
{
    partial class frmItemMasterDefault
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmItemMasterDefault));
            this.label13 = new System.Windows.Forms.Label();
            this.cmbCostMethod = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbItemClass = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(43, 30);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(67, 13);
            this.label13.TabIndex = 23;
            this.label13.Text = "Cost Method";
            // 
            // cmbCostMethod
            // 
            this.cmbCostMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCostMethod.FormattingEnabled = true;
            this.cmbCostMethod.Items.AddRange(new object[] {
            "FIFO",
            "LIFO",
            "Average",
            "Specific Unit"});
            this.cmbCostMethod.Location = new System.Drawing.Point(151, 26);
            this.cmbCostMethod.Name = "cmbCostMethod";
            this.cmbCostMethod.Size = new System.Drawing.Size(186, 21);
            this.cmbCostMethod.TabIndex = 22;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(262, 57);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 24;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(43, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 26;
            this.label1.Text = "Item Class";
            this.label1.Visible = false;
            // 
            // cmbItemClass
            // 
            this.cmbItemClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbItemClass.FormattingEnabled = true;
            this.cmbItemClass.Location = new System.Drawing.Point(12, 73);
            this.cmbItemClass.Name = "cmbItemClass";
            this.cmbItemClass.Size = new System.Drawing.Size(186, 21);
            this.cmbItemClass.TabIndex = 25;
            this.cmbItemClass.Visible = false;
            // 
            // frmItemMasterDefault
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(381, 113);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbItemClass);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.cmbCostMethod);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmItemMasterDefault";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmItemMasterDefault";
            this.Load += new System.EventHandler(this.frmItemMasterDefault_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox cmbCostMethod;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbItemClass;
    }
}