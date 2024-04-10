using MySqlConnector;
using System.Linq.Expressions;
using System.Text;

namespace VRS;

public partial class EquipmentPage : ContentPage
{
	public EquipmentPage()
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
        List<CategoryList> categories = access.FetchAllCategory();
        CategoryPicker.ItemsSource = categories;
        CategoryPicker.ItemDisplayBinding = new Binding("FullDetails");
    }
    //back to main page button
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

        //insert into equipment database
        public void InsertRecordIfNotExists(int equipment_id, string equipment_name, string equipment_description, double daily_rate, int category_id)
        {
            using (var connection = new MySqlConnection(BuilderString.ConnectionString))
            {

                connection.Open();


                // Proceed with insertion if the record does not exist
                string query = "INSERT INTO equipment (equipment_id, equipment_name, equipment_description, daily_rate, category_id) value (@equipment_id, @equipment_name, @equipment_description, @daily_rate, @category_id)"; 
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@equipment_id", equipment_id);
                    command.Parameters.AddWithValue("@equipment_name", equipment_name);
                    command.Parameters.AddWithValue("@equipment_description", equipment_description);
                    command.Parameters.AddWithValue("@daily_rate", daily_rate);
                    command.Parameters.AddWithValue("@category_id", category_id);
                    int result = command.ExecuteNonQuery();
                    if (result < 0)
                    {
                        Console.WriteLine("Error inserting data into the database.");
                    }
                }

              
            }
        }

     
        public delegate void AlertCallback(string title, string message, string cancel);

       
        public AlertCallback OnAlert { get; set; }

        //delete equipment from database
        public void DeleteEquipmentRecord(int equipmentId)
        {
            try
            {
                using (var connection1 = new MySqlConnection(BuilderString.ConnectionString))
                {
                    connection1.Open();

                    string query = "DELETE FROM equipment WHERE equipment_id = @equipment_id";

                    using (var command = new MySqlCommand(query, connection1))
                    {
                        command.Parameters.AddWithValue("@equipment_id", equipmentId);
                        var result = command.ExecuteNonQuery();


                        if (result > 0)
                        {
                            OnAlert?.Invoke("Success", "Record deleted successfully.", "OK");
                        }
                        else
                        {
                            OnAlert?.Invoke("Notice", "No record was deleted.", "OK");
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                OnAlert?.Invoke("Error", "An error occurred: " + ex.Message, "OK");
            }
        }
    
        //load equipment from database and list them
    public string LoadEquipmentsToListView()
        {
            StringBuilder allEquipmentDetails = new StringBuilder();

            using (var connection = new MySqlConnection(BuilderString.ConnectionString))
            {
                connection.Open();
                string sql = "SELECT equipment_id, equipment_name, equipment_description, daily_rate, category_id FROM equipment";
                MySqlCommand command = new MySqlCommand(sql, connection);
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {

                    int equipmentid = reader.GetInt32(0);
                    string equipmentName = reader.GetString(1);
                    string equipmentDescription = reader.GetString(2);
                    double dailyRate = reader.GetDouble(3);
                    int categoryId = reader.GetInt32(4);

                    // Append each equipment's details to the StringBuilder
                    allEquipmentDetails.AppendLine($"Equipment ID: {equipmentid}, Equipment Name: {equipmentName} Equipment Description: {equipmentDescription}, Daily Rate: {dailyRate}, Category ID: {categoryId}");

                }
                return allEquipmentDetails.ToString();
            }

        }

        //show all category in the picker
        
        public List<CategoryList> FetchAllCategory()
        {
            List<CategoryList> categories = new List<CategoryList>();
            using (var connection = new MySqlConnection(BuilderString.ConnectionString))
            {
                connection.Open ();
                string sql = "SELECT category_id, category_name FROM category";
                MySqlCommand command = new MySqlCommand( sql, connection);
                using(MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(new CategoryList()
                        {
                            Categoryid = reader.GetInt32(0),
                            Categoryname = reader.GetString(1),
                        });
                    }
                }
                connection.Close();
            }
            return categories;
        }

    }

    //clear all information after submit successfully
    private void ClearEquipmentForm()    //empty all entry and allow next input
    {
        EquipmentidEntry.Text = string.Empty;
        EquipnameEntry.Text = string.Empty;
        EquipDescrEntry.Text = string.Empty;
        rateEntry.Text = string.Empty;
    }


    private async void OnAddEquipmentSubmitButtonClicked(object sender, EventArgs e)   //add equipment btn clicked event
    {
        EquipmentinputToDatabaseAsync(sender, e);
        await DisplayAlert("Submit successfully", "This equipment has been added successfully", "OK");
        ClearEquipmentForm();
    }


    private async void OnDeleteEquipmentSubmitButtonClicked(object sender, EventArgs e)   //delete equipment btn clicked event
    {
        EquipmentDeleteFromDatabaseAsync(sender, e);
        await DisplayAlert("Delete successfully", "This equipment has been deleted successfully", "OK");
        ClearEquipmentForm();
    }





    public void EquipmentDeleteFromDatabaseAsync(object sender, EventArgs e)
    {

        var builder = new MySqlConnectionStringBuilder
        {
            Server = "localhost",
            UserID = "root",
            Password = "password",
            Database = "vrs",
        };
        DatabaseAccess db3Access = new DatabaseAccess(builder);
        int idEntry = Convert.ToInt32(EquipmentidEntry.Text);


        db3Access.DeleteEquipmentRecord(idEntry);
    }


    public void OnDisplayAllEquipmentBtnClick(object sender, EventArgs e)  //make all equipment information display on the screen
    {
        var builder = new MySqlConnectionStringBuilder
        {
            Server = "localhost",
            UserID = "root",
            Password = "password",
            Database = "vrs",
        };
        DatabaseAccess db4Access = new DatabaseAccess(builder);
        equipmentEditor.Text = db4Access.LoadEquipmentsToListView();

    }
    public int Category_Id;
    public void CategoryPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        var selectedItem = (CategoryList)picker.SelectedItem;
        if (selectedItem != null)
        {
           Category_Id =Convert.ToInt32(selectedItem.Categoryid);
        }
        
    }

    public void EquipmentinputToDatabaseAsync(object sender, EventArgs e)
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
        int Equipmentid = Convert.ToInt32(EquipmentidEntry.Text);
        string Equipname = EquipnameEntry.Text;
        string EquipDescr = EquipDescrEntry.Text;
        double rate = double.Parse(rateEntry.Text);
        int CategoryId = Category_Id;

        dbAccess.InsertRecordIfNotExists(Equipmentid, Equipname, EquipDescr, rate, CategoryId);
    }


}