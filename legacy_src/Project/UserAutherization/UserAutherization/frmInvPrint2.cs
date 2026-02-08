using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UserAutherization
{
    public partial class frmInvPrint2 : Form
    {
        public static string ConnectionString;
        DataSet ds;
        public frmInvPrint2(frmInvoiceAR frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.DSInvoicing;
        }
        public void setConnectionString()
        {
            try
            {
               
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }
        }
        private void frmInvPrint2_Load(object sender, EventArgs e)
        {
            //CRInvoiceTax cr = new CRInvoiceTax();
            //cr.SetDataSource(ds);
            //crvTaxInvoice.ReportSource = cr;
        }
    }
}