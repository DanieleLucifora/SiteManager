namespace SiteManager;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        UsernameEntry.Completed += (s, e) => PasswordEntry.Focus();
        PasswordEntry.Completed += (s, e) => LoginButton_Clicked(null, EventArgs.Empty);
    }

    private async void LoginButton_Clicked(object? sender, EventArgs e)
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
