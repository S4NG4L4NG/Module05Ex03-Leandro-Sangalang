using Module07dataaccess.Services;
using MySql.Data.MySqlClient;

namespace Module07dataaccess;

public partial class MainPage : ContentPage
{
    int count = 0;

    private readonly DataBaseConnectionService _dbConnenctonService;

    public MainPage()
    {
        InitializeComponent();

        //initialize database connectivity
        _dbConnenctonService = new DataBaseConnectionService();
    }

    private async void PersonelPage(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ViewPersonal");
    }

    private async void OnTestConnection(object sender, EventArgs e)
    {
        var connectionString = _dbConnenctonService.GetConnectionString();
        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                ConnectionStatusLabel.Text = "Connection SUCCESS YEAH!";
                ConnectionStatusLabel.TextColor = Colors.Yellow;
            }
        }

        catch (Exception ex) 
        {
            ConnectionStatusLabel.Text = $"Connection Failed: {ex.Message}";
            ConnectionStatusLabel.TextColor = Colors.Red;

        }

      

    }

}