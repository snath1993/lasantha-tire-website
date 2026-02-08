using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MultiColumnComboBoxDemo
{
    public partial class DemoForm : Form
    {
        public DemoForm()
        {
            InitializeComponent();

            SetupData();
        }

        private void SetupData()
        {
            // Populate using a DataTable

            DataTable dataTable = new DataTable("Employees");

            dataTable.Columns.Add("Employee ID", typeof(String));
            dataTable.Columns.Add("Name", typeof(String));
            dataTable.Columns.Add("Designation", typeof(String));

            dataTable.Rows.Add(new String[] { "D1", "Natalia", "Developer" });
            dataTable.Rows.Add(new String[] { "D2", "Jonathan", "Developer" });
            dataTable.Rows.Add(new String[] { "D3", "Jake", "Developer" });
            dataTable.Rows.Add(new String[] { "D4", "Abraham", "Developer" });
            dataTable.Rows.Add(new String[] { "T1", "Mary", "Team Lead" });
            dataTable.Rows.Add(new String[] { "PM1", "Calvin", "Project Manager" });
            dataTable.Rows.Add(new String[] { "T2", "Sarah", "Team Lead" });
            dataTable.Rows.Add(new String[] { "D12", "Monica", "Developer" });
            dataTable.Rows.Add(new String[] { "D13", "Donna", "Developer" });

            multiColumnComboBox1.DataSource = dataTable;
            multiColumnComboBox1.DisplayMember = "Employee ID";
            multiColumnComboBox1.ValueMember = "Name";

            // Populate using a collection

            Student[] studentArray = new Student[] 
            { new Student("Andrew White", 10), new Student("Thomas Smith", 10), new Student("Alice Brown", 11),
              new Student("Lana Jones", 10), new Student("Jason Smith", 9), new Student("Amamda Williams", 11)
            };

            multiColumnComboBox2.DataSource = studentArray;
            multiColumnComboBox2.DisplayMember = multiColumnComboBox2.ValueMember = "Name";

            // Drop-down list (non-editable)

            List<Student> studentList = new List<Student>(studentArray);

            multiColumnComboBox3.DataSource = studentList;

            // Trying to use as a regular combobox

            multiColumnComboBox4.Items.Add("Cat");
            multiColumnComboBox4.Items.Add("Tiger");
            multiColumnComboBox4.Items.Add("Lion");
            multiColumnComboBox4.Items.Add("Cheetah");
            multiColumnComboBox4.SelectedIndex = 0;
        }

        public class Student
        {
            public Student(String name, int age)
            {
                this.name = name;
                this.age = age;
            }

            String name;

            public String Name
            {
                get { return name; }
            }

            int age;

            public int Age
            {
                get { return age; }
            }
        }

        private void DemoForm_Load(object sender, EventArgs e)
        {

        }
    }
}