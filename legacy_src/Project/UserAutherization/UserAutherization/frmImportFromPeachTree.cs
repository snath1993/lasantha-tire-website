using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PCMBLL;

namespace UserAutherization
{
    public partial class frmImportFromPeachTree : Form
    {
        clsBBLPTImport objclsBBLPTImport;

        public frmImportFromPeachTree()
        {
            InitializeComponent();
        }

        private void btnInventory_Click(object sender, EventArgs e)
        {
            objclsBBLPTImport = new clsBBLPTImport();
            try
            {
                ultraProgressBar1.Visible = true;
                //ultraProgressBar1.Value = 50;
                objclsBBLPTImport.ImportPhases_List(ultraProgressBar1);
                ultraProgressBar1.Visible = false;
                MessageBox.Show("Peactree Phases Imported Successfully...!", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            objclsBBLPTImport = new clsBBLPTImport();
            try
            {
                ultraProgressBar1.Visible = true;
                objclsBBLPTImport.ImportJob_List(ultraProgressBar1);
                ultraProgressBar1.Visible = false;
                MessageBox.Show("Peactree Jobs Imported Successfully...!", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            objclsBBLPTImport = new clsBBLPTImport();
            try
            {
                ultraProgressBar1.Visible = true;
                objclsBBLPTImport.ImportSubPhases_List(ultraProgressBar1);
                ultraProgressBar1.Visible = false;
                MessageBox.Show("Peactree Sub-Phases Imported Successfully...!", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            objclsBBLPTImport = new clsBBLPTImport();
            try
            {
                objclsBBLPTImport.ImportVenders_List();
                MessageBox.Show("Peactree Sub-Contractors Imported Successfully...!", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}