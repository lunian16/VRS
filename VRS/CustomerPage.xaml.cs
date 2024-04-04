using MySqlConnector;
using System.Text;
using System.Xml.Linq;

namespace VRS;

public partial class CustomerPage : ContentPage
{
    public CustomerPage()
    {
        InitializeComponent();
    }

    private async void OnBackToMainButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
    }


    //manage database

    public class DatabaseAccess
    {
        public MySqlConnectionStringBuilder BuilderString { get; set; }

        public DatabaseAccess(MySqlConnectionStringBuilder builderString)
        {
            BuilderString = builderString;
        }


        public void InsertRecordIfNotExists(int customerid, string lastname, string firstname, string contactphone, string email)
        {
            using (var connection = new MySqlConnection(BuilderString.ConnectionString))
            {
                connection.Open();


                // Proceed with insertion if the record does not exist
                string query = "INSERT INTO customer (customer_id, last_name, first_name, contact_phone, email) value (@customer_id, @last_name, @first_name, @contact_phone, @email)"; using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@customer_id", customerid);
                    command.Parameters.AddWithValue("@last_name", lastname);
                    command.Parameters.AddWithValue("@first_name", firstname);
                    command.Parameters.AddWithValue("@contact_phone", contactphone);
                    command.Parameters.AddWithValue("@email", email);
                    int result = command.ExecuteNonQuery();
                    if (result < 0)
                    {
                        Console.WriteLine("Error inserting data into the database.");
                    }
                }

                connection.Close();
            }
        }


        public void DeleteRecord(int customerid)
        {
            using (var connection = new MySqlConnection(BuilderString.ConnectionString))
            {
                connection.Open();


                string query = "Delete from customer WHERE customer_id=@customer_id"; 
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@customer_id", customerid);
                    command.ExecuteNonQuery();

                }

                connection.Close();
            }
        }






        public string LoadCustomersToListView()
        {
            StringBuilder allCustomerDetails = new StringBuilder();

            using (var connection = new MySqlConnection(BuilderString.ConnectionString))
            {
                connection.Open();
                string sql = "SELECT customer_id, last_name, first_name, contact_phone, email FROM customer";
                MySqlCommand command = new MySqlCommand(sql, connection);
                MySqlDataReader reader= command.ExecuteReader();
                    while (reader.Read())
                    {

                    int CustomerId = reader.GetInt32(0);
                    string LastName = reader.GetString(1);
                    string FirstName = reader.GetString(2);
                    string PhoneNumber = reader.GetString(3);
                    string Email = reader.GetString(4);

                    // Append each customer's details to the StringBuilder
                    allCustomerDetails.AppendLine($"Customer ID: {CustomerId}, Name: {LastName} {FirstName}, Phone Number: {PhoneNumber}, Email: {Email}");
                  
                }
                  return allCustomerDetails.ToString();
            }
           
        }
  

    }



        private void ClearCustomerForm()    //empty all entry and allow next input
            {
                customeridEntry.Text = string.Empty;
                lastnameEntry.Text = string.Empty;
                firstnameEntry.Text = string.Empty;
                phoneEntry.Text = string.Empty;
                emailEntry.Text = string.Empty;
            }


        private async void OnAddCustomerSubmitButtonClicked(object sender, EventArgs e)   //add customer btn clicked event
        {
            CustomerinputToDatabaseAsync(sender, e);
            await DisplayAlert("Submit successfully", "This customer has been added successfully", "OK");
            ClearCustomerForm();
        }


            private async void OnDeleteCustomerSubmitButtonClicked(object sender, EventArgs e)   //delete customer btn clicked event
            {
                CustomerDeleteFromDatabaseAsync(sender, e);
                await DisplayAlert("Delete successfully", "This customer has been deleted successfully", "OK");
                ClearCustomerForm();
            }


    public void CustomerinputToDatabaseAsync(object sender, EventArgs e)
        {

            var builder = new MySqlConnectionStringBuilder
            {
                Server = "localhost",
                UserID = "root",
                Password = "password",
                Database = "vrs",
            };
            DatabaseAccess dbAccess = new DatabaseAccess(builder);


            // Get the text from the Entry
            int idEntry = Convert.ToInt32(customeridEntry.Text);
            string lastName = lastnameEntry.Text;
            string firstName = firstnameEntry.Text;
            string contactphone = phoneEntry.Text;
            string email = emailEntry.Text;

            dbAccess.InsertRecordIfNotExists(idEntry, lastName, firstName, contactphone, email); 
        }


    public void CustomerDeleteFromDatabaseAsync(object sender, EventArgs e)
    {

        var builder = new MySqlConnectionStringBuilder
        {
            Server = "localhost",
            UserID = "root",
            Password = "password",
            Database = "vrs",
        };
        DatabaseAccess dbAccess = new DatabaseAccess(builder);
        int idEntry = Convert.ToInt32(customeridEntry.Text);


        dbAccess.DeleteRecord(idEntry);
    }


    public void OnDisplayAllCustomerBtnClick(object sender, EventArgs e)  //make all customer information display on the screen
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = "localhost",
                UserID = "root",
                Password = "password",
                Database = "vrs",
            };
            DatabaseAccess db2Access = new DatabaseAccess(builder);
            customersEditor.Text = db2Access.LoadCustomersToListView();

        }


}