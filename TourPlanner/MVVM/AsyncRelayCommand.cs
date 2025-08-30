using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SWEN2_TourPlannerGroupProject.MVVM
{
    internal class AsyncRelayCommand : ICommand
    {
        // Keep this private field, but we'll expose it safely via ExecuteAsync
        private readonly Func<object?, Task> _executeAsync;
        private readonly Predicate<object?>? _canExecute;

        public AsyncRelayCommand(Func<object?, Task> executeAsync, Predicate<object?>? canExecute = null)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        // normal ICommand Execute (for buttons)
        public async void Execute(object? parameter) => await _executeAsync(parameter);

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        // safely expose Task for tests
        public Task ExecuteAsync(object? parameter = null)
        {
            if (!CanExecute(parameter)) return Task.CompletedTask;
            return _executeAsync(parameter);
        }
    }
}
