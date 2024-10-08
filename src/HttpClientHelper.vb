Imports System.Net.Http

Public Class HttpClientHelper
    Public Shared Async Function PostAsync(url As String, data As Dictionary(Of String, String)) As Task(Of String)
        Using client As New HttpClient()
            Dim content = New FormUrlEncodedContent(data)
            Dim response = Await client.PostAsync(url, content)
            Return Await response.Content.ReadAsStringAsync()
        End Using
    End Function
End Class
