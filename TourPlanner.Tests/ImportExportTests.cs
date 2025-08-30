using NUnit.Framework;
using SWEN2_TourPlannerGroupProject.Helpers;
using SWEN2_TourPlannerGroupProject.Models;
using SWEN2_TourPlannerGroupProject.DTOs;
using SWEN2_TourPlannerGroupProject.Logging;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;

namespace SWEN2_TourPlannerGroupProject.Tests.Helpers
{
    [TestFixture]
    public class ImportExportHelperTests
    {
        private ObservableCollection<Tour> _collection;
        private Mock<ILoggerWrapper> _loggerMock;
        private string _tempFilePath;

        [SetUp]
        public void SetUp()
        {
            _collection = new ObservableCollection<Tour>();
            _loggerMock = new Mock<ILoggerWrapper>();
            _tempFilePath = Path.Combine(Path.GetTempPath(), $"test_tours_{Guid.NewGuid()}.json");
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_tempFilePath))
                File.Delete(_tempFilePath);
        }

        private async Task CreateJsonFileWithTours(params TourDto[] dtos)
        {
            var json = JsonSerializer.Serialize(dtos, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_tempFilePath, json);
        }

        [Test]
        public async Task ImportToursAsync_ShouldImportTours()
        {
            // Arrange
            var tour1Id = Guid.NewGuid();
            var tour2Id = Guid.NewGuid();
            var dtos = new[]
            {
                new TourDto { Name = "Tour1", BackupId = tour1Id },
                new TourDto { Name = "Tour2", BackupId = tour2Id }
            };
            await CreateJsonFileWithTours(dtos);

            // Act
            await ImportExportHelper.ImportToursAsync(_collection, _tempFilePath, null, _loggerMock.Object);

            // Assert
            Assert.AreEqual(2, _collection.Count);
            Assert.IsTrue(_collection.Any(t => t.BackupId == tour1Id));
            Assert.IsTrue(_collection.Any(t => t.BackupId == tour2Id));
            _loggerMock.Verify(l => l.Info(It.Is<string>(s => s.Contains("Imported 2 tours"))), Times.Once);
        }

        [Test]
        public async Task ImportToursAsync_ShouldSkipDuplicatesInMemory()
        {
            // Arrange
            var duplicateId = Guid.NewGuid();
            _collection.Add(new Tour { Name = "ExistingTour", BackupId = duplicateId });

            var dtos = new[]
            {
                new TourDto { Name = "TourDuplicate", BackupId = duplicateId },
                new TourDto { Name = "TourNew", BackupId = Guid.NewGuid() }
            };
            await CreateJsonFileWithTours(dtos);

            // Act
            await ImportExportHelper.ImportToursAsync(_collection, _tempFilePath, null, _loggerMock.Object);

            // Assert
            Assert.AreEqual(2, _collection.Count); // ExistingTour + TourNew
            Assert.IsTrue(_collection.Any(t => t.Name == "TourNew"));
            _loggerMock.Verify(l => l.Warn(It.Is<string>(s => s.Contains("Skipping duplicate tour"))), Times.Once);
        }

        [Test]
        public async Task ExportTours_ShouldWriteJsonFile()
        {
            // Arrange
            var tour1Id = Guid.NewGuid();
            var tours = new List<Tour>
            {
                new Tour { Name = "Tour1", BackupId = tour1Id },
                new Tour { Name = "Tour2", BackupId = Guid.NewGuid() }
            };

            // Act
            ImportExportHelper.ExportTours(tours, _tempFilePath, _loggerMock.Object);

            // Assert
            Assert.IsTrue(File.Exists(_tempFilePath));
            var content = await File.ReadAllTextAsync(_tempFilePath);
            Assert.IsTrue(content.Contains("Tour1"));
            Assert.IsTrue(content.Contains("Tour2"));
            _loggerMock.Verify(l => l.Info(It.Is<string>(s => s.Contains("Exported 2 tours"))), Times.Once);
        }

        [Test]
        public void ExportTours_ShouldWarnOnNullTours()
        {
            // Act
            ImportExportHelper.ExportTours(null, _tempFilePath, _loggerMock.Object);

            // Assert
            _loggerMock.Verify(l => l.Warn(It.Is<string>(s => s.Contains("Tried to export null tour list"))), Times.Once);
        }
    }
}
