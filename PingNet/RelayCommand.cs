using System;
using System.Windows.Input;

namespace PingNet
{
    /// <summary>
    /// These delegates store methods to be called that contain the body of the Execute and CanExecute methods
    /// for each particular instance of RelayCommand
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region Private members
        private readonly Predicate<object> _CanExecute;
        private readonly Action<object> _Execute;
        #endregion

        #region Events
        /// <summary>
        /// Can execute changed event
        /// </summary>
        public event EventHandler CanExecuteChanged;
        #endregion

        #region Constructors
        /// <summary>
        /// Relays the command
        /// </summary>
        /// <param name="execute">The command to execute</param>
        /// <param name="canExecute">Predicate for can execute</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _Execute = execute;
            _CanExecute = canExecute;
        }
        
        /// <summary>
        /// Relays the command but doesnt check if it can be executed
        /// </summary>
        /// <param name="execute">The command to execute</param>
        public RelayCommand(Action<object> execute)
            : this (execute, null)
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Determines if the passed command can be executed
        /// </summary>
        /// <param name="parameter">The command to evaluate</param>
        /// <returns>Returns true if the command can be executed</returns>
        public bool CanExecute(object parameter)
        {
            return _CanExecute == null ? true : _CanExecute(parameter);
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="parameter">The command to execute</param>
        public void Execute(object parameter)
        {
            if (!CanExecute(parameter))
                return;

            _Execute(parameter);
        }

        /// <summary>
        /// Trigger a manual refresh on the result of CanExecute
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
