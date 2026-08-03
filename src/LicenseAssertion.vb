Imports System.IdentityModel.Tokens.Jwt
Imports System.Linq
Imports System.Net.Http
Imports System.Threading.Tasks
Imports Microsoft.IdentityModel.Tokens

Namespace LicenseChain.VB
    ''' <summary>
    ''' RS256 license_token verification via JWKS (parity with Node verifyLicenseAssertionJwt).
    ''' </summary>
    Public NotInheritable Class LicenseAssertion
        Public Const LICENSE_TOKEN_USE_CLAIM As String = "licensechain_license_v1"

        Public Class VerifyLicenseAssertionOptions
            Public Property ExpectedAppId As String
            Public Property Issuer As String
        End Class

        Public Shared Async Function VerifyLicenseAssertionJwtAsync(
            httpClient As HttpClient,
            token As String,
            jwksUrl As String,
            Optional options As VerifyLicenseAssertionOptions = Nothing
        ) As Task(Of JwtSecurityToken)
            If options Is Nothing Then options = New VerifyLicenseAssertionOptions()
            Dim t = If(token, String.Empty).Trim()
            Dim j = If(jwksUrl, String.Empty).Trim()
            If t.Length = 0 Then Throw New ArgumentException("empty token", NameOf(token))
            If j.Length = 0 Then Throw New ArgumentException("empty jwksUrl", NameOf(jwksUrl))

            Dim jwksJson = Await httpClient.GetStringAsync(j).ConfigureAwait(False)
            Dim jwks = New JsonWebKeySet(jwksJson)
            Dim keys = jwks.GetSigningKeys()

            Dim handler As New JwtSecurityTokenHandler()
            Dim validationParameters As New TokenValidationParameters With {
                .ValidateIssuerSigningKey = True,
                .IssuerSigningKeys = keys,
                .ValidateIssuer = Not String.IsNullOrWhiteSpace(options.Issuer),
                .ValidIssuer = options.Issuer,
                .ValidateAudience = False,
                .ValidateLifetime = True,
                .ClockSkew = TimeSpan.FromMinutes(2),
                .ValidAlgorithms = {SecurityAlgorithms.RsaSha256}
            }

            Dim validatedToken As SecurityToken = Nothing
            handler.ValidateToken(t, validationParameters, validatedToken)
            Dim jwt = CType(validatedToken, JwtSecurityToken)

            Dim tuObj As Object = Nothing
            Dim tu As String = Nothing
            If jwt.Payload.TryGetValue("token_use", tuObj) AndAlso tuObj IsNot Nothing Then
                tu = tuObj.ToString()
            End If
            If tu <> LICENSE_TOKEN_USE_CLAIM Then
                Throw New SecurityTokenException($"Invalid license token: expected token_use ""{LICENSE_TOKEN_USE_CLAIM}""")
            End If

            If Not String.IsNullOrWhiteSpace(options.ExpectedAppId) Then
                Dim want = options.ExpectedAppId.Trim()
                If Not jwt.Audiences.Contains(want) Then
                    Throw New SecurityTokenException("Invalid license token: aud does not match expected app id")
                End If
            End If

            Return jwt
        End Function
    End Class
End Namespace
