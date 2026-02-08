namespace UserAutherization
{
    partial class frmBOMList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBOMList));
            this.dgvAssembylist = new System.Windows.Forms.DataGridView();
            this.Reference = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AssemblyID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AssemblyDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Qty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WarehouseID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Action = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label6 = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.gradientWaitingBar2 = new KDHLib.Controls.GradientWaitingBar();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAssembylist)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvAssembylist
            // 
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvAssembylist.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvAssembylist.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvAssembylist.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvAssembylist.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAssembylist.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Reference,
            this.AssemblyID,
            this.AssemblyDesc,
            this.date,
            this.Qty,
            this.WarehouseID,
            this.Action,
            this.Status});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvAssembylist.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvAssembylist.Location = new System.Drawing.Point(3, 43);
            this.dgvAssembylist.Name = "dgvAssembylist";
            this.dgvAssembylist.RowHeadersVisible = false;
            this.dgvAssembylist.Size = new System.Drawing.Size(858, 330);
            this.dgvAssembylist.TabIndex = 58;
            this.dgvAssembylist.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAssembylist_CellDoubleClick);
            // 
            // Reference
            // 
            this.Reference.HeaderText = "Reference";
            this.Reference.Name = "Reference";
            this.Reference.ReadOnly = true;
            // 
            // AssemblyID
            // 
            this.AssemblyID.HeaderText = "AssemblyID";
            this.AssemblyID.Name = "AssemblyID";
            this.AssemblyID.ReadOnly = true;
            this.AssemblyID.Width = 130;
            // 
            // AssemblyDesc
            // 
            this.AssemblyDesc.HeaderText = "Description";
            this.AssemblyDesc.Name = "AssemblyDesc";
            this.AssemblyDesc.Width = 220;
            // 
            // date
            // 
            this.date.HeaderText = "AssemblyDate";
            this.date.Name = "date";
            this.date.ReadOnly = true;
            // 
            // Qty
            // 
            this.Qty.HeaderText = "AssemblyQty";
            this.Qty.Name = "Qty";
            this.Qty.ReadOnly = true;
            // 
            // WarehouseID
            // 
            this.WarehouseID.HeaderText = "Warehouse ID";
            this.WarehouseID.Name = "WarehouseID";
            this.WarehouseID.ReadOnly = true;
            this.WarehouseID.Visible = false;
            // 
            // Action
            // 
            this.Action.HeaderText = "Action";
            this.Action.Name = "Action";
            this.Action.ReadOnly = true;
            // 
            // Status
            // 
            this.Status.HeaderText = "Status";
            this.Status.Name = "Status";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(513, 17);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(155, 16);
            this.label6.TabIndex = 80;
            this.label6.Text = "Search By Reference";
            // 
            // txtSearch
            // 
            this.txtSearch.BackColor = System.Drawing.Color.White;
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearch.Location = new System.Drawing.Point(679, 11);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(182, 23);
            this.txtSearch.TabIndex = 81;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Gainsboro;
            this.panel1.Controls.Add(this.gradientWaitingBar2);
            this.panel1.Controls.Add(this.dgvAssembylist);
            this.panel1.Controls.Add(this.txtSearch);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Location = new System.Drawing.Point(5, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(872, 397);
            this.panel1.TabIndex = 82;
            // 
            // gradientWaitingBar2
            // 
            this.gradientWaitingBar2.BackColor = System.Drawing.Color.White;
            this.gradientWaitingBar2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gradientWaitingBar2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.gradientWaitingBar2.GradientColor1 = System.Drawing.Color.Black;
            this.gradientWaitingBar2.GradientColor2 = System.Drawing.Color.White;
            this.gradientWaitingBar2.Interval = 10;
            this.gradientWaitingBar2.Location = new System.Drawing.Point(3, 379);
            this.gradientWaitingBar2.Name = "gradientWaitingBar2";
            this.gradientWaitingBar2.ScrollWAY = KDHLib.Controls.GradientWaitingBar.SCROLLGRADIENTALIGN.HORIZONTAL;
            this.gradientWaitingBar2.Size = new System.Drawing.Size(858, 10);
            this.gradientWaitingBar2.Speed = 1;
            this.gradientWaitingBar2.TabIndex = 82;
            this.gradientWaitingBar2.Text = "aaaaaa";
            // 
            // frmBOMList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(888, 413);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmBOMList";
            this.Text = "Select Assembly Adjustment";
            this.Load += new System.EventHandler(this.frmBOMList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAssembylist)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvAssembylist;
        private System.Windows.Forms.DataGridViewTextBoxColumn Reference;
        private System.Windows.Forms.DataGridViewTextBoxColumn AssemblyID;
        private System.Windows.Forms.DataGridViewTextBoxColumn AssemblyDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn date;
        private System.Windows.Forms.DataGridViewTextBoxColumn Qty;
        private System.Windows.Forms.DataGridViewTextBoxColumn WarehouseID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Action;
        private System.Windows.Forms.DataGridViewTextBoxColumn Status;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Panel panel1;
        private KDHLib.Controls.GradientWaitingBar gradientWaitingBar2;
    }
}