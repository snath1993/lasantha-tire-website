using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using DataAccess;

namespace UserAutherization
{

    public partial class frmTransNoteList : Form
    {
        public static string ConnectionString;
        //public WhuseTransfer wh = new WhuseTransfer();
        public dsAPCommon dsTNoteList = new dsAPCommon();
        

        public frmTransNoteList()
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
            catch { }
        }

        public DataSet dsWarehouse;
        public DataSet dsToWarehouse;
        public string StrSql;
        public void GetFromHouseDataSet()
        {
            dsWarehouse = new DataSet();
            try
            {
                dsWarehouse.Clear();
                StrSql = " SELECT WhseId, WhseName,ArAccount,CashAccount,SalesGLAccount FROM tblWhseMaster";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsWarehouse, "DtWarehouse");

                cmbWH.DataSource = dsWarehouse.Tables["DtWarehouse"];
                cmbWH.DisplayMember = "WhseId";
                cmbWH.ValueMember = "WhseId";
                cmbWH.DisplayLayout.Bands["DtWarehouse"].Columns["WhseId"].Width = 50;
                cmbWH.DisplayLayout.Bands["DtWarehouse"].Columns["WhseName"].Width = 150;
                cmbWH.DisplayLayout.Bands["DtWarehouse"].Columns["ArAccount"].Hidden = true;
                cmbWH.DisplayLayout.Bands["DtWarehouse"].Columns["CashAccount"].Hidden = true;
                cmbWH.DisplayLayout.Bands["DtWarehouse"].Columns["SalesGLAccount"].Hidden = true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetToWHouseDataSet()
        {
            dsToWarehouse = new DataSet();
            try
            {
                dsToWarehouse.Clear();
                StrSql = " SELECT WhseId, WhseName,ArAccount,CashAccount,SalesGLAccount FROM tblWhseMaster";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsToWarehouse, "DtWarehouse");

                cmbToWH.DataSource = dsToWarehouse.Tables["DtWarehouse"];
                cmbToWH.DisplayMember = "WhseId";
                cmbToWH.ValueMember = "WhseId";
                cmbToWH.DisplayLayout.Bands["DtWarehouse"].Columns["WhseId"].Width = 50;
                cmbToWH.DisplayLayout.Bands["DtWarehouse"].Columns["WhseName"].Width = 150;
                cmbToWH.DisplayLayout.Bands["DtWarehouse"].Columns["ArAccount"].Hidden = true;
                cmbToWH.DisplayLayout.Bands["DtWarehouse"].Columns["CashAccount"].Hidden = true;
                cmbToWH.DisplayLayout.Bands["DtWarehouse"].Columns["SalesGLAccount"].Hidden = true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void frmTransNoteList_Load(object sender, EventArgs e)
        {
            GetFromHouseDataSet();
            GetToWHouseDataSet();
        }

        private void txtSearchBy_TextChanged(object sender, EventArgs e)
        {
            //{
            ////if (txtSearchBy.Text != "")
            ////{
            ////    //Transfer Note
            ////    try
            ////    {
            ////        if (cmbSearchBy.Text.ToString().Trim() == "Transfer Note No")
            ////        {
            ////            string add = txtSearchBy.Text;
            ////            if (add != "")
            ////            {
            ////                //dgvTransferNoteList.Rows.Clear();
            ////                //String S = "Select tblWhseTransfer.WhseTransId,tblWhseTransfer.FrmWhseId,tblWhseTransfer.ToWhseId,tblWhseTransfer.TDate	 from tblWhseTransfer where WhseTransId LIKE  '" + add + "%'";
            ////                String S = "select * form tblWhseTransfer where WhseTransId LIKE  '" + add + "%'";
            ////                SqlCommand cmd = new SqlCommand(S);
            ////                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            ////                DataSet dt = new DataSet();
            ////                da.Fill(dt);

            ////                String S1 = "select * form tblWhseTransLine where WhseTransId LIKE  '" + add + "%'";
            ////                SqlCommand cmd1 = new SqlCommand(S1);
            ////                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
            ////                DataSet dt1 = new DataSet();
            ////                da1.Fill(dt1);

            ////                if (dt.Tables[0].Rows.Count > 0)
            ////                {
            ////                    for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            ////                    {
            ////                        dgvTransferNoteList.Rows.Add();
            ////                        dgvTransferNoteList.Rows[i].Cells[0].Value = dt.Tables[0].Rows[i].ItemArray[0].ToString();
            ////                        dgvTransferNoteList.Rows[i].Cells[1].Value = dt.Tables[0].Rows[i].ItemArray[1].ToString();
            ////                        dgvTransferNoteList.Rows[i].Cells[2].Value = dt.Tables[0].Rows[i].ItemArray[2].ToString();
            ////                        dgvTransferNoteList.Rows[i].Cells[3].Value = dt.Tables[0].Rows[i].ItemArray[3].ToString();

            ////                        dgvTransferNoteList.Rows[i].Cells[4].Value = dt1.Tables[0].Rows[i].ItemArray[1].ToString();
            ////                        dgvTransferNoteList.Rows[i].Cells[5].Value = dt1.Tables[0].Rows[i].ItemArray[2].ToString();
            ////                        dgvTransferNoteList.Rows[i].Cells[6].Value = dt1.Tables[0].Rows[i].ItemArray[3].ToString();
            ////                        dgvTransferNoteList.Rows[i].Cells[7].Value = dt1.Tables[0].Rows[i].ItemArray[4].ToString();

            ////                    }
            ////                }
            ////            }

            ////        }
            ////    }
            ////    catch { }

            ////    //        //Item ID
            ////    //        try
            ////    //        {
            ////    //            if (cmbSearchBy.Text.ToString().Trim() == "From Warehouse Id")
            ////    //            {
            ////    //                string add = txtSearchBy.Text;
            ////    //                if (add != "")
            ////    //                {
            ////    //                    String S = "Select tblWhseTransfer.WhseTransId,tblWhseTransfer.FrmWhseId,tblWhseTransfer.ToWhseId,tblWhseTransfer.TDate	 from tblWhseTransfer where FrmWhseId LIKE  '" + add + "%'";
            ////    //                    SqlCommand cmd = new SqlCommand(S);
            ////    //                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            ////    //                    DataSet dt = new DataSet();
            ////    //                    da.Fill(dt);
            ////    //                    dgvTransferNoteList.Rows.Clear();

            ////    //                    if (dt.Tables[0].Rows.Count > 0)
            ////    //                    {
            ////    //                        for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            ////    //                        {
            ////    //                            dgvTransferNoteList.Rows.Add();
            ////    //                            dgvTransferNoteList.Rows[i].Cells[0].Value = dt.Tables[0].Rows[i].ItemArray[0].ToString();
            ////    //                            dgvTransferNoteList.Rows[i].Cells[1].Value = dt.Tables[0].Rows[i].ItemArray[1].ToString();
            ////    //                            dgvTransferNoteList.Rows[i].Cells[2].Value = dt.Tables[0].Rows[i].ItemArray[2].ToString();
            ////    //                            dgvTransferNoteList.Rows[i].Cells[3].Value = dt.Tables[0].Rows[i].ItemArray[3].ToString();
            ////    //                        }
            ////    //                    }
            ////    //                }
            ////    //            }
            ////    //        }
            ////    //        catch { }

            ////    //        //month
            ////    //        try
            ////    //        {
            ////    //            if (cmbSearchBy.Text.ToString().Trim() == "To Warehouse Id")
            ////    //            {
            ////    //                string add = txtSearchBy.Text;
            ////    //                if (add != "")
            ////    //                {
            ////    //                    String S = "Select tblWhseTransfer.WhseTransId,tblWhseTransfer.FrmWhseId,tblWhseTransfer.ToWhseId,tblWhseTransfer.TDate	 from tblWhseTransfer where ToWhseId LIKE  '" + add + "%'";
            ////    //                    SqlCommand cmd = new SqlCommand(S);
            ////    //                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            ////    //                    DataSet dt = new DataSet();
            ////    //                    da.Fill(dt);
            ////    //                    dgvTransferNoteList.Rows.Clear();

            ////    //                    if (dt.Tables[0].Rows.Count > 0)
            ////    //                    {
            ////    //                        for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            ////    //                        {
            ////    //                            dgvTransferNoteList.Rows.Add();
            ////    //                            dgvTransferNoteList.Rows[i].Cells[0].Value = dt.Tables[0].Rows[i].ItemArray[0].ToString();
            ////    //                            dgvTransferNoteList.Rows[i].Cells[1].Value = dt.Tables[0].Rows[i].ItemArray[1].ToString();
            ////    //                            dgvTransferNoteList.Rows[i].Cells[2].Value = dt.Tables[0].Rows[i].ItemArray[2].ToString();
            ////    //                            dgvTransferNoteList.Rows[i].Cells[3].Value = dt.Tables[0].Rows[i].ItemArray[3].ToString();
            ////    //                        }
            ////    //                    }
            ////    //                }
            ////    //            }
            ////    //        }
            ////    //        catch { }

            ////    //        //year
            ////    //        try
            ////    //        {
            ////    //            if (cmbSearchBy.Text.ToString().Trim() == "Item Id")
            ////    //            {
            ////    //                string add = txtSearchBy.Text;
            ////    //                if (add != "")
            ////    //                {
            ////    //                    String S = "Select tblWhseTransLine.WhseTransId,tblWhseTransLine.ItemId,tblWhseTransLine.ItemDis,tblWhseTransLine.QTY,tblWhseTransLine.UOM from tblWhseTransLine where ItemId LIKE  '" + add + "%'";
            ////    //                    SqlCommand cmd = new SqlCommand(S);
            ////    //                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            ////    //                    DataSet dt = new DataSet();
            ////    //                    da.Fill(dt);
            ////    //                    dgvTransferNoteList.Rows.Clear();

            ////    //                    if (dt.Tables[0].Rows.Count > 0)
            ////    //                    {
            ////    //                        for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            ////    //                        {
            ////    //                            dgvTransferNoteList.Rows.Add();
            ////    //                            dgvTransferNoteList.Rows[i].Cells[0].Value = dt.Tables[0].Rows[i].ItemArray[0].ToString();
            ////    //                            dgvTransferNoteList.Rows[i].Cells[1].Value = dt.Tables[0].Rows[i].ItemArray[1].ToString();
            ////    //                            dgvTransferNoteList.Rows[i].Cells[2].Value = dt.Tables[0].Rows[i].ItemArray[2].ToString();
            ////    //                            dgvTransferNoteList.Rows[i].Cells[3].Value = dt.Tables[0].Rows[i].ItemArray[3].ToString();
            ////    //                        }
            ////    //                    }
            ////    //                }
            ////    //            }
            ////    //        }
            ////    //        catch { }
            ////    //    }
            ////    //    else
            ////    //    {
            ////    //        String S = "Select RepCode,RepName,Month,Year from tblSchedule";
            ////    //        SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            ////    //        DataSet dt = new DataSet();
            ////    //        da.Fill(dt);
            ////    //        dgvTransferNoteList.Rows.Clear();


            ////    //        if (dt.Tables[0].Rows.Count > 0)
            ////    //        {
            ////    //            for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            ////    //            {
            ////    //                dgvTransferNoteList.Rows.Add();
            ////    //                dgvTransferNoteList.Rows[i].Cells[0].Value = dt.Tables[0].Rows[i].ItemArray[0].ToString();
            ////    //                dgvTransferNoteList.Rows[i].Cells[1].Value = dt.Tables[0].Rows[i].ItemArray[1].ToString();
            ////    //                dgvTransferNoteList.Rows[i].Cells[2].Value = dt.Tables[0].Rows[i].ItemArray[2].ToString();
            ////    //                dgvTransferNoteList.Rows[i].Cells[3].Value = dt.Tables[0].Rows[i].ItemArray[3].ToString();
            ////    //            }
            ////    //        }
            ////    //    }

            ////    //}
            ////}
            ////else

            //string rdate = Get_Date(dateTimePicker1.Value.ToShortDateString());
            //string rdate1 = Get_Date(dateTimePicker2.Value.ToShortDateString());
            DateTime DTP = Convert.ToDateTime(dateTimePicker1.Text);
            string Dformat = "MM/dd/yyyy";
            string WHTDate = DTP.ToString(Dformat);
            DateTime DTP1 = Convert.ToDateTime(dateTimePicker1.Text);
            //string Dformat1 = "MM/dd/yyyy";
            string WHTDate1 = DTP.ToString(Dformat);

            //{
            //////String S = "select * form tblWhseTransfer "; //where WhseTransId   '" +  + "'";
            //////           SqlCommand cmd = new SqlCommand(S);
            //////           SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            //////           DataSet dt = new DataSet();
            //////           da.Fill(dt);

            //////           String S1 = "select * form tblWhseTransLine"; //where WhseTransId LIKE  '" +  + "'";
            //////           SqlCommand cmd1 = new SqlCommand(S1);
            //////           SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
            //////           DataSet dt1 = new DataSet();
            //////           da1.Fill(dt1);

            //////           if (dt.Tables[0].Rows.Count > 0)
            //////           {
            //////               for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            //////               {
            //////                   dgvTransferNoteList.Rows.Add();
            //////                   dgvTransferNoteList.Rows[i].Cells[0].Value = dt.Tables[0].Rows[i].ItemArray[0].ToString();
            //////                   dgvTransferNoteList.Rows[i].Cells[1].Value = dt.Tables[0].Rows[i].ItemArray[1].ToString();
            //////                   dgvTransferNoteList.Rows[i].Cells[2].Value = dt.Tables[0].Rows[i].ItemArray[2].ToString();
            //////                   dgvTransferNoteList.Rows[i].Cells[3].Value = dt.Tables[0].Rows[i].ItemArray[3].ToString();

            //////                   dgvTransferNoteList.Rows[i].Cells[4].Value = dt1.Tables[0].Rows[i].ItemArray[1].ToString();
            //////                   dgvTransferNoteList.Rows[i].Cells[5].Value = dt1.Tables[0].Rows[i].ItemArray[2].ToString();
            //////                   dgvTransferNoteList.Rows[i].Cells[6].Value = dt1.Tables[0].Rows[i].ItemArray[3].ToString();
            //////                   dgvTransferNoteList.Rows[i].Cells[7].Value = dt1.Tables[0].Rows[i].ItemArray[4].ToString();

            //////               }
            //////           }
            //////      // }

        }

        private void cmbSearchBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            ////txtSearchBy.Focus();
        }

        public string Get_Date(string G_date)
        {
            string New_date = "";
            try
            {
                string[] Date = G_date.Split('/');
                string oldDate = "";
                string month = "";
                if (Date[0].Length == 1)
                {
                    oldDate = "0" + Date[0];
                }
                else
                {
                    oldDate = Date[0];
                }
                if (Date[1].Length == 1)
                {
                    month = "0" + Date[1];
                }
                else
                {
                    month = Date[1];
                }

                //New_date = month + "/" + oldDate + "/" + Date[2];
                New_date = oldDate + "/" + month + "/" + Date[2];
                // Convert.ToDateTime(New_date);
            }
            catch { }
            //   DateTime dat = Convert.ToDateTime(New_date);
            return New_date;
        }

        public string rptOpt = "Detail";
        private void btnPrint_Click(object sender, EventArgs e)
        {
            DateTime DTP = Convert.ToDateTime(dateTimePicker1.Text);
            string Dformat = "MM/dd/yyyy";
            string WHTDate = DTP.ToString(Dformat);
            DateTime DTP1 = Convert.ToDateTime(dateTimePicker2.Text);
            string Dformat1 = "MM/dd/yyyy";
            string WHTDate1 = DTP1.ToString(Dformat1);

            dsTNoteList.Clear();

            String StrSql = "SELECT * FROM tblCompanyInformation";
            SqlDataAdapter da251 = new SqlDataAdapter(StrSql, ConnectionString);
            da251.Fill(dsTNoteList, "DtCompaniInfo");
            String S25 = null;

            dsTNoteList.Dt_tblWhseTransfer.Clear();
            if (chkFromWHAll.Checked == true && chkToWHAll.Checked == true)
            {
                S25 = "Select * from tblWhseTransfer where TDate between '" + WHTDate + "' and '" + WHTDate1 + "'";//
            }

            if (chkFromWHAll.Checked == true && chkToWHAll.Checked == false)
            {
                if (cmbToWH.Text.ToString() == string.Empty)
                {
                    MessageBox.Show("Please Select To Warehouse");
                    return;
                }
                 S25 = "Select * from tblWhseTransfer where TDate between '" + WHTDate + "' and '" + WHTDate1 + "' and ToWhseId='" + cmbToWH.Text.ToString().Trim() + "'";//
                
            }
            if (chkFromWHAll.Checked == false && chkToWHAll.Checked == true)
            {

                if (cmbWH.Text.ToString() == string.Empty)
                {
                    MessageBox.Show("Please Select From Warehouse");
                    return;
                }

                 S25 = "Select * from tblWhseTransfer where TDate between '" + WHTDate + "' and '" + WHTDate1 + "' and FrmWhseId='" + cmbWH.Text.ToString().Trim() + "'";//
            }
            if (chkFromWHAll.Checked == false && chkToWHAll.Checked == false)
            {

                if (cmbWH.Text.ToString() == string.Empty)
                {
                    MessageBox.Show("Please Select From Warehouse");
                    return;
                }
                if (cmbToWH.Text.ToString() == string.Empty)
                {
                    MessageBox.Show("Please Select To Warehouse");
                    return;
                }
                S25 = "Select * from tblWhseTransfer where TDate between '" + WHTDate + "' and '" + WHTDate1 + "' and FrmWhseId='" + cmbWH.Text.ToString().Trim() + "' and ToWhseId='" + cmbToWH.Text.ToString().Trim() + "'";//
            }




            SqlDataAdapter da25 = new SqlDataAdapter(S25, ConnectionString);
            da25.Fill(dsTNoteList, "Dt_tblWhseTransfer");
            for (int i = 0; i < dsTNoteList.Tables[0].Rows.Count; i++)
            {

                String AB = "Select Distinct WhseTransId from tblWhseTransLine where WhseTransId='" + dsTNoteList.Tables[0].Rows[i].ItemArray[0] + "'";//
                SqlDataAdapter ABC = new SqlDataAdapter(AB, ConnectionString);
                DataTable dt = new DataTable();
                ABC.Fill(dt);
                //da251.Fill(wh, "Dt_tblWhseTransLine");
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    String S251 = "Select * from tblWhseTransLine where WhseTransId='" + dsTNoteList.Tables[0].Rows[i].ItemArray[0] + "'";//
                    SqlDataAdapter da2512 = new SqlDataAdapter(S251, ConnectionString);
                    da2512.Fill(dsTNoteList, "Dt_tblWhseTransLine");
                }
            }
            if (OptDetail.Checked == true)
            {
                rptOpt = "Detail";
            }
            if (optsummary.Checked == true)
            {
                rptOpt = "Summery";
            }

            if (OptPtoAll.Checked == true)
            {
                rptOpt = "FromToALL";
            }

            if (OptAlltoP.Checked == true)
            {
                rptOpt = "AllToFrom";
            }
            frmTransNoteListView trns = new frmTransNoteListView(this);
            trns.Show();

        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chkFromWHAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkFromWHAll.Checked)
            {
                cmbWH.Text = "";
                cmbWH.Enabled = false;
            }
            else
            {
                cmbWH.Enabled = true;
            }
        }

        private void chkToWHAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkToWHAll.Checked)
            {
                cmbToWH.Text = "";
                cmbToWH.Enabled = false;
            }
            else
            {
                cmbToWH.Enabled = true;
            }
        }
    }
}