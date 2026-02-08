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
    public partial class frmPOrVariicationPrint : Form
    {
        public frmPOrVariicationPrint(FrmPurchaseOrderVarification frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.POList;
        }

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

        private void frmPOrVariicationPrint_Load(object sender, EventArgs e)
        {
            try
            {
                string Myfullpath;
                ReportDocument crp = new ReportDocument();
                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRPOVarification.rpt";
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                crvPOVarisication.ReportSource = crp;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
            }
        }
    }
}