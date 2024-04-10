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
            DatabaseAccess dbAccess = new DatabaseAccess();


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
        DatabaseAccess dbAccess = new DatabaseAccess();
        int idEntry = Convert.ToInt32(customeridEntry.Text);


        dbAccess.DeleteCustomerRecord(idEntry);
    }


    public void OnDisplayAllCustomerBtnClick(object sender, EventArgs e)  //make all customer information display on the screen
        {
            DatabaseAccess db2Access = new DatabaseAccess();
            customersEditor.Text = db2Access.LoadCustomersToListView();

        }


}