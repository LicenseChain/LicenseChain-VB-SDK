# LicenseChain VB.NET SDK

[![License](https://img.shields.io/badge/license-Elastic--2.0-blue.svg)](LICENSE)
[![VB.NET](https://img.shields.io/badge/VB.NET-8.0+-blue.svg)](https://docs.microsoft.com/en-us/dotnet/visual-basic/)
[![.NET](https://img.shields.io/badge/.NET-6.0+-green.svg)](https://dotnet.microsoft.com/)

Official VB.NET SDK for LicenseChain - Secure license management for VB.NET applications.

## 🚀 Features

- **🔐 Secure Authentication** - User registration, login, and session management
- **📜 License Management** - Create, validate, update, and revoke licenses
- **🛡 Hardware ID Validation** - Prevent license sharing and unauthorized access
- **🔔 Webhook Support** - Real-time license events and notifications
- **📊 Analytics Integration** - Track license usage and performance metrics
- **⚡ High Performance** - Optimized for production workloads
- **🔄 Async Operations** - Non-blocking HTTP requests and data processing
- **🛠 Easy Integration** - Simple API with comprehensive documentation


## License assertion JWT (RS256 + JWKS)

**API base:** `https://api.licensechain.app/v1`

Successful **`POST /v1/licenses/verify`** may include **`license_token`**, **`license_token_expires_at`**, and **`license_jwks_uri`**. The JWT must carry **`token_use`** = **`licensechain_license_v1`**.

**RS256 + JWKS:** Verify using keys from **`GET /v1/licenses/jwks`** (or the returned **`license_jwks_uri`**). Use **`VerifyLicenseAssertionJwtAsync`** from the shared .NET stack (same as C#).

**Runnable (this repo):** `dotnet run --project examples/jwks_only/jwks_only.vbproj` with **`LICENSECHAIN_LICENSE_TOKEN`** and **`LICENSECHAIN_LICENSE_JWKS_URI`** (optional **`LICENSECHAIN_EXPECTED_APP_ID`**). **C# reference:** [LicenseChain-CSharp-SDK/examples/jwks_only](https://github.com/LicenseChain/LicenseChain-CSharp-SDK/tree/main/examples/jwks_only). See [THIN_CLIENT_PARITY](https://docs.licensechain.app/).

## 📦 Installation

### Method 1: NuGet Package (Recommended)

```bash
# Install via Package Manager Console
Install-Package LicenseChain.SDK

# Or via .NET CLI
dotnet add package LicenseChain.SDK
```

### Method 2: PackageReference

Add to your `.vbproj`:

```xml
<PackageReference Include="LicenseChain.SDK" Version="1.0.0" />
```

### Method 3: Manual Installation

1. Download the latest release from [GitHub Releases](https://github.com/LicenseChain/LicenseChain-VB-SDK/releases)
2. Add the DLL reference to your project
3. Install required dependencies

## 🚀 Quick Start

### Basic Setup

```vb
Imports LicenseChain.SDK

Module Program
    Async Function Main(args As String()) As Task
        ' Initialize the client
        Dim client As New LicenseChainClient(New LicenseChainConfig With {
            .ApiKey = "your-api-key",
            .AppName = "your-app-name",
            .Version = "1.0.0"
        })
        
        ' Connect to LicenseChain
        Try
            Await client.ConnectAsync()
            Console.WriteLine("Connected to LicenseChain successfully!")
        Catch ex As LicenseChainException
            Console.WriteLine($"Failed to connect: {ex.Message}")
            Return
        End Try
    End Function
End Module
```

### User Authentication

```vb
' Register a new user
Try
    Dim user As User = Await client.RegisterAsync("username", "password", "email@example.com")
    Console.WriteLine("User registered successfully!")
    Console.WriteLine($"User ID: {user.Id}")
Catch ex As LicenseChainException
    Console.WriteLine($"Registration failed: {ex.Message}")
End Try

' Login existing user
Try
    Dim user As User = Await client.LoginAsync("username", "password")
    Console.WriteLine("User logged in successfully!")
    Console.WriteLine($"Session ID: {user.SessionId}")
Catch ex As LicenseChainException
    Console.WriteLine($"Login failed: {ex.Message}")
End Try
```

### License Management

```vb
' Validate a license
Try
    Dim license As License = Await client.ValidateLicenseAsync("LICENSE-KEY-HERE")
    Console.WriteLine("License is valid!")
    Console.WriteLine($"License Key: {license.Key}")
    Console.WriteLine($"Status: {license.Status}")
    Console.WriteLine($"Expires: {license.Expires}")
    Console.WriteLine($"Features: {String.Join(", ", license.Features)}")
    Console.WriteLine($"User: {license.User}")
Catch ex As LicenseChainException
    Console.WriteLine($"License validation failed: {ex.Message}")
End Try

' Get user's licenses
Try
    Dim licenses As List(Of License) = Await client.GetUserLicensesAsync()
    Console.WriteLine($"Found {licenses.Count} licenses:")
    For i As Integer = 0 To licenses.Count - 1
        Dim license As License = licenses(i)
        Console.WriteLine($"  {i + 1}. {license.Key} - {license.Status} (Expires: {license.Expires})")
    Next
Catch ex As LicenseChainException
    Console.WriteLine($"Failed to get licenses: {ex.Message}")
End Try
```

### Hardware ID Validation

```vb
' Get hardware ID (automatically generated)
Dim hardwareId As String = client.GetHardwareId()
Console.WriteLine($"Hardware ID: {hardwareId}")

' Validate hardware ID with license
Try
    Dim isValid As Boolean = Await client.ValidateHardwareIdAsync("LICENSE-KEY-HERE", hardwareId)
    If isValid Then
        Console.WriteLine("Hardware ID is valid for this license!")
    Else
        Console.WriteLine("Hardware ID is not valid for this license.")
    End If
Catch ex As LicenseChainException
    Console.WriteLine($"Hardware ID validation failed: {ex.Message}")
End Try
```

### Webhook Integration

```vb
' Set up webhook handler
client.SetWebhookHandler(Async Function(eventName As String, data As Dictionary(Of String, String))
    Console.WriteLine($"Webhook received: {eventName}")
    
    Select Case eventName
        Case "license.created"
            Console.WriteLine($"New license created: {data("licenseKey")}")
        Case "license.updated"
            Console.WriteLine($"License updated: {data("licenseKey")}")
        Case "license.revoked"
            Console.WriteLine($"License revoked: {data("licenseKey")}")
    End Select
End Function)

' Start webhook listener
Await client.StartWebhookListenerAsync()
```

## 📚 API Reference

### LicenseChainClient

#### Constructor

```vb
Dim client As New LicenseChainClient(New LicenseChainConfig With {
    .ApiKey = "your-api-key",
    .AppName = "your-app-name",
    .Version = "1.0.0",
    .BaseUrl = "https://api.licensechain.app" ' Optional
})
```

#### Methods

##### Connection Management

```vb
' Connect to LicenseChain
Await client.ConnectAsync()

' Disconnect from LicenseChain
Await client.DisconnectAsync()

' Check connection status
Dim isConnected As Boolean = client.IsConnected
```

##### User Authentication

```vb
' Register a new user
Dim user As User = Await client.RegisterAsync(username, password, email)

' Login existing user
Dim user As User = Await client.LoginAsync(username, password)

' Logout current user
Await client.LogoutAsync()

' Get current user info
Dim user As User = Await client.GetCurrentUserAsync()
```

##### License Management

```vb
' Validate a license
Dim license As License = Await client.ValidateLicenseAsync(licenseKey)

' Get user's licenses
Dim licenses As List(Of License) = Await client.GetUserLicensesAsync()

' Create a new license
Dim license As License = Await client.CreateLicenseAsync(userId, features, expires)

' Update a license
Dim license As License = Await client.UpdateLicenseAsync(licenseKey, updates)

' Revoke a license
Await client.RevokeLicenseAsync(licenseKey)

' Extend a license
Dim license As License = Await client.ExtendLicenseAsync(licenseKey, days)
```

##### Hardware ID Management

```vb
' Get hardware ID
Dim hardwareId As String = client.GetHardwareId()

' Validate hardware ID
Dim isValid As Boolean = Await client.ValidateHardwareIdAsync(licenseKey, hardwareId)

' Bind hardware ID to license
Await client.BindHardwareIdAsync(licenseKey, hardwareId)
```

##### Webhook Management

```vb
' Set webhook handler
client.SetWebhookHandler(handler)

' Start webhook listener
Await client.StartWebhookListenerAsync()

' Stop webhook listener
Await client.StopWebhookListenerAsync()
```

##### Analytics

```vb
' Track event
Await client.TrackEventAsync(eventName, properties)

' Get analytics data
Dim analytics As Analytics = Await client.GetAnalyticsAsync(timeRange)
```

## 🔧 Configuration

### App Settings

Add to your `appsettings.json`:

```json
{
  "LicenseChain": {
    "ApiKey": "your-api-key",
    "AppName": "your-app-name",
    "Version": "1.0.0",
    "BaseUrl": "https://api.licensechain.app",
    "Timeout": 30,
    "Retries": 3,
    "Debug": false
  }
}
```

### Dependency Injection

```vb
' In Startup.vb or Program.vb
services.AddLicenseChain(Sub(options)
    options.ApiKey = Configuration("LicenseChain:ApiKey")
    options.AppName = Configuration("LicenseChain:AppName")
    options.Version = Configuration("LicenseChain:Version")
End Sub)
```

### Environment Variables

Set these in your environment or through your build process:

```bash
# Required
export LICENSECHAIN_API_KEY=your-api-key
export LICENSECHAIN_APP_NAME=your-app-name
export LICENSECHAIN_APP_VERSION=1.0.0

# Optional
export LICENSECHAIN_BASE_URL=https://api.licensechain.app
export LICENSECHAIN_DEBUG=true
```

## 🛡 Security Features

### Hardware ID Protection

The SDK automatically generates and manages hardware IDs to prevent license sharing:

```vb
' Hardware ID is automatically generated and stored
Dim hardwareId As String = client.GetHardwareId()

' Validate against license
Dim isValid As Boolean = Await client.ValidateHardwareIdAsync(licenseKey, hardwareId)
```

### Secure Communication

- All API requests use HTTPS
- API keys are securely stored and transmitted
- Session tokens are automatically managed
- Webhook signatures are verified

### License Validation

- Real-time license validation
- Hardware ID binding
- Expiration checking
- Feature-based access control

## 📊 Analytics and Monitoring

### Event Tracking

```vb
' Track custom events
Await client.TrackEventAsync("app.started", New Dictionary(Of String, Object) From {
    {"level", 1},
    {"playerCount", 10}
})

' Track license events
Await client.TrackEventAsync("license.validated", New Dictionary(Of String, Object) From {
    {"licenseKey", "LICENSE-KEY"},
    {"features", "premium,unlimited"}
})
```

### Performance Monitoring

```vb
' Get performance metrics
Dim metrics As PerformanceMetrics = Await client.GetPerformanceMetricsAsync()
Console.WriteLine($"API Response Time: {metrics.AverageResponseTime}ms")
Console.WriteLine($"Success Rate: {metrics.SuccessRate:P}")
Console.WriteLine($"Error Count: {metrics.ErrorCount}")
```

## 🔄 Error Handling

### Custom Exception Types

```vb
Try
    Dim license As License = Await client.ValidateLicenseAsync("invalid-key")
Catch ex As InvalidLicenseException
    Console.WriteLine("License key is invalid")
Catch ex As ExpiredLicenseException
    Console.WriteLine("License has expired")
Catch ex As NetworkException
    Console.WriteLine("Network connection failed")
Catch ex As LicenseChainException
    Console.WriteLine($"LicenseChain error: {ex.Message}")
End Try
```

### Retry Logic

```vb
' Automatic retry for network errors
Dim client As New LicenseChainClient(New LicenseChainConfig With {
    .ApiKey = "your-api-key",
    .AppName = "your-app-name",
    .Version = "1.0.0",
    .Retries = 3, ' Retry up to 3 times
    .RetryDelay = TimeSpan.FromSeconds(1) ' Wait 1 second between retries
})
```

## 🧪 Testing

### Unit Tests

```bash
# Run tests
dotnet test
```

### Integration Tests

```bash
# Test with real API
dotnet test --filter Category=Integration
```

## 📝 Examples

See the `examples/` directory for complete examples:

- `BasicUsageExample.vb` - Basic SDK usage
- `AdvancedFeaturesExample.vb` - Advanced features and configuration
- `WebhookIntegrationExample.vb` - Webhook handling

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guide](CONTRIBUTING.md) for details.

### Development Setup

1. Clone the repository
2. Install .NET 6.0 or later
3. Build: `dotnet build`
4. Test: `dotnet test`

## 📄 License

This project is licensed under the Elastic License 2.0 (ELv2) — see the [LICENSE](LICENSE) file for details.

## 🆘 Support

- **Documentation**: [https://docs.licensechain.app/vb](https://docs.licensechain.app/vb)
- **Issues**: [GitHub Issues](https://github.com/LicenseChain/LicenseChain-VB-SDK/issues)
- **Discord**: [LicenseChain Discord](https://discord.gg/licensechain)
- **Email**: support@licensechain.app

## 🔗 Related Projects

- [LicenseChain JavaScript SDK](https://github.com/LicenseChain/LicenseChain-JavaScript-SDK)
- [LicenseChain Python SDK](https://github.com/LicenseChain/LicenseChain-Python-SDK)
- [LicenseChain Node.js SDK](https://github.com/LicenseChain/LicenseChain-NodeJS-SDK)
---

**Made with ❤️ for the VB.NET community**


## API Endpoints

All endpoints automatically use the /v1 prefix when connecting to https://api.licensechain.app.

### Base URL
- **Production**: https://api.licensechain.app/v1\n- **Development**: https://api.licensechain.app/v1\n\n### Available Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /v1/health | Health check |
| POST | /v1/auth/login | User login |
| POST | /v1/auth/register | User registration |
| GET | /v1/apps | List applications |
| POST | /v1/apps | Create application |
| GET | /v1/licenses | List licenses |
| POST | /v1/licenses/verify | Verify license |
| GET | /v1/webhooks | List webhooks |
| POST | /v1/webhooks | Create webhook |
| GET | /v1/analytics | Get analytics |

**Note**: The SDK automatically prepends /v1 to all endpoints, so you only need to specify the path (e.g., /auth/login instead of /v1/auth/login).


## LicenseChain API (v1)

This SDK targets the **LicenseChain HTTP API v1** implemented by the LicenseChain API service.

- **Production base URL:** https://api.licensechain.app/v1
- **API reference:** [docs.licensechain.app](https://docs.licensechain.app/)
- **Baseline REST mapping (documented for integrators):**
  - GET /health
  - POST /auth/register
  - POST /licenses/verify
  - PATCH /licenses/:id/revoke
  - PATCH /licenses/:id/activate
  - PATCH /licenses/:id/extend
  - GET /analytics/stats

