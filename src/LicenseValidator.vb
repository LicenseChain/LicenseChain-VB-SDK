Imports System.Net.Http
Imports Newtonsoft.Json

Public Class LicenseValidator
    Private ReadOnly apiKey As String
    Private ReadOnly apiUrl As String = "https://licensechain.app/api/license/validate"

    Public Sub New(apiKey As String)
        Me.apiKey = apiKey
    End Sub

    Public Async Function ValidateLicense(licenseKey As String) As Task(Of Boolean)
        Dim client As New HttpClient()
        Dim requestBody As New Dictionary(Of String, String) From {
            {"api_key", apiKey},
            {"license_key", licenseKey}
        }

        Dim content = New FormUrlEncodedContent(requestBody)
        Dim response = Await client.PostAsync(apiUrl, content)

        If response.IsSuccessStatusCode Then
            Dim jsonResponse = Await response.Content.ReadAsStringAsync()
            Dim apiResponse As ApiResponse = JsonConvert.DeserializeObject(Of ApiResponse)(jsonResponse)

            Return apiResponse.isValid
        Else
            Throw New Exception("Failed to validate license")
        End If
    End Function
End Class
