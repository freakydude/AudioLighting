using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AudioLighting.ViewModels.Commands
{
    /// <inheritdoc cref="IAsyncCommand"/>
    /// <summary>
    /// A simple async command without error handling
    /// </summary>
    public class AsyncCommand : IAsyncCommand
    {
        public event EventHandler CanExecuteChanged;

        private bool isExecuting;
        private readonly Func<Task> execute;
        private readonly Func<bool> canExecute;

        public AsyncCommand(Func<Task> execute, Func<bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <inheritdoc />
        public bool CanExecute()
        {
            return !isExecuting && (canExecute?.Invoke() ?? true);
        }

        /// <inheritdoc />
        public async Task ExecuteAsync()
        {
            if (CanExecute())
            {
                try
                {
                    isExecuting = true;
                    RaiseCanExecuteChanged();
                    await execute();
                }
                finally
                {
                    isExecuting = false;
                    RaiseCanExecuteChanged();
                }
            }
        }

        protected void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        async void ICommand.Execute(object parameter)
        {
            await ExecuteAsync();
        }
    }

    /// <inheritdoc cref="IAsyncCommand{T}"/>
    /// <summary>
    /// A simple typed async command without error handling
    /// </summary>
    public class AsyncCommand<T> : IAsyncCommand<T>
    {
        public event EventHandler CanExecuteChanged;

        private bool isExecuting;
        private readonly Func<T, Task> execute;
        private readonly Func<T, bool> canExecute;

        public AsyncCommand(Func<T, Task> execute, Func<T, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <inheritdoc />
        public bool CanExecute(T parameter)
        {
            return !isExecuting && (canExecute?.Invoke(parameter) ?? true);
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(T parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    isExecuting = true;
                    RaiseCanExecuteChanged();
                    await execute(parameter);
                }
                finally
                {
                    isExecuting = false;
                    RaiseCanExecuteChanged();
                }
            }
        }

        protected void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute((T)parameter);
        }

        async void ICommand.Execute(object parameter)
        {
            await ExecuteAsync((T)parameter);
        }
    }
}