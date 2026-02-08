using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;

namespace UserAutherization
{
    // Author:	<Tharaka>
    // Create date: <06/08/2010>
    //Modified By :
    public class Controlers
    {

        public static string ConnectionString;
        public string MergeAccUser;

         public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch (Exception ex) { throw ex; }
        }

        public void FocusControl(Control Next, Control Previous, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                Next.Focus();
            if (e.KeyCode == Keys.Up)
                Previous.Focus();
        }

        public void ValidateAmountsFields(TextBox objField, KeyPressEventArgs e)
        {

            if (e.KeyChar != (char)Keys.Back)
            {
                if (!Char.IsDigit(e.KeyChar))
                {
                    if (e.KeyChar.ToString() != ".")
                    {
                        e.Handled = true;
                    }
                    else
                    {
                        //if (objField.Text.Contains(".."))
                        //    e.Handled = true;
                        e.Handled = false;
                    }
                }
                //& (objField.Text.IndexOf('.') != -1)
            }
        }

        public bool ValidateAmountsFields(string objField)
        {
            foreach (char chr in objField)
            {
                if (!char.IsDigit(chr))
                {
                    if (chr.ToString() == ".")
                        return true;
                    else
                        return false;
                }               
            }
            return true;
        }

        public bool ValidateNumberFields(string objField)
        {
            foreach (char chr in objField)
            {
                if (!char.IsDigit(chr))
                {                    
                     return false;
                }
            }
            return true;
        }

        public void ValidateNumberFields(TextBox objField, KeyPressEventArgs e)
        {

            if (e.KeyChar != (char)Keys.Back)
            {
                if (!Char.IsDigit(e.KeyChar))
                {
                    //if (e.KeyChar.ToString() != ".")
                    //{
                    //    e.Handled = true;
                    //}
                    //else
                    //{
                        //if (objField.Text.Contains(".."))
                        //    e.Handled = true;
                        e.Handled = true;
                        //objField.Text = "0";
                    //}
                }
                //& (objField.Text.IndexOf('.') != -1)
            }
        }

        public void ValidateNumberFields(string objField, KeyPressEventArgs e)
        {

            if (e.KeyChar != (char)Keys.Back)
            {
                if (!Char.IsDigit(e.KeyChar))
                {
                    //if (e.KeyChar.ToString() != ".")
                    //{
                    //    e.Handled = true;
                    //}
                    //else
                    //{
                    //if (objField.Text.Contains(".."))
                    //    e.Handled = true;
                    e.Handled = false;
                    //}
                }
                //& (objField.Text.IndexOf('.') != -1)
            }
        }

        public void setDecimalPoints(int noOfPoints, TextBox obj_TextBox)
        {
            try
            {
                string zeros = ".";
                if (noOfPoints == 0)
                    return;
                for (int points = 1; points <= noOfPoints; points++)
                {
                    zeros = zeros + "0";
                }
                obj_TextBox.Text = double.Parse(obj_TextBox.Text.Trim()).ToString(zeros);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void fillComboBox(ComboBox combo, string ValueField, string DisplayField, DataSet dataset)
        {
            try
            {
                combo.Items.Clear();
                
                if (dataset.Tables[0].Rows.Count != 0)
                {
                    combo.DataSource = dataset.Tables[0];
                    combo.DisplayMember = DisplayField;
                    combo.ValueMember = ValueField;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void FilListBox(string[] arrCloumn, ListBox lstBox, DataTable dtable)
        {
            try
            {
                lstBox.Items.Clear();
                foreach (DataRow dr in dtable.Rows)
                {
                    ListViewItem lvi = null;
                    foreach (string column in arrCloumn)
                    {
                        lvi = new ListViewItem();
                        lvi.SubItems.Add(dr[column].ToString());
                    }
                    lstBox.Items.Add(lvi);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool checkDateRangeValidity(DateTime From, DateTime To)
        {
            try
            {
                if (!string.IsNullOrEmpty(From.ToString()) && !string.IsNullOrEmpty(To.ToString()))
                {
                    if (From != To)
                    {
                        if (From > To)
                            return false;
                        else
                            return true;
                    }
                    else
                        return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void txtBoxReadOnlyChanged(object sender, EventArgs e,TextBox txtBox)
        {
            if (txtBox.ReadOnly == true)
            {
                txtBox.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            }
            else
            {
                txtBox.BackColor = System.Drawing.SystemColors.Window;
            }
        }

        public void dtPickerReadOnlyChanged(object sender, EventArgs e, DateTimePicker dtp)
        {
            if (dtp.Enabled == true)
            {
                dtp.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            }
            else
            {
                dtp.BackColor = System.Drawing.SystemColors.Window;
            }
        }

        public void fileMove(DataTable DocList)
        {
            try
            {
                if (DocList != null)
                {
                    foreach (DataRow dr in DocList.Rows)
                    {
                        string filepath = dr["Path"].ToString();
                        //FileInfo file = new FileInfo(filepath);
                        string name = dr["Name"].ToString();

                        if (!Directory.Exists(Application.StartupPath.ToString() + "//Attactments"))
                        {
                            Directory.CreateDirectory(Application.StartupPath.ToString() + "//Attactments");
                        }

                        if (!System.IO.File.Exists(Application.StartupPath.ToString() + "//Attactments//" + name))
                            System.IO.File.Copy(filepath, Application.StartupPath.ToString() + "//Attactments//" + name);
                        //else
                        //{
                        //    System.IO.File.Delete(Application.StartupPath.ToString());
                        //    System.IO.File.Copy(filepath, Application.StartupPath.ToString() + "//Attactments//" + name);
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string setDateFormatAsMMddyyyy(string date)
        {
            DateTime DateFormated = new DateTime();
            string d = "";
            try
            {
                if(date.Contains(" "))
                {
                    date = date.Substring(0, date.IndexOf(" "));
                }

                if (date.Length == 10)
                {
                    d = ((date.Substring(3, 2) + "/" + date.Substring(0, 2) + "/" + date.Substring(6, 4)));
                }
                //else
                //    return date;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return d;
        }

        public void validateGridNumericCells(DataGridViewCellEventArgs e, DataGridView dgv)
        {
            try
            {
                foreach (char l in dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString())
                {
                    if (!Char.IsDigit(l)&& l.ToString()!=".")
                    {
                        dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "0.00";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void validateGridNumericCells(DataGridViewCellFormattingEventArgs e,DataGridView dgv)
        {
            try
            {
                foreach(char l in e.Value.ToString())
                {
                    if (!Char.IsDigit(l))
                    {
                        dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RemoveEmptyRowsInGrid(DataGridView dgv)
        {
            try
            {
                if (dgv.Rows.Count > 1)
                {
                    foreach (DataGridViewRow dgvr in dgv.Rows)
                    {
                        if (dgvr.Index > 1)
                        {
                            if (dgvr.Cells[1].Value == null || dgvr.Cells[1].Value.ToString() == "")
                                dgv.Rows.Remove(dgvr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DateTime ConvertDDMMYYYY_To_MMDDYYYY(string Date)
        {
            DateTime dt=new DateTime ();
            try
            {
                IFormatProvider provider = new System.Globalization.CultureInfo("en-CA", true);
                String datetime = Date;
                dt = DateTime.Parse(datetime, provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public bool ValidateRecordisAlreadyExistsInGrid(DataGridView dgvData, string Column1Name, string Column2Name, string Column1Value, string Column2Value)
        {
            try
            {
                foreach (DataGridViewRow dgvr in dgvData.Rows)
                {
                    if (dgvr.Cells[Column1Name].Value == null) return true;
                    if (Column2Name.Length == 0)
                    {
                        if (dgvr.Cells[Column1Name].Value.ToString() == Column1Value) return false;
                    }
                    else
                    {
                        if (dgvr.Cells[Column1Name].Value.ToString() == Column1Value && dgvr.Cells[Column2Name].Value.ToString() == Column2Value) return false;
                        //else return true;
                    }
                    //else
                    //    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }

        public void FillGridAmountForGivenUnitPriceAndQty(DataGridView dgv, string UnitPrice, string Quantity, string Amount,int RowIndex)
        {
            try
            {

                if ((dgv.Rows[RowIndex].Cells[Quantity].Value != null ||
                        dgv.Rows[RowIndex].Cells[UnitPrice].Value != null) &&
                        (dgv.Rows[RowIndex].Cells[Quantity].Value != "" ||
                        dgv.Rows[RowIndex].Cells[UnitPrice].Value != ""))
                {

                    if (dgv.Rows[RowIndex].Cells[UnitPrice].Value != null && dgv.Rows[RowIndex].Cells[UnitPrice].Value.ToString().Trim() != "")
                    {
                        if (dgv.Rows[RowIndex].Cells[Amount].Value == null || dgv.Rows[RowIndex].Cells[Amount].Value == "")
                        {
                            if (dgv.Rows[RowIndex].Cells[Quantity].Value != null && dgv.Rows[RowIndex].Cells[UnitPrice].Value != null)
                                dgv.Rows[RowIndex].Cells[Amount].Value = (double.Parse(dgv.Rows[RowIndex].Cells[Quantity].Value.ToString()) * double.Parse(dgv.Rows[RowIndex].Cells[UnitPrice].Value.ToString())).ToString("0.00");
                        }
                        else
                            dgv.Rows[RowIndex].Cells[Amount].Value = (double.Parse(dgv.Rows[RowIndex].Cells[Amount].Value.ToString()) + (double.Parse(dgv.Rows[RowIndex].Cells[Quantity].Value.ToString()) * double.Parse(dgv.Rows[RowIndex].Cells[UnitPrice].Value.ToString()))).ToString("0.00");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string getGridTotalsForaGivenColumn(DataGridView dgv,string ColumnName)
        {
            double TotalAmt = 0;
            try
            {               
                foreach (DataGridViewRow dgvr in dgv.Rows)
                {
                    if (dgvr.Cells[ColumnName].Value != null && dgvr.Cells[ColumnName].Value.ToString() != "")
                    {
                        TotalAmt = TotalAmt + double.Parse(dgvr.Cells[ColumnName].Value.ToString());
                    }
                }                               
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return TotalAmt.ToString("0.00"); 
        }

        public int DatagridDistinctRowsCountForaGivenColumn(DataGridView dgv, string ColumnName)
        {
            DataTable dtbData = new DataTable();
            dtbData.Columns.Add("Class");
            int CountRows = 0;

            try
            {
                DataRow row;              

                foreach (DataGridViewRow dgvr in dgv.Rows)
                {
                    row = dtbData.NewRow();
                    row["Class"] = dgvr.Cells["Class"].Value;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return CountRows;
        }

        public void DeleteSelectedRow(DataGridView dgv)
        {
            try
            {
                if (dgv.CurrentCell.Value.ToString() == "Delete")
                {
                    dgv.Rows.Remove(dgv.CurrentRow);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public double GetAmountForGivenUnitPriceAndQty(double UnitPrice, double Qty)
        {
            double Amount = 0.00;
            try
            {
                Amount = UnitPrice * Qty;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Amount;
        }

        public static double GetDoubleValue(string maskedValue)
        {
            string strippedValue = "";
            try
            {
                char[] splits = { '$', '_' };
                //string maskedValue = maskedTextBox.Text;
                string[] maskedSegs = maskedValue.Split(splits);
                
                for (int a = 0; a < maskedSegs.Length; a++)
                {
                    if (maskedSegs[a].Length > 0)
                    {
                        strippedValue += maskedSegs[a];
                    }
                }
                //maskStrippedTextBox.Text = strippedValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Convert.ToDouble(strippedValue);
        }

        public bool IsGridEmpty(DataGridView dgv, string CellName)
        {
            try
            {
                foreach (DataGridViewRow dgvr in dgv.Rows)
                {
                    if (!string.IsNullOrEmpty(dgvr.Cells[CellName].Value.ToString()))
                    {
                        if (double.Parse(dgvr.Cells[CellName].Value.ToString()) > 0)
                            return true;
                        else
                            return false;
                    }
                    else
                        return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string AmountInWords(string Amount)
        {
            string AmountInWords="";

            try
            {
                int indexofDot = Amount.IndexOf('.');
                long Rupees = long.Parse(Amount.Substring(0, indexofDot));
                long Cents = long.Parse(Amount.Substring(indexofDot + 1));
                string _Rupees = "ZERO";
                string _Cents = "ZERO";
                if (Rupees != 0)
                {
                    _Rupees = IntegerToWords(Rupees).ToString().Replace("ZERO", "");
                }
                if (Cents != 0)
                {
                    _Cents = IntegerToWords(Cents).ToString().Replace("ZERO", "");
                }
                AmountInWords = _Rupees + " RUPEES AND " + _Cents + " CENTS ONLY";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return AmountInWords;
        }

        private string IntegerToWords(long inputNum)
        {
            try
            {
                int dig1, dig2, dig3, level = 0, lasttwo, threeDigits;

                string retval = "";
                string x = "";
                string[] ones ={
            "ZERO",
            "ONE",
            "TWO",
            "THREE",
            "FOUR",
            "FIVE",
            "SIX",
            "SEVEN",
            "EIGHT",
            "NINE",
            "TEN",
            "ELEVEN",
            "TWELVE",
            "THIRTEEN",
            "FOURTEEN",
            "FIFTEEN",
            "SIXTEEN",
            "SEVENTEEN",
            "EIGHTTEEN",
            "NINETEEN"
          };
                string[] tens ={
            "ZERO",
            "TEN",
            "TWENTY",
            "THIRTY",
            "FOURTY",
            "FIFTY",
            "SIXTY",
            "SEVENTY",
            "EIGHTY",
            "NINETY"
          };
                string[] thou ={
            "",
            "THOUSAND",
            "MILLION",
            "BILLION",
            "TRILLION",
            "QUADRILLION",
            "QUINTILLION"
          };

                bool isNegative = false;
                if (inputNum < 0)
                {
                    isNegative = true;
                    inputNum *= -1;
                }

                if (inputNum == 0)
                    return ("ZERO");

                string s = inputNum.ToString();

                while (s.Length > 0)
                {
                    // Get the three rightmost characters
                    x = (s.Length < 3) ? s : s.Substring(s.Length - 3, 3);

                    // Separate the three digits
                    threeDigits = int.Parse(x);
                    lasttwo = threeDigits % 100;
                    dig1 = threeDigits / 100;
                    dig2 = lasttwo / 10;
                    dig3 = (threeDigits % 10);

                    // append a "thousand" where appropriate
                    if (level > 0 && dig1 + dig2 + dig3 > 0)
                    {
                        retval = thou[level] + " " + retval;
                        retval = retval.Trim();
                    }

                    // check that the last two digits is not a zero
                    if (lasttwo > 0)
                    {
                        if (lasttwo < 20) // if less than 20, use "ones" only
                            retval = ones[lasttwo] + " " + retval;
                        else // otherwise, use both "tens" and "ones" array
                            retval = tens[dig2] + " " + ones[dig3] + " " + retval;
                    }

                    // if a hundreds part is there, translate it
                    if (dig1 > 0)
                        retval = ones[dig1] + " HUNDRED " + retval;

                    s = (s.Length - 3) > 0 ? s.Substring(0, s.Length - 3) : "";
                    level++;
                }

                while (retval.IndexOf("  ") > 0)
                    retval = retval.Replace("  ", " ");

                retval = retval.Trim();

                if (isNegative)
                    retval = "NEGATIVE " + retval;

                return (retval);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ValidateForDirectissue(CheckBox chkBox, TextBox txtBoxJobNo,TextBox txtIssueType)
        {
            try
            {
                if (txtIssueType.Text.Trim() == "JOB")
                {
                    chkBox.Checked = false;
                    txtBoxJobNo.Text = "Direct";
                }
                else
                {
                    chkBox.Checked = true;
                    //txtBox.Text=
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public double GetColumnTotalsFor_DuplicatedRowsInGrid(DataGridView dgvData, string KeyColumnName, string ValueColumnName, string KeyColumnValue, double ValueColumnValue)
        {
            string _KeyCode = "";
            double _KeyValue = 0;
            try
            {
                _KeyCode = KeyColumnValue;
                //_KeyValue = ValueColumnValue;

                foreach (DataGridViewRow dgvr in dgvData.Rows)
                {
                    if (dgvr.Cells[KeyColumnName].Value != null && dgvr.Cells[KeyColumnName].Value.ToString().Trim().Length > 0)
                    {
                        if (_KeyCode == dgvr.Cells[KeyColumnName].Value.ToString())
                            _KeyValue = _KeyValue + double.Parse(dgvr.Cells[ValueColumnName].Value.ToString());
                    }

                    //_KeyCode = dgvr.Cells[KeyColumnName].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _KeyValue;
        }

        private bool VendorValidation(string VendorID)
        {
            try
            {
                string StrSql = "SELECT VendorID FROM tblVendorMaster where VendorID='" + VendorID + "'";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
            return true;
        }


        private bool ItemValidation(string ItemID)
        {
            try
            {
                string StrSql = "SELECT ItemID FROM tblItemMaster where ItemID='" + ItemID + "'";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
            return true;
        }
        private bool JobValidation(string JobID)
        {
            try
            {
                string StrSql = "SELECT JobID FROM tblJobMaster where JobID='" + JobID + "'";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
            return true;
        }
        private bool SalesRepValidation(string SalesRep)
        {
            try
            {
                string StrSql = "SELECT RepName FROM tblSalesRep where RepName='" + SalesRep + "'";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
            return true;
        }
        private bool CustomerValidation(string Customer)
        {
            try
            {
                string StrSql = "SELECT CutomerID FROM tblCustomerMaster where CutomerID='" + Customer + "'";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
            return true;
        }

        private bool DepartmentValidation(string DepCode)
        {
            try
            {
                string StrSql = "SELECT DepartmentCode FROM tblDeparmentMaster where DepartmentCode='" + DepCode + "'";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
            return true;
        }

        private bool AccountValidation(string AccountID)
        {
            try
            {
                string StrSql = "SELECT AcountID FROM tblChartofAcounts where AcountID='" + AccountID + "'";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
            return true;
        }

        private bool WarehouseValidation(string WID)
        {
            try
            {
                string StrSql = "SELECT WhseId FROM tblWhseMaster where WhseId='" + WID + "'";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
            return true;
        }



        public bool HeaderValidation_Vendor(string Vendor,string Msg)
        {
            setConnectionString();           

            if(Vendor.Trim()!=string.Empty)
            {
                if (!VendorValidation(Vendor.ToString().Trim()))
                {
                    MessageBox.Show("Incorrect Vendor", Msg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Please Select  Vendor", Msg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }            
            return true;
        }

        public bool HeaderValidation_Customer(string Customer, string Msg)
        {
            setConnectionString();
            
            if (Customer.Trim() != string.Empty)
            {
                if (!CustomerValidation(Customer.ToString().Trim()))
                {
                    MessageBox.Show("Incorrect Customer", Msg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Please Select Customer", Msg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }            
            return true;
        }

        public bool HeaderValidation_AccountID(string AccountID, string Msg)
        {
            setConnectionString();            

            if (AccountID.Trim() != string.Empty)
            {
                if (!AccountValidation(AccountID.ToString().Trim()))
                {
                    MessageBox.Show("Incorrect Account", Msg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Account Empty", Msg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            
            return true;
        }


        public bool HeaderValidation_DepartmentCode(string DepCode, string Msg)
        {
            setConnectionString();

            if (DepCode.Trim() != string.Empty)
            {
                if (!DepartmentValidation(DepCode.ToString().Trim()))
                {
                    MessageBox.Show("Incorrect Account", Msg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Account Empty", Msg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            return true;
        }

        public bool HeaderValidation_Warehouse(string WHId, string Msg)
        {
            setConnectionString();

            if (WHId.Trim() != string.Empty)
            {
                if (!WarehouseValidation(WHId.Trim()))
                {
                    MessageBox.Show("Incorrect Warehouse", Msg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Please Select Warehouse", Msg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }            
            return true;
        }

        public bool HeaderValidation_SaleRep(string SaleRep, string Msg)
        {
            setConnectionString();           

            if (SaleRep.Trim() != string.Empty)
            {
                if (!SalesRepValidation(SaleRep.ToString().Trim()))
                {
                    MessageBox.Show("Incorrect SalesRep", Msg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Customer SalesRep", Msg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            return true;
        }

        public bool HeaderValidation_ItemID(string ItemID, string Msg)
        {
            //sanjeewa
            setConnectionString();

            if (ItemID.Trim() != string.Empty)
            {
                if (!ItemValidation(ItemID.ToString().Trim()))
                {
                    MessageBox.Show("Incorrect ItemID", Msg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Item ID", Msg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            return true;
        }

        public bool HeaderValidation_JobID(string JobID, string Msg)
        {
            //sanjeewa
            setConnectionString();

            if (JobID.Trim() != string.Empty)
            {
                if (!JobValidation(JobID.ToString().Trim()))
                {
                    MessageBox.Show("Incorrect Job ID", Msg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Incorrect Job ID", Msg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            return true;
        }

        public bool CheckSerialNo(int rowCount, UltraGrid dgv, string ItemHeaderName, string QtyHeaderName, DataTable dataTable)
        {
            try
            {
                setConnectionString();
                for (int a = 0; a < rowCount; a++)
                {
                    string ItemClass = "";
                    String S = "Select * from tblItemMaster where ItemID  = '" + dgv.Rows[a].Cells[ItemHeaderName].Value.ToString().Trim() + "'";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataSet dt = new DataSet();
                    da.Fill(dt);

                    if (dt.Tables[0].Rows.Count > 0)
                    {
                        ItemClass = dt.Tables[0].Rows[0].ItemArray[2].ToString();
                    }
                    if (ItemClass == "10" || ItemClass == "11")
                    {
                        double ItemCount = 0;
                        foreach (DataRow dr in clsSerializeItem.DtsSerialNoList.Rows)
                        {
                            if (dr["ItemCode"].ToString() == dgv.Rows[a].Cells[ItemHeaderName].Value.ToString().Trim())
                                ItemCount = ItemCount + 1;
                        }
                        if (ItemCount < double.Parse(dgv.Rows[a].Cells[QtyHeaderName].Value.ToString().Trim()))
                            return false;
                        else
                            return true;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
