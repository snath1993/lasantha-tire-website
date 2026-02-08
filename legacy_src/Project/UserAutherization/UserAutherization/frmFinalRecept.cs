using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UserAutherization
{
    public partial class frmFinalRecept : Form
    {
        private DataSet dsCustomer;
        private string ConnectionString;
        private string strINVNO;
        private string strTOTAL;
        private string StrSql;
        private int newRow;
        public bool isConfirm = false;
        public dsFinalReceipt ds = new dsFinalReceipt();
        public frmFinalRecept()
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
            catch (Exception ex) { throw ex; }
        }
        private void cmbCustomer_RowSelected(object sender, Infragistics.Win.UltraWinGrid.RowSelectedEventArgs e)
        {
            New();
            try
            {
                txtName.Text = cmbCustomer.ActiveRow.Cells[2].Value.ToString();
                txtContact.Text = cmbCustomer.ActiveRow.Cells[4].Value.ToString();
                strINVNO = cmbCustomer.ActiveRow.Cells[0].Value.ToString();
                strTOTAL = cmbCustomer.ActiveRow.Cells[5].Value.ToString();
                var IsConfirm = cmbCustomer.ActiveRow.Cells[6].Value.ToString();
                if (IsConfirm == "True")
                {
                    isConfirm = true;
                }
                LoadGrid();
                Calculate();
                checkBox1.Enabled = true;
            }
            catch
            {

            }
        }

        private void New()
        {

            txtContact.Text = "";
            txtName.Text = "";
            txtNetValue.Text = "0.00";
            ClearGrid();
        }

        private void LoadGrid()
        {
            try
            {
                StrSql = "SELECT [FinalInvoiceNO],[DisNo],[Instalment],[isFullPay],[Date],FinallReceptNO,isVoid FROM [tblFinalRecept] WHERE FinalInvoiceNO='" + strINVNO + "'";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    int i = 0;
                    UltraGridRow ugR;
                    foreach (DataRow Dr in dt.Rows)
                    {

                        ugR = ug.DisplayLayout.Bands[0].AddNew();
                        if (i == 0)
                        {
                            ugR.Cells["FinalInvoiceNO"].Value = strINVNO;
                            ugR.Cells["InvoiceTotal"].Value = strTOTAL;

                        }
                        ugR.Cells["DisNo"].Value = Dr["DisNo"];
                        DateTime dtDT = Convert.ToDateTime(Dr["Date"]);
                        ugR.Cells["Date"].Value = dtDT.ToShortDateString();
                        ugR.Cells["Instalment"].Value = Dr["Instalment"];
                        ugR.Cells["ReceptNo"].Value = Dr["FinallReceptNO"];
                        if (Dr["isVoid"].ToString() == "True")
                        {
                            ugR.Appearance.BackColor = Color.LightPink;
                        }

                        i++;
                    }
                }
                else
                {
                    UltraGridRow ugR;
                    ugR = ug.DisplayLayout.Bands[0].AddNew();
                    ugR.Cells["FinalInvoiceNO"].Value = strINVNO;
                    ugR.Cells["InvoiceTotal"].Value = strTOTAL;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void frmFinalRecept_Load(object sender, EventArgs e)
        {
            GetCustomer();
            getpaymentmode();
            GetFiNV();
        }

        private void GetFiNV()
        {
            try
            {

                Int32 intX;
                Int32 intZ;
                string StrInvNo;
                Int32 intP;
                Int32 intI;
                String StrInV;

                StrSql = "SELECT [FinalReceptPref],[FinalReceptPad],[FinelReceptNo]FROM tblDefualtSetting";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    StrInvNo = dt.Rows[0].ItemArray[0].ToString().Trim();
                    intX = Int32.Parse(dt.Rows[0].ItemArray[1].ToString().Trim());
                    intZ = Int32.Parse(dt.Rows[0].ItemArray[2].ToString().Trim());

                    intP = 1;
                    for (intI = 1; intI <= intX; intI++)
                    {
                        intP = intP * 10;
                    }
                    intP = intP + intZ;
                    StrInV = intP.ToString();

                    txtReceptNo.Text = StrInvNo + StrInV.Substring(1, intX);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void UpdatePrefixNo(SqlConnection con, SqlTransaction Trans)
        {
            int intCRNNo;
            SqlCommand command;

            StrSql = "SELECT  TOP 1(FinelReceptNo) FROM tblDefualtSetting ORDER BY FinelReceptNo DESC";

            command = new SqlCommand(StrSql, con, Trans);
            SqlDataAdapter da = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                intCRNNo = (int.Parse(dt.Rows[0].ItemArray[0].ToString().Trim())) + 1;
            }
            else
            {
                intCRNNo = 1;
            }
            StrSql = "UPDATE tblDefualtSetting SET FinelReceptNo='" + intCRNNo + "'";

            command = new SqlCommand(StrSql, con, Trans);
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
        }

        public void GetCustomer()
        {
            dsCustomer = new DataSet();
            try
            {
                dsCustomer.Clear();
                string StrSql = "SELECT distinct([InvoiceNo]) as [InvoiceNo],[CustomerID],[Name],[Room],ContactNo,GrandTotal,[IsConfirm],Type FROM[ViewFinalBillList] WHERE (isFullpay='0') OR ([IsConfirm]='0')";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsCustomer, "DtClient");

                cmbCustomer.DataSource = dsCustomer.Tables["DtClient"];
                cmbCustomer.DisplayMember = "CustomerID";
                cmbCustomer.ValueMember = "Type";

                cmbCustomer.DisplayLayout.Bands["DtClient"].Columns["InvoiceNo"].Width = 100;
                cmbCustomer.DisplayLayout.Bands["DtClient"].Columns["CustomerID"].Width = 70;
                cmbCustomer.DisplayLayout.Bands["DtClient"].Columns["Name"].Width = 150;
                cmbCustomer.DisplayLayout.Bands["DtClient"].Columns["Room"].Hidden = true;
                cmbCustomer.DisplayLayout.Bands["DtClient"].Columns["ContactNo"].Hidden = true;
                cmbCustomer.DisplayLayout.Bands["DtClient"].Columns["GrandTotal"].Hidden = true;
                cmbCustomer.DisplayLayout.Bands["DtClient"].Columns["IsConfirm"].Hidden = true;
                cmbCustomer.DisplayLayout.Bands["DtClient"].Columns["Type"].Hidden = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            cmbCashAccount.Text = "";
            cmbCustomer.Text = "";
            GetCustomer();
            GetFiNV();
            New();
            txtPayment.Text = "0.00";
            cmbCustomer.Enabled = true;
            cmbCashAccount.Enabled = true;
            txtPayment.Enabled = true;
            checkBox1.Enabled = true;
            btnVoid.Enabled = false;
            toolStripButton3.Enabled = true;
            dtpDate.Enabled = true;
            dtpDate.Value = DateTime.Now;
        }

        private void ClearGrid()
        {
            try
            {
                foreach (UltraGridRow ugR in ug.Rows.All)
                {
                    ugR.Delete(false);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void Calculate()
        {
            int i = 0;
            double GTotal = 0;
            double pay = 0;
            int lop = 0;
            try
            {
                foreach (UltraGridRow ugR in ug.Rows.All)
                {
                    if (i == 0)
                    {
                        if (ugR.Cells["InvoiceTotal"].Value.ToString() != string.Empty)
                        {
                            GTotal = Convert.ToDouble(ugR.Cells["InvoiceTotal"].Value);

                        }

                    }
                }
                foreach (UltraGridRow ugR in ug.Rows.All)
                {
                    if (ugR.Cells["Instalment"].Value.ToString() != string.Empty)
                    {
                        pay = pay + Convert.ToDouble(ugR.Cells["Instalment"].Value);
                    }

                }
               // var tt =Convert.ToDouble( txtPayment.Text);
                txtNetValue.Text = (GTotal).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ug_KeyDown(object sender, KeyEventArgs e)
        {
            //try
            //{
            //    UltraGridRow ugR;
            //    switch (e.KeyValue)
            //    {
            //        case 9:
            //            {
            //                if (ug.ActiveCell.Column.Key == "Instalment")
            //                {
            //                    if (newRow == 0)
            //                    {


            //                        ugR = ug.DisplayLayout.Bands[0].AddNew();
            //                        ugR.Cells["DisNo"].Value = ugR.Index + 1;
            //                        ugR.Cells["Instalment"].Selected = false;
            //                        ugR.Cells["Instalment"].Activated = true;
            //                        newRow++;
            //                    }
            //                }
            //                break;
            //            }
            //        case 13:
            //            {
            //                if (ug.ActiveCell.Column.Key == "Instalment")
            //                {
            //                    ugR = this.ug.ActiveRow;
            //                    if (ug.ActiveRow.Cells["Instalment"].Text.ToString() != string.Empty)
            //                    {

            //                        DateTime dt = System.DateTime.Today;
            //                        ugR.Cells["Date"].Value = dt.ToShortDateString();


            //                    }
            //                }
            //                break;
            //            }
            //    }
            //}
            //catch
            //{

            //}


        }
        private void getpaymentmode()
        {

            DataSet dspaymetmode = new DataSet();
            try
            {
                dspaymetmode.Clear();
                StrSql = "SELECT [CardType] As PaymentMethod,[GL_Account] FROM [tblCreditData]";
                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dspaymetmode, "DtClient");

                cmbCashAccount.DataSource = dspaymetmode.Tables["DtClient"];
                cmbCashAccount.DisplayMember = "PaymentMethod";
                cmbCashAccount.ValueMember = "GL_Account";
                cmbCashAccount.DisplayLayout.Bands["DtClient"].Columns["GL_Account"].Hidden = true;


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ug_Click(object sender, EventArgs e)
        {
            try
            {
                UltraGridRow ugR;


                if (ug.Rows.Count == 0)
                {
                    ugR = ug.DisplayLayout.Bands[0].AddNew();
                    ugR.Cells["DisNo"].Value = ugR.Index + 1;
                    ugR.Cells["Instalment"].Selected = false;
                    ugR.Cells["Instalment"].Activated = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //DltEmtyRow();
            int countRow = CountRow();
            string str = txtNetValue.Text.ToString();
            //ClearGrid();
            try
            {
                if (checkBox1.Checked == true)
                {
                    //StrSql = "SELECT [FinalInvoiceNO],[DisNo],[Instalment],[isFullPay],[Date]FROM [tblFinalRecept] WHERE FinalInvoiceNO='" + strINVNO + "'";
                    //int i = 0;
                    //SqlCommand cmd = new SqlCommand(StrSql);
                    //SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                    //DataTable dt = new DataTable();
                    //da.Fill(dt);
                    //UltraGridRow ugR;
                    //if (dt.Rows.Count > 0)
                    //{

                    //    //UltraGridRow ugR;
                    //    foreach (DataRow Dr in dt.Rows)
                    //    {

                    //        ugR = ug.DisplayLayout.Bands[0].AddNew();
                    //        if (i == 0)
                    //        {
                    //            ugR.Cells["FinalInvoiceNO"].Value = strINVNO;
                    //            ugR.Cells["InvoiceTotal"].Value = strTOTAL;
                    //        }
                    //        ugR.Cells["DisNo"].Value = Dr["DisNo"];
                    //        DateTime dtDT = Convert.ToDateTime(Dr["Date"]);
                    //        ugR.Cells["Date"].Value = dtDT.ToShortDateString();
                    //        ugR.Cells["Instalment"].Value = Dr["Instalment"];
                    //        i++;
                    //    }
                    //}
                    var bal = txtNetValue.Text.ToString();
                    //ugR = ug.DisplayLayout.Bands[0].AddNew();
                    //if (i == 0)
                    //{
                    //    ugR.Cells["FinalInvoiceNO"].Value = strINVNO;
                    //    ugR.Cells["InvoiceTotal"].Value = strTOTAL;
                    //}


                    //var val = System.DateTime.Today;
                    //ugR.Cells["DisNo"].Value = i + 1;
                    //ugR.Cells["Date"].Value = val.ToShortDateString();
                    //ugR.Cells["Instalment"].Value = bal;
                    txtPayment.Text = bal;
                    txtPayment.Enabled = false;

                }
                else
                {
                    //LoadGrid();
                    txtPayment.Enabled = true;
                    txtPayment.Text = "0.00";
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private int CountRow()
        {
            int ffffff = 0;
            foreach (UltraGridRow ugR in ug.Rows.All)
            {
                if (ugR.Cells["Instalment"].Value.ToString() == string.Empty && ugR.Cells["Date"].Value.ToString() == string.Empty)
                {
                    return ffffff;

                }
                ffffff++;
            }
            return ffffff;
        }

        private void DltEmtyRow()
        {
            try
            {
                ug.PerformAction(UltraGridAction.CommitRow);
                foreach (UltraGridRow ugR in ug.Rows.All)
                {
                    if (ugR.Cells[2].Value.ToString().Trim().Length == 0 || ugR.Cells[3].Value.ToString().Trim().Length == 0 || ugR.Cells[4].Value.ToString().Trim().Length == 0)
                    {
                        ugR.Delete(false);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ug_AfterExitEditMode(object sender, EventArgs e)
        {


            Calculate();
        }

        private void ug_AfterCellUpdate(object sender, CellEventArgs e)
        {

            Calculate();
        }
        public bool full = false;
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            //string v = cmbCashAccount.Value.ToString();
            //string vv = cmbCashAccount.Text.ToString();

            if (cmbCustomer.Text.ToString() == string.Empty)
            {
                MessageBox.Show("Please Select Customer");
                return;
            }
            if (cmbCashAccount.Text.ToString() == string.Empty)
            {
                MessageBox.Show("Plese Select Payment Method");
                return;
            }
            //DltEmtyRow();
            //if (ug.Rows.Count == 0)
            //{
            //    MessageBox.Show("Please Fill Payment in Grid");
            //    return;
            //}
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            SqlTransaction trans = con.BeginTransaction();
            try
            {
                GetFiNV();
                UpdatePrefixNo(con, trans);
                SaveEvent(con, trans);

                if (isConfirm == true)
                {
                    if (txtPayment.Text == txtNetValue.Text)
                    {
                        full = true;
                    }
                    if (checkBox1.Checked == true)
                    {
                        full = true;
                    }
                }

                if (full == true)
                {
                    UpdateFinalInvoice(con, trans);

                }

                SaveScanchanel(con, trans);


                trans.Commit();
                ClearGrid();
                LoadGrid();
                MessageBox.Show("Final Recept Save Successfuly");
                checkBox1.Checked = false;
                txtPayment.Text = "0.00";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                trans.Rollback();
            }
        }

        private void SaveScanchanel(SqlConnection con, SqlTransaction trans)
        {
            String S12 = "insert into [tblScanChannel](ConsultFee,HospitalFee,TotalFee,ReceiptTotal,IsConfirm,IsExport,DueDate,GLAccount,ItemDescription,ItemID,Consultant,TokenNo,[ReceiptNo],[Date],[PatientName],[ContactNo],[PaymentMethod],[NetTotal],[PatientNo],[PationType],[IsVoid],DistributionNumber,CurrentUser,[ItemType],CollectTime)" +
                   "values ('0','" + txtPayment.Text.ToString() + "','" + txtPayment.Text.ToString() + "','" + txtPayment.Text.ToString() + "','TRUE','FALSE','" + dtpDate.Value.ToShortDateString() + "','10210','FINALL BILL AMOUNT','FINAL','FINAL','1','" + txtReceptNo.Text.ToString() + "','" + dtpDate.Value.ToShortDateString() + "', '" + txtName.Text.ToString() + "','" + txtContact.Text.ToString() + "','" + cmbCashAccount.Text.ToString() + "','" + txtPayment.Text.ToString() + "','" + cmbCustomer.Text.ToString() + "','" + cmbCustomer.Value.ToString() + "','False','1','" + user.userName + "','" + cmbCustomer.Value.ToString() + "','"+System.DateTime.Now.ToString("HH:mm:ss")+"')";
            SqlCommand cmd12 = new SqlCommand(S12, con, trans);
            cmd12.ExecuteNonQuery();
        }

        private void UpdateFinalInvoice(SqlConnection con, SqlTransaction trans)
        {
            String S2 = "UPDATE [dbo].[tblFinalInvoice]SET [isFullpay] = 'true'WHERE InvoiceNo='" + ug.Rows[0].Cells["FinalInvoiceNO"].Value.ToString() + "'";
            SqlCommand cmd2 = new SqlCommand(S2, con, trans);
            cmd2.ExecuteNonQuery();
        }

        private void DeletEvent(SqlConnection con, SqlTransaction trans)
        {
            //String S2 = "DELETE FROM [tblFinalRecept]WHERE FinallReceptNO ='" +  + "'";
            //SqlCommand cmd2 = new SqlCommand(S2, con, trans);
            //cmd2.ExecuteNonQuery();
        }

        private void SaveEvent(SqlConnection con, SqlTransaction trans)
        {
            int count = 0;


            for (int Grid = 0; Grid < ug.Rows.Count; Grid++)
            {
                count = Grid + 1;

            }

            String S2 = "insert into [tblFinalRecept]([FinalInvoiceNO],[DisCount],[DisNo],[Instalment],[isFullPay],[Date],[CurrentUser],FinallReceptNO,PaymentMethod)" +
                "values ('" + strINVNO + "','" + ug.Rows.Count + "', '" + count + "', '" + txtPayment.Text.ToString() + "', '" + full + "', '" + dtpDate.Value + "', '" + user.userName + "', '" + txtReceptNo.Text.ToString() + "','" + cmbCashAccount.Text.ToString() + "')";
            SqlCommand cmd2 = new SqlCommand(S2, con, trans);
            cmd2.ExecuteNonQuery();




        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            frmFinalReceiptList ffrl = new frmFinalReceiptList();
            ffrl.ShowDialog();
            if (Search.searchFinalReceiptInvoiceNo != string.Empty || Search.searchFinalReceiptNo != string.Empty)
            {
                cmbCustomer.Text = Search.FinalReceiptCus;
                txtName.Text = Search.FinalReceiptCusName;
                strINVNO = Search.FinalReINV;
                strTOTAL = Search.FinalGeand;
                SearchDetail(Search.searchFinalReceiptInvoiceNo);
                SearchReceipt(Search.searchFinalReceiptNo);
                dtpDate.Value = Convert.ToDateTime((Search.searchFinalDate).ToString());
                cmbCustomer.Enabled = false;
                cmbCashAccount.Enabled = false;
                txtPayment.Enabled = false;
                checkBox1.Enabled = false;
                btnVoid.Enabled = true;
                toolStripButton3.Enabled = false;
                dtpDate.Enabled = false;
            }

        }

        private void SearchReceipt(string searchFinalReceiptNo)
        {
            try
            {
                StrSql = "SELECT distinct([FinallReceptNO])as [FinallReceptNO],[Instalment],[PaymentMethod],[ContactNo] FROM [ViewFinalReceipList] WHERE FinallReceptNO='" + searchFinalReceiptNo + "'";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    txtContact.Text = dt.Rows[0].ItemArray[3].ToString().Trim(); ;
                    txtPayment.Text = dt.Rows[0].ItemArray[1].ToString().Trim(); ;
                    txtReceptNo.Text = searchFinalReceiptNo;
                    cmbCashAccount.Text = dt.Rows[0].ItemArray[2].ToString().Trim(); ;
                }
            }
            catch
            {

            }
        }

        private void SearchDetail(string val)
        {
            ClearGrid();
            try
            {
                StrSql = "SELECT distinct([FinallReceptNO]) as FinallReceptNO,[DisNo],[Instalment],[isFullPay],[Date],FinalInvoiceNO,isVoid FROM [tblFinalRecept] WHERE FinalInvoiceNO='" + val + "'";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    int i = 1;
                    UltraGridRow ugR;
                    foreach (DataRow Dr in dt.Rows)
                    {

                        ugR = ug.DisplayLayout.Bands[0].AddNew();
                        if (i == 1)
                        {
                            ugR.Cells["FinalInvoiceNO"].Value = strINVNO;
                            ugR.Cells["InvoiceTotal"].Value = strTOTAL;

                        }
                        ugR.Cells["DisNo"].Value = i.ToString();
                        DateTime dtDT = Convert.ToDateTime(Dr["Date"]);
                        ugR.Cells["Date"].Value = dtDT.ToShortDateString();
                        ugR.Cells["Instalment"].Value = Dr["Instalment"];
                        ugR.Cells["ReceptNo"].Value = Dr["FinallReceptNO"];
                        if (Dr["isVoid"].ToString() == "True")
                        {
                            ugR.Appearance.BackColor = Color.LightPink;
                        }
                        i++;
                    }
                }
                else
                {
                    UltraGridRow ugR;
                    ugR = ug.DisplayLayout.Bands[0].AddNew();
                    ugR.Cells["FinalInvoiceNO"].Value = strINVNO;
                    ugR.Cells["InvoiceTotal"].Value = strTOTAL;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnVoid_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            SqlTransaction trans = con.BeginTransaction();
            try
            {

                String S2 = "UPDATE [tblFinalRecept] SET [isVoid] = 'True' WHERE FinallReceptNO ='" + txtReceptNo.Text.ToString().Trim() + "'";
                SqlCommand cmd2 = new SqlCommand(S2, con, trans);
                cmd2.ExecuteNonQuery();

                String S12 = "UPDATE [tblScanChannel] SET [isVoid] = 'True' WHERE [ReceiptNo] ='" + txtReceptNo.Text.ToString().Trim() + "'";
                SqlCommand cmd12 = new SqlCommand(S12, con, trans);
                cmd12.ExecuteNonQuery();


                trans.Commit();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                trans.Rollback();


            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {

                Print(txtReceptNo.Text.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Print(string strCRNNo)
        {
            try
            {
                // ds = new DataSet();
                DialogResult reply = MessageBox.Show("Are You Sure, You Want To Print This?", "Print", MessageBoxButtons.OKCancel);
                if (reply == DialogResult.Cancel)
                {
                    return;
                }


                ds.Clear();
                String S1 = "SELECT distinct([FinallReceptNO])as[FinallReceptNO], [CustomerID],[InvoiceNo],[Name],[Date],[isVoid],[Instalment],[PaymentMethod],[ContactNo],Type,CurrentUser FROM [ViewFinalReceipList] WHERE [FinallReceptNO] = '" + strCRNNo + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlConnection con1 = new SqlConnection(ConnectionString);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
                //da1.Fill(ds, "DTReturn");
                da1.Fill(ds.tblFinalReceipt);



                // DirectPrint();

                frmFinalReceiptViewer cusReturn = new frmFinalReceiptViewer(this);
                cusReturn.Show();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

    }

}