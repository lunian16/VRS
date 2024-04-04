namespace VRS
{
    public partial class MainPage : ContentPage
    {
        
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnGoToCustomerButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CustomerPage());
        }

        private async void OnGoToEquipmentButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EquipmentPage());
        }

        private async void OnGoToRentalButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RentalPage());
        }

    }

}
