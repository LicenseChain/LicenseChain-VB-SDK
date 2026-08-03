' JWKS-only: verify license_token with token + JWKS URI (parity with C# examples/jwks_only).
' Env: LICENSECHAIN_LICENSE_TOKEN, LICENSECHAIN_LICENSE_JWKS_URI
' Optional: LICENSECHAIN_EXPECTED_APP_ID
'
' Run: dotnet run --project examples/jwks_only/jwks_only.vbproj (from repo root)

Imports System.Net.Http
Imports Newtonsoft.Json.Linq

Module Program
    Public Sub Main(args As String())
        MainAsync().GetAwaiter().GetResult()
    End Sub

    Private Async Function MainAsync() As Task
        Dim token = Environment.GetEnvironmentVariable("LICENSECHAIN_LICENSE_TOKEN")?.Trim()
        Dim jwks = Environment.GetEnvironmentVariable("LICENSECHAIN_LICENSE_JWKS_URI")?.Trim()
        If String.IsNullOrEmpty(token) OrElse String.IsNullOrEmpty(jwks) Then
            Console.Error.WriteLine("Set LICENSECHAIN_LICENSE_TOKEN and LICENSECHAIN_LICENSE_JWKS_URI")
            Environment.Exit(1)
        End If

        Dim opts As New LicenseChain.VB.LicenseAssertion.VerifyLicenseAssertionOptions()
        Dim appId = Environment.GetEnvironmentVariable("LICENSECHAIN_EXPECTED_APP_ID")
        If Not String.IsNullOrWhiteSpace(appId) Then
            opts.ExpectedAppId = appId.Trim()
        End If

        Using http As New HttpClient()
            Dim jwt = Await LicenseChain.VB.LicenseAssertion.VerifyLicenseAssertionJwtAsync(http, token, jwks, opts).ConfigureAwait(False)
            Console.WriteLine(JObject.Parse(jwt.Payload.SerializeToJson()).ToString())
        End Using
    End Function
End Module
