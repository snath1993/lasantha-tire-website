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

namespace UserAutherization
{
    public partial class frmIssueNoteLIstPrint : Form
    {
        public frmIssueNoteLIstPrint()
        {
            InitializeComponent();
            setConnectionString();
        }
        public static string ConnectionString;

        private void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }
        }
        public dsAPCommon dsobjIssueList = new dsAPCommon();


        private void btnPrint_Click(object sender, EventArgs e)
        {
            DateTime DTP = Convert.ToDateTime(dateTimePicker1.Text);
            string Dformat = "MM/dd/yyyy";
            string WHTDate = DTP.ToString(Dformat);
            DateTime DTP1 = Convert.ToDateTime(dateTimePicker2.Text);
            string Dformat1 = "MM/dd/yyyy";
            string WHTDate1 = DTP1.ToString(Dformat1);

            dsobjIssueList.Clear();

            String StrSql = "SELECT * FROM tblCompanyInformation";
            SqlDataAdapter da251 = new SqlDataAdapter(StrSql, ConnectionString);
            da251.Fill(dsobjIssueList, "DtCompaniInfo");

        
            String S25 = "Select * from tblIssueNoteIC where IssueDate between '" + WHTDate + "' and '" + WHTDate1 + "'";//
            SqlDataAdapter da25 = new SqlDataAdapter(S25, ConnectionString);
            da25.Fill(dsobjIssueList, "dt_IssueH_Header");

            for (int i = 0; i < dsobjIssueList.Tables[0].Rows.Count; i++)
            {
                String AB = "Select Distinct IssueNoteNo from tblIssueNoteLine where IssueNoteNo='" + dsobjIssueList.Tables[0].Rows[i].ItemArray[0] + "'";//
                SqlDataAdapter ABC = new SqlDataAdapter(AB, ConnectionString);
                DataTable dt = new DataTable();
                ABC.Fill(dt);
               
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    String S251 = "Select * from tblIssueNoteLine where IssueNoteNo='" + dsobjIssueList.Tables[0].Rows[i].ItemArray[0] + "'";//
                    SqlDataAdapter da2512 = new SqlDataAdapter(S251, ConnectionString);
                    da2512.Fill(dsobjIssueList, "dt_IssueLine");
                }
            }

            frmViewerIsueNoteListprint objIssueNoteListPrint = new frmViewerIsueNoteListprint(this);
            objIssueNoteListPrint.Show();
           
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}