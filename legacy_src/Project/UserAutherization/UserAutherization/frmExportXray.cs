using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;
using System.Collections;
using System.Data.SqlClient;
using System.Xml;
using UserAutherization;

namespace Perfect_Hospital_Management_System
{
    public partial class frmExportOPD : Form
    {
        public frmExportOPD()
        {
            InitializeComponent();
        }

        public static string ConnectionString;
        public void setConnectionString()
        {
            try
            {
                TextReader tr = new StreamReader("Connection.txt");
                ConnectionString = tr.ReadLine();
                tr.Close();
            }
            catch { }
        }

        private void btnok_Click(object sender, EventArgs e)
        {
            string Fdate = dtpFromDate.Text.ToString().Trim();//start date 
            string Edate = dtpToDate.Text.ToString().Trim();//End Date

            DataSet ReceiptExport = new DataSet();
            ReceiptExport = GetStoreScaning(Fdate, Edate);//get stored data from table tblscanchannel
            try
            {
                File.Delete(@"C:\Receipts2.xml");
            }
            catch
            {

            }
            writeReceiptXML(Fdate, Edate);//Create a Xml file for export
            try
            {
                Connector abc = new Connector();//export to peach tree
                abc.Import_Receipt_Journal();
            }

            catch { }
        }

        private void frmExportXray_Load(object sender, EventArgs e)
        {

        }

        private void dtpToDate_ValueChanged(object sender, EventArgs e)
        {
            string Fdate = dtpFromDate.Text.ToString().Trim();//Start Date 
            string Edate = dtpToDate.Text.ToString().Trim();//End Date

            try
            {
                DataSet Scan = new DataSet();
                Scan = GetStoreScaning(Fdate, Edate);

                ShowInfo(Scan);
            }
            catch { }
        }
        //................................
        public void ShowInfo(DataSet ds)
        {
            if (ds.Tables[0].Rows.Count == 0)
            {

            }
            else
            {

            }

            dgvImportCannel.Rows.Clear();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                dgvImportCannel.Rows.Add();
                dgvImportCannel.Rows[i].Cells[0].Value = ds.Tables[0].Rows[i].ItemArray[0].ToString();
                dgvImportCannel.Rows[i].Cells[1].Value = ds.Tables[0].Rows[i].ItemArray[1].ToString();
            }

        }
        //............................



        //===============================
        public DataSet GetStoreScaning(string StartDate, string EndDate)
        {
            setConnectionString();
            DataSet ds = new DataSet();
            try
            {

                string ConnString = ConnectionString;//ReceiptsNo ConsultantName
                // String S = "select * from tblScanChannel where RepDate >= '" + dtpitemsale1.Text.ToString().Trim() + "' AND RepDate <= '" + dtpitemsales2.Text.ToString().Trim() + "' AND ItemDescription = '" + cmbItem.Text.ToString().Trim() + "'AND Consultant = '" + cmbConsultant.Text.ToString().Trim() + "'";
                // string S2 = "select ReceiptsNo,ConsultantName from tblChannelingDetails where RepDate >= '" + StartDate + "' AND RepDate <= '" + EndDate + "' AND IsExport='false'";
                string S2 = "select distinct(ReceiptNo),Consultant from tblScanChannel where RepDate >= '" + StartDate + "' AND RepDate <= '" + EndDate + "' AND IsExport='false' AND ItemType='Xray'";
                //string S2 = "select ReceiptsNo,ConsultantName from tblChannelingDetails where RepDate between '" + StartDate + "' and '" + EndDate + "'  and IsExport='false'";
                SqlCommand cmd2 = new SqlCommand(S2);
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                da2.Fill(ds);

            }
            catch
            {

            }
            return ds;
        }
        //================================

        //=====================
        public ArrayList GetRepeatReceiptsForDateRange(string RepNO)
        {
            setConnectionString();
            ArrayList ReceiptListForDay = new ArrayList();

            //===================
            string ConnString = ConnectionString;//ReceiptsNo ConsultantName
            // string S2 = "select distinct(ReceiptsNo) from tblChannelingDetails where RepDate between '" + StartDate + "' and '" + EndDate + "'  and IsExport='false'";
            // string S2 = "select ReceiptNo from tblScanChannel where RepDate >= '" + StartDate + "' AND RepDate <= '" + EndDate + "' AND IsExport='false'";
            string S2 = "select ItemID from tblScanChannel where ReceiptNo= '" + RepNO + "'";// AND RepDate <= '" + EndDate + "' AND IsExport='false'";
            // string S2 = "select ReceiptsNo from tblChannelingDetails where RepDate between '" + StartDate + "' and '" + EndDate + "'  and IsExport='false'";
            SqlCommand cmd2 = new SqlCommand(S2);
            SqlConnection Conn = new SqlConnection(ConnString);
            cmd2.Connection = Conn;
            Conn.Open();
            SqlDataReader reader = cmd2.ExecuteReader();
            while (reader.Read())
            {
                ReceiptListForDay.Add(reader.GetValue(0));
            }
            reader.Close();
            Conn.Close();
            return ReceiptListForDay;//this is regading the scan

        }
        //=====================================
        public DataSet GetReceiptInfo(string RepNo)//this is regarding the Scan Details
        {
            setConnectionString();
            DataSet RepInfo = new DataSet();

            try
            {
                string ConnString = ConnectionString;//ReceiptsNo ConsultantName
                // string S2 = "select ReceiptsNo,ConsultantName from tblChannelingDetails where RepDate between '" + StartDate + "' and '" + EndDate + "'  and IsExport='false'";
                string S2 = "select *  from tblScanChannel where ReceiptNo='" + RepNo + "'";
                SqlCommand cmd2 = new SqlCommand(S2);
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                da2.Fill(RepInfo);

            }
            catch
            {
                MessageBox.Show(" No Purchase Order");
            }
            return RepInfo;
        }
        //==============================================================


        //==================================Create a Xml
        public void writeReceiptXML(string fromDate, string ToDate)
        {

            //=============================================
            double DisRate = 0.00;
            double donetAmount = 0.00;
            double testAmount = 0.00;

            double DoctorRefund = 0.00;
            double testrefundAmount = 0.00;


            DataSet ReceiptInfo = new DataSet();
            ArrayList DistinctReceipts = new ArrayList();
            ArrayList RepeatReceipts = new ArrayList();

            // RepeatReceipts = GetRepeatReceiptsForDateRange(fromDate, ToDate);

            DistinctReceipts = GetReceiptsForDateRange(fromDate, ToDate);


            try
            {
                XmlTextWriter Writer = new XmlTextWriter(@"c:\Receipts2.xml", System.Text.Encoding.UTF8);
                Writer.Formatting = Formatting.Indented;
                Writer.WriteStartElement("PAW_Receipts");
                Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                for (int j = 0; j < DistinctReceipts.Count; j++)//this is the distit receipt no
                {
                    RepeatReceipts = GetRepeatReceiptsForDateRange(DistinctReceipts[j].ToString().Trim());//DistinctReceipts[i].ToString().Trim()
                    // Writer.WriteStartElement("PAW_Receipt");
                    for (int i = 0; i < RepeatReceipts.Count; i++)//this is the distit receipt no
                    {
                        ReceiptInfo = GetReceiptInfo(DistinctReceipts[j].ToString().Trim());
                        //===================================================================================================
                        if (i == 0)
                        {
                            Writer.WriteStartElement("PAW_Receipt");
                            Writer.WriteAttributeString("xsi:type", "paw:Receipt");
                        }
                        //Writer.WriteStartElement("ReceiptNumber");
                        //Writer.WriteString(ReceiptInfo.Tables[0].Rows[i].ItemArray[0].ToString().Trim());//Receipts Number Should be here 
                        //Writer.WriteEndElement();//ReceiptInfo.Tables[0].Rows[0].ItemArray[0].ToString().Trim()

                        string CustomerName = ReceiptInfo.Tables[0].Rows[0].ItemArray[5].ToString().Trim();
                        string A = CustomerName.Substring(0, 1);
                        string distributionNo = "";

                        string ReferenceNo = ReceiptInfo.Tables[0].Rows[i].ItemArray[0].ToString().Trim() + "Scan";

                        string Postedrefund = ReceiptInfo.Tables[0].Rows[0].ItemArray[26].ToString().Trim();//if this flag= true then values 


                        Writer.WriteStartElement("Customer_ID");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(ReceiptInfo.Tables[0].Rows[0].ItemArray[30].ToString().Trim());//Customer ID should be here = Ptient No
                        Writer.WriteEndElement();




                        DateTime repdate = Convert.ToDateTime(ReceiptInfo.Tables[0].Rows[0].ItemArray[20].ToString().Trim());

                        string MM = Convert.ToString(repdate.Month);
                        string DD = Convert.ToString(repdate.Day);
                        string YYYY = Convert.ToString(repdate.Year);
                        string ImportDate = MM + "/" + DD + "/" + YYYY;




                        Writer.WriteStartElement("Reference");
                        Writer.WriteString(ReferenceNo);//Reference
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Date ");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        //  Writer.WriteString(dtpDateFrom.Text.ToString().Trim());//Date 
                        Writer.WriteString(ImportDate);//Date 
                        // 03/15/2007
                        Writer.WriteEndElement();



                        Writer.WriteStartElement("Payment_Method");
                        Writer.WriteString("Cash");//PayMethod
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Cash_Account");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString("70000");
                        // Writer.WriteString("70010");//Cash Account//10200-00
                        Writer.WriteEndElement();



                        Writer.WriteStartElement("Number_of_Distributions ");
                        if (RepeatReceipts.Count == 1)
                        {
                            distributionNo = "2";
                        }
                        if (RepeatReceipts.Count == 2)
                        {
                            distributionNo = "4";
                        }
                        if (RepeatReceipts.Count == 3)
                        {
                            distributionNo = "6";
                        }
                        if (RepeatReceipts.Count == 4)
                        {
                            distributionNo = "8";
                        }
                        if (RepeatReceipts.Count == 5)
                        {
                            distributionNo = "10";
                        }
                        if (RepeatReceipts.Count == 6)
                        {
                            distributionNo = "12";
                        }
                        if (RepeatReceipts.Count == 7)
                        {
                            distributionNo = "14";
                        }
                        if (RepeatReceipts.Count == 8)
                        {
                            distributionNo = "16";
                        }
                        if (RepeatReceipts.Count == 9)
                        {
                            distributionNo = "18";
                        }
                        if (RepeatReceipts.Count == 10)
                        {
                            distributionNo = "20";
                        }
                        if (RepeatReceipts.Count == 11)
                        {
                            distributionNo = "22";
                        }
                        if (RepeatReceipts.Count == 12)
                        {
                            distributionNo = "24";
                        }
                        if (RepeatReceipts.Count == 13)
                        {
                            distributionNo = "26";
                        }
                        if (RepeatReceipts.Count == 14)
                        {
                            distributionNo = "28";
                        }
                        if (RepeatReceipts.Count == 15)
                        {
                            distributionNo = "30";
                        }
                        if (RepeatReceipts.Count == 16)
                        {
                            distributionNo = "32";
                        }
                        if (RepeatReceipts.Count == 17)
                        {
                            distributionNo = "34";
                        }
                        if (RepeatReceipts.Count == 18)
                        {
                            distributionNo = "36";
                        }
                        if (RepeatReceipts.Count == 19)
                        {
                            distributionNo = "38";
                        }
                        if (RepeatReceipts.Count == 20)
                        {
                            distributionNo = "40";
                        }


                        Writer.WriteString(distributionNo);
                        // Writer.WriteString(Convert.ToString(RepeatReceipts.Count));
                        Writer.WriteEndElement();


                        Writer.WriteStartElement("InvoicePaid");
                        Writer.WriteString("");//PayMethod
                        Writer.WriteEndElement();



                        //........................................................

                        //Writer.WriteStartElement("Transaction_Number");
                        //Writer.WriteString("1");
                        //Writer.WriteEndElement();
                        //.....................................
                        //Writer.WriteStartElement("ItemID");
                        //Writer.WriteString(ReceiptInfo.Tables[0].Rows[i].ItemArray[3].ToString().Trim());
                        //Writer.WriteEndElement();

                        Writer.WriteStartElement("Description");
                        Writer.WriteString("Doctor Charges");
                        Writer.WriteEndElement();
                        //...........................
                        //Writer.WriteStartElement("Quantity");
                        //Writer.WriteString("1");//Doctor Charge
                        //Writer.WriteEndElement();
                        ////.................................

                        DisRate = Convert.ToDouble(ReceiptInfo.Tables[0].Rows[i].ItemArray[23].ToString().Trim()) / 100;
                        donetAmount = Convert.ToDouble(ReceiptInfo.Tables[0].Rows[i].ItemArray[13].ToString().Trim()) - (Convert.ToDouble(ReceiptInfo.Tables[0].Rows[i].ItemArray[13].ToString().Trim()) * DisRate);
                        //=======================================
                        testAmount = Convert.ToDouble(ReceiptInfo.Tables[0].Rows[i].ItemArray[14].ToString().Trim()) - (Convert.ToDouble(ReceiptInfo.Tables[0].Rows[i].ItemArray[14].ToString().Trim()) * DisRate);

                        if (Postedrefund == "True")
                        {

                            // DoctorRefund = Convert.ToDouble(ReceiptInfo.Tables[0].Rows[i].ItemArray[28].ToString().Trim()) - (Convert.ToDouble(ReceiptInfo.Tables[0].Rows[i].ItemArray[28].ToString().Trim()) * DisRate);

                            DoctorRefund = Convert.ToDouble(ReceiptInfo.Tables[0].Rows[i].ItemArray[28].ToString().Trim());
                            testrefundAmount = Convert.ToDouble(ReceiptInfo.Tables[0].Rows[i].ItemArray[29].ToString().Trim()) - (Convert.ToDouble(ReceiptInfo.Tables[0].Rows[i].ItemArray[29].ToString().Trim()) * DisRate);

                            if (testAmount == 0)
                            {
                                Writer.WriteStartElement("Amount");
                                Writer.WriteString(testrefundAmount.ToString().Trim());//HospitalCharge
                                Writer.WriteEndElement();
                            }
                            else
                            {
                                Writer.WriteStartElement("Amount");
                                // Writer.WriteString(RefundDotorcharges1.ToString().Trim());//HospitalCharge
                                Writer.WriteString("0");//HospitalCharge
                                Writer.WriteEndElement();
                            }
                        }

                        else
                        {
                            //=========================================================



                            // DoctorRefund = Convert.ToDouble(ReceiptInfo.Tables[0].Rows[i].ItemArray[13].ToString().Trim()) - (Convert.ToDouble(ReceiptInfo.Tables[0].Rows[i].ItemArray[13].ToString().Trim()) * DisRate);

                            Writer.WriteStartElement("Amount");
                            Writer.WriteString("-" + ReceiptInfo.Tables[0].Rows[i].ItemArray[13].ToString().Trim());//Doctor Charge
                            Writer.WriteEndElement();
                        }

                        Writer.WriteStartElement("GL_Account ");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString("76006");//Doctor payable Account 
                        // Writer.WriteString(dgvScanItems[2, i].Value.ToString().Trim());//Doctor payable Account 
                        Writer.WriteEndElement();


                        Writer.WriteStartElement("Transaction_Number");
                        Writer.WriteString("1");
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("ReceiptNumber");
                        Writer.WriteString(ReceiptInfo.Tables[0].Rows[i].ItemArray[0].ToString().Trim());//Receipts Number Should be here 
                        Writer.WriteEndElement();//ReceiptInfo.Tables[0].Rows[0].ItemArray[0].ToString().Trim()




                        //second part


                        Writer.WriteStartElement("Customer_ID");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(ReceiptInfo.Tables[0].Rows[0].ItemArray[30].ToString().Trim());//Customer ID should be here = Ptient No
                        Writer.WriteEndElement();


                        //===================================================================


                        Writer.WriteStartElement("Reference");
                        Writer.WriteString(ReferenceNo);//Reference
                        Writer.WriteEndElement();
                        //.......................................
                        Writer.WriteStartElement("Date ");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        //  Writer.WriteString(dtpDateFrom.Text.ToString().Trim());//Date 
                        // Writer.WriteString(ReceiptInfo.Tables[0].Rows[i].ItemArray[20].ToString().Trim());//Date 
                        Writer.WriteString(ImportDate);
                        // 03/15/2007
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Payment_Method");
                        Writer.WriteString("Cash");//PayMethod
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Cash_Account");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        // Writer.WriteString("70010");//Cash Account
                        Writer.WriteString("70000");
                        Writer.WriteEndElement();




                        Writer.WriteStartElement("Number_of_Distributions ");
                        if (RepeatReceipts.Count == 1)
                        {
                            distributionNo = "2";
                        }
                        if (RepeatReceipts.Count == 2)
                        {
                            distributionNo = "4";
                        }
                        if (RepeatReceipts.Count == 3)
                        {
                            distributionNo = "6";
                        }
                        if (RepeatReceipts.Count == 4)
                        {
                            distributionNo = "8";
                        }
                        if (RepeatReceipts.Count == 5)
                        {
                            distributionNo = "10";
                        }
                        if (RepeatReceipts.Count == 6)
                        {
                            distributionNo = "12";
                        }
                        if (RepeatReceipts.Count == 7)
                        {
                            distributionNo = "14";
                        }
                        if (RepeatReceipts.Count == 8)
                        {
                            distributionNo = "16";
                        }
                        if (RepeatReceipts.Count == 9)
                        {
                            distributionNo = "18";
                        }
                        if (RepeatReceipts.Count == 10)
                        {
                            distributionNo = "20";
                        }
                        if (RepeatReceipts.Count == 11)
                        {
                            distributionNo = "22";
                        }
                        if (RepeatReceipts.Count == 12)
                        {
                            distributionNo = "24";
                        }
                        if (RepeatReceipts.Count == 13)
                        {
                            distributionNo = "26";
                        }
                        if (RepeatReceipts.Count == 14)
                        {
                            distributionNo = "28";
                        }
                        if (RepeatReceipts.Count == 15)
                        {
                            distributionNo = "30";
                        }
                        if (RepeatReceipts.Count == 16)
                        {
                            distributionNo = "32";
                        }
                        if (RepeatReceipts.Count == 17)
                        {
                            distributionNo = "34";
                        }
                        if (RepeatReceipts.Count == 18)
                        {
                            distributionNo = "36";
                        }
                        if (RepeatReceipts.Count == 19)
                        {
                            distributionNo = "38";
                        }
                        if (RepeatReceipts.Count == 20)
                        {
                            distributionNo = "40";
                        }

                        Writer.WriteString(distributionNo);
                        // Writer.WriteString(Convert.ToString(RepeatReceipts.Count));
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("InvoicePaid");
                        Writer.WriteString("");//PayMethod
                        Writer.WriteEndElement();




                        Writer.WriteStartElement("Quantity");
                        Writer.WriteString("1");//Doctor Charge
                        Writer.WriteEndElement();



                        Writer.WriteStartElement("Item_ID");
                        Writer.WriteString(ReceiptInfo.Tables[0].Rows[i].ItemArray[3].ToString().Trim());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Description");
                        Writer.WriteString(ReceiptInfo.Tables[0].Rows[i].ItemArray[10].ToString().Trim());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("GL_Account ");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        // Writer.WriteString("40000-00");
                        Writer.WriteString(ReceiptInfo.Tables[0].Rows[i].ItemArray[11].ToString().Trim());
                        Writer.WriteEndElement();





                        //========================================================
                        Writer.WriteStartElement("Tax_Type");
                        Writer.WriteString("1");//Doctor Charge
                        Writer.WriteEndElement();

                        //=======================================================


                        testAmount = Convert.ToDouble(ReceiptInfo.Tables[0].Rows[i].ItemArray[14].ToString().Trim()) - (Convert.ToDouble(ReceiptInfo.Tables[0].Rows[i].ItemArray[14].ToString().Trim()) * DisRate);
                        if (Postedrefund == "True")
                        {
                            // RefundHospitalCarges1 = Convert.ToDouble(ReceiptInfo.Tables[0].Rows[0].ItemArray[33].ToString().Trim()) - (Convert.ToDouble(ReceiptInfo.Tables[0].Rows[0].ItemArray[33].ToString().Trim()) * DiscountRate);

                            if (donetAmount == 0)
                            {

                                Writer.WriteStartElement("Amount");
                                Writer.WriteString(DoctorRefund.ToString().Trim());//HospitalCharge
                                Writer.WriteEndElement();

                            }
                            else
                            {
                                Writer.WriteStartElement("Amount");
                                // Writer.WriteString(RefundHospitalCarges1.ToString().Trim());//HospitalCharge
                                Writer.WriteString("0");//HospitalCharge
                                Writer.WriteEndElement();
                            }

                            //Writer.WriteStartElement("Amount");
                            //Writer.WriteString(HoschaAmount.ToString().Trim());//Doctor Charge
                            // Writer.WriteEndElement();
                        }

                        else
                        {


                            //==============================
                            // testAmount =Convert.ToDouble(ReceiptInfo.Tables[0].Rows[i].ItemArray[14].ToString().Trim())-(Convert.ToDouble(ReceiptInfo.Tables[0].Rows[i].ItemArray[14].ToString().Trim())* DisRate);

                            Writer.WriteStartElement("Amount");
                            Writer.WriteString("-" + testAmount);//HospitalCharge
                            Writer.WriteEndElement();
                        }


                        Writer.WriteStartElement("Transaction_Number");
                        Writer.WriteString("1");
                        Writer.WriteEndElement();


                        Writer.WriteStartElement("ReceiptNumber");
                        Writer.WriteString(ReceiptInfo.Tables[0].Rows[i].ItemArray[0].ToString().Trim());//Receipts Number Should be here 
                        Writer.WriteEndElement();


                    }

                    Writer.WriteEndElement();
                    try
                    {
                        string Export = "True";
                        // string Exportold="False";
                        //String S = "Update tblChannelingDetails SET TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "', PatientNo  = '" + txtPatientNo.Text.ToString().Trim() + "', ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "', FirstName  = '" + txtFirstName.Text.ToString().Trim() + "', ConsultantType = '" + txtCategory.Text.ToString().Trim() + "', Date = '" + dtpDateFrom.Text.ToString() + "', Session = '" + cmbSession.Text.ToString().Trim() + "', Room = '" + txtRoom.Text.ToString().Trim() + "', ContactNO = '" + txtContactNo.Text.ToString().Trim() + "', HospitalFee = '" + txtHospitalCharge.Text.ToString().Trim() + "', DoctorCharge = '" + txtConsultantCharge.Text.ToString().Trim() + "', Total = '" + txtTotal.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', CHosCharge = '" + txtCHosCharge.Text.ToString().Trim() + "', CConCharge = '" + txtCConsCharge.Text.ToString().Trim() + "', CTotal = '" + txtCTotal.Text.ToString().Trim() + "', Rate = '" + txtRate.Text.ToString().Trim() + "',CurrentUser='" + CU + "',RepDate='" + dtpReceiptDate.Text.ToString().Trim() + "',Refund='" + Ref + "' WHERE ReceiptsNo = '" + txtReceiptNo.Text.ToString().Trim() + "'";
                        String S = "Update tblScanChannel SET IsExport = '" + Export + "' WHERE ReceiptNo = '" + DistinctReceipts[j].ToString().Trim() + "'";
                        SqlCommand cmd = new SqlCommand(S);
                        SqlConnection con = new SqlConnection(ConnectionString);
                        SqlDataAdapter da = new SqlDataAdapter(S, con);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        // MessageBox.Show("Refund Successfully");
                    }

                    catch { }




                    // ReceiptInfo = GetReceiptInfo(DistinctReceipts[j].ToString().Trim());
                }
                Writer.Close();

            }
            catch { }
            //==============================================================
        //    double DisRate = 0.00;
        //    double donetAmount = 0.00;
        //    double testAmount = 0.00;

        //    double DoctorRefund = 0.00;
        //    double testrefundAmount = 0.00;


        //    DataSet ReceiptInfo = new DataSet();
        //    ArrayList DistinctReceipts = new ArrayList();
        //    ArrayList RepeatReceipts = new ArrayList();

        //    // RepeatReceipts = GetRepeatReceiptsForDateRange(fromDate, ToDate);

        //    DistinctReceipts = GetReceiptsForDateRange(fromDate, ToDate);


        //    try
        //    {
        //        XmlTextWriter Writer = new XmlTextWriter(@"c:\Receipts2.xml", System.Text.Encoding.UTF8);
        //        Writer.Formatting = Formatting.Indented;
        //        Writer.WriteStartElement("PAW_Receipts");
        //        Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
        //        Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
        //        Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

        //        for (int j = 0; j < DistinctReceipts.Count; j++)//this is the distit receipt no
        //        {
        //            RepeatReceipts = GetRepeatReceiptsForDateRange(DistinctReceipts[j].ToString().Trim());//DistinctReceipts[i].ToString().Trim()
        //            // Writer.WriteStartElement("PAW_Receipt");
        //            for (int i = 0; i < RepeatReceipts.Count; i++)//this is the distit receipt no
        //            {
        //                ReceiptInfo = GetReceiptInfo(DistinctReceipts[j].ToString().Trim());
        //                //===================================================================================================
        //                if (i == 0)
        //                {
        //                    Writer.WriteStartElement("PAW_Receipt");
        //                    Writer.WriteAttributeString("xsi:type", "paw:Receipt");
        //                }
        //                //Writer.WriteStartElement("ReceiptNumber");
        //                //Writer.WriteString(ReceiptInfo.Tables[0].Rows[i].ItemArray[0].ToString().Trim());//Receipts Number Should be here 
        //                //Writer.WriteEndElement();//ReceiptInfo.Tables[0].Rows[0].ItemArray[0].ToString().Trim()

        //                string CustomerName = ReceiptInfo.Tables[0].Rows[0].ItemArray[5].ToString().Trim();
        //                string A = CustomerName.Substring(0, 1);
        //                string distributionNo = "";

        //                string ReferenceNo = ReceiptInfo.Tables[0].Rows[i].ItemArray[0].ToString().Trim() + "Scan";

        //                string Postedrefund = ReceiptInfo.Tables[0].Rows[0].ItemArray[26].ToString().Trim();//if this flag= true then values 

        //                // if(txtFirstName.Text==
        //                if (A == "a" || A == "A")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CA0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "b" || A == "B")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CB0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "c" || A == "C")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CC0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "d" || A == "D")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CD0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "e" || A == "E")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CE0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "f" || A == "F")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CF0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "g" || A == "G")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CG0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "h" || A == "H")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CH0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "i" || A == "I")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CI0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "j" || A == "J")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CJ0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "k" || A == "K")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CK0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "l" || A == "L")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CL0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "m" || A == "M")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CM0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "n" || A == "N")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CN0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "o" || A == "O")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CO0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "p" || A == "P")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CP0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "q" || A == "Q")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CQ0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "r" || A == "R")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CR0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "s" || A == "S")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CS0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "t" || A == "T")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CT0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "u" || A == "U")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CU0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "v" || A == "V")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CV0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "w" || A == "W")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CW0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "x" || A == "X")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CX0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "y" || A == "Y")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CY0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "z" || A == "Z")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CZ0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("C0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                //====================================================================================
        //                //.......................................

        //                DateTime repdate = Convert.ToDateTime(ReceiptInfo.Tables[0].Rows[0].ItemArray[20].ToString().Trim());

        //                string MM = Convert.ToString(repdate.Month);
        //                string DD = Convert.ToString(repdate.Day);
        //                string YYYY = Convert.ToString(repdate.Year);
        //                string ImportDate = MM + "/" + DD + "/" + YYYY;




        //                Writer.WriteStartElement("Reference");
        //                Writer.WriteString(ReferenceNo);//Reference
        //                Writer.WriteEndElement();

        //                Writer.WriteStartElement("Date ");
        //                Writer.WriteAttributeString("xsi:type", "paw:id");
        //                //  Writer.WriteString(dtpDateFrom.Text.ToString().Trim());//Date 
        //                Writer.WriteString(ImportDate);//Date 
        //                // 03/15/2007
        //                Writer.WriteEndElement();



        //                Writer.WriteStartElement("Payment_Method");
        //                Writer.WriteString("Cash");//PayMethod
        //                Writer.WriteEndElement();

        //                Writer.WriteStartElement("Cash_Account");
        //                Writer.WriteAttributeString("xsi:type", "paw:id");
        //                Writer.WriteString("70000");
        //                // Writer.WriteString("70010");//Cash Account//10200-00
        //                Writer.WriteEndElement();



        //                Writer.WriteStartElement("Number_of_Distributions ");
        //                if (RepeatReceipts.Count == 1)
        //                {
        //                    distributionNo = "2";
        //                }
        //                if (RepeatReceipts.Count == 2)
        //                {
        //                    distributionNo = "4";
        //                }
        //                if (RepeatReceipts.Count == 3)
        //                {
        //                    distributionNo = "6";
        //                }
        //                if (RepeatReceipts.Count == 4)
        //                {
        //                    distributionNo = "8";
        //                }
        //                if (RepeatReceipts.Count == 5)
        //                {
        //                    distributionNo = "10";
        //                }
        //                if (RepeatReceipts.Count == 6)
        //                {
        //                    distributionNo = "12";
        //                }
        //                if (RepeatReceipts.Count == 7)
        //                {
        //                    distributionNo = "14";
        //                }
        //                if (RepeatReceipts.Count == 8)
        //                {
        //                    distributionNo = "16";
        //                }
        //                if (RepeatReceipts.Count == 9)
        //                {
        //                    distributionNo = "18";
        //                }
        //                if (RepeatReceipts.Count == 10)
        //                {
        //                    distributionNo = "20";
        //                }
        //                if (RepeatReceipts.Count == 11)
        //                {
        //                    distributionNo = "22";
        //                }
        //                if (RepeatReceipts.Count == 12)
        //                {
        //                    distributionNo = "24";
        //                }
        //                if (RepeatReceipts.Count == 13)
        //                {
        //                    distributionNo = "26";
        //                }
        //                if (RepeatReceipts.Count == 14)
        //                {
        //                    distributionNo = "28";
        //                }
        //                if (RepeatReceipts.Count == 15)
        //                {
        //                    distributionNo = "30";
        //                }
        //                if (RepeatReceipts.Count == 16)
        //                {
        //                    distributionNo = "32";
        //                }
        //                if (RepeatReceipts.Count == 17)
        //                {
        //                    distributionNo = "34";
        //                }
        //                if (RepeatReceipts.Count == 18)
        //                {
        //                    distributionNo = "36";
        //                }
        //                if (RepeatReceipts.Count == 19)
        //                {
        //                    distributionNo = "38";
        //                }
        //                if (RepeatReceipts.Count == 20)
        //                {
        //                    distributionNo = "40";
        //                }


        //                Writer.WriteString(distributionNo);
        //                // Writer.WriteString(Convert.ToString(RepeatReceipts.Count));
        //                Writer.WriteEndElement();


        //                Writer.WriteStartElement("InvoicePaid");
        //                Writer.WriteString("");//PayMethod
        //                Writer.WriteEndElement();



        //                //........................................................

        //                //Writer.WriteStartElement("Transaction_Number");
        //                //Writer.WriteString("1");
        //                //Writer.WriteEndElement();
        //                //.....................................
        //                //Writer.WriteStartElement("ItemID");
        //                //Writer.WriteString(ReceiptInfo.Tables[0].Rows[i].ItemArray[3].ToString().Trim());
        //                //Writer.WriteEndElement();

        //                Writer.WriteStartElement("Description");
        //                Writer.WriteString("Doctor Charges");
        //                Writer.WriteEndElement();
        //                //...........................
        //                //Writer.WriteStartElement("Quantity");
        //                //Writer.WriteString("1");//Doctor Charge
        //                //Writer.WriteEndElement();
        //                ////.................................

        //                DisRate = Convert.ToDouble(ReceiptInfo.Tables[0].Rows[i].ItemArray[23].ToString().Trim()) / 100;
        //                donetAmount = Convert.ToDouble(ReceiptInfo.Tables[0].Rows[i].ItemArray[13].ToString().Trim()) - (Convert.ToDouble(ReceiptInfo.Tables[0].Rows[i].ItemArray[13].ToString().Trim()) * DisRate);
        //                //=======================================

        //                if (Postedrefund == "True")
        //                {

        //                    DoctorRefund = Convert.ToDouble(ReceiptInfo.Tables[0].Rows[i].ItemArray[28].ToString().Trim()) - (Convert.ToDouble(ReceiptInfo.Tables[0].Rows[i].ItemArray[28].ToString().Trim()) * DisRate);
        //                    testrefundAmount = Convert.ToDouble(ReceiptInfo.Tables[0].Rows[i].ItemArray[29].ToString().Trim()) - (Convert.ToDouble(ReceiptInfo.Tables[0].Rows[i].ItemArray[29].ToString().Trim()) * DisRate);

        //                    if (donetAmount == 0)
        //                    {
        //                        Writer.WriteStartElement("Amount");
        //                        Writer.WriteString(testrefundAmount.ToString().Trim());//HospitalCharge
        //                        Writer.WriteEndElement();
        //                    }
        //                    else
        //                    {
        //                        Writer.WriteStartElement("Amount");
        //                        // Writer.WriteString(RefundDotorcharges1.ToString().Trim());//HospitalCharge
        //                        Writer.WriteString("0");//HospitalCharge
        //                        Writer.WriteEndElement();
        //                    }
        //                }

        //                else
        //                {
        //                    //=========================================================



        //                    // DoctorRefund = Convert.ToDouble(ReceiptInfo.Tables[0].Rows[i].ItemArray[13].ToString().Trim()) - (Convert.ToDouble(ReceiptInfo.Tables[0].Rows[i].ItemArray[13].ToString().Trim()) * DisRate);

        //                    Writer.WriteStartElement("Amount");
        //                    Writer.WriteString("-" + donetAmount);//Doctor Charge
        //                    Writer.WriteEndElement();
        //                }

        //                Writer.WriteStartElement("GL_Account ");
        //                Writer.WriteAttributeString("xsi:type", "paw:id");
        //                Writer.WriteString("76005");//Doctor payable Account 
        //                // Writer.WriteString(dgvScanItems[2, i].Value.ToString().Trim());//Doctor payable Account 
        //                Writer.WriteEndElement();


        //                Writer.WriteStartElement("Transaction_Number");
        //                Writer.WriteString("1");
        //                Writer.WriteEndElement();

        //                Writer.WriteStartElement("ReceiptNumber");
        //                Writer.WriteString(ReceiptInfo.Tables[0].Rows[i].ItemArray[0].ToString().Trim());//Receipts Number Should be here 
        //                Writer.WriteEndElement();//ReceiptInfo.Tables[0].Rows[0].ItemArray[0].ToString().Trim()


        //                //................taxype

        //                //Writer.WriteStartElement("Tax_Type");
        //                //Writer.WriteString("1");//Doctor Charge
        //                //Writer.WriteEndElement();



        //                //second part
        //                //=======================================================================================2222
        //                //Writer.WriteStartElement("ReceiptNumber");
        //                //Writer.WriteString(ReceiptInfo.Tables[0].Rows[i].ItemArray[0].ToString().Trim());//Receipts Number Should be here 
        //                //Writer.WriteEndElement();
        //                //=====================================================
        //                //=================================================
        //                // string CustomerName = txtFirstName.Text;
        //                //string A = CustomerName.Substring(0, 1);
        //                // if(txtFirstName.Text==
        //                if (A == "a" || A == "A")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CA0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "b" || A == "B")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CB0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "c" || A == "C")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CC0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "d" || A == "D")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CD0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "e" || A == "E")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CE0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "f" || A == "F")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CF0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "g" || A == "G")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CG0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "h" || A == "H")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CH0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "i" || A == "I")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CI0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "j" || A == "J")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CJ0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "k" || A == "K")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CK0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "l" || A == "L")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CL0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "m" || A == "M")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CM0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "n" || A == "N")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CN0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "o" || A == "O")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CO0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "p" || A == "P")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CP0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "q" || A == "Q")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CQ0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "r" || A == "R")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CR0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "s" || A == "S")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CS0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "t" || A == "T")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CT0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "u" || A == "U")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CU0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "v" || A == "V")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CV0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "w" || A == "W")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CW0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "x" || A == "X")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CX0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "y" || A == "Y")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CY0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else if (A == "z" || A == "Z")
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("CZ0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                else
        //                {
        //                    Writer.WriteStartElement("Customer_ID");
        //                    Writer.WriteAttributeString("xsi:type", "paw:id");
        //                    Writer.WriteString("C0001");//Customer ID should be here = Ptient No
        //                    Writer.WriteEndElement();
        //                }
        //                //===================================================================


        //                Writer.WriteStartElement("Reference");
        //                Writer.WriteString(ReferenceNo);//Reference
        //                Writer.WriteEndElement();
        //                //.......................................
        //                Writer.WriteStartElement("Date ");
        //                Writer.WriteAttributeString("xsi:type", "paw:id");
        //                //  Writer.WriteString(dtpDateFrom.Text.ToString().Trim());//Date 
        //                // Writer.WriteString(ReceiptInfo.Tables[0].Rows[i].ItemArray[20].ToString().Trim());//Date 
        //                Writer.WriteString(ImportDate);
        //                // 03/15/2007
        //                Writer.WriteEndElement();

        //                Writer.WriteStartElement("Payment_Method");
        //                Writer.WriteString("Cash");//PayMethod
        //                Writer.WriteEndElement();

        //                Writer.WriteStartElement("Cash_Account");
        //                Writer.WriteAttributeString("xsi:type", "paw:id");
        //                // Writer.WriteString("70010");//Cash Account
        //                Writer.WriteString("70000");
        //                Writer.WriteEndElement();




        //                //....................
        //                //Writer.WriteStartElement("Number_of_Distributions ");
        //                //Writer.WriteString(Convert.ToString(dgvScanItems.Rows.Count - 1));
        //                //Writer.WriteEndElement();



        //                Writer.WriteStartElement("Number_of_Distributions ");
        //                if (RepeatReceipts.Count == 1)
        //                {
        //                    distributionNo = "2";
        //                }
        //                if (RepeatReceipts.Count == 2)
        //                {
        //                    distributionNo = "4";
        //                }
        //                if (RepeatReceipts.Count == 3)
        //                {
        //                    distributionNo = "6";
        //                }
        //                if (RepeatReceipts.Count == 4)
        //                {
        //                    distributionNo = "8";
        //                }
        //                if (RepeatReceipts.Count == 5)
        //                {
        //                    distributionNo = "10";
        //                }
        //                if (RepeatReceipts.Count == 6)
        //                {
        //                    distributionNo = "12";
        //                }
        //                if (RepeatReceipts.Count == 7)
        //                {
        //                    distributionNo = "14";
        //                }
        //                if (RepeatReceipts.Count == 8)
        //                {
        //                    distributionNo = "16";
        //                }
        //                if (RepeatReceipts.Count == 9)
        //                {
        //                    distributionNo = "18";
        //                }
        //                if (RepeatReceipts.Count == 10)
        //                {
        //                    distributionNo = "20";
        //                }
        //                if (RepeatReceipts.Count == 11)
        //                {
        //                    distributionNo = "22";
        //                }
        //                if (RepeatReceipts.Count == 12)
        //                {
        //                    distributionNo = "24";
        //                }
        //                if (RepeatReceipts.Count == 13)
        //                {
        //                    distributionNo = "26";
        //                }
        //                if (RepeatReceipts.Count == 14)
        //                {
        //                    distributionNo = "28";
        //                }
        //                if (RepeatReceipts.Count == 15)
        //                {
        //                    distributionNo = "30";
        //                }
        //                if (RepeatReceipts.Count == 16)
        //                {
        //                    distributionNo = "32";
        //                }
        //                if (RepeatReceipts.Count == 17)
        //                {
        //                    distributionNo = "34";
        //                }
        //                if (RepeatReceipts.Count == 18)
        //                {
        //                    distributionNo = "36";
        //                }
        //                if (RepeatReceipts.Count == 19)
        //                {
        //                    distributionNo = "38";
        //                }
        //                if (RepeatReceipts.Count == 20)
        //                {
        //                    distributionNo = "40";
        //                }

        //                Writer.WriteString(distributionNo);
        //                // Writer.WriteString(Convert.ToString(RepeatReceipts.Count));
        //                Writer.WriteEndElement();

        //                Writer.WriteStartElement("InvoicePaid");
        //                Writer.WriteString("");//PayMethod
        //                Writer.WriteEndElement();



        //                //Writer.WriteStartElement("Number_of_Distributions ");
        //                //Writer.WriteString(Convert.ToString(RepeatReceipts.Count));
        //                //Writer.WriteEndElement();

        //                Writer.WriteStartElement("Quantity");
        //                Writer.WriteString("1");//Doctor Charge
        //                Writer.WriteEndElement();



        //                Writer.WriteStartElement("Item_ID");
        //                Writer.WriteString(ReceiptInfo.Tables[0].Rows[i].ItemArray[3].ToString().Trim());
        //                Writer.WriteEndElement();

        //                Writer.WriteStartElement("Description");
        //                Writer.WriteString(ReceiptInfo.Tables[0].Rows[i].ItemArray[10].ToString().Trim());
        //                Writer.WriteEndElement();

        //                Writer.WriteStartElement("GL_Account ");
        //                Writer.WriteAttributeString("xsi:type", "paw:id");
        //                // Writer.WriteString("40000-00");
        //                Writer.WriteString(ReceiptInfo.Tables[0].Rows[i].ItemArray[11].ToString().Trim());
        //                Writer.WriteEndElement();





        //                //========================================================
        //                Writer.WriteStartElement("Tax_Type");
        //                Writer.WriteString("1");//Doctor Charge
        //                Writer.WriteEndElement();

        //                //=======================================================


        //                testAmount = Convert.ToDouble(ReceiptInfo.Tables[0].Rows[i].ItemArray[14].ToString().Trim()) - (Convert.ToDouble(ReceiptInfo.Tables[0].Rows[i].ItemArray[14].ToString().Trim()) * DisRate);
        //                if (Postedrefund == "True")
        //                {
        //                    // RefundHospitalCarges1 = Convert.ToDouble(ReceiptInfo.Tables[0].Rows[0].ItemArray[33].ToString().Trim()) - (Convert.ToDouble(ReceiptInfo.Tables[0].Rows[0].ItemArray[33].ToString().Trim()) * DiscountRate);

        //                    if (testAmount == 0)
        //                    {

        //                        Writer.WriteStartElement("Amount");
        //                        Writer.WriteString(DoctorRefund.ToString().Trim());//HospitalCharge
        //                        Writer.WriteEndElement();

        //                    }
        //                    else
        //                    {
        //                        Writer.WriteStartElement("Amount");
        //                        // Writer.WriteString(RefundHospitalCarges1.ToString().Trim());//HospitalCharge
        //                        Writer.WriteString("0");//HospitalCharge
        //                        Writer.WriteEndElement();
        //                    }

        //                    //Writer.WriteStartElement("Amount");
        //                    //Writer.WriteString(HoschaAmount.ToString().Trim());//Doctor Charge
        //                    // Writer.WriteEndElement();
        //                }

        //                else
        //                {


        //                    //==============================
        //                    // testAmount =Convert.ToDouble(ReceiptInfo.Tables[0].Rows[i].ItemArray[14].ToString().Trim())-(Convert.ToDouble(ReceiptInfo.Tables[0].Rows[i].ItemArray[14].ToString().Trim())* DisRate);

        //                    Writer.WriteStartElement("Amount");
        //                    Writer.WriteString("-" + testAmount);//HospitalCharge
        //                    Writer.WriteEndElement();
        //                }


        //                Writer.WriteStartElement("Transaction_Number");
        //                Writer.WriteString("1");
        //                Writer.WriteEndElement();


        //                Writer.WriteStartElement("ReceiptNumber");
        //                Writer.WriteString(ReceiptInfo.Tables[0].Rows[i].ItemArray[0].ToString().Trim());//Receipts Number Should be here 
        //                Writer.WriteEndElement();


        //            }

        //            Writer.WriteEndElement();
        //            try
        //            {
        //                string Export = "True";
        //                // string Exportold="False";
        //                //String S = "Update tblChannelingDetails SET TokenNo = '" + txtTokenNo.Text.ToString().Trim() + "', PatientNo  = '" + txtPatientNo.Text.ToString().Trim() + "', ConsultantName = '" + txtConsultant.Text.ToString().Trim() + "', FirstName  = '" + txtFirstName.Text.ToString().Trim() + "', ConsultantType = '" + txtCategory.Text.ToString().Trim() + "', Date = '" + dtpDateFrom.Text.ToString() + "', Session = '" + cmbSession.Text.ToString().Trim() + "', Room = '" + txtRoom.Text.ToString().Trim() + "', ContactNO = '" + txtContactNo.Text.ToString().Trim() + "', HospitalFee = '" + txtHospitalCharge.Text.ToString().Trim() + "', DoctorCharge = '" + txtConsultantCharge.Text.ToString().Trim() + "', Total = '" + txtTotal.Text.ToString().Trim() + "', PaymentMethod = '" + cmbPaymentMethod.Text.ToString().Trim() + "', Remarks = '" + txtRemarks.Text.ToString().Trim() + "', CreditCardNo = '" + txtCreditCardNo.Text.ToString().Trim() + "', CHosCharge = '" + txtCHosCharge.Text.ToString().Trim() + "', CConCharge = '" + txtCConsCharge.Text.ToString().Trim() + "', CTotal = '" + txtCTotal.Text.ToString().Trim() + "', Rate = '" + txtRate.Text.ToString().Trim() + "',CurrentUser='" + CU + "',RepDate='" + dtpReceiptDate.Text.ToString().Trim() + "',Refund='" + Ref + "' WHERE ReceiptsNo = '" + txtReceiptNo.Text.ToString().Trim() + "'";
        //                String S = "Update tblScanChannel SET IsExport = '" + Export + "' WHERE ReceiptNo = '" + DistinctReceipts[j].ToString().Trim() + "'";
        //                SqlCommand cmd = new SqlCommand(S);
        //                SqlConnection con = new SqlConnection(ConnectionString);
        //                SqlDataAdapter da = new SqlDataAdapter(S, con);
        //                DataTable dt = new DataTable();
        //                da.Fill(dt);
        //                // MessageBox.Show("Refund Successfully");
        //            }

        //            catch { }




        //            // ReceiptInfo = GetReceiptInfo(DistinctReceipts[j].ToString().Trim());
        //        }
        //        Writer.Close();

        //    }
        //    catch { }
        }

        //============================================

        //================================
        public ArrayList GetReceiptsForDateRange(string StartDate, string EndDate)
        {
            setConnectionString();
            ArrayList ReceiptListForDay = new ArrayList();

            //===================
            string ConnString = ConnectionString;//ReceiptsNo ConsultantName
            // string S2 = "select distinct(ReceiptsNo) from tblChannelingDetails where RepDate between '" + StartDate + "' and '" + EndDate + "'  and IsExport='false'";
            string S2 = "select distinct(ReceiptNo) from tblScanChannel where RepDate >= '" + StartDate + "' AND RepDate <= '" + EndDate + "' AND IsExport='false' AND ItemType='Xray'";
            // string S2 = "select ReceiptsNo from tblChannelingDetails where RepDate between '" + StartDate + "' and '" + EndDate + "'  and IsExport='false'";
            SqlCommand cmd2 = new SqlCommand(S2);
            SqlConnection Conn = new SqlConnection(ConnString);
            cmd2.Connection = Conn;
            Conn.Open();
            SqlDataReader reader = cmd2.ExecuteReader();
            while (reader.Read())
            {
                ReceiptListForDay.Add(reader.GetValue(0));
            }
            reader.Close();
            Conn.Close();
            return ReceiptListForDay;//this is regading the scan

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //=============================

    }
}