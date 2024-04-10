using Microsoft.VisualBasic;
using MySqlConnector;
using System.Text;
using System.Xml.Linq;

namespace VRS;

public partial class RentalPage : ContentPage
{
	public RentalPage()
	{
        InitializeComponent();

        var builder = new MySqlConnectionStringBuilder
        {
            Server = "localhost",
            UserID = "root",
            Password = "password",
            Database = "vrs",
        };

        DatabaseAccess access = new DatabaseAccess(builder);
        List<Customer> customer = access.FetchAllCustomer();
        CustomerPicker.ItemsSource = customer;
        CustomerPicker.ItemDisplayBinding = new Binding("FullDetails");

        List<Equipment> equipment = access.FetchAllEquipment();
        EquipmentPicker.ItemsSource = equipment;
        EquipmentPicker.ItemDisplayBinding = new Binding("FullDetails");

    }

    async void RefreshPage()
    {
        await Navigation.PopAsync(); // Pop the current page
        await Navigation.PushAsync(new RentalPage()); // 
    }


    private async void OnBackToMainButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
    }


    public class DatabaseAccess
    {
        public MySqlConnectionStringBuilder BuilderString { get; set; }

        public DatabaseAccess(MySqlConnectionStringBuilder builderString)
        {
            BuilderString = builderString;
        }


        public void InsertRecordIfNotExists(int rental_id, DateTime applydate, DateTime rental_date, DateTime return_date, double cost, int equipment_id, int customer_id)
        {
            using (var connection = new MySqlConnection(BuilderString.ConnectionString))
            {
                connection.Open();


                // Proceed with insertion if the record does not exist
                string query = "INSERT INTO rental (rental_id, applydate, rental_date, return_date, cost,equipment_id,customer_id) value (@rental_id, @applydate, @rental_date, @return_date, @cost, @equipment_id, @customer_id)"; using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@rental_id", rental_id);
                    command.Parameters.AddWithValue("@applydate", applydate);
                    command.Parameters.AddWithValue("@rental_date", rental_date);
                    command.Parameters.AddWithValue("@return_date", return_date);
                    command.Parameters.AddWithValue("@cost", cost);
                    command.Parameters.AddWithValue("@equipment_id", equipment_id);
                    command.Parameters.AddWithValue("@customer_id", customer_id);
                    int result = command.ExecuteNonQuery();
                    if (result < 0)
                    {
                        Console.WriteLine("Error inserting data into the database.");
                    }
                }

                connection.Close();
            }
        }


        public void DeleteRecord(int rental_id)
        {
            using (var connection = new MySqlConnection(BuilderString.ConnectionString))
            {
                connection.Open();


                string query = "Delete from rental WHERE rental_id=@rental_id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@rental_id", rental_id);
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }


        public string LoadRentalsToListView()
        {
            StringBuilder allRentalDetails = new StringBuilder();

            using (var connection = new MySqlConnection(BuilderString.ConnectionString))
            {
                connection.Open();
                string sql = "SELECT rental_id, applydate, rental_date, return_date, cost,equipment_id,customer_id FROM rental";
                MySqlCommand command = new MySqlCommand(sql, connection);
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {

                    int rental_id = reader.GetInt32(0);
                    DateTime applydate = reader.GetDateTime(1);
                    DateTime rental_date = reader.GetDateTime(2);
                    DateTime return_date = reader.GetDateTime(3);
                    double cost = reader.GetDouble(4);
                    int equipment_id = reader.GetInt32(5);
                    int customer_id = reader.GetInt32(6);

                    // Append each customer's details to the StringBuilder
                    allRentalDetails.AppendLine($"Rental ID: {rental_id}, Applydate: {applydate}, Rental from {rental_date} to {return_date}, Total cost: {cost}, Equipment Id:{equipment_id}, Customer Id:{customer_id}");

                }
                return allRentalDetails.ToString();
            }

        }

        //list all customers from database
        public List<Customer> FetchAllCustomer()
        {
            List<Customer> customer = new List<Customer>();
            using (var connection = new MySqlConnection(BuilderString.ConnectionString))
            {
                connection.Open();
                string sql = "SELECT CONCAT(last_name, ' ', first_name) AS customer_name, customer_id, contact_phone FROM customer";
                MySqlCommand command = new MySqlCommand(sql, connection);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        customer.Add(new Customer()
                        {
                            Name = reader.GetString(0),
                            Customer_Num = reader.GetInt32(1),
                            PhoneNumber = reader.GetString(2),
                        });
                    }

                }

                connection.Close();
            }
            return customer;
        }

        //list all equipments from database
        public List<Equipment> FetchAllEquipment()
        {
            List<Equipment> equipment = new List<Equipment>();
            using (var connection = new MySqlConnection(BuilderString.ConnectionString))
            {
                connection.Open();
                string sql = "SELECT equipment_id, equipment_name, daily_rate FROM equipment " +
                    "WHERE equipment_id NOT IN (SELECT equipment_id FROM rental)";
                MySqlCommand command = new MySqlCommand(sql, connection);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        equipment.Add(new Equipment()
                        {
                            Equipmentid = reader.GetInt32(0),
                            Equipmentname = reader.GetString(1),
                            Equipmentprice = reader.GetDouble(2),
                        });
                    }
                }
                connection.Close();

            }
            return equipment;
        }

        }


    private void ClearRentalForm()    //empty all entry and allow next input
    {
        RentalidEntry.Text = string.Empty;
        CustomerPicker.SelectedItem = null;
        EquipmentPicker.SelectedItem = null;
        rentaldatePicker.Date = DateTime.Now;
        returndatePicker.Date= DateTime.Now;
    }



    private async void OnAddRentalSubmitButtonClicked(object sender, EventArgs e)   //add rental btn clicked event
    {
        RentalinputToDatabaseAsync(sender, e);
        await DisplayAlert("Submit successfully", "This rental has been added successfully", "OK");
        ClearRentalForm();
        RefreshPage();

    }


    private async void OnDeleteRentalSubmitButtonClicked(object sender, EventArgs e)   //delete Rental btn clicked event
    {
        RentalDeleteFromDatabaseAsync(sender, e);
        await DisplayAlert("Delete successfully", "This rental has been deleted successfully", "OK");
        ClearRentalForm();
    }

    //get equipment id from equipment list
    public int equipment_id;
    public double equipment_price;
    public void EquipmentPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        var selectedItem = (Equipment)picker.SelectedItem;
        if (selectedItem != null)
        {
            equipment_id = Convert.ToInt32(selectedItem.Equipmentid);
            equipment_price=Convert.ToDouble(selectedItem.Equipmentprice);
        }

    }

    //get customer id from customer list

    public int customer_id;
    public void CustomerPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        var selectedItem = (Customer)picker.SelectedItem;
        if (selectedItem != null)
        {
            customer_id = Convert.ToInt32(selectedItem.Customer_Num);
        }

    }


    //rental input into database
    public void RentalinputToDatabaseAsync(object sender, EventArgs e)
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
        int Rentalid = Convert.ToInt32(RentalidEntry.Text);
        DateTime applydate = applydatePicker.Date;
        DateTime rentaldate = returndatePicker.Date;
        DateTime returndate = returndatePicker.Date;     
        double cost = Convert.ToDouble(totalPrice.Text);
        cost = Math.Round(cost, 2);
        int Equipmentid= equipment_id;
        int Customerid=customer_id;


        dbAccess.InsertRecordIfNotExists(Rentalid, applydate, rentaldate, returndate, cost, Equipmentid, Customerid);
    }

    //rental delete from database
    public void RentalDeleteFromDatabaseAsync(object sender, EventArgs e)
    {

        var builder = new MySqlConnectionStringBuilder
        {
            Server = "localhost",
            UserID = "root",
            Password = "password",
            Database = "vrs",
        };
        DatabaseAccess dbAccess = new DatabaseAccess(builder);
        int idEntry = Convert.ToInt32(RentalidEntry.Text);


        dbAccess.DeleteRecord(idEntry);
    }


    //display all rental event
    public void OnDisplayAllRentalBtnClick(object sender, EventArgs e)  //make all rental information display on the screen
    {
        var builder = new MySqlConnectionStringBuilder
        {
            Server = "localhost",
            UserID = "root",
            Password = "password",
            Database = "vrs",
        };
        DatabaseAccess db2Access = new DatabaseAccess(builder);
        rentalsEditor.Text = db2Access.LoadRentalsToListView();

    }

    //calculate total price event
    public void OnCalculateTotalPrice(object sender, EventArgs e)
    {
        DateTime rentaldate = rentaldatePicker.Date;
        DateTime returndate = returndatePicker.Date;
        TimeSpan timeSpan = returndate - rentaldate;
        int days = timeSpan.Days;
        double total_price = equipment_price * days;
        total_price = Math.Round(total_price, 2);
        totalPrice.Text = total_price.ToString();
    }











}