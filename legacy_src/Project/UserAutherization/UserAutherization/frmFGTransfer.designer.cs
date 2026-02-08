namespace WarehouseTransfer
{
    partial class frmFGTransfer
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFGTransfer));
            this.cmbWarehouse = new System.Windows.Forms.ComboBox();
            this.dgvIssueNote = new System.Windows.Forms.DataGridView();
            this.btnReset = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.txtIssueNoteId = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.btnSNO = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnclose = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.txtWarehouseAddress = new System.Windows.Forms.TextBox();
            this.txtWarehouseName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.mltcmbboxItemSelect = new MultiColumnComboBoxDemo.MultiColumnComboBox();
            this.cmbjob = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dtItemBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dSWhuseAndItemList = new WarehouseTransfer.DSWhuseAndItemList();
            this.Item = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UnitCost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvIssueNote)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtItemBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dSWhuseAndItemList)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbWarehouse
            // 
            this.cmbWarehouse.BackColor = System.Drawing.Color.White;
            this.cmbWarehouse.DisplayMember = "ItemId";
            this.cmbWarehouse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWarehouse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbWarehouse.FormattingEnabled = true;
            this.cmbWarehouse.Location = new System.Drawing.Point(114, 14);
            this.cmbWarehouse.Name = "cmbWarehouse";
            this.cmbWarehouse.Size = new System.Drawing.Size(141, 21);
            this.cmbWarehouse.TabIndex = 4;
            this.cmbWarehouse.ValueMember = "ItemId";
            this.cmbWarehouse.SelectedIndexChanged += new System.EventHandler(this.cmbWarehouse_SelectedIndexChanged);
            this.cmbWarehouse.TextChanged += new System.EventHandler(this.cmbWarehouse_TextChanged);
            // 
            // dgvIssueNote
            // 
            this.dgvIssueNote.BackgroundColor = System.Drawing.Color.White;
            this.dgvIssueNote.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvIssueNote.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Item,
            this.Description,
            this.Column1,
            this.Column2,
            this.Column3,
            this.UnitCost});
            this.dgvIssueNote.Location = new System.Drawing.Point(7, 124);
            this.dgvIssueNote.Name = "dgvIssueNote";
            this.dgvIssueNote.RowHeadersVisible = false;
            this.dgvIssueNote.Size = new System.Drawing.Size(839, 314);
            this.dgvIssueNote.TabIndex = 35;
            this.dgvIssueNote.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvIssueNote_CellLeave);
            this.dgvIssueNote.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvIssueNote_CellEndEdit);
            this.dgvIssueNote.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvIssueNote_CellClick);
            this.dgvIssueNote.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvIssueNote_CellEnter);
            // 
            // btnReset
            // 
            this.btnReset.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnReset.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnReset.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReset.Image = global::WarehouseTransfer.Properties.Resources.Transfer11;
            this.btnReset.Location = new System.Drawing.Point(503, 0);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(54, 35);
            this.btnReset.TabIndex = 5;
            this.btnReset.Text = "Reset";
            this.btnReset.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnReset.UseVisualStyleBackColor = false;
            this.btnReset.Visible = false;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(6, 15);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(79, 13);
            this.label10.TabIndex = 38;
            this.label10.Text = " Warehouse ID";
            // 
            // txtIssueNoteId
            // 
            this.txtIssueNoteId.BackColor = System.Drawing.Color.White;
            this.txtIssueNoteId.Enabled = false;
            this.txtIssueNoteId.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtIssueNoteId.Location = new System.Drawing.Point(690, 50);
            this.txtIssueNoteId.Name = "txtIssueNoteId";
            this.txtIssueNoteId.ReadOnly = true;
            this.txtIssueNoteId.Size = new System.Drawing.Size(155, 20);
            this.txtIssueNoteId.TabIndex = 1;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(605, 53);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(68, 13);
            this.label11.TabIndex = 50;
            this.label11.Text = "FG Trans No";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(7, 39);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(93, 13);
            this.label9.TabIndex = 40;
            this.label9.Text = "Warehouse Name";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel3.Controls.Add(this.button1);
            this.panel3.Controls.Add(this.btnSNO);
            this.panel3.Controls.Add(this.label6);
            this.panel3.Controls.Add(this.btnSearch);
            this.panel3.Controls.Add(this.btnReset);
            this.panel3.Controls.Add(this.btnSave);
            this.panel3.Controls.Add(this.btnPrint);
            this.panel3.Controls.Add(this.btnclose);
            this.panel3.Controls.Add(this.btnNew);
            this.panel3.Location = new System.Drawing.Point(4, 4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(852, 39);
            this.panel3.TabIndex = 48;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Image = global::WarehouseTransfer.Properties.Resources.Return;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button1.Location = new System.Drawing.Point(417, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(85, 34);
            this.button1.TabIndex = 61;
            this.button1.Text = "Remove";
            this.button1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnSNO
            // 
            this.btnSNO.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnSNO.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnSNO.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSNO.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSNO.Image = global::WarehouseTransfer.Properties.Resources.barcode1;
            this.btnSNO.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnSNO.Location = new System.Drawing.Point(277, 2);
            this.btnSNO.Name = "btnSNO";
            this.btnSNO.Size = new System.Drawing.Size(65, 35);
            this.btnSNO.TabIndex = 60;
            this.btnSNO.Text = "Serial";
            this.btnSNO.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSNO.UseVisualStyleBackColor = false;
            this.btnSNO.Click += new System.EventHandler(this.btnSNO_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(573, 5);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(280, 29);
            this.label6.TabIndex = 56;
            this.label6.Text = "Finished Goods Transfer";
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnSearch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearch.Image = global::WarehouseTransfer.Properties.Resources.listNEw;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnSearch.Location = new System.Drawing.Point(343, 2);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(73, 35);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Text = "List";
            this.btnSearch.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnSave.Image = global::WarehouseTransfer.Properties.Resources.CAIJ0ZXM;
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnSave.Location = new System.Drawing.Point(134, 2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(76, 35);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save/Print";
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnPrint.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnPrint.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPrint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrint.Image = global::WarehouseTransfer.Properties.Resources.rinticon;
            this.btnPrint.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnPrint.Location = new System.Drawing.Point(210, 2);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(65, 35);
            this.btnPrint.TabIndex = 3;
            this.btnPrint.Text = "Print";
            this.btnPrint.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnclose
            // 
            this.btnclose.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnclose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnclose.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnclose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnclose.Image = global::WarehouseTransfer.Properties.Resources.Close;
            this.btnclose.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnclose.Location = new System.Drawing.Point(4, 2);
            this.btnclose.Name = "btnclose";
            this.btnclose.Size = new System.Drawing.Size(66, 35);
            this.btnclose.TabIndex = 0;
            this.btnclose.Text = "Close";
            this.btnclose.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnclose.UseVisualStyleBackColor = false;
            this.btnclose.Click += new System.EventHandler(this.btnclose_Click);
            // 
            // btnNew
            // 
            this.btnNew.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnNew.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNew.Image = global::WarehouseTransfer.Properties.Resources.newButton;
            this.btnNew.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnNew.Location = new System.Drawing.Point(76, 2);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(54, 35);
            this.btnNew.TabIndex = 1;
            this.btnNew.Text = "New";
            this.btnNew.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnNew.UseVisualStyleBackColor = false;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // dtpDate
            // 
            this.dtpDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDate.Location = new System.Drawing.Point(685, 6);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(155, 20);
            this.dtpDate.TabIndex = 7;
            this.dtpDate.Value = new System.DateTime(2010, 12, 9, 0, 0, 0, 0);
            this.dtpDate.ValueChanged += new System.EventHandler(this.dtpDate_ValueChanged);
            // 
            // txtWarehouseAddress
            // 
            this.txtWarehouseAddress.AcceptsTab = true;
            this.txtWarehouseAddress.BackColor = System.Drawing.Color.White;
            this.txtWarehouseAddress.Enabled = false;
            this.txtWarehouseAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWarehouseAddress.Location = new System.Drawing.Point(114, 60);
            this.txtWarehouseAddress.Name = "txtWarehouseAddress";
            this.txtWarehouseAddress.ReadOnly = true;
            this.txtWarehouseAddress.Size = new System.Drawing.Size(141, 20);
            this.txtWarehouseAddress.TabIndex = 6;
            this.txtWarehouseAddress.Visible = false;
            // 
            // txtWarehouseName
            // 
            this.txtWarehouseName.AcceptsTab = true;
            this.txtWarehouseName.BackColor = System.Drawing.Color.White;
            this.txtWarehouseName.Enabled = false;
            this.txtWarehouseName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWarehouseName.Location = new System.Drawing.Point(114, 37);
            this.txtWarehouseName.Name = "txtWarehouseName";
            this.txtWarehouseName.ReadOnly = true;
            this.txtWarehouseName.Size = new System.Drawing.Size(141, 20);
            this.txtWarehouseName.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(8, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 41;
            this.label4.Text = "Address";
            this.label4.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(600, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Date";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.White;
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel4.Controls.Add(this.mltcmbboxItemSelect);
            this.panel4.Controls.Add(this.dgvIssueNote);
            this.panel4.Controls.Add(this.dtpDate);
            this.panel4.Controls.Add(this.txtWarehouseAddress);
            this.panel4.Controls.Add(this.txtWarehouseName);
            this.panel4.Controls.Add(this.label4);
            this.panel4.Controls.Add(this.label9);
            this.panel4.Controls.Add(this.cmbjob);
            this.panel4.Controls.Add(this.label1);
            this.panel4.Controls.Add(this.cmbWarehouse);
            this.panel4.Controls.Add(this.label10);
            this.panel4.Controls.Add(this.label2);
            this.panel4.Location = new System.Drawing.Point(3, 78);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(850, 445);
            this.panel4.TabIndex = 0;
            // 
            // mltcmbboxItemSelect
            // 
            this.mltcmbboxItemSelect.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.mltcmbboxItemSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mltcmbboxItemSelect.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.mltcmbboxItemSelect.FormattingEnabled = true;
            this.mltcmbboxItemSelect.Location = new System.Drawing.Point(10, 145);
            this.mltcmbboxItemSelect.Name = "mltcmbboxItemSelect";
            this.mltcmbboxItemSelect.Size = new System.Drawing.Size(130, 21);
            this.mltcmbboxItemSelect.TabIndex = 0;
            this.mltcmbboxItemSelect.Visible = false;
            this.mltcmbboxItemSelect.SelectionChangeCommitted += new System.EventHandler(this.mltcmbboxItemSelect_SelectionChangeCommitted);
            this.mltcmbboxItemSelect.SelectedIndexChanged += new System.EventHandler(this.mltcmbboxItemSelect_SelectedIndexChanged);
            this.mltcmbboxItemSelect.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mltcmbboxItemSelect_KeyDown);
            // 
            // cmbjob
            // 
            this.cmbjob.BackColor = System.Drawing.Color.White;
            this.cmbjob.DisplayMember = "ItemId";
            this.cmbjob.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbjob.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbjob.FormattingEnabled = true;
            this.cmbjob.Location = new System.Drawing.Point(685, 31);
            this.cmbjob.Name = "cmbjob";
            this.cmbjob.Size = new System.Drawing.Size(155, 21);
            this.cmbjob.TabIndex = 4;
            this.cmbjob.ValueMember = "ItemId";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(600, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 38;
            this.label1.Text = "Select a Job";
            // 
            // dtItemBindingSource
            // 
            this.dtItemBindingSource.DataMember = "Dt_Item";
            this.dtItemBindingSource.DataSource = this.dSWhuseAndItemList;
            // 
            // dSWhuseAndItemList
            // 
            this.dSWhuseAndItemList.DataSetName = "DSWhuseAndItemList";
            this.dSWhuseAndItemList.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // Item
            // 
            this.Item.HeaderText = "Item";
            this.Item.Name = "Item";
            this.Item.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Item.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Item.Width = 132;
            // 
            // Description
            // 
            this.Description.HeaderText = "Description";
            this.Description.Name = "Description";
            this.Description.ReadOnly = true;
            this.Description.Width = 250;
            // 
            // Column1
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.Column1.DefaultCellStyle = dataGridViewCellStyle1;
            this.Column1.HeaderText = "On Hand Qty";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 125;
            // 
            // Column2
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.Column2.DefaultCellStyle = dataGridViewCellStyle2;
            this.Column2.HeaderText = "Transfer Qty";
            this.Column2.MaxInputLength = 13;
            this.Column2.Name = "Column2";
            this.Column2.Width = 125;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "UOM";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            // 
            // UnitCost
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.UnitCost.DefaultCellStyle = dataGridViewCellStyle3;
            this.UnitCost.HeaderText = "Unit Cost";
            this.UnitCost.Name = "UnitCost";
            // 
            // frmFGTransfer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(857, 535);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.txtIssueNoteId);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.panel3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(145, 155);
            this.Name = "frmFGTransfer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FG Transfer";
            this.Load += new System.EventHandler(this.frmIssueNote_Load);
            this.Activated += new System.EventHandler(this.frmReturnNote_Activated);
            ((System.ComponentModel.ISupportInitialize)(this.dgvIssueNote)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtItemBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dSWhuseAndItemList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ComboBox cmbWarehouse;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnclose;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtIssueNoteId;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.TextBox txtWarehouseAddress;
        private System.Windows.Forms.TextBox txtWarehouseName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.DataGridView dgvIssueNote;
        private System.Windows.Forms.BindingSource dtItemBindingSource;
        private DSWhuseAndItemList dSWhuseAndItemList;
        private MultiColumnComboBoxDemo.MultiColumnComboBox mltcmbboxItemSelect;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnSNO;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox cmbjob;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Item;
        private System.Windows.Forms.DataGridViewTextBoxColumn Description;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn UnitCost;

    }
}