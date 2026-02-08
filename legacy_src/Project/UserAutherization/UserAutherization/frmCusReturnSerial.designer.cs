namespace UserAutherization
{
    partial class frmCusReturnSerial
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCusReturnSerial));
            this.label1 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.ChklistBOX = new System.Windows.Forms.CheckedListBox();
            this.lbl_Date = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btn_Delete = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.btn_Add = new System.Windows.Forms.Button();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.txtItemID = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btn_Clear = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.save = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(84, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(146, 20);
            this.label1.TabIndex = 96;
            this.label1.Text = "Serial Numbering";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel4.Controls.Add(this.ChklistBOX);
            this.panel4.Controls.Add(this.lbl_Date);
            this.panel4.Controls.Add(this.label6);
            this.panel4.Controls.Add(this.groupBox2);
            this.panel4.Controls.Add(this.groupBox1);
            this.panel4.Location = new System.Drawing.Point(8, 30);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(328, 375);
            this.panel4.TabIndex = 101;
            this.panel4.Paint += new System.Windows.Forms.PaintEventHandler(this.panel4_Paint);
            // 
            // ChklistBOX
            // 
            this.ChklistBOX.CheckOnClick = true;
            this.ChklistBOX.FormattingEnabled = true;
            this.ChklistBOX.Location = new System.Drawing.Point(18, 91);
            this.ChklistBOX.Name = "ChklistBOX";
            this.ChklistBOX.Size = new System.Drawing.Size(296, 274);
            this.ChklistBOX.TabIndex = 100;
            // 
            // lbl_Date
            // 
            this.lbl_Date.AutoSize = true;
            this.lbl_Date.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Date.Location = new System.Drawing.Point(233, 143);
            this.lbl_Date.Name = "lbl_Date";
            this.lbl_Date.Size = new System.Drawing.Size(0, 12);
            this.lbl_Date.TabIndex = 99;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 72);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(111, 13);
            this.label6.TabIndex = 94;
            this.label6.Text = "Select Serial Numbers";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btn_Delete);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.numericUpDown1);
            this.groupBox2.Controls.Add(this.checkBox1);
            this.groupBox2.Controls.Add(this.btn_Add);
            this.groupBox2.Controls.Add(this.textBox3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(286, 63);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(28, 14);
            this.groupBox2.TabIndex = 93;
            this.groupBox2.TabStop = false;
            // 
            // btn_Delete
            // 
            this.btn_Delete.BackColor = System.Drawing.Color.Silver;
            this.btn_Delete.Enabled = false;
            this.btn_Delete.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_Delete.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Delete.ForeColor = System.Drawing.Color.Black;
            this.btn_Delete.Location = new System.Drawing.Point(246, 51);
            this.btn_Delete.Name = "btn_Delete";
            this.btn_Delete.Size = new System.Drawing.Size(58, 23);
            this.btn_Delete.TabIndex = 98;
            this.btn_Delete.Text = "Delete";
            this.btn_Delete.UseVisualStyleBackColor = false;
            this.btn_Delete.Click += new System.EventHandler(this.btn_Delete_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(105, 57);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(135, 13);
            this.label5.TabIndex = 97;
            this.label5.Text = "Consecutive Serial Number";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Enabled = false;
            this.numericUpDown1.Location = new System.Drawing.Point(54, 54);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(47, 20);
            this.numericUpDown1.TabIndex = 96;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(10, 55);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(45, 17);
            this.checkBox1.TabIndex = 95;
            this.checkBox1.Text = "Add";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // btn_Add
            // 
            this.btn_Add.BackColor = System.Drawing.Color.Silver;
            this.btn_Add.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_Add.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Add.ForeColor = System.Drawing.Color.Black;
            this.btn_Add.Location = new System.Drawing.Point(246, 25);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new System.Drawing.Size(58, 23);
            this.btn_Add.TabIndex = 94;
            this.btn_Add.Text = "Add";
            this.btn_Add.UseVisualStyleBackColor = false;
            this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(8, 27);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(232, 20);
            this.textBox3.TabIndex = 93;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(148, 13);
            this.label4.TabIndex = 92;
            this.label4.Text = "Enter a Serial Number To Add";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtDescription);
            this.groupBox1.Controls.Add(this.txtItemID);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(10, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(310, 59);
            this.groupBox1.TabIndex = 92;
            this.groupBox1.TabStop = false;
            // 
            // txtDescription
            // 
            this.txtDescription.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtDescription.Location = new System.Drawing.Point(70, 34);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ReadOnly = true;
            this.txtDescription.Size = new System.Drawing.Size(234, 20);
            this.txtDescription.TabIndex = 8;
            // 
            // txtItemID
            // 
            this.txtItemID.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtItemID.Location = new System.Drawing.Point(70, 11);
            this.txtItemID.Name = "txtItemID";
            this.txtItemID.ReadOnly = true;
            this.txtItemID.Size = new System.Drawing.Size(234, 20);
            this.txtItemID.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Description";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Item ID ";
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.Gray;
            this.panel5.Location = new System.Drawing.Point(12, 34);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(328, 375);
            this.panel5.TabIndex = 100;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel3.Controls.Add(this.btn_Clear);
            this.panel3.Controls.Add(this.button1);
            this.panel3.Controls.Add(this.save);
            this.panel3.Controls.Add(this.btn_Cancel);
            this.panel3.Location = new System.Drawing.Point(6, 411);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(332, 35);
            this.panel3.TabIndex = 99;
            // 
            // btn_Clear
            // 
            this.btn_Clear.BackColor = System.Drawing.Color.Silver;
            this.btn_Clear.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_Clear.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Clear.ForeColor = System.Drawing.Color.Black;
            this.btn_Clear.Location = new System.Drawing.Point(183, 5);
            this.btn_Clear.Name = "btn_Clear";
            this.btn_Clear.Size = new System.Drawing.Size(67, 23);
            this.btn_Clear.TabIndex = 99;
            this.btn_Clear.Text = "Clear";
            this.btn_Clear.UseVisualStyleBackColor = false;
            this.btn_Clear.Click += new System.EventHandler(this.btn_Clear_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Silver;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.Black;
            this.button1.Location = new System.Drawing.Point(19, 7);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(44, 23);
            this.button1.TabIndex = 86;
            this.button1.Text = "Help";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Visible = false;
            // 
            // save
            // 
            this.save.BackColor = System.Drawing.Color.Silver;
            this.save.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.save.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.save.ForeColor = System.Drawing.Color.Black;
            this.save.Location = new System.Drawing.Point(120, 5);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(58, 23);
            this.save.TabIndex = 85;
            this.save.Text = "OK";
            this.save.UseVisualStyleBackColor = false;
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.BackColor = System.Drawing.Color.Silver;
            this.btn_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_Cancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Cancel.ForeColor = System.Drawing.Color.Black;
            this.btn_Cancel.Location = new System.Drawing.Point(256, 5);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(66, 23);
            this.btn_Cancel.TabIndex = 69;
            this.btn_Cancel.Text = "Close";
            this.btn_Cancel.UseVisualStyleBackColor = false;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Gray;
            this.panel2.Location = new System.Drawing.Point(10, 415);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(332, 35);
            this.panel2.TabIndex = 98;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.Location = new System.Drawing.Point(0, -2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(342, 16);
            this.panel1.TabIndex = 97;
            // 
            // frmCusReturnSerial
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(344, 455);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCusReturnSerial";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Serial Numbering";
            this.Activated += new System.EventHandler(this.frmCusReturnSerial_Activated);
            this.Load += new System.EventHandler(this.Add_Serial_Numbers_Load);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label lbl_Date;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btn_Delete;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button btn_Add;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.TextBox txtItemID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btn_Clear;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button save;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckedListBox ChklistBOX;

    }
}