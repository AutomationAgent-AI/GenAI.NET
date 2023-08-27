using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Automation.GenerativeAI.UX.Core
{
    /// <summary>
    /// Implements RelayCommandAsync that runs a command asynchronously
    /// </summary>
    public class RelayCommandAsync : ICommand
    {
        private readonly Func<object, Task> _execute;
        private readonly Predicate<object> _canExecute;
        private bool isExecuting;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="execute">Execute function that can run asynchronously</param>
        public RelayCommandAsync(Func<object, Task> execute) : this(execute, null) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="execute">Execute function that can run asynchronously</param>
        /// <param name="canExecute">A predicate function to check if the command can execute</param>
        public RelayCommandAsync(Func<object, Task> execute, Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Checks if the command can execute witht the given parameter
        /// </summary>
        /// <param name="parameter">Input parameter passed by the system</param>
        /// <returns>True if the command can execute</returns>
        public bool CanExecute(object parameter)
        {
            if (!isExecuting && _canExecute == null) return true;
            return (!isExecuting && _canExecute(parameter));
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Async method to execute the command
        /// </summary>
        /// <param name="parameter">input parameter</param>
        public async void Execute(object parameter)
        {
            isExecuting = true;
            try { await _execute(parameter); }
            finally { isExecuting = false; }
        }
    }
}
