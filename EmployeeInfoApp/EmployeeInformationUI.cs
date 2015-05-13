using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmployeeInfoApp
{
    public partial class EmployeeInformationUI : Form
    {
        public EmployeeInformationUI()
        {

            InitializeComponent();
        }

        private string connectionString =
            ConfigurationManager.ConnectionStrings["EmployeeInfoConString"].ConnectionString;

        private bool isEmailExits = false;

        private bool isUpdateMode = false;

        private int employeeId;

        private void saveButton_Click(object sender, EventArgs e)
        {

            Employee employeeInfo = new Employee();
            employeeInfo.name = nameTextBox.Text;
            employeeInfo.address = addressTextBox.Text;
            employeeInfo.email = emailTextBox.Text;
            employeeInfo.salary = (float)Convert.ToDouble(salaryTextBox.Text);

            //Updating The Database
            if (isUpdateMode)
            {
                if (!IsEmailExists(employeeInfo.email))
                {
                    SqlConnection connection = new SqlConnection(connectionString);


                    //2. write query 

                    string query = "UPDATE tbl_employee SET Name ='" + employeeInfo.name + "', Address ='" +
                                   employeeInfo.address + "', Email='" + employeeInfo.email + "', Salary='" +
                                   employeeInfo.salary + "' WHERE ID = '" + employeeId + "'";


                    // 3. execute query 

                    SqlCommand command = new SqlCommand(query, connection);

                    connection.Open();
                    int rowAffected = command.ExecuteNonQuery();
                    connection.Close();



                    //4. see result

                    if (rowAffected > 0)
                    {
                        MessageBox.Show("Updated Successfully!");

                        saveButton.Text = "Save";
                        employeeId = 0;
                        isUpdateMode = false;
                        LoadEmployeeListView();

                    }
                    else
                    {
                        MessageBox.Show("Update Failed!");
                    }
                }
                else
                {
                    MessageBox.Show("Email Already Exits!");
                }
            }

            //Checking Email and Inserting Information.
            else if (!IsEmailExists(employeeInfo.email))
            {

                SqlConnection connection = new SqlConnection(connectionString);


                //2. write query 

                String querry = "INSERT INTO tbl_employee VALUES ('" + employeeInfo.name + "','" + employeeInfo.address +
                                "','" + employeeInfo.email + "','" + employeeInfo.salary + "')";


                // 3. execute query 

                SqlCommand command = new SqlCommand(querry, connection);

                connection.Open();
                int rowAffected = command.ExecuteNonQuery();
                connection.Close();


                //4. Result

                if (rowAffected > 0)
                {
                    MessageBox.Show("Information Inserted Successfully!");
                    isEmailExits = false;
                    nameTextBox.Clear();
                    addressTextBox.Clear();
                    emailTextBox.Clear();
                    salaryTextBox.Clear();
                }

                else
                {
                    MessageBox.Show("Insertion Failed!");
                }

            }

            else
            {

                // Is RegNo Exists? if exists not insert, else insert

                if (IsEmailExists(employeeInfo.email))
                {
                    MessageBox.Show("Email already exists! Please Enter a Valid Email Address");
                    nameTextBox.Clear();
                    addressTextBox.Clear();
                    emailTextBox.Clear();
                    salaryTextBox.Clear();
                    return;
                }



                // 1. Database Connection String

                SqlConnection connection = new SqlConnection(connectionString);

                //2. Write Querry


                String querry = "INSERT INTO tbl_employee VALUES ('" + employeeInfo.name + "','" + employeeInfo.address +
                                "','" + employeeInfo.email + "','" + employeeInfo.salary + "')";

                //3. Execute Querry


                SqlCommand command = new SqlCommand(querry, connection);

                connection.Open();
                int rowAffected = command.ExecuteNonQuery();
                connection.Close();

                //4. Result

                if (rowAffected > 0)
                {
                    MessageBox.Show("Information Inserted Successfully!");
                    nameTextBox.Clear();
                    addressTextBox.Clear();
                    emailTextBox.Clear();
                    salaryTextBox.Clear();
                }

                else
                {
                    MessageBox.Show("Insertion Failed!");
                }

            }




        }

        public bool IsEmailExists(string email)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            //2. write query 

            string query = "SELECT * FROM tbl_employee WHERE Email = '" + email + "'";


            // 3. execute query 

            SqlCommand command = new SqlCommand(query, connection);

            bool isEmailExists = false;

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                isEmailExists = true;
            }
            reader.Close();
            connection.Close();

            return isEmailExists;

        }


        //Loading Listview into Listview Box.
        public void LoadEmployeeListView()
        {
            employeeListView.Items.Clear();
            foreach (Employee employee in ShowAllEmployee())
            {
                ListViewItem item = new ListViewItem();
                item.Text = employee.name;
                item.SubItems.Add(employee.address);
                item.SubItems.Add(employee.email);
                item.SubItems.Add(employee.salary.ToString());
                item.Tag = employee;
                employeeListView.Items.Add(item);
            }
        }



        public List<Employee> ShowAllEmployee()
        {
            SqlConnection connection = new SqlConnection(connectionString);

            //2. write query 

            string query = "SELECT * FROM tbl_employee";


            // 3. execute query 

            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            List<Employee> employeeList = new List<Employee>();

            while (reader.Read())
            {

                Employee employee = new Employee();
                employee.id = (int)reader["ID"];
                employee.name = reader["Name"].ToString();
                employee.address = reader["Address"].ToString();
                employee.email = reader["Email"].ToString();
                employee.salary = (float)Convert.ToDouble(reader["Salary"].ToString());

                employeeList.Add(employee);


            }
            reader.Close();
            connection.Close();

            // populate list view with data 
            return employeeList;

        }


        private void EmployeeInformationUI_Load(object sender, EventArgs e)
        {
            LoadEmployeeListView();
        }

        private void showButton_Click_1(object sender, EventArgs e)
        {
            LoadEmployeeListView();
        }




        private void employeeListView_DoubleClick(object sender, EventArgs e)
        {
            // 1. Select selected Student

            ListViewItem item = employeeListView.SelectedItems[0];

            //Selecting the Item of the list
            Employee employee = (Employee)item.Tag;

            if (employee != null)
            {
                //2. Enable update mode -- save button = update button, grab id

                isUpdateMode = true;
                employeeId = employee.id;

                saveButton.Text = "Update";



                //3. Fill Text with student data 

                nameTextBox.Text = employee.name;
                addressTextBox.Text = employee.address;
                emailTextBox.Text = employee.email;
                salaryTextBox.Text = Convert.ToString(employee.salary);
            }

        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            ListViewItem item = employeeListView.SelectedItems[0];

            //Selecting the Item of the list
            Employee employeeInfo = (Employee)item.Tag;
            employeeInfo.name = nameTextBox.Text;
            employeeInfo.address = addressTextBox.Text;
            employeeInfo.email = emailTextBox.Text;
            employeeInfo.salary = (float)Convert.ToDouble(salaryTextBox.Text);
            DialogResult dialog = MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

            if (dialog == DialogResult.Yes)
            {
                if (employeeInfo != null)
                {

                    SqlConnection connection = new SqlConnection(connectionString);

                    //2. Write Querry


                    String querry = "DELETE FROM tbl_employee WHERE  ID = '" + employeeId + "'";

                    //3. Execute Querry


                    SqlCommand command = new SqlCommand(querry, connection);

                    connection.Open();
                    int rowAffected = command.ExecuteNonQuery();
                    connection.Close();

                    //4. Result

                    if (rowAffected > 0)
                    {
                        MessageBox.Show("Deleted Successfully!");
                        nameTextBox.Clear();
                        addressTextBox.Clear();
                        emailTextBox.Clear();
                        salaryTextBox.Clear();
                        LoadEmployeeListView();
                    }

                    else
                    {
                        MessageBox.Show("Please Select Your Information to DELETE!");
                    }
                }
            }

            


        }
    }

}
