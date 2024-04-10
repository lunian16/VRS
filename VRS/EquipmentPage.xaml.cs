using MySqlConnector;
using System.Linq.Expressions;
using System.Text;

namespace VRS;

public partial class EquipmentPage : ContentPage
{
	public EquipmentPage()
	{
		InitializeComponent();

        DatabaseAccess access = new DatabaseAccess();
        List<CategoryList> categories = access.FetchAllCategory();
        CategoryPicker.ItemsSource = categories;
        CategoryPicker.ItemDisplayBinding = new Binding("FullDetails");
    }
    //back to main page button
    private async void OnBackToMainButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
    }

    //clear all information after submit successfully
    private void ClearEquipmentForm()    //empty all entry and allow next input
    {
        EquipmentidEntry.Text = string.Empty;
        EquipnameEntry.Text = string.Empty;
        EquipDescrEntry.Text = string.Empty;
        rateEntry.Text = string.Empty;
        CategoryPicker.SelectedItem = null;
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

        DatabaseAccess db3Access = new DatabaseAccess();
        int idEntry = Convert.ToInt32(EquipmentidEntry.Text);


        db3Access.DeleteEquipmentRecord(idEntry);
    }


    public void OnDisplayAllEquipmentBtnClick(object sender, EventArgs e)  //make all equipment information display on the screen
    {

        DatabaseAccess db4Access = new DatabaseAccess();
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

        DatabaseAccess dbAccess = new DatabaseAccess();


        // Get the text from the Entry
        int Equipmentid = Convert.ToInt32(EquipmentidEntry.Text);
        string Equipname = EquipnameEntry.Text;
        string EquipDescr = EquipDescrEntry.Text;
        double rate = double.Parse(rateEntry.Text);
        int CategoryId = Category_Id;

        dbAccess.InsertRecordIfNotExists(Equipmentid, Equipname, EquipDescr, rate, CategoryId);
    }


}