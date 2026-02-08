using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Data.SqlClient;
using UserAutherization;

namespace UserAutherization
{
    public partial class frmDefaultSettings : Form
    {
        clsCommon objclsCommon = new clsCommon();
        public static string ConnectionString;
        bool canAdd = false;//to chk authntication of add new (edit)
        DataTable dtUser = new DataTable();
        bool run = false;

        bool allowMinusQty; // for cheak minus qty
        bool allowMWhse; // for cheak Multi warehouse
        bool allowOverGRN;
        int exsitTransaction = 0;
        int isEditCompany = 0;

        bool tax;
        bool taxOnTax;
        bool overSo;
        bool DilYN;
        public frmDefaultSettings()
        {
            InitializeComponent();
            setConnectionString();
        }
        public void setConnectionString()
        {
            clsDataAccess objclsDataAccess = new clsDataAccess();
            ConnectionString = objclsDataAccess.StrConectionStringLocal;
            //TextReader tr = new StreamReader("Connection.txt");
            //ConnectionString = tr.ReadLine();
            //tr.Close();
        }
        //++++++++++++++++++++for master settings+++++++++++++
        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlCommand myCommand = new SqlCommand();
            SqlTransaction myTrans;
            myConnection.Open();
            myCommand.Connection = myConnection;

            myTrans = myConnection.BeginTransaction();
            myCommand.Transaction = myTrans;//start the trasaction scope
            if (txtDelNoteNo.Text != "" && txtDelNotePre.Text != "")
            {
                try
                {
//                    SELECT     IsMinusAllow, MultiWhse, OverGRN, IsTaxApplicable, IsTaxOnTax, OverSO, IsAssignItem
//FROM         tblDefualtSetting

                    myCommand.CommandText = "UPDATE tblDefualtSetting with (rowlock) SET " +
                        " IsMinusAllow='" + chkAllowMinusQty.Checked + "'," +
                        " MultiWhse='" + chkAllowMultiWhse.Checked + "'," +
                        " OverGRN='" + chkOGRN.Checked + "'," +
                        " IsTaxApplicable='" + chkTax.Checked + "'," +
                        " IsTaxOnTax='" + chkTaxOnTax.Checked + "'," +
                        " OverSO='" + chkOvrSO.Checked + "',";

                    SqlDataAdapter daDelNote = new SqlDataAdapter(myCommand.CommandText, ConnectionString);
                    DataTable dtDelNote = new DataTable();
                    daDelNote.Fill(dtDelNote);                   


                    myTrans.Commit();//Transaction is cometted here

                   
                }
                catch(Exception ex)
                {
                    myTrans.Rollback();
                    objclsCommon.ErrorLog("Default Settings", ex.Message, sender.ToString(), ex.StackTrace);
                }
                finally
                {
                    myConnection.Close();
                }
            }
            





            ////for chk minus qty satus
            //if (chkAllowMinusQty.Checked == true)
            //{
            //    allowMinusQty = true;
            //}
            //else
            //{
            //    allowMinusQty = false;
            //}
            //// fro chk multi whse status
            //if (chkAllowMultiWhse.Checked == true)
            //{
            //    allowMWhse = true;
            //}
            //else
            //{
            //    allowMWhse = false;
            //}
            ////chking iver grn satus
            //if (chkOGRN.Checked == true)
            //{
            //    allowOverGRN = true;
            //}
            //else
            //{
            //    allowOverGRN = false;
            //}

            ////chkin g minus qty 
            //String SMinusQty = "select IsMinusAllow from tblDefualtSetting";
            //SqlDataAdapter daMinusQty = new SqlDataAdapter(SMinusQty, ConnectionString);
            //DataTable dtMinusQty = new DataTable();
            //daMinusQty.Fill(dtMinusQty);
            //setConnectionString();
            ////SqlConnection myConnection = new SqlConnection(ConnectionString);
            //////SqlCommand myCommand = new SqlCommand();
            ////myConnection.Open();
            ////SqlTransaction myTrans = myConnection.BeginTransaction();

            ////select APAccount from tblDefualtSetting", myConnection, myTrans);


            //if (dtMinusQty.Rows.Count > 0)
            //{
            //    String S3 = "update tblDefualtSetting set IsMinusAllow = '" + allowMinusQty.ToString().Trim() + "'";
            //    SqlCommand cmd3 = new SqlCommand(S3);
            //    SqlConnection con3 = new SqlConnection(ConnectionString);
            //    SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
            //    DataTable dt3 = new DataTable();
            //    da3.Fill(dt3);
            //}
            //else
            //{
            //    String S1 = "Insert into tblDefualtSetting (IsMinusAllow) values ('" + allowMinusQty.ToString().Trim() + "')";
            //    SqlCommand cmd1 = new SqlCommand(S1);
            //    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
            //    DataTable dt1 = new DataTable();
            //    da1.Fill(dt1);
            //}

            ////ching over grn
            //String SOverGRN = "select OverGRN from tblDefualtSetting";
            //SqlDataAdapter daOverGRN = new SqlDataAdapter(SOverGRN, ConnectionString);
            //DataTable dtOverGRN = new DataTable();
            //daOverGRN.Fill(dtOverGRN);

            //if (dtOverGRN.Rows.Count > 0)
            //{
            //    String S3 = "update tblDefualtSetting set OverGRN = '" + allowOverGRN.ToString().Trim() + "'";
            //    SqlCommand cmd3 = new SqlCommand(S3);
            //    SqlConnection con3 = new SqlConnection(ConnectionString);
            //    SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
            //    DataTable dt3 = new DataTable();
            //    da3.Fill(dt3);
            //}
            //else
            //{
            //    String S1 = "Insert into tblDefualtSetting (OverGRN) values ('" + allowOverGRN.ToString().Trim() + "')";
            //    SqlCommand cmd1 = new SqlCommand(S1);
            //    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
            //    DataTable dt1 = new DataTable();
            //    da1.Fill(dt1);
            //}
            //// this code is for multiwarehouse
            //String SMultiWhse = "select MultiWhse from tblDefualtSetting";
            //SqlDataAdapter daMultiWhse = new SqlDataAdapter(SMultiWhse, ConnectionString);
            //DataTable dtMultiWhse = new DataTable();
            //daMultiWhse.Fill(dtMultiWhse);

            ////if option is not yet set means in the table it is null
            //if (dtMultiWhse.Rows[0].ItemArray[0].ToString().Trim() != "")
            //{
            //    bool chkTrue = Convert.ToBoolean(dtMultiWhse.Rows[0].ItemArray[0].ToString().Trim());//if chkbox true
            //    bool chkFalse = Convert.ToBoolean(dtMultiWhse.Rows[0].ItemArray[0].ToString().Trim());//if chk box false

            //    //previously its true and user chking it false
            //    if ((chkTrue == true) && (chkAllowMultiWhse.Checked == false))
            //    {
            //        //checking already warehouse are there
            //        String S2 = "select WhseId from tblWhseMaster";
            //        SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
            //        DataTable dt2 = new DataTable();
            //        da2.Fill(dt2);

            //        //if there are warehouses then chk wether transactions are there to any warehouse 
            //        if (dt2.Rows.Count > 0)
            //        {
            //            for (int i = 0; i < dt2.Rows.Count; i++)
            //            {
            //                String S5 = "Select WhseId from tblOpeningBal where WhseId='" + dt2.Rows[i].ItemArray[0].ToString().Trim() + "'";
            //                SqlDataAdapter da5 = new SqlDataAdapter(S5, ConnectionString);
            //                DataTable dt5 = new DataTable();
            //                da5.Fill(dt5);

            //                String S51 = "Select WhseId from tblItemWhse where WhseId='" + dt2.Rows[i].ItemArray[0].ToString().Trim() + "'";
            //                SqlDataAdapter da51 = new SqlDataAdapter(S51, ConnectionString);
            //                DataTable dt51 = new DataTable();
            //                da51.Fill(dt51);

            //                String S52 = "Select WareHouseID from tblGRNTran where WareHouseID='" + dt2.Rows[i].ItemArray[0].ToString().Trim() + "'";
            //                SqlDataAdapter da52 = new SqlDataAdapter(S52, ConnectionString);
            //                DataTable dt52 = new DataTable();
            //                da52.Fill(dt52);

            //                String S53 = "Select WareHouseID from tblInvoiceTransaction where WareHouseID='" + dt2.Rows[i].ItemArray[0].ToString().Trim() + "'";
            //                SqlDataAdapter da53 = new SqlDataAdapter(S53, ConnectionString);
            //                DataTable dt53 = new DataTable();
            //                da53.Fill(dt53);

            //                String S54 = "Select FrmWhseId from tblInvTransaction where FrmWhseId='" + dt2.Rows[i].ItemArray[0].ToString().Trim() + "'";
            //                SqlDataAdapter da54 = new SqlDataAdapter(S54, ConnectionString);
            //                DataTable dt54 = new DataTable();
            //                da54.Fill(dt54);

            //                String S55 = "Select ToWhseId from tblInvTransaction where ToWhseId='" + dt2.Rows[i].ItemArray[0].ToString().Trim() + "'";
            //                SqlDataAdapter da55 = new SqlDataAdapter(S55, ConnectionString);
            //                DataTable dt55 = new DataTable();
            //                da55.Fill(dt55);

            //                //if there are any transaction exsitTransaction increasing
            //                if (dt5.Rows.Count > 0 || dt51.Rows.Count > 0 || dt52.Rows.Count > 0 || dt53.Rows.Count > 0 || dt54.Rows.Count > 0 || dt55.Rows.Count > 0)
            //                {
            //                    exsitTransaction = exsitTransaction + 1;

            //                }
            //            }
            //            //if transactions are there
            //            if (exsitTransaction > 1)
            //            {
            //                MessageBox.Show("Transactions have done to Warehouses, Cannot Change the settings.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //            }
            //            //if no transactions
            //            else
            //            {
            //                //deleting exsisting warehouses
            //                for (int a = 0; a < dt2.Rows.Count; a++)
            //                {
            //                    String S4 = "delete from tblWhseMaster where WhseId='" + dt2.Rows[a].ItemArray[0].ToString().Trim() + "'";
            //                    SqlCommand cmd4 = new SqlCommand(S4);
            //                    SqlDataAdapter da4 = new SqlDataAdapter(S4, ConnectionString);
            //                    DataSet dt4 = new DataSet();
            //                    da4.Fill(dt4);

            //                    String S3 = "delete from tblWhseWiseItem where WhseId='" + dt2.Rows[a].ItemArray[0].ToString().Trim() + "'";
            //                    SqlCommand cmd3 = new SqlCommand(S3);
            //                    SqlDataAdapter da3 = new SqlDataAdapter(S3, ConnectionString);
            //                    DataSet dt3 = new DataSet();
            //                    da3.Fill(dt3);

            //                }
            //                //inserting default warehouse and the items
            //                String SMulWhseIn = "Insert into tblWhseMaster (WhseId,WhseName,Address1) values ('Default','Default Warehouse','Default Warehouse')";
            //                SqlCommand cmdMulWhseIn = new SqlCommand(SMulWhseIn);
            //                SqlDataAdapter daMulWhseIn = new SqlDataAdapter(SMulWhseIn, ConnectionString);
            //                DataTable dtMulWhseIn = new DataTable();
            //                daMulWhseIn.Fill(dtMulWhseIn);

            //                //String S16 = "Insert into tblWhseWiseItem (WhseId,ItemId) values ('Default')";
            //                //SqlCommand cmd16 = new SqlCommand(S16);
            //                //SqlDataAdapter da16 = new SqlDataAdapter(S16, ConnectionString);
            //                //DataTable dt16 = new DataTable();
            //                //da16.Fill(dt16);

            //                String S14 = "SELECT DISTINCT ItemId FROM tblItemMaster where ItemClass='" + 1 + "' or ItemClass='" + 3 + "' or ItemClass='" + 8 + "' or ItemClass='" + 9 + "'";
            //                SqlDataAdapter da14 = new SqlDataAdapter(S14, ConnectionString);
            //                DataSet ds14 = new DataSet();
            //                da14.Fill(ds14);
            //                for (int a = 0; a < ds14.Tables[0].Rows.Count; a++)
            //                {

            //                    String S16 = "Insert into tblWhseWiseItem (WhseId,ItemId) values ('Default','" + ds14.Tables[0].Rows[a].ItemArray[0] + "')";
            //                    SqlCommand cmd16 = new SqlCommand(S16);
            //                    SqlDataAdapter da16 = new SqlDataAdapter(S16, ConnectionString);
            //                    DataTable dt16 = new DataTable();
            //                    da16.Fill(dt16);
            //                }

            //                //updating default setting
            //                String S31 = "update tblDefualtSetting set MultiWhse = '" + allowMWhse.ToString().Trim() + "'";
            //                SqlCommand cmd31 = new SqlCommand(S31);
            //                SqlConnection con31 = new SqlConnection(ConnectionString);
            //                SqlDataAdapter da31 = new SqlDataAdapter(S31, con31);
            //                DataTable dt31 = new DataTable();
            //                da31.Fill(dt31);

            //            }

            //        }
            //        //if there are no warehouse exsist
            //        else
            //        {
            //            //inserting defaul warehouse to warehouse master

            //            String SMulWhseIn = "Insert into tblWhseMaster (WhseId,WhseName,Address1) values ('Default','Default Warehouse','Default Warehouse')";
            //            SqlCommand cmdMulWhseIn = new SqlCommand(SMulWhseIn);
            //            SqlDataAdapter daMulWhseIn = new SqlDataAdapter(SMulWhseIn, ConnectionString);
            //            DataTable dtMulWhseIn = new DataTable();
            //            daMulWhseIn.Fill(dtMulWhseIn);

            //            //String S16 = "Insert into tblWhseWiseItem (WhseId,ItemId) values ('Default','Default Warehouse')";
            //            //SqlCommand cmd16 = new SqlCommand(S16);
            //            //SqlDataAdapter da16 = new SqlDataAdapter(S16, ConnectionString);
            //            //DataTable dt16 = new DataTable();
            //            //da16.Fill(dt16);

            //            //inserting all the items to warehouse master

            //            String S14 = "SELECT DISTINCT ItemId FROM tblItemMaster where ItemClass='" + 1 + "' or ItemClass='" + 3 + "' or ItemClass='" + 8 + "' or ItemClass='" + 9 + "'";
            //            SqlDataAdapter da14 = new SqlDataAdapter(S14, ConnectionString);
            //            DataSet ds14 = new DataSet();
            //            da14.Fill(ds14);
            //            for (int a = 0; a < ds14.Tables[0].Rows.Count; a++)
            //            {

            //                String S16 = "Insert into tblWhseWiseItem (WhseId,ItemId) values ('Default','" + ds14.Tables[0].Rows[a].ItemArray[0] + "')";
            //                SqlCommand cmd16 = new SqlCommand(S16);
            //                SqlDataAdapter da16 = new SqlDataAdapter(S16, ConnectionString);
            //                DataTable dt16 = new DataTable();
            //                da16.Fill(dt16);
            //            }

            //            //updating default settings
            //            String S31 = "update tblDefualtSetting set MultiWhse = '" + allowMWhse.ToString().Trim() + "'";
            //            SqlCommand cmd31 = new SqlCommand(S31);
            //            SqlConnection con31 = new SqlConnection(ConnectionString);
            //            SqlDataAdapter da31 = new SqlDataAdapter(S31, con31);
            //            DataTable dt31 = new DataTable();
            //            da31.Fill(dt31);
            //        }

            //    }

            //    //previously it has marked as false then making as true
            //    if ((chkTrue == false) && (chkAllowMultiWhse.Checked == true))
            //    {
            //        //chking exsisting warehouses
            //        String S2 = "select WhseId from tblWhseMaster";
            //        SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
            //        DataTable dt2 = new DataTable();
            //        da2.Fill(dt2);
            //        if (dt2.Rows.Count > 0)
            //        {
            //            for (int i = 0; i < dt2.Rows.Count; i++)
            //            {
            //                String S5 = "Select WhseId from tblOpeningBal where WhseId='" + dt2.Rows[0].ItemArray[0].ToString().Trim() + "'";
            //                SqlDataAdapter da5 = new SqlDataAdapter(S5, ConnectionString);
            //                DataTable dt5 = new DataTable();
            //                da5.Fill(dt5);

            //                String S51 = "Select WhseId from tblItemWhse where WhseId='" + dt2.Rows[0].ItemArray[0].ToString().Trim() + "'";
            //                SqlDataAdapter da51 = new SqlDataAdapter(S51, ConnectionString);
            //                DataTable dt51 = new DataTable();
            //                da51.Fill(dt51);

            //                String S52 = "Select WareHouseID from tblGRNTran where WareHouseID='" + dt2.Rows[0].ItemArray[0].ToString().Trim() + "'";
            //                SqlDataAdapter da52 = new SqlDataAdapter(S52, ConnectionString);
            //                DataTable dt52 = new DataTable();
            //                da52.Fill(dt52);

            //                String S53 = "Select WareHouseID from tblInvoiceTransaction where WareHouseID='" + dt2.Rows[0].ItemArray[0].ToString().Trim() + "'";
            //                SqlDataAdapter da53 = new SqlDataAdapter(S53, ConnectionString);
            //                DataTable dt53 = new DataTable();
            //                da53.Fill(dt53);

            //                String S54 = "Select FrmWhseId from tblInvTransaction where FrmWhseId='" + dt2.Rows[0].ItemArray[0].ToString().Trim() + "'";
            //                SqlDataAdapter da54 = new SqlDataAdapter(S54, ConnectionString);
            //                DataTable dt54 = new DataTable();
            //                da54.Fill(dt54);

            //                String S55 = "Select ToWhseId from tblInvTransaction where ToWhseId='" + dt2.Rows[0].ItemArray[0].ToString().Trim() + "'";
            //                SqlDataAdapter da55 = new SqlDataAdapter(S55, ConnectionString);
            //                DataTable dt55 = new DataTable();
            //                da55.Fill(dt55);

            //                if (dt5.Rows.Count > 0 || dt51.Rows.Count > 0 || dt52.Rows.Count > 0 || dt53.Rows.Count > 0 || dt54.Rows.Count > 0 || dt55.Rows.Count > 0)
            //                {
            //                    exsitTransaction = exsitTransaction + 1;

            //                }
            //            }
            //            //if there are any transsactions
            //            if (exsitTransaction > 0)
            //            {
            //                MessageBox.Show("Transactions have done to Warehouses,Cannot Change the settings.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //            }
            //            //if no transactions
            //            else
            //            {
            //                //deleting warehouses
            //                for (int a = 0; a < dt2.Rows.Count; a++)
            //                {
            //                    String S4 = "delete from tblWhseMaster where WhseId='" + dt2.Rows[0].ItemArray[0].ToString().Trim() + "'";
            //                    SqlCommand cmd4 = new SqlCommand(S4);
            //                    SqlDataAdapter da4 = new SqlDataAdapter(S4, ConnectionString);
            //                    DataSet dt4 = new DataSet();
            //                    da4.Fill(dt4);

            //                    String S3 = "delete from tblWhseWiseItem where WhseId='" + dt2.Rows[0].ItemArray[0].ToString().Trim() + "'";
            //                    SqlCommand cmd3 = new SqlCommand(S3);
            //                    SqlDataAdapter da3 = new SqlDataAdapter(S3, ConnectionString);
            //                    DataSet dt3 = new DataSet();
            //                    da3.Fill(dt3);
            //                }

            //                String SMulWhseUp = "update tblDefualtSetting set MultiWhse = '" + allowMWhse.ToString().Trim() + "'";
            //                SqlCommand cmdMulWhseUp = new SqlCommand(SMulWhseUp);
            //                SqlConnection conMulWhseUp = new SqlConnection(ConnectionString);
            //                SqlDataAdapter daMulWhseUp = new SqlDataAdapter(SMulWhseUp, conMulWhseUp);
            //                DataTable dtMulWhseUp = new DataTable();
            //                daMulWhseUp.Fill(dtMulWhseUp);

            //            }
            //        }
            //        else
            //        {
            //            String SMulWhseUp = "update tblDefualtSetting set MultiWhse = '" + allowMWhse.ToString().Trim() + "'";
            //            SqlCommand cmdMulWhseUp = new SqlCommand(SMulWhseUp);
            //            SqlConnection conMulWhseUp = new SqlConnection(ConnectionString);
            //            SqlDataAdapter daMulWhseUp = new SqlDataAdapter(SMulWhseUp, conMulWhseUp);
            //            DataTable dtMulWhseUp = new DataTable();
            //            daMulWhseUp.Fill(dtMulWhseUp);
            //        }
            //    }

            //}
            //else
            //{
            //    String SMultiWhse1 = "select MultiWhse from tblDefualtSetting";
            //    SqlDataAdapter daMultiWhse1 = new SqlDataAdapter(SMultiWhse1, ConnectionString);
            //    DataTable dtMultiWhse1 = new DataTable();
            //    daMultiWhse1.Fill(dtMultiWhse1);
            //    if (dtMultiWhse1.Rows.Count > 0)
            //    {
            //        String S3 = "update tblDefualtSetting set MultiWhse = '" + allowMWhse.ToString().Trim() + "'";
            //        SqlCommand cmd3 = new SqlCommand(S3);
            //        SqlConnection con3 = new SqlConnection(ConnectionString);
            //        SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
            //        DataTable dt3 = new DataTable();
            //        da3.Fill(dt3);
            //    }
            //    else
            //    {
            //        String SMulWhseIn = "Insert into tblDefualtSetting (MultiWhse) values ('" + allowMWhse.ToString().Trim() + "')";
            //        SqlCommand cmdMulWhseIn = new SqlCommand(SMulWhseIn);
            //        SqlDataAdapter daMulWhseIn = new SqlDataAdapter(SMulWhseIn, ConnectionString);
            //        DataTable dtMulWhseIn = new DataTable();
            //        daMulWhseIn.Fill(dtMulWhseIn);

            //    }

            //    if (chkAllowMultiWhse.Checked == false)
            //    {
            //        String S2 = "select WhseId from tblWhseMaster";
            //        SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
            //        DataTable dt2 = new DataTable();
            //        da2.Fill(dt2);
            //        if (dt2.Rows.Count < 1)
            //        {
            //            String SMulWhseIn1 = "Insert into tblWhseMaster (WhseId,WhseName,Address1) values ('Default','Default Warehouse','Default Warehouse')";
            //            SqlCommand cmdMulWhseIn1 = new SqlCommand(SMulWhseIn1);
            //            SqlDataAdapter daMulWhseIn1 = new SqlDataAdapter(SMulWhseIn1, ConnectionString);
            //            DataTable dtMulWhseIn1 = new DataTable();
            //            daMulWhseIn1.Fill(dtMulWhseIn1);

            //            String S14 = "SELECT DISTINCT ItemId FROM tblItemMaster where ItemClass='" + 1 + "' or ItemClass='" + 3 + "' or ItemClass='" + 8 + "' or ItemClass='" + 9 + "'";
            //            SqlDataAdapter da14 = new SqlDataAdapter(S14, ConnectionString);
            //            DataSet ds14 = new DataSet();
            //            da14.Fill(ds14);
            //            for (int a = 0; a < ds14.Tables[0].Rows.Count; a++)
            //            {

            //                String S16 = "Insert into tblWhseWiseItem (WhseId,ItemId) values ('Default','" + ds14.Tables[0].Rows[a].ItemArray[0] + "')";
            //                SqlCommand cmd16 = new SqlCommand(S16);
            //                SqlDataAdapter da16 = new SqlDataAdapter(S16, ConnectionString);
            //                DataTable dt16 = new DataTable();
            //                da16.Fill(dt16);
            //            }

            //            //String S16 = "Insert into tblWhseWiseItem (WhseId,ItemId) values ('Default','Default Warehouse')";
            //            //SqlCommand cmd16 = new SqlCommand(S16);
            //            //SqlDataAdapter da16 = new SqlDataAdapter(S16, ConnectionString);
            //            //DataTable dt16 = new DataTable();
            //            //da16.Fill(dt16);
            //        }
            //    }

            //}

            //MessageBox.Show("Master Settings Saved Successfully ", "", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //chkAllowMinusQty.Enabled = false;
            //chkAllowMultiWhse.Enabled = false;
            //chkOGRN.Enabled = false;
            //btnSave.Enabled = false;
            //this.Close();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //bool canAdd = false;
            //dtUser = DataAccess.Access.setUserAuthentication(UserAutherization.user.userName, "frmMasterSettings");
            //if (dtUser.Rows.Count > 0)
            //{
            //    canAdd = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
            //}
            //if (canAdd)
            //{
            //    chkAllowMinusQty.Enabled = true;
            //    chkAllowMultiWhse.Enabled = true;
            //    chkOGRN.Enabled = true;
            //    btnSave.Enabled = true;

            //}
            //else
            //{
            //    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }

        private void tabMaster_Click(object sender, EventArgs e)
        {
            chkAllowMinusQty.Enabled = false;
            chkAllowMultiWhse.Enabled = false;
            chkOGRN.Enabled = false;
            btnSave.Enabled = false;

            //to show the current statua of all options
            String SMinusQty = "select IsMinusAllow from tblDefualtSetting";
            SqlDataAdapter daMinusQty = new SqlDataAdapter(SMinusQty, ConnectionString);
            DataTable dtMinusQty = new DataTable();
            daMinusQty.Fill(dtMinusQty);

            if (dtMinusQty.Rows.Count > 0 && dtMinusQty.Rows[0].ItemArray[0].ToString().Trim() != "")
            {
                allowMinusQty = Convert.ToBoolean(dtMinusQty.Rows[0].ItemArray[0].ToString().Trim());
                if (allowMinusQty == true)
                {
                    chkAllowMinusQty.Checked = true;
                }
                else
                {
                    chkAllowMinusQty.Checked = false;
                }
            }
            else
            {
                chkAllowMinusQty.Checked = false;
            }

            //chking over grn
            String SOverGRN = "select OverGRN from tblDefualtSetting";
            SqlDataAdapter daOverGRN = new SqlDataAdapter(SOverGRN, ConnectionString);
            DataTable dtOverGRN = new DataTable();
            daOverGRN.Fill(dtOverGRN);

            if (dtOverGRN.Rows.Count > 0 && dtOverGRN.Rows[0].ItemArray[0].ToString().Trim() != "")
            {
                allowOverGRN = Convert.ToBoolean(dtOverGRN.Rows[0].ItemArray[0].ToString().Trim());
                if (allowOverGRN == true)
                {
                    chkOGRN.Checked = true;
                }
                else
                {
                    chkOGRN.Checked = false;
                }
            }
            else
            {
                chkOGRN.Checked = false;
            }
            // this code is for multiwarehouse
            String SMultiWhse = "select MultiWhse from tblDefualtSetting";
            SqlDataAdapter daMultiWhse = new SqlDataAdapter(SMultiWhse, ConnectionString);
            DataTable dtMultiWhse = new DataTable();
            daMultiWhse.Fill(dtMultiWhse);
            if (dtMultiWhse.Rows.Count > 0 && dtMultiWhse.Rows[0].ItemArray[0].ToString().Trim() != "")
            {
                allowMWhse = Convert.ToBoolean(dtMultiWhse.Rows[0].ItemArray[0].ToString().Trim());
                if (allowMWhse == true)
                {
                    chkAllowMultiWhse.Checked = true;
                }
                else
                {
                    chkAllowMultiWhse.Checked = false;
                }
            }
            else
            {
                chkAllowMultiWhse.Checked = false;
            }
        }
        //++++++++++++++++++++++++++++++++++++++++++
        public void TaxcodeLoad()
        {
            cmbtaxCode.Items.Clear();
            try
            {
                String S = "Select TaxID from tblTaxApplicable";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    cmbtaxCode.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
                }
            }
            catch { }

        }
        private void CustomerReturnLoad()
        {
            try
            {
                String SCusRe = "select CusReturnNo,CusReturnPrefix from tblDefualtSetting";
                SqlDataAdapter daCusRe = new SqlDataAdapter(SCusRe, ConnectionString);
                DataTable dtCusRe = new DataTable();
                daCusRe.Fill(dtCusRe);

                if (dtCusRe.Rows.Count > 0)
                {
                    lblCusReNo.Text = dtCusRe.Rows[0].ItemArray[0].ToString().Trim();
                    lblCusRePre.Text = dtCusRe.Rows[0].ItemArray[1].ToString().Trim();
                }
            }
            catch { }
        }
        private void InvoiceLoad()
        {
            try
            {
                String SInvoice = "select InvoiceNo,InvoicePrefix from tblDefualtSetting";
                SqlDataAdapter daInvoice = new SqlDataAdapter(SInvoice, ConnectionString);
                DataTable dtInvoice = new DataTable();
                daInvoice.Fill(dtInvoice);

                if (dtInvoice.Rows.Count > 0)
                {
                    lblInvoiceNo.Text = dtInvoice.Rows[0].ItemArray[0].ToString().Trim();
                    lblInvoicePre.Text = dtInvoice.Rows[0].ItemArray[1].ToString().Trim();
                }
            }
            catch { }
        }

        private void DeliveryNOteLoad()
        {
            try
            {
                String SDelNote = "select DeliveryNoteNo,DeliveryNotePrefix from tblDefualtSetting";
                SqlDataAdapter daDelNote = new SqlDataAdapter(SDelNote, ConnectionString);
                DataTable dtDelNote = new DataTable();
                daDelNote.Fill(dtDelNote);

                if (dtDelNote.Rows.Count > 0)
                {
                    lblDelNoteNo.Text = dtDelNote.Rows[0].ItemArray[0].ToString().Trim();
                    lblDelNotePre.Text = dtDelNote.Rows[0].ItemArray[1].ToString().Trim();
                }
            }
            catch { }

        }
        private void SupplyRetuenLoad()
        {
            try
            {
                String SSupRe = "select SupplierReturnNo,SupReturnPrefix from tblDefualtSetting";
                SqlDataAdapter daSupRe = new SqlDataAdapter(SSupRe, ConnectionString);
                DataTable dtSupRe = new DataTable();
                daSupRe.Fill(dtSupRe);

                if (dtSupRe.Rows.Count > 0)
                {
                    lblSupRe.Text = dtSupRe.Rows[0].ItemArray[0].ToString().Trim();
                    lblSupRePre.Text = dtSupRe.Rows[0].ItemArray[1].ToString().Trim();
                }
            }
            catch { }
        }
        private void InvoiceDataLoad()
        {
            try
            {
                String SSupIn = "select SupplierInvoiceNo,SupInvoicePrefix from tblDefualtSetting";
                SqlDataAdapter daSupIn = new SqlDataAdapter(SSupIn, ConnectionString);
                DataTable dtSupIn = new DataTable();
                daSupIn.Fill(dtSupIn);
                if (dtSupIn.Rows.Count > 0)
                {
                    lblSupInNo.Text = dtSupIn.Rows[0].ItemArray[0].ToString().Trim();
                    lblSupInPre.Text = dtSupIn.Rows[0].ItemArray[1].ToString().Trim();
                }

            }
            catch { }
        }
        private void GRNDataload()
        {
            try
            {
                String SGRN = "select GRNNo,GRNPrefix from tblDefualtSetting";
                SqlDataAdapter daGRN = new SqlDataAdapter(SGRN, ConnectionString);
                DataTable dtGRN = new DataTable();
                daGRN.Fill(dtGRN);
                if (dtGRN.Rows.Count > 0)
                {
                    lblGRNNo.Text = dtGRN.Rows[0].ItemArray[0].ToString().Trim();
                    lblGrnPfx.Text = dtGRN.Rows[0].ItemArray[1].ToString().Trim();
                }
            }
            catch { }
        }
        private void TransferDataLoad()
        {
            try
            {
                String STransNote = "select TransNoteNo,TransNotePrefix from tblDefualtSetting";
                SqlDataAdapter daTransNote = new SqlDataAdapter(STransNote, ConnectionString);
                DataTable dtTransNote = new DataTable();
                daTransNote.Fill(dtTransNote);

                if (dtTransNote.Rows.Count > 0)
                {
                    lblTransNoteNo.Text = dtTransNote.Rows[0].ItemArray[0].ToString().Trim();
                    lblTransNotePre.Text = dtTransNote.Rows[0].ItemArray[1].ToString().Trim();
                }
            }
            catch { }
        }

        private void IssueNoteLoad()
        {
            try
            {
                String SIssueNote = "select IssueNoteNo,IssuNotePrefix from tblDefualtSetting";
                SqlDataAdapter daIssueNote = new SqlDataAdapter(SIssueNote, ConnectionString);
                DataTable dtIssueNote = new DataTable();
                daIssueNote.Fill(dtIssueNote);

                if (dtIssueNote.Rows.Count > 0)
                {
                    lblIssueNoteNo.Text = dtIssueNote.Rows[0].ItemArray[0].ToString().Trim();
                    lblIssueNotePre.Text = dtIssueNote.Rows[0].ItemArray[1].ToString().Trim();
                }
            }
            catch { }
        }
        private void GRnAcountload()
        {
            try
            {
                String SGRNDrAc = "select GRNDrAc,GRNCrAc from tblDefualtSetting";
                SqlDataAdapter daGRNDrAc = new SqlDataAdapter(SGRNDrAc, ConnectionString);
                DataTable dtGRNDrAc = new DataTable();
                daGRNDrAc.Fill(dtGRNDrAc);

                if (dtGRNDrAc.Rows.Count > 0)
                {
                    lblGrnDrAc.Text = dtGRNDrAc.Rows[0].ItemArray[0].ToString().Trim();
                    lblGrnCrAc.Text = dtGRNDrAc.Rows[0].ItemArray[1].ToString().Trim();
                }
            }
            catch { }
        }
        private void InvoiceAccountLoad()
        {
            try
            {
                String SSalesInvDrAc = "select SalesInvDrAc,SalesInvCrAc from tblDefualtSetting";
                SqlDataAdapter daSalesInvDrAc = new SqlDataAdapter(SSalesInvDrAc, ConnectionString);
                DataTable dtSalesInvDrAc = new DataTable();
                daSalesInvDrAc.Fill(dtSalesInvDrAc);

                if (dtSalesInvDrAc.Rows.Count > 0)
                {
                    lblSupInDrAc.Text = dtSalesInvDrAc.Rows[0].ItemArray[0].ToString().Trim();
                    lblSupInCrAc.Text = dtSalesInvDrAc.Rows[0].ItemArray[1].ToString().Trim();
                }
            }
            catch { }
        }
        private void DelNoteAacountLoad()
        {
            try
            {
                String SDelNoteDrAc = "select DelNoteDrAc,DelNoteCrAc from tblDefualtSetting";
                SqlDataAdapter daDelNoteDrAc = new SqlDataAdapter(SDelNoteDrAc, ConnectionString);
                DataTable dtDelNoteDrAc = new DataTable();
                daDelNoteDrAc.Fill(dtDelNoteDrAc);

                if (dtDelNoteDrAc.Rows.Count > 0)
                {
                    lblDelNoteDrAc1.Text = dtDelNoteDrAc.Rows[0].ItemArray[0].ToString().Trim();
                    lblDelNoteCrAc1.Text = dtDelNoteDrAc.Rows[0].ItemArray[1].ToString().Trim();
                }
            }
            catch { }
        }
        private void CusReturnDataLoad()
        {

            try
            {
                String SCusretnDrAc = "select CusretnDrAc,CusretnCrAc from tblDefualtSetting";
                SqlDataAdapter daCusretnDrAc = new SqlDataAdapter(SCusretnDrAc, ConnectionString);
                DataTable dtCusretnDrAc = new DataTable();
                daCusretnDrAc.Fill(dtCusretnDrAc);

                if (dtCusretnDrAc.Rows.Count > 0)
                {
                    lblCusretnDrAc1.Text = dtCusretnDrAc.Rows[0].ItemArray[0].ToString().Trim();
                    lblCusretnCrAc1.Text = dtCusretnDrAc.Rows[0].ItemArray[1].ToString().Trim();
                }
            }
            catch {}
        }
        private void frmDefaultSettings_Load(object sender, EventArgs e)
        {
            //+++++++for master settings+++++++++++++++
            //TaxcodeLoad();
            chkAllowMinusQty.Enabled = false;
            chkAllowMultiWhse.Enabled = false;
            chkOGRN.Enabled = false;
            btnSave.Enabled = false;


            //multi colomn
            mltDeliveryDr.Enabled = false;
            mltDeliveryCr.Enabled = false;
            mltCustomerCr.Enabled = false;
            mltCustomerDr.Enabled = false;
            mltDrAccount.Enabled = false;
            mltCrAccount.Enabled = false;
            mltDrSup.Enabled = false;
            mltCrSup.Enabled = false;
            mltDrAccount.SelectedValue = "";
            mltCrAccount.SelectedValue = "";
            mltDrSup.SelectedValue = "";
            mltCrSup.SelectedValue = "";
            mltDeliveryDr.SelectedValue = "";
            mltDeliveryCr.SelectedValue = "";
            mltCustomerCr.SelectedValue = "";
            mltCustomerDr.SelectedValue = "";

            chkTaxOnTax.Enabled = false;
            chkTax.Enabled = false;
            chkOvrSO.Enabled = false;

            //to show the current statua of all options
            String SMinusQty = "select IsMinusAllow from tblDefualtSetting";
            SqlDataAdapter daMinusQty = new SqlDataAdapter(SMinusQty, ConnectionString);
            DataTable dtMinusQty = new DataTable();
            daMinusQty.Fill(dtMinusQty);

            if (dtMinusQty.Rows.Count > 0 && dtMinusQty.Rows[0].ItemArray[0].ToString().Trim() != "")
            {
                allowMinusQty = Convert.ToBoolean(dtMinusQty.Rows[0].ItemArray[0].ToString().Trim());
                if (allowMinusQty == true)
                {
                    chkAllowMinusQty.Checked = true;
                }
                else
                {
                    chkAllowMinusQty.Checked = false;
                }
            }
            else
            {
                chkAllowMinusQty.Checked = false;
            }

            //chking over grn
            String SOverGRN = "select OverGRN from tblDefualtSetting";
            SqlDataAdapter daOverGRN = new SqlDataAdapter(SOverGRN, ConnectionString);
            DataTable dtOverGRN = new DataTable();
            daOverGRN.Fill(dtOverGRN);

            if (dtOverGRN.Rows.Count > 0 && dtOverGRN.Rows[0].ItemArray[0].ToString().Trim() != "")
            {
                allowOverGRN = Convert.ToBoolean(dtOverGRN.Rows[0].ItemArray[0].ToString().Trim());
                if (allowOverGRN == true)
                {
                    chkOGRN.Checked = true;
                }
                else
                {
                    chkOGRN.Checked = false;
                }
            }
            else
            {
                chkOGRN.Checked = false;
            }
            //chking over tax applicable
            String STax = "select IsTaxApplicable from tblDefualtSetting";
            SqlDataAdapter daTax = new SqlDataAdapter(STax, ConnectionString);
            DataTable dtTax = new DataTable();
            daTax.Fill(dtTax);

            if (dtTax.Rows.Count > 0 && dtTax.Rows[0].ItemArray[0].ToString().Trim() != "")
            {
                tax = Convert.ToBoolean(dtTax.Rows[0].ItemArray[0].ToString().Trim());
                if (tax == true)
                {
                    chkTax.Checked = true;
                }
                else
                {
                    chkTax.Checked = false;
                }
            }
            else
            {
                chkTax.Checked = false;
            }
            //chking over tax on tax applicable
            String STaxOn = "select IsTaxOnTax from tblDefualtSetting";
            SqlDataAdapter daTaxOn = new SqlDataAdapter(STaxOn, ConnectionString);
            DataTable dtTaxOn = new DataTable();
            daTaxOn.Fill(dtTaxOn);

            if (dtTaxOn.Rows.Count > 0 && dtTaxOn.Rows[0].ItemArray[0].ToString().Trim() != "")
            {
                taxOnTax = Convert.ToBoolean(dtTaxOn.Rows[0].ItemArray[0].ToString().Trim());
                if (taxOnTax == true)
                {
                    chkTaxOnTax.Checked = true;
                }
                else
                {
                    chkTaxOnTax.Checked = false;
                }
            }
            else
            {
                chkTaxOnTax.Checked = false;
            }
            String SSo = "select OverSO,SalesWithDiliveryOrder from tblDefualtSetting";
            SqlDataAdapter daSo = new SqlDataAdapter(SSo, ConnectionString);
            DataTable dtSo = new DataTable();
            daSo.Fill(dtSo);

            if (dtSo.Rows.Count > 0 && dtSo.Rows[0].ItemArray[0].ToString().Trim() != "")
            {
                overSo = Convert.ToBoolean(dtSo.Rows[0].ItemArray[0].ToString().Trim());
                if (overSo == true)
                {
                    chkOvrSO.Checked = true;
                }
                else
                {
                    chkOvrSO.Checked = false;
                }

                DilYN = Convert.ToBoolean(dtSo.Rows[1].ItemArray[0].ToString().Trim());
                if (DilYN == true)
                {
                    radioadyes.Checked = true;
                }
                else if (DilYN == false )
                {
                    chkOvrSO.Checked = false;
                }
            }
            else
            {
                chkOvrSO.Checked = false;
            }

            try
            {
                SetupChartOfAccounts();

                // h
            }
            catch { }
            CustomerReturnLoad();
            InvoiceDataLoad(); 
            DeliveryNOteLoad();
            SupplyRetuenLoad();
            InvoiceDataLoad();
            GRNDataload();
            TransferDataLoad();
            IssueNoteLoad();

            GRnAcountload();
            InvoiceAccountLoad();
            DelNoteAacountLoad();
            CusReturnDataLoad();


            //loading the information for labels
            //String SGRN = "select GRNNo,GRNPrefix from tblDefualtSetting";
            //SqlDataAdapter daGRN = new SqlDataAdapter(SGRN, ConnectionString);
            //DataTable dtGRN = new DataTable();
            //daGRN.Fill(dtGRN);
            //if (dtGRN.Rows.Count > 0)
            //{
            //    lblGRNNo.Text = dtGRN.Rows[0].ItemArray[0].ToString().Trim();
            //    lblGrnPfx.Text = dtGRN.Rows[0].ItemArray[1].ToString().Trim();
            //}

            //String SSupIn = "select SupplierInvoiceNo,SupInvoicePrefix from tblDefualtSetting";
            //SqlDataAdapter daSupIn = new SqlDataAdapter(SSupIn, ConnectionString);
            //DataTable dtSupIn = new DataTable();
            //daSupIn.Fill(dtSupIn);
            //if (dtSupIn.Rows.Count > 0)
            //{
            //    lblSupInNo.Text = dtSupIn.Rows[0].ItemArray[0].ToString().Trim();
            //    lblSupInPre.Text = dtSupIn.Rows[0].ItemArray[1].ToString().Trim();
            //}

            //String SSupRe = "select SupplierReturnNo,SupReturnPrefix from tblDefualtSetting";
            //SqlDataAdapter daSupRe = new SqlDataAdapter(SSupRe, ConnectionString);
            //DataTable dtSupRe = new DataTable();
            //daSupRe.Fill(dtSupRe);

            //if (dtSupRe.Rows.Count > 0)
            //{
            //    lblSupRe.Text = dtSupRe.Rows[0].ItemArray[0].ToString().Trim();
            //    lblSupRePre.Text = dtSupRe.Rows[0].ItemArray[1].ToString().Trim();
            //}
            //String SDelNote = "select DeliveryNoteNo,DeliveryNotePrefix from tblDefualtSetting";
            //SqlDataAdapter daDelNote = new SqlDataAdapter(SDelNote, ConnectionString);
            //DataTable dtDelNote = new DataTable();
            //daDelNote.Fill(dtDelNote);

            //if (dtDelNote.Rows.Count > 0)
            //{
            //    lblDelNoteNo.Text = dtDelNote.Rows[0].ItemArray[0].ToString().Trim();
            //    lblDelNotePre.Text = dtDelNote.Rows[0].ItemArray[1].ToString().Trim();
            //}


            //String SInvoice = "select InvoiceNo,InvoicePrefix from tblDefualtSetting";
            //SqlDataAdapter daInvoice = new SqlDataAdapter(SInvoice, ConnectionString);
            //DataTable dtInvoice = new DataTable();
            //daInvoice.Fill(dtInvoice);

            //if (dtInvoice.Rows.Count > 0)
            //{
            //    lblInvoiceNo.Text = dtInvoice.Rows[0].ItemArray[0].ToString().Trim();
            //    lblInvoicePre.Text = dtInvoice.Rows[0].ItemArray[1].ToString().Trim();
            //}

            //String SCusRe = "select CusReturnNo,CusReturnPrefix from tblDefualtSetting";
            //SqlDataAdapter daCusRe = new SqlDataAdapter(SCusRe, ConnectionString);
            //DataTable dtCusRe = new DataTable();
            //daCusRe.Fill(dtCusRe);

            //if (dtCusRe.Rows.Count > 0)
            //{
            //    lblCusReNo.Text = dtCusRe.Rows[0].ItemArray[0].ToString().Trim();
            //    lblCusRePre.Text = dtCusRe.Rows[0].ItemArray[1].ToString().Trim();
            //}
            //String STransNote = "select TransNoteNo,TransNotePrefix from tblDefualtSetting";
            //SqlDataAdapter daTransNote = new SqlDataAdapter(STransNote, ConnectionString);
            //DataTable dtTransNote = new DataTable();
            //daTransNote.Fill(dtTransNote);

            //if (dtTransNote.Rows.Count > 0)
            //{
            //    lblTransNoteNo.Text = dtTransNote.Rows[0].ItemArray[0].ToString().Trim();
            //    lblTransNotePre.Text = dtTransNote.Rows[0].ItemArray[1].ToString().Trim();
            //}

            //String SIssueNote = "select IssueNoteNo,IssuNotePrefix from tblDefualtSetting";
            //SqlDataAdapter daIssueNote = new SqlDataAdapter(SIssueNote, ConnectionString);
            //DataTable dtIssueNote = new DataTable();
            //daIssueNote.Fill(dtIssueNote);

            //if (dtIssueNote.Rows.Count > 0)
            //{
            //    lblIssueNoteNo.Text = dtIssueNote.Rows[0].ItemArray[0].ToString().Trim();
            //    lblIssueNotePre.Text = dtIssueNote.Rows[0].ItemArray[1].ToString().Trim();
            //}
            //String SGRNDrAc = "select GRNDrAc,GRNCrAc from tblDefualtSetting";
            //SqlDataAdapter daGRNDrAc = new SqlDataAdapter(SGRNDrAc, ConnectionString);
            //DataTable dtGRNDrAc = new DataTable();
            //daGRNDrAc.Fill(dtGRNDrAc);

            //if (dtTransNote.Rows.Count > 0)
            //{
            //    lblGrnDrAc.Text = dtGRNDrAc.Rows[0].ItemArray[0].ToString().Trim();
            //    lblGrnCrAc.Text = dtGRNDrAc.Rows[0].ItemArray[1].ToString().Trim();
            //}

            //String SSalesInvDrAc = "select SalesInvDrAc,SalesInvCrAc from tblDefualtSetting";
            //SqlDataAdapter daSalesInvDrAc = new SqlDataAdapter(SSalesInvDrAc, ConnectionString);
            //DataTable dtSalesInvDrAc = new DataTable();
            //daSalesInvDrAc.Fill(dtSalesInvDrAc);

            //if (dtIssueNote.Rows.Count > 0)
            //{
            //    lblSupInDrAc.Text = dtSalesInvDrAc.Rows[0].ItemArray[0].ToString().Trim();
            //    lblSupInCrAc.Text = dtSalesInvDrAc.Rows[0].ItemArray[1].ToString().Trim();
            //}
            //String SDelNoteDrAc = "select DelNoteDrAc,DelNoteCrAc from tblDefualtSetting";
            //SqlDataAdapter daDelNoteDrAc = new SqlDataAdapter(SDelNoteDrAc, ConnectionString);
            //DataTable dtDelNoteDrAc = new DataTable();
            //daDelNoteDrAc.Fill(dtDelNoteDrAc);

            //if (dtTransNote.Rows.Count > 0)
            //{
            //    lblDelNoteDrAc1.Text = dtDelNoteDrAc.Rows[0].ItemArray[0].ToString().Trim();
            //    lblDelNoteCrAc1.Text = dtDelNoteDrAc.Rows[0].ItemArray[1].ToString().Trim();
            //}

            //String SCusretnDrAc = "select CusretnDrAc,CusretnCrAc from tblDefualtSetting";
            //SqlDataAdapter daCusretnDrAc = new SqlDataAdapter(SCusretnDrAc, ConnectionString);
            //DataTable dtCusretnDrAc = new DataTable();
            //daCusretnDrAc.Fill(dtCusretnDrAc);

            //if (dtCusretnDrAc.Rows.Count > 0)
            //{
            //    lblCusretnDrAc1.Text = dtCusretnDrAc.Rows[0].ItemArray[0].ToString().Trim();
            //    lblCusretnCrAc1.Text = dtCusretnDrAc.Rows[0].ItemArray[1].ToString().Trim();
            //}
            btnSupInAcSet.Enabled = false;
            btnSupInSet.Enabled = false;
            btnSupReturnSet.Enabled = false;
            btnTransNoteSet.Enabled = false;
            btnInvoiceSet.Enabled = false;
            btnIssueNoteSet.Enabled = false;
            btnGrnAcSet.Enabled = false;
            button5.Enabled = false;
            btnDelNoteSet.Enabled = false;
            btnCusREturn.Enabled = false;
            btnCusretnAcSet.Enabled = false;
            btnDelNoteAcSet.Enabled = false;
            ///////////////////////////////////////////////////////////////////


            //// this code is for multiwarehouse
            //String SMultiWhse = "select MultiWhse from tblDefualtSetting";
            //SqlDataAdapter daMultiWhse = new SqlDataAdapter(SMultiWhse, ConnectionString);
            //DataTable dtMultiWhse = new DataTable();
            //daMultiWhse.Fill(dtMultiWhse);
            //if (dtMultiWhse.Rows.Count > 0 && dtMultiWhse.Rows[0].ItemArray[0].ToString().Trim() != "")
            //{
            //    allowMWhse = Convert.ToBoolean(dtMultiWhse.Rows[0].ItemArray[0].ToString().Trim());
            //    if (allowMWhse == true)
            //    {
            //        chkAllowMultiWhse.Checked = true;
            //    }
            //    else
            //    {
            //        chkAllowMultiWhse.Checked = false;
            //    }
            //}
            //else
            //{
            //    chkAllowMultiWhse.Checked = false;
            //}
            //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            //+++++++++++++++ for trans type settings +++++++++++++
            txtCusReNo.Enabled = false;
            txtCusRePre.Enabled = false;
            txtDelNoteNo.Enabled = false;
            txtDelNotePre.Enabled = false;
            txtInvoiceNo.Enabled = false;
            txtInvoicePre.Enabled = false;
            txtIsueNoteNo.Enabled = false;
            txtIsueNotePre.Enabled = false;
            txtSupInNo.Enabled = false;
            txtSupInPre.Enabled = false;
            txtSupReNo.Enabled = false;
            txtSupRePre.Enabled = false;
            txtTransNotePre.Enabled = false;
            txtTrnsNoteNo.Enabled = false;
            txtGrnNo.Enabled = false;
            txtGrnPrefix.Enabled = false;


            txtCusReNo.Text = "";
            txtCusRePre.Text = "";
            txtDelNoteNo.Text = "";
            txtDelNotePre.Text = "";
            txtInvoiceNo.Text = "";
            txtInvoicePre.Text = "";
            txtIsueNoteNo.Text = "";
            txtIsueNotePre.Text = "";
            txtSupInNo.Text = "";
            txtSupInPre.Text = "";
            txtSupReNo.Text = "";
            txtSupRePre.Text = "";
            txtTransNotePre.Text = "";
            txtTrnsNoteNo.Text = "";
            txtGrnPrefix.Text = "";
            txtGrnNo.Text = "";
            btnSet.Enabled = false;
            //++++++++++++++++++++++++++++++++++++++++++++++++++

            //++++++++++++++++++ for default number settimgs +++++++
            ///  loadChartofAcount();//Load the all achar of accounts to the Current Combo
            // LoadDefualtDetails();//load all the defualt details to the labels
            //LoadItemID();
            TaxcodeLoad();
            //+++++++++++++++++++++++++++++++++++++++++++++++



            //--Fiil the data to company details
            String s = "select * from tblCompanyInformation";
            SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                txtCompanyName.Text = dt.Rows[0].ItemArray[0].ToString().Trim();
                txtAddress1.Text = dt.Rows[0].ItemArray[1].ToString().Trim();
                txtAddress2.Text = dt.Rows[0].ItemArray[2].ToString().Trim();
                txtCity.Text = dt.Rows[0].ItemArray[3].ToString().Trim();
                cmbState.Text = dt.Rows[0].ItemArray[4].ToString().Trim();
                txtZip.Text = dt.Rows[0].ItemArray[5].ToString().Trim();
                txtCountry.Text = dt.Rows[0].ItemArray[6].ToString().Trim();
                txtTelephone.Text = dt.Rows[0].ItemArray[7].ToString().Trim();
                txtFax.Text = dt.Rows[0].ItemArray[8].ToString().Trim();
                txtWebSite.Text = dt.Rows[0].ItemArray[9].ToString().Trim();
                txtEmail.Text = dt.Rows[0].ItemArray[10].ToString().Trim();
                txtStateEmployer.Text = dt.Rows[0].ItemArray[11].ToString().Trim();
                txtFedEmployer.Text = dt.Rows[0].ItemArray[12].ToString().Trim();
                txtStateUnemployer.Text = dt.Rows[0].ItemArray[13].ToString().Trim();
                cmbFormOfBussiness.Text = dt.Rows[0].ItemArray[14].ToString().Trim();
                txtDirectory.Text = dt.Rows[0].ItemArray[15].ToString().Trim();

                txtCompanyName.ReadOnly = true;
                txtAddress1.ReadOnly = true;
                txtAddress2.ReadOnly = true;
                txtCity.ReadOnly = true;
                cmbState.Enabled = false;
                txtZip.ReadOnly = true;
                txtCountry.ReadOnly = true;
                txtTelephone.ReadOnly = true;
                txtFax.ReadOnly = true;
                txtWebSite.ReadOnly = true;
                txtEmail.ReadOnly = true;
                txtStateEmployer.ReadOnly = true;
                txtFedEmployer.ReadOnly = true;
                txtStateUnemployer.ReadOnly = true;
                cmbFormOfBussiness.Enabled = false;
                btnBrowse.Enabled = false;

                btnComEdit.Enabled = true;
                btnComSave.Enabled = false;
            }
            else
            {
                txtCompanyName.ReadOnly = false;
                txtAddress1.ReadOnly = false;
                txtAddress2.ReadOnly = false;
                txtCity.ReadOnly = false;
                cmbState.Enabled = true;
                txtZip.ReadOnly = false;
                txtCountry.ReadOnly = false;
                txtTelephone.ReadOnly = false;
                txtFax.ReadOnly = false;
                txtWebSite.ReadOnly = false;
                txtEmail.ReadOnly = false;
                txtStateEmployer.ReadOnly = false;
                txtFedEmployer.ReadOnly = false;
                txtStateUnemployer.ReadOnly = false;
                cmbFormOfBussiness.Enabled = true;
                btnBrowse.Enabled = true;

                txtCompanyName.Text = "";
                txtAddress1.Text = "";
                txtAddress2.Text = "";
                txtCity.Text = "";
                cmbState.SelectedItem = null;
                txtZip.Text = "";
                txtCountry.Text = "";
                txtTelephone.Text = "";
                txtFax.Text = "";
                txtWebSite.Text = "";
                txtEmail.Text = "";
                txtStateEmployer.Text = "";
                txtFedEmployer.Text = "";
                txtStateUnemployer.Text = "";
                cmbFormOfBussiness.SelectedItem = null;
                txtDirectory.Text = "";

                btnComEdit.Enabled = false;
                btnComSave.Enabled = true;
            }



            //**************
            //multi colomn
            mltDeliveryDr.Enabled = false;
            mltDeliveryCr.Enabled = false;
            mltCustomerCr.Enabled = false;
            mltCustomerDr.Enabled = false;
            mltDrAccount.Enabled = false;
            mltCrAccount.Enabled = false;
            mltDrSup.Enabled = false;
            mltCrSup.Enabled = false;
            mltDrAccount.SelectedValue = "";
            mltCrAccount.SelectedValue = "";
            mltDrSup.SelectedValue = "";
            mltCrSup.SelectedValue = "";
            mltDeliveryDr.SelectedValue = "";
            mltDeliveryCr.SelectedValue = "";
            mltCustomerCr.SelectedValue = "";
            mltCustomerDr.SelectedValue = "";
        }

        private void SetupChartOfAccounts()
        {
            try
            {

                DataTable dt123 = new DataTable();
                dt123.Columns.Add("AcountID", typeof(String));
                dt123.Columns.Add("AccountDescription", typeof(String));
                String S123 = "select AcountID,AccountDescription from tblChartofAcounts";
                SqlCommand cmd123 = new SqlCommand(S123);
                SqlDataAdapter da123 = new SqlDataAdapter(S123, ConnectionString);
                da123.Fill(dt123);
                mltDrAccount.DataSource = dt123;
                mltDrAccount.DisplayMember = "AcountID";
                mltDrAccount.ValueMember = "AccountDescription";

                DataTable dt124 = new DataTable();
                dt124.Columns.Add("AcountID", typeof(String));
                dt124.Columns.Add("AccountDescription", typeof(String));
                String S124 = "select AcountID,AccountDescription from tblChartofAcounts";
                SqlCommand cmd124 = new SqlCommand(S124);
                SqlDataAdapter da124 = new SqlDataAdapter(S124, ConnectionString);
                da124.Fill(dt124);
                mltCrAccount.DataSource = dt124;
                mltCrAccount.DisplayMember = "AcountID";
                mltCrAccount.ValueMember = "AccountDescription";


                DataTable dt125 = new DataTable();
                dt125.Columns.Add("AcountID", typeof(String));
                dt125.Columns.Add("AccountDescription", typeof(String));
                String S125 = "select AcountID,AccountDescription from tblChartofAcounts";
                SqlCommand cmd125 = new SqlCommand(S125);
                SqlDataAdapter da125 = new SqlDataAdapter(S125, ConnectionString);
                da125.Fill(dt125);
                mltDrSup.DataSource = dt125;
                mltDrSup.DisplayMember = "AcountID";
                mltDrSup.ValueMember = "AccountDescription";


                DataTable dt126 = new DataTable();
                dt126.Columns.Add("AcountID", typeof(String));
                dt126.Columns.Add("AccountDescription", typeof(String));
                String S126 = "select AcountID,AccountDescription from tblChartofAcounts";
                SqlCommand cmd126 = new SqlCommand(S126);
                SqlDataAdapter da126 = new SqlDataAdapter(S126, ConnectionString);
                da126.Fill(dt126);
                mltCrSup.DataSource = dt126;
                mltCrSup.DisplayMember = "AcountID";
                mltCrSup.ValueMember = "AccountDescription";

                //---Sales
                DataTable dt127 = new DataTable();
                dt127.Columns.Add("AcountID", typeof(String));
                dt127.Columns.Add("AccountDescription", typeof(String));
                String S127 = "select AcountID,AccountDescription from tblChartofAcounts";
                SqlCommand cmd127 = new SqlCommand(S127);
                SqlDataAdapter da127 = new SqlDataAdapter(S127, ConnectionString);
                da127.Fill(dt127);
                mltDeliveryDr.DataSource = dt127;
                mltDeliveryDr.DisplayMember = "AcountID";
                mltDeliveryDr.ValueMember = "AccountDescription";

                DataTable dt128 = new DataTable();
                dt128.Columns.Add("AcountID", typeof(String));
                dt128.Columns.Add("AccountDescription", typeof(String));
                String S128 = "select AcountID,AccountDescription from tblChartofAcounts";
                SqlCommand cmd128 = new SqlCommand(S128);
                SqlDataAdapter da128 = new SqlDataAdapter(S128, ConnectionString);
                da128.Fill(dt128);
                mltDeliveryCr.DataSource = dt128;
                mltDeliveryCr.DisplayMember = "AcountID";
                mltDeliveryCr.ValueMember = "AccountDescription";


                DataTable dt129 = new DataTable();
                dt129.Columns.Add("AcountID", typeof(String));
                dt129.Columns.Add("AccountDescription", typeof(String));
                String S129 = "select AcountID,AccountDescription from tblChartofAcounts";
                SqlCommand cmd129 = new SqlCommand(S129);
                SqlDataAdapter da129 = new SqlDataAdapter(S129, ConnectionString);
                da129.Fill(dt129);
                mltCustomerCr.DataSource = dt129;
                mltCustomerCr.DisplayMember = "AcountID";
                mltCustomerCr.ValueMember = "AccountDescription";


                DataTable dt130 = new DataTable();
                dt130.Columns.Add("AcountID", typeof(String));
                dt130.Columns.Add("AccountDescription", typeof(String));
                String S130 = "select AcountID,AccountDescription from tblChartofAcounts";
                SqlCommand cmd130 = new SqlCommand(S130);
                SqlDataAdapter da130 = new SqlDataAdapter(S130, ConnectionString);
                da130.Fill(dt130);
                mltCustomerDr.DataSource = dt130;
                mltCustomerDr.DisplayMember = "AcountID";
                mltCustomerDr.ValueMember = "AccountDescription";


                DataTable dt131 = new DataTable();
                dt131.Columns.Add("AcountID", typeof(String));
                dt131.Columns.Add("AccountDescription", typeof(String));
                String S131 = "select AcountID,AccountDescription from tblChartofAcounts";
                SqlCommand cmd131 = new SqlCommand(S131);
                SqlDataAdapter da131 = new SqlDataAdapter(S131, ConnectionString);
                da131.Fill(dt131);
                txttaxAccount.DataSource = dt131;
                txttaxAccount.DisplayMember = "AcountID";
                txttaxAccount.ValueMember = "AccountDescription";
                 

            }
            catch { }
        }

        //++++++++++ trans type settings +++++++++++++++++++++
        private void button2_Click(object sender, EventArgs e)//for edit button
        {
            bool canAdd = false;
            dtUser = DataAccess.Access.setUserAuthentication(UserAutherization.user.userName, "frmMasterSettings");
            if (dtUser.Rows.Count > 0)
            {
                canAdd = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
            }
            if (canAdd)
            {
                txtCusReNo.Enabled = true;
                txtCusRePre.Enabled = true;
                txtDelNoteNo.Enabled = true;
                txtDelNotePre.Enabled = true;
                txtInvoiceNo.Enabled = true;
                txtInvoicePre.Enabled = true;
                txtIsueNoteNo.Enabled = true;
                txtIsueNotePre.Enabled = true;
                txtSupInNo.Enabled = true;
                txtSupInPre.Enabled = true;
                txtSupReNo.Enabled = true;
                txtSupRePre.Enabled = true;
                txtTransNotePre.Enabled = true;
                txtTrnsNoteNo.Enabled = true;

                txtCusReNo.Text = "";
                txtCusRePre.Text = "";
                txtDelNoteNo.Text = "";
                txtDelNotePre.Text = "";
                txtInvoiceNo.Text = "";
                txtInvoicePre.Text = "";
                txtIsueNoteNo.Text = "";
                txtIsueNotePre.Text = "";
                txtSupInNo.Text = "";
                txtSupInPre.Text = "";
                txtSupReNo.Text = "";
                txtSupRePre.Text = "";
                txtTransNotePre.Text = "";
                txtTrnsNoteNo.Text = "";

                btnSet.Enabled = true;

                String SSupIn = "select SupplierInvoiceNo,SupInvoicePrefix from tblDefualtSetting";
                SqlDataAdapter daSupIn = new SqlDataAdapter(SSupIn, ConnectionString);
                DataTable dtSupIn = new DataTable();
                daSupIn.Fill(dtSupIn);
                if (dtSupIn.Rows.Count > 0)
                {
                    lblSupInNo.Text = dtSupIn.Rows[0].ItemArray[0].ToString().Trim();
                    lblSupInPre.Text = dtSupIn.Rows[0].ItemArray[1].ToString().Trim();
                }

                String SSupRe = "select SupplierReturnNo,SupReturnPrefix from tblDefualtSetting";
                SqlDataAdapter daSupRe = new SqlDataAdapter(SSupRe, ConnectionString);
                DataTable dtSupRe = new DataTable();
                daSupRe.Fill(dtSupRe);

                if (dtSupRe.Rows.Count > 0)
                {
                    lblSupRe.Text = dtSupRe.Rows[0].ItemArray[0].ToString().Trim();
                    lblSupRePre.Text = dtSupRe.Rows[0].ItemArray[1].ToString().Trim();
                }

                String SDelNote = "select DeliveryNoteNo,DeliveryNotePrefix from tblDefualtSetting";
                SqlDataAdapter daDelNote = new SqlDataAdapter(SDelNote, ConnectionString);
                DataTable dtDelNote = new DataTable();
                daDelNote.Fill(dtDelNote);

                if (dtDelNote.Rows.Count > 0)
                {
                    lblDelNoteNo.Text = dtDelNote.Rows[0].ItemArray[0].ToString().Trim();
                    lblDelNotePre.Text = dtDelNote.Rows[0].ItemArray[1].ToString().Trim();
                }

                String STransNote = "select TransNoteNo,TransNotePrefix from tblDefualtSetting";
                SqlDataAdapter daTransNote = new SqlDataAdapter(STransNote, ConnectionString);
                DataTable dtTransNote = new DataTable();
                daTransNote.Fill(dtTransNote);

                if (dtTransNote.Rows.Count > 0)
                {
                    lblTransNoteNo.Text = dtTransNote.Rows[0].ItemArray[0].ToString().Trim();
                    lblTransNotePre.Text = dtTransNote.Rows[0].ItemArray[1].ToString().Trim();
                }

                String SIssueNote = "select IssueNoteNo,IssuNotePrefix from tblDefualtSetting";
                SqlDataAdapter daIssueNote = new SqlDataAdapter(SIssueNote, ConnectionString);
                DataTable dtIssueNote = new DataTable();
                daIssueNote.Fill(dtIssueNote);

                if (dtIssueNote.Rows.Count > 0)
                {
                    lblIssueNoteNo.Text = dtIssueNote.Rows[0].ItemArray[0].ToString().Trim();
                    lblIssueNotePre.Text = dtIssueNote.Rows[0].ItemArray[1].ToString().Trim();
                }

                String SInvoice = "select InvoiceNo,InvoicePrefix from tblDefualtSetting";
                SqlDataAdapter daInvoice = new SqlDataAdapter(SInvoice, ConnectionString);
                DataTable dtInvoice = new DataTable();
                daInvoice.Fill(dtInvoice);

                if (dtInvoice.Rows.Count > 0)
                {
                    lblInvoiceNo.Text = dtInvoice.Rows[0].ItemArray[0].ToString().Trim();
                    lblInvoicePre.Text = dtInvoice.Rows[0].ItemArray[1].ToString().Trim();
                }

                String SCusRe = "select CusReturnNo,CusReturnPrefix from tblDefualtSetting";
                SqlDataAdapter daCusRe = new SqlDataAdapter(SCusRe, ConnectionString);
                DataTable dtCusRe = new DataTable();
                daCusRe.Fill(dtCusRe);

                if (dtCusRe.Rows.Count > 0)
                {
                    lblCusReNo.Text = dtCusRe.Rows[0].ItemArray[0].ToString().Trim();
                    lblCusRePre.Text = dtCusRe.Rows[0].ItemArray[1].ToString().Trim();
                }


            }
            else
            {
                MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            //SqlCommand myCommand = new SqlCommand();
            myConnection.Open();
            SqlTransaction myTrans = myConnection.BeginTransaction();

            try
            {
                //++++++++++++++++++++++++++ THIS PART IS FOR SETTING NUMBERS+++++++++++++++++++1
                //++++++++++++ SUPPLER INVOICE++++++++1
                SqlCommand cmd1 = new SqlCommand("UPDATE tblDefualtSetting  set SupplierInvoiceNo = '" + txtSupInNo.Text.ToString().Trim() + "' select SupplierInvoiceNo from tblDefualtSetting", myConnection, myTrans);
                SqlDataAdapter daSupIn = new SqlDataAdapter(cmd1);
                DataTable dtSupIn = new DataTable();
                daSupIn.Fill(dtSupIn);
                if (dtSupIn.Rows.Count <= 0)
                {
                    SqlCommand cmd15 = new SqlCommand("insert into tblDefualtSetting(SupplierInvoiceNo) values ('" + txtSupInNo.Text.ToString().Trim() + "')", myConnection, myTrans);
                    cmd15.ExecuteNonQuery();
                }

                //++++++++++++++++++++ SUPPLIER RETURN++++++++++++++++2
                SqlCommand cmd2 = new SqlCommand("UPDATE tblDefualtSetting  set SupplierReturnNo = '" + txtSupReNo.Text.ToString().Trim() + "' select SupplierReturnNo from tblDefualtSetting", myConnection, myTrans);
                SqlDataAdapter daSupRe = new SqlDataAdapter(cmd2);
                DataTable dtSupRe = new DataTable();
                daSupRe.Fill(dtSupRe);

                if (dtSupRe.Rows.Count <= 0)
                {
                    SqlCommand cmd17 = new SqlCommand("insert into tblDefualtSetting(SupplierReturnNo) values ('" + txtSupReNo.Text.ToString().Trim() + "')", myConnection, myTrans);
                    cmd17.ExecuteNonQuery();
                }

                //++++++++++++++ DELIVARY NOTE ++++++++3
                SqlCommand cmd3 = new SqlCommand("UPDATE tblDefualtSetting  set DeliveryNoteNo = '" + txtDelNoteNo.Text.ToString().Trim() + "' select DeliveryNoteNo from tblDefualtSetting", myConnection, myTrans);
                SqlDataAdapter daDelNote = new SqlDataAdapter(cmd3);
                DataTable dtDelNote = new DataTable();
                daDelNote.Fill(dtDelNote);

                if (dtDelNote.Rows.Count <= 0)
                {
                    SqlCommand cmd18 = new SqlCommand("insert into tblDefualtSetting(DeliveryNoteNo) values ('" + txtDelNoteNo.Text.ToString().Trim() + "')", myConnection, myTrans);
                    cmd18.ExecuteNonQuery();
                }


                //++++++++++++++++ TRANSFER NOTE +++++++++++4
                SqlCommand cmd4 = new SqlCommand("UPDATE tblDefualtSetting  set TransNoteNo = '" + txtTrnsNoteNo.Text.ToString().Trim() + "' select TransNoteNo from tblDefualtSetting", myConnection, myTrans);
                SqlDataAdapter daTransNote = new SqlDataAdapter(cmd4);
                DataTable dtTransNote = new DataTable();
                daTransNote.Fill(dtTransNote);

                if (dtTransNote.Rows.Count <= 0)
                {
                    //myCommand.CommandText = "Insert into tblDefualtSetting (TransNoteNo) values ('" + txtTrnsNoteNo.Text.ToString().Trim() + "')";
                    SqlCommand cmd19 = new SqlCommand("insert into tblDefualtSetting(TransNoteNo) values ('" + txtTrnsNoteNo.Text.ToString().Trim() + "')", myConnection, myTrans);
                    cmd19.ExecuteNonQuery();
                }

                //++++++++++++++++ ISSUE NOTE ++++++++++5
                //myCommand.CommandText = "UPDATE tblDefualtSetting with (rowlock) SET IssueNoteNo = '" + txtIsueNoteNo.Text.ToString().Trim() + "' select IssueNoteNo from tblDefualtSetting  with (rowlock)";
                SqlCommand cmd5 = new SqlCommand("UPDATE tblDefualtSetting  set IssueNoteNo = '" + txtIsueNoteNo.Text.ToString().Trim() + "' select IssueNoteNo from tblDefualtSetting", myConnection, myTrans);
                SqlDataAdapter daIssueNote = new SqlDataAdapter(cmd5);
                DataTable dtIssueNote = new DataTable();
                daIssueNote.Fill(dtIssueNote);

                if (dtIssueNote.Rows.Count <= 0)
                {
                    //myCommand.CommandText = "Insert into tblDefualtSetting (IssueNoteNo) values ('" + txtIsueNoteNo.Text.ToString().Trim() + "')";
                    SqlCommand cmd20 = new SqlCommand("insert into tblDefualtSetting(IssueNoteNo) values ('" + txtIsueNoteNo.Text.ToString().Trim() + "')", myConnection, myTrans);
                    cmd20.ExecuteNonQuery();
                }

                //+++++++++++++++++ INVOICE +++++++6
                //myCommand.CommandText = "UPDATE tblDefualtSetting with (rowlock) SET InvoiceNo = '" + txtInvoiceNo.Text.ToString().Trim() + "' select InvoiceNo from tblDefualtSetting  with (rowlock)";
                SqlCommand cmd6 = new SqlCommand("UPDATE tblDefualtSetting  set InvoiceNo = '" + txtInvoiceNo.Text.ToString().Trim() + "' select InvoiceNo from tblDefualtSetting", myConnection, myTrans);
                SqlDataAdapter daInvoice = new SqlDataAdapter(cmd6);
                DataTable dtInvoice = new DataTable();
                daInvoice.Fill(dtInvoice);

                if (dtInvoice.Rows.Count <= 0)
                {
                    //myCommand.CommandText = "Insert into tblDefualtSetting (InvoiceNo) values ('" + txtInvoiceNo.Text.ToString().Trim() + "')";
                    SqlCommand cmd21 = new SqlCommand("insert into tblDefualtSetting(InvoiceNo) values ('" + txtInvoiceNo.Text.ToString().Trim() + "')", myConnection, myTrans);
                    cmd21.ExecuteNonQuery();
                }

                //+++++++++++++++++++++ CUSTOMER RETUN ++++++++++++++++++7
                //myCommand.CommandText = "UPDATE tblDefualtSetting with (rowlock) SET CusReturnNo = '" + txtCusReNumber.Text.ToString().Trim() + "' select CusReturnNo from tblDefualtSetting  with (rowlock)";
                SqlCommand cmd7 = new SqlCommand("UPDATE tblDefualtSetting  set CusReturnNo = '" + txtCusReNo.Text.ToString().Trim() + "' select CusReturnNo from tblDefualtSetting", myConnection, myTrans);
                SqlDataAdapter daCusRe = new SqlDataAdapter(cmd7);
                DataTable dtCusRe = new DataTable();
                daCusRe.Fill(dtCusRe);

                if (dtCusRe.Rows.Count <= 0)
                {
                    //myCommand.CommandText = "Insert into tblDefualtSetting (CusReturnNo) values ('" + txtCusReNumber.Text.ToString().Trim() + "')";
                    SqlCommand cmd22 = new SqlCommand("insert into tblDefualtSetting(CusReturnNo) values ('" + txtCusReNo.Text.ToString().Trim() + "')", myConnection, myTrans);
                    cmd22.ExecuteNonQuery();
                }

                //+++++++++++++++++++++++ this part is for Setting prefix +++++++++++++++++++++++++++++++++++

                //++++++++++++ SUPLLIER INVOICE +++++++++++++++++++++++++1
                //myCommand.CommandText = "UPDATE tblDefualtSetting with (rowlock) SET SupInvoicePrefix = '" + txtSupInPre.Text.ToString().Trim() + "' select SupInvoicePrefix from tblDefualtSetting  with (rowlock)";
                SqlCommand cmd8 = new SqlCommand("UPDATE tblDefualtSetting  set SupInvoicePrefix = '" + txtSupInPre.Text.ToString().Trim() + "' select SupInvoicePrefix from tblDefualtSetting", myConnection, myTrans);
                SqlDataAdapter daSupInPre = new SqlDataAdapter(cmd8);
                DataTable dtSupInPre = new DataTable();
                daSupInPre.Fill(dtSupInPre);

                if (dtSupInPre.Rows.Count <= 0)
                {
                    //myCommand.CommandText = "Insert into tblDefualtSetting (SupInvoicePrefix) values ('" + txtSupInPre.Text.ToString().Trim() + "')";
                    SqlCommand cmd23 = new SqlCommand("insert into tblDefualtSetting(SupInvoicePrefix) values ('" + txtSupInPre.Text.ToString().Trim() + "')", myConnection, myTrans);
                    cmd23.ExecuteNonQuery();
                }

                //+++++++++ SUPLIER RETURN ++++++++++++++2
                //myCommand.CommandText = "UPDATE tblDefualtSetting with (rowlock) SET SupReturnPrefix = '" + txtSupRePre.Text.ToString().Trim() + "' select SupReturnPrefix from tblDefualtSetting  with (rowlock)";
                SqlCommand cmd9 = new SqlCommand("UPDATE tblDefualtSetting  set SupReturnPrefix = '" + txtSupRePre.Text.ToString().Trim() + "' select SupReturnPrefix from tblDefualtSetting", myConnection, myTrans);
                SqlDataAdapter daSupRePre = new SqlDataAdapter(cmd9);
                DataTable dtSupRePre = new DataTable();
                daSupRePre.Fill(dtSupRePre);

                if (dtSupRePre.Rows.Count <= 0)
                {
                    //myCommand.CommandText = "Insert into tblDefualtSetting (SupReturnPrefix) values ('" + txtSupRePre.Text.ToString().Trim() + "')";
                    SqlCommand cmd24 = new SqlCommand("insert into tblDefualtSetting(SupReturnPrefix) values ('" + txtSupRePre.Text.ToString().Trim() + "')", myConnection, myTrans);
                    cmd24.ExecuteNonQuery();
                }

                //+++++++++++++++++++++++ DELIVERY NOTE +++++++++++++3 
                //myCommand.CommandText = "UPDATE tblDefualtSetting with (rowlock) SET DeliveryNotePrefix = '" + txtDelNotePre.Text.ToString().Trim() + "' select DeliveryNotePrefix from tblDefualtSetting  with (rowlock)";
                SqlCommand cmd10 = new SqlCommand("UPDATE tblDefualtSetting  set DeliveryNotePrefix = '" + txtDelNotePre.Text.ToString().Trim() + "' select DeliveryNotePrefix from tblDefualtSetting", myConnection, myTrans);
                SqlDataAdapter daDelNotePre = new SqlDataAdapter(cmd10);
                DataTable dtDelNotePre = new DataTable();
                daDelNotePre.Fill(dtDelNotePre);

                if (dtDelNotePre.Rows.Count <= 0)
                {
                    //myCommand.CommandText = "Insert into tblDefualtSetting (DeliveryNotePrefix) values ('" + txtDelNotePre.Text.ToString().Trim() + "')";
                    SqlCommand cmd25 = new SqlCommand("insert into tblDefualtSetting(DeliveryNotePrefix) values ('" + txtDelNotePre.Text.ToString().Trim() + "')", myConnection, myTrans);
                    cmd25.ExecuteNonQuery();
                }

                //++++++++++++++++ TRANSFER NOTE +++++++++4
                //myCommand.CommandText = "UPDATE tblDefualtSetting with (rowlock) SET TransNotePrefix = '" + txtTransNotePre.Text.ToString().Trim() + "' select TransNotePrefix from tblDefualtSetting  with (rowlock)";
                SqlCommand cmd11 = new SqlCommand("UPDATE tblDefualtSetting  set TransNotePrefix = '" + txtTransNotePre.Text.ToString().Trim() + "' select TransNotePrefix from tblDefualtSetting", myConnection, myTrans);
                SqlDataAdapter daTransNotePre = new SqlDataAdapter(cmd11);
                DataTable dtTransNotePre = new DataTable();
                daTransNotePre.Fill(dtTransNotePre);

                if (dtTransNotePre.Rows.Count <= 0)
                {
                    //myCommand.CommandText = "Insert into tblDefualtSetting (TransNotePrefix) values ('" + txtTransNotePre.Text.ToString().Trim() + "')";
                    SqlCommand cmd26 = new SqlCommand("insert into tblDefualtSetting(TransNotePrefix) values ('" + txtTransNotePre.Text.ToString().Trim() + "')", myConnection, myTrans);
                    cmd26.ExecuteNonQuery();
                }

                //++++++++++++++++ ISSUE NOTE+++++++++++++++++5
                //myCommand.CommandText = "UPDATE tblDefualtSetting with (rowlock) SET IssuNotePrefix = '" + txtIsueNotePre.Text.ToString().Trim() + "' select IssuNotePrefix from tblDefualtSetting  with (rowlock)";
                SqlCommand cmd12 = new SqlCommand("UPDATE tblDefualtSetting  set IssuNotePrefix = '" + txtIsueNotePre.Text.ToString().Trim() + "' select IssuNotePrefix from tblDefualtSetting", myConnection, myTrans);
                SqlDataAdapter daIssueNotePre = new SqlDataAdapter(cmd12);
                DataTable dtIssueNotePre = new DataTable();
                daIssueNotePre.Fill(dtIssueNotePre);

                if (dtIssueNotePre.Rows.Count <= 0)
                {
                    //myCommand.CommandText = "Insert into tblDefualtSetting (SupplierReturnNo) values ('" + txtIsueNotePre.Text.ToString().Trim() + "')";
                    SqlCommand cmd26 = new SqlCommand("insert into tblDefualtSetting(IssuNotePrefix) values ('" + txtIsueNotePre.Text.ToString().Trim() + "')", myConnection, myTrans);
                    cmd26.ExecuteNonQuery();
                }

                //+++++++++++INVOICE+++++++++++6 
                //myCommand.CommandText = "UPDATE tblDefualtSetting with (rowlock) SET InvoicePrefix = '" + txtInvoicePre.Text.ToString().Trim() + "' select InvoicePrefix from tblDefualtSetting  with (rowlock)";
                SqlCommand cmd13 = new SqlCommand("UPDATE tblDefualtSetting  set InvoicePrefix = '" + txtInvoicePre.Text.ToString().Trim() + "' select InvoicePrefix from tblDefualtSetting", myConnection, myTrans);
                SqlDataAdapter daInvoicePre = new SqlDataAdapter(cmd13);
                DataTable dtInvoicePre = new DataTable();
                daInvoicePre.Fill(dtInvoicePre);

                if (dtInvoicePre.Rows.Count <= 0)
                {
                    //myCommand.CommandText = "Insert into tblDefualtSetting (InvoicePrefix) values ('" + txtInvoicePre.Text.ToString().Trim() + "')";
                    SqlCommand cmd27 = new SqlCommand("insert into tblDefualtSetting(InvoicePrefix) values ('" + txtInvoicePre.Text.ToString().Trim() + "')", myConnection, myTrans);
                    cmd27.ExecuteNonQuery();
                }

                //+++++++++++++ CUSTOMER RETURN++++++++7
                //myCommand.CommandText = "UPDATE tblDefualtSetting with (rowlock) SET CusReturnPrefix = '" + txtCusRePrefix.Text.ToString().Trim() + "' select CusReturnPrefix from tblDefualtSetting  with (rowlock)";
                SqlCommand cmd14 = new SqlCommand("UPDATE tblDefualtSetting  set CusReturnPrefix = '" + txtCusRePre.Text.ToString().Trim() + "' select CusReturnPrefix from tblDefualtSetting", myConnection, myTrans);
                SqlDataAdapter daCusRePre = new SqlDataAdapter(cmd14);
                DataTable dtCusRePre = new DataTable();
                daCusRePre.Fill(dtCusRePre);

                if (dtCusRePre.Rows.Count <= 0)
                {
                    //myCommand.CommandText = "Insert into tblDefualtSetting (CusReturnPrefix) values ('" + txtCusRePrefix.Text.ToString().Trim() + "')";
                    SqlCommand cmd28 = new SqlCommand("insert into tblDefualtSetting(CusReturnPrefix) values ('" + txtCusRePre.Text.ToString().Trim() + "')", myConnection, myTrans);
                    cmd28.ExecuteNonQuery();
                }
                myTrans.Commit();//Transaction is cometted here

                DialogResult reply = MessageBox.Show("Settings have Saved Successfully", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (reply == DialogResult.OK)
                {
                    txtCusReNo.Enabled = false;
                    txtCusRePre.Enabled = false;
                    txtDelNoteNo.Enabled = false;
                    txtDelNotePre.Enabled = false;
                    txtInvoiceNo.Enabled = false;
                    txtInvoicePre.Enabled = false;
                    txtIsueNoteNo.Enabled = false;
                    txtIsueNotePre.Enabled = false;
                    txtSupInNo.Enabled = false;
                    txtSupInPre.Enabled = false;
                    txtSupReNo.Enabled = false;
                    txtSupRePre.Enabled = false;
                    txtTransNotePre.Enabled = false;
                    txtTrnsNoteNo.Enabled = false;

                    txtCusReNo.Text = "";
                    txtCusRePre.Text = "";
                    txtDelNoteNo.Text = "";
                    txtDelNotePre.Text = "";
                    txtInvoiceNo.Text = "";
                    txtInvoicePre.Text = "";
                    txtIsueNoteNo.Text = "";
                    txtIsueNotePre.Text = "";
                    txtSupInNo.Text = "";
                    txtSupInPre.Text = "";
                    txtSupReNo.Text = "";
                    txtSupRePre.Text = "";
                    txtTransNotePre.Text = "";
                    txtTrnsNoteNo.Text = "";

                    btnSet.Enabled = false;
                }

            }
            catch
            {
                myTrans.Rollback();
                MessageBox.Show("Error occured while Transfering, whole prosess is Rollback", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                myConnection.Close();
            }
        }

        private void btnSupInSet_Click(object sender, EventArgs e)
        {
            try
            {
                setConnectionString();
                SqlConnection myConnection = new SqlConnection(ConnectionString);
                SqlCommand myCommand = new SqlCommand();
                SqlTransaction myTrans;
                myConnection.Open();
                myCommand.Connection = myConnection;

                myTrans = myConnection.BeginTransaction();
                myCommand.Transaction = myTrans;//start the trasaction scope
                if (txtSupInNo.Text != "" && txtSupInPre.Text != "")
                {
                    try
                    {
                        //++++++++++++++++++++++++++ THIS PART IS FOR SETTING NUMBERS+++++++++++++++++++1
                        //++++++++++++ SUPPLER INVOICE++++++++1
                        myCommand.CommandText = "UPDATE tblDefualtSetting with (rowlock) SET SupplierInvoiceNo = '" + txtSupInNo.Text.ToString().Trim() + "' select SupplierInvoiceNo from tblDefualtSetting  with (rowlock)";
                        SqlDataAdapter daSupIn = new SqlDataAdapter(myCommand.CommandText, ConnectionString);
                        DataTable dtSupIn = new DataTable();
                        daSupIn.Fill(dtSupIn);
                        if (dtSupIn.Rows.Count <= 0)
                        {
                            myCommand.CommandText = "Insert into tblDefualtSetting (SupplierInvoiceNo) values ('" + txtSupInNo.Text.ToString().Trim() + "')";
                            myCommand.ExecuteNonQuery();
                        }

                        myCommand.CommandText = "UPDATE tblDefualtSetting with (rowlock) SET SupInvoicePrefix = '" + txtSupInPre.Text.ToString().Trim() + "' select SupInvoicePrefix from tblDefualtSetting  with (rowlock)";
                        SqlDataAdapter daSupInPre = new SqlDataAdapter(myCommand.CommandText, ConnectionString);
                        DataTable dtSupInPre = new DataTable();
                        daSupInPre.Fill(dtSupInPre);

                        if (dtSupInPre.Rows.Count <= 0)
                        {
                            myCommand.CommandText = "Insert into tblDefualtSetting (SupInvoicePrefix) values ('" + txtSupInPre.Text.ToString().Trim() + "')";
                            myCommand.ExecuteNonQuery();
                        }

                        SqlCommand cmd3 = new SqlCommand("UPDATE tblDefaultSettingCodeGen set IsAuto='" + chkSInVAuto.Checked + "' where TransType='SINV'", myConnection, myTrans);
                        daSupInPre = new SqlDataAdapter(cmd3);
                        dtSupInPre = new DataTable();
                        daSupInPre.Fill(dtSupInPre);

                        myTrans.Commit();//Transaction is cometted here

                        DialogResult reply = MessageBox.Show("Setting has Saved Successfully", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtSupInNo.Text = "";
                        txtSupInPre.Text = "";
                        txtSupInNo.Enabled = false;
                        txtSupInPre.Enabled = false;
                        btnSupInSet.Enabled = false;
                    }
                    catch
                    {
                        myTrans.Rollback();
                        MessageBox.Show("Error occured while Transfering, whole prosess is Rollback", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    finally
                    {
                        myConnection.Close();
                    }
                }
                else
                {
                    MessageBox.Show("You must Enter Prifix Or number");
                  //  MessageBox.Show("Fill The Fieldes");
                }
            }
            catch { }
            InvoiceDataLoad();
           // frmDefaultSettings_Load(sender, e);
        }

        private void btnSupReturnSet_Click(object sender, EventArgs e)
        {
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlCommand myCommand = new SqlCommand();
            SqlTransaction myTrans;
            myConnection.Open();
            myCommand.Connection = myConnection;

            myTrans = myConnection.BeginTransaction();
            myCommand.Transaction = myTrans;//start the trasaction scope
            if (txtSupReNo.Text != "" && txtSupRePre.Text != "")
            {
                try
                {
                    //++++++++++++++++++++ SUPPLIER RETURN++++++++++++++++2
                    myCommand.CommandText = "UPDATE tblDefualtSetting with (rowlock) SET SupplierReturnNo = '" + txtSupReNo.Text.ToString().Trim() + "' select SupplierReturnNo from tblDefualtSetting  with (rowlock)";
                    SqlDataAdapter daSupRe = new SqlDataAdapter(myCommand.CommandText, ConnectionString);
                    DataTable dtSupRe = new DataTable();
                    daSupRe.Fill(dtSupRe);

                    if (dtSupRe.Rows.Count <= 0)
                    {
                        myCommand.CommandText = "Insert into tblDefualtSetting (SupplierReturnNo) values ('" + txtSupReNo.Text.ToString().Trim() + "')";
                        myCommand.ExecuteNonQuery();
                    }
                    //+++++++++ SUPLIER RETURN ++++++++++++++2
                    myCommand.CommandText = "UPDATE tblDefualtSetting with (rowlock) SET SupReturnPrefix = '" + txtSupRePre.Text.ToString().Trim() + "' select SupReturnPrefix from tblDefualtSetting  with (rowlock)";
                    SqlDataAdapter daSupRePre = new SqlDataAdapter(myCommand.CommandText, ConnectionString);
                    DataTable dtSupRePre = new DataTable();
                    daSupRePre.Fill(dtSupRePre);

                    if (dtSupRePre.Rows.Count <= 0)
                    {
                        myCommand.CommandText = "Insert into tblDefualtSetting (SupReturnPrefix) values ('" + txtSupRePre.Text.ToString().Trim() + "')";
                        myCommand.ExecuteNonQuery();
                    }

                    SqlCommand cmd3 = new SqlCommand("UPDATE tblDefaultSettingCodeGen set IsAuto='" + chkGRNAuto.Checked + "' where TransType='SRTN'", myConnection, myTrans);
                    SqlDataAdapter daSupInPre = new SqlDataAdapter(cmd3);
                    DataTable dtSupInPre = new DataTable();
                    daSupInPre.Fill(dtSupInPre);

                    myTrans.Commit();//Transaction is cometted here

                    DialogResult reply = MessageBox.Show("Setting has Saved Successfully", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtSupReNo.Text = "";
                    txtSupRePre.Text = "";
                    txtSupReNo.Enabled = false;
                    txtSupRePre.Enabled = false;
                    btnSupReturnSet.Enabled = false;
                    //btnSupInAcSet.Enabled = true;
                }
                catch
                {
                    myTrans.Rollback();
                    MessageBox.Show("Error occured while Transfering, whole prosess is Rollback", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                finally
                {
                    myConnection.Close();
                }
            }
            else
            {
                MessageBox.Show("You must Enter Prifix Or number");
            }
           // frmDefaultSettings_Load(sender, e);
            SupplyRetuenLoad();
        }



        //++++++++++ default nu number settings
        private void btnDelNoteSet_Click(object sender, EventArgs e)
        {
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlCommand myCommand = new SqlCommand();
            SqlTransaction myTrans;
            myConnection.Open();
            myCommand.Connection = myConnection;

            myTrans = myConnection.BeginTransaction();
            myCommand.Transaction = myTrans;//start the trasaction scope
            if (txtDelNoteNo.Text != "" && txtDelNotePre.Text != "")
            {
                try
                {
                    //++++++++++++++ DELIVARY NOTE ++++++++3
                    myCommand.CommandText = "UPDATE tblDefualtSetting with (rowlock) SET DeliveryNoteNo = '" + txtDelNoteNo.Text.ToString().Trim() + "' select DeliveryNoteNo from tblDefualtSetting  with (rowlock)";
                    SqlDataAdapter daDelNote = new SqlDataAdapter(myCommand.CommandText, ConnectionString);
                    DataTable dtDelNote = new DataTable();
                    daDelNote.Fill(dtDelNote);

                    if (dtDelNote.Rows.Count <= 0)
                    {
                        myCommand.CommandText = "Insert into tblDefualtSetting (DeliveryNoteNo) values ('" + txtDelNoteNo.Text.ToString().Trim() + "')";
                        myCommand.ExecuteNonQuery();
                    }
                    //+++++++++++++++++++++++ DELIVERY NOTE +++++++++++++3 
                    myCommand.CommandText = "UPDATE tblDefualtSetting with (rowlock) SET DeliveryNotePrefix = '" + txtDelNotePre.Text.ToString().Trim() + "' select DeliveryNotePrefix from tblDefualtSetting  with (rowlock)";
                    SqlDataAdapter daDelNotePre = new SqlDataAdapter(myCommand.CommandText, ConnectionString);
                    DataTable dtDelNotePre = new DataTable();
                    daDelNotePre.Fill(dtDelNotePre);

                    if (dtDelNotePre.Rows.Count <= 0)
                    {
                        myCommand.CommandText = "Insert into tblDefualtSetting (DeliveryNotePrefix) values ('" + txtDelNotePre.Text.ToString().Trim() + "')";
                        myCommand.ExecuteNonQuery();
                    }

                    SqlCommand cmd3 = new SqlCommand("UPDATE tblDefaultSettingCodeGen set IsAuto='" + chkGRNAuto.Checked + "' where TransType='DN'", myConnection, myTrans);
                     SqlDataAdapter daSupInPre = new SqlDataAdapter(cmd3);
                     DataTable dtSupInPre = new DataTable();
                    daSupInPre.Fill(dtSupInPre);

                    myTrans.Commit();//Transaction is cometted here

                    DialogResult reply = MessageBox.Show("Setting has Saved Successfully", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtDelNoteNo.Text = "";
                    txtDelNotePre.Text = "";
                    txtDelNoteNo.Enabled = false;
                    txtDelNotePre.Enabled = false;
                    btnDelNoteSet.Enabled = false;
                    btnSet.Enabled = false;
                }
                catch
                {
                    myTrans.Rollback();
                    MessageBox.Show("Error occured while Transfering, whole prosess is Rollback", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                finally
                {
                    myConnection.Close();
                }
            }
            else
            {
                MessageBox.Show("You must Enter Prifix Or number");
            }
           // frmDefaultSettings_Load(sender, e);
            DeliveryNOteLoad();
        }

        private void btnInvoiceSet_Click(object sender, EventArgs e)
        {
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlCommand myCommand = new SqlCommand();
            SqlTransaction myTrans;
            myConnection.Open();
            myCommand.Connection = myConnection;

            myTrans = myConnection.BeginTransaction();
            myCommand.Transaction = myTrans;//start the trasaction scope
            if (txtInvoiceNo.Text != "" && txtInvoicePre.Text != "")
            {
                try
                {

                    //+++++++++++++++++ INVOICE +++++++6
                    myCommand.CommandText = "UPDATE tblDefualtSetting with (rowlock) SET InvoiceNo = '" + txtInvoiceNo.Text.ToString().Trim() + "' select InvoiceNo from tblDefualtSetting  with (rowlock)";
                    SqlDataAdapter daInvoice = new SqlDataAdapter(myCommand.CommandText, ConnectionString);
                    DataTable dtInvoice = new DataTable();
                    daInvoice.Fill(dtInvoice);

                    if (dtInvoice.Rows.Count <= 0)
                    {
                        myCommand.CommandText = "Insert into tblDefualtSetting (InvoiceNo) values ('" + txtInvoiceNo.Text.ToString().Trim() + "')";
                        myCommand.ExecuteNonQuery();
                    }
                    //+++++++++++INVOICE+++++++++++6 
                    myCommand.CommandText = "UPDATE tblDefualtSetting with (rowlock) SET InvoicePrefix = '" + txtInvoicePre.Text.ToString().Trim() + "' select InvoicePrefix from tblDefualtSetting  with (rowlock)";
                    SqlDataAdapter daInvoicePre = new SqlDataAdapter(myCommand.CommandText, ConnectionString);
                    DataTable dtInvoicePre = new DataTable();
                    daInvoicePre.Fill(dtInvoicePre);

                    if (dtInvoicePre.Rows.Count <= 0)
                    {
                        myCommand.CommandText = "Insert into tblDefualtSetting (InvoicePrefix) values ('" + txtInvoicePre.Text.ToString().Trim() + "')";
                        myCommand.ExecuteNonQuery();
                    }

                    SqlCommand cmd3 = new SqlCommand("UPDATE tblDefaultSettingCodeGen set IsAuto='" + chkGRNAuto.Checked + "' where TransType='CINV'", myConnection, myTrans);
                    SqlDataAdapter daSupInPre = new SqlDataAdapter(cmd3);
                    DataTable dtSupInPre = new DataTable();
                    daSupInPre.Fill(dtSupInPre);

                    myTrans.Commit();//Transaction is cometted here

                    DialogResult reply = MessageBox.Show("Setting has Saved Successfully", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtInvoiceNo.Text = "";
                    txtInvoicePre.Text = "";
                    txtInvoiceNo.Enabled = false;
                    txtInvoicePre.Enabled = false;
                    btnInvoiceSet.Enabled = false;
                    btnSet.Enabled = false;
                }
                catch
                {
                    myTrans.Rollback();
                    MessageBox.Show("Error occured while Transfering, whole prosess is Rollback", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                finally
                {
                    myConnection.Close();
                }
            }
            else
            {
                MessageBox.Show("You must Enter Prifix Or number");
            }
            //frmDefaultSettings_Load(sender, e);
            InvoiceLoad();
        }

        private void btnCusREturn_Click(object sender, EventArgs e)
        {
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlCommand myCommand = new SqlCommand();
            SqlTransaction myTrans;
            myConnection.Open();
            myCommand.Connection = myConnection;

            myTrans = myConnection.BeginTransaction();
            myCommand.Transaction = myTrans;//start the trasaction scope
            if (txtCusReNo.Text != "" && txtCusRePre.Text != "")
            {
                try
                {
                    //+++++++++++++++++++++ CUSTOMER RETUN ++++++++++++++++++7
                    myCommand.CommandText = "UPDATE tblDefualtSetting with (rowlock) SET CusReturnNo = '" + txtCusReNo.Text.ToString().Trim() + "' select CusReturnNo from tblDefualtSetting  with (rowlock)";
                    SqlDataAdapter daCusRe = new SqlDataAdapter(myCommand.CommandText, ConnectionString);
                    DataTable dtCusRe = new DataTable();
                    daCusRe.Fill(dtCusRe);

                    if (dtCusRe.Rows.Count <= 0)
                    {
                        myCommand.CommandText = "Insert into tblDefualtSetting (CusReturnNo) values ('" + txtCusReNo.Text.ToString().Trim() + "')";
                        myCommand.ExecuteNonQuery();
                    }
                    //+++++++++++++ CUSTOMER RETURN++++++++7
                    myCommand.CommandText = "UPDATE tblDefualtSetting with (rowlock) SET CusReturnPrefix = '" + txtCusRePre.Text.ToString().Trim() + "' select CusReturnPrefix from tblDefualtSetting  with (rowlock)";
                    SqlDataAdapter daCusRePre = new SqlDataAdapter(myCommand.CommandText, ConnectionString);
                    DataTable dtCusRePre = new DataTable();
                    daCusRePre.Fill(dtCusRePre);

                    if (dtCusRePre.Rows.Count <= 0)
                    {
                        myCommand.CommandText = "Insert into tblDefualtSetting (CusReturnPrefix) values ('" + txtCusRePre.Text.ToString().Trim() + "')";
                        myCommand.ExecuteNonQuery();
                    }

                    SqlCommand cmd3 = new SqlCommand("UPDATE tblDefaultSettingCodeGen set IsAuto='" + chkGRNAuto.Checked + "' where TransType='CRTN'", myConnection, myTrans);
                    SqlDataAdapter daSupInPre = new SqlDataAdapter(cmd3);
                    DataTable dtSupInPre = new DataTable();
                    daSupInPre.Fill(dtSupInPre);

                    myTrans.Commit();//Transaction is cometted here

                    DialogResult reply = MessageBox.Show("Setting has Saved Successfully", "", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    txtCusReNo.Text = "";
                    txtCusRePre.Text = "";

                    txtCusReNo.Enabled = false;
                    txtCusRePre.Enabled = false;
                    btnCusREturn.Enabled = false;
                    btnSet.Enabled = false;

                }
                catch
                {
                    myTrans.Rollback();
                    MessageBox.Show("Error occured while Transfering, whole prosess is Rollback", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                finally
                {
                    myConnection.Close();
                }
            }
            else
            {
                MessageBox.Show("Fill The Fieldes");
            }
            CustomerReturnLoad();
           // frmDefaultSettings_Load(sender, e);
        }

        private void btnTransNoteSet_Click(object sender, EventArgs e)
        {
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlCommand myCommand = new SqlCommand();
            SqlTransaction myTrans;
            myConnection.Open();
            myCommand.Connection = myConnection;

            myTrans = myConnection.BeginTransaction();
            myCommand.Transaction = myTrans;//start the trasaction scope
            if (txtTrnsNoteNo.Text != "" && txtTransNotePre.Text != "")
            {
                try
                {
                    //++++++++++++++++ TRANSFER NOTE +++++++++++4
                    myCommand.CommandText = "UPDATE tblDefualtSetting with (rowlock) SET TransNoteNo = '" + txtTrnsNoteNo.Text.ToString().Trim() + "' select TransNoteNo from tblDefualtSetting  with (rowlock)";
                    SqlDataAdapter daTransNote = new SqlDataAdapter(myCommand.CommandText, ConnectionString);
                    DataTable dtTransNote = new DataTable();
                    daTransNote.Fill(dtTransNote);

                    if (dtTransNote.Rows.Count <= 0)
                    {
                        myCommand.CommandText = "Insert into tblDefualtSetting (TransNoteNo) values ('" + txtTrnsNoteNo.Text.ToString().Trim() + "')";
                        myCommand.ExecuteNonQuery();
                    }
                    //++++++++++++++++ TRANSFER NOTE +++++++++4
                    myCommand.CommandText = "UPDATE tblDefualtSetting with (rowlock) SET TransNotePrefix = '" + txtTransNotePre.Text.ToString().Trim() + "' select TransNotePrefix from tblDefualtSetting  with (rowlock)";
                    SqlDataAdapter daTransNotePre = new SqlDataAdapter(myCommand.CommandText, ConnectionString);
                    DataTable dtTransNotePre = new DataTable();
                    daTransNotePre.Fill(dtTransNotePre);

                    SqlCommand cmd3 = new SqlCommand("UPDATE tblDefaultSettingCodeGen set IsAuto='" + chkGRNAuto.Checked + "' where TransType='TN'", myConnection, myTrans);
                    SqlDataAdapter daSupInPre = new SqlDataAdapter(cmd3);
                    DataTable dtSupInPre = new DataTable();
                    daSupInPre.Fill(dtSupInPre);

                    if (dtTransNotePre.Rows.Count <= 0)
                    {
                        myCommand.CommandText = "Insert into tblDefualtSetting (TransNotePrefix) values ('" + txtTransNotePre.Text.ToString().Trim() + "')";
                        myCommand.ExecuteNonQuery();
                    }

                    myTrans.Commit();//Transaction is cometted here

                    DialogResult reply = MessageBox.Show("Setting has Saved Successfully", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtTransNotePre.Text = "";
                    txtTrnsNoteNo.Text = "";
                    txtTransNotePre.Enabled = false;
                    txtTrnsNoteNo.Enabled = false;
                    btnSet.Enabled = false;

                }
                catch
                {
                    myTrans.Rollback();
                    MessageBox.Show("Error occured while Transfering, whole prosess is Rollback", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                finally
                {
                    myConnection.Close();
                }
            }
            else
            {
                MessageBox.Show("You must Enter Prifix Or number");
            }
           // frmDefaultSettings_Load(sender, e);
            TransferDataLoad();
        }

        private void btnIssueNoteSet_Click(object sender, EventArgs e)
        {
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlCommand myCommand = new SqlCommand();
            SqlTransaction myTrans;
            myConnection.Open();
            myCommand.Connection = myConnection;

            myTrans = myConnection.BeginTransaction();
            myCommand.Transaction = myTrans;//start the trasaction scope
            if (txtIsueNoteNo.Text != "" && txtIsueNotePre.Text != "")
            {
                try
                {
                    //++++++++++++++++ ISSUE NOTE ++++++++++5
                    myCommand.CommandText = "UPDATE tblDefualtSetting with (rowlock) SET IssueNoteNo = '" + txtIsueNoteNo.Text.ToString().Trim() + "' select IssueNoteNo from tblDefualtSetting  with (rowlock)";
                    SqlDataAdapter daIssueNote = new SqlDataAdapter(myCommand.CommandText, ConnectionString);
                    DataTable dtIssueNote = new DataTable();
                    daIssueNote.Fill(dtIssueNote);

                    if (dtIssueNote.Rows.Count <= 0)
                    {
                        myCommand.CommandText = "Insert into tblDefualtSetting (IssueNoteNo) values ('" + txtIsueNoteNo.Text.ToString().Trim() + "')";
                        myCommand.ExecuteNonQuery();
                    }

                    //++++++++++++++++ ISSUE NOTE+++++++++++++++++5
                    myCommand.CommandText = "UPDATE tblDefualtSetting with (rowlock) SET IssuNotePrefix = '" + txtIsueNotePre.Text.ToString().Trim() + "' select IssuNotePrefix from tblDefualtSetting  with (rowlock)";
                    SqlDataAdapter daIssueNotePre = new SqlDataAdapter(myCommand.CommandText, ConnectionString);
                    DataTable dtIssueNotePre = new DataTable();
                    daIssueNotePre.Fill(dtIssueNotePre);

                    SqlCommand cmd3 = new SqlCommand("UPDATE tblDefaultSettingCodeGen set IsAuto='" + chkGRNAuto.Checked + "' where TransType='ISN'", myConnection, myTrans);
                    SqlDataAdapter daSupInPre = new SqlDataAdapter(cmd3);
                    DataTable dtSupInPre = new DataTable();
                    daSupInPre.Fill(dtSupInPre);

                    if (dtIssueNotePre.Rows.Count <= 0)
                    {
                        myCommand.CommandText = "Insert into tblDefualtSetting (IssuNotePrefix) values ('" + txtIsueNotePre.Text.ToString().Trim() + "')";
                        myCommand.ExecuteNonQuery();
                    }
                    myTrans.Commit();//Transaction is cometted here

                    DialogResult reply = MessageBox.Show("Setting has Saved Successfully", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtIsueNoteNo.Text = "";
                    txtIsueNotePre.Text = "";
                    txtIsueNoteNo.Enabled = false;
                    txtIsueNotePre.Enabled = false;
                    btnSet.Enabled = false;
                }
                catch
                {
                    myTrans.Rollback();
                    MessageBox.Show("Error occured while Transfering, whole prosess is Rollback", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                finally
                {
                    myConnection.Close();
                }
            }
            else
            {
                MessageBox.Show("You must Enter Prifix Or number");
            }
           // frmDefaultSettings_Load(sender, e);
            IssueNoteLoad();
        }

        private void txtSupInNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar);
        }

        private void txtSupReNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar);
        }

        private void txtDelNoteNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar);
        }

        private void txtInvoiceNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar);
        }

        private void txtCusReNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar);
        }

        private void txtTrnsNoteNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar);
        }

        private void txtIsueNoteNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar);
        }

        private void txtSupInPre_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar);
        }

        private void txtSupRePre_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar);
        }

        private void txtDelNotePre_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar);
        }

        private void txtInvoicePre_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar);
        }

        private void txtCusRePrefix_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar);
        }

        private void txtTransNotePre_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar);
        }

        private void txtIsueNotePre_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar);
        }

        private void txtSupInNo_Leave(object sender, EventArgs e)
        {
            String SSupIn = "select SupplierInvoiceNo from tblDefualtSetting";
            SqlDataAdapter daSupIn = new SqlDataAdapter(SSupIn, ConnectionString);
            DataTable dtSupIn = new DataTable();
            daSupIn.Fill(dtSupIn);
            if (dtSupIn.Rows.Count > 0 && dtSupIn.Rows[0].ItemArray[0].ToString().Trim() != "")
            {
                int txtNo;
                int dtNo;
                dtNo = Convert.ToInt32(dtSupIn.Rows[0].ItemArray[0].ToString().Trim());
                if (txtSupInNo.Text.ToString().Trim() != "")
                {
                    txtNo = Convert.ToInt32(txtSupInNo.Text.ToString().Trim());

                    if (txtNo < dtNo)
                    {
                        MessageBox.Show("Please Enter upper value than this", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtSupInNo.Text = "";
                    }
                }
            }
            else
            {
            }
        }

        private void txtSupReNo_Leave(object sender, EventArgs e)
        {
            String SSupRe = "select SupplierReturnNo from tblDefualtSetting";
            SqlDataAdapter daSupRe = new SqlDataAdapter(SSupRe, ConnectionString);
            DataTable dtSupRe = new DataTable();
            daSupRe.Fill(dtSupRe);

            if (dtSupRe.Rows.Count > 0 && dtSupRe.Rows[0].ItemArray[0].ToString().Trim() != "")
            {
                int txtNo;
                int dtNo;
                dtNo = Convert.ToInt32(dtSupRe.Rows[0].ItemArray[0].ToString().Trim());
                if (txtSupReNo.Text.ToString().Trim() != "")
                {
                    txtNo = Convert.ToInt32(txtSupReNo.Text.ToString().Trim());

                    if (txtNo < dtNo)
                    {
                        MessageBox.Show("Please Enter upper value than this", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtSupReNo.Text = "";
                    }
                }
            }
            else
            {
            }
        }

        private void txtDelNoteNo_Leave(object sender, EventArgs e)
        {
            String SDelNote = "select DeliveryNoteNo from tblDefualtSetting";
            SqlDataAdapter daDelNote = new SqlDataAdapter(SDelNote, ConnectionString);
            DataTable dtDelNote = new DataTable();
            daDelNote.Fill(dtDelNote);

            if (dtDelNote.Rows.Count > 0 && dtDelNote.Rows[0].ItemArray[0].ToString().Trim() != "")
            {
                int txtNo;
                int dtNo;
                dtNo = Convert.ToInt32(dtDelNote.Rows[0].ItemArray[0].ToString().Trim());
                if (txtDelNoteNo.Text.ToString().Trim() != "")
                {
                    txtNo = Convert.ToInt32(txtDelNoteNo.Text.ToString().Trim());

                    if (txtNo < dtNo)
                    {
                        MessageBox.Show("Please Enter upper value than this", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtDelNoteNo.Text = "";
                    }
                }
            }
            else
            {
            }
        }

        private void txtInvoiceNo_Leave(object sender, EventArgs e)
        {
            String SInvoice = "select InvoiceNo from tblDefualtSetting";
            SqlDataAdapter daInvoice = new SqlDataAdapter(SInvoice, ConnectionString);
            DataTable dtInvoice = new DataTable();
            daInvoice.Fill(dtInvoice);

            if (dtInvoice.Rows.Count > 0 && dtInvoice.Rows[0].ItemArray[0].ToString().Trim() != "")
            {
                int txtNo;
                int dtNo;
                dtNo = Convert.ToInt32(dtInvoice.Rows[0].ItemArray[0].ToString().Trim());
                if (txtInvoiceNo.Text.ToString().Trim() != "")
                {
                    txtNo = Convert.ToInt32(txtInvoiceNo.Text.ToString().Trim());

                    if (txtNo < dtNo)
                    {
                        MessageBox.Show("Please Enter upper value than this", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtInvoiceNo.Text = "";
                    }
                }
            }
            else
            {
            }
        }

        private void txtCusReNumber_Leave(object sender, EventArgs e)
        {

            String SCusRe = "select CusReturnNo from tblDefualtSetting";
            SqlDataAdapter daCusRe = new SqlDataAdapter(SCusRe, ConnectionString);
            DataTable dtCusRe = new DataTable();
            daCusRe.Fill(dtCusRe);

            if (dtCusRe.Rows.Count > 0 && dtCusRe.Rows[0].ItemArray[0].ToString().Trim() != "")
            {
                int txtNo;
                int dtNo;
                dtNo = Convert.ToInt32(dtCusRe.Rows[0].ItemArray[0].ToString().Trim());
                if (txtCusReNo.Text.ToString().Trim() != "")
                {
                    txtNo = Convert.ToInt32(txtCusReNo.Text.ToString().Trim());

                    if (txtNo < dtNo)
                    {
                        MessageBox.Show("Please Enter upper value than this", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtCusReNo.Text = "";
                    }
                }
            }
            else
            {
            }

        }

        private void txtTrnsNoteNo_Leave(object sender, EventArgs e)
        {
            //String STransNote = "select TransNoteNo from tblDefualtSetting";
            //SqlDataAdapter daTransNote = new SqlDataAdapter(STransNote, ConnectionString);
            //DataTable dtTransNote = new DataTable();
            //daTransNote.Fill(dtTransNote);

            //if (dtTransNote.Rows.Count > 0 && dtTransNote.Rows[0].ItemArray[0].ToString().Trim() != "")
            //{
            //    int txtNo;
            //    int dtNo;
            //    dtNo = Convert.ToInt32(dtTransNote.Rows[0].ItemArray[0].ToString().Trim());
            //    txtNo = Convert.ToInt32(txtTrnsNoteNo.Text.ToString().Trim());
            //    if (txtNo < dtNo)
            //    {
            //        MessageBox.Show("Please Enter upper value than this", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        txtTrnsNoteNo.Text = "";
            //    }

            //}
            //else
            //{
            //}
        }

        private void txtIsueNoteNo_Leave(object sender, EventArgs e)
        {
            String SIssueNote = "select IssueNoteNo from tblDefualtSetting";
            SqlDataAdapter daIssueNote = new SqlDataAdapter(SIssueNote, ConnectionString);
            DataTable dtIssueNote = new DataTable();
            daIssueNote.Fill(dtIssueNote);

            if (dtIssueNote.Rows.Count > 0 && dtIssueNote.Rows[0].ItemArray[0].ToString().Trim() != "")
            {
                int txtNo;
                int dtNo;
                dtNo = Convert.ToInt32(dtIssueNote.Rows[0].ItemArray[0].ToString().Trim());
                txtNo = Convert.ToInt32(txtIsueNoteNo.Text.ToString().Trim());
                if (txtNo < dtNo)
                {
                    MessageBox.Show("Please Enter upper value than this", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtIsueNoteNo.Text = "";
                }

            }
            else
            {
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        //+++++++++++++ default number settings +++++++++++++++++++++++++++++
        //public void loadChartofAcount()
        //{
        //    try
        //    {
        //        String S = "Select * from tblChartofAcounts";
        //        SqlCommand cmd = new SqlCommand(S);
        //        SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
        //        DataSet dt = new DataSet();
        //        da.Fill(dt);

        //        for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
        //        {
        //            cmbArAccount.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
        //            cmbAPAccount.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
        //            // cmbTax1ID.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
        //            // cmbTax2ID.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
        //            //cmbDiscountID.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
        //        }
        //    }
        //    catch { }

        //}
        //=================================================
        //public void LoadItemID()
        //{
        //    try
        //    {
        //        String S = "Select ItemID from tblItemMaster where ItemClass =7";
        //        SqlCommand cmd = new SqlCommand(S);
        //        SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
        //        DataSet dt = new DataSet();
        //        da.Fill(dt);

        //        for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
        //        {
        //            cmbTax1ID.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
        //            //cmbTax2ID.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
        //            comboBox1.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
        //            cmbDiscountID.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
        //        }
        //    }
        //    catch { }

        //}
        //============================================

        //public void LoadDefualtDetails()
        //{
        //    try
        //    {
        //        String S = "Select * from tblDefualtSetting";
        //        SqlCommand cmd = new SqlCommand(S);
        //        SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
        //        DataSet dt = new DataSet();
        //        da.Fill(dt);

        //        for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
        //        {

        //            lblArAccount.Text = dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim();
        //            lblApAccount.Text = dt.Tables[0].Rows[i].ItemArray[1].ToString().Trim();

        //            lblTax1ID.Text = dt.Tables[0].Rows[i].ItemArray[2].ToString().Trim();
        //            lblTax1Name.Text = dt.Tables[0].Rows[i].ItemArray[3].ToString().Trim();
        //            lblTax1GL.Text = dt.Tables[0].Rows[i].ItemArray[4].ToString().Trim();

        //            label33.Text = dt.Tables[0].Rows[i].ItemArray[5].ToString().Trim();
        //            label30.Text = dt.Tables[0].Rows[i].ItemArray[6].ToString().Trim();
        //            label29.Text = dt.Tables[0].Rows[i].ItemArray[7].ToString().Trim();

        //            lblDiscountID.Text = dt.Tables[0].Rows[i].ItemArray[8].ToString().Trim();
        //            label17.Text = dt.Tables[0].Rows[i].ItemArray[9].ToString().Trim();
        //            label14.Text = dt.Tables[0].Rows[i].ItemArray[10].ToString().Trim();

        //        }
        //    }
        //    catch { }

        //}

        //private void cmbArAccount_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        String S = "Select ItemDescription,SalesGLAccount from tblItemMaster where ItemID ='" + cmbTax1ID.Text.ToString().Trim() + "'";
        //        SqlCommand cmd = new SqlCommand(S);
        //        SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
        //        DataSet dt = new DataSet();
        //        da.Fill(dt);

        //        for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
        //        {
        //            txtTax1Name.Text = dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim();
        //            txtTax1GL.Text = dt.Tables[0].Rows[i].ItemArray[1].ToString().Trim();
        //        }
        //    }
        //    catch { }
        //}

        //private void cmbAPAccount_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        String S = "Select ItemDescription,SalesGLAccount from tblItemMaster where ItemID ='" + cmbTax1ID.Text.ToString().Trim() + "'";
        //        SqlCommand cmd = new SqlCommand(S);
        //        SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
        //        DataSet dt = new DataSet();
        //        da.Fill(dt);

        //        for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
        //        {
        //            txtTax1Name.Text = dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim();
        //            txtTax1GL.Text = dt.Tables[0].Rows[i].ItemArray[1].ToString().Trim();
        //        }
        //    }
        //    catch { }
        //}

        //private void cmbTax1ID_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        String S = "Select ItemDescription,SalesGLAccount from tblItemMaster where ItemID ='" + cmbTax1ID.Text.ToString().Trim() + "'";
        //        SqlCommand cmd = new SqlCommand(S);
        //        SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
        //        DataSet dt = new DataSet();
        //        da.Fill(dt);

        //        for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
        //        {
        //            txtTax1Name.Text = dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim();
        //            txtTax1GL.Text = dt.Tables[0].Rows[i].ItemArray[1].ToString().Trim();
        //        }
        //    }
        //    catch { }
        //}

        //private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        String S = "Select ItemDescription,SalesGLAccount from tblItemMaster where ItemID ='" + comboBox1.Text.ToString().Trim() + "'";
        //        SqlCommand cmd = new SqlCommand(S);
        //        SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
        //        DataSet dt = new DataSet();
        //        da.Fill(dt);

        //        for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
        //        {
        //            txtTax2Name.Text = dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim();
        //            txtTax2GL.Text = dt.Tables[0].Rows[i].ItemArray[1].ToString().Trim();
        //        }
        //    }
        //    catch { }
        //}

        //private void cmbDiscountID_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        String S = "Select ItemDescription,SalesGLAccount from tblItemMaster where ItemID ='" + cmbDiscountID.Text.ToString().Trim() + "'";
        //        SqlCommand cmd = new SqlCommand(S);
        //        SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
        //        DataSet dt = new DataSet();
        //        da.Fill(dt);

        //        for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
        //        {
        //            textBox1.Text = dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim();
        //            textBox5.Text = dt.Tables[0].Rows[i].ItemArray[1].ToString().Trim();
        //        }
        //    }
        //    catch { }
        //}

        //private void btnARSet_Click(object sender, EventArgs e)
        //{
        //    if (cmbArAccount.Text == "")
        //    {
        //        MessageBox.Show("Enter AR Account");
        //    }
        //    else
        //    {
        //        setConnectionString();
        //        SqlConnection myConnection = new SqlConnection(ConnectionString);
        //        //SqlCommand myCommand = new SqlCommand();
        //        myConnection.Open();
        //        SqlTransaction myTrans = myConnection.BeginTransaction(); ;

        //        try
        //        {
        //            SqlCommand cmd2 = new SqlCommand("UPDATE tblDefualtSetting  set ArAccount = '" + cmbArAccount.Text.ToString().Trim() + "' select ArAccount from tblDefualtSetting", myConnection, myTrans);
        //            SqlDataAdapter da41 = new SqlDataAdapter(cmd2);
        //            DataTable dt41 = new DataTable();
        //            da41.Fill(dt41);
        //            MessageBox.Show("AR Account Update Successfully");
        //            //String S = "Select ArAccount from tblDefualtSetting";
        //            //SqlCommand cmd = new SqlCommand(S);
        //            //SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
        //            //DataSet dt = new DataSet();
        //            //da.Fill(dt);
        //            //if (dt.Tables[0].Rows.Count > 0)
        //            //{
        //            //    String S1 = "Update tblDefualtSetting SET ArAccount = '" + cmbArAccount.Text.ToString().Trim() + "'";
        //            //    SqlCommand cmd1 = new SqlCommand(S);
        //            //    SqlConnection con = new SqlConnection(ConnectionString);
        //            //    SqlDataAdapter da1 = new SqlDataAdapter(S1, con);
        //            //    DataTable dt1 = new DataTable();
        //            //    da.Fill(dt1);
        //            //    MessageBox.Show("AR Account Update Successfully");

        //            //}
        //            if (dt41.Rows.Count <= 0)
        //            {
        //                SqlCommand cmd5 = new SqlCommand("insert into tblDefualtSetting(ArAccount) values ('" + cmbArAccount.Text.ToString().Trim() + "')", myConnection, myTrans);
        //                cmd5.ExecuteNonQuery();
        //                MessageBox.Show("AR Account Insert Successfully");
        //            }
        //            //else
        //            //{
        //            //    String S2 = "insert into tblDefualtSetting(ArAccount) values ('" + cmbArAccount.Text.ToString().Trim() + "')";
        //            //    SqlCommand cmd2 = new SqlCommand(S2);
        //            //    SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
        //            //    DataSet ds2 = new DataSet();
        //            //    da2.Fill(ds2);
        //            //    MessageBox.Show("AR Account Insert Successfully");

        //            //}
        //            myTrans.Commit();//Transaction is cometted here
        //        }
        //        catch
        //        {
        //            myTrans.Rollback();
        //            MessageBox.Show("Error occured while Transfering, whole prosess is Rollback", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        }
        //        finally
        //        {
        //            myConnection.Close();
        //        }
        //    }
        //}

        //private void btnAPSet_Click(object sender, EventArgs e)
        //{
        //    ////=================================================
        //    if (cmbAPAccount.Text == "")
        //    {
        //        MessageBox.Show("Select AP Account");
        //    }
        //    //else
        //    //{
        //    //    try
        //    //    {
        //    //        String S = "Select APAccount from tblDefualtSetting";
        //    //        SqlCommand cmd = new SqlCommand(S);
        //    //        SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
        //    //        DataSet dt = new DataSet();
        //    //        da.Fill(dt);
        //    //        if (dt.Tables[0].Rows.Count > 0)
        //    //        {
        //    //            String S1 = "Update tblDefualtSetting SET APAccount = '" + cmbAPAccount.Text.ToString().Trim() + "'";
        //    //            SqlCommand cmd1 = new SqlCommand(S);
        //    //            SqlConnection con = new SqlConnection(ConnectionString);
        //    //            SqlDataAdapter da1 = new SqlDataAdapter(S1, con);
        //    //            DataTable dt1 = new DataTable();
        //    //            da.Fill(dt1);
        //    //            MessageBox.Show("AR Account Update Successfully");

        //    //        }
        //    //        else
        //    //        {
        //    //            String S2 = "insert into tblDefualtSetting(APAccount) values ('" + cmbAPAccount.Text.ToString().Trim() + "')";
        //    //            SqlCommand cmd2 = new SqlCommand(S2);
        //    //            SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
        //    //            DataSet ds2 = new DataSet();
        //    //            da2.Fill(ds2);
        //    //            MessageBox.Show("AR Account Insert Successfully");

        //    //        }

        //    //    }
        //    //    catch { }
        //    //}
        //    else
        //    {
        //        setConnectionString();
        //        SqlConnection myConnection = new SqlConnection(ConnectionString);
        //        //SqlCommand myCommand = new SqlCommand();
        //        myConnection.Open();
        //        SqlTransaction myTrans = myConnection.BeginTransaction(); ;

        //        try
        //        {
        //            SqlCommand cmd2 = new SqlCommand("UPDATE tblDefualtSetting  set APAccount = '" + cmbAPAccount.Text.ToString().Trim() + "' select APAccount from tblDefualtSetting", myConnection, myTrans);
        //            SqlDataAdapter da41 = new SqlDataAdapter(cmd2);
        //            DataTable dt41 = new DataTable();
        //            da41.Fill(dt41);
        //            MessageBox.Show("AR Account Update Successfully");

        //            if (dt41.Rows.Count <= 0)
        //            {
        //                SqlCommand cmd5 = new SqlCommand("insert into tblDefualtSetting(APAccount) values ('" + cmbAPAccount.Text.ToString().Trim() + "')", myConnection, myTrans);
        //                cmd5.ExecuteNonQuery();
        //                MessageBox.Show("AR Account Insert Successfully");
        //            }

        //            myTrans.Commit();//Transaction is cometted here
        //        }
        //        catch
        //        {
        //            myTrans.Rollback();
        //            MessageBox.Show("Error occured while Transfering, whole prosess is Rollback", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        }
        //        finally
        //        {
        //            myConnection.Close();
        //        }
        //    }


        //    //with the transaction scope.
        //}

        //private void btnTax1set_Click(object sender, EventArgs e)
        //{
        //    //===============================================
        //    if (cmbTax1ID.Text == "")
        //    {
        //        MessageBox.Show("Select Tax1 Account");
        //    }
        //    else
        //    {
        //        try
        //        {
        //            String S = "Select tax1ID from tblDefualtSetting";
        //            SqlCommand cmd = new SqlCommand(S);
        //            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
        //            DataSet dt = new DataSet();
        //            da.Fill(dt);
        //            if (dt.Tables[0].Rows.Count > 0)
        //            {
        //                String S1 = "Update tblDefualtSetting SET tax1ID = '" + cmbTax1ID.Text.ToString().Trim() + "', tax1Name = '" + txtTax1Name.Text.ToString().Trim() + "', tax1GL  = '" + txtTax1GL.Text.ToString().Trim() + "'";
        //                SqlCommand cmd1 = new SqlCommand(S1);
        //                SqlConnection con = new SqlConnection(ConnectionString);
        //                SqlDataAdapter da1 = new SqlDataAdapter(S1, con);
        //                DataTable dt1 = new DataTable();
        //                da1.Fill(dt1);
        //                MessageBox.Show("tax1ID update Successfully");

        //            }
        //            else
        //            {
        //                String S2 = "insert into tblDefualtSetting(tax1ID,tax1Name,tax1GL) values ('" + cmbTax1ID.Text.ToString().Trim() + "','" + txtTax1Name.Text.ToString().Trim() + "','" + txtTax1GL.Text.ToString().Trim() + "')";
        //                SqlCommand cmd2 = new SqlCommand(S2);
        //                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
        //                DataSet ds2 = new DataSet();
        //                da2.Fill(ds2);
        //                MessageBox.Show("tax1ID Insert Successfully");
        //            }

        //        }
        //        catch { }
        //    }
        //}

        //private void btntaxAc2_Click(object sender, EventArgs e)
        //{
        //    //=========================================================
        //    if (comboBox1.Text == "")
        //    {
        //        MessageBox.Show("Select Tax2 Account");
        //    }
        //    else
        //    {
        //        try
        //        {
        //            String S = "Select tax2ID from tblDefualtSetting";
        //            SqlCommand cmd = new SqlCommand(S);
        //            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
        //            DataSet dt = new DataSet();
        //            da.Fill(dt);
        //            if (dt.Tables[0].Rows.Count > 0)
        //            {
        //                String S1 = "Update tblDefualtSetting SET tax2ID = '" + comboBox1.Text.ToString().Trim() + "', tax2Name = '" + txtTax2Name.Text.ToString().Trim() + "', tax2GL  = '" + txtTax2GL.Text.ToString().Trim() + "'";
        //                SqlCommand cmd1 = new SqlCommand(S1);
        //                SqlConnection con = new SqlConnection(ConnectionString);
        //                SqlDataAdapter da1 = new SqlDataAdapter(S1, con);
        //                DataTable dt1 = new DataTable();
        //                da1.Fill(dt1);
        //                MessageBox.Show("Tax2 Account update Successfully");
        //            }
        //            else
        //            {
        //                String S2 = "insert into tblDefualtSetting(tax2ID,tax2Name,tax2GL) values ('" + cmbTax1ID.Text.ToString().Trim() + "','" + txtTax2Name.Text.ToString().Trim() + "','" + txtTax2GL.Text.ToString().Trim() + "')";
        //                SqlCommand cmd2 = new SqlCommand(S2);
        //                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
        //                DataSet ds2 = new DataSet();
        //                da2.Fill(ds2);
        //                MessageBox.Show("Tax2 Account Insert Successfully");

        //            }

        //        }
        //        catch { }
        //    }
        //}

        //private void btnDisAc_Click(object sender, EventArgs e)
        //{
        //    if (cmbDiscountID.Text == "")
        //    {
        //        MessageBox.Show("Select Discount Account");
        //    }
        //    else
        //    {
        //        try
        //        {
        //            String S = "Select DiscountID from tblDefualtSetting";
        //            SqlCommand cmd = new SqlCommand(S);
        //            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
        //            DataSet dt = new DataSet();
        //            da.Fill(dt);
        //            if (dt.Tables[0].Rows.Count > 0)
        //            {
        //                String S1 = "Update tblDefualtSetting SET DiscountID = '" + cmbDiscountID.Text.ToString().Trim() + "', DiscountName = '" + textBox1.Text.ToString().Trim() + "', DiscountGL  = '" + textBox5.Text.ToString().Trim() + "'";
        //                SqlCommand cmd1 = new SqlCommand(S1);
        //                SqlConnection con = new SqlConnection(ConnectionString);
        //                SqlDataAdapter da1 = new SqlDataAdapter(S1, con);
        //                DataTable dt1 = new DataTable();
        //                da1.Fill(dt1);
        //                MessageBox.Show("Discount Account update Successfully");
        //            }
        //            else
        //            {
        //                String S2 = "insert into tblDefualtSetting(DiscountID,DiscountName,DiscountGL) values ('" + cmbDiscountID.Text.ToString().Trim() + "','" + textBox1.Text.ToString().Trim() + "','" + textBox5.Text.ToString().Trim() + "')";
        //                SqlCommand cmd2 = new SqlCommand(S2);
        //                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
        //                DataSet ds2 = new DataSet();
        //                da2.Fill(ds2);
        //                MessageBox.Show("Discount Account Insert Successfully");

        //            }

        //        }
        //        catch { }
        //    }

        //}

        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAddTax_Click(object sender, EventArgs e)
        {
            //bool run = false;
            //try
            //{
            //    dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmTaxDetails");
            //    if (dtUser.Rows.Count > 0)
            //    {
            //        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
            //    }
            //    if (run)
            //    {
            frmTaxDetails taxd = new frmTaxDetails();
            taxd.Show();
            //    }
            //    else
            //    {
            //        MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }

            //}
            //catch { }
        }

        private void chkAllowMinusQty_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void tabTransType_Click(object sender, EventArgs e)
        {

        }

        private void btnDecimal_Click(object sender, EventArgs e)
        {
            try
            {
                frmsetDecimalPoint setd = new frmsetDecimalPoint();
                //setd.ShowDialog();
                setd.TopMost = true;
                setd.ShowDialog();
            }
            catch { }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            btnAddTaxData.Enabled = true;

            txtTaxCode.Text = "";
            txtTaxDescription.Text = "";
            txtTaxRate.Text = "";
            txttaxAccount.Text = "";
            txttaxRank.Text = "";
            txttaxRank.Visible = true;
            label61.Visible = true;


            //dgvtaxappy.Rows.Clear();
        }
        bool isTaxEdit = false;
        private void btnEditTax_Click(object sender, EventArgs e)
        {
            isTaxEdit = true;
            btnAddTaxData.Enabled = true;

            txtTaxDescription.Enabled = true;
            txtTaxRate.Enabled = true;
            txttaxRank.Enabled = true;
            txttaxAccount.Enabled = true;
            chbxIsActive.Enabled = true;
            rdobtnIndependentax.Enabled = true;
            rdobtnTaxonTax.Enabled = true;
        }

        private void btnAddTaxData_Click(object sender, EventArgs e)
        {
            bool isTaxActive = false;
            bool ISTaxonTax = false;
            if (chbxIsActive.Checked == true)
            {
                isTaxActive = true;
            }
            if (rdobtnTaxonTax.Checked == true)
            {
                ISTaxonTax = true;
            }

            int checkint = 0;
            try
            {
                Convert.ToDouble(txtTaxRate.Text);

            }
            catch { checkint = 1; }


            if (txtTaxCode.Text == "" || txtTaxDescription.Text == "" || txtTaxRate.Text == "" || txttaxAccount.Text == "" || checkint == 1 || txttaxRank.Text == "")
            {
                if (checkint == 1)
                {
                    MessageBox.Show("Tax Rate must be a number");
                    //btnAddTaxData.Focus();
                }
                else
                {
                    MessageBox.Show("you must fill all Detals");
                    //btnAddTaxData.Focus();
                }

            }
            else
            {
                try
                {
                    if (isTaxEdit == false)
                    {
                        string ConnString = ConnectionString;
                        String S2 = "insert into tblTaxApplicable(TaxID,TaxName,Rate,Rank,Account,IsActive,IsTaxOnTax) values ('" + txtTaxCode.Text.ToString().Trim() + "','" + txtTaxDescription.Text.ToString().Trim() + "','" + Convert.ToDouble(txtTaxRate.Text) + "','" + txttaxRank.Text.ToString().Trim() + "','" + txttaxAccount.Text.ToString().Trim() + "','" + isTaxActive + "','" + ISTaxonTax + "')";
                        SqlCommand cmd2 = new SqlCommand(S2);
                        SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnString);
                        DataSet ds2 = new DataSet();
                        da2.Fill(ds2);

                        MessageBox.Show("Saved Successfully","Save",MessageBoxButtons.OK,MessageBoxIcon.Information);
                        txtTaxCode.Text = "";
                        txtTaxDescription.Text = "";
                        txtTaxRate.Text = "";
                        txttaxRank.Text = "";
                        txttaxAccount.Text = "";
                        btnAddTaxData.Enabled = false;
                        //  btnSave.Enabled = false;
                        //btnNew.Focus();
                    }
                    else
                    {
                        String S = "Update tblTaxApplicable SET TaxName = '" + txtTaxDescription.Text.ToString().Trim() + "',Rank='" + txttaxRank.Text.ToString() + "',Rate ='" + Convert.ToDouble(txtTaxRate.Text) + "',Account = '" + txttaxAccount.Text.ToString().Trim() + "',IsActive='" + isTaxActive + "',IsTaxOnTax='" + ISTaxonTax + "' where TaxID = '" + txtTaxCode.Text.ToString().Trim() + "'";
                        SqlCommand cmd = new SqlCommand(S);
                        SqlConnection con = new SqlConnection(ConnectionString);
                        SqlDataAdapter da = new SqlDataAdapter(S, con);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        MessageBox.Show("Updated Successfully","Save",MessageBoxButtons.OK,MessageBoxIcon.Information);
                        txtTaxCode.Text = "";
                        txtTaxDescription.Text = "";
                        txtTaxRate.Text = "";
                        txttaxAccount.Text = "";
                        btnAddTaxData.Enabled = false;
                        cmbtaxCode.Focus();
                    }
                }
                catch { }
            }



        }

        private void rdobtnTaxonTax_CheckedChanged(object sender, EventArgs e)
        {
            //if (rdobtnTaxonTax.Checked == true)
            //{
            //    txttaxRank.Visible = true;
            //    label61.Visible = true;
            //}
            //else
            //{
            //    txttaxRank.Text = "0";
            //    txttaxRank.Visible = false;
            //    label61.Visible = false;
            //}
        }

        private void cmbtaxCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                String S = "Select * from tblTaxApplicable where TaxID = '" + cmbtaxCode.Text.ToString().Trim() + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                if (dt.Tables[0].Rows.Count > 0)
                {

                    txtTaxCode.Text = dt.Tables[0].Rows[0].ItemArray[0].ToString();
                    txtTaxDescription.Text = dt.Tables[0].Rows[0].ItemArray[1].ToString();
                    txtTaxRate.Text = dt.Tables[0].Rows[0].ItemArray[2].ToString();
                    txttaxRank.Text = dt.Tables[0].Rows[0].ItemArray[3].ToString();
                    txttaxAccount.Text = dt.Tables[0].Rows[0].ItemArray[4].ToString();
                    if (Convert.ToBoolean(dt.Tables[0].Rows[0].ItemArray[5]) == true)
                    {
                        chbxIsActive.Checked = true;
                    }
                    else
                    {
                        chbxIsActive.Checked = false;
                    }

                    if (Convert.ToBoolean(dt.Tables[0].Rows[0].ItemArray[6]) == true)
                    {
                        rdobtnTaxonTax.Checked = true;
                    }
                    else
                    {
                        rdobtnIndependentax.Checked = true;
                    }
                }
            }
            catch { }
            //  btnNew.Enabled = false;
            btnEdit.Enabled = true;
            btnSave.Enabled = false;
            btnAddTaxData.Enabled = false;
            //   btnaddd
            // btnDelete.Enabled = true;
            //btnClose.Enabled = true;
        }

        private void btnGeneralSave_Click(object sender, EventArgs e)
        {
            //for chk minus qty satus
            if (chkAllowMinusQty.Checked == true)
            {
                allowMinusQty = true;
            }
            else
            {
                allowMinusQty = false;
            }
            // fro chk multi whse status
            if (chkAllowMultiWhse.Checked == true)
            {
                allowMWhse = true;
            }
            else
            {
                allowMWhse = false;
            }
            //chking iver grn satus
            if (chkOGRN.Checked == true)
            {
                allowOverGRN = true;
            }
            else
            {
                allowOverGRN = false;
            }
            if (chkTax.Checked == true)
            {
                tax = true;
            }
            else
            {
                tax = false;
            }
            if (chkTaxOnTax.Checked == true)
            {
                taxOnTax = true;
            }
            else
            {
                taxOnTax = false;
            }
            if (chkOvrSO.Checked == true)
            {
                overSo = true;
            }
            else
            {
                overSo = false;
            }
            //chkin g minus qty 
            String SMinusQty = "select IsMinusAllow from tblDefualtSetting";
            SqlDataAdapter daMinusQty = new SqlDataAdapter(SMinusQty, ConnectionString);
            DataTable dtMinusQty = new DataTable();
            daMinusQty.Fill(dtMinusQty);
            setConnectionString();
            //SqlConnection myConnection = new SqlConnection(ConnectionString);
            ////SqlCommand myCommand = new SqlCommand();
            //myConnection.Open();
            //SqlTransaction myTrans = myConnection.BeginTransaction();

            //select APAccount from tblDefualtSetting", myConnection, myTrans);


            if (dtMinusQty.Rows.Count > 0)
            {
                String S3 = "update tblDefualtSetting set IsMinusAllow = '" + allowMinusQty.ToString().Trim() + "'";
                SqlCommand cmd3 = new SqlCommand(S3);
                SqlConnection con3 = new SqlConnection(ConnectionString);
                SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
                DataTable dt3 = new DataTable();
                da3.Fill(dt3);
            }
            else
            {
                String S1 = "Insert into tblDefualtSetting (IsMinusAllow) values ('" + allowMinusQty.ToString().Trim() + "')";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
            }

            //ching over grn
            String SOverGRN = "select OverGRN from tblDefualtSetting";
            SqlDataAdapter daOverGRN = new SqlDataAdapter(SOverGRN, ConnectionString);
            DataTable dtOverGRN = new DataTable();
            daOverGRN.Fill(dtOverGRN);

            if (dtOverGRN.Rows.Count > 0)
            {
                String S3 = "update tblDefualtSetting set OverGRN = '" + allowOverGRN.ToString().Trim() + "'";
                SqlCommand cmd3 = new SqlCommand(S3);
                SqlConnection con3 = new SqlConnection(ConnectionString);
                SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
                DataTable dt3 = new DataTable();
                da3.Fill(dt3);
            }
            else
            {
                String S1 = "Insert into tblDefualtSetting (OverGRN) values ('" + allowOverGRN.ToString().Trim() + "')";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
            }
            //chking over tax applicable
            String STax = "select IsTaxApplicable from tblDefualtSetting";
            SqlDataAdapter daTax = new SqlDataAdapter(STax, ConnectionString);
            DataTable dtTax = new DataTable();
            daTax.Fill(dtTax);

            if (dtTax.Rows.Count > 0)
            {
                String S31 = "update tblDefualtSetting set IsTaxApplicable = '" + tax.ToString().Trim() + "'";
                SqlCommand cmd31 = new SqlCommand(S31);
                SqlConnection con31 = new SqlConnection(ConnectionString);
                SqlDataAdapter da31 = new SqlDataAdapter(S31, con31);
                DataTable dt31 = new DataTable();
                da31.Fill(dt31);
            }
            else
            {
                String S11 = "Insert into tblDefualtSetting (IsTaxApplicable) values ('" + tax.ToString().Trim() + "')";
                SqlCommand cmd11 = new SqlCommand(S11);
                SqlDataAdapter da11 = new SqlDataAdapter(S11, ConnectionString);
                DataTable dt11 = new DataTable();
                da11.Fill(dt11);
            }


            //chking over tax on tax applicable
            String STaxOn = "select IsTaxOnTax from tblDefualtSetting";
            SqlDataAdapter daTaxOn = new SqlDataAdapter(STaxOn, ConnectionString);
            DataTable dtTaxOn = new DataTable();
            daTaxOn.Fill(dtTaxOn);

            if (dtTaxOn.Rows.Count > 0)
            {
                String S32 = "update tblDefualtSetting set IsTaxOnTax = '" + taxOnTax.ToString().Trim() + "'";
                SqlCommand cmd32 = new SqlCommand(S32);
                SqlConnection con32 = new SqlConnection(ConnectionString);
                SqlDataAdapter da32 = new SqlDataAdapter(S32, con32);
                DataTable dt32 = new DataTable();
                da32.Fill(dt32);
            }
            else
            {
                String S12 = "Insert into tblDefualtSetting (IsTaxOnTax) values ('" + taxOnTax.ToString().Trim() + "')";
                SqlCommand cmd12 = new SqlCommand(S12);
                SqlDataAdapter da12 = new SqlDataAdapter(S12, ConnectionString);
                DataTable dt12 = new DataTable();
                da12.Fill(dt12);
            }

            //chking over tax on tax applicable
            String SSo = "select OverSO from tblDefualtSetting";
            SqlDataAdapter daSo = new SqlDataAdapter(SSo, ConnectionString);
            DataTable dtSo = new DataTable();
            daSo.Fill(dtSo);

            if (dtSo.Rows.Count > 0)
            {
                String S10 = "update tblDefualtSetting set OverSO = '" + overSo.ToString().Trim() + "'";
                SqlCommand cmd10 = new SqlCommand(S10);
                SqlConnection con10 = new SqlConnection(ConnectionString);
                SqlDataAdapter da10 = new SqlDataAdapter(S10, con10);
                DataTable dt10 = new DataTable();
                da10.Fill(dt10);
            }
            else
            {
                String S22 = "Insert into tblDefualtSetting (OverSO) values ('" + overSo.ToString().Trim() + "')";
                SqlCommand cmd22 = new SqlCommand(S22);
                SqlDataAdapter da22 = new SqlDataAdapter(S22, ConnectionString);
                DataTable dt22 = new DataTable();
                da22.Fill(dt22);
            }

            MessageBox.Show("Master Settings Saved Successfully ", "", MessageBoxButtons.OK, MessageBoxIcon.Information);

            chkAllowMinusQty.Enabled = false;
            chkAllowMultiWhse.Enabled = false;
            chkOGRN.Enabled = false;
            btnSave.Enabled = false;
            chkTax.Enabled = false;
            chkTaxOnTax.Enabled = false;
            chkOvrSO.Enabled = false;
            // this.Close();
        }

        private void btnGeneralEdit_Click(object sender, EventArgs e)
        {
            bool canAdd = false;
            dtUser = DataAccess.Access.setUserAuthentication(UserAutherization.user.userName, "frmMasterSettings");
            if (dtUser.Rows.Count > 0)
            {
                canAdd = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
            }
            if (canAdd)
            {
                chkAllowMinusQty.Enabled = true;
                chkAllowMultiWhse.Enabled = true;
                chkOGRN.Enabled = true;
                btnSave.Enabled = true;
                chkTax.Enabled = true;
                chkTaxOnTax.Enabled = true;
                chkOvrSO.Enabled = true;
            }
            else
            {
                MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbtaxCode_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            try
            {
                String S = "Select * from tblTaxApplicable where TaxID = '" + cmbtaxCode.Text.ToString().Trim() + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                if (dt.Tables[0].Rows.Count > 0)
                {

                    txtTaxCode.Text = dt.Tables[0].Rows[0].ItemArray[0].ToString();
                    txtTaxDescription.Text = dt.Tables[0].Rows[0].ItemArray[1].ToString();
                    txtTaxRate.Text = dt.Tables[0].Rows[0].ItemArray[2].ToString();
                    txttaxRank.Text = dt.Tables[0].Rows[0].ItemArray[3].ToString();
                    txttaxAccount.Text = dt.Tables[0].Rows[0].ItemArray[4].ToString();
                    if (Convert.ToBoolean(dt.Tables[0].Rows[0].ItemArray[5]) == true)
                    {
                        chbxIsActive.Checked = true;
                    }
                    else
                    {
                        chbxIsActive.Checked = false;
                    }

                    if (Convert.ToBoolean(dt.Tables[0].Rows[0].ItemArray[6]) == true)
                    {
                        rdobtnTaxonTax.Checked = true;
                    }
                    else
                    {
                        rdobtnIndependentax.Checked = true;
                    }
                }
            }
            catch { }
            //  btnNew.Enabled = false;
            txtTaxCode.Enabled = false;
            btnEditTax.Enabled = true;
            // btnSave.Enabled = false;
            btnAddTaxData.Enabled = false;

            txtTaxDescription.Enabled = false;
            txtTaxRate.Enabled = false;
            txttaxRank.Enabled = false;
            txttaxAccount.Enabled = false;
            chbxIsActive.Enabled = false;
            rdobtnIndependentax.Enabled = false;
            rdobtnTaxonTax.Enabled = false;
        }

        private void rdobtnIndependentax_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnComSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (isEditCompany == 0)
                {
                    string ConnString = ConnectionString;
                    String S2 = "insert into tblCompanyInformation(CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email,StateEmployer,FedEmployer,StateUnemployer,FormOfBussiness,Directory) values ('" + txtCompanyName.Text.ToString().Trim() + "','" + txtAddress1.Text.ToString().Trim() + "','" + txtAddress2.Text.ToString().Trim() + "','" + txtCity.Text.ToString().Trim() + "','" + cmbState.Text.ToString().Trim() + "','" + txtZip.Text.ToString().Trim() + "','" + txtCountry.Text.ToString().Trim() + "','" + txtTelephone.Text.ToString().Trim() + "','" + txtFax.Text.ToString().Trim() + "','" + txtWebSite.Text.ToString().Trim() + "','" + txtEmail.Text.ToString().Trim() + "','" + txtStateEmployer.Text.ToString().Trim() + "','" + txtFedEmployer.Text.ToString().Trim() + "','" + txtStateUnemployer.Text.ToString().Trim() + "','" + cmbFormOfBussiness.Text.ToString().Trim() + "','" + txtDirectory.Text.ToString().Trim() + "')";
                    SqlCommand cmd2 = new SqlCommand(S2);
                    SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnString);
                    DataSet ds2 = new DataSet();
                    da2.Fill(ds2);
                    MessageBox.Show("Saved Successfully","Save",MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
                else if (isEditCompany == 1)
                {
                    string ConnString = ConnectionString;
                    String S2 = "update tblCompanyInformation set CompanyName = '" + txtCompanyName.Text.ToString().Trim() + "',Address1 = '" + txtAddress1.Text.ToString().Trim() + "',Address2 = '" + txtAddress2.Text.ToString().Trim() + "',City = '" + txtCity.Text.ToString().Trim() + "',State = '" + cmbState.Text.ToString().Trim() + "',Zip = '" + txtZip.Text.ToString().Trim() + "',Country = '" + txtCountry.Text.ToString().Trim() + "',Telephone = '" + txtTelephone.Text.ToString().Trim() + "',Fax = '" + txtFax.Text.ToString().Trim() + "',WebSite = '" + txtWebSite.Text.ToString().Trim() + "',Email = '" + txtEmail.Text.ToString().Trim() + "',StateEmployer = '" + txtStateEmployer.Text.ToString().Trim() + "',FedEmployer = '" + txtFedEmployer.Text.ToString().Trim() + "',StateUnemployer = '" + txtStateUnemployer.Text.ToString().Trim() + "',FormOfBussiness = '" + cmbFormOfBussiness.Text.ToString().Trim() + "',Directory = '" + txtDirectory.Text.ToString().Trim() + "'";
                    SqlCommand cmd2 = new SqlCommand(S2);
                    SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnString);
                    DataSet ds2 = new DataSet();
                    da2.Fill(ds2);
                    MessageBox.Show("Updated Successfully", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                txtCompanyName.ReadOnly = true;
                txtAddress1.ReadOnly = true;
                txtAddress2.ReadOnly = true;
                txtCity.ReadOnly = true;
                cmbState.Enabled = false;
                txtZip.ReadOnly = true;
                txtCountry.ReadOnly = true;
                txtTelephone.ReadOnly = true;
                txtFax.ReadOnly = true;
                txtWebSite.ReadOnly = true;
                txtEmail.ReadOnly = true;
                txtStateEmployer.ReadOnly = true;
                txtFedEmployer.ReadOnly = true;
                txtStateUnemployer.ReadOnly = true;
                cmbFormOfBussiness.Enabled = false;

                btnComEdit.Enabled = true;
                btnComSave.Enabled = false;
            }
            catch { }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                folderBrowserDialog1.ShowDialog();
                txtDirectory.Text = folderBrowserDialog1.SelectedPath;
                folderBrowserDialog1.Dispose();
            }
            catch { }
        }

        private void btnComEdit_Click(object sender, EventArgs e)
        {
            isEditCompany = 1;
            txtCompanyName.ReadOnly = false;
            txtAddress1.ReadOnly = false;
            txtAddress2.ReadOnly = false;
            txtCity.ReadOnly = false;
            cmbState.Enabled = true;
            txtZip.ReadOnly = false;
            txtCountry.ReadOnly = false;
            txtTelephone.ReadOnly = false;
            txtFax.ReadOnly = false;
            txtWebSite.ReadOnly = false;
            txtEmail.ReadOnly = false;
            txtStateEmployer.ReadOnly = false;
            txtFedEmployer.ReadOnly = false;
            txtStateUnemployer.ReadOnly = false;
            cmbFormOfBussiness.Enabled = true;
            txtCompanyName.Focus();
            btnBrowse.Enabled = true;
            btnComEdit.Enabled = false;
            btnComSave.Enabled = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //setConnectionString();
            //SqlConnection myConnection = new SqlConnection(ConnectionString);
            //SqlCommand myCommand = new SqlCommand();
            //SqlTransaction myTrans;
            //myConnection.Open();
            //myCommand.Connection = myConnection;

            //myTrans = myConnection.BeginTransaction();
            //myCommand.Transaction = myTrans;//start the trasaction scope
            //connection fro local database
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            //SqlConnection myConnection = new SqlConnection(ConnectionString);
            myConnection.Open();
            SqlTransaction myTrans = myConnection.BeginTransaction();

            if (txtGrnPrefix.Text != "" && txtGrnNo.Text != "")
            {
                try
                {
                    //++++++++++++++++++++++++++ THIS PART IS FOR SETTING NUMBERS+++++++++++++++++++1
                    //++++++++++++ SUPPLER INVOICE++++++++1
                    SqlCommand cmd1 = new SqlCommand("UPDATE tblDefualtSetting SET GRNNo = '" + txtGrnPrefix.Text.ToString().Trim() + "' select GRNNo from tblDefualtSetting", myConnection, myTrans);
                    SqlDataAdapter daSupIn = new SqlDataAdapter(cmd1);
                    DataTable dtSupIn = new DataTable();
                    daSupIn.Fill(dtSupIn);
                    if (dtSupIn.Rows.Count <= 0)
                    {
                        SqlCommand cmd2 = new SqlCommand("Insert into tblDefualtSetting (GRNNo) values ('" + txtGrnPrefix.Text.ToString().Trim() + "')", myConnection, myTrans);
                        cmd2.ExecuteNonQuery();
                    }

                    SqlCommand cmd3 = new SqlCommand("UPDATE tblDefualtSetting SET GRNPrefix = '" + txtGrnNo.Text.ToString().Trim() + "' select GRNPrefix from tblDefualtSetting", myConnection, myTrans);
                    SqlDataAdapter daSupInPre = new SqlDataAdapter(cmd3);
                    DataTable dtSupInPre = new DataTable();
                    daSupInPre.Fill(dtSupInPre);

                    if (dtSupInPre.Rows.Count <= 0)
                    {
                        SqlCommand cmd4 = new SqlCommand("Insert into tblDefualtSetting (GRNPrefix) values ('" + txtGrnNo.Text.ToString().Trim() + "')", myConnection, myTrans);
                        cmd3.ExecuteNonQuery();
                    }

                    cmd3 = new SqlCommand("UPDATE tblDefaultSettingCodeGen set IsAuto='"+chkGRNAuto.Checked+"' where TransType='GRN'", myConnection, myTrans);
                    daSupInPre = new SqlDataAdapter(cmd3);
                    dtSupInPre = new DataTable();
                    daSupInPre.Fill(dtSupInPre);

                    myTrans.Commit();//Transaction is cometted here

                    DialogResult reply = MessageBox.Show("Setting has Saved Successfully", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtGrnNo.Enabled = false;
                    txtGrnPrefix.Enabled = false;
                    txtGrnPrefix.Text = "";
                    txtGrnNo.Text = "";
                    button5.Enabled = false;
                }
                catch
                {
                    myTrans.Rollback();
                    MessageBox.Show("Error occured while Transfering, whole prosess is Rollback", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                finally
                {
                    myConnection.Close();
                }
            }
            else
            {
                MessageBox.Show("You must Enter Prifix Or number");
            }
           // frmDefaultSettings_Load(sender, e);
            GRNDataload();
        }

        private void txtGrnNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar);
        }

        private void txtGrnNo_Leave(object sender, EventArgs e)
        {

        }

        private void txtGrnPrefix_Leave(object sender, EventArgs e)
        {
            String SSupIn = "select GRNNo from tblDefualtSetting";
            SqlDataAdapter daSupIn = new SqlDataAdapter(SSupIn, ConnectionString);
            DataTable dtSupIn = new DataTable();
            daSupIn.Fill(dtSupIn);
            if (dtSupIn.Rows.Count > 0 && dtSupIn.Rows[0].ItemArray[0].ToString().Trim() != "")
            {
                int txtNo;
                int dtNo;
                dtNo = Convert.ToInt32(dtSupIn.Rows[0].ItemArray[0].ToString().Trim());
                if (txtGrnPrefix.Text.ToString().Trim() != "")
                {
                    txtNo = Convert.ToInt32(txtGrnPrefix.Text.ToString().Trim());

                    if (txtNo < dtNo)
                    {
                        MessageBox.Show("Please Enter upper value than this", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtGrnPrefix.Text = "";
                    }
                }
            }
            else
            {
            }
        }

        private void txtGrnPrefix_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar);
        }

        private void btnEditInv_Click(object sender, EventArgs e)
        {
            bool canAdd = false;
            dtUser = DataAccess.Access.setUserAuthentication(UserAutherization.user.userName, "frmMasterSettings");
            if (dtUser.Rows.Count > 0)
            {
                canAdd = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
            }
            if (canAdd)
            {

                txtIsueNoteNo.Enabled = true;
                txtIsueNotePre.Enabled = true;
                txtTransNotePre.Enabled = true;
                txtTrnsNoteNo.Enabled = true;


                txtIsueNoteNo.Text = "";
                txtIsueNotePre.Text = "";

                txtTransNotePre.Text = "";
                txtTrnsNoteNo.Text = "";
                btnTransNoteSet.Enabled = true;
                btnIssueNoteSet.Enabled = true;

                btnSet.Enabled = true;

                String STransNote = "select TransNoteNo,TransNotePrefix from tblDefualtSetting";
                SqlDataAdapter daTransNote = new SqlDataAdapter(STransNote, ConnectionString);
                DataTable dtTransNote = new DataTable();
                daTransNote.Fill(dtTransNote);

                if (dtTransNote.Rows.Count > 0)
                {
                    lblTransNoteNo.Text = dtTransNote.Rows[0].ItemArray[0].ToString().Trim();
                    lblTransNotePre.Text = dtTransNote.Rows[0].ItemArray[1].ToString().Trim();
                }

                String SIssueNote = "select IssueNoteNo,IssuNotePrefix from tblDefualtSetting";
                SqlDataAdapter daIssueNote = new SqlDataAdapter(SIssueNote, ConnectionString);
                DataTable dtIssueNote = new DataTable();
                daIssueNote.Fill(dtIssueNote);

                if (dtIssueNote.Rows.Count > 0)
                {
                    lblIssueNoteNo.Text = dtIssueNote.Rows[0].ItemArray[0].ToString().Trim();
                    lblIssueNotePre.Text = dtIssueNote.Rows[0].ItemArray[1].ToString().Trim();
                }

            }
            else
            {
                MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEditSales_Click(object sender, EventArgs e)
        {
            bool canAdd = false;
            dtUser = DataAccess.Access.setUserAuthentication(UserAutherization.user.userName, "frmMasterSettings");
            if (dtUser.Rows.Count > 0)
            {
                canAdd = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
            }
            if (canAdd)
            {
                txtCusReNo.Enabled = true;
                txtCusRePre.Enabled = true;
                txtDelNoteNo.Enabled = true;
                txtDelNotePre.Enabled = true;
                txtInvoiceNo.Enabled = true;
                txtInvoicePre.Enabled = true;

                mltDeliveryDr.Enabled = true;
                mltDeliveryCr.Enabled = true;
                mltCustomerCr.Enabled = true;
                mltCustomerDr.Enabled = true;


                txtCusReNo.Text = "";
                txtCusRePre.Text = "";
                txtDelNoteNo.Text = "";
                txtDelNotePre.Text = "";
                txtInvoiceNo.Text = "";
                txtInvoicePre.Text = "";
                txtIsueNoteNo.Text = "";
                txtIsueNotePre.Text = "";

                btnSet.Enabled = true;
                btnInvoiceSet.Enabled = true;
                btnDelNoteSet.Enabled = true;
                btnCusREturn.Enabled = true;
                btnDelNoteAcSet.Enabled = true;
                btnCusretnAcSet.Enabled = true;
                cmbDelNoteCrAc.Enabled = true;
                cmbCusretnDrAc.Enabled = true;
                mltDelDr.Enabled = true;
                cmbCusretnCrAc.Enabled = true;

                String SDelNote = "select DeliveryNoteNo,DeliveryNotePrefix from tblDefualtSetting";
                SqlDataAdapter daDelNote = new SqlDataAdapter(SDelNote, ConnectionString);
                DataTable dtDelNote = new DataTable();
                daDelNote.Fill(dtDelNote);

                if (dtDelNote.Rows.Count > 0)
                {
                    lblDelNoteNo.Text = dtDelNote.Rows[0].ItemArray[0].ToString().Trim();
                    lblDelNotePre.Text = dtDelNote.Rows[0].ItemArray[1].ToString().Trim();
                }


                String SInvoice = "select InvoiceNo,InvoicePrefix from tblDefualtSetting";
                SqlDataAdapter daInvoice = new SqlDataAdapter(SInvoice, ConnectionString);
                DataTable dtInvoice = new DataTable();
                daInvoice.Fill(dtInvoice);

                if (dtInvoice.Rows.Count > 0)
                {
                    lblInvoiceNo.Text = dtInvoice.Rows[0].ItemArray[0].ToString().Trim();
                    lblInvoicePre.Text = dtInvoice.Rows[0].ItemArray[1].ToString().Trim();
                }

                String SCusRe = "select CusReturnNo,CusReturnPrefix from tblDefualtSetting";
                SqlDataAdapter daCusRe = new SqlDataAdapter(SCusRe, ConnectionString);
                DataTable dtCusRe = new DataTable();
                daCusRe.Fill(dtCusRe);

                if (dtCusRe.Rows.Count > 0)
                {
                    lblCusReNo.Text = dtCusRe.Rows[0].ItemArray[0].ToString().Trim();
                    lblCusRePre.Text = dtCusRe.Rows[0].ItemArray[1].ToString().Trim();
                }
                String SInvoice1 = "select DelNoteDrAc,DelNoteCrAc from tblDefualtSetting";
                SqlDataAdapter daInvoice1 = new SqlDataAdapter(SInvoice1, ConnectionString);
                DataTable dtInvoice1 = new DataTable();
                daInvoice1.Fill(dtInvoice1);

                if (dtInvoice1.Rows.Count > 0)
                {
                    lblDelNoteDrAc1.Text = dtInvoice1.Rows[0].ItemArray[0].ToString().Trim();
                    lblDelNoteCrAc1.Text = dtInvoice1.Rows[0].ItemArray[1].ToString().Trim();
                }

                String SCusRe1 = "select CusretnDrAc,CusretnCrAc from tblDefualtSetting";
                SqlDataAdapter daCusRe1 = new SqlDataAdapter(SCusRe1, ConnectionString);
                DataTable dtCusRe1 = new DataTable();
                daCusRe1.Fill(dtCusRe1);

                if (dtCusRe1.Rows.Count > 0)
                {
                    lblCusretnDrAc1.Text = dtCusRe1.Rows[0].ItemArray[0].ToString().Trim();
                    lblCusretnCrAc1.Text = dtCusRe1.Rows[0].ItemArray[1].ToString().Trim();
                }

            }
            else
            {
                MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEditPurchase_Click(object sender, EventArgs e)
        {
            bool canAdd = false;
            dtUser = DataAccess.Access.setUserAuthentication(UserAutherization.user.userName, "frmMasterSettings");
            if (dtUser.Rows.Count > 0)
            {
                canAdd = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
            }
            if (canAdd)
            {
                mltDrAccount.Enabled = true;
                mltCrAccount.Enabled = true;
                mltDrSup.Enabled = true;
                mltCrSup.Enabled = true;


                txtSupInNo.Enabled = true;
                txtSupInPre.Enabled = true;
                txtSupReNo.Enabled = true;
                txtSupRePre.Enabled = true;
                cmbSupInCr.Enabled = true;
                cmbSupInDr.Enabled = true;
                cmbGrnCr.Enabled = true;
                cmbGrnDr.Enabled = true;

                txtSupInNo.Text = "";
                txtSupInPre.Text = "";
                txtSupReNo.Text = "";
                txtSupRePre.Text = "";

                btnSupInAcSet.Enabled = true;
                btnSupInSet.Enabled = true;
                btnSupReturnSet.Enabled = true;
                btnGrnAcSet.Enabled = true;
                button5.Enabled = true;
                btnCusretnAcSet.Enabled = false;
                btnDelNoteAcSet.Enabled = false;
                txtGrnNo.Enabled = true;
                txtGrnPrefix.Enabled = true;
                txtGrnPrefix.Text = "";
                txtGrnNo.Text = "";


                btnSet.Enabled = true;
                String SGRN = "select GRNNo,GRNPrefix from tblDefualtSetting";
                SqlDataAdapter daGRN = new SqlDataAdapter(SGRN, ConnectionString);
                DataTable dtGRN = new DataTable();
                daGRN.Fill(dtGRN);
                if (dtGRN.Rows.Count > 0)
                {
                    lblGRNNo.Text = dtGRN.Rows[0].ItemArray[0].ToString().Trim();
                    lblGrnPfx.Text = dtGRN.Rows[0].ItemArray[1].ToString().Trim();
                }

                String SSupIn = "select SupplierInvoiceNo,SupInvoicePrefix from tblDefualtSetting";
                SqlDataAdapter daSupIn = new SqlDataAdapter(SSupIn, ConnectionString);
                DataTable dtSupIn = new DataTable();
                daSupIn.Fill(dtSupIn);
                if (dtSupIn.Rows.Count > 0)
                {
                    lblSupInNo.Text = dtSupIn.Rows[0].ItemArray[0].ToString().Trim();
                    lblSupInPre.Text = dtSupIn.Rows[0].ItemArray[1].ToString().Trim();
                }

                String SSupRe = "select SupplierReturnNo,SupReturnPrefix from tblDefualtSetting";
                SqlDataAdapter daSupRe = new SqlDataAdapter(SSupRe, ConnectionString);
                DataTable dtSupRe = new DataTable();
                daSupRe.Fill(dtSupRe);

                if (dtSupRe.Rows.Count > 0)
                {
                    lblSupRe.Text = dtSupRe.Rows[0].ItemArray[0].ToString().Trim();
                    lblSupRePre.Text = dtSupRe.Rows[0].ItemArray[1].ToString().Trim();
                }

                String SGRNDrAc = "select GRNDrAc,GRNCrAc from tblDefualtSetting";
                SqlDataAdapter daGRNDrAc = new SqlDataAdapter(SGRNDrAc, ConnectionString);
                DataTable dtGRNDrAc = new DataTable();
                daGRNDrAc.Fill(dtGRNDrAc);

                if (dtGRNDrAc.Rows.Count > 0)
                {
                    lblGrnDrAc.Text = dtGRNDrAc.Rows[0].ItemArray[0].ToString().Trim();
                    lblGrnCrAc.Text = dtGRNDrAc.Rows[0].ItemArray[1].ToString().Trim();
                }

                String SSalesInvDrAc = "select SalesInvDrAc,SalesInvCrAc from tblDefualtSetting";
                SqlDataAdapter daSalesInvDrAc = new SqlDataAdapter(SSalesInvDrAc, ConnectionString);
                DataTable dtSalesInvDrAc = new DataTable();
                daSalesInvDrAc.Fill(dtSalesInvDrAc);

                if (dtSalesInvDrAc.Rows.Count > 0)
                {
                    lblSupInDrAc.Text = dtSalesInvDrAc.Rows[0].ItemArray[0].ToString().Trim();
                    lblSupInCrAc.Text = dtSalesInvDrAc.Rows[0].ItemArray[1].ToString().Trim();
                }
                String SDelNoteDrAc = "select DelNoteDrAc,DelNoteCrAc from tblDefualtSetting";
                SqlDataAdapter daDelNoteDrAc = new SqlDataAdapter(SDelNoteDrAc, ConnectionString);
                DataTable dtDelNoteDrAc = new DataTable();
                daDelNoteDrAc.Fill(dtDelNoteDrAc);

                if (dtDelNoteDrAc.Rows.Count > 0)
                {
                    lblDelNoteDrAc1.Text = dtDelNoteDrAc.Rows[0].ItemArray[0].ToString().Trim();
                    lblDelNoteCrAc1.Text = dtDelNoteDrAc.Rows[0].ItemArray[1].ToString().Trim();
                }

                String SCusretnDrAc = "select CusretnDrAc,CusretnCrAc from tblDefualtSetting";
                SqlDataAdapter daCusretnDrAc = new SqlDataAdapter(SCusretnDrAc, ConnectionString);
                DataTable dtCusretnDrAc = new DataTable();
                daCusretnDrAc.Fill(dtCusretnDrAc);

                if (dtCusretnDrAc.Rows.Count > 0)
                {
                    lblCusretnDrAc.Text = dtCusretnDrAc.Rows[0].ItemArray[0].ToString().Trim();
                    lblCusretnCrAc.Text = dtCusretnDrAc.Rows[0].ItemArray[1].ToString().Trim();
                }
            }
            else
            {
                MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtTrnsNoteNo_Leave_1(object sender, EventArgs e)
        {
            String STransNote = "select TransNoteNo from tblDefualtSetting";
            SqlDataAdapter daTransNote = new SqlDataAdapter(STransNote, ConnectionString);
            DataTable dtTransNote = new DataTable();
            daTransNote.Fill(dtTransNote);

            if (dtTransNote.Rows.Count > 0 && dtTransNote.Rows[0].ItemArray[0].ToString().Trim() != "")
            {
                int txtNo;
                int dtNo;
                dtNo = Convert.ToInt32(dtTransNote.Rows[0].ItemArray[0].ToString().Trim());
                if (txtTrnsNoteNo.Text.ToString().Trim() != "")
                {
                    txtNo = Convert.ToInt32(txtTrnsNoteNo.Text.ToString().Trim());
                    if (txtNo < dtNo)
                    {
                        MessageBox.Show("Please Enter upper value than this", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtTrnsNoteNo.Text = "";
                    }
                }
            }
            else
            {
            }
        }

        private void btnGrnAcSet_Click(object sender, EventArgs e)
        {
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            //SqlConnection myConnection = new SqlConnection(ConnectionString);
            myConnection.Open();
            SqlTransaction myTrans = myConnection.BeginTransaction();
            if (mltDrAccount.Text != "")
            {
                try
                {
                    //++++++++++++++++++++++++++ THIS PART IS FOR SETTING NUMBERS+++++++++++++++++++1
                    //++++++++++++ SUPPLER INVOICE++++++++1
                    SqlCommand cmd1 = new SqlCommand("UPDATE tblDefualtSetting SET GRNDrAc = '" + mltDrAccount.Text.ToString().Trim() + "' select GRNDrAc from tblDefualtSetting", myConnection, myTrans);
                    SqlDataAdapter daSupIn = new SqlDataAdapter(cmd1);
                    DataTable dtSupIn = new DataTable();
                    daSupIn.Fill(dtSupIn);
                    if (dtSupIn.Rows.Count <= 0)
                    {
                        SqlCommand cmd2 = new SqlCommand("Insert into tblDefualtSetting (GRNDrAc) values ('" + mltDrAccount.Text.ToString().Trim() + "')", myConnection, myTrans);
                        cmd2.ExecuteNonQuery();
                    }

                    SqlCommand cmd3 = new SqlCommand("UPDATE tblDefualtSetting SET GRNCrAc = '" + mltCrAccount.Text.ToString().Trim() + "' select GRNCrAc from tblDefualtSetting", myConnection, myTrans);
                    SqlDataAdapter daSupInPre = new SqlDataAdapter(cmd3);
                    DataTable dtSupInPre = new DataTable();
                    daSupInPre.Fill(dtSupInPre);

                    if (dtSupInPre.Rows.Count <= 0)
                    {
                        SqlCommand cmd4 = new SqlCommand("Insert into tblDefualtSetting (GRNCrAc) values ('" + mltCrAccount.Text.ToString().Trim() + "')", myConnection, myTrans);
                        cmd3.ExecuteNonQuery();
                    }
                    myTrans.Commit();//Transaction is cometted here

                    DialogResult reply = MessageBox.Show("Setting has Saved Successfully", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    mltDrAccount.Enabled = false;
                    mltCrAccount.Enabled = false;
                    mltDrAccount.Text = "";
                    mltCrAccount.Text = "";
                    btnGrnAcSet.Enabled = false;
                    //btnSupInAcSet.Enabled = true;
                }
                catch
                {
                    myTrans.Rollback();
                    MessageBox.Show("Error occured while Transfering, whole prosess is Rollback", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                finally
                {
                    myConnection.Close();
                }
            }
            else
            {
                MessageBox.Show("Select a Valid Account");
            }
        }

        private void btnSupInAcSet_Click(object sender, EventArgs e)
        {
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            //SqlConnection myConnection = new SqlConnection(ConnectionString);
            myConnection.Open();
            SqlTransaction myTrans = myConnection.BeginTransaction();
            if (mltDrSup.Text != "")
            {
                try
                {
                    //++++++++++++++++++++++++++ THIS PART IS FOR SETTING NUMBERS+++++++++++++++++++1
                    //++++++++++++ SUPPLER INVOICE++++++++1
                    SqlCommand cmd1 = new SqlCommand("UPDATE tblDefualtSetting SET SalesInvDrAc = '" + mltDrSup.Text.ToString().Trim() + "' select SalesInvDrAc from tblDefualtSetting", myConnection, myTrans);
                    SqlDataAdapter daSupIn = new SqlDataAdapter(cmd1);
                    DataTable dtSupIn = new DataTable();
                    daSupIn.Fill(dtSupIn);
                    if (dtSupIn.Rows.Count <= 0)
                    {
                        SqlCommand cmd2 = new SqlCommand("Insert into tblDefualtSetting (SalesInvDrAc) values ('" + mltDrSup.Text.ToString().Trim() + "')", myConnection, myTrans);
                        cmd2.ExecuteNonQuery();
                    }

                    SqlCommand cmd3 = new SqlCommand("UPDATE tblDefualtSetting SET SalesInvCrAc = '" + mltCrSup.Text.ToString().Trim() + "' select GRNCrAc from tblDefualtSetting", myConnection, myTrans);
                    SqlDataAdapter daSupInPre = new SqlDataAdapter(cmd3);
                    DataTable dtSupInPre = new DataTable();
                    daSupInPre.Fill(dtSupInPre);

                    if (dtSupInPre.Rows.Count <= 0)
                    {
                        SqlCommand cmd4 = new SqlCommand("Insert into tblDefualtSetting (SalesInvCrAc) values ('" + mltCrSup.Text.ToString().Trim() + "')", myConnection, myTrans);
                        cmd3.ExecuteNonQuery();
                    }
                    myTrans.Commit();//Transaction is cometted here

                    DialogResult reply = MessageBox.Show("Setting has Saved Successfully", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    mltDrSup.Enabled = false;
                    mltCrSup.Enabled = false;
                    mltDrSup.Text = "";
                    mltCrSup.Text = "";
                    btnSupInAcSet.Enabled = false;
                }
                catch
                {
                    myTrans.Rollback();
                    MessageBox.Show("Error occured while Transfering, whole prosess is Rollback", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                finally
                {
                    myConnection.Close();
                }
            }
            else
            {
                MessageBox.Show("Select a Valid Account");
            }
            InvoiceAccountLoad();
          //  frmDefaultSettings_Load(sender, e);
        }

        private void btnDelNoteAcSet_Click(object sender, EventArgs e)
        {
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            //SqlConnection myConnection = new SqlConnection(ConnectionString);
            myConnection.Open();
            SqlTransaction myTrans = myConnection.BeginTransaction();
            if (mltDeliveryCr.Text != "")
            {
                try
                {
                    //++++++++++++++++++++++++++ THIS PART IS FOR SETTING NUMBERS+++++++++++++++++++1
                    //++++++++++++ SUPPLER INVOICE++++++++1
                    SqlCommand cmd1 = new SqlCommand("UPDATE tblDefualtSetting SET DelNoteDrAc = '" + mltDeliveryDr.Text.ToString().Trim() + "' select DelNoteDrAc from tblDefualtSetting", myConnection, myTrans);
                    SqlDataAdapter daSupIn = new SqlDataAdapter(cmd1);
                    DataTable dtSupIn = new DataTable();
                    daSupIn.Fill(dtSupIn);
                    if (dtSupIn.Rows.Count <= 0)
                    {
                        SqlCommand cmd2 = new SqlCommand("Insert into tblDefualtSetting (DelNoteDrAc) values ('" + mltDeliveryDr.Text.ToString().Trim() + "')", myConnection, myTrans);
                        cmd2.ExecuteNonQuery();
                    }

                    SqlCommand cmd3 = new SqlCommand("UPDATE tblDefualtSetting SET DelNoteCrAc = '" + mltDeliveryCr.Text.ToString().Trim() + "' select DelNoteDrAc from tblDefualtSetting", myConnection, myTrans);
                    SqlDataAdapter daSupInPre = new SqlDataAdapter(cmd3);
                    DataTable dtSupInPre = new DataTable();
                    daSupInPre.Fill(dtSupInPre);

                    if (dtSupInPre.Rows.Count <= 0)
                    {
                        SqlCommand cmd4 = new SqlCommand("Insert into tblDefualtSetting (DelNoteCrAc) values ('" + mltDeliveryCr.Text.ToString().Trim() + "')", myConnection, myTrans);
                        cmd3.ExecuteNonQuery();
                    }
                    myTrans.Commit();//Transaction is cometted here

                    DialogResult reply = MessageBox.Show("Setting has Saved Successfully", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    mltDelDr.Enabled = false;
                    cmbDelNoteCrAc.Enabled = false;
                    mltDelDr.Text = "";
                    cmbDelNoteCrAc.Text = "";
                    btnDelNoteAcSet.Enabled = false;
                }
                catch
                {
                    myTrans.Rollback();
                    MessageBox.Show("Error occured while Transfering, whole prosess is Rollback", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                finally
                {
                    myConnection.Close();
                }
            }
            else
            {
                MessageBox.Show("Select a Valid Account");
            }
        }

        private void btnCusretnAcSet_Click(object sender, EventArgs e)
        {
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            //SqlConnection myConnection = new SqlConnection(ConnectionString);
            myConnection.Open();
            SqlTransaction myTrans = myConnection.BeginTransaction();
            if (mltCustomerCr.Text != "")
            {
                try
                {
                    //++++++++++++++++++++++++++ THIS PART IS FOR SETTING NUMBERS+++++++++++++++++++1
                    //++++++++++++ SUPPLER INVOICE++++++++1
                    double OptYN = 0;
                    if (radioadyes.Checked  == true)
                    {
                        OptYN = 1;
                    }
                    else if (radioadno.Checked == true)
                    {
                        OptYN = 0;
                    }
                    SqlCommand cmd1 = new SqlCommand("UPDATE tblDefualtSetting SET CusretnDrAc = '" + mltCustomerCr.Text.ToString().Trim() + "',SalesWithDiliveryOrder='" + OptYN.ToString() + "' select CusretnDrAc,SalesWithDiliveryOrder from tblDefualtSetting", myConnection, myTrans);
                    SqlDataAdapter daSupIn = new SqlDataAdapter(cmd1);
                    DataTable dtSupIn = new DataTable();
                    daSupIn.Fill(dtSupIn);
                    if (dtSupIn.Rows.Count <= 0)
                    {
                        SqlCommand cmd2 = new SqlCommand("Insert into tblDefualtSetting (CusretnDrAc,SalesWithDiliveryOrder) values ('" + mltCustomerCr.Text.ToString().Trim() + "','" + OptYN.ToString() + "')", myConnection, myTrans);
                        cmd2.ExecuteNonQuery();
                    }

                    SqlCommand cmd3 = new SqlCommand("UPDATE tblDefualtSetting SET CusretnCrAc = '" + mltCustomerDr.Text.ToString().Trim() + "' select CusretnDrAc from tblDefualtSetting", myConnection, myTrans);
                    SqlDataAdapter daSupInPre = new SqlDataAdapter(cmd3);
                    DataTable dtSupInPre = new DataTable();
                    daSupInPre.Fill(dtSupInPre);

                    if (dtSupInPre.Rows.Count <= 0)
                    {
                        SqlCommand cmd4 = new SqlCommand("Insert into tblDefualtSetting (CusretnCrAc) values ('" + mltCustomerDr.Text.ToString().Trim() + "')", myConnection, myTrans);
                        cmd3.ExecuteNonQuery();
                    }
                    myTrans.Commit();//Transaction is cometted here

                    DialogResult reply = MessageBox.Show("Setting has Saved Successfully", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    mltCustomerDr.Enabled = false;
                    mltCustomerCr.Enabled = false;
                    cmbCusretnCrAc.Text = "";
                    mltCustomerDr.Text = "";
                    mltCustomerCr.Enabled = false;
                }
                catch
                {
                    myTrans.Rollback();
                    MessageBox.Show("Error occured while Transfering, whole prosess is Rollback", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                finally
                {
                    myConnection.Close();
                }
            }
            else
            {
                MessageBox.Show("Select a Valid Account");
            }
            InvoiceAccountLoad();
           // frmDefaultSettings_Load(sender, e);
        }

        private void cmbDelNoteDrAc_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtSupReNo_TextChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbGrnDr_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbSupInCr_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            AccountingPeriods acctpers = new AccountingPeriods();
            //listView1.Items.Clear();
            //listView1.Columns.Clear();
            //listView1.View = View.Details;
            //listView1.GridLines = true;

            //listView1.Columns.Add("Period", -2, HorizontalAlignment.Center);
            //listView1.Columns.Add("Begin Date", -2, HorizontalAlignment.Right);
            //listView1.Columns.Add("to", -2, HorizontalAlignment.Center);
            //listView1.Columns.Add("End Date", -2, HorizontalAlignment.Left);
            //listView1.Columns.Add("Period", -2, HorizontalAlignment.Center);
            //listView1.Columns.Add("Begin Date", -2, HorizontalAlignment.Right);
            //listView1.Columns.Add("to", -2, HorizontalAlignment.Center);
            //listView1.Columns.Add("End Date", -2, HorizontalAlignment.Left);
            //listView1.Columns.Add("Period", -2, HorizontalAlignment.Center);
            //listView1.Columns.Add("Begin Date", -2, HorizontalAlignment.Right);
            //listView1.Columns.Add("to", -2, HorizontalAlignment.Center);
            //listView1.Columns.Add("End Date", -2, HorizontalAlignment.Left);

            int pernum;

            DateTime perdate = new DateTime();

            SqlConnection myConnection = new SqlConnection(ConnectionString);
            //SqlConnection myConnection = new SqlConnection(ConnectionString);
            myConnection.Open();
            SqlTransaction myTrans = myConnection.BeginTransaction();

            SqlCommand cmd2 = new SqlCommand(" delete from dbo.tblPeachtreeAccountPeriods ", myConnection, myTrans);
            cmd2.ExecuteNonQuery();
            int index = 0;

            try
            {
                for (int i = 1; i < acctpers.StartDate.Length; i++)
                {       
            
                    cmd2 = new SqlCommand(" insert into dbo.tblPeachtreeAccountPeriods(ID,[From],[To]) " +
                    " values(" + (index+4) + ",'" + DateTime.Parse(acctpers.StartDate[i].ToString()) + "','" + DateTime.Parse(acctpers.EndDate[i].ToString()) + "')", myConnection, myTrans);
                    cmd2.ExecuteNonQuery();
                    index = index + 1;

                    //listView1.Items[i - 1].SubItems.Add(pernum.ToString());
                }

                //cmd2 = new SqlCommand("exec SetAccountingPeriods '" + utxtNo.Text.Trim() + "','" + udtpFrom.Value + "','" + udtpTo.Value + "'", myConnection, myTrans);
                //    cmd2.ExecuteNonQuery();

//                ]
//@No int,
//@From datetime,
//@To datetime
                
                myTrans.Commit();//Transaction is cometted here                   
            }
            catch
            {
                myTrans.Rollback();
                MessageBox.Show("Error occured while Transfering, whole prosess is Rollback", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                myConnection.Close();
            }  

            

            //foreach (ColumnHeader col in listView1.Columns)
            //{
            //    col.Width = -2;
            //}

            acctpers = null;
        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }


        private void saveAccouintPeriod(int ID,DateTime From,DateTime To)
        {
                      
        }

        private void chkGRNAuto_CheckedChanged(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Delivery Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtSupInPre_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblSupInPre_Click(object sender, EventArgs e)
        {

        }

       





        //++++++++++++++++++++++++++++++++++++++++++++++++
    }
}