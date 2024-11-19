using Module07dataaccess.ViewModel;

namespace Module07dataaccess.View;

public partial class ViewPersonal : ContentPage
{
	public ViewPersonal()
	{
		InitializeComponent();

        var personalViewModel = new PersonalViewModel();
        BindingContext = personalViewModel;
    }
}