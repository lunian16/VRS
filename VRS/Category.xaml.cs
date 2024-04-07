using MySqlConnector;
using System.Text;

namespace VRS;

public partial class Category : ContentPage
{
	public Category()
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


        public void InsertRecordIfNotExists(int category_id, string category_name)
        {
            using (var connection = new MySqlConnection(BuilderString.ConnectionString))
            {
                connection.Open();


                // Proceed with insertion if the record does not exist
                string query = "INSERT INTO category (category_id, category_name) value (@category_id, @category_name)"; using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@category_id", category_id);
                    command.Parameters.AddWithValue("@category_name", category_name);
                    int result = command.ExecuteNonQuery();
                    if (result < 0)
                    {
                        Console.WriteLine("Error inserting data into the database.");
                    }
                }

                connection.Close();
            }
        }


        public void DeleteRecord(int category_id)
        {
            using (var connection = new MySqlConnection(BuilderString.ConnectionString))
            {
                connection.Open();


                string query = "Delete from category WHERE category_id=@category_id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@category_id", category_id);
                    command.ExecuteNonQuery();

                }

                connection.Close();
            }
        }






        public string LoadCategoryToListView()
        {
            StringBuilder allCategoryDetails = new StringBuilder();

            using (var connection = new MySqlConnection(BuilderString.ConnectionString))
            {
                connection.Open();
                string sql = "SELECT category_id, category_name FROM category";
                MySqlCommand command = new MySqlCommand(sql, connection);
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {

                    int CategoryId = reader.GetInt32(0);
                    string CategoryName = reader.GetString(1);

                    // Append each category's details to the StringBuilder
                    allCategoryDetails.AppendLine($"Category ID: {CategoryId}, Category Name: {CategoryName}");

                }
                return allCategoryDetails.ToString();
            }

        }


    }



    private void ClearCategoryForm()    //empty all entry and allow next input
    {
        categoryidEntry.Text = string.Empty;
        categorynameEntry.Text = string.Empty;

    }


    private async void OnAddCategorySubmitButtonClicked(object sender, EventArgs e)   //add category btn clicked event
    {
        CategoryinputToDatabaseAsync(sender, e);
        await DisplayAlert("Submit successfully", "This category has been added successfully", "OK");
        ClearCategoryForm();
    }


    private async void OnDeleteCategorySubmitButtonClicked(object sender, EventArgs e)   //delete category btn clicked event
    {
        CategoryDeleteFromDatabaseAsync(sender, e);
        await DisplayAlert("Delete successfully", "This category has been deleted successfully", "OK");
        ClearCategoryForm();
    }


    public void CategoryinputToDatabaseAsync(object sender, EventArgs e)
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
        int idEntry = Convert.ToInt32(categoryidEntry.Text);
        string categoryName = categorynameEntry.Text;


        dbAccess.InsertRecordIfNotExists(idEntry, categoryName);
    }


    public void CategoryDeleteFromDatabaseAsync(object sender, EventArgs e)
    {

        var builder = new MySqlConnectionStringBuilder
        {
            Server = "localhost",
            UserID = "root",
            Password = "password",
            Database = "vrs",
        };
        DatabaseAccess dbAccess = new DatabaseAccess(builder);
        int idEntry = Convert.ToInt32(categoryidEntry.Text);

        dbAccess.DeleteRecord(idEntry);
    }


    public void OnDisplayAllCategoryBtnClick(object sender, EventArgs e)  //make all category information display on the screen
    {
        var builder = new MySqlConnectionStringBuilder
        {
            Server = "localhost",
            UserID = "root",
            Password = "password",
            Database = "vrs",
        };
        DatabaseAccess db2Access = new DatabaseAccess(builder);
        categoryEditor.Text = db2Access.LoadCategoryToListView();

    }

}