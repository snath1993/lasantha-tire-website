using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Timers;

namespace UserAutherization
{
    public partial class frmSalesSummary : Form
    {
        public static string ConnectionString;
        Connector conn = new Connector();
        public DTInvoiceData DSInvoicing = new DTInvoiceData();
        clsCommon objclsCommon = new clsCommon();

        public frmSalesSummary()
        {
            InitializeComponent();
            setConnectionString();
        }
        public void setConnectionString()
        {
            clsDataAccess objclsDataAccess = new clsDataAccess();
            ConnectionString = objclsDataAccess.StrConectionStringLocal;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DSInvoicing.Clear();

            string sSQL = "Select * from tblSalesInvoices where [InvoiceDate] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [InvoiceDate]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'";               
            SqlCommand cmd3 = new SqlCommand(sSQL);
            SqlConnection con3 = new SqlConnection(ConnectionString);
            SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);
            da3.Fill(DSInvoicing, "DTInvoiceTransaction");

            frminvoicesummaryviewer ObjViwerInvPrint = new frminvoicesummaryviewer(this);
            ObjViwerInvPrint.Show();
        }

    }
}
