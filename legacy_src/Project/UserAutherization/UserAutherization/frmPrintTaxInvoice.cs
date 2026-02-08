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
using CrystalDecisions.Shared;

namespace UserAutherization
{
    public partial class frmPrintTaxInvoice : Form
    {
        public static string ConnectionString;
        DataSet ds;
        public int InvType;
        public int InvTAXType;
        ReportDocument crp = new ReportDocument();
        public frmPrintTaxInvoice()
        {
            InitializeComponent();
            setConnectionString();
        }
        bool credit = false; bool card = false;
        public frmPrintTaxInvoice(frmInvoiceARList frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.DSInvoicing;
       

        }
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
        // public frmPrintTaxInvoice(frmInvoicing frmParent)
        //{
        //    InitializeComponent();
        //    setConnectionString();
        //    ds = frmParent.DSInvoicing;

        //}

        //public frmPrintTaxInvoice(frmInvoices1 frmParent)
        //{
        //    InitializeComponent();
        //    setConnectionString();
        //    ds = frmParent.DsItemWise;
        //    InvType = frmParent.InvoiceType;
        //}

        bool POSPrint,WithDisPrint,PDFView,ToolStripView;
        public frmPrintTaxInvoice(frmInvoices frmParent,bool pdf)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.DsItemWise;
            InvType = frmParent.InvoiceType;
            POSPrint = frmParent.POSPrint;
            WithDisPrint = frmParent.WithDisPrint;
            credit = frmParent.Credit;
            card = frmParent.Card;
            PDFView = pdf;
        }

        string Myfullpath;

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
           
      

        }

        private void axAcroPDF1_OnError(object sender, EventArgs e)
        {

        }

        private void btnPrint_Click_1(object sender, EventArgs e)
        {
            crp.PrintOptions.PaperOrientation = PaperOrientation.Portrait;
            crp.PrintToPrinter(1, true, 0, 0);
        }

        private void btnClose_Click_1(object sender, EventArgs e)
        {

        }

        private void btnClose2_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void axAcroPDF11_OnError(object sender, EventArgs e)
        {
        }

        private void btnClose_Click_2(object sender, EventArgs e)
        {
            this.Close();
        }
     
        private void btnClose_Click_3(object sender, EventArgs e)
        {
            frmInvoices.Close =true;
            frmInvoices.GoEdit = false;
            this.Close();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            frmInvoices.GoEdit = true;
            frmInvoices.Close = false;
            this.Close();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void frmPrintTaxInvoice_Load(object sender, EventArgs e)
        {
            try
            {

                if(ToolStripView == false)
                {
                  //  toolStrip1.Visible = false;
                }
                else
                {
                  //  toolStrip1.Visible = true;
                }
                               
                if (POSPrint == true)
                {

                    if (WithDisPrint == true)
                    {
                        Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRPOSInvoicePRINTWithDis.rpt";
                    }
                    else
                    {
                        Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRPOSInvoicePRINT.rpt";
                    }

                }
                else
                {

                    if (InvType == 1)
                    {
                        if (WithDisPrint == true)
                        {

                            Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRInvoiceWithDiscountNonVat.rpt";
                        }
                        else
                        {
                            if (credit == true||card ==true)
                            {
                                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRInvoiceCreditNonVat.rpt";
                            }
                            else
                            {
                                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRInvoiceNonVat.rpt";
                            }
                        }
                    }
                    else
                    {
                        if (WithDisPrint == true)
                        {

                            Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRInvoiceWithDiscount.rpt";
                        }
                        else
                        {
                           
                           
                                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRInvoice.rpt";
                            
                        }
                    }
                    //if (InvType == 2)
                    //{
                    //    if (InvTAXType == 1)
                    //    {
                    //        Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\VATINCCRInvoice.rpt";
                    //    }
                    //    else if (InvTAXType == 0)
                    //    {
                    //        Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\VATEXCCRInvoice.rpt";
                    //    }

                    //    //                    Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRTaxInvoice.rpt";
                    //    //Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\VATCRInvoice.rpt";
                    //}
                    //if (InvType == 3)
                    //{
                    //    Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRSVATInvoice.rpt";
                    //}
                }
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                crvTaxInvoice.ReportSource = crp;

                //if (PDFView == true)
                //{
                //    axAcroPDF3.Visible = true;
                //    crvTaxInvoice.Visible = false;
                //    crp.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, System.Windows.Forms.Application.StartupPath + "\\PDF\\INV.pdf");


                //    //MemoryStream ms;
                //    //ms = (MemoryStream)crp.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                //    axAcroPDF3.src = System.Windows.Forms.Application.StartupPath + "\\PDF\\INV.pdf";
                //    axAcroPDF3.LoadFile(System.Windows.Forms.Application.StartupPath + "\\PDF\\INV.pdf");

                    //  axAcroPDF1.src =ms.ToString();
               // }
                //else
                //{
                //    axAcroPDF3.Visible = false;
                //    crvTaxInvoice.Visible = true;
                //}

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
            }
                       
        }
        private static readonly Encoding LocalEncoding = Encoding.UTF8;

    }
}