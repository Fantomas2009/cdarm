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
        // Impl�mentation de l'interface INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Propri�t�s li�es aux champs du formulaire
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

        // R�f�rence � la fen�tre active pour les dialogues
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

        // M�thode pour cr�er le projet
        private void CreateProject()
        {
            if (string.IsNullOrWhiteSpace(ProjectNumber) ||
                string.IsNullOrWhiteSpace(ProjectName) ||
                string.IsNullOrWhiteSpace(UsedStandard) ||
                string.IsNullOrWhiteSpace(AccessPath))
            {
                // Afficher un message d'erreur � l'utilisateur
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

            // Naviguer vers la page d'accueil ou rafra�chir la liste des projets
            // Cette partie d�pend de votre impl�mentation sp�cifique
            ShowMessage("Projet cr�� avec succ�s !");
            // Par exemple, vous pouvez appeler une m�thode pour revenir � la page d'accueil
        }

        // M�thode pour sauvegarder le projet dans un fichier .arm
        private void SaveProject(ProjectModel projectModel)
        {
            try
            {
                // S�rialiser le projet en JSON
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(projectModel, options);

                // V�rifier si le dossier existe, sinon le cr�er
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
                // G�rer les exceptions
                ShowMessage("Erreur lors de la sauvegarde du projet : " + ex.Message);
            }
        }

        // M�thode pour mettre � jour l'index des projets
        private void UpdateIndex(string projectPath, string projectName)
        {
            try
            {
                string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string applicationFolder = Path.Combine(appDataFolder, "CADARM"); // Remplacez par le nom de votre application

                // V�rifier si le dossier de l'application existe, sinon le cr�er
                if (!Directory.Exists(applicationFolder))
                {
                    Directory.CreateDirectory(applicationFolder);
                }

                string indexPath = Path.Combine(applicationFolder, "index.json");
                List<string> projects;

                // V�rifier si l'index existe d�j�
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

                // Ajouter le nouveau projet � la liste s'il n'est pas d�j� pr�sent
                string projectFilePath = Path.Combine(projectPath, projectName + ".arm");
                if (!projects.Contains(projectFilePath))
                {
                    projects.Add(projectFilePath);
                }

                // S�rialiser et sauvegarder l'index mis � jour
                var options = new JsonSerializerOptions { WriteIndented = true };
                string newJsonIndex = JsonSerializer.Serialize(projects, options);
                File.WriteAllText(indexPath, newJsonIndex);
            }
            catch (Exception ex)
            {
                // G�rer les exceptions
                ShowMessage("Erreur lors de la mise � jour de l'index : " + ex.Message);
            }
        }

        // M�thode pour parcourir les dossiers et s�lectionner le chemin d'acc�s
        private async Task BrowseFolder()
        {
            var dialog = new OpenFolderDialog
            {
                Title = "S�lectionner le dossier du projet"
            };

            string result = await dialog.ShowAsync(window);

            if (!string.IsNullOrEmpty(result))
            {
                AccessPath = result;
            }
        }

        // M�thode pour afficher un message � l'utilisateur
        private void ShowMessage(string message)
        {
            // Vous pouvez utiliser un MessageBox ou un autre m�canisme pour afficher le message
            // Exemple avec MessageBox.Avalonia
             
        }
    }
}
