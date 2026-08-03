Imports System
Imports System.Threading.Tasks
Imports Newtonsoft.Json.Linq

Namespace LicenseChain.VB

    ''' <summary>
    ''' License validator for easy license validation
    ''' </summary>
    Public Class LicenseValidator

        Private ReadOnly _client As LicenseChainClient

        ''' <summary>
        ''' Initializes a new instance of the LicenseValidator
        ''' </summary>
        ''' <param name="apiKey">Your LicenseChain API key</param>
        ''' <param name="baseUrl">Base URL for the API (optional)</param>
        Public Sub New(apiKey As String, Optional baseUrl As String = "https://api.licensechain.app")
            _client = New LicenseChainClient(apiKey, baseUrl)
        End Sub

        ''' <summary>
        ''' Validate a license key
        ''' </summary>
        ''' <param name="licenseKey">The license key to validate</param>
        ''' <param name="appId">Optional application ID for validation</param>
        ''' <returns>Validation result</returns>
        Public Async Function ValidateLicenseAsync(licenseKey As String, Optional appId As String = Nothing) As Task(Of ValidationResult)
            Try
                Dim response = Await _client.ValidateLicenseAsync(licenseKey, appId)
                Return New ValidationResult(response)
            Catch ex As Exception
                Return New ValidationResult(False, error:=ex.Message)
            End Try
        End Function

        ''' <summary>
        ''' Check if license is valid (quick check)
        ''' </summary>
        ''' <param name="licenseKey">The license key to check</param>
        ''' <param name="appId">Optional application ID for validation</param>
        ''' <returns>True if valid, false otherwise</returns>
        Public Async Function IsValidAsync(licenseKey As String, Optional appId As String = Nothing) As Task(Of Boolean)
            Try
                Dim result = Await ValidateLicenseAsync(licenseKey, appId)
                Return result.IsValid
            Catch
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Get license information
        ''' </summary>
        ''' <param name="licenseKey">The license key</param>
        ''' <param name="appId">Optional application ID</param>
        ''' <returns>License information or null if invalid</returns>
        Public Async Function GetLicenseInfoAsync(licenseKey As String, Optional appId As String = Nothing) As Task(Of License)
            Try
                Dim result = Await ValidateLicenseAsync(licenseKey, appId)
                If result.IsValid AndAlso result.LicenseData IsNot Nothing Then
                    Return New License(result.LicenseData)
                End If
                Return Nothing
            Catch
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Check if license is expired
        ''' </summary>
        ''' <param name="licenseKey">The license key</param>
        ''' <param name="appId">Optional application ID</param>
        ''' <returns>True if expired, false otherwise</returns>
        Public Async Function IsExpiredAsync(licenseKey As String, Optional appId As String = Nothing) As Task(Of Boolean)
            Try
                Dim result = Await ValidateLicenseAsync(licenseKey, appId)
                If result.IsValid AndAlso result.LicenseData IsNot Nothing Then
                    Dim license = New License(result.LicenseData)
                    Return license.IsExpired
                End If
                Return True
            Catch
                Return True
            End Try
        End Function

        ''' <summary>
        ''' Get days until expiration
        ''' </summary>
        ''' <param name="licenseKey">The license key</param>
        ''' <param name="appId">Optional application ID</param>
        ''' <returns>Days until expiration or null if no expiration date</returns>
        Public Async Function GetDaysUntilExpirationAsync(licenseKey As String, Optional appId As String = Nothing) As Task(Of Integer?)
            Try
                Dim result = Await ValidateLicenseAsync(licenseKey, appId)
                If result.IsValid AndAlso result.LicenseData IsNot Nothing Then
                    Dim license = New License(result.LicenseData)
                    Return license.DaysUntilExpiration
                End If
                Return Nothing
            Catch
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Dispose the validator
        ''' </summary>
        Public Sub Dispose()
            _client?.Dispose()
        End Sub

    End Class

    ''' <summary>
    ''' Result of license validation
    ''' </summary>
    Public Class ValidationResult

        Public ReadOnly Property IsValid As Boolean
        Public ReadOnly Property LicenseData As Dictionary(Of String, Object)
        Public ReadOnly Property UserData As Dictionary(Of String, Object)
        Public ReadOnly Property AppData As Dictionary(Of String, Object)
        Public ReadOnly Property ExpiresAt As String
        Public ReadOnly Property Metadata As Dictionary(Of String, Object)
        Public ReadOnly Property [Error] As String

        Public Sub New(isValid As Boolean, Optional licenseData As Dictionary(Of String, Object) = Nothing, Optional userData As Dictionary(Of String, Object) = Nothing, Optional appData As Dictionary(Of String, Object) = Nothing, Optional expiresAt As String = Nothing, Optional metadata As Dictionary(Of String, Object) = Nothing, Optional [error] As String = Nothing)
            Me.IsValid = isValid
            Me.LicenseData = licenseData
            Me.UserData = userData
            Me.AppData = appData
            Me.ExpiresAt = expiresAt
            Me.Metadata = metadata
            Me.[Error] = [error]
        End Sub

        Public Sub New(response As JObject)
            IsValid = If(response("valid"), False)
            LicenseData = If(response("license") IsNot Nothing, response("license").ToObject(Of Dictionary(Of String, Object))(), New Dictionary(Of String, Object)())
            UserData = If(response("user") IsNot Nothing, response("user").ToObject(Of Dictionary(Of String, Object))(), New Dictionary(Of String, Object)())
            AppData = If(response("app") IsNot Nothing, response("app").ToObject(Of Dictionary(Of String, Object))(), New Dictionary(Of String, Object)())
            ExpiresAt = If(response("expires_at"), String.Empty)
            Metadata = If(response("metadata") IsNot Nothing, response("metadata").ToObject(Of Dictionary(Of String, Object))(), New Dictionary(Of String, Object)())
            Me.[Error] = If(response("error"), String.Empty)
        End Sub

        Public ReadOnly Property UserEmail As String
            Get
                If UserData IsNot Nothing AndAlso UserData.ContainsKey("email") Then
                    Return UserData("email").ToString()
                End If
                Return String.Empty
            End Get
        End Property

        Public ReadOnly Property UserName As String
            Get
                If UserData IsNot Nothing AndAlso UserData.ContainsKey("name") Then
                    Return UserData("name").ToString()
                End If
                Return String.Empty
            End Get
        End Property

        Public ReadOnly Property AppName As String
            Get
                If AppData IsNot Nothing AndAlso AppData.ContainsKey("name") Then
                    Return AppData("name").ToString()
                End If
                Return String.Empty
            End Get
        End Property

        Public ReadOnly Property LicenseKey As String
            Get
                If LicenseData IsNot Nothing AndAlso LicenseData.ContainsKey("key") Then
                    Return LicenseData("key").ToString()
                End If
                Return String.Empty
            End Get
        End Property

        Public ReadOnly Property LicenseId As String
            Get
                If LicenseData IsNot Nothing AndAlso LicenseData.ContainsKey("id") Then
                    Return LicenseData("id").ToString()
                End If
                Return String.Empty
            End Get
        End Property

        Public ReadOnly Property Features As String()
            Get
                If LicenseData IsNot Nothing AndAlso LicenseData.ContainsKey("features") AndAlso TypeOf LicenseData("features") Is String() Then
                    Return DirectCast(LicenseData("features"), String())
                End If
                Return New String() {}
            End Get
        End Property

        Public ReadOnly Property UsageCount As Integer
            Get
                If LicenseData IsNot Nothing AndAlso LicenseData.ContainsKey("usage_count") Then
                    Return If(LicenseData("usage_count"), 0)
                End If
                Return 0
            End Get
        End Property

    End Class

End Namespace
