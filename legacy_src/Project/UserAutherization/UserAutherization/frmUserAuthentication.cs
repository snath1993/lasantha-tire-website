using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Collections;


namespace UserAutherization
{
    public partial class frmUserAuthentication : Form
    {
        public static string ConnectionString;
        public DataSet dsWarehouse;
        public DataSet dsSalesRep;        
        //private string _msgTitle = "Settings";
        //int intGrid;
        string SQL = string.Empty;
        string StrSql = string.Empty;
        string StrSql1 = string.Empty;
       // SqlCommand cmd;
        public void setConnectionString()
        {
            //TextReader tr = new StreamReader("Connection.txt");
            //ConnectionString = tr.ReadLine();
            //tr.Close();
            clsDataAccess objclsDataAccess = new clsDataAccess();
            ConnectionString = objclsDataAccess.StrConectionStringLocal;
        }
        public frmUserAuthentication()
        {
            InitializeComponent();
            setConnectionString();
        }
        private void  clearoption()
        {
            cmbInvoiceType.Text = "" ;
            cmbPayType.Text = "";
            cmbRep.Text = "";
            cmbwh.Text = "";
            combMode.Text = "";
            chklockedtax.Checked=false;
            chklockedPay.Checked=false;
            chklockedRep.Checked=false;
            chklockedRep.Checked=false;
            chklockedWH.Checked=false;
            chkforceunitprice.Checked=false;
            chkforceitqty.Checked=false;
            chkforcediscount.Checked=false;

        }
        private void cmbUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            clearoption();
            loadDefaltOption();            
            String S1 = "select * from UserAuthentication where UserName = '" + cmbUser.Text.ToString().Trim() + "'";
            SqlCommand cmd1 = new SqlCommand(S1);
            SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
            DataSet dt1 = new DataSet();
            da1.Fill(dt1);

            String S = "Select ActName,UserActName from Activities";
            //String S = "Select UserActName from Activities";
            SqlCommand cmd = new SqlCommand(S);
            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            DataSet dt = new DataSet();
            da.Fill(dt);

            dgvAuthentication.DataSource = dt.Tables[0];

            //Load the Activities to the grid
            for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            { 
              dgvAuthentication.Rows[i].Cells[0].Value = dt.Tables[0].Rows[i].ItemArray[0].ToString();
              dgvAuthentication.Rows[i].Cells[1].Value = dt.Tables[0].Rows[i].ItemArray[1].ToString();
            }    

            try
            {
                if (dt1.Tables[0].Rows.Count > 0)//if user authentications already added
                {
                    for (int j = 0; j < dgvAuthentication.Rows.Count; j++)
                    {
                        dgvAuthentication[3, j].Value = false;
                        dgvAuthentication[4, j].Value = false;
                        dgvAuthentication[5, j].Value = false;
                        dgvAuthentication[6, j].Value = false;
                        dgvAuthentication[2, j].Value = false;
                        dgvAuthentication[7, j].Value = false;
                    }      

                    for (int j = 0; j < dgvAuthentication.Rows.Count; j++)
                    {
                        for (int i = 0; i < dt1.Tables[0].Rows.Count; i++)
                        {
                            if (dt1.Tables[0].Rows[i].ItemArray[1].ToString().Trim() == dgvAuthentication[0, j].Value.ToString().Trim())
                            {
                                if (dt1.Tables[0].Rows[i].ItemArray[2].ToString().Trim() == "True")
                                {
                                    dgvAuthentication[3, j].Value = true;
                                }
                                if (dt1.Tables[0].Rows[i].ItemArray[3].ToString().Trim() == "True")
                                {
                                    dgvAuthentication[4, j].Value = true;
                                }
                                if (dt1.Tables[0].Rows[i].ItemArray[4].ToString().Trim() == "True")
                                {
                                    dgvAuthentication[5, j].Value = true;
                                }
                                if (dt1.Tables[0].Rows[i].ItemArray[5].ToString().Trim() == "True")
                                {
                                    dgvAuthentication[6, j].Value = true;
                                }
                                if (dt1.Tables[0].Rows[i].ItemArray[6].ToString().Trim() == "True")
                                {
                                    dgvAuthentication[2, j].Value = true;
                                }
                                if (dt1.Tables[0].Rows[i].ItemArray[7].ToString().Trim() == "True")
                                {
                                    dgvAuthentication[7, j].Value = true;
                                }
                            }
                        }
                    }               
                             
                }
                else//if (dt1.Tables[0].Rows.Count < 0)//if user authentications not add yet
                {
                    for (int j = 0; j < dgvAuthentication.Rows.Count; j++)
                    {
                        dgvAuthentication[3, j].Value = false;
                        dgvAuthentication[4, j].Value = false;
                        dgvAuthentication[5, j].Value = false;
                        dgvAuthentication[6, j].Value = false;
                        dgvAuthentication[2, j].Value = false;
                        dgvAuthentication[7, j].Value = false;
                    }                   
                }   
              
            }
            catch
            { }           
        }

        //Load the user ID's to Combo box
        private void frmUserAuthentication_Load(object sender, EventArgs e)
        {
            GetSalesRep();
            GetWareHouseDataSet();
            String S = "Select UserID from Login";
            SqlCommand cmd = new SqlCommand(S);
            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            DataSet dt = new DataSet();
            da.Fill(dt);

            for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            {
                cmbUser.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString());
            }
            
        }

        private void btnSave_Click(object sender, EventArgs e)
        {       
 
            //check the user authentications already add or not
            String S10 = " delete  from tblTax_Default where UserName = '" + cmbUser.Text.ToString().Trim() + "' ";
            SqlCommand cmd10 = new SqlCommand(S10);
            SqlDataAdapter da10 = new SqlDataAdapter(S10, ConnectionString);
            DataTable dt10 = new DataTable();
            da10.Fill(dt10);

            DeleteDefaults();
            SaveDefalutTaxType();
            SaveDefalutPayType();
            SaveDefalutRep();
            SaveDefalutWhereHouse();
            SaveInvoiceSetting();
            SaveDefalutINVType();

            String S1 = "select UserName from UserAuthentication where UserName = '"+ cmbUser.Text.ToString().Trim() +"'";
            SqlCommand cmd1 = new SqlCommand(S1);
            SqlDataAdapter da1 = new SqlDataAdapter(S1,ConnectionString);
            DataTable dt1 = new DataTable();
            da1.Fill(dt1);

            if (dt1.Rows.Count > 0)//atleast one userauthentication added for the selected user
            {
                String S9 = " delete  from UserAuthentication where UserName = '" + cmbUser.Text.ToString().Trim() + "' ";
                SqlCommand cmd9 = new SqlCommand(S9);
                SqlDataAdapter da9 = new SqlDataAdapter(S9, ConnectionString);
                DataTable dt9 = new DataTable();
                da9.Fill(dt9);
              
                if (cmbUser.Text.ToString().Trim() == dt1.Rows[0].ItemArray[0].ToString().Trim())//if user authentications already added for the selected user
                {
                    for (int x = 0; x < (dgvAuthentication.Rows.Count-1); x++)
                    {
                        //check the select activity check or not
                        bool act;
                        if (dgvAuthentication[2, x].Value != null)
                        {
                            if (Convert.ToBoolean(dgvAuthentication[2, x].Value.ToString()) == true)
                            {
                                act = true;
                            }
                            else
                            {
                                act = false;
                            }
                        }
                        else
                        {
                            act = false;
                        }
                        //check the Run check or not
                        bool run;
                        if (dgvAuthentication[3, x].Value != null)
                        {
                            if (Convert.ToBoolean(dgvAuthentication[3, x].Value.ToString()) == true)
                            {
                                run = true;
                            }
                            else
                            {
                                run = false;
                            }
                        }
                        else
                        {
                            run = false;
                        }
                        //check the Add check or not
                        bool add;
                        if (dgvAuthentication[4, x].Value != null)
                        {
                            if (Convert.ToBoolean(dgvAuthentication[4, x].Value.ToString()) == true)
                            {
                                add = true;
                            }
                            else
                            {
                                add = false;
                            }
                        }
                        else
                        {
                            add = false;
                        }
                        //check the Edit check or not
                        bool edit;
                        if (dgvAuthentication[5, x].Value != null)
                        {
                            if (Convert.ToBoolean(dgvAuthentication[5, x].Value.ToString()) == true)
                            {
                                edit = true;
                            }
                            else
                            {
                                edit = false;
                            }
                        }
                        else
                        {
                            edit = false;
                        }
                        //check the Delete check or not
                        bool delete;
                        if (dgvAuthentication[6, x].Value != null)
                        {
                            if (Convert.ToBoolean(dgvAuthentication[6, x].Value.ToString()) == true)
                            {
                                delete = true;
                            }
                            else
                            {
                                delete = false;
                            }
                        }
                        else
                        {
                            delete = false;
                        }

                        bool SpecialEdit;
                        if (dgvAuthentication[7, x].Value != null)
                        {
                            if (Convert.ToBoolean(dgvAuthentication[7, x].Value.ToString()) == true)
                            {
                                SpecialEdit = true;
                            }
                            else
                            {
                                SpecialEdit = false;
                            }
                        }
                        else
                        {
                            SpecialEdit = false;
                        }

                        try
                        {
                            if (act == true || run == true || add == true || edit == true || delete == true || SpecialEdit == true)
                            {
                                String S0 = "Insert into UserAuthentication (UserName,ActivityName,CanAct,CanRun,CanAdd,CanEdit,CanDelete,SpecialEdit) Values ('" + cmbUser.Text.ToString().Trim() + "','" + dgvAuthentication[0, x].Value.ToString().Trim() + "','" + act.ToString().Trim() + "','" + run.ToString().Trim() + "','" + add.ToString().Trim() + "','" + edit.ToString().Trim() + "','" + delete.ToString().Trim() + "','" + SpecialEdit.ToString().Trim() + "')";
                                SqlCommand cmd0 = new SqlCommand(S0);
                                SqlDataAdapter da0 = new SqlDataAdapter(S0, ConnectionString);
                                DataTable dt0 = new DataTable();
                                da0.Fill(dt0);
                            }                     
                        }
                        catch { }
                    }
                    MessageBox.Show(" User Authentications Successfully Updated", "Successfull", MessageBoxButtons.OK);                  
                }
                else//if user authentications not add yet for the selected user
                {
                    for (int x = 0; x < (dgvAuthentication.Rows.Count-1); x++)
                    {
                        //check the select activity check or not
                        bool act;
                        if (dgvAuthentication[2, x].Value != null)
                        {
                            if (Convert.ToBoolean(dgvAuthentication[2, x].Value.ToString()) == true)
                            {
                                act = true;
                            }
                            else
                            {
                                act = false;
                            }
                        }
                        else
                        {
                            act = false;
                        }
                        //check the Run check or not
                        bool run;
                        if (dgvAuthentication[3, x].Value != null)
                        {
                            if (Convert.ToBoolean(dgvAuthentication[3, x].Value.ToString()) == true)
                            {
                                run = true;
                            }
                            else
                            {
                                run = false;
                            }
                        }
                        else
                        {
                            run = false;
                        }
                        //check the Add check or not
                        bool add;
                        if (dgvAuthentication[4, x].Value != null)
                        {
                            if (Convert.ToBoolean(dgvAuthentication[4, x].Value.ToString()) == true)
                            {
                                add = true;
                            }
                            else
                            {
                                add = false;
                            }
                        }
                        else
                        {
                            add = false;
                        }
                        //check the Edit check or not
                        bool edit;
                        if (dgvAuthentication[5, x].Value != null)
                        {
                            if (Convert.ToBoolean(dgvAuthentication[5, x].Value.ToString()) == true)
                            {
                                edit = true;
                            }
                            else
                            {
                                edit = false;
                            }
                        }
                        else
                        {
                            edit = false;
                        }
                        //check the Delete check or not
                        bool delete;
                        if (dgvAuthentication[6, x].Value != null)
                        {
                            if (Convert.ToBoolean(dgvAuthentication[6, x].Value.ToString()) == true)
                            {
                                delete = true;
                            }
                            else
                            {
                                delete = false;
                            }
                        }
                        else
                        {
                            delete = false;
                        }


                        bool SpecialEdit;
                        if (dgvAuthentication[7, x].Value != null)
                        {
                            if (Convert.ToBoolean(dgvAuthentication[6, x].Value.ToString()) == true)
                            {
                                SpecialEdit = true;
                            }
                            else
                            {
                                SpecialEdit = false;
                            }
                        }
                        else
                        {
                            SpecialEdit = false;
                        }


                        try
                        {
                            if (act == true || run == true || add == true || edit == true || delete == true || SpecialEdit == true)
                            {
                                String S = "Insert into UserAuthentication (UserName,ActivityName,CanAct,CanRun,CanAdd,CanEdit,CanDelete,SpecialEdit) Values ('" + cmbUser.Text.ToString().Trim() + "','" + dgvAuthentication[0, x].Value.ToString().Trim() + "','" + act.ToString().Trim() + "','" + run.ToString().Trim() + "','" + add.ToString().Trim() + "','" + edit.ToString().Trim() + "','" + delete.ToString().Trim() + "','" + SpecialEdit.ToString().Trim() + "')";
                                SqlCommand cmd = new SqlCommand(S);
                                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                                DataTable dt = new DataTable();
                                da.Fill(dt);
                            }

                        }
                        catch { }
                    }
                    MessageBox.Show("User Authentications Successfully Added", "Successfull", MessageBoxButtons.OK);                   
                }
            }
            else //atleast one user authentication not added for the selected user
            {
                for (int x = 0; x < (dgvAuthentication.Rows.Count-1); x++)
                {
                    //check the select activity check or not
                    bool act;
                    if (dgvAuthentication[2, x].Value != null)
                    {
                        if (Convert.ToBoolean(dgvAuthentication[2, x].Value.ToString()) == true)
                        {
                            act = true;
                        }
                        else
                        {
                            act = false;
                        }
                    }
                    else
                    {
                        act = false;
                    }
                    //check the Run check or not
                    bool run;
                    if (dgvAuthentication[3, x].Value != null)
                    {
                        if (Convert.ToBoolean(dgvAuthentication[3, x].Value.ToString()) == true)
                        {
                            run = true;
                        }
                        else
                        {
                            run = false;
                        }
                    }
                    else
                    {
                        run = false;
                    }
                    //check the Add check or not
                    bool add;
                    if (dgvAuthentication[4, x].Value != null)
                    {
                        if (Convert.ToBoolean(dgvAuthentication[4, x].Value.ToString()) == true)
                        {
                            add = true;
                        }
                        else
                        {
                            add = false;
                        }
                    }
                    else
                    {
                        add = false;
                    }
                    //check the Edit check or not
                    bool edit;
                    if (dgvAuthentication[5, x].Value != null)
                    {
                        if (Convert.ToBoolean(dgvAuthentication[5, x].Value.ToString()) == true)
                        {
                            edit = true;
                        }
                        else
                        {
                            edit = false;
                        }
                    }
                    else
                    {
                        edit = false;
                    }
                    //check the Delete check or not
                    bool delete;
                    if (dgvAuthentication[6, x].Value != null)
                    {
                        if (Convert.ToBoolean(dgvAuthentication[6, x].Value.ToString()) == true)
                        {
                            delete = true;
                        }
                        else
                        {
                            delete = false;
                        }
                    }
                    else
                    {
                        delete = false;
                    }

                    bool SpecialEdit;
                    if (dgvAuthentication[7, x].Value != null)
                    {
                        if (Convert.ToBoolean(dgvAuthentication[6, x].Value.ToString()) == true)
                        {
                            SpecialEdit = true;
                        }
                        else
                        {
                            SpecialEdit = false;
                        }
                    }
                    else
                    {
                        SpecialEdit = false;
                    }
                    try
                {
                    if (act == true || run == true || add == true || edit == true || delete == true || SpecialEdit == true)
                    {
                        String S4 = "Insert into UserAuthentication (UserName,ActivityName,CanAct,CanRun,CanAdd,CanEdit,CanDelete,SpecialEdit) Values ('" + cmbUser.Text.ToString().Trim() + "','" + dgvAuthentication[0, x].Value.ToString().Trim() + "','" + act.ToString().Trim() + "','" + run.ToString().Trim() + "','" + add.ToString().Trim() + "','" + edit.ToString().Trim() + "','" + delete.ToString().Trim() + "','" + SpecialEdit.ToString().Trim() + "')";
                        SqlCommand cmd4 = new SqlCommand(S4);
                        SqlDataAdapter da4 = new SqlDataAdapter(S4, ConnectionString);
                        DataTable dt4 = new DataTable();
                        da4.Fill(dt4);
                    }
                    else
                    {                       
                    }
                }
                catch { }               
            }     
            MessageBox.Show("User Authentications Successfully Added", "Successfull", MessageBoxButtons.OK);
            }           
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            frmCopyUserAuthentication copy = new frmCopyUserAuthentication();
            copy.ShowDialog();
        }
        private void SaveDefalutTaxType()
        {
            try
            {
                SqlConnection conn = new SqlConnection(ConnectionString);
                DataTable dtable = new DataTable();
                SqlDataAdapter adp1 = new SqlDataAdapter("Insert into tblTax_Default (Tid,TAXID,flg,locked,UserName) values('" + cmbInvoiceType.Value + "','" + cmbInvoiceType.Text + "','TAX','" + chklockedtax.Checked + "','" + cmbUser.Text.ToString().Trim() + "')", conn);
                conn.Open();
                adp1.Fill(dtable);
                conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void SaveDefalutINVType()
        {
            try
            {
                SqlConnection conn = new SqlConnection(ConnectionString);
                DataTable dtable = new DataTable();
                SqlDataAdapter adp1 = new SqlDataAdapter("Insert into tblTax_Default (Tid,TAXID,flg,locked,UserName) values('" + combMode.Value + "','" + combMode.Text + "','INV','" + chkforceinv.Checked + "','" + cmbUser.Text.ToString().Trim() + "')", conn);
                conn.Open();
                adp1.Fill(dtable);
                conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void SaveInvoiceSetting()
        {
            try
            {
                SqlConnection conn = new SqlConnection(ConnectionString);
                DataTable dtable = new DataTable();
                SqlDataAdapter adp1 = new SqlDataAdapter("Insert into tblTax_Default (Tid,TAXID,flg,locked,UserName) values('URS','UNITPRICE','PRL','" + chkforceunitprice.Checked + "','" + cmbUser.Text.ToString().Trim() + "')", conn);                
                conn.Open();
                adp1.Fill(dtable);
                conn.Close();


                SqlConnection conn2 = new SqlConnection(ConnectionString);
                DataTable dtable1 = new DataTable();
                SqlDataAdapter adp2 = new SqlDataAdapter("Insert into tblTax_Default (Tid,TAXID,flg,locked,UserName) values('IQT','QUONTITY','QTY','" + chkforceitqty.Checked + "','" + cmbUser.Text.ToString().Trim() + "')", conn2);
                conn2.Open();
                adp2.Fill(dtable1);
                conn2.Close();

                SqlConnection conn3 = new SqlConnection(ConnectionString);
                DataTable dtable2 = new DataTable();
                SqlDataAdapter adp3 = new SqlDataAdapter("Insert into tblTax_Default (Tid,TAXID,flg,locked,UserName) values('DIS','DISCOUNT','DST','" + chkforcediscount.Checked + "','" + cmbUser.Text.ToString().Trim() + "')", conn3);
                conn3.Open();
                adp3.Fill(dtable);
                conn3.Close();

                

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void SaveDefalutPayType()
        {
            try
            {
                SqlConnection conn = new SqlConnection(ConnectionString);
                DataTable dtable = new DataTable();
                SqlDataAdapter adp1 = new SqlDataAdapter("Insert into tblTax_Default (Tid,TAXID,flg,locked,UserName) values('" + cmbPayType.Value + "','" + cmbPayType.Text + "','PAY','" + chklockedPay.Checked + "','" + cmbUser.Text.ToString().Trim() + "')", conn);
                conn.Open();
                adp1.Fill(dtable);
                conn.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void SaveDefalutRep()
        {
            try
            {
                SqlConnection conn = new SqlConnection(ConnectionString);
                DataTable dtable = new DataTable();
                SqlDataAdapter adp1 = new SqlDataAdapter("Insert into tblTax_Default (Tid,TAXID,flg,locked,UserName) values('" + cmbRep.Value + "','" + cmbRep.Text + "','REP','" + chklockedRep.Checked + "','" + cmbUser.Text.ToString().Trim() + "')", conn);
                conn.Open();
                adp1.Fill(dtable);
                conn.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void SaveDefalutWhereHouse()
        {
            try
            {
                SqlConnection conn = new SqlConnection(ConnectionString);
                DataTable dtable = new DataTable();
                SqlDataAdapter adp1 = new SqlDataAdapter("Insert into tblTax_Default (Tid,TAXID,flg,locked,UserName) values('" + cmbwh.Value + "','" + cmbwh.Text + "','WEH','" + chklockedWH.Checked + "','" + cmbUser.Text.ToString().Trim() + "')", conn);
                conn.Open();
                adp1.Fill(dtable);
                conn.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void DeleteDefaults()
        {
            try
            {

                SqlConnection connection = new SqlConnection(ConnectionString);
                string sqlStatement = "DELETE FROM tblTax_Default where UserName='" + cmbUser.Text.ToString().Trim() + "'";
                connection.Open();
                SqlCommand cmd = new SqlCommand(sqlStatement, connection);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                connection.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void GetWareHouseDataSet()
        {
            dsWarehouse = new DataSet();
            try
            {
                dsWarehouse.Clear();
                StrSql = " SELECT WhseId, WhseName FROM tblWhseMaster";
                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsWarehouse, "DtWarehouse");
                cmbwh.DataSource = dsWarehouse.Tables["DtWarehouse"];
                cmbwh.DisplayMember = "WhseName";
                cmbwh.ValueMember = "WhseId";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void GetSalesRep()
        {
            dsSalesRep = new DataSet();
            try
            {
                dsSalesRep.Clear();
                StrSql = " SELECT RepCode, RepName FROM tblSalesRep order by RepCode";
                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsSalesRep, "DtSalesRep");
                cmbRep.DataSource = dsSalesRep.Tables["DtSalesRep"];
                cmbRep.DisplayMember = "RepName";
                cmbRep.ValueMember = "RepCode";

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void loadDefaltOption()
        {
            try
            {
                StrSql = "Select Tid,TAXID,locked,flg from tblTax_Default where Flg='TAX' and UserName='" + cmbUser.Text.ToString().Trim() + "'";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        cmbInvoiceType.Value = dt.Rows[i]["Tid"].ToString();
                        chklockedtax.Checked = bool.Parse(dt.Rows[i]["locked"].ToString());
                    }
                }
                StrSql = "Select Tid,TAXID,locked from tblTax_Default where Flg='PAY' and UserName='" + cmbUser.Text.ToString().Trim() + "'";
                SqlCommand cmd1 = new SqlCommand(StrSql);
                SqlDataAdapter da1 = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
                if (dt1.Rows.Count > 0)
                {
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        cmbPayType.Value = dt1.Rows[i]["Tid"].ToString();
                        chklockedPay.Checked = bool.Parse(dt1.Rows[i]["locked"].ToString());
                    }
                }

                StrSql = "Select Tid,TAXID,locked from tblTax_Default where Flg ='REP' and UserName='" + cmbUser.Text.ToString().Trim() + "'";
                SqlCommand cmd2 = new SqlCommand(StrSql);
                SqlDataAdapter da2 = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);
                if (dt2.Rows.Count > 0)
                {
                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        cmbRep.Value = dt2.Rows[i]["Tid"].ToString();
                        chklockedRep.Checked = bool.Parse(dt2.Rows[i]["locked"].ToString());
                    }
                }

                StrSql = "Select Tid,TAXID,locked from tblTax_Default where Flg='WEH' and UserName='" + cmbUser.Text.ToString().Trim() + "'";
                SqlCommand cmd3 = new SqlCommand(StrSql);
                SqlDataAdapter da3 = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt3 = new DataTable();
                da3.Fill(dt3);
                if (dt3.Rows.Count > 0)
                {
                    for (int i = 0; i < dt3.Rows.Count; i++)
                    {
                        cmbwh.Value = dt3.Rows[i]["Tid"].ToString();
                        chklockedWH.Checked = bool.Parse(dt3.Rows[i]["locked"].ToString());
                    }
                }

                StrSql = "Select Tid,TAXID,locked from tblTax_Default where Flg='PRL' and UserName='" + cmbUser.Text.ToString().Trim() + "'";
                SqlCommand cmd4 = new SqlCommand(StrSql);
                SqlDataAdapter da4 = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt4 = new DataTable();
                da4.Fill(dt4);
                if (dt4.Rows.Count > 0)
                {
                    for (int i = 0; i < dt4.Rows.Count; i++)
                    {                       
                        chkforceunitprice.Checked = bool.Parse(dt4.Rows[i]["locked"].ToString());
                    }
                }
                StrSql = "Select Tid,TAXID,locked from tblTax_Default where Flg='QTY' and UserName='" + cmbUser.Text.ToString().Trim() + "'";
                SqlCommand cmd5 = new SqlCommand(StrSql);
                SqlDataAdapter da5 = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt5 = new DataTable();
                da5.Fill(dt5);
                if (dt5.Rows.Count > 0)
                {
                    for (int i = 0; i < dt5.Rows.Count; i++)
                    {
                        chkforceitqty.Checked = bool.Parse(dt5.Rows[i]["locked"].ToString());
                    }
                }
                StrSql = "Select Tid,TAXID,locked from tblTax_Default where Flg='DST' and UserName='" + cmbUser.Text.ToString().Trim() + "'";
                SqlCommand cmd6 = new SqlCommand(StrSql);
                SqlDataAdapter da6 = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt6 = new DataTable();
                da6.Fill(dt6);
                if (dt6.Rows.Count > 0)
                {
                    for (int i = 0; i < dt6.Rows.Count; i++)
                    {
                        chkforcediscount.Checked = bool.Parse(dt6.Rows[i]["locked"].ToString());
                    }
                }
                StrSql = "Select Tid,TAXID,locked from tblTax_Default where Flg='INV' and UserName='" + cmbUser.Text.ToString().Trim() + "'";
                SqlCommand cmd7 = new SqlCommand(StrSql);
                SqlDataAdapter da7 = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt7 = new DataTable();
                da7.Fill(dt7);
                if (dt7.Rows.Count > 0)
                {
                    for (int i = 0; i < dt7.Rows.Count; i++)
                    {
                        combMode.Value = dt7.Rows[i]["Tid"].ToString();
                        chkforceinv.Checked = bool.Parse(dt7.Rows[i]["locked"].ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}