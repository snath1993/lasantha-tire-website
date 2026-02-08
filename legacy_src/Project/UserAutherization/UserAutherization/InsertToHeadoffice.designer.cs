namespace UserAutherization
{
    partial class frmInsertToHeadoffice
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
            this.btnloadInvAdjust = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtInvAdjustment = new System.Windows.Forms.TextBox();
            this.txtTransferNotes = new System.Windows.Forms.TextBox();
            this.txtCustomer = new System.Windows.Forms.TextBox();
            this.txtInvoices = new System.Windows.Forms.TextBox();
            this.txtCrteditNotes = new System.Windows.Forms.TextBox();
            this.btnTransload = new System.Windows.Forms.Button();
            this.btnCusLoad = new System.Windows.Forms.Button();
            this.btnInvoiceLoad = new System.Windows.Forms.Button();
            this.btnCreditLoad = new System.Windows.Forms.Button();
            this.gbCreateXML = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.btnInserFiles = new System.Windows.Forms.Button();
            this.gbSelectFile = new System.Windows.Forms.GroupBox();
            this.chkReceipts = new System.Windows.Forms.CheckBox();
            this.chkInvoices = new System.Windows.Forms.CheckBox();
            this.chkCreditNote = new System.Windows.Forms.CheckBox();
            this.chkCustomerMasters = new System.Windows.Forms.CheckBox();
            this.chkTransfers = new System.Windows.Forms.CheckBox();
            this.chkInvAdjustments = new System.Windows.Forms.CheckBox();
            this.txtReceiptPath = new System.Windows.Forms.TextBox();
            this.txtReceipts = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.txtAdjustPath = new System.Windows.Forms.TextBox();
            this.txtCreditPath = new System.Windows.Forms.TextBox();
            this.txtInvoicesPath = new System.Windows.Forms.TextBox();
            this.txtCustomerPath = new System.Windows.Forms.TextBox();
            this.txtTransPath = new System.Windows.Forms.TextBox();
            this.OFD = new System.Windows.Forms.OpenFileDialog();
            this.gbCreateXML.SuspendLayout();
            this.gbSelectFile.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnloadInvAdjust
            // 
            this.btnloadInvAdjust.BackColor = System.Drawing.Color.Transparent;
            this.btnloadInvAdjust.Font = new System.Drawing.Font("Rockwell", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnloadInvAdjust.ForeColor = System.Drawing.Color.Black;
            this.btnloadInvAdjust.Location = new System.Drawing.Point(529, 14);
            this.btnloadInvAdjust.Name = "btnloadInvAdjust";
            this.btnloadInvAdjust.Size = new System.Drawing.Size(30, 25);
            this.btnloadInvAdjust.TabIndex = 4;
            this.btnloadInvAdjust.Text = ".....";
            this.btnloadInvAdjust.UseVisualStyleBackColor = false;
            this.btnloadInvAdjust.Click += new System.EventHandler(this.btnloadInvAdjust_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(138, 13);
            this.label3.TabIndex = 23;
            this.label3.Text = "Inventory Adjustments";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 174);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 24;
            this.label1.Text = "Transfer Notes";
            this.label1.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 13);
            this.label2.TabIndex = 25;
            this.label2.Text = "Customer Master";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 76);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 26;
            this.label4.Text = "Invoices";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 105);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 13);
            this.label5.TabIndex = 27;
            this.label5.Text = "CreditNotes";
            // 
            // txtInvAdjustment
            // 
            this.txtInvAdjustment.BackColor = System.Drawing.Color.White;
            this.txtInvAdjustment.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtInvAdjustment.Location = new System.Drawing.Point(371, 16);
            this.txtInvAdjustment.Name = "txtInvAdjustment";
            this.txtInvAdjustment.ReadOnly = true;
            this.txtInvAdjustment.Size = new System.Drawing.Size(157, 21);
            this.txtInvAdjustment.TabIndex = 28;
            // 
            // txtTransferNotes
            // 
            this.txtTransferNotes.BackColor = System.Drawing.Color.White;
            this.txtTransferNotes.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTransferNotes.Location = new System.Drawing.Point(370, 162);
            this.txtTransferNotes.Name = "txtTransferNotes";
            this.txtTransferNotes.ReadOnly = true;
            this.txtTransferNotes.Size = new System.Drawing.Size(157, 21);
            this.txtTransferNotes.TabIndex = 28;
            this.txtTransferNotes.Visible = false;
            // 
            // txtCustomer
            // 
            this.txtCustomer.BackColor = System.Drawing.Color.White;
            this.txtCustomer.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCustomer.Location = new System.Drawing.Point(371, 44);
            this.txtCustomer.Name = "txtCustomer";
            this.txtCustomer.ReadOnly = true;
            this.txtCustomer.Size = new System.Drawing.Size(157, 21);
            this.txtCustomer.TabIndex = 28;
            // 
            // txtInvoices
            // 
            this.txtInvoices.BackColor = System.Drawing.Color.White;
            this.txtInvoices.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtInvoices.Location = new System.Drawing.Point(371, 72);
            this.txtInvoices.Name = "txtInvoices";
            this.txtInvoices.ReadOnly = true;
            this.txtInvoices.Size = new System.Drawing.Size(157, 21);
            this.txtInvoices.TabIndex = 28;
            // 
            // txtCrteditNotes
            // 
            this.txtCrteditNotes.BackColor = System.Drawing.Color.White;
            this.txtCrteditNotes.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCrteditNotes.Location = new System.Drawing.Point(371, 101);
            this.txtCrteditNotes.Name = "txtCrteditNotes";
            this.txtCrteditNotes.ReadOnly = true;
            this.txtCrteditNotes.Size = new System.Drawing.Size(157, 21);
            this.txtCrteditNotes.TabIndex = 28;
            // 
            // btnTransload
            // 
            this.btnTransload.BackColor = System.Drawing.Color.Transparent;
            this.btnTransload.Font = new System.Drawing.Font("Rockwell", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTransload.ForeColor = System.Drawing.Color.Black;
            this.btnTransload.Location = new System.Drawing.Point(531, 160);
            this.btnTransload.Name = "btnTransload";
            this.btnTransload.Size = new System.Drawing.Size(30, 25);
            this.btnTransload.TabIndex = 4;
            this.btnTransload.Text = "....";
            this.btnTransload.UseVisualStyleBackColor = false;
            this.btnTransload.Visible = false;
            this.btnTransload.Click += new System.EventHandler(this.btnTransload_Click);
            // 
            // btnCusLoad
            // 
            this.btnCusLoad.BackColor = System.Drawing.Color.Transparent;
            this.btnCusLoad.Font = new System.Drawing.Font("Rockwell", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCusLoad.ForeColor = System.Drawing.Color.Black;
            this.btnCusLoad.Location = new System.Drawing.Point(529, 42);
            this.btnCusLoad.Name = "btnCusLoad";
            this.btnCusLoad.Size = new System.Drawing.Size(30, 25);
            this.btnCusLoad.TabIndex = 4;
            this.btnCusLoad.Text = "....";
            this.btnCusLoad.UseVisualStyleBackColor = false;
            this.btnCusLoad.Click += new System.EventHandler(this.btnCusLoad_Click);
            // 
            // btnInvoiceLoad
            // 
            this.btnInvoiceLoad.BackColor = System.Drawing.Color.Transparent;
            this.btnInvoiceLoad.Font = new System.Drawing.Font("Rockwell", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInvoiceLoad.ForeColor = System.Drawing.Color.Black;
            this.btnInvoiceLoad.Location = new System.Drawing.Point(529, 70);
            this.btnInvoiceLoad.Name = "btnInvoiceLoad";
            this.btnInvoiceLoad.Size = new System.Drawing.Size(30, 25);
            this.btnInvoiceLoad.TabIndex = 4;
            this.btnInvoiceLoad.Text = ".....";
            this.btnInvoiceLoad.UseVisualStyleBackColor = false;
            this.btnInvoiceLoad.Click += new System.EventHandler(this.btnInvoiceLoad_Click);
            // 
            // btnCreditLoad
            // 
            this.btnCreditLoad.BackColor = System.Drawing.Color.Transparent;
            this.btnCreditLoad.Font = new System.Drawing.Font("Rockwell", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCreditLoad.ForeColor = System.Drawing.Color.Black;
            this.btnCreditLoad.Location = new System.Drawing.Point(529, 99);
            this.btnCreditLoad.Name = "btnCreditLoad";
            this.btnCreditLoad.Size = new System.Drawing.Size(30, 25);
            this.btnCreditLoad.TabIndex = 4;
            this.btnCreditLoad.Text = ".....";
            this.btnCreditLoad.UseVisualStyleBackColor = false;
            this.btnCreditLoad.Click += new System.EventHandler(this.btnCreditLoad_Click);
            // 
            // gbCreateXML
            // 
            this.gbCreateXML.BackColor = System.Drawing.Color.WhiteSmoke;
            this.gbCreateXML.Controls.Add(this.button1);
            this.gbCreateXML.Controls.Add(this.label6);
            this.gbCreateXML.Controls.Add(this.btnInserFiles);
            this.gbCreateXML.Controls.Add(this.gbSelectFile);
            this.gbCreateXML.ForeColor = System.Drawing.Color.Black;
            this.gbCreateXML.Location = new System.Drawing.Point(10, 2);
            this.gbCreateXML.Name = "gbCreateXML";
            this.gbCreateXML.Size = new System.Drawing.Size(625, 252);
            this.gbCreateXML.TabIndex = 31;
            this.gbCreateXML.TabStop = false;
            this.gbCreateXML.Text = " ";
            this.gbCreateXML.UseCompatibleTextRendering = true;
            this.gbCreateXML.Enter += new System.EventHandler(this.gbCreateXML_Enter);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.button1.ForeColor = System.Drawing.Color.Black;
            this.button1.Image = global::UserAutherization.Properties.Resources.Close;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(551, 215);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(60, 25);
            this.button1.TabIndex = 25;
            this.button1.Text = "Exit";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Rockwell", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(26, 303);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(0, 19);
            this.label6.TabIndex = 22;
            // 
            // btnInserFiles
            // 
            this.btnInserFiles.Location = new System.Drawing.Point(464, 215);
            this.btnInserFiles.Name = "btnInserFiles";
            this.btnInserFiles.Size = new System.Drawing.Size(82, 25);
            this.btnInserFiles.TabIndex = 19;
            this.btnInserFiles.Text = "Inser Files";
            this.btnInserFiles.UseVisualStyleBackColor = true;
            this.btnInserFiles.Click += new System.EventHandler(this.btnInserFiles_Click);
            // 
            // gbSelectFile
            // 
            this.gbSelectFile.BackColor = System.Drawing.Color.WhiteSmoke;
            this.gbSelectFile.Controls.Add(this.chkReceipts);
            this.gbSelectFile.Controls.Add(this.chkInvoices);
            this.gbSelectFile.Controls.Add(this.chkCreditNote);
            this.gbSelectFile.Controls.Add(this.chkCustomerMasters);
            this.gbSelectFile.Controls.Add(this.chkTransfers);
            this.gbSelectFile.Controls.Add(this.chkInvAdjustments);
            this.gbSelectFile.Controls.Add(this.txtReceiptPath);
            this.gbSelectFile.Controls.Add(this.txtReceipts);
            this.gbSelectFile.Controls.Add(this.label7);
            this.gbSelectFile.Controls.Add(this.button2);
            this.gbSelectFile.Controls.Add(this.label3);
            this.gbSelectFile.Controls.Add(this.btnloadInvAdjust);
            this.gbSelectFile.Controls.Add(this.txtAdjustPath);
            this.gbSelectFile.Controls.Add(this.txtInvAdjustment);
            this.gbSelectFile.Controls.Add(this.txtCreditPath);
            this.gbSelectFile.Controls.Add(this.txtCrteditNotes);
            this.gbSelectFile.Controls.Add(this.btnTransload);
            this.gbSelectFile.Controls.Add(this.txtInvoicesPath);
            this.gbSelectFile.Controls.Add(this.label5);
            this.gbSelectFile.Controls.Add(this.txtInvoices);
            this.gbSelectFile.Controls.Add(this.txtCustomerPath);
            this.gbSelectFile.Controls.Add(this.btnCreditLoad);
            this.gbSelectFile.Controls.Add(this.txtCustomer);
            this.gbSelectFile.Controls.Add(this.txtTransPath);
            this.gbSelectFile.Controls.Add(this.label1);
            this.gbSelectFile.Controls.Add(this.txtTransferNotes);
            this.gbSelectFile.Controls.Add(this.label4);
            this.gbSelectFile.Controls.Add(this.btnCusLoad);
            this.gbSelectFile.Controls.Add(this.label2);
            this.gbSelectFile.Controls.Add(this.btnInvoiceLoad);
            this.gbSelectFile.Location = new System.Drawing.Point(13, 14);
            this.gbSelectFile.Name = "gbSelectFile";
            this.gbSelectFile.Size = new System.Drawing.Size(601, 195);
            this.gbSelectFile.TabIndex = 18;
            this.gbSelectFile.TabStop = false;
            this.gbSelectFile.Text = "Select Files";
            // 
            // chkReceipts
            // 
            this.chkReceipts.AutoSize = true;
            this.chkReceipts.Location = new System.Drawing.Point(580, 136);
            this.chkReceipts.Name = "chkReceipts";
            this.chkReceipts.Size = new System.Drawing.Size(15, 14);
            this.chkReceipts.TabIndex = 38;
            this.chkReceipts.UseVisualStyleBackColor = true;
            // 
            // chkInvoices
            // 
            this.chkInvoices.AutoSize = true;
            this.chkInvoices.Location = new System.Drawing.Point(580, 79);
            this.chkInvoices.Name = "chkInvoices";
            this.chkInvoices.Size = new System.Drawing.Size(15, 14);
            this.chkInvoices.TabIndex = 37;
            this.chkInvoices.UseVisualStyleBackColor = true;
            // 
            // chkCreditNote
            // 
            this.chkCreditNote.AutoSize = true;
            this.chkCreditNote.Location = new System.Drawing.Point(580, 110);
            this.chkCreditNote.Name = "chkCreditNote";
            this.chkCreditNote.Size = new System.Drawing.Size(15, 14);
            this.chkCreditNote.TabIndex = 36;
            this.chkCreditNote.UseVisualStyleBackColor = true;
            // 
            // chkCustomerMasters
            // 
            this.chkCustomerMasters.AutoSize = true;
            this.chkCustomerMasters.Location = new System.Drawing.Point(580, 50);
            this.chkCustomerMasters.Name = "chkCustomerMasters";
            this.chkCustomerMasters.Size = new System.Drawing.Size(15, 14);
            this.chkCustomerMasters.TabIndex = 35;
            this.chkCustomerMasters.UseVisualStyleBackColor = true;
            // 
            // chkTransfers
            // 
            this.chkTransfers.AutoSize = true;
            this.chkTransfers.Location = new System.Drawing.Point(579, 165);
            this.chkTransfers.Name = "chkTransfers";
            this.chkTransfers.Size = new System.Drawing.Size(15, 14);
            this.chkTransfers.TabIndex = 34;
            this.chkTransfers.UseVisualStyleBackColor = true;
            this.chkTransfers.Visible = false;
            // 
            // chkInvAdjustments
            // 
            this.chkInvAdjustments.AutoSize = true;
            this.chkInvAdjustments.Location = new System.Drawing.Point(580, 18);
            this.chkInvAdjustments.Name = "chkInvAdjustments";
            this.chkInvAdjustments.Size = new System.Drawing.Size(15, 14);
            this.chkInvAdjustments.TabIndex = 33;
            this.chkInvAdjustments.UseVisualStyleBackColor = true;
            // 
            // txtReceiptPath
            // 
            this.txtReceiptPath.BackColor = System.Drawing.Color.White;
            this.txtReceiptPath.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtReceiptPath.Location = new System.Drawing.Point(155, 130);
            this.txtReceiptPath.Name = "txtReceiptPath";
            this.txtReceiptPath.ReadOnly = true;
            this.txtReceiptPath.Size = new System.Drawing.Size(202, 21);
            this.txtReceiptPath.TabIndex = 31;
            // 
            // txtReceipts
            // 
            this.txtReceipts.BackColor = System.Drawing.Color.White;
            this.txtReceipts.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtReceipts.Location = new System.Drawing.Point(371, 130);
            this.txtReceipts.Name = "txtReceipts";
            this.txtReceipts.ReadOnly = true;
            this.txtReceipts.Size = new System.Drawing.Size(157, 21);
            this.txtReceipts.TabIndex = 32;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(17, 137);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 13);
            this.label7.TabIndex = 30;
            this.label7.Text = "Receipts";
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Transparent;
            this.button2.Font = new System.Drawing.Font("Rockwell", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.Color.Black;
            this.button2.Location = new System.Drawing.Point(529, 128);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(30, 25);
            this.button2.TabIndex = 29;
            this.button2.Text = ".....";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // txtAdjustPath
            // 
            this.txtAdjustPath.BackColor = System.Drawing.Color.White;
            this.txtAdjustPath.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAdjustPath.Location = new System.Drawing.Point(155, 16);
            this.txtAdjustPath.Name = "txtAdjustPath";
            this.txtAdjustPath.ReadOnly = true;
            this.txtAdjustPath.Size = new System.Drawing.Size(202, 21);
            this.txtAdjustPath.TabIndex = 28;
            // 
            // txtCreditPath
            // 
            this.txtCreditPath.BackColor = System.Drawing.Color.White;
            this.txtCreditPath.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCreditPath.Location = new System.Drawing.Point(155, 101);
            this.txtCreditPath.Name = "txtCreditPath";
            this.txtCreditPath.ReadOnly = true;
            this.txtCreditPath.Size = new System.Drawing.Size(202, 21);
            this.txtCreditPath.TabIndex = 28;
            // 
            // txtInvoicesPath
            // 
            this.txtInvoicesPath.BackColor = System.Drawing.Color.White;
            this.txtInvoicesPath.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtInvoicesPath.Location = new System.Drawing.Point(155, 72);
            this.txtInvoicesPath.Name = "txtInvoicesPath";
            this.txtInvoicesPath.ReadOnly = true;
            this.txtInvoicesPath.Size = new System.Drawing.Size(202, 21);
            this.txtInvoicesPath.TabIndex = 28;
            // 
            // txtCustomerPath
            // 
            this.txtCustomerPath.BackColor = System.Drawing.Color.White;
            this.txtCustomerPath.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCustomerPath.Location = new System.Drawing.Point(155, 44);
            this.txtCustomerPath.Name = "txtCustomerPath";
            this.txtCustomerPath.ReadOnly = true;
            this.txtCustomerPath.Size = new System.Drawing.Size(202, 21);
            this.txtCustomerPath.TabIndex = 28;
            // 
            // txtTransPath
            // 
            this.txtTransPath.BackColor = System.Drawing.Color.White;
            this.txtTransPath.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTransPath.Location = new System.Drawing.Point(163, 163);
            this.txtTransPath.Name = "txtTransPath";
            this.txtTransPath.ReadOnly = true;
            this.txtTransPath.Size = new System.Drawing.Size(202, 21);
            this.txtTransPath.TabIndex = 28;
            this.txtTransPath.Visible = false;
            // 
            // OFD
            // 
            this.OFD.FileName = "openFileDialog1";
            // 
            // frmInsertToHeadoffice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(644, 260);
            this.Controls.Add(this.gbCreateXML);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frmInsertToHeadoffice";
            this.Text = "Insert File";
            this.Load += new System.EventHandler(this.frmInsertToHeadoffice_Load);
            this.gbCreateXML.ResumeLayout(false);
            this.gbCreateXML.PerformLayout();
            this.gbSelectFile.ResumeLayout(false);
            this.gbSelectFile.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Button btnloadInvAdjust;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtInvAdjustment;
        private System.Windows.Forms.TextBox txtTransferNotes;
        private System.Windows.Forms.TextBox txtCustomer;
        private System.Windows.Forms.TextBox txtInvoices;
        private System.Windows.Forms.TextBox txtCrteditNotes;
        public System.Windows.Forms.Button btnTransload;
        public System.Windows.Forms.Button btnCusLoad;
        public System.Windows.Forms.Button btnInvoiceLoad;
        public System.Windows.Forms.Button btnCreditLoad;
        private System.Windows.Forms.GroupBox gbCreateXML;
        public System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnInserFiles;
        private System.Windows.Forms.GroupBox gbSelectFile;
        private System.Windows.Forms.OpenFileDialog OFD;
        private System.Windows.Forms.TextBox txtAdjustPath;
        private System.Windows.Forms.TextBox txtCreditPath;
        private System.Windows.Forms.TextBox txtInvoicesPath;
        private System.Windows.Forms.TextBox txtCustomerPath;
        private System.Windows.Forms.TextBox txtTransPath;
        private System.Windows.Forms.TextBox txtReceiptPath;
        private System.Windows.Forms.TextBox txtReceipts;
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox chkReceipts;
        private System.Windows.Forms.CheckBox chkInvoices;
        private System.Windows.Forms.CheckBox chkCreditNote;
        private System.Windows.Forms.CheckBox chkCustomerMasters;
        private System.Windows.Forms.CheckBox chkTransfers;
        private System.Windows.Forms.CheckBox chkInvAdjustments;
    }
}