using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace UserAutherization
{
    public partial class frmSettingsAccounts : Form
    {
        //clsBLLPhases objclsBLLPhases = null;
        clsDataAccess objclsDataAccess = null;
        SqlConnection Con;
        SqlParameter[] objParams;
        clsCommon objclsCommon = new clsCommon();
        Controlers objControlers = new Controlers();
        public DataSet dsWarehouse;
        public DataSet dsSalesRep ;
        public static string ConnectionString;
        private string _msgTitle = "Settings";
        int intGrid;      
        string SQL = string.Empty;
        string StrSql = string.Empty;
        string StrSql1 = string.Empty;
        SqlCommand cmd;
        public string sMsg = "Distribution Module - Account Setting";
        SqlConnection myConnection = new SqlConnection(ConnectionString);
        SqlTransaction myTrans = null;        
        
        public frmSettingsAccounts()
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
                throw ex;
            }
        }

        private void loadglcode()
        {
            try
            {
                string ConnString = ConnectionString;
                String S1 = "SELECT CardType,GL_Account from  tblCreditData ORDER BY CardType";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataSet ds1 = new DataSet();
                da1.Fill(ds1);
                for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                {
                    dataGridView1.Rows.Add();
                    dataGridView1.Rows[i].Cells[0].Value = ds1.Tables[0].Rows[i].ItemArray[0].ToString();
                    dataGridView1.Rows[i].Cells[1].Value = ds1.Tables[0].Rows[i].ItemArray[1].ToString();
                }
            }
            catch (Exception ex)
            {
                return;
            }
            
        }
        private void loadDefaltOption()
        {          
                try
                {
                    StrSql="Select Tid,TAXID,locked,flg from tblTax_Default where Flg='TAX'";
                    SqlCommand cmd = new SqlCommand(StrSql);
                    SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            cmbInvoiceType.Value = dt.Rows[i]["Tid"].ToString();
                            chklockedtax.Checked = bool.Parse(dt.Rows[i]["locked"].ToString() );
                        }
                    }
                    StrSql="Select Tid,TAXID,locked from tblTax_Default where Flg='PAY'";
                    SqlCommand cmd1 = new SqlCommand(StrSql);
                    SqlDataAdapter da1 = new SqlDataAdapter(StrSql, ConnectionString);
                    DataTable dt1 = new DataTable();
                    da1.Fill(dt1);
                    if (dt1.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt1.Rows.Count; i++)
                        {
                            cmbPayType.Value = dt1.Rows[i]["Tid"].ToString();
                            chklockedPay.Checked = bool.Parse(dt1.Rows[i]["locked"].ToString() );
                        }
                    }

                    StrSql = "Select Tid,TAXID,locked from tblTax_Default where Flg ='REP'";
                    SqlCommand cmd2 = new SqlCommand(StrSql);
                    SqlDataAdapter da2 = new SqlDataAdapter(StrSql, ConnectionString);
                    DataTable dt2 = new DataTable();
                    da2.Fill(dt2);
                    if (dt2.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt2.Rows.Count; i++)
                        {
                            cmbRep.Value = dt2.Rows[i]["Tid"].ToString();
                            chklockedRep.Checked = bool.Parse(dt2.Rows[i]["locked"].ToString() );
                        }
                    }

                    StrSql = "Select Tid,TAXID,locked from tblTax_Default where Flg='WEH'";
                    SqlCommand cmd3 = new SqlCommand(StrSql);
                    SqlDataAdapter da3 = new SqlDataAdapter(StrSql, ConnectionString);
                    DataTable dt3 = new DataTable();
                    da3.Fill(dt3);
                    if (dt3.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt3.Rows.Count; i++)
                        {
                            cmbwh.Value = dt3.Rows[i]["Tid"].ToString();
                            chklockedWH.Checked = bool.Parse(dt3.Rows[i]["locked"].ToString() );
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }  

        }
       

        public void loadglcombo()
        {
            try
            {
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter("Select AcountID,AccountDescription from tblChartofAcounts", ConnectionString);
                da.Fill(ds, "FillDropDown");
                dgGl.DisplayMember = "AccountDescription";
                dgGl.ValueMember = "AcountID";
                dgGl.DataSource = ds.Tables["FillDropDown"];
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
        private void frmSettingsAccounts_Load(object sender, EventArgs e)
        {
            try
            {
                loadglcombo();
                loadglcode();
                loadDefaltOption();
                GetSalesRep();
                GetWareHouseDataSet();
                objclsDataAccess = new clsDataAccess();
                objParams = new SqlParameter[0];
                DataSet _dts = objclsDataAccess.ExecuteSPReturnDataset("tblChartofAcounts_Select", objParams);
                DataSet _dtsItem = objclsDataAccess.ExecuteSPReturnDataset("tblItemMaster_select_Dropdown", objParams);

                cmbSerChargID.DataSource = _dtsItem.Tables[0];
                cmbSerChargID.DisplayMember = "ItemID";
                cmbSerChargID.ValueMember = "ItemID";

                cmbDiscID.DataSource = _dtsItem.Tables[0];
                cmbDiscID.DisplayMember = "ItemID";
                cmbDiscID.ValueMember = "ItemID";

                cmbTaxRec1ID.DataSource = _dtsItem.Tables[0];
                cmbTaxRec1ID.DisplayMember = "ItemID";
                cmbTaxRec1ID.ValueMember = "ItemID";

                cmbTaxRec2ID.DataSource = _dtsItem.Tables[0];
                cmbTaxRec2ID.DisplayMember = "ItemID";
                cmbTaxRec2ID.ValueMember = "ItemID";

                cmbTaxPay2ID.DataSource = _dtsItem.Tables[0];
                cmbTaxPay2ID.DisplayMember = "ItemID";
                cmbTaxPay2ID.ValueMember = "ItemID";

                cmbTaxPay1ID.DataSource = _dtsItem.Tables[0];
                cmbTaxPay1ID.DisplayMember = "ItemID";
                cmbTaxPay1ID.ValueMember = "ItemID";

                cmbTransportID.DataSource = _dtsItem.Tables[0];
                cmbTransportID.DisplayMember = "ItemID";
                cmbTransportID.ValueMember = "ItemID";

                cmbTransAcc.DataSource = _dts.Tables[0];
                cmbTransAcc.DisplayMember = "AcountID";
                cmbTransAcc.ValueMember = "AcountID";

                cmbAP.DataSource = _dts.Tables[0];
                cmbAP.DisplayMember = "AcountID";
                cmbAP.ValueMember = "AcountID";

                cmbAR.DataSource = _dts.Tables[0];
                cmbAR.DisplayMember = "AcountID";
                cmbAR.ValueMember = "AcountID";

                cmbSerChargGL.DataSource = _dts.Tables[0];
                cmbSerChargGL.DisplayMember = "AcountID";
                cmbSerChargGL.ValueMember = "AcountID";

                cmbAdj.DataSource = _dts.Tables[0];
                cmbAdj.DisplayMember = "AcountID";
                cmbAdj.ValueMember = "AcountID";

                cmbDisc.DataSource = _dts.Tables[0];
                cmbDisc.DisplayMember = "AcountID";
                cmbDisc.ValueMember = "AcountID";

                cmbFGT.DataSource = _dts.Tables[0];
                cmbFGT.DisplayMember = "AcountID";
                cmbFGT.ValueMember = "AcountID";

                cmbLCharg.DataSource = _dts.Tables[0];
                cmbLCharg.DisplayMember = "AcountID";
                cmbLCharg.ValueMember = "AcountID";

                cmbRetRec.DataSource = _dts.Tables[0];
                cmbRetRec.DisplayMember = "AcountID";
                cmbRetRec.ValueMember = "AcountID";

                cmbTaxPay1.DataSource = _dts.Tables[0];
                cmbTaxPay1.DisplayMember = "AcountID";
                cmbTaxPay1.ValueMember = "AcountID";

                cmbTaxPay2.DataSource = _dts.Tables[0];
                cmbTaxPay2.DisplayMember = "AcountID";
                cmbTaxPay2.ValueMember = "AcountID";

                cmbTaxRec1.DataSource = _dts.Tables[0];
                cmbTaxRec1.DisplayMember = "AcountID";
                cmbTaxRec1.ValueMember = "AcountID";

                cmbTaxRec2.DataSource = _dts.Tables[0];
                cmbTaxRec2.DisplayMember = "AcountID";
                cmbTaxRec2.ValueMember = "AcountID";

                //objParams = new SqlParameter[0];
                _dts = objclsDataAccess.ExecuteSPReturnDataset("tblDefualtSetting_SelectAll", objParams);



                cmbAR.Text = _dts.Tables[0].Rows[0]["ArAccount"].ToString();
                cmbAP.Text = _dts.Tables[0].Rows[0]["ApAccount"].ToString();
                cmbTaxRec1.Text = _dts.Tables[0].Rows[0]["tax1GL"].ToString();
                cmbTaxRec2.Text = _dts.Tables[0].Rows[0]["tax2GL"].ToString();
                cmbDisc.Text = _dts.Tables[0].Rows[0]["DiscountGL"].ToString();
                //cmbAR.Text = _dts.Tables[0].Rows[0]["SalesGLAccount"].ToString();
                //cmbAP.Text = _dts.Tables[0].Rows[0]["GLJob"].ToString();
                cmbRetRec.Text = _dts.Tables[0].Rows[0]["IssueNoteCurrentAC"].ToString();
                cmbTaxPay1.Text = _dts.Tables[0].Rows[0]["TaxPayGL1"].ToString();
                cmbTaxPay2.Text = _dts.Tables[0].Rows[0]["TaxPayGL2"].ToString();
                cmbAdj.Text = _dts.Tables[0].Rows[0]["AdjustGL"].ToString();
                cmbRetRec.Text = _dts.Tables[0].Rows[0]["RetrnIssuGL"].ToString();
                cmbFGT.Text = _dts.Tables[0].Rows[0]["FTransGL"].ToString();
                cmbLCharg.Text = _dts.Tables[0].Rows[0]["LabChargGL"].ToString();

                cmbTransAcc.Text = _dts.Tables[0].Rows[0]["TransportGL"].ToString();
                cmbTransportID.Text = _dts.Tables[0].Rows[0]["TransportItemID"].ToString();
                cmbDiscID.Text = _dts.Tables[0].Rows[0]["DiscountID"].ToString();

                cmbTaxPay1ID.Text = _dts.Tables[0].Rows[0]["TaxPayID1"].ToString();
                cmbTaxPay2ID.Text = _dts.Tables[0].Rows[0]["TaxPayID2"].ToString();
                cmbTaxRec1ID.Text = _dts.Tables[0].Rows[0]["tax1ID"].ToString();
                cmbTaxRec2ID.Text = _dts.Tables[0].Rows[0]["tax2ID"].ToString();//
                cmbSerChargGL.Text = _dts.Tables[0].Rows[0]["ServChargGL"].ToString();
                cmbSerChargID.Text = _dts.Tables[0].Rows[0]["ServChargID"].ToString();

            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {


                if (!objControlers.HeaderValidation_AccountID(cmbAR.Text, sMsg)) return;//1
                if (!objControlers.HeaderValidation_AccountID(cmbAP.Text, sMsg)) return;//2

                if (!objControlers.HeaderValidation_AccountID(cmbTaxRec1.Text, sMsg)) return;//3
                if (!objControlers.HeaderValidation_AccountID(cmbTaxRec2.Text, sMsg)) return;//4

                if (!objControlers.HeaderValidation_AccountID(cmbDisc.Text, sMsg)) return;//5

                if (!objControlers.HeaderValidation_AccountID(cmbTaxPay1.Text, sMsg)) return;//6
                if (!objControlers.HeaderValidation_AccountID(cmbTaxPay2.Text, sMsg)) return;//7


                if (!objControlers.HeaderValidation_AccountID(cmbAdj.Text, sMsg)) return;//8
                if (!objControlers.HeaderValidation_AccountID(cmbRetRec.Text, sMsg)) return;//9

                if (!objControlers.HeaderValidation_AccountID(cmbFGT.Text, sMsg)) return;//11
                if (!objControlers.HeaderValidation_AccountID(cmbLCharg.Text, sMsg)) return;//12
               
                if (!objControlers.HeaderValidation_AccountID(cmbSerChargGL.Text, sMsg)) return;//14
                if (!objControlers.HeaderValidation_AccountID(cmbTransAcc.Text, sMsg)) return;//15


                objclsDataAccess = new clsDataAccess();
                objclsDataAccess.BeginTransaction();

                objParams = new SqlParameter[23];

                objParams[0] = new SqlParameter("ArAccount", cmbAR.Text.Trim());
                objParams[1] = new SqlParameter("ApAccount", cmbAP.Text.Trim());
                objParams[2] = new SqlParameter("tax1GL", cmbTaxRec1.Text.Trim());
                objParams[3] = new SqlParameter("tax2GL", cmbTaxRec2.Text.Trim());
                objParams[4] = new SqlParameter("DiscountGL", cmbDisc.Text.Trim());
                objParams[5] = new SqlParameter("SalesGLAccount", cmbAR.Text.Trim());
                objParams[6] = new SqlParameter("GLJob", cmbAP.Text.Trim());
                objParams[7] = new SqlParameter("IssueNoteCurrentAC", cmbRetRec.Text.Trim());
                objParams[8] = new SqlParameter("TaxPayGL1", cmbTaxPay1.Text.Trim());
                objParams[9] = new SqlParameter("TaxPayGL2", cmbTaxPay2.Text.Trim());
                objParams[10] = new SqlParameter("AdjustGL", cmbAdj.Text.Trim());
                objParams[11] = new SqlParameter("RetrnIssuGL", cmbRetRec.Text.Trim());
                objParams[12] = new SqlParameter("FTransGL", cmbFGT.Text.Trim());
                objParams[13] = new SqlParameter("LabChargGL", cmbLCharg.Text.Trim());
                objParams[14] = new SqlParameter("TransportGL", cmbTransAcc.Text.Trim());
                objParams[15] = new SqlParameter("TransportItemID", cmbTransportID.Text.Trim());
                objParams[16] = new SqlParameter("DiscountID", cmbDiscID.Text.Trim());
                objParams[17] = new SqlParameter("TaxPayID1", cmbTaxPay1ID.Text.Trim());
                objParams[18] = new SqlParameter("TaxPayID2", cmbTaxPay2ID.Text.Trim());
                objParams[19] = new SqlParameter("tax1ID", cmbTaxRec1ID.Text.Trim());
                objParams[20] = new SqlParameter("tax2ID", cmbTaxRec2ID.Text.Trim());
                objParams[21] = new SqlParameter("ServChargGL", cmbSerChargGL.Text.Trim());
                objParams[22] = new SqlParameter("ServChargID", cmbSerChargID.Text.Trim());

                objclsDataAccess.ExecuteSPReturnObject("tblDefualtSetting_Insert_Accounts", objParams);

                ////card Codes
                DeleteCards();
               // DeleteDefaults();
                SaveCardGLAccount();
              //  SaveDefalutTaxType();
               // SaveDefalutPayType();
               // SaveDefalutRep();
             //   SaveDefalutWhereHouse();
                objclsDataAccess.CommitTransaction();
                
                
                //
                MessageBox.Show("Saved Successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                MessageBox.Show("Restart the System to Apply Changes...!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                objclsDataAccess.RollBackTransaction();
                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }


        private void DeleteCards()
                {
                    try
                    {
                        
                        SqlConnection connection = new SqlConnection(ConnectionString);
                        string sqlStatement = "DELETE FROM tblCreditData" ;
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
        private void DeleteDefaults()
        {
            try
            {

                SqlConnection connection = new SqlConnection(ConnectionString);
                string sqlStatement = "DELETE FROM tblTax_Default";
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
        private void SaveCardGLAccount()
        {
            try
            {
                SqlConnection conn = new SqlConnection(ConnectionString);
                DataTable dtable = new DataTable();
                SqlDataAdapter adp = new SqlDataAdapter("select * from tblCreditData", conn);
                adp.Fill(dtable);
                for (int intGrid = 0; intGrid < dataGridView1.Rows.Count-1; intGrid++)
                {
                   SqlDataAdapter adp1 = new SqlDataAdapter("Insert into tblCreditData (CardType,GL_Account) values('" + dataGridView1.Rows[intGrid].Cells["dgCardType"].Value.ToString() + "','" + dataGridView1.Rows[intGrid].Cells["dgGl"].Value.ToString() + "')", conn);
                   conn.Open();
                   adp1.Fill(dtable);
                   conn.Close();
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void SaveDefalutTaxType()
        {
            try
            {
                SqlConnection conn = new SqlConnection(ConnectionString);
                DataTable dtable = new DataTable();
                SqlDataAdapter adp = new SqlDataAdapter("select * from tblTax_Default", conn);
                adp.Fill(dtable);
                SqlDataAdapter adp1 = new SqlDataAdapter("Insert into tblTax_Default (Tid,TAXID,flg,locked) values('" + cmbInvoiceType.Value + "','" + cmbInvoiceType.Text + "','TAX','" + chklockedtax.Checked + "')", conn);
                conn.Open();
                adp1.Fill(dtable);
                conn.Close();
                
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
                SqlDataAdapter adp = new SqlDataAdapter("select * from tblTax_Default", conn);
                adp.Fill(dtable);
                SqlDataAdapter adp1 = new SqlDataAdapter("Insert into tblTax_Default (Tid,TAXID,flg,locked) values('" + cmbPayType.Value + "','" + cmbPayType.Text + "','PAY','" + chklockedPay.Checked + "')", conn);
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
                SqlDataAdapter adp = new SqlDataAdapter("select * from tblTax_Default", conn);
                adp.Fill(dtable);
                SqlDataAdapter adp1 = new SqlDataAdapter("Insert into tblTax_Default (Tid,TAXID,flg,locked) values('" + cmbRep.Value + "','" + cmbRep.Text + "','REP','" + chklockedRep.Checked + "')", conn);
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
                SqlDataAdapter adp = new SqlDataAdapter("select * from tblTax_Default", conn);
                adp.Fill(dtable);
                SqlDataAdapter adp1 = new SqlDataAdapter("Insert into tblTax_Default (Tid,TAXID,flg,locked) values('" + cmbwh.Value + "','" + cmbwh.Text + "','WEH','" + chklockedWH.Checked + "')", conn);
                conn.Open();
                adp1.Fill(dtable);
                conn.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        

        private void cmbTransportID_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

        }
    }
}