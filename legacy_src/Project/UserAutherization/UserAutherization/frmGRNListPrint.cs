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
    public partial class frmGRNListPrint : Form
    {
        public frmGRNListPrint(frmGRNList frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.GRNData;
        }


        public static string ConnectionString;
        DataSet ds;

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

        private void frmGRNListPrint_Load(object sender, EventArgs e)
        {
            try
            {
                string Myfullpath;
                ReportDocument crp = new ReportDocument();
                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRGRNList.rpt";
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                crvGRNList.ReportSource = crp;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
            }
        }
    }
}