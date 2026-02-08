using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UserAutherization
{
    public partial class Registration3 : Form
    {
        public Registration3()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Registration2 R2 = new Registration2();
            R2.Show();
            this.Close();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (rdobtnLocalServer.Checked == true || rdobtnWorkstation.Checked == true)
            {
                Registration4 R4 = new Registration4();
                R4.Show();
                this.Close();
            }
            else 
            {
                MessageBox.Show("Please select an option");
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

            Registration2 R2 = new Registration2();
            R2.Show();
            this.Close();
        }
    }
}