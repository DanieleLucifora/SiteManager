namespace SiteManager;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
		// Imposta le dimensioni fisse per la finestra
		this.WidthRequest = 300;
		this.HeightRequest = 600;
	}

    private async void LoginButton_Clicked(object sender, EventArgs e)
    {
		string username = UsernameEntry.Text;
		string password = PasswordEntry.Text;

		if (username == "admin" && password == "password")
		{
			await Navigation.PushAsync(new MainPage());
		}
		else
		{
			await DisplayAlert("Errore", "Username o password errati", "OK");
		}
    }	
}