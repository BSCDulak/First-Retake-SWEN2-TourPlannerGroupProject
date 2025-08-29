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
            var jsonResponse = @"{""features"":[{""geometry"":{""coordinates"":[16.3738,48.2082]}}]}";

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
            typeof(MapViewModel).GetField("_httpClient", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(_mapVM, httpClient);

            var method = _mapVM.GetType().GetMethod("GetCoordinatesAsync", BindingFlags.NonPublic | BindingFlags.Instance);
            var task = (Task<(double lat, double lng)?>)method!.Invoke(_mapVM, new object[] { "Vienna" })!;
            var coordinates = await task;

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
            typeof(MapViewModel).GetField("_httpClient", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(_mapVM, httpClient);

            var method = _mapVM.GetType().GetMethod("GetCoordinatesAsync", BindingFlags.NonPublic | BindingFlags.Instance);
            var task = (Task<(double lat, double lng)?>)method!.Invoke(_mapVM, new object[] { "Unknown" })!;
            var coordinates = await task;

            Assert.IsNull(coordinates);
        }

        #endregion

        #region JavaScript Generation

        [Test]
        public void GenerateRouteJavaScript_ContainsMarkersAndPolyline()
        {
            // Use explicit tuple names for start, end, and coordinates
            var start = (lat: 48.2082, lng: 16.3738);
            var end = (lat: 48.2100, lng: 16.3800);
            var coordinates = new[]
            {
                (lat: 48.2082, lng: 16.3738),
                (lat: 48.2100, lng: 16.3800)
            };

            var js = _mapVM.GetType()
                .GetMethod("GenerateRouteJavaScript", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.Invoke(_mapVM, new object[]
                {
                    start,
                    end,
                    coordinates,
                    "StartAddress",
                    "EndAddress",
                    "Walking"
                }) as string;

            Assert.IsNotNull(js, "Generated JS should not be null");
            Assert.IsTrue(js!.Contains("L.polyline"), "JS should contain polyline");
            Assert.IsTrue(js.Contains(start.lat.ToString()), "Start latitude should be in the JS");
            Assert.IsTrue(js.Contains(end.lat.ToString()), "End latitude should be in the JS");
            Assert.IsTrue(js.Contains("Walking"), "Transport type should be in the JS");
        }

        #endregion

        #region Map Update Event

        [Test]
        public async Task UpdateMap_FiresOnMapUpdateRequested_WhenTourHasNoCoordinates()
        {
            var tcs = new TaskCompletionSource<string>();
            _mapVM.OnMapUpdateRequested += html => tcs.TrySetResult(html);

            var emptyTour = new Tour { StartLocation = "", EndLocation = "" };
            _mapVM.SetSelectedTour(emptyTour);

            await Task.WhenAny(tcs.Task, Task.Delay(2000));

            Assert.IsTrue(tcs.Task.IsCompleted, "OnMapUpdateRequested should fire");
            Assert.IsTrue(tcs.Task.Result.Contains("Vienna"), "Default map should show Vienna");
        }

        [Test]
        public async Task UpdateMap_FiresOnMapUpdateRequested_WhenTourHasCoordinates()
        {
            var tcs = new TaskCompletionSource<string>();
            _mapVM.OnMapUpdateRequested += html => tcs.TrySetResult(html);

            _mapVM.SetSelectedTour(_sampleTour);

            await Task.WhenAny(tcs.Task, Task.Delay(5000));

            Assert.IsTrue(tcs.Task.IsCompleted, "OnMapUpdateRequested should fire");
            Assert.IsTrue(tcs.Task.Result.Contains("L.marker"), "Map HTML should contain markers");
        }

        #endregion
    }
}
