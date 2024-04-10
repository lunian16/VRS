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
        DatabaseAccess dbAccess = new DatabaseAccess();
        // Get the text from the Entry
        int idEntry = Convert.ToInt32(categoryidEntry.Text);
        string categoryName = categorynameEntry.Text;
        dbAccess.InsertRecordIfNotExists(idEntry, categoryName);
    }


    public void CategoryDeleteFromDatabaseAsync(object sender, EventArgs e)
    {
        DatabaseAccess dbAccess = new DatabaseAccess();
        int idEntry = Convert.ToInt32(categoryidEntry.Text);
        dbAccess.DeleteRecord(idEntry);
    }


    public void OnDisplayAllCategoryBtnClick(object sender, EventArgs e)  //make all category information display on the screen
    {

        DatabaseAccess db2Access = new DatabaseAccess();
        categoryEditor.Text = db2Access.LoadCategoryToListView();

    }

}