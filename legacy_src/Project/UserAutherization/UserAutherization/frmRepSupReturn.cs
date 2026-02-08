using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;


namespace UserAutherization
{
    public partial class frmRepSupReturn : Form
    {
        DataSet ds;
        public static string ConnectionString;

        public frmRepSupReturn(frmSupplierReturn frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.ds;
           // ConnectionString = DBUtil.clsUtils.conStringSqlServer();
        }

        private void setConnectionString()
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

        private void frmRepSupReturn_Load(object sender, EventArgs e)
        {
            try
            {

                string Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptSupReturn.rpt";
                ReportDocument crp = new ReportDocument();
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                crvSupplierReturn.ReportSource = crp;
            }
            catch { }
        }

        private void crvSupplierReturn_Load(object sender, EventArgs e)
        {

        }
    }
}