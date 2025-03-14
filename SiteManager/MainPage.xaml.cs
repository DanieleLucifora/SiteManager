﻿namespace SiteManager;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();
	}

	private async void OperaiPageClicked(object sender, EventArgs e)
	{
        await Navigation.PushAsync(new OperaiPage());
	}

	private async void CantieriPageClicked(object sender, EventArgs e)
	{
        await Navigation.PushAsync(new CantieriPage());
	}

	private async void MaterialiPageClicked(object sender, EventArgs e)
	{
        await Navigation.PushAsync(new MaterialiPage());
	}
	
	private async void ReportingPageClicked(object sender, EventArgs e)
	{
        await Navigation.PushAsync(new ReportingPage());
	}

	private async void StatistichePageClicked(object sender, EventArgs e)
	{
        await Navigation.PushAsync(new StatistichePage());
	}
}
