using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using DataAccess;

namespace UserAutherization
{
    public partial class frmPatientList : Form
    {


        public static string ConnectionString;
        public frmPatientList()
        {
           
            InitializeComponent();
            setConnectionString();
        }


        private void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch (Exception ex) { throw ex; }

        }

        private void frmPatientList_Load(object sender, EventArgs e)
        {
            Load_PatientLIst();
        }


        private void Load_PatientLIst()
        {
            try
            {
                string s = "SELECT[CutomerID],[CustomerName],[Address1],[Address2],[Phone1],[Custom1],[Custom2],[Custom3]" +
                          " ,[Custom4],[Custom5],[ShipToAddress1],[ShipToAddress2],[Phone2],[Fax],[Email],[DueDays]" +
                          "  ,[VATNo],[Balance],[City],[Country],[ContactPerson],[ShipToCountry],[ShipToCity],[ShipToTax]" +
                          " ,[ShipToContactPerson],[Pricing_Level],[Cus_Type] FROM[dbo].[tblCustomerMaster] order by [CutomerID]";
                SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvPatientList.Rows.Clear();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dgvPatientList.Rows.Add();
                        dgvPatientList.Rows[i].Cells[0].Value = "";
                        dgvPatientList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                        dgvPatientList.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[1].ToString().Trim();


                        //DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                        //dgvPatientList.Rows[i].Cells[2].Value = abc.ToShortDateString();

                        dgvPatientList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                        //dgvPatientList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString();

                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }
    }
}
