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

namespace UserAutherization
{
    class SerialNumber
    {
        public static ArrayList ItemInfo = new ArrayList();
        public static  ArrayList SerialNumbers = new ArrayList();

        public static string GRN_date = "";
       // ReturnSerial rs = new ReturnSerial();
       // ValidateInputs VInput = new ValidateInputs();

        //To Shiment
        public static int TextBoxIndex = -1;
        public static double OrderedQty = 0;
        public static double RcvdQty = 0;
        public static string itemID="";

      


        #region To Shipments
        public void setItem_ID(string ItemID)
        {
            itemID = ItemID;
        }

        
        public ArrayList getItemInfo()
        {
            return ItemInfo;
        }

        // sanjeewa added this to get serial numbers
        public ArrayList getSerialNOS()
        {
            return SerialNumbers;
        }




        //=============================
        public void setDate(string Date)
        {
            GRN_date = Date;
        }
        //=============================
        public string getDate()
        {
            return GRN_date;
        }
        //=============================
        public void setOrddQTY(double qty)
        {
            OrderedQty = qty;
        }



        public void setReceivedQty(double RQty)
        {
             RcvdQty = RQty;
        
        }




        //=============================
        public void SETTextBoxIndex(int addrs)
        {
            TextBoxIndex = addrs;
        }
        //=============================
        public int getTextBoxAddrs()
        {
            return TextBoxIndex;
        }
        //==============================
        public double GetOrddQTY()
        {
            return OrderedQty;
        }

        //======Set Recieved QTY
        public void SET_rcvdQTY(double rcvdQty)
        {
            RcvdQty = rcvdQty;
        }
        public double GetRcvdQty()
        {
            return RcvdQty;
        }



        public static string ConnectionString;
        public void setConnectionString()
        {
            TextReader tr = new StreamReader("Connection.txt");
            ConnectionString = tr.ReadLine();
            tr.Close();
        }

        //...................................................................................
        //the following code segment get the serial numbers from the database

       // public ArrayList GetSerialNumbers(string GrN1, string Item)
       // {
           // setConnectionString();

           //// ArrayList SerialNumbers = new ArrayList();
           // SerialNumbers.Clear();
           // string ConnString = ConnectionString;// @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=ImportCosting.mdb";//" + ConnectioString;         //C:\Documents and Settings\Prabhash\Desktop\IMPORT COSTING\MultiColumnComboBoxDemo\Import_Costing.mdb";
           // string sql = "select Serial_No from Serialized_Items where GRN_NO='" + GrN1 + "' and  Item_ID='" + Item + "' and GRN_Type<>'Return Good'";
           //   SqlConnection Conn = new SqlConnection(ConnString);
           // PsqlCommand cmd = new PsqlCommand(sql);
           // cmd.Connection = Conn;
           // Conn.Open();
           // PsqlDataReader reader = cmd.ExecuteReader();
           // while (reader.Read())
           // {
           //     SerialNumbers.Add(reader.GetString(0).Trim());
           // }

           // reader.Close();
           // Conn.Close();
           // return SerialNumbers;


     //   }

        //...............................................................................






       // public bool IsSerialItem(string ItemID)
       // {
            //try
            //{
            //setConnectionString();
            //string ConnString = ConnectionString; //@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=ImportCosting.mdb";
            //    string sql = "select ItemID,Item_Description from Item where ItemClass='10' and ItemID='" + ItemID + "'";
            //    Pervasive.Data.SqlClient.PsqlConnection Conn = new PsqlConnection(ConnString);
            //    PsqlCommand cmd = new PsqlCommand(sql);
            //    cmd.Connection = Conn;
            //    Conn.Open();
            //    PsqlDataReader reader = cmd.ExecuteReader();
            //    if (reader.HasRows)
            //    {
            //        while (reader.Read())
            //        {
            //            ItemInfo.Clear();
            //            ItemInfo.Add(reader.GetValue(0).ToString().Trim());
            //            ItemInfo.Add(reader.GetValue(1).ToString().Trim ());
            //        }
            //        reader.Close();
            //        Conn.Close();
            //        return true;
            //    }
            //    else
            //    {
            //        ItemInfo.Clear();
            //        reader.Close();
            //        Conn.Close();
            //        return false;
            //    }
            //}
            //catch
            //{ }
        //}

        public string Item2 = "";
        public string getItem()

        {

            Item2 = ItemInfo[0].ToString();
            return Item2;
        }









        #region Insert Serial Numbers
        public void insertSerialItems(string GRN, ArrayList Snos, string GRNType, string IsReturned)
        {
            //try
            //{
            //    setConnectionString();
            //    RcvdQty = Snos.Count;
            //    //  DateTime poDate = Convert.ToDateTime(date);
            //    string ConnString = ConnectionString; // @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=ImportCosting.mdb";
            //    Pervasive.Data.SqlClient.PsqlConnection Conn = new PsqlConnection(ConnString);
            //    PsqlCommand cmd = Conn.CreateCommand();
            //  //  string GRN_date1 = VInput.Get_Date(GRN_date);
            //    Conn.Open();
            //    try
            //    {
            //        for (int i = 0; i < Snos.Count; i++)
            //        {
            //            cmd.CommandText = "INSERT INTO Serialized_Items (GRN_NO,Item_ID,Serial_No,GRN_Type,IsReturned) VALUES('" + GRN + "','" + ItemInfo[0].ToString() + "','" + Snos[i].ToString() + "','" + GRNType + "','" + IsReturned + "')";
            //            cmd.ExecuteNonQuery();
            //        }
            //    }
            //    catch
            //    {
                    
            //    }
            //    Conn.Close();

            //}
            //catch
            //{
            //    //  MessageBox.Show(" PO has some invalid data     (Duplicate)  ");
            //}
        }
        #endregion


        //.............................................................


        #region Delete SerialNumbers
        public void DeleteSerialItems(string GRN , string Item)
        {
            //try
            //{
            //    setConnectionString();
            //  //  RcvdQty = Snos.Count;
            //    //  DateTime poDate = Convert.ToDateTime(date);
            //    string ConnString = ConnectionString; // @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=ImportCosting.mdb";
            //    Pervasive.Data.SqlClient.PsqlConnection Conn = new PsqlConnection(ConnString);
            //    PsqlCommand cmd = Conn.CreateCommand();
            //    //  string GRN_date1 = VInput.Get_Date(GRN_date);
            //    Conn.Open();
            //    try
            //    {
            //       // for (int i = 0; i < Snos.Count; i++)
            //        //{
            //        cmd.CommandText = "Delete  Serialized_Items where GRN_NO ='" + GRN + "' and Item_ID='" + Item + "'";
            //        cmd.ExecuteNonQuery();
            //    }
            //    catch
            //    {

            //    }
            //    Conn.Close();

            //}
            //catch
            //{
            //    //  MessageBox.Show(" PO has some invalid data     (Duplicate)  ");
            //}
        }
        #endregion

        //................................................................................................















         #endregion 
   
        //===========================================================
        //To Opening Stock 
        //===========================================================

        //=============================
        //To Opening stocks
        public static int GridCellIndex = -1;
       // public static int OrderedQty = 0;
        public static int Qty = 0;
        public static ArrayList ItemIinfo = new ArrayList();
      //  public static string itemDescription = "";

        #region To Opening Stock
        public void setGridCellIndex(int index)
        {
            GridCellIndex = index;
        }
        public int GET_GridCellIndex()
        {
            return GridCellIndex;
        }
        //Set Item Information
        public void SET_Iteminfo(ArrayList Item_Info)
        {
           // ItemIinfo.Clear();
            ItemIinfo = Item_Info;      
        }
        //Get Item Information
        public ArrayList GET_ItemInfo()
        {
            return ItemIinfo;
        }
        //Set Quentity
        public void SET_Qty(int qty)
        {
            Qty = qty;
        }
        //Get Quentity
        public int GET_Qty()
        {
            return Qty;
        }

        #endregion
    }
   

    }
