using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfApplication1.ViewModels
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> executeHandler;
        private readonly Func<object, bool> canExecuteHandler;

        public DelegateCommand(Action<object> executeHandler, Func<object, bool> canExecuteHandler = null)
        {
            this.executeHandler = executeHandler;
            this.canExecuteHandler = canExecuteHandler;
        }

        public bool CanExecute(object parameter)
        {
            if (this.canExecuteHandler != null)
                return canExecuteHandler(parameter);

            return true;
        }

        public void Execute(object parameter)
        {
            if (this.executeHandler != null)
                executeHandler(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
