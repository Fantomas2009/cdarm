using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CADARM.ViewModels
{
    public class RelayCommand : ICommand
    {
        private readonly Func<Task> executeAsync;
        private readonly Action execute;
        private readonly Func<bool> canExecute;

        public event EventHandler CanExecuteChanged;

        // Constructeur pour les actions synchrones
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        // Constructeur pour les actions asynchrones
        public RelayCommand(Func<Task> executeAsync, Func<bool> canExecute = null)
        {
            this.executeAsync = executeAsync;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => canExecute?.Invoke() ?? true;

        public async void Execute(object parameter)
        {
            if (execute != null)
            {
                execute();
            }
            else if (executeAsync != null)
            {
                await executeAsync();
            }
        }

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
