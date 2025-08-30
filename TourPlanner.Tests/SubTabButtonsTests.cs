using NUnit.Framework;
using System.Threading.Tasks;
using System.Windows.Input;
using SWEN2_TourPlannerGroupProject.ViewModels;
using SWEN2_TourPlannerGroupProject.MVVM;

namespace SWEN2_TourPlannerGroupProject.Tests
{
    [TestFixture]
    internal class SubTabButtonsViewModelTests
    {
        private bool _addExecuted;
        private bool _deleteExecuted;
        private bool _updateExecuted;
        private bool _reportExecuted;

        private ICommand _addCommand;
        private ICommand _deleteCommand;
        private ICommand _updateCommand;
        private ICommand _reportCommand;

        [SetUp]
        public void Setup()
        {
            _addExecuted = false;
            _deleteExecuted = false;
            _updateExecuted = false;
            _reportExecuted = false;

            _addCommand = new AsyncRelayCommand(_ => { _addExecuted = true; return Task.CompletedTask; });
            _deleteCommand = new AsyncRelayCommand(_ => { _deleteExecuted = true; return Task.CompletedTask; });
            _updateCommand = new AsyncRelayCommand(_ => { _updateExecuted = true; return Task.CompletedTask; });
            _reportCommand = new AsyncRelayCommand(_ => { _reportExecuted = true; return Task.CompletedTask; });
        }

        [Test]
        public async Task Commands_ExecuteCorrectly_WhenProvided()
        {
            // Arrange
            var vm = new SubTabButtonsViewModel(_addCommand, _deleteCommand, _updateCommand, _reportCommand);

            // Act
            await vm.AddCommand.ExecuteAsync(null);
            await vm.DeleteCommand.ExecuteAsync(null);
            await vm.UpdateCommand.ExecuteAsync(null);
            await vm.ReportCommand.ExecuteAsync(null);

            // Assert
            Assert.IsTrue(_addExecuted, "AddCommand should execute");
            Assert.IsTrue(_deleteExecuted, "DeleteCommand should execute");
            Assert.IsTrue(_updateExecuted, "UpdateCommand should execute");
            Assert.IsTrue(_reportExecuted, "ReportCommand should execute");
        }

        [Test]
        public async Task Commands_Defaults_DoNotThrow_WhenNullProvided()
        {
            // Arrange: pass null for update/report to trigger default no-op commands
            var vm = new SubTabButtonsViewModel(_addCommand, _deleteCommand, null, null);

            // Act & Assert: Add/Delete should still work
            await vm.AddCommand.ExecuteAsync(null);
            await vm.DeleteCommand.ExecuteAsync(null);

            // Default commands for Update/Report should not throw
            Assert.DoesNotThrowAsync(async () => await vm.UpdateCommand.ExecuteAsync(null));
            Assert.DoesNotThrowAsync(async () => await vm.ReportCommand.ExecuteAsync(null));
        }

        [Test]
        public void DesignTimeConstructor_ProvidesNonNullCommands()
        {
            // Arrange & Act
            var vm = new SubTabButtonsViewModel();

            // Assert
            Assert.NotNull(vm.AddCommand);
            Assert.NotNull(vm.DeleteCommand);
            Assert.NotNull(vm.UpdateCommand);
            Assert.NotNull(vm.ReportCommand);
        }
    }

    // Extension method to simplify async command testing
    internal static class ICommandExtensions
    {
        public static Task ExecuteAsync(this ICommand command, object? parameter)
        {
            if (command is AsyncRelayCommand asyncCommand)
                return asyncCommand.ExecuteAsync(parameter);
            command.Execute(parameter);
            return Task.CompletedTask;
        }
    }
}
