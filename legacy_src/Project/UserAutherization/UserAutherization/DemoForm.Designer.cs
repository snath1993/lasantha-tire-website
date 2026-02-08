namespace MultiColumnComboBoxDemo
{
    partial class DemoForm
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
            this.multiColumnComboBox4 = new MultiColumnComboBoxDemo.MultiColumnComboBox();
            this.multiColumnComboBox3 = new MultiColumnComboBoxDemo.MultiColumnComboBox();
            this.multiColumnComboBox2 = new MultiColumnComboBoxDemo.MultiColumnComboBox();
            this.multiColumnComboBox1 = new MultiColumnComboBoxDemo.MultiColumnComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // multiColumnComboBox4
            // 
            this.multiColumnComboBox4.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.multiColumnComboBox4.FormattingEnabled = true;
            this.multiColumnComboBox4.Location = new System.Drawing.Point(29, 178);
            this.multiColumnComboBox4.Name = "multiColumnComboBox4";
            this.multiColumnComboBox4.Size = new System.Drawing.Size(121, 21);
            this.multiColumnComboBox4.TabIndex = 0;
            // 
            // multiColumnComboBox3
            // 
            this.multiColumnComboBox3.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.multiColumnComboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.multiColumnComboBox3.FormattingEnabled = true;
            this.multiColumnComboBox3.Location = new System.Drawing.Point(29, 120);
            this.multiColumnComboBox3.Name = "multiColumnComboBox3";
            this.multiColumnComboBox3.Size = new System.Drawing.Size(121, 21);
            this.multiColumnComboBox3.TabIndex = 0;
            // 
            // multiColumnComboBox2
            // 
            this.multiColumnComboBox2.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.multiColumnComboBox2.FormattingEnabled = true;
            this.multiColumnComboBox2.Location = new System.Drawing.Point(29, 66);
            this.multiColumnComboBox2.Name = "multiColumnComboBox2";
            this.multiColumnComboBox2.Size = new System.Drawing.Size(121, 21);
            this.multiColumnComboBox2.TabIndex = 0;
            // 
            // multiColumnComboBox1
            // 
            this.multiColumnComboBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.multiColumnComboBox1.FormattingEnabled = true;
            this.multiColumnComboBox1.Location = new System.Drawing.Point(29, 17);
            this.multiColumnComboBox1.Name = "multiColumnComboBox1";
            this.multiColumnComboBox1.Size = new System.Drawing.Size(121, 21);
            this.multiColumnComboBox1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(203, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Bound to DataTable";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(203, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Bound to Array";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(203, 123);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(143, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Bound to List (drop-down list)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(203, 178);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Unbound usage";
            // 
            // DemoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 335);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.multiColumnComboBox4);
            this.Controls.Add(this.multiColumnComboBox3);
            this.Controls.Add(this.multiColumnComboBox2);
            this.Controls.Add(this.multiColumnComboBox1);
            this.Name = "DemoForm";
            this.Text = "MultiColumnComboBox Demo";
            this.Load += new System.EventHandler(this.DemoForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MultiColumnComboBox multiColumnComboBox1;
        private MultiColumnComboBox multiColumnComboBox2;
        private MultiColumnComboBox multiColumnComboBox3;
        private MultiColumnComboBox multiColumnComboBox4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}

