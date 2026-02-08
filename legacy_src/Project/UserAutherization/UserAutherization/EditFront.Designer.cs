namespace UserAutherization
{
    partial class EditFront
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
            this.dgvAuthentication = new System.Windows.Forms.DataGridView();
            this.btnSave = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAuthentication)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvAuthentication
            // 
            this.dgvAuthentication.BackgroundColor = System.Drawing.Color.White;
            this.dgvAuthentication.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAuthentication.GridColor = System.Drawing.Color.DarkBlue;
            this.dgvAuthentication.Location = new System.Drawing.Point(22, 46);
            this.dgvAuthentication.Name = "dgvAuthentication";
            this.dgvAuthentication.RowHeadersVisible = false;
            this.dgvAuthentication.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvAuthentication.Size = new System.Drawing.Size(658, 289);
            this.dgvAuthentication.TabIndex = 2;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(22, 12);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(91, 27);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // EditFront
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(712, 382);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dgvAuthentication);
            this.Name = "EditFront";
            this.Text = "EditFront";
            this.Load += new System.EventHandler(this.EditFront_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAuthentication)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvAuthentication;
        private System.Windows.Forms.Button btnSave;
    }
}