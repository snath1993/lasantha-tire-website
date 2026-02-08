using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.VisualBasic.Compatibility;
using Interop.PeachwServer;

//namespace CSSDK
    namespace UserAutherization
{
	public class frmOpenCompany : System.Windows.Forms.Form
	{
		private CompanyInfoList compList;
		private CompanyInfo compInfo;
        public bool withGUID;
        private Connect ptApp = new Connect();
        //public  ListView listView1;
        public Microsoft.VisualBasic.Compatibility.VB6.DriveListBox driveListBox1;
        public Microsoft.VisualBasic.Compatibility.VB6.DirListBox dirListBox1;
        private ListView listView1;
        private Button btnclose;
       // private Connector ptApp = new Connector();
       
		private System.ComponentModel.Container components = null;

		public frmOpenCompany()
		{
			InitializeComponent();
		}



		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		private void InitializeComponent()
		{
            this.dirListBox1 = new Microsoft.VisualBasic.Compatibility.VB6.DirListBox();
            this.driveListBox1 = new Microsoft.VisualBasic.Compatibility.VB6.DriveListBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.btnclose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // dirListBox1
            // 
            this.dirListBox1.BackColor = System.Drawing.Color.White;
            this.dirListBox1.FormattingEnabled = true;
            this.dirListBox1.IntegralHeight = false;
            this.dirListBox1.Location = new System.Drawing.Point(442, 12);
            this.dirListBox1.Name = "dirListBox1";
            this.dirListBox1.Size = new System.Drawing.Size(198, 188);
            this.dirListBox1.TabIndex = 0;
            this.dirListBox1.SelectedIndexChanged += new System.EventHandler(this.dirListBox1_SelectedIndexChanged);
            this.dirListBox1.DoubleClick += new System.EventHandler(this.dirListBox1_DoubleClick);
            // 
            // driveListBox1
            // 
            this.driveListBox1.BackColor = System.Drawing.Color.White;
            this.driveListBox1.FormattingEnabled = true;
            this.driveListBox1.Location = new System.Drawing.Point(440, 203);
            this.driveListBox1.Name = "driveListBox1";
            this.driveListBox1.Size = new System.Drawing.Size(200, 21);
            this.driveListBox1.TabIndex = 1;
            this.driveListBox1.SelectedIndexChanged += new System.EventHandler(this.driveListBox1_SelectedIndexChanged);
            // 
            // listView1
            // 
            this.listView1.BackColor = System.Drawing.Color.White;
            this.listView1.Location = new System.Drawing.Point(12, 8);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(424, 216);
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            // 
            // btnclose
            // 
            this.btnclose.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnclose.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnclose.Location = new System.Drawing.Point(565, 230);
            this.btnclose.Name = "btnclose";
            this.btnclose.Size = new System.Drawing.Size(75, 35);
            this.btnclose.TabIndex = 3;
            this.btnclose.Text = "Close";
            this.btnclose.UseVisualStyleBackColor = false;
            this.btnclose.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // frmOpenCompany
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(648, 270);
            this.Controls.Add(this.btnclose);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.driveListBox1);
            this.Controls.Add(this.dirListBox1);
            this.Location = new System.Drawing.Point(10, 10);
            this.Name = "frmOpenCompany";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Open Company";
            this.Load += new System.EventHandler(this.frmOpenCompany_Load);
            this.ResumeLayout(false);

		}
		#endregion

		public void CompanyList(bool byGuid)
		{
            compList = (Interop.PeachwServer.CompanyInfoList)ptApp.app.GetCompanyInfoList(this.dirListBox1.Path.ToString());
			listView1.Items.Clear();
			listView1.Columns.Clear();
			if(byGuid == true)
			{
				this.listView1.Columns.Add("GUID", -2, HorizontalAlignment.Left);
				this.listView1.Columns.Add("Company Name", -2, HorizontalAlignment.Left);
				this.listView1.Columns.Add("Path", -2, HorizontalAlignment.Left);
				for(int i = 0;i <= compList.Count -1;i++)
				{
					compInfo = (CompanyInfo)compList.Item(i);
					this.listView1.Items.Add(compInfo.GUID);
					this.listView1.Items[i].SubItems.Add(compInfo.Name);
					this.listView1.Items[i].SubItems.Add(compInfo.Path);
				}
			}
			else
			{	
				this.listView1.Columns.Add("Company Name", -2, HorizontalAlignment.Left);
				this.listView1.Columns.Add("Path", -2, HorizontalAlignment.Left);
				for(int i = 0;i <= compList.Count - 1;i++)
				{
					compInfo = (CompanyInfo)compList.Item(i);
					this.listView1.Items.Add(compInfo.Name);
					this.listView1.Items[i].SubItems.Add(compInfo.Path);
				}
			}
			this.listView1.View = View.Details;
			foreach(ColumnHeader col in listView1.Columns)
			{
				col.Width = -2;
			}
			compList = null;
			compInfo = null;
		}
		
		private void frmOpenCompany_Load(object sender, System.EventArgs e)
		{
			this.dirListBox1.Path = ptApp.app.DataPath.ToString();
			this.driveListBox1.Drive = ptApp.app.DataPath.ToString();

		}

		private void dirListBox1_DoubleClick(object sender, System.EventArgs e)
		{
			CompanyList(withGUID);
		}

		private void driveListBox1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			this.dirListBox1.Path = driveListBox1.Drive.ToString();
			CompanyList(withGUID);
		}

		private void listView1_DoubleClick(object sender, System.EventArgs e)
		{
           // string sPath;
           // if (withGUID == true)
           // {
           //     sPath = this.listView1.Items[listView1.FocusedItem.Index].SubItems[2].Text;
           // }
           // else
           // {
           //     sPath = this.listView1.Items[listView1.FocusedItem.Index].SubItems[1].Text;
           // }

           // if (ptApp.app.CheckCompanyUsesPasswords(sPath))
           // {
           //     frmCompany_Password dlgPassword = new frmCompany_Password();

           //     if (dlgPassword.ShowDialog() == System.Windows.Forms.DialogResult.OK)
           //     {
           //        // string sUserName = dlgPassword.textBoxUserName.Text;//Tom Aligood
           //        // string sUserPassword = dlgPassword.textBoxUserPassword.Text;//3M3336RJP111X7A

           //         string sUserName = "Tom Aligood";
           //         string sUserPassword = "3M3336RJP111X7A";


           //         ptApp.app.OpenCompanySecure(sPath, sUserName, sUserPassword);
           //     }

           // }
           // else
           // {
           //     if (withGUID == true)
           //         ptApp.app.OpenCompanyByGUID(this.listView1.Items[listView1.FocusedItem.Index].Text);
           //     else
           //         ptApp.app.OpenCompany(sPath);
           // }
           //// this.Dispose();
           // //frmMain mm = new frmMain();
           //// mm.GetcompanyName();
           //// this.Close();
           // this.Dispose();

            
		}

        private void dirListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
           // this.Close();
            frmMain aa = new frmMain();
            aa.Text = ptApp.app.CurrentCompanyName;
            this.Close();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Close();
           // frmMain mm = new frmMain();
          //  mm.GetcompanyName();
           // this.Close();
        }
	}
}
