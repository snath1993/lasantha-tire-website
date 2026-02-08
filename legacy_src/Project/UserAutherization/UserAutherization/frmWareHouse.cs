using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using DataAccess;


namespace UserAutherization
{
    public partial class frmWareHouse : Form
    {
        Controlers objControlers = new Controlers();
        clsCommon objclsCommon = new clsCommon();
        SqlConnection Con;
        SqlCommand cmd;
        DataSet ds;
        string SQL = string.Empty;
        SqlDataAdapter da;
        SqlTransaction Trans;
        public string StrSql;

        public DataSet dsWarehouse;
        public DataSet dsVendor;
        public DataSet dsAR;
        public string sMsg = "Peachtree - Add Warehouse";

        public bool IsEdit=false;
        public static string ConnectionString;
        int edit = 0;
        //int whseId;
        int add = 0;
        
        bool canAdd = false;//to chk authntication of add new button
        bool canDelete = false; //to chk authntication of delete new button
        bool canEdit = false; //to chk authntication of edit new button
        DataTable dtUser = new DataTable();
        int exsitTransaction;

        bool closeStk = false;//flag is false b4 clse da stktake & its becoming true 4 relevent whse whn stktk is clsed.
        public frmWareHouse()
        {
            InitializeComponent();
            setConnectionString();
        }
        private void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            try
            {

               //if (!objControlers.HeaderValidation_AccountID(cmbAR.Text, sMsg)) return;

                if (!objControlers.HeaderValidation_AccountID(cmbAP.Text, sMsg))
                {
                    return;
                }
                if (!objControlers.HeaderValidation_AccountID(cmbAR.Text, sMsg))
                {
                    return;
                }
                if (!objControlers.HeaderValidation_AccountID(cmbCash.Text, sMsg))
                {
                    return;
                }
                if (!objControlers.HeaderValidation_AccountID(cmbCostofSales.Text, sMsg))
                {
                    return;
                }
                if (!objControlers.HeaderValidation_AccountID(cmbInventoryAccount.Text, sMsg))
                {
                    return;
                }
                if (textWhseId.Text == "")
                {
                    MessageBox.Show("Enter Warehouse ID", "Add Warehouse", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (textWhseName.Text == "")
                {
                    MessageBox.Show("Enter Warehouse Name", "Add Warehouse", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (cmbAP.Value == null)
                {
                    MessageBox.Show("Select Valid AP Account", "Add Warehouse", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (cmbAR.Value == null)
                {
                    MessageBox.Show("Select Valid AR Account", "Add Warehouse", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (cmbCash.Value == null)
                {
                    MessageBox.Show("Select Valid Cash Account", "Add Warehouse", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (cmbSales.Value == null)
                {
                    MessageBox.Show("Select Valid Sales Account", "Add Warehouse", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (cmbCostofSales.Value == null)
                {
                    MessageBox.Show("Select Valid CostofSales Account", "Add Warehouse", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (cmbInventoryAccount.Value == null)
                {
                    MessageBox.Show("Select Valid CostofSales Account", "Add Warehouse", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                //if (cmbRank.Text == "")
                //{
                //    MessageBox.Show("Select Valid Rank", "Add Warehouse", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //    return;
                //}

                Con = new SqlConnection(ConnectionString);
                Con.Open();
                Trans = Con.BeginTransaction();

                if (chkIsdefaultWH.Checked == true)
                {

                    SQL = "SELECT IsDefault,WhseId FROM tblWhseMaster";
                    cmd = new SqlCommand(SQL, Con, Trans);
                    cmd.CommandType = CommandType.Text;
                    da = new SqlDataAdapter(cmd);
                    ds = new DataSet();
                    da.Fill(ds);
                    for (int i = 0; i <= ds.Tables[0].Rows.Count-1; i++)
                    {
                        if (Convert.ToBoolean(ds.Tables[0].Rows[i].ItemArray[0]) == true)
                        {
                            MessageBox.Show("There is a default warehouse in the system", "Add Warehouse", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                }


                SQL = "SELECT WhseId FROM tblWhseMaster where WhseId='" + textWhseId.Text.ToString().Trim() + "'";
                cmd = new SqlCommand(SQL, Con, Trans);
                cmd.CommandType = CommandType.Text;
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    SQL = "Insert into tblWhseMaster (WhseId,WhseName,Address1,ArAccount,CashAccount,SalesGLAccount,IsDefault,APAccount,CostofSales,InventoryAccount) values('" + textWhseId.Text.ToString().Trim() + "','" + textWhseName.Text.ToString().Trim() + "','" + textWhseName.Text.ToString().Trim() + "','" + cmbAR.Value.ToString().Trim() + "','" + cmbCash.Value.ToString().Trim() + "','" + cmbSales.Value.ToString().Trim() + "','" + Convert.ToBoolean(chkIsdefaultWH.Checked) + "','" + cmbAP.Value.ToString().Trim() + "','" + cmbCostofSales.Value.ToString().Trim() + "','" + cmbInventoryAccount.Value.ToString().Trim() + "')";
                    cmd = new SqlCommand(SQL, Con, Trans);
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    Trans.Commit();
                    Con.Close();
                    MessageBox.Show("New Warehouse Added Successfully", "Add Warehouse", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                }
                else
                {
                    SQL = "UPDATE tblWhseMaster SET WhseName='" + textWhseName.Text.ToString().Trim() + "',Address1='" + textWhseName.Text.ToString().Trim() + "',APAccount='" + cmbAP.Value.ToString().Trim() + "',ArAccount='" + cmbAR.Value.ToString().Trim() + "',CashAccount='" + cmbCash.Value.ToString().Trim() + "',SalesGLAccount='" + cmbSales.Value.ToString().Trim() + "',IsDefault='" + Convert.ToBoolean(chkIsdefaultWH.Checked) + "',CostofSales='" + cmbCostofSales.Value.ToString().Trim() + "',InventoryAccount='" + cmbInventoryAccount.Value.ToString().Trim() + "' where WhseId='" + textWhseId.Text.ToString().Trim() + "'";
                    cmd = new SqlCommand(SQL, Con, Trans);
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    Trans.Commit();
                    Con.Close();
                    MessageBox.Show("Updated Warehouse Details Successfully", "Add Warehouse", MessageBoxButtons.OK, MessageBoxIcon.Information);
                   
                }

                ClearData();
                //Trans.Commit();
                //Con.Close();
                //MessageBox.Show("New Warehouse Added Successfully", "Add Warehouse", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //ClearData();
                
            }
            catch (Exception ex)
            {
                Trans.Rollback();
                Con.Close();
                objclsCommon.ErrorLog("Add Warehouse", ex.Message, sender.ToString(), ex.StackTrace);                
            }            
        }
        
        public void GetARAccount()
        {
            dsAR = new DataSet();
            try
            {
                dsAR.Clear();
                StrSql = " SELECT AcountID, AccountDescription FROM tblChartofAcounts order by AcountID";
                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsAR, "DtAR");

                cmbAR.DataSource = dsAR.Tables["DtAR"];
                cmbAR.DisplayMember = "AcountID";
                cmbAR.ValueMember = "AcountID";
                cmbAR.DisplayLayout.Bands["DtAR"].Columns["AcountID"].Width = 100;
                cmbAR.DisplayLayout.Bands["DtAR"].Columns["AccountDescription"].Width = 150;

                cmbAP.DataSource = dsAR.Tables["DtAR"];
                cmbAP.DisplayMember = "AcountID";
                cmbAP.ValueMember = "AcountID";
                cmbAP.DisplayLayout.Bands["DtAR"].Columns["AcountID"].Width = 100;
                cmbAP.DisplayLayout.Bands["DtAR"].Columns["AccountDescription"].Width = 200;


                cmbCash.DataSource = dsAR.Tables["DtAR"];
                cmbCash.DisplayMember = "AcountID";
                cmbCash.ValueMember = "AcountID";
                cmbCash.DisplayLayout.Bands["DtAR"].Columns["AcountID"].Width = 100;
                cmbCash.DisplayLayout.Bands["DtAR"].Columns["AccountDescription"].Width = 200;

                cmbSales.DataSource = dsAR.Tables["DtAR"];
                cmbSales.DisplayMember = "AcountID";
                cmbSales.ValueMember = "AcountID";
                cmbSales.DisplayLayout.Bands["DtAR"].Columns["AcountID"].Width = 100;
                cmbSales.DisplayLayout.Bands["DtAR"].Columns["AccountDescription"].Width = 200;

                cmbCostofSales.DataSource = dsAR.Tables["DtAR"];
                cmbCostofSales.DisplayMember = "AcountID";
                cmbCostofSales.ValueMember = "AcountID";
                cmbCostofSales.DisplayLayout.Bands["DtAR"].Columns["AcountID"].Width = 100;
                cmbCostofSales.DisplayLayout.Bands["DtAR"].Columns["AccountDescription"].Width = 200;

                cmbInventoryAccount.DataSource = dsAR.Tables["DtAR"];
                cmbInventoryAccount.DisplayMember = "AcountID";
                cmbInventoryAccount.ValueMember = "AcountID";
                cmbInventoryAccount.DisplayLayout.Bands["DtAR"].Columns["AcountID"].Width = 100;
                cmbInventoryAccount.DisplayLayout.Bands["DtAR"].Columns["AccountDescription"].Width = 200;     
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void frmWareHouse_Load(object sender, EventArgs e)
        {
            try
            {
                GetARAccount();
                btnAdd_Click(sender, e);
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Add Warehouse", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }


       
        private void btnEdit_Click(object sender, EventArgs e)
        {
            IsEdit = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {           
                this.Close();
        }

        private void ClearData()
        {

            chkIsdefaultWH.Checked = false;

            textWhseId.Text = "";
            textWhseName.Text = "";
            textWhseId.ReadOnly = false;

            cmbAP.Text = "";
            cmbAR.Text = "";
            cmbCash.Text = "";
            cmbCostofSales.Text = "";
            cmbSales.Text = "";
            cmbInventoryAccount.Text = "";

           // cmbRank.Text = "";
            IsEdit = false;
            textWhseId.Enabled = true;
            textWhseName.Enabled = true;
        }



        private void btnAdd_Click(object sender, EventArgs e)
        {
            ClearData();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                //try
                //{
                dtUser = DataAccess.Access.setUserAuthentication(UserAutherization.user.userName, "frmWareHouse");
                if (dtUser.Rows.Count > 0)
                {
                    canDelete = Convert.ToBoolean(dtUser.Rows[0].ItemArray[5].ToString());

                }
                if (canDelete)
                {

                    btnAdd.Enabled = true;

                    String S1 = "Select WhseId from tblOpeningBal where WhseId='" + textWhseId.Text.ToString().Trim() + "'";
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataTable dt1 = new DataTable();
                    da1.Fill(dt1);

                    String S2 = "Select FrmWhseId,ToWhseId from tblWhseTransfer where FrmWhseId='" + textWhseId.Text.ToString().Trim() + "'";
                    SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                    DataTable dt2 = new DataTable();
                    da2.Fill(dt2);

                    String S5 = "Select FrmWhseId,ToWhseId from tblWhseTransfer where ToWhseId='" + textWhseId.Text.ToString().Trim() + "'";
                    SqlDataAdapter da5 = new SqlDataAdapter(S5, ConnectionString);
                    DataTable dt5 = new DataTable();
                    da5.Fill(dt5);

                    String S3 = "Select WhseId from tblItemWhse where WhseId='" + textWhseId.Text.ToString().Trim() + "'";
                    SqlDataAdapter da3 = new SqlDataAdapter(S3, ConnectionString);
                    DataTable dt3 = new DataTable();
                    da3.Fill(dt3);


                    if (dt1.Rows.Count > 0 || dt2.Rows.Count > 0 || dt3.Rows.Count > 0 || dt5.Rows.Count > 0)
                    {
                        MessageBox.Show("You can't Delete this Warehouse..............Transactions are associated with this Warehouse", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {

                        DialogResult result1 = MessageBox.Show("Do You  want to Delete the Warehouse? ", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (result1 == DialogResult.OK)
                        {
                            SqlConnection conn = new SqlConnection(ConnectionString);
                            SqlCommand command = new SqlCommand();
                            String S4 = "delete from tblWhseMaster where WhseId='" + textWhseId.Text.ToString().Trim() + "'";
                            SqlDataAdapter da4 = new SqlDataAdapter(S4, ConnectionString);
                            DataTable dt4 = new DataTable();
                            da4.Fill(dt4);


                            DialogResult result = MessageBox.Show("Warehouse has been Deleted", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            if (result == DialogResult.OK)
                            {
                                ClearData();
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Add Warehouse", ex.Message, sender.ToString(), ex.StackTrace);
            }

        }

       

        private void frmWareHouse_Activated(object sender, EventArgs e)
        {
                try
                {

                    if (Search.WhseId!=null && Search.WhseId.Trim()!=string.Empty)
                    {
                        String S3 = "select WhseId,WhseName,ArAccount,CashAccount,SalesGLAccount,IsDefault,APAccount,CostofSales,InventoryAccount from tblWhseMaster where WhseId = '" + Search.WhseId.ToString().Trim() + "'";
                        SqlDataAdapter da3 = new SqlDataAdapter(S3, ConnectionString);
                        DataTable dt3 = new DataTable();
                        da3.Fill(dt3);
                        if (dt3.Rows.Count > 0)
                        {
                            textWhseId.Text = dt3.Rows[0].ItemArray[0].ToString();
                            textWhseName.Text = dt3.Rows[0].ItemArray[1].ToString();

                            cmbAP.Text = dt3.Rows[0].ItemArray[6].ToString();
                            cmbAR.Text = dt3.Rows[0].ItemArray[2].ToString();
                            cmbCash.Text = dt3.Rows[0].ItemArray[3].ToString();
                            cmbCostofSales.Text = dt3.Rows[0].ItemArray[7].ToString();
                            cmbSales.Text = dt3.Rows[0].ItemArray[4].ToString();
                            cmbInventoryAccount.Text = dt3.Rows[0].ItemArray[8].ToString();
                           // cmbRank.Text = dt3.Rows[0].ItemArray[5].ToString();
                            IsEdit = false;
                            
                        }
                    }
                }
                catch (Exception ex)
                {
                    objclsCommon.ErrorLog("Add Warehouse", ex.Message, sender.ToString(), ex.StackTrace);
                }
            }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //if (add == 0)
            //{
                btnAdd.Enabled = true;
                //btnEdit.Enabled = true;
                btnSave.Enabled = true;
                btnList.Enabled = true;
                bjn.Enabled = true;
                textWhseId.Enabled = true;
              //  textWhseLoc1.Enabled = false;
                textWhseName.Enabled = true;

                frmWarehouseSearch sch = new frmWarehouseSearch();
                sch.ShowDialog();

                textWhseId.Text = Search.WhseId;
                textWhseName.Text = Search.WhseName;
                textWhseId.ReadOnly = true;
                textWhseId.Enabled = false;
            
           // }
        }

        

        private void textWhseId_Leave(object sender, EventArgs e)
        {
            
        }

        private void textWhseId_KeyDown(object sender, KeyEventArgs e)
        {
            objControlers.FocusControl(textWhseName, textWhseId, e);
        }

        private void textWhseName_KeyDown(object sender, KeyEventArgs e)
        {
            objControlers.FocusControl(cmbAP, textWhseId, e);
        }

        private void cmbAP_KeyDown(object sender, KeyEventArgs e)
        {
            objControlers.FocusControl(cmbAR, textWhseName, e);
        }

        private void cmbAR_KeyDown(object sender, KeyEventArgs e)
        {
            objControlers.FocusControl(cmbCash, cmbAP, e);
        }

        private void cmbCash_KeyDown(object sender, KeyEventArgs e)
        {
            objControlers.FocusControl(cmbSales, cmbAR, e);
        }

        private void cmbSales_KeyDown(object sender, KeyEventArgs e)
        {
            objControlers.FocusControl(cmbCostofSales, cmbCash, e);
        }

        private void cmbCostofSales_KeyDown(object sender, KeyEventArgs e)
        {
          //  objControlers.FocusControl(cmbRank, cmbSales, e);
        }

        private void cmbRank_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSave.Select();
                btnSave.PerformClick();
                //btnSave.
            }
        }

        private void textWhseId_TextChanged(object sender, EventArgs e)
        {
            try
            {
                String S51 = "select * from tblWhseMaster where WhseId='" + textWhseId.Text.ToString().Trim() + "' ";
                SqlDataAdapter da51 = new SqlDataAdapter(S51, ConnectionString);
                DataTable dt51 = new DataTable();
                da51.Fill(dt51);

                if (dt51.Rows.Count > 0)
                {
                    //SQL = "Insert into tblWhseMaster (WhseId,
                    //,
                    //,,
                    //,,
                    //,,
                    //) values('" + 
                    //textWhseId.Text = dt51.Rows[0]["WhseName"].ToString();
                    textWhseName.Text = dt51.Rows[0]["WhseName"].ToString();
                    cmbAR.Value = dt51.Rows[0]["ArAccount"].ToString();
                    cmbCash.Value = dt51.Rows[0]["CashAccount"].ToString();
                    cmbSales.Text = dt51.Rows[0]["SalesGLAccount"].ToString();
                    cmbAP.Value = dt51.Rows[0]["APAccount"].ToString();
                    cmbCostofSales.Value = dt51.Rows[0]["CostofSales"].ToString();
                    chkIsdefaultWH.Checked = Convert.ToBoolean(dt51.Rows[0]["IsDefault"].ToString());

                        //.Text.ToString().Trim() + "','" + 
                        //textWhseName.Text.ToString().Trim() + "','" + 
                        //cmbAR.Value.ToString().Trim() + "','" + 
                        //cmbCash.Value.ToString().Trim() + "','" + 
                        //.Value.ToString().Trim() + "','" + 
                        //Convert.ToDouble(cmbRank.Text) + "','" + 
                        //.ToString().Trim() + "','" + 
                        //.ToString().Trim() + "')";

                    //MessageBox.Show("This Warehouse Id Already Exsist Under Warehouse name is '" + dt51.Rows[0].ItemArray[1] + "'", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //textWhseId.Text = "";
                    //// textWhseLoc1.Text = "";
                    //textWhseName.Text = "";
                    textWhseId.Focus();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Add Warehouse", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }
            
        
    }
}