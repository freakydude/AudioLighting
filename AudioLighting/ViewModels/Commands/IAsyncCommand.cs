using System.Threading.Tasks;
using System.Windows.Input;

namespace AudioLighting.ViewModels.Commands
{
    /// <inheritdoc />
    public interface IAsyncCommand : ICommand
    {
        /// <summary>
        /// Async wrapper for <see cref="ICommand.Execute"/>
        /// </summary>
        Task ExecuteAsync();

        /// <summary>
        /// Async wrapper for <see cref="ICommand.CanExecute"/>
        /// </summary>
        bool CanExecute();
    }

    /// <inheritdoc />
    public interface IAsyncCommand<in T> : ICommand
    {
        /// <summary>
        /// Async wrapper for <see cref="ICommand.Execute"/>
        /// </summary>
        Task ExecuteAsync(T parameter);

        /// <summary>
        /// Async wrapper for <see cref="ICommand.CanExecute"/>
        /// </summary>
        bool CanExecute(T parameter);
    }
}