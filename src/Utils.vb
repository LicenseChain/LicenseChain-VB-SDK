Imports System
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions

Namespace LicenseChain.VB
    Public Module Utils
        ''' <summary>
        ''' Validates if a string is a valid email address
        ''' </summary>
        ''' <param name="email">Email address to validate</param>
        ''' <returns>True if valid email, False otherwise</returns>
        Public Function IsValidEmail(email As String) As Boolean
            If String.IsNullOrEmpty(email) Then
                Return False
            End If

            Try
                Dim emailRegex As New Regex("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
                Return emailRegex.IsMatch(email)
            Catch
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Validates if a string is a valid license key format
        ''' </summary>
        ''' <param name="licenseKey">License key to validate</param>
        ''' <returns>True if valid format, False otherwise</returns>
        Public Function IsValidLicenseKey(licenseKey As String) As Boolean
            If String.IsNullOrEmpty(licenseKey) Then
                Return False
            End If

            ' License key should be 32 characters long and contain only alphanumeric characters
            If licenseKey.Length <> 32 Then
                Return False
            End If

            Dim keyRegex As New Regex("^[a-zA-Z0-9]+$")
            Return keyRegex.IsMatch(licenseKey)
        End Function

        ''' <summary>
        ''' Generates a random license key
        ''' </summary>
        ''' <returns>Random 32-character license key</returns>
        Public Function GenerateLicenseKey() As String
            Dim chars As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
            Dim random As New Random()
            Dim result As New StringBuilder(32)

            For i As Integer = 0 To 31
                result.Append(chars(random.Next(chars.Length)))
            Next

            Return result.ToString()
        End Function

        ''' <summary>
        ''' Generates MD5 hash of a string
        ''' </summary>
        ''' <param name="input">String to hash</param>
        ''' <returns>MD5 hash as hexadecimal string</returns>
        Public Function GenerateMD5Hash(input As String) As String
            If String.IsNullOrEmpty(input) Then
                Return String.Empty
            End If

            Using md5 As MD5 = MD5.Create()
                Dim inputBytes As Byte() = Encoding.UTF8.GetBytes(input)
                Dim hashBytes As Byte() = md5.ComputeHash(inputBytes)
                Return BitConverter.ToString(hashBytes).Replace("-", "").ToLower()
            End Using
        End Function

        ''' <summary>
        ''' Generates SHA256 hash of a string
        ''' </summary>
        ''' <param name="input">String to hash</param>
        ''' <returns>SHA256 hash as hexadecimal string</returns>
        Public Function GenerateSHA256Hash(input As String) As String
            If String.IsNullOrEmpty(input) Then
                Return String.Empty
            End If

            Using sha256 As SHA256 = SHA256.Create()
                Dim inputBytes As Byte() = Encoding.UTF8.GetBytes(input)
                Dim hashBytes As Byte() = sha256.ComputeHash(inputBytes)
                Return BitConverter.ToString(hashBytes).Replace("-", "").ToLower()
            End Using
        End Function

        ''' <summary>
        ''' Sanitizes input string to prevent injection attacks
        ''' </summary>
        ''' <param name="input">Input string to sanitize</param>
        ''' <returns>Sanitized string</returns>
        Public Function SanitizeInput(input As String) As String
            If String.IsNullOrEmpty(input) Then
                Return String.Empty
            End If

            Return input.Replace("'", "''").Replace("""", """""").Replace("<", "&lt;").Replace(">", "&gt;")
        End Function

        ''' <summary>
        ''' Formats a date to ISO 8601 format
        ''' </summary>
        ''' <param name="date">Date to format</param>
        ''' <returns>ISO 8601 formatted date string</returns>
        Public Function FormatISODate([date] As DateTime) As String
            Return [date].ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
        End Function

        ''' <summary>
        ''' Parses ISO 8601 date string to DateTime
        ''' </summary>
        ''' <param name="isoDate">ISO 8601 date string</param>
        ''' <returns>Parsed DateTime or Nothing if invalid</returns>
        Public Function ParseISODate(isoDate As String) As DateTime?
            If String.IsNullOrEmpty(isoDate) Then
                Return Nothing
            End If

            Try
                Return DateTime.ParseExact(isoDate, "yyyy-MM-ddTHH:mm:ss.fffZ", Nothing)
            Catch
                Try
                    Return DateTime.Parse(isoDate)
                Catch
                    Return Nothing
                End Try
            End Try
        End Function

        ''' <summary>
        ''' Converts object to JSON string
        ''' </summary>
        ''' <param name="obj">Object to convert</param>
        ''' <returns>JSON string representation</returns>
        Public Function ToJson(obj As Object) As String
            If obj Is Nothing Then
                Return "null"
            End If

            Try
                Return Newtonsoft.Json.JsonConvert.SerializeObject(obj)
            Catch ex As Exception
                Return "{}"
            End Try
        End Function

        ''' <summary>
        ''' Converts JSON string to object
        ''' </summary>
        ''' <typeparam name="T">Type to deserialize to</typeparam>
        ''' <param name="json">JSON string</param>
        ''' <returns>Deserialized object or Nothing if invalid</returns>
        Public Function FromJson(Of T)(json As String) As T
            If String.IsNullOrEmpty(json) Then
                Return Nothing
            End If

            Try
                Return Newtonsoft.Json.JsonConvert.DeserializeObject(Of T)(json)
            Catch
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Sleeps for specified milliseconds
        ''' </summary>
        ''' <param name="milliseconds">Milliseconds to sleep</param>
        Public Sub Sleep(milliseconds As Integer)
            System.Threading.Thread.Sleep(milliseconds)
        End Sub

        ''' <summary>
        ''' Sleeps asynchronously for specified milliseconds
        ''' </summary>
        ''' <param name="milliseconds">Milliseconds to sleep</param>
        ''' <returns>Task</returns>
        Public Async Function SleepAsync(milliseconds As Integer) As Task
            Await Task.Delay(milliseconds)
        End Function

        ''' <summary>
        ''' Retries an operation with exponential backoff
        ''' </summary>
        ''' <param name="operation">Operation to retry</param>
        ''' <param name="maxRetries">Maximum number of retries</param>
        ''' <param name="baseDelay">Base delay in milliseconds</param>
        ''' <returns>Task</returns>
        Public Async Function RetryAsync(operation As Func(Of Task), maxRetries As Integer, baseDelay As Integer) As Task
            Dim retryCount As Integer = 0
            Dim delay As Integer = baseDelay
            Dim shouldRetry As Boolean

            While retryCount < maxRetries
                shouldRetry = False
                Try
                    Await operation()
                    Return
                Catch ex As Exception
                    retryCount += 1
                    If retryCount >= maxRetries Then
                        Throw
                    End If
                    shouldRetry = True
                End Try
                If shouldRetry Then
                    Await SleepAsync(delay)
                    delay *= 2 ' Exponential backoff
                End If
            End While
        End Function
    End Module
End Namespace
