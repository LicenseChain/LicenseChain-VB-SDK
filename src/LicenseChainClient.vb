Imports System
Imports System.Net.Http
Imports System.Text
Imports System.Threading.Tasks
Imports System.Collections.Generic
Imports System.Security.Cryptography
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Namespace LicenseChain.VB

    ''' <summary>
    ''' Main client for interacting with the LicenseChain API
    ''' </summary>
    Public Class LicenseChainClient
        Implements IDisposable

        Private ReadOnly _httpClient As HttpClient
        Private ReadOnly _apiKey As String
        Private ReadOnly _baseUrl As String
        Private _disposed As Boolean = False

        ''' <summary>
        ''' Initializes a new instance of the LicenseChainClient
        ''' </summary>
        ''' <param name="apiKey">Your LicenseChain API key</param>
        ''' <param name="baseUrl">Base URL for the API (optional, defaults to production)</param>
        Public Sub New(apiKey As String, Optional baseUrl As String = "https://api.licensechain.app")
            If String.IsNullOrEmpty(apiKey) Then
                Throw New ArgumentException("API key is required", NameOf(apiKey))
            End If

            _apiKey = apiKey
            _baseUrl = baseUrl.TrimEnd("/"c)
            _httpClient = New HttpClient()
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}")
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "LicenseChain-VB-SDK/1.0.0")
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json")
        End Sub

        ' Authentication Methods

        ''' <summary>
        ''' Register a new user account
        ''' </summary>
        Public Async Function RegisterUserAsync(email As String, password As String, Optional name As String = Nothing, Optional company As String = Nothing) As Task(Of JObject)
            Dim payload = New With {
                .email = email,
                .password = password,
                .name = name,
                .company = company
            }
            Return Await PostAsync("/auth/register", payload)
        End Function

        ''' <summary>
        ''' Login with email and password
        ''' </summary>
        Public Async Function LoginAsync(email As String, password As String) As Task(Of JObject)
            Dim payload = New With {
                .email = email,
                .password = password
            }
            Return Await PostAsync("/auth/login", payload)
        End Function

        ''' <summary>
        ''' Logout the current user
        ''' </summary>
        Public Async Function LogoutAsync() As Task(Of JObject)
            Return Await PostAsync("/auth/logout", Nothing)
        End Function

        ''' <summary>
        ''' Refresh authentication token
        ''' </summary>
        Public Async Function RefreshTokenAsync(refreshToken As String) As Task(Of JObject)
            Dim payload = New With {
                .refresh_token = refreshToken
            }
            Return Await PostAsync("/auth/refresh", payload)
        End Function

        ''' <summary>
        ''' Get current user profile
        ''' </summary>
        Public Async Function GetUserProfileAsync() As Task(Of JObject)
            Return Await GetAsync("/auth/me")
        End Function

        ''' <summary>
        ''' Update user profile
        ''' </summary>
        Public Async Function UpdateUserProfileAsync(attributes As Object) As Task(Of JObject)
            Throw New LicenseChainException("updateUserProfile is not available in API v1")
        End Function

        ''' <summary>
        ''' Change user password
        ''' </summary>
        Public Async Function ChangePasswordAsync(currentPassword As String, newPassword As String) As Task(Of JObject)
            Dim payload = New With {
                .current_password = currentPassword,
                .new_password = newPassword
            }
            Return Await PatchAsync("/auth/password", payload)
        End Function

        ''' <summary>
        ''' Request password reset
        ''' </summary>
        Public Async Function RequestPasswordResetAsync(email As String) As Task(Of JObject)
            Dim payload = New With {
                .email = email
            }
            Return Await PostAsync("/auth/forgot-password", payload)
        End Function

        ''' <summary>
        ''' Reset password with token
        ''' </summary>
        Public Async Function ResetPasswordAsync(token As String, newPassword As String) As Task(Of JObject)
            Dim payload = New With {
                .token = token,
                .new_password = newPassword
            }
            Return Await PostAsync("/auth/reset-password", payload)
        End Function

        ' Application Management

        ''' <summary>
        ''' Create a new application
        ''' </summary>
        Public Async Function CreateApplicationAsync(name As String, Optional description As String = Nothing, Optional webhookUrl As String = Nothing, Optional allowedOrigins As String() = Nothing) As Task(Of JObject)
            Dim payload = New With {
                .name = name,
                .description = description,
                .webhook_url = webhookUrl,
                .allowed_origins = allowedOrigins
            }
            Return Await PostAsync("/apps", payload)
        End Function

        ''' <summary>
        ''' List applications with pagination
        ''' </summary>
        Public Async Function ListApplicationsAsync(Optional page As Integer = 1, Optional limit As Integer = 20) As Task(Of JObject)
            Dim queryParams = New Dictionary(Of String, String) From {
                {"page", page.ToString()},
                {"limit", limit.ToString()}
            }
            Return Await GetAsync("/apps", queryParams)
        End Function

        ''' <summary>
        ''' Get application details
        ''' </summary>
        Public Async Function GetApplicationAsync(appId As String) As Task(Of JObject)
            Return Await GetAsync($"/apps/{appId}")
        End Function

        ''' <summary>
        ''' Update application
        ''' </summary>
        Public Async Function UpdateApplicationAsync(appId As String, attributes As Object) As Task(Of JObject)
            Return Await PatchAsync($"/apps/{appId}", attributes)
        End Function

        ''' <summary>
        ''' Delete application
        ''' </summary>
        Public Async Function DeleteApplicationAsync(appId As String) As Task(Of JObject)
            Return Await DeleteAsync($"/apps/{appId}")
        End Function

        ''' <summary>
        ''' Regenerate API key for application
        ''' </summary>
        Public Async Function RegenerateApiKeyAsync(appId As String) As Task(Of JObject)
            Return Await PostAsync($"/apps/{appId}/regenerate-key", Nothing)
        End Function

        ' License Management

        ''' <summary>
        ''' Create a new license
        ''' </summary>
        Public Async Function CreateLicenseAsync(appId As String, userEmail As String, Optional userName As String = Nothing, Optional expiresAt As String = Nothing, Optional metadata As Object = Nothing) As Task(Of JObject)
            Dim payload = New With {
                .appId = appId,
                .issuedEmail = userEmail,
                .issuedTo = userName,
                .expiresAt = expiresAt,
                .metadata = metadata
            }
            Return Await PostAsync($"/apps/{appId}/licenses", payload)
        End Function

        ''' <summary>
        ''' List licenses with filters
        ''' </summary>
        Public Async Function ListLicensesAsync(Optional appId As String = Nothing, Optional page As Integer = 1, Optional limit As Integer = 20, Optional status As String = Nothing) As Task(Of JObject)
            Dim queryParams As New Dictionary(Of String, String) From {
                {"page", page.ToString()},
                {"limit", limit.ToString()}
            }
            If Not String.IsNullOrEmpty(appId) Then queryParams.Add("app_id", appId)
            If Not String.IsNullOrEmpty(status) Then queryParams.Add("status", status)
            Return Await GetAsync("/licenses", queryParams)
        End Function

        ''' <summary>
        ''' Get license details
        ''' </summary>
        Public Async Function GetLicenseAsync(licenseId As String) As Task(Of JObject)
            Return Await GetAsync($"/licenses/{licenseId}")
        End Function

        ''' <summary>
        ''' Update license
        ''' </summary>
        Public Async Function UpdateLicenseAsync(licenseId As String, attributes As Object) As Task(Of JObject)
            Return Await PatchAsync($"/licenses/{licenseId}", attributes)
        End Function

        ''' <summary>
        ''' Delete license
        ''' </summary>
        Public Async Function DeleteLicenseAsync(licenseId As String) As Task(Of JObject)
            Return Await DeleteAsync($"/licenses/{licenseId}")
        End Function

        ''' <summary>
        ''' Validate a license key. Optional hwuid for ecosystem HMAC/HWUID contract.
        ''' </summary>
        Public Async Function ValidateLicenseAsync(licenseKey As String, Optional appId As String = Nothing, Optional hwuid As String = Nothing) As Task(Of JObject)
            Dim resolvedHwuid = If(String.IsNullOrWhiteSpace(hwuid), GenerateDefaultHwuid(), hwuid.Trim())
            Dim payload = New With {
                .key = licenseKey,
                .app_id = If(String.IsNullOrWhiteSpace(appId), Nothing, appId),
                .hwuid = resolvedHwuid
            }
            Return Await PostAsync("/licenses/verify", payload)
        End Function

        ''' <summary>
        ''' Full POST /licenses/verify JSON (valid, optional license_token, license_jwks_uri, etc.).
        ''' </summary>
        Public Async Function VerifyLicenseWithDetailsAsync(licenseKey As String, Optional appId As String = Nothing, Optional hwuid As String = Nothing) As Task(Of JObject)
            Return Await ValidateLicenseAsync(licenseKey, appId, hwuid)
        End Function

        ''' <summary>
        ''' Revoke a license
        ''' </summary>
        Public Async Function RevokeLicenseAsync(licenseId As String, Optional reason As String = Nothing) As Task(Of JObject)
            Dim payload = New With {
                .reason = reason
            }
            Return Await PatchAsync($"/licenses/{licenseId}/revoke", payload)
        End Function

        ''' <summary>
        ''' Activate a license
        ''' </summary>
        Public Async Function ActivateLicenseAsync(licenseId As String) As Task(Of JObject)
            Return Await PatchAsync($"/licenses/{licenseId}/activate", Nothing)
        End Function

        ''' <summary>
        ''' Extend license expiration
        ''' </summary>
        Public Async Function ExtendLicenseAsync(licenseId As String, newExpiresAt As String) As Task(Of JObject)
            Dim payload = New With {
                .expiresAt = newExpiresAt
            }
            Return Await PatchAsync($"/licenses/{licenseId}/extend", payload)
        End Function

        ' Webhook Management

        ''' <summary>
        ''' Create a webhook
        ''' </summary>
        Public Async Function CreateWebhookAsync(appId As String, url As String, events As String(), Optional secret As String = Nothing) As Task(Of JObject)
            Dim payload = New With {
                .app_id = appId,
                .url = url,
                .events = events,
                .secret = secret
            }
            Return Await PostAsync("/webhooks", payload)
        End Function

        ''' <summary>
        ''' List webhooks
        ''' </summary>
        Public Async Function ListWebhooksAsync(Optional appId As String = Nothing) As Task(Of JObject)
            Dim queryParams As New Dictionary(Of String, String)
            If Not String.IsNullOrEmpty(appId) Then queryParams.Add("app_id", appId)
            Return Await GetAsync("/webhooks", queryParams)
        End Function

        ''' <summary>
        ''' Get webhook details
        ''' </summary>
        Public Async Function GetWebhookAsync(webhookId As String) As Task(Of JObject)
            Return Await GetAsync($"/webhooks/{webhookId}")
        End Function

        ''' <summary>
        ''' Update webhook
        ''' </summary>
        Public Async Function UpdateWebhookAsync(webhookId As String, attributes As Object) As Task(Of JObject)
            Return Await PatchAsync($"/webhooks/{webhookId}", attributes)
        End Function

        ''' <summary>
        ''' Delete webhook
        ''' </summary>
        Public Async Function DeleteWebhookAsync(webhookId As String) As Task(Of JObject)
            Return Await DeleteAsync($"/webhooks/{webhookId}")
        End Function

        ''' <summary>
        ''' Test webhook
        ''' </summary>
        Public Async Function TestWebhookAsync(webhookId As String) As Task(Of JObject)
            Return Await PostAsync($"/webhooks/{webhookId}/test", Nothing)
        End Function

        ' Analytics

        ''' <summary>
        ''' Get analytics data
        ''' </summary>
        Public Async Function GetAnalyticsAsync(Optional appId As String = Nothing, Optional startDate As String = Nothing, Optional endDate As String = Nothing, Optional metric As String = Nothing) As Task(Of JObject)
            Dim queryParams As New Dictionary(Of String, String)
            If Not String.IsNullOrEmpty(appId) Then queryParams.Add("app_id", appId)
            If Not String.IsNullOrEmpty(startDate) Then queryParams.Add("start_date", startDate)
            If Not String.IsNullOrEmpty(endDate) Then queryParams.Add("end_date", endDate)
            If Not String.IsNullOrEmpty(metric) Then queryParams.Add("metric", metric)
            Return Await GetAsync("/analytics/stats", queryParams)
        End Function

        ''' <summary>
        ''' Get license analytics
        ''' </summary>
        Public Async Function GetLicenseAnalyticsAsync(licenseId As String) As Task(Of JObject)
            Return Await GetAsync($"/licenses/{licenseId}/analytics")
        End Function

        ''' <summary>
        ''' Get usage statistics
        ''' </summary>
        Public Async Function GetUsageStatsAsync(Optional appId As String = Nothing, Optional period As String = "30d") As Task(Of JObject)
            Dim queryParams As New Dictionary(Of String, String) From {
                {"period", period}
            }
            If Not String.IsNullOrEmpty(appId) Then queryParams.Add("app_id", appId)
            Return Await GetAsync("/analytics/usage", queryParams)
        End Function

        ' System Status

        ''' <summary>
        ''' Get system status
        ''' </summary>
        Public Async Function GetSystemStatusAsync() As Task(Of JObject)
            Return Await GetAsync("/health")
        End Function

        ''' <summary>
        ''' Get health check
        ''' </summary>
        Public Async Function GetHealthCheckAsync() As Task(Of JObject)
            Return Await GetAsync("/health")
        End Function

        ' HTTP Methods

        Private Async Function GetAsync(path As String, Optional queryParams As Dictionary(Of String, String) = Nothing) As Task(Of JObject)
            Dim url = BuildUrl(NormalizePath(path), queryParams)
            Dim response = Await _httpClient.GetAsync(url)
            Return Await ProcessResponseAsync(response)
        End Function

        Private Async Function PostAsync(path As String, payload As Object) As Task(Of JObject)
            Dim url = _baseUrl + NormalizePath(path)
            Dim json = If(payload IsNot Nothing, JsonConvert.SerializeObject(payload), "{}")
            Dim content = New StringContent(json, Encoding.UTF8, "application/json")
            Dim response = Await _httpClient.PostAsync(url, content)
            Return Await ProcessResponseAsync(response)
        End Function

        Private Async Function PatchAsync(path As String, payload As Object) As Task(Of JObject)
            Dim url = _baseUrl + NormalizePath(path)
            Dim json = If(payload IsNot Nothing, JsonConvert.SerializeObject(payload), "{}")
            Dim content = New StringContent(json, Encoding.UTF8, "application/json")
            Dim request = New HttpRequestMessage(HttpMethod.Patch, url) With {.Content = content}
            Dim response = Await _httpClient.SendAsync(request)
            Return Await ProcessResponseAsync(response)
        End Function

        Private Async Function DeleteAsync(path As String) As Task(Of JObject)
            Dim url = _baseUrl + NormalizePath(path)
            Dim response = Await _httpClient.DeleteAsync(url)
            Return Await ProcessResponseAsync(response)
        End Function

        Private Function BuildUrl(path As String, queryParams As Dictionary(Of String, String)) As String
            Dim url = _baseUrl + path
            If queryParams IsNot Nothing AndAlso queryParams.Count > 0 Then
                Dim queryString = String.Join("&", queryParams.Select(Function(kvp) $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"))
                url += $"?{queryString}"
            End If
            Return url
        End Function

        Private Function NormalizePath(path As String) As String
            If path.StartsWith("/v1/", StringComparison.OrdinalIgnoreCase) Then
                Return path
            End If
            If path.StartsWith("/", StringComparison.Ordinal) Then
                Return "/v1" + path
            End If
            Return "/v1/" + path
        End Function

        Private Async Function ProcessResponseAsync(response As HttpResponseMessage) As Task(Of JObject)
            Dim content = Await response.Content.ReadAsStringAsync()

            If Not response.IsSuccessStatusCode Then
                Throw New LicenseChainException($"HTTP {CInt(response.StatusCode)}: {content}")
            End If

            Try
                Return JObject.Parse(content)
            Catch ex As JsonException
                Throw New LicenseChainException($"Invalid JSON response: {ex.Message}")
            End Try
        End Function

        Private Function GenerateDefaultHwuid() As String
            Dim raw = $"licensechain|vb|{Environment.MachineName}|{Environment.OSVersion}|{Environment.ProcessorCount}"
            Using sha = SHA256.Create()
                Dim bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw))
                Return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant()
            End Using
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not _disposed Then
                If disposing Then
                    _httpClient?.Dispose()
                End If
                _disposed = True
            End If
        End Sub

    End Class

End Namespace
