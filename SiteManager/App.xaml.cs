namespace SiteManager;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();	//Inizializza i componenti XAML
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new NavigationPage(new LoginPage()));
	}
}