using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Input;
using Avalonia.Controls;

namespace CADARM.ViewModels;

public class HomePageViewModel : ViewModelBase
{
    // Propriété qui contiendra le contenu du ContentControl
    private UserControl _currentContent;
    public UserControl CurrentContent
    {
        get => _currentContent;
        set => SetProperty(ref _currentContent, value);
    }

    // Commande associée au bouton
    public ICommand ShowCreateProjectFormCommand { get; }

    public HomePageViewModel()
    {
        // Initialisation de la commande
        ShowCreateProjectFormCommand = new RelayCommand(ShowCreateProjectForm);
    }

    // Méthode exécutée quand on clique sur le bouton
    private void ShowCreateProjectForm()
    {
        // Assigner dynamiquement le CreateProjectPageView à la propriété CurrentContent
        CurrentContent = new CADARM.Views.CreateProjectPageView();
    }
}
