using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using CrystalDecisions.CrystalReports.Engine;


namespace UserAutherization
{
    public partial class frmSalesVarificationPrint : Form
    {

        public static string ConnectionString;
        DataSet ds;


        public void setConnectionString()
        {
            try
            {
                //TextReader tr = new StreamReader("Connection.txt");
                //ConnectionString = tr.ReadLine();
                //tr.Close();
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }
        }
        public frmSalesVarificationPrint(FrmSalesOrderVarification frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.DSDispatch;
        }

        private void frmSalesVarificationPrint_Load(object sender, EventArgs e)
        {

            string Myfullpath = Path.GetFullPath("CRSalesOrderVarify.rpt");
            ReportDocument crp = new ReportDocument();
            crp.Load(Myfullpath);

            crp.SetDataSource(ds);
            CRVSalesVarification.ReportSource = crp;
            //CRSalesOrderVarify cr = new CRSalesOrderVarify();
            //cr.SetDataSource(ds);
            //CRVSalesVarification.ReportSource = cr;
        }
    }
}