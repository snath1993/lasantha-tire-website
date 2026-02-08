using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using Infragistics.Win.UltraWinGrid;

namespace UserAutherization
{
    public partial class frmLabPackages : Form
    {
        public static string ConnectionString;
        public frmLabPackages()
        {
            InitializeComponent();
            setConnectionString();
        }
        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch (Exception ex)
            {
              
            }
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void frmLabPackages_Load(object sender, EventArgs e)
        {
            //getItem();
            gettypecu();
            getPackages();
        }
        public DataSet dsPack;
        private void getPackages()
        {
            dsPack = new DataSet();
            try
            {
                dsPack.Clear();


                String StrSql = "SELECT DISTINCT PackageName FROM tblLabPackage ORDER BY PackageName ";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsPack, "DtClient");

                cmbPackageType.DataSource = dsPack.Tables["DtClient"];
                cmbPackageType.DisplayMember = "PackageName";
                cmbPackageType.ValueMember = "PackageName";

                cmbPackageType.DisplayLayout.Bands["DtClient"].Columns["PackageName"].Width = 350;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void getItem()
        {
            try {
                String S = "Select  ItemID, ItemDescription, HospitalFee,GLAccount from View_GetPackageItem ORDER BY ItemID ";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                        UltraGridRow ugR;
                        foreach (DataRow Dr in dt.Rows)
                        {
                            ugR = dgvItems.DisplayLayout.Bands[0].AddNew();
                            ugR.Cells["LineNo"].Value = ugR.Index + 1;
                            ugR.Cells["ItemID"].Value = Dr["ItemID"];
                            ugR.Cells["ItemDescription"].Value = Dr["ItemDescription"];
                            ugR.Cells["Price"].Value = Dr["HospitalFee"];
                            ugR.Cells["Check"].Value = false;
                        ugR.Cells["GL"].Value = Dr["GLAccount"];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnNewn_Click(object sender, EventArgs e)
        {
            Eneble();
            
        }

        private void Eneble()
        {
            try
            {
                btnVoid.Enabled = false;
                cmbPackageType.Enabled = true;
                txtAmount.Enabled = true;
                dgvItems.Enabled = true;
               // txtTotal.Enabled = true;
                cmbPackageType.ResetText();
                cmbTestType.Enabled = true;
               
                if (dgvItems.Rows == null) return;
                foreach (UltraGridRow ugR in dgvItems.Rows.All)
                {
                    ugR.Delete(false);
                }
                txtAmount.Value = 0;
                txtTotal.Value = 0;
                cmbTestType.ResetText();
                txtConsultant.Text = string.Empty;
                txtScanName.Text = string.Empty;
                // getItem();

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvItems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
           
        }
        double checkvale;

        private void dgvItems_AfterExitEditMode(object sender, EventArgs e)
        {
            
        }

        private void dgvItems_AfterCellUpdate(object sender, CellEventArgs e)
        {
            //getitemvalue();

        }

        private void getitemvalue()
        {
            try
            {
                checkvale = 0;
                //if (dgvItems.ActiveCell.Column.Key == "LineTotal")
                //{
                ////    //if (Convert.ToBoolean(dgvItems.ActiveRow.Cells["Check"].Value.ToString()) == true)
                    //{
                        for (int i = 0; i  < dgvItems.Rows.Count; i++)
                    {

                       
                            checkvale += double.Parse(dgvItems.Rows[i].Cells["LineTotal"].Value.ToString());
                        
                    }
                    double totalvalu = checkvale;
                   txtTotal.Value = totalvalu;
                    //}

                //}
                //{

                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvItems_BeforeExitEditMode(object sender, BeforeExitEditModeEventArgs e)
        {
          
        }

        private void dgvItems_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;
            if (cmbPackageType.Text.ToString() == string.Empty)
            {
                MessageBox.Show("You Must Select Package", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                DialogResult reply = MessageBox.Show("Are you sure, you want to Save this record?", "Information", MessageBoxButtons.OKCancel);

                if (reply == DialogResult.Cancel)
                {
                    return;
                }

                myConnection.Open();
                myTrans = myConnection.BeginTransaction();
                dgvItems.DisplayLayout.Bands[0].AddNew();

                String S = "Select * from tblLabPackage WHERE PackageName='" + cmbPackageType.Text.ToString().Trim() + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    string S5 = " DELETE FROM tblLabPackage WHERE PackageName='" + cmbPackageType.Text.ToString() + "' ";
                    SqlCommand cmd5 = new SqlCommand(S5, myConnection, myTrans);
                    SqlDataAdapter da5 = new SqlDataAdapter(cmd5);
                    DataTable dt5 = new DataTable();
                    da5.Fill(dt5);
                    for (int i = 0; i < dgvItems.Rows.Count; i++)
                    {
                        if (!(dgvItems.Rows[i].Cells["ItemID"].Value.ToString() == string.Empty))
                        {

                            string S2 = "INSERT INTO tblLabPackage([PackageName],[Amount],[ItemID],[ItemDescription],[ConsultFee],[Total],[GLAccount],[HospitalFee],[Consultant]) VALUES('" + cmbPackageType.Text.ToString() + "','" + txtAmount.Value + "','" + dgvItems.Rows[i].Cells["ItemID"].Value.ToString() + "','" + dgvItems.Rows[i].Cells["ItemDescription"].Value.ToString() + "','" + dgvItems.Rows[i].Cells["ConsultFee"].Value.ToString() + "','" + txtTotal.Value + "','" + dgvItems.Rows[i].Cells["GL"].Value.ToString() + "','" + dgvItems.Rows[i].Cells["HospitalFee"].Value.ToString() + "','" + dgvItems.Rows[i].Cells["Consultant"].Value.ToString() + "')"; 
                             SqlCommand cmd1 = new SqlCommand(S2, myConnection, myTrans);
                            SqlDataAdapter da2 = new SqlDataAdapter(cmd1);
                            DataTable dt2 = new DataTable();
                            da2.Fill(dt2);
                        }
                       
                    }
                    myTrans.Commit();
                    MessageBox.Show("Update Details Successfully", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnNewn_Click(sender, e);
                }
                else
                {
                    for (int i = 0; i < dgvItems.Rows.Count; i++)
                    {
                        if (!(dgvItems.Rows[i].Cells["ItemID"].Value.ToString() == string.Empty))
                        {
                            string S2 = "INSERT INTO tblLabPackage([PackageName],[Amount],[ItemID],[ItemDescription],[ConsultFee],[Total],[GLAccount],[HospitalFee],[Consultant]) VALUES('" + cmbPackageType.Text.ToString() + "','" + txtAmount.Value + "','" + dgvItems.Rows[i].Cells["ItemID"].Value.ToString() + "','" + dgvItems.Rows[i].Cells["ItemDescription"].Value.ToString() + "','" + dgvItems.Rows[i].Cells["ConsultFee"].Value.ToString() + "','" + txtTotal.Value + "','" + dgvItems.Rows[i].Cells["GL"].Value.ToString() + "','" + dgvItems.Rows[i].Cells["HospitalFee"].Value.ToString() + "','" + dgvItems.Rows[i].Cells["Consultant"].Value.ToString() + "')";
                            SqlCommand cmd1 = new SqlCommand(S2, myConnection, myTrans);
                            SqlDataAdapter da2 = new SqlDataAdapter(cmd1);
                            DataTable dt2 = new DataTable();
                            da2.Fill(dt2);
                        }
                       
                    }
                    myTrans.Commit();
                    MessageBox.Show("Saved  Details Successfully", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnNewn_Click(sender, e);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                myTrans.Rollback();
            }
        }

        private void cmbPackageType_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

        }

        private void cmbPackageType_ValueChanged(object sender, EventArgs e)
        {
            try {
                if (dgvItems.Rows == null) return;
                foreach (UltraGridRow ugR in dgvItems.Rows.All)
                {
                    ugR.Delete(false);
                }
                txtAmount.Value = 0;
                txtTotal.Value = 0;
                cmbTestType.ResetText();
                txtConsultant.Text = string.Empty;
                txtScanName.Text = string.Empty;

                String S = "Select * from tblLabPackage WHERE PackageName='"+cmbPackageType.Text.ToString().Trim()+"'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    btnVoid.Enabled = true;
                    cmbPackageType.Enabled = false;
                    txtAmount.Text = dt.Rows[0].ItemArray[1].ToString().Trim();
                    UltraGridRow ugR;
                    foreach (DataRow Dr in dt.Rows)
                    {
                        ugR = dgvItems.DisplayLayout.Bands[0].AddNew();
                        ugR.Cells["LineNo"].Value = ugR.Index + 1;
                        ugR.Cells["ItemID"].Value = Dr["ItemID"];
                        ugR.Cells["ItemDescription"].Value = Dr["ItemDescription"];
                        ugR.Cells["HospitalFee"].Value = Dr["HospitalFee"];
                        ugR.Cells["ConsultFee"].Value = Dr["ConsultFee"];
                        ugR.Cells["GL"].Value = Dr["GLAccount"];
                        ugR.Cells["LineTotal"].Value = Convert.ToDouble(Dr["HospitalFee"]) + Convert.ToDouble(Dr["ConsultFee"]);
                        ugR.Cells["Consultant"].Value = Dr["Consultant"];
                        getitemvalue();
                    }
                }

                }
            catch(Exception ex)
            {
               MessageBox.Show(ex.Message);
            }
        }

        private void dgvItems_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

        }

        private void dgvItems_AfterCellActivate(object sender, EventArgs e)
        {
          
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvItems_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            
        }

        private void dgvItems_ClickCell(object sender, ClickCellEventArgs e)
        {
           
        }

        private void dgvItems_ClickCellButton(object sender, CellEventArgs e)
        {
          
        }
        Class1 a = new Class1();
        private void frmLabPackages_Activated(object sender, EventArgs e)
        {
            getPackages();
            if (Class1.flg == 1)
            {
                txtConsultant.Text = a.GetText();
                DataAccess.Access.Consultant = a.GetText();

                Class1.flg = 0;
            }//getitemvalue();
            if (Class1.flg2 == 1)
            {
                txtScanName.Text = a.GetText2();
                //dgvScanItems.Rows.Add();
                

                string ConnString = ConnectionString;
                string sql = "Select * from tblConsultantItemFee where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (ItemID = '" + txtScanName.Text.ToString().Trim() + "')";
                SqlConnection Conn = new SqlConnection(ConnString);
                SqlCommand cmd = new SqlCommand(sql);
                SqlDataAdapter adapter = new SqlDataAdapter(sql, ConnString);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // dgvScanItems[0, x].Value = a.GetText2();
                if (dt.Rows.Count > 0)
                {

                    UltraGridRow ugR;
                    foreach (DataRow Dr in dt.Rows)
                    {
                        ugR = dgvItems.DisplayLayout.Bands[0].AddNew();
                        ugR.Cells["LineNo"].Value = ugR.Index + 1;
                        ugR.Cells["ItemID"].Value = Dr["ItemID"];
                        ugR.Cells["ItemDescription"].Value = Dr["ItemDescription"];
                        ugR.Cells["HospitalFee"].Value = Dr["HospitalFee"];
                        ugR.Cells["ConsultFee"].Value = Dr["ConsultFee"];
                        ugR.Cells["GL"].Value = Dr["GLAccount"];
                        ugR.Cells["LineTotal"].Value = Convert.ToDouble(Dr["HospitalFee"]) + Convert.ToDouble(Dr["ConsultFee"]);
                        ugR.Cells["Consultant"].Value = txtConsultant.Text.ToString();
                        //ugR.Cells["ItemType"].Value = cmbTestType.Text.ToString();
                        getitemvalue();
                    }
                    // ReceiptTotal = ReceiptTotal + Convert.ToDouble (dgvScanItems[4, x].Value);
                    //txtTotal.Text = Convert.ToString(Convert.ToDouble(dgvScanItems[4, x].Value));
                    // txtScanTotal.Text = Convert.ToString(Convert.ToDouble(dgvScanItems[5, x].Value));
                }
                //ReceiptTotal = ReceiptTotal + Convert.ToDouble(dgvScanItems[4, x].Value);
                // ReceiptTotal = ReceiptTotal + Convert.ToDouble(dgvScanItems.Columns[4].ToString());
                // txtTotal.Text = Convert.ToString(ReceiptTotal);
                Class1.flg2 = 0;
            }
        }

        private void gettypecu()
        {
            DataSet dsCustomer = new DataSet();
            try
            {
                dsCustomer.Clear();
                //StrSql = "SELECT distinct([ItemType]) as[ItemType] FROM[HospitalSystem].[dbo].[tblConsultantItemFee]";
               string StrSql = "SELECT [TypeName]FROM [tblPatientMasterType]";
                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsCustomer, "DtClient");

                cmbTestType.DataSource = dsCustomer.Tables["DtClient"];
                cmbTestType.DisplayMember = "TypeName";
                cmbTestType.ValueMember = "TypeName";

                cmbTestType.DisplayLayout.Bands["DtClient"].Columns["TypeName"].Width = 150;
            }
            catch (Exception ex)
            {
               
            }
        }
        public string StrFormType = "";
       
        private void btnConsultant_Click(object sender, EventArgs e)
        {
           
            StrFormType = cmbTestType.Text.ToString();
            DataAccess.Access.type = cmbTestType.Text.ToString();
            frmLabPackagesSelectCons frm = new frmLabPackagesSelectCons(this);
            frm.Show();
        }

        private void btnScanNames_Click(object sender, EventArgs e)
        {
            try
            {
                DataAccess.Access.type = cmbTestType.Text.ToString();
                frmItemList frm = new frmItemList();
                frm.Show();
            }
            catch { }
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            dgvItems.ActiveRow.Delete();
            getitemvalue();
        }

        private void btnVoid_Click(object sender, EventArgs e)
        {
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;
            if (cmbPackageType.Text.ToString() == string.Empty)
            {
                MessageBox.Show("You Must Select Package", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            try
            {
                DialogResult reply = MessageBox.Show("Are you sure, you want to Void this record?", "Information", MessageBoxButtons.OKCancel);

                if (reply == DialogResult.Cancel)
                {
                    return;
                }

                myConnection.Open();
                myTrans = myConnection.BeginTransaction();
                dgvItems.DisplayLayout.Bands[0].AddNew();

                String S = "Select * from tblLabPackage WHERE PackageName='" + cmbPackageType.Text.ToString().Trim() + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    string S5 = " DELETE FROM tblLabPackage WHERE PackageName='" + cmbPackageType.Text.ToString() + "' ";
                    SqlCommand cmd5 = new SqlCommand(S5, myConnection, myTrans);
                    SqlDataAdapter da5 = new SqlDataAdapter(cmd5);
                    DataTable dt5 = new DataTable();
                    da5.Fill(dt5);
                    
                    myTrans.Commit();
                    MessageBox.Show("Void Details Successfully", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnNewn_Click(sender, e);
                }
              
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                myTrans.Rollback();
            }
        }
    }
}
