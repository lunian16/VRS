using Microsoft.VisualBasic;
using MySqlConnector;
using System.Text;

namespace VRS;

public partial class RentalPage : ContentPage
{
	public RentalPage()
	{
		InitializeComponent();
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
                    double cost=reader.GetDouble(4);
                    int equipment_id = reader.GetInt32(5);
                    int customer_id = reader.GetInt32(6);

                    // Append each customer's details to the StringBuilder
                    allRentalDetails.AppendLine($"Rental ID: {rental_id}, applydate: {applydate}, rental from {rental_date} to {return_date}, Total cost: {cost}, equipment id:{equipment_id}, customer id:{customer_id}");

                }
                return allRentalDetails.ToString();
            }

        }


    }



    private void ClearRentalForm()    //empty all entry and allow next input
    {
        RentalidEntry.Text = string.Empty;
        CostEntry.Text = string.Empty;
        EquipmentIdEntry.Text = string.Empty;
        CustomerIdEntry.Text = string.Empty;
    }


    private async void OnAddRentalSubmitButtonClicked(object sender, EventArgs e)   //add rental btn clicked event
    {
        RentalinputToDatabaseAsync(sender, e);
        await DisplayAlert("Submit successfully", "This rental has been added successfully", "OK");
        ClearRentalForm();
    }


    private async void OnDeleteRentalSubmitButtonClicked(object sender, EventArgs e)   //delete Rental btn clicked event
    {
        RentalDeleteFromDatabaseAsync(sender, e);
        await DisplayAlert("Delete successfully", "This rental has been deleted successfully", "OK");
        ClearRentalForm();
    }


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
      
        double cost = Convert.ToDouble(CostEntry.Text);
        int Equipmentid=Convert.ToInt32(EquipmentIdEntry.Text);
        int Customerid=Convert.ToInt32(CustomerIdEntry.Text);


        dbAccess.InsertRecordIfNotExists(Rentalid, applydate, rentaldate, returndate, cost, Equipmentid, Customerid);
    }


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












}