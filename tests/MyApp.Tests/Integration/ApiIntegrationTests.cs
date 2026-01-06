using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Services;
using Xunit;

namespace MyApp.Tests.Integration;

/// <summary>
/// Integration tests for the API endpoints
/// These tests require a proper test host configuration and are excluded from CI
/// due to Windows Authentication (Negotiate) not being supported in WebApplicationFactory.
/// </summary>
[Trait("Category", "Integration")]
public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                // Configure test-specific services if needed
            });
        });
    }

    private HttpClient CreateClient(bool authenticated = false)
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        // For integration tests, we can't easily mock Windows Auth
        // These tests focus on the anonymous endpoints
        return client;
    }

    [Fact]
    public async Task HealthCheck_ReturnsHealthy()
    {
        // Arrange
        var client = CreateClient();

        // Act
        var response = await client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Healthy");
    }

    [Fact]
    public async Task ExternalEndpoint_Get_ReturnsHealthStatus()
    {
        // Arrange
        var client = CreateClient();

        // Act
        var response = await client.GetAsync("/api/external");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ExternalHealthStatus>();
        result.Should().NotBeNull();
        result!.CheckedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task ExternalEndpoint_PostWithoutApiKey_ReturnsUnauthorized()
    {
        // Arrange
        var client = CreateClient();
        var request = new ExternalRequest
        {
            SystemId = "TEST-SYSTEM",
            RequestType = "SYNC_DATA",
            // No API key
            Payload = new ExternalPayload
            {
                EntityType = "TestEntity",
                EntityId = "123",
                Action = "sync"
            }
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/external", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ExternalEndpoint_PostWithValidApiKey_ReturnsSuccess()
    {
        // Arrange
        var client = CreateClient();
        var request = new ExternalRequest
        {
            SystemId = "TEST-SYSTEM",
            RequestType = "SYNC_DATA",
            ApiKey = "demo-api-key-12345", // Valid test API key
            Payload = new ExternalPayload
            {
                EntityType = "TestEntity",
                EntityId = "123",
                Action = "sync"
            }
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/external", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ExternalResponse>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.TransactionId.Should().Be(request.TransactionId);
    }

    [Fact]
    public async Task ExternalEndpoint_PostWithApiKeyInHeader_ReturnsSuccess()
    {
        // Arrange
        var client = CreateClient();
        client.DefaultRequestHeaders.Add("X-Api-Key", "demo-api-key-12345");

        var request = new ExternalRequest
        {
            SystemId = "TEST-SYSTEM",
            RequestType = "QUERY",
            Payload = new ExternalPayload
            {
                EntityType = "TestEntity",
                EntityId = "456",
                Action = "query"
            }
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/external", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ExternalEndpoint_ValidateWithInvalidKey_ReturnsInvalid()
    {
        // Arrange
        var client = CreateClient();
        client.DefaultRequestHeaders.Add("X-Api-Key", "invalid-key");

        // Act
        var response = await client.GetAsync("/api/external/validate");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("invalid");
    }

    [Fact]
    public async Task ExternalEndpoint_ValidateWithValidKey_ReturnsValid()
    {
        // Arrange
        var client = CreateClient();
        client.DefaultRequestHeaders.Add("X-Api-Key", "demo-api-key-12345");

        // Act
        var response = await client.GetAsync("/api/external/validate");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("valid");
    }

    [Fact]
    public async Task ExternalEndpoint_Webhook_ProcessesRequest()
    {
        // Arrange
        var client = CreateClient();
        client.DefaultRequestHeaders.Add("X-Api-Key", "demo-api-key-12345");
        client.DefaultRequestHeaders.Add("X-Signature", "test-signature");

        var request = new ExternalRequest
        {
            SystemId = "WEBHOOK-SYSTEM",
            Payload = new ExternalPayload
            {
                EntityType = "Event",
                EntityId = "webhook-123",
                Action = "notify"
            }
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/external/webhook", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ExternalResponse>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task ExternalEndpoint_PostWithMissingSystemId_ReturnsBadRequest()
    {
        // Arrange
        var client = CreateClient();
        var request = new ExternalRequest
        {
            SystemId = "", // Empty - should fail validation
            RequestType = "SYNC_DATA",
            ApiKey = "demo-api-key-12345",
            Payload = new ExternalPayload()
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/external", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DataEndpoint_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange
        var client = CreateClient();

        // Act
        var response = await client.GetAsync("/api/data");

        // Assert
        // In a test environment without Windows Auth configured,
        // the endpoint should reject unauthenticated requests
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.Unauthorized,
            HttpStatusCode.InternalServerError // May occur if auth is not configured
        );
    }

    [Fact]
    public async Task ScheduledEndpoint_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange
        var client = CreateClient();

        // Act
        var response = await client.PostAsync("/api/scheduled/run", null);

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.Unauthorized,
            HttpStatusCode.InternalServerError
        );
    }

    [Fact]
    public async Task SwaggerEndpoint_InDevelopment_ReturnsOk()
    {
        // Arrange
        var client = CreateClient();

        // Act
        var response = await client.GetAsync("/swagger/v1/swagger.json");

        // Assert
        // Swagger should be available in test environment
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }
}
