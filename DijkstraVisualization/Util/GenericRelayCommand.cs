using System;
using System.Windows.Input;

namespace DijkstraVisualization.Util
{
    public class GenericRelayCommand<T> : ICommand
    {
        private readonly Action<T> _exec;

        public GenericRelayCommand(Action<T> exec)
        {
            _exec = exec;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _exec?.Invoke((T)parameter);
        }
    }
}
