using System;
using System.Windows.Input;

namespace Automation.GenerativeAI.UX.Core
{
    /// <summary>
    /// Implements RelayCommand by wraping an execute action
    /// </summary>
    public class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="execute">Execute action that takes an object as input parameter</param>
        /// <param name="canExecute">A predicate function to check if the command can execute.</param>
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Checks if the command can execute for the given parameter
        /// </summary>
        /// <param name="parameter">Input parameter</param>
        /// <returns>True if command can execute</returns>
        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        /// <summary>
        /// Executes the command with the given input parameter
        /// </summary>
        /// <param name="parameter">Input parameter</param>
        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }
}
