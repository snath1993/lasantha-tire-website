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
using System.Xml;
using System.Collections;
using System.Threading;

namespace UserAutherization
{
    public partial class frmCusReturnSerial : Form
    {
        public frmCusReturnSerial()
        {
            InitializeComponent();
            setConnectionString();
        }
        public static string ConnectionString;
        SerialNumber sn = new SerialNumber();

        ClassDriiDown ObjGetitem = new ClassDriiDown();
        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;

            }
            catch { }
        }
        public static int NoOfItems = 0;
        public static int RemoveSno = -1;
        public static string Link1 = "";
        public static int OrdedQty = 0;
        public static string GRN_NO = "";


        ArrayList arrSNumbers = new ArrayList();

       

        public void SetLink(string GRNType)
        {
            Link1 = GRNType;
        }
        public void SetGRN(string GRNNO)
        {
            GRN_NO = GRNNO;
        }


        private void btn_Add_Click(object sender, EventArgs e)
        {
            try
            {
                int rowCount = ChklistBOX.Items.Count;
                bool IsDuplicate = false;

                for (int j = 0; j < rowCount; j++)
                {
                    if (textBox3.Text == ChklistBOX.Items[j].ToString())
                    {
                        IsDuplicate = true;
                    }
                }
                int NumberOfSno = Convert.ToInt32(numericUpDown1.Value);
                string SerialNumber = textBox3.Text.Trim();
                if (SerialNumber != "" && IsDuplicate==false)
                {
                    if (numericUpDown1.Enabled == true)
                    {
                        ChklistBOX.Items.Clear();
                        arrSNumbers.Clear();

                        NoOfItems = 0;
                        RemoveSno = -1;

                        if (numericUpDown1.Value > NoOfItems)
                        {
                            if (textBox3.Text != "")
                            {
                                for (int i = 0; i < NumberOfSno; i++)
                                {
                                    SerialNumber = getNextID(SerialNumber);
                                    ChklistBOX.Items.Add(SerialNumber);
                                    arrSNumbers.Add(SerialNumber);
                                    NoOfItems++;
                                }


                            }

                        }
                        else
                        {
                            // MessageBox.Show(" Error  ");
                        }
                        btn_Add.Focus();
                    }

                    else
                    {
                        if (numericUpDown1.Value > NoOfItems || numericUpDown1.Enabled == false)
                        {
                            if (textBox3.Text != "")
                            {
                                ChklistBOX.Items.Add(SerialNumber);
                                arrSNumbers.Add(SerialNumber);
                                NoOfItems++;
                                textBox3.Text = null;
                            }

                        }
                        else
                        {
                            // MessageBox.Show(" Error  ");
                        }
                        btn_Add.Focus();
                    }
                }
                else
                {
                    if (IsDuplicate == true)
                    {
                        MessageBox.Show("Duplicate Serial number no allowed for same Item");
                    }
                    if (textBox3.Text == "")
                    {
                        MessageBox.Show("Please Enter Serial Number ");
                    }
                }
            }
            catch
            {
                MessageBox.Show(" Please enter 20 numeric numbers");
            }
            AvoidActivated = true;
        }

        private void Add_Serial_Numbers_Load(object sender, EventArgs e)
        {
        }

        private void save_Click(object sender, EventArgs e)
        {
            int rowCount = ChklistBOX.CheckedItems.Count;
            if (rowCount == 0 || (rowCount != RecQuantity))
            {
                if (rowCount == 0)
                {
                    MessageBox.Show("Please Select Serial Number");
                }
                else if (rowCount != RecQuantity)
                {
                    MessageBox.Show("The number of serial numbers selected for this Item must match the Return Quantity");
                    //MessageBox.Show("You have entered more serial numbers than order Quantity");
                }
                AvoidActivated = true;
            }
            else
            {
                SqlConnection myConnection = new SqlConnection(ConnectionString);
                myConnection.Open();
                SqlTransaction myTrans = myConnection.BeginTransaction();
                try
                {

                    SqlCommand myCommand21 = new SqlCommand("delete from tblSerialCusReturnTemp where ItemID = '" + txtItemID.Text.ToString().Trim() + "'", myConnection, myTrans);
                    myCommand21.ExecuteNonQuery();
                    //============================================
                    for (int i = 0; i < rowCount; i++)
                    {
                        bool IsGRNProcess = false;
                        string TranType = "GRN";
                        SqlCommand myCommand2 = new SqlCommand("insert into tblSerialCusReturnTemp(ItemID,Description,SerialNO,TransactionType,IsGRNProcess)values ('" + txtItemID.Text.ToString().Trim() + "','" + txtDescription.Text.ToString().Trim() + "','" + ChklistBOX.CheckedItems[i].ToString() + "','" + TranType + "','" + IsGRNProcess + "')", myConnection, myTrans);
                        myCommand2.ExecuteNonQuery();
                       
                    }
                    myTrans.Commit();
                    this.Close();
                }
                catch (Exception ex)
                {
                    myTrans.Rollback();
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    myConnection.Close();
                }

            }

        }

        private void btn_Clear_Click(object sender, EventArgs e)
        {
            ChklistBOX.Items.Clear();
            NoOfItems = 0;
            //RemoveSno = -1;
            textBox3.Text = "";
            numericUpDown1.Value = 0;
           // checkBox1.Checked = false;
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            try
            {
                if (RemoveSno != -1)
                {
                    ChklistBOX.Items.RemoveAt(RemoveSno);
                    NoOfItems--;
                }
            }
            catch
            { return; }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                numericUpDown1.Enabled = true;
            }
            else
            {
                numericUpDown1.Enabled = false;
            }
        }


        public string getNextID(string s)
        {
            int i = 0;
            string nextID = "";
            while (i < s.Length - 1)
            {
                if ((Char.IsDigit(s[i]) && Char.IsLetter(s[i + 1])) || (Char.IsLetter(s[i]) && Char.IsDigit(s[i + 1]) || ((s[i] == '-')) || ((s[i] == ' '))))
                {
                    s = s.Insert(i + 1, "*");
                }
                i++;
            }
            bool Islarge = false;
            string[] arr = s.Split('*');
            i = arr.Length - 1;
            for (int no = i; no >= 0; no--)
            {
                if (arr[i].Length > 19)
                {
                    Islarge = true;
                }
                else
                {
                    Islarge = false;
                }
            }
            if (Islarge == false)
            {

                while (i >= 0)
                {
                    try
                    {
                        //if (arr[i].Length<=19)
                        //{
                        long no = long.Parse(arr[i]);
                        i = 0;
                        while (i < arr.Length)
                        {
                            if (arr[i] == no.ToString())
                            {
                                no++;
                                arr[i] = no.ToString();
                            }
                            nextID = nextID + arr[i];
                            i++;
                        }
                        return nextID;

                    }
                    catch { }
                    i--;
                }
                return s + "1";
            }
            else
            {
                //  MessageBox.Show(" Please Enter Number of digit less than 20");
                return null;
            }

        }

        private void listBox1_Click(object sender, EventArgs e)
        {
            RemoveSno = ChklistBOX.SelectedIndex;
            if (ChklistBOX.SelectedIndex >= 0)
            {
                btn_Delete.Enabled = true;
                btn_Delete.Focus();
            }
            else
            {
                btn_Delete.Enabled = false;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        string GRNNO = "";
        string ItemID = "";
        string Description = "";
        double RecQuantity = 0.0;
        bool IsCheckSearch = false;

        //frmGRN ObjGRN = new frmGRN();
        bool AvoidActivated=false;
        //private void Add_Serial_Numbers_Activated(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (AvoidActivated == false)
        //        {
        //            listBox1.Items.Clear();
        //            IsCheckSearch = ObjGetitem.GetCheckSearchToForm();
        //        //If this flag is true it sayas that serching item
        //            if (IsCheckSearch == true)
        //            {

        //                GRNNO = ObjGetitem.GetGRNNOToForm();
        //                ItemID = ObjGetitem.GetItemIDToForm();
        //                Description = ObjGetitem.GetItemDescriptionToForm();
        //                RecQuantity = ObjGetitem.GetReceiveQtyToForm();

        //                string Location = ObjGetitem.GetLocationToForm();

        //                txtItemID.Text = ItemID.ToString();
        //                txtDescription.Text = Description.ToString();
        //                numericUpDown1.Value = Convert.ToDecimal(RecQuantity);

        //                //=============================================
        //                String S = "Select * from tblSerializeItem where ItemID = '" + ItemID.ToString().Trim() + "' and GRNNO='" + GRNNO.ToString().Trim() + "'";
        //                SqlCommand cmd = new SqlCommand(S);
        //                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
        //                DataSet dt = new DataSet();
        //                da.Fill(dt);
        //                //=========================================
        //                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
        //                {
        //                    listBox1.Items.Add(Convert.ToDouble(dt.Tables[0].Rows[i].ItemArray[3]));
        //                }

        //                //disable btn in edit mode
        //                listBox1.Enabled = false;
        //                btn_Add.Enabled = false;
        //                btn_Delete.Enabled = false;
        //                save.Enabled = false;
        //                btn_Clear.Enabled = false;
        //                textBox3.Enabled = false;
        //                //======================
        //            }
        //            else
        //            {
        //                //GRNNO = ObjGetitem.GetGRNNOToForm();


        //                ItemID = ObjGetitem.GetItemIDToForm();
        //                Description = ObjGetitem.GetItemDescriptionToForm();
        //                RecQuantity = ObjGetitem.GetReceiveQtyToForm();

        //                string Location = ObjGetitem.GetLocationToForm();

        //                txtItemID.Text = ItemID.ToString();
        //                txtDescription.Text = Description.ToString();
        //                numericUpDown1.Value = Convert.ToDecimal(RecQuantity);

        //                //=============================================
        //                String S = "Select * from tblSerializeItemTemp where ItemID = '" + ItemID.ToString().Trim() + "'";
        //                SqlCommand cmd = new SqlCommand(S);
        //                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
        //                DataSet dt = new DataSet();
        //                da.Fill(dt);
        //                //=========================================
        //                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
        //                {
        //                    listBox1.Items.Add(Convert.ToDouble(dt.Tables[0].Rows[i].ItemArray[2]));
        //                }

        //                listBox1.Enabled = true;
        //                btn_Add.Enabled = true;
        //                btn_Delete.Enabled = true;
        //                save.Enabled = true;

        //                btn_Clear.Enabled = true;
        //                textBox3.Enabled = true;
        //            }

        //        }
        //        //=============================================
        //    }
        //    catch { }
        //}

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void frmCusReturnSerial_Activated(object sender, EventArgs e)
        {
            try
            {
                if (AvoidActivated == false)
                {
                    ChklistBOX.Items.Clear();
                    IsCheckSearch = ObjGetitem.GetCheckSearchToForm();
                    //If this flag is true it sayas that serching item
                    if (IsCheckSearch == true)
                    {

                        GRNNO = ObjGetitem.GetGRNNOToForm();
                        ItemID = ObjGetitem.GetItemIDToForm();
                        Description = ObjGetitem.GetItemDescriptionToForm();
                        RecQuantity = ObjGetitem.GetReceiveQtyToForm();

                        string Location = ObjGetitem.GetLocationToForm();

                        txtItemID.Text = ItemID.ToString();
                        txtDescription.Text = Description.ToString();
                        numericUpDown1.Value = Convert.ToDecimal(RecQuantity);

                        //=============================================
                        String S = "Select SerialNO from tblSerialCusReturn where ItemID = '" + ItemID.ToString().Trim() + "' and CRTNO ='" + GRNNO.ToString().Trim() + "'";
                        SqlCommand cmd = new SqlCommand(S);
                        SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                        DataSet dt = new DataSet();
                        da.Fill(dt);
                        //=========================================
                        for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                        {
                            //ChklistBOX
                            ChklistBOX.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
                        }

                        //disable btn in edit mode
                        ChklistBOX.Enabled = false;
                        btn_Add.Enabled = false;
                        btn_Delete.Enabled = false;
                        save.Enabled = false;
                        btn_Clear.Enabled = false;
                        textBox3.Enabled = false;
                        //======================
                    }
                    else
                    {
                        //GRNNO = ObjGetitem.GetGRNNOToForm();

                        bool INVPRocess = true;
                        string Status = "Sold";
                        ItemID = ObjGetitem.GetItemIDToForm();
                        Description = ObjGetitem.GetItemDescriptionToForm();
                        RecQuantity = ObjGetitem.GetReceiveQtyToForm();

                        string Location = ObjGetitem.GetLocationToForm();

                        txtItemID.Text = ItemID.ToString();
                        txtDescription.Text = Description.ToString();
                        numericUpDown1.Value = Convert.ToDecimal(RecQuantity);

                        //=============================================
                      //  String S = "Select * from tblSerializeItem where ItemID = '" + ItemID.ToString().Trim() + "' and GRNNO='" + GRNNO.ToString().Trim() + "'";
                       // String S = "Select * from tblInvoiceSerializeItem where ItemID = '" + ItemID.ToString().Trim() + "' and WLocation ='" + Location + "' and IsINVProcess='" + INVPRocess + "'";
                        String S = "Select * from tblSerialItemTransaction where ItemID = '" + ItemID.ToString().Trim() + "' and WareHouseID ='" + Location + "' and Status='" + Status + "'";
                        SqlCommand cmd = new SqlCommand(S);
                        SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                        DataSet dt = new DataSet();
                        da.Fill(dt);
                        //=========================================
                        for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                        {
                            ChklistBOX.Items.Add(Convert.ToDouble(dt.Tables[0].Rows[i].ItemArray[2]));
                        }

                        ChklistBOX.Enabled = true;
                        btn_Add.Enabled = true;
                        btn_Delete.Enabled = true;
                        save.Enabled = true;

                        btn_Clear.Enabled = true;
                        textBox3.Enabled = true;
                    }

                }
                //=============================================
            }
            catch { }
        }




    }
}