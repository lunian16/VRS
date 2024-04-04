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
}