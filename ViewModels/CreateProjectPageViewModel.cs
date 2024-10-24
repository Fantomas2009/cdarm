using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using CADARM.Models;
using CADARM.Views; // Assurez-vous d'importer le namespace correct pour ProjectModel

namespace CADARM.ViewModels
{

    public class CreateProjectPageViewModel : INotifyPropertyChanged
    {
        // Implémentation de l'interface INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Propriétés liées aux champs du formulaire
        private string projectNumber;
        public string ProjectNumber
        {
            get => projectNumber;
            set
            {
                if (projectNumber != value)
                {
                    projectNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        private string projectName;
        public string ProjectName
        {
            get => projectName;
            set
            {
                if (projectName != value)
                {
                    projectName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string usedStandard;
        public string UsedStandard
        {
            get => usedStandard;
            set
            {
                if (usedStandard != value)
                {
                    usedStandard = value;
                    OnPropertyChanged();
                }
            }
        }

        private string accessPath;
        private CreateProjectPageView createProjectPageView;

        public string AccessPath
        {
            get => accessPath;
            set
            {
                if (accessPath != value)
                {
                    accessPath = value;
                    OnPropertyChanged();
                }
            }
        }

        // Commandes
        public ICommand CreateProjectCommand { get; }
        public ICommand BrowseCommand { get; }

        // Référence à la fenêtre active pour les dialogues
        private readonly Window window;

        // Constructeur
        public CreateProjectPageViewModel(Window window)
        {
            this.window = window;

            // Initialisation des commandes
            CreateProjectCommand = new RelayCommand(CreateProject, () => true);
            BrowseCommand = new RelayCommand(async () => await BrowseFolder());
        }

        public CreateProjectPageViewModel(CreateProjectPageView createProjectPageView)
        {
            this.createProjectPageView = createProjectPageView;
        }

        // Méthode pour créer le projet
        private void CreateProject()
        {
            if (string.IsNullOrWhiteSpace(ProjectNumber) ||
                string.IsNullOrWhiteSpace(ProjectName) ||
                string.IsNullOrWhiteSpace(UsedStandard) ||
                string.IsNullOrWhiteSpace(AccessPath))
            {
                // Afficher un message d'erreur à l'utilisateur
                ShowMessage("Veuillez remplir tous les champs obligatoires.");
                return;
            }

            var projectModel = new ProjectModel
            {
                ProjectNumber = this.ProjectNumber,
                ProjectName = this.ProjectName,
                UsedStandard = this.UsedStandard,
                AccessPath = this.AccessPath
            };

            SaveProject(projectModel);
            UpdateIndex(projectModel.AccessPath, projectModel.ProjectName);

            // Naviguer vers la page d'accueil ou rafraîchir la liste des projets
            // Cette partie dépend de votre implémentation spécifique
            ShowMessage("Projet créé avec succès !");
            // Par exemple, vous pouvez appeler une méthode pour revenir à la page d'accueil
        }

        // Méthode pour sauvegarder le projet dans un fichier .arm
        private void SaveProject(ProjectModel projectModel)
        {
            try
            {
                // Sérialiser le projet en JSON
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(projectModel, options);

                // Vérifier si le dossier existe, sinon le créer
                if (!Directory.Exists(projectModel.AccessPath))
                {
                    Directory.CreateDirectory(projectModel.AccessPath);
                }

                // Construire le chemin complet du fichier .arm
                string filePath = Path.Combine(projectModel.AccessPath, projectModel.ProjectName + ".arm");

                // Enregistrer le fichier
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                // Gérer les exceptions
                ShowMessage("Erreur lors de la sauvegarde du projet : " + ex.Message);
            }
        }

        // Méthode pour mettre à jour l'index des projets
        private void UpdateIndex(string projectPath, string projectName)
        {
            try
            {
                string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string applicationFolder = Path.Combine(appDataFolder, "CADARM"); // Remplacez par le nom de votre application

                // Vérifier si le dossier de l'application existe, sinon le créer
                if (!Directory.Exists(applicationFolder))
                {
                    Directory.CreateDirectory(applicationFolder);
                }

                string indexPath = Path.Combine(applicationFolder, "index.json");
                List<string> projects;

                // Vérifier si l'index existe déjà
                if (File.Exists(indexPath))
                {
                    // Lire l'index existant
                    string jsonIndex = File.ReadAllText(indexPath);
                    projects = JsonSerializer.Deserialize<List<string>>(jsonIndex);
                }
                else
                {
                    projects = new List<string>();
                }

                // Ajouter le nouveau projet à la liste s'il n'est pas déjà présent
                string projectFilePath = Path.Combine(projectPath, projectName + ".arm");
                if (!projects.Contains(projectFilePath))
                {
                    projects.Add(projectFilePath);
                }

                // Sérialiser et sauvegarder l'index mis à jour
                var options = new JsonSerializerOptions { WriteIndented = true };
                string newJsonIndex = JsonSerializer.Serialize(projects, options);
                File.WriteAllText(indexPath, newJsonIndex);
            }
            catch (Exception ex)
            {
                // Gérer les exceptions
                ShowMessage("Erreur lors de la mise à jour de l'index : " + ex.Message);
            }
        }

        // Méthode pour parcourir les dossiers et sélectionner le chemin d'accès
        private async Task BrowseFolder()
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Sélectionner le dossier du projet"
            };

            string result = await dialog.ShowAsync(window);

            if (!string.IsNullOrEmpty(result))
            {
                AccessPath = result;
            }
        }

        // Méthode pour afficher un message à l'utilisateur
        private void ShowMessage(string message)
        {
            // Vous pouvez utiliser un MessageBox ou un autre mécanisme pour afficher le message
            // Exemple avec MessageBox.Avalonia
             
        }
    }
}
