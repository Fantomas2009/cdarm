using Avalonia.Controls;
using CADARM.ViewModels;

namespace CADARM.Views
{
    public partial class CreateProjectPageView : UserControl
    {
        public CreateProjectPageView()
        {
            InitializeComponent();
            DataContext = new CreateProjectPageViewModel(this);  // Assure-toi de bien passer la bonne ViewModel ici
        }

        private void OnCancelButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // Masquer simplement la vue actuelle sans fermer la fenêtre
            this.IsVisible = false; // Masque le contrôle mais ne ferme pas la fenêtre
        }


    }
}
