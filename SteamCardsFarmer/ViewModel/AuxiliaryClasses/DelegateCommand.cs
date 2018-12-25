using System;
using System.Windows.Input;

namespace SteamCardsFarmer.ViewModel.AuxiliaryClasses
{
    /// <summary>Класс делегатной команды</summary>
    class DelegateCommand : ICommand
    {
        Action<object> execute;
        Func<object, bool> canExecute;
        /// <summary>Обработчик события, реагирующий на изменение состояния возможности исполнения команд</summary>
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
        /// <summary>Проверка, может ли быть выполнена команда</summary>
        /// <param name="parameter">Проверяемая команда</param>
        public bool CanExecute(object parameter)
        {
            if (canExecute != null) return canExecute(parameter);
            else return true;
        }
        /// <summary>Метод, который выполняет делегируемый метод</summary>
        /// <param name="parameter">Команда, которую необходимо выполнить</param>
        public void Execute(object parameter) => execute?.Invoke(parameter);
        /// <summary>Конструктор класса для действия, которое не надо проверять на возможность исполнения</summary>
        /// <param name="executeAction">Выполняемое действие</param>
        public DelegateCommand(Action<object> executeAction) : this (executeAction, null) { }
        /// <summary>Конструктор класса для действия, которое необходимо проверить</summary>
        /// <param name="executeAction">Выполяемое действие</param>
        /// <param name="canExecuteFunc">Функция, которая определяет условия, необходимые для исполнения действия</param>
        public DelegateCommand(Action<object> executeAction, Func<object, bool> canExecuteFunc)
        {
            execute = executeAction;
            canExecute = canExecuteFunc;
        }
    }
}
