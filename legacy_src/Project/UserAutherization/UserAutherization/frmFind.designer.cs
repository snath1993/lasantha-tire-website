namespace UserAutherization
{
    partial class frmFind
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFind));
            this.txtFindWord = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvFind = new System.Windows.Forms.DataGridView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbtnOk = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.tsBtnPrint = new System.Windows.Forms.ToolStripButton();
            this.tsbtnPreview = new System.Windows.Forms.ToolStripButton();
            this.tsbtnClear = new System.Windows.Forms.ToolStripButton();
            this.tsBtnRefresh = new System.Windows.Forms.ToolStripButton();
            this.tsBtnCancel = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFind)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtFindWord
            // 
            this.txtFindWord.Location = new System.Drawing.Point(76, 35);
            this.txtFindWord.Name = "txtFindWord";
            this.txtFindWord.Size = new System.Drawing.Size(409, 21);
            this.txtFindWord.TabIndex = 0;
            this.txtFindWord.TextChanged += new System.EventHandler(this.txtFindWord_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Desktop;
            this.label1.Location = new System.Drawing.Point(15, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 14);
            this.label1.TabIndex = 57;
            this.label1.Text = "Find By";
            // 
            // dgvFind
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightSteelBlue;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvFind.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvFind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvFind.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFind.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.LightSteelBlue;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.Desktop;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFind.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvFind.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.LightSteelBlue;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvFind.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvFind.EnableHeadersVisualStyles = false;
            this.dgvFind.GridColor = System.Drawing.Color.Silver;
            this.dgvFind.Location = new System.Drawing.Point(8, 61);
            this.dgvFind.Name = "dgvFind";
            this.dgvFind.ReadOnly = true;
            this.dgvFind.RowHeadersVisible = false;
            this.dgvFind.Size = new System.Drawing.Size(767, 465);
            this.dgvFind.TabIndex = 1;
            //this.dgvFind.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvFind_RowEnter);
            this.dgvFind.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvFind_CellDoubleClick);
            this.dgvFind.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvFind_ColumnHeaderMouseClick_1);
            //this.dgvFind.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvFind_CellContentDoubleClick);
            this.dgvFind.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvFind_KeyDown);
            //this.dgvFind.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvFind_CellContentClick);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbtnOk,
            this.toolStripButton1,
            this.tsBtnPrint,
            this.tsbtnPreview,
            this.tsbtnClear,
            this.tsBtnRefresh,
            this.tsBtnCancel});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(787, 25);
            this.toolStrip1.TabIndex = 64;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbtnOk
            // 
            this.tsbtnOk.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnOk.Image")));
            this.tsbtnOk.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnOk.Name = "tsbtnOk";
            this.tsbtnOk.Size = new System.Drawing.Size(43, 22);
            this.tsbtnOk.Text = "OK";
            this.tsbtnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(50, 22);
            this.toolStripButton1.Text = "Find";
            this.toolStripButton1.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // tsBtnPrint
            // 
            this.tsBtnPrint.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnPrint.Image")));
            this.tsBtnPrint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnPrint.Name = "tsBtnPrint";
            this.tsBtnPrint.Size = new System.Drawing.Size(52, 22);
            this.tsBtnPrint.Text = "Print";
            // 
            // tsbtnPreview
            // 
            this.tsbtnPreview.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnPreview.Image")));
            this.tsbtnPreview.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnPreview.Name = "tsbtnPreview";
            this.tsbtnPreview.Size = new System.Drawing.Size(68, 22);
            this.tsbtnPreview.Text = "Preview";
            // 
            // tsbtnClear
            // 
            this.tsbtnClear.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnClear.Image")));
            this.tsbtnClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnClear.Name = "tsbtnClear";
            this.tsbtnClear.Size = new System.Drawing.Size(54, 22);
            this.tsbtnClear.Text = "Clear";
            this.tsbtnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // tsBtnRefresh
            // 
            this.tsBtnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnRefresh.Image")));
            this.tsBtnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnRefresh.Name = "tsBtnRefresh";
            this.tsBtnRefresh.Size = new System.Drawing.Size(66, 22);
            this.tsBtnRefresh.Text = "Refresh";
            this.tsBtnRefresh.Click += new System.EventHandler(this.tsBtnRefresh_Click);
            // 
            // tsBtnCancel
            // 
            this.tsBtnCancel.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnCancel.Image")));
            this.tsBtnCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnCancel.Name = "tsBtnCancel";
            this.tsBtnCancel.Size = new System.Drawing.Size(63, 22);
            this.tsBtnCancel.Text = "Cancel";
            this.tsBtnCancel.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // frmFind
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(787, 538);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.dgvFind);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtFindWord);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmFind";
            this.Text = "Find";
            this.Load += new System.EventHandler(this.frmFind_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmFind_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFind)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtFindWord;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvFind;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbtnOk;
        private System.Windows.Forms.ToolStripButton tsBtnPrint;
        private System.Windows.Forms.ToolStripButton tsbtnPreview;
        private System.Windows.Forms.ToolStripButton tsbtnClear;
        private System.Windows.Forms.ToolStripButton tsBtnRefresh;
        private System.Windows.Forms.ToolStripButton tsBtnCancel;
        private System.Windows.Forms.ToolStripButton toolStripButton1;

    }
}