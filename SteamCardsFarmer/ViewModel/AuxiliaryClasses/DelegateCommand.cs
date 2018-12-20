using System;
using System.Windows.Input;

namespace SteamCardsFarmer.ViewModel.AuxiliaryClasses
{
    /// <summary>Класс делегирования команд</summary>
    class DelegateCommand : ICommand
    {
        Action<object> execute;
        Func<object, bool> canExecute;
        /// <summary>Обработчик события</summary>
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
        /// <summary>Проверка, может ли...</summary>
        /// <param name="parameter">...</param>
        public bool CanExecute(object parameter)
        {
            if (canExecute != null) return canExecute(parameter);
            else return true;
        }
        /// <summary></summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter) => execute?.Invoke(parameter);
        /// <summary>Конструктор класса</summary>
        /// <param name="executeAction"></param>
        public DelegateCommand(Action<object> executeAction) : this (executeAction, null) { }
        /// <summary>Конструктор класса</summary>
        /// <param name="executeAction"></param>
        /// <param name="canExecuteFunc"></param>
        public DelegateCommand(Action<object> executeAction, Func<object, bool> canExecuteFunc)
        {
            execute = executeAction;
            canExecute = canExecuteFunc;
        }
    }
}
