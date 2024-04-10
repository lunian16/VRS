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


        DatabaseAccess access = new DatabaseAccess();
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

        DatabaseAccess dbAccess = new DatabaseAccess();

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

        DatabaseAccess dbAccess = new DatabaseAccess();
        int idEntry = Convert.ToInt32(RentalidEntry.Text);


        dbAccess.DeleteRentalRecord(idEntry);
    }


    //display all rental event
    public void OnDisplayAllRentalBtnClick(object sender, EventArgs e)  //make all rental information display on the screen
    {
        DatabaseAccess db2Access = new DatabaseAccess();
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