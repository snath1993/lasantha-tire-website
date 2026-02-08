namespace UserAutherization
{
    partial class frmImportHeadToBranch
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
            this.btnImports = new System.Windows.Forms.Button();
            this.OFD = new System.Windows.Forms.OpenFileDialog();
            this.button1 = new System.Windows.Forms.Button();
            this.gbCreateXML = new System.Windows.Forms.GroupBox();
            this.button3 = new System.Windows.Forms.Button();
            this.txtTrnsferNO = new System.Windows.Forms.TextBox();
            this.txtselect = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.gbCreateXML.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnImports
            // 
            this.btnImports.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnImports.ForeColor = System.Drawing.Color.Black;
            this.btnImports.Location = new System.Drawing.Point(11, 20);
            this.btnImports.Name = "btnImports";
            this.btnImports.Size = new System.Drawing.Size(124, 30);
            this.btnImports.TabIndex = 3;
            this.btnImports.Text = "Insert Files";
            this.btnImports.UseVisualStyleBackColor = false;
            this.btnImports.Click += new System.EventHandler(this.btnImports_Click);
            // 
            // OFD
            // 
            this.OFD.FileName = "openFileDialog1";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.button1.ForeColor = System.Drawing.Color.Black;
            this.button1.Location = new System.Drawing.Point(425, 56);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(87, 30);
            this.button1.TabIndex = 7;
            this.button1.Text = "Save File";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // gbCreateXML
            // 
            this.gbCreateXML.BackColor = System.Drawing.Color.WhiteSmoke;
            this.gbCreateXML.Controls.Add(this.button3);
            this.gbCreateXML.Controls.Add(this.button1);
            this.gbCreateXML.Controls.Add(this.txtTrnsferNO);
            this.gbCreateXML.Controls.Add(this.txtselect);
            this.gbCreateXML.Controls.Add(this.button2);
            this.gbCreateXML.Controls.Add(this.label2);
            this.gbCreateXML.Controls.Add(this.label3);
            this.gbCreateXML.ForeColor = System.Drawing.Color.Black;
            this.gbCreateXML.Location = new System.Drawing.Point(14, 12);
            this.gbCreateXML.Name = "gbCreateXML";
            this.gbCreateXML.Size = new System.Drawing.Size(581, 97);
            this.gbCreateXML.TabIndex = 13;
            this.gbCreateXML.TabStop = false;
            this.gbCreateXML.Text = "Head Office to Branch";
            this.gbCreateXML.UseCompatibleTextRendering = true;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.WhiteSmoke;
            this.button3.ForeColor = System.Drawing.Color.Black;
            this.button3.Image = global::UserAutherization.Properties.Resources.Close;
            this.button3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button3.Location = new System.Drawing.Point(518, 55);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(55, 30);
            this.button3.TabIndex = 21;
            this.button3.Text = "Exit";
            this.button3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // txtTrnsferNO
            // 
            this.txtTrnsferNO.BackColor = System.Drawing.Color.White;
            this.txtTrnsferNO.Location = new System.Drawing.Point(75, 61);
            this.txtTrnsferNO.Name = "txtTrnsferNO";
            this.txtTrnsferNO.ReadOnly = true;
            this.txtTrnsferNO.Size = new System.Drawing.Size(344, 21);
            this.txtTrnsferNO.TabIndex = 20;
            this.txtTrnsferNO.TextChanged += new System.EventHandler(this.txtTrnsferNO_TextChanged);
            // 
            // txtselect
            // 
            this.txtselect.Location = new System.Drawing.Point(75, 23);
            this.txtselect.Name = "txtselect";
            this.txtselect.Size = new System.Drawing.Size(470, 21);
            this.txtselect.TabIndex = 19;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.button2.ForeColor = System.Drawing.Color.Black;
            this.button2.Location = new System.Drawing.Point(551, 22);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(24, 22);
            this.button2.TabIndex = 18;
            this.button2.Text = "....";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "File Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "Path";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btnImports);
            this.groupBox5.Location = new System.Drawing.Point(439, 124);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(153, 72);
            this.groupBox5.TabIndex = 16;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "TransferNote";
            // 
            // frmImportHeadToBranch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(602, 203);
            this.Controls.Add(this.gbCreateXML);
            this.Controls.Add(this.groupBox5);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmImportHeadToBranch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create Item Transfer";
            this.Load += new System.EventHandler(this.frmImportExport_Load);
            this.gbCreateXML.ResumeLayout(false);
            this.gbCreateXML.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog OFD;
        public System.Windows.Forms.Button btnImports;
        public System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox gbCreateXML;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.TextBox txtselect;
        public System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txtTrnsferNO;
        public System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label2;

    }
}