using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SearchApi.IntegrationTests
{
    [TestFixture]
    internal class SearchControllerTests
    {
        private readonly HttpClient _client;

        public SearchControllerTests()
        {
            // Инициализация HttpClient для обращения к API
            _client = new HttpClient { BaseAddress = new Uri("/api/search/LimitedSearch") };
        }

        [Test]
        public async Task CkeckStatus_SendRequest_ShouldReturnOk()
        {
            // Arrange
            string searchString = "a";

            // Act
            var response = await _client.GetAsync($"/api/search/LimitedSearch?search={searchString}");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            string responseContent = await response.Content.ReadAsStringAsync();
            Assert.Equals("Ok", responseContent);

        }
    }
}
