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
    public partial class frmViewerLocationSales : Form
    {
        DataSet ds;
        public static string ConnectionString;

        public frmViewerLocationSales(frmLocationWiseSales frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.ds;
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

        private void frmViewerLocationSales_Load(object sender, EventArgs e)
        {
            try
            {
                string Myfullpath = Path.GetFullPath("CRLocationWiseSales.rpt");
                ReportDocument crp = new ReportDocument();
                crp.Load(Myfullpath);

                crp.SetDataSource(ds);
                crvLocationSales.ReportSource = crp;
            }
            catch { }
        }
    }
}