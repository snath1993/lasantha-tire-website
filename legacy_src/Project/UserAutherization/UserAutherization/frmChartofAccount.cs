using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UserAutherization
{
    public partial class frmChartofAccount : Form
    {
        public frmChartofAccount()
        {
            InitializeComponent();
        }
        Connector conn = new Connector();
        private void btnImport_Click(object sender, EventArgs e)
        {

            try
            {
                conn.ImportChartofAccounts();
                conn.fillchartofAcc();
                MessageBox.Show(" Chart of Accounts Import Successfully from Peachtree","Import",MessageBoxButtons.OK,MessageBoxIcon.Information);
                this.Close();
            }
            catch { }

        }

        private void frmChartofAccount_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                conn.ImportChartofAccounts();
                conn.fillchartofAcc();
                MessageBox.Show(" Chart of Accounts Import Successfully from Peachtree", "Import", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch { }

        }
    }
}