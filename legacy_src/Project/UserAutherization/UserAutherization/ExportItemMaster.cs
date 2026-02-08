using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace UserAutherization
{
    public partial class ExportItemMaster : Form
    {
        private string ConnectionString;

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
        public ExportItemMaster()
        {
            InitializeComponent();
            setConnectionString();
        }

        private void btnInventory_Click(object sender, EventArgs e)
        {

            String StrSql = "SELECT * FROM [tblItemMaster]";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {


                    XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\ItemMaster.xml", System.Text.Encoding.UTF8);
                    Writer.Formatting = Formatting.Indented;
                    Writer.WriteStartElement("PAW_Items");
                    Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                    Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                    Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");








                    Writer.WriteStartElement("PAW_Item");
                    Writer.WriteAttributeString("xsi:type", "paw:item");

                    Writer.WriteStartElement("ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(dt.Rows[i]["ItemID"].ToString().Trim());//Vendor ID should be here = Ptient No
                    Writer.WriteEndElement();

                    //if (i == 0)
                    //{
                    Writer.WriteStartElement("Description");
                    //Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(dt.Rows[i]["ItemDescription"].ToString().Trim());
                    Writer.WriteEndElement();
                    //}                       

                    Writer.WriteStartElement("Class");
                    Writer.WriteString(dt.Rows[i]["ItemClass"].ToString().Trim());//Date 
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("isInactive");
                    Writer.WriteString(dt.Rows[i]["inactive"].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Description_for_Sales");
                    Writer.WriteString(dt.Rows[i]["DescriptionForSale"].ToString().Trim());//Cash Account
                    Writer.WriteEndElement();//CreditMemoType

                    Writer.WriteStartElement("Sales_Prices");

                    Writer.WriteStartElement("Sales_Price_Info");
                    Writer.WriteAttributeString("Key", "1");

                    Writer.WriteStartElement("Sales_Price");
                    Writer.WriteString(dt.Rows[i]["PriceLevel1"].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Sales_Price_Info");
                    Writer.WriteAttributeString("Key", "2");

                    Writer.WriteStartElement("Sales_Price");
                    Writer.WriteString(dt.Rows[i]["PriceLevel2"].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();

                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Last_Unit_Cost");
                    Writer.WriteString(dt.Rows[i]["UnitCost"].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("GL_Sales_Account");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(dt.Rows[i]["SalesGLAccount"].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("GL_Inventory_Account");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(dt.Rows[i]["InventoryAcc"].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("GL_COGSSalary_Acct");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(dt.Rows[i]["CostOfSalesAcc"].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Type");
                    Writer.WriteString(dt.Rows[i]["TaxType"].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Stocking_UM");
                    Writer.WriteString(dt.Rows[i]["UOM"].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Tax_Type_Name");
                    Writer.WriteString(dt.Rows[i]["TaxType"].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("CustomFields");

                    Writer.WriteStartElement("CustomField");

                    Writer.WriteStartElement("Value");
                    Writer.WriteAttributeString("Index ", "1");//Change time and date both
                    Writer.WriteString(dt.Rows[i]["Custom1"].ToString().Trim());
                    Writer.WriteEndElement();


                    Writer.WriteStartElement("Value");
                    Writer.WriteAttributeString("Index ", "2");//Change time and date both
                    Writer.WriteString(dt.Rows[i]["Custom2"].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Value");
                    Writer.WriteAttributeString("Index ", "3");//Change time and date both
                    Writer.WriteString(dt.Rows[i]["Custom3"].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Value");
                    Writer.WriteAttributeString("Index ", "4");//Change time and date both
                    Writer.WriteString(dt.Rows[i]["Custom4"].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Value");
                    Writer.WriteAttributeString("Index ", "5");//Change time and date both
                    Writer.WriteString(dt.Rows[i]["Custom5"].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();

                    Writer.WriteEndElement();

                    Writer.WriteEndElement();
                    //********************
                    Writer.WriteEndElement();//last line
                    Writer.Close();

                    Connector conn = new Connector();
                    conn.ImportItemMaster();
                }
            }
            MessageBox.Show("Item Master file Successfully Exported to Peachtree", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);


        }
    }
}
