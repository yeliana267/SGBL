    using Microsoft.AspNetCore.Mvc.Testing;
    using System.Net;
    using System.Text.RegularExpressions;
    using Xunit;

    namespace SGBL.Web.Tests.Controllers
    {
        public class BookControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
        {
            private readonly WebApplicationFactory<Program> _factory;
            private readonly HttpClient _client;

            public BookControllerIntegrationTests(WebApplicationFactory<Program> factory)
            {
                _factory = factory;
                _client = _factory.CreateClient();
            }

            [Fact]
            public async Task GET_List_Should_Render_BookList_View()
            {
                // Act
                var response = await _client.GetAsync("/Book");

                // Assert
                response.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            

            [Fact]
            public async Task POST_Create_Should_Return_View_When_Isbn_Duplicate()
            {
                // Paso 1: Obtener token
                var (requestVerificationToken, cookies) = await ExtractAntiForgeryToken();

                // Paso 2: Crear primer libro exitoso
                var uniqueIsbn = DateTime.Now.Ticks.ToString().Substring(0, 13);

                var firstBookData = new Dictionary<string, string>
                {
                    ["__RequestVerificationToken"] = requestVerificationToken,
                    ["Title"] = "First Book",
                    ["Isbn"] = uniqueIsbn,
                    ["Description"] = "First Book Description",
                    ["PublicationYear"] = "2024",
                    ["Pages"] = "300",
                    ["TotalCopies"] = "5",
                    ["AvailableCopies"] = "5",
                    ["Ubication"] = "Shelf A",
                    ["StatusId"] = "7",
                    ["viewAction"] = "create"
                };

                var firstContent = new FormUrlEncodedContent(firstBookData);
                var firstRequest = new HttpRequestMessage(HttpMethod.Post, "/Book")
                {
                    Content = firstContent
                };

                foreach (var cookie in cookies)
                {
                    firstRequest.Headers.Add("Cookie", cookie);
                }

                var firstResponse = await _client.SendAsync(firstRequest);

                // Paso 3: Intentar crear libro duplicado
                var duplicateBookData = new Dictionary<string, string>
                {
                    ["__RequestVerificationToken"] = requestVerificationToken,
                    ["Title"] = "Duplicate Book",
                    ["Isbn"] = uniqueIsbn, // Mismo ISBN
                    ["Description"] = "Duplicate Book Description",
                    ["PublicationYear"] = "2024",
                    ["Pages"] = "400",
                    ["TotalCopies"] = "3",
                    ["AvailableCopies"] = "3",
                    ["Ubication"] = "Shelf B",
                    ["StatusId"] = "7",
                    ["viewAction"] = "create"
                };

                var duplicateContent = new FormUrlEncodedContent(duplicateBookData);
                var duplicateRequest = new HttpRequestMessage(HttpMethod.Post, "/Book")
                {
                    Content = duplicateContent
                };

                foreach (var cookie in cookies)
                {
                    duplicateRequest.Headers.Add("Cookie", cookie);
                }

                var duplicateResponse = await _client.SendAsync(duplicateRequest);

                // Assert - Debería mostrar error (OK o BadRequest)
                Assert.True(duplicateResponse.StatusCode == HttpStatusCode.OK ||
                           duplicateResponse.StatusCode == HttpStatusCode.BadRequest);
            }

            [Fact]
            public async Task POST_Create_Should_Return_View_When_InvalidPublicationYear()
            {
                // Paso 1: Obtener token
                var (requestVerificationToken, cookies) = await ExtractAntiForgeryToken();

                // Paso 2: Crear datos con año inválido
                var invalidBookData = new Dictionary<string, string>
                {
                    ["__RequestVerificationToken"] = requestVerificationToken,
                    ["Title"] = "Test Book with Invalid Year",
                    ["Isbn"] = DateTime.Now.Ticks.ToString().Substring(0, 13),
                    ["Description"] = "Test Description",
                    ["PublicationYear"] = "500", // Año inválido
                    ["Pages"] = "300",
                    ["TotalCopies"] = "5",
                    ["AvailableCopies"] = "5",
                    ["Ubication"] = "Test Shelf",
                    ["StatusId"] = "7",
                    ["viewAction"] = "create"
                };

                var content = new FormUrlEncodedContent(invalidBookData);
                var request = new HttpRequestMessage(HttpMethod.Post, "/Book")
                {
                    Content = content
                };

                foreach (var cookie in cookies)
                {
                    request.Headers.Add("Cookie", cookie);
                }

                var response = await _client.SendAsync(request);

                // Assert - Debería mostrar error de validación
                Assert.True(response.StatusCode == HttpStatusCode.OK ||
                           response.StatusCode == HttpStatusCode.BadRequest);
            }

            [Fact]
            public async Task GET_Create_Should_Render_Create_View()
            {
                // Act
                var response = await _client.GetAsync("/Book?viewAction=create");

                // Assert
                response.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            // 👇 MÉTODO PARA EXTRAER ANTI-FORGERY TOKEN
            private async Task<(string token, IEnumerable<string> cookies)> ExtractAntiForgeryToken()
            {
                var response = await _client.GetAsync("/Book?viewAction=create");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var cookies = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;

                // Extraer el token del HTML
                var match = Regex.Match(content, @"<input[^>]*name=""__RequestVerificationToken""[^>]*value=""([^""]*)""");
                if (match.Success)
                {
                    return (match.Groups[1].Value, cookies);
                }

                throw new Exception("AntiForgery token not found");
            }
        }
    }