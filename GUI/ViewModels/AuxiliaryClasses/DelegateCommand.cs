using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.ViewModels.AuxiliaryClasses
{
    class DelegateCommand : ICommand
    {
        Action<object> execute;
        Func<object, bool> canExecute;
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }
        public bool CanExecute(object parameter)
        {
            if (canExecute != null) return canExecute(parameter);
            else return true;
        }
        public void Execute(object parameter) => execute?.Invoke(parameter);
        public DelegateCommand(Action<object> executeAction) : this (executeAction, null) { }
        public DelegateCommand(Action<object> executeAction, Func<object, bool> canExecuteFunc)
        {
            execute = executeAction;
            canExecute = canExecuteFunc;
        }
    }
}
