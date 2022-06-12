using System;
using System.Windows.Input;

namespace DijkstraVisualization.Util
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _exec;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> exec, Func<object, bool> canExecute)
        {
            _exec = exec;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter) => _canExecute.Invoke(parameter);

        public void Execute(object parameter)
        {
            if (!CanExecute(parameter))
                return;

            _exec?.Invoke(parameter);
        }
    }
}
