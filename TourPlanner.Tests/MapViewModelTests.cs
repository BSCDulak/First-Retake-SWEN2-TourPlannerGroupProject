using NUnit.Framework;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SWEN2_TourPlannerGroupProject.ViewModels;
using SWEN2_TourPlannerGroupProject.Models;
using System;
using System.Reflection;

namespace SWEN2_TourPlannerGroupProject.Tests
{
    [TestFixture]
    internal class MapViewModelTests
    {
        private MapViewModel _mapVM;
        private Tour _sampleTour;

        [SetUp]
        public void Setup()
        {
            _mapVM = new MapViewModel();

            _sampleTour = new Tour
            {
                Name = "Test Tour",
                StartLocation = "Vienna",
                EndLocation = "Graz"
            };
        }

        #region Coordinate Fetching

        [Test]
        public async Task GetCoordinatesAsync_ReturnsCoordinates_WhenApiReturnsValidData()
        {
            // Arrange: mock HttpClient
            var jsonResponse = @"{
                ""features"": [
                    { ""geometry"": { ""coordinates"": [16.3738, 48.2082] } }
                ]
            }";

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                   ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(jsonResponse)
               });

            var httpClient = new HttpClient(handlerMock.Object);

            // Inject via reflection (since constructor uses private readonly)
            typeof(MapViewModel)
                .GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(_mapVM, httpClient);

            // Act
            var method = _mapVM.GetType()
                                     .GetMethod("GetCoordinatesAsync", BindingFlags.NonPublic | BindingFlags.Instance);
                                     
            var task = (Task<(double lat, double lng)?>)method!.Invoke(_mapVM, new object[] { "Vienna" })!;

            var coordinates = await task;

            // Assert
            Assert.IsNotNull(coordinates);
            Assert.AreEqual(48.2082, coordinates.Value.lat);
            Assert.AreEqual(16.3738, coordinates.Value.lng);
        }

        [Test]
        public async Task GetCoordinatesAsync_ReturnsNull_WhenApiReturnsEmptyFeatures()
        {
            var jsonResponse = @"{ ""features"": [] }";

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(jsonResponse)
               });

            var httpClient = new HttpClient(handlerMock.Object);
            typeof(MapViewModel).GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(_mapVM, httpClient);

            var method = _mapVM.GetType()
                                     .GetMethod("GetCoordinatesAsync", BindingFlags.NonPublic | BindingFlags.Instance);

            var task = (Task<(double lat, double lng)?>)method!.Invoke(_mapVM, new object[] { "Unknown" })!;

            var coordinates = await task;

            Assert.IsNull(coordinates);
        }

        #endregion

        #region JavaScript Generation

        [Test]
        public void GenerateRouteJavaScript_ContainsMarkersAndPolyline()
        {
            var coordinates = new (double lat, double lng)[] { (48.2082, 16.3738), (48.2100, 16.3800) };

            var js = _mapVM.GetType()
                .GetMethod("GenerateRouteJavaScript", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.Invoke(_mapVM, new object[]
                {
                    (48.2082, 16.3738),
                    (48.2100, 16.3800),
                    coordinates,
                    "StartAddress",
                    "EndAddress",
                    "Walking"
                }) as string;

            Assert.IsNotNull(js);
            Assert.IsTrue(js!.Contains("L.polyline"));
            Assert.IsTrue(js.Contains("48.2082"));
            Assert.IsTrue(js.Contains("Walking"));
        }

        #endregion

        #region Map Update Event

        [Test]
        public async Task UpdateMap_FiresOnMapUpdateRequested_WhenTourHasNoCoordinates()
        {
            bool eventFired = false;
            _mapVM.OnMapUpdateRequested += _ => eventFired = true;

            var emptyTour = new Tour { StartLocation = "", EndLocation = "" };
            _mapVM.SetSelectedTour(emptyTour);

            await Task.Delay(200); // allow async void UpdateMap to complete

            Assert.IsTrue(eventFired);
        }

        [Test]
        public async Task UpdateMap_FiresOnMapUpdateRequested_WhenTourHasCoordinates()
        {
            // We mock GetCoordinatesAsync to always return some coordinates
            var method = _mapVM.GetType().GetMethod("GetCoordinatesAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var coordinates = (48.2082, 16.3738);

            // Replace method using Moq? Not easily possible since it's private. So for integration test, we can just set a real tour with StartLocation/EndLocation to real strings
            bool eventFired = false;
            _mapVM.OnMapUpdateRequested += _ => eventFired = true;

            _mapVM.SetSelectedTour(_sampleTour);

            await Task.Delay(2000); // wait for async void UpdateMap to complete API call

            // We cannot assert exact route without network, but we can check the event fired
            Assert.IsTrue(eventFired);
        }

        #endregion
    }
}
