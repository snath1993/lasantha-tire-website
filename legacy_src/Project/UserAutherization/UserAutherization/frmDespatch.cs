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
    public partial class frmDespatch : Form
    {
        public frmDespatch()
        {
            InitializeComponent();
            setConnectionString();
        }
        public static string ConnectionString;

        private void btnGetSalesorder_Click(object sender, EventArgs e)
        {
            Connector conn = new Connector();
            conn.ImportSalesOrderList();
            conn.Insert_SalesOrder();
            MessageBox.Show("Sales orders Loaded");
        }

        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
                //TextReader tr = new StreamReader("Connection.txt");
                //ConnectionString = tr.ReadLine();
                //tr.Close();
            }
            catch { }
        }

        private void cmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnPrint_Click(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {

        }
    }
}