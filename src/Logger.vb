Imports System
Imports System.IO
Imports System.Threading.Tasks

Namespace LicenseChain.VB
    ''' <summary>
    ''' Simple logging utility for the LicenseChain SDK
    ''' </summary>
    Public Class Logger
        Private Shared _logLevel As LogLevel = LogLevel.Info
        Private Shared _logToFile As Boolean = False
        Private Shared _logFilePath As String = "licensechain.log"

        Public Enum LogLevel
            Debug = 0
            Info = 1
            Warning = 2
            [Error] = 3
            Fatal = 4
        End Enum

        ''' <summary>
        ''' Sets the minimum log level
        ''' </summary>
        ''' <param name="level">Minimum log level</param>
        Public Shared Sub SetLogLevel(level As LogLevel)
            _logLevel = level
        End Sub

        ''' <summary>
        ''' Enables or disables file logging
        ''' </summary>
        ''' <param name="enabled">True to enable file logging</param>
        ''' <param name="filePath">Optional custom file path</param>
        Public Shared Sub SetFileLogging(enabled As Boolean, Optional filePath As String = Nothing)
            _logToFile = enabled
            If Not String.IsNullOrEmpty(filePath) Then
                _logFilePath = filePath
            End If
        End Sub

        ''' <summary>
        ''' Logs a debug message
        ''' </summary>
        ''' <param name="message">Message to log</param>
        ''' <param name="ex">Optional exception</param>
        Public Shared Sub Debug(message As String, Optional ex As Exception = Nothing)
            Log(LogLevel.Debug, message, ex)
        End Sub

        ''' <summary>
        ''' Logs an info message
        ''' </summary>
        ''' <param name="message">Message to log</param>
        ''' <param name="ex">Optional exception</param>
        Public Shared Sub Info(message As String, Optional ex As Exception = Nothing)
            Log(LogLevel.Info, message, ex)
        End Sub

        ''' <summary>
        ''' Logs a warning message
        ''' </summary>
        ''' <param name="message">Message to log</param>
        ''' <param name="ex">Optional exception</param>
        Public Shared Sub Warning(message As String, Optional ex As Exception = Nothing)
            Log(LogLevel.Warning, message, ex)
        End Sub

        ''' <summary>
        ''' Logs an error message
        ''' </summary>
        ''' <param name="message">Message to log</param>
        ''' <param name="ex">Optional exception</param>
        Public Shared Sub [Error](message As String, Optional ex As Exception = Nothing)
            Log(LogLevel.[Error], message, ex)
        End Sub

        ''' <summary>
        ''' Logs a fatal message
        ''' </summary>
        ''' <param name="message">Message to log</param>
        ''' <param name="ex">Optional exception</param>
        Public Shared Sub Fatal(message As String, Optional ex As Exception = Nothing)
            Log(LogLevel.Fatal, message, ex)
        End Sub

        ''' <summary>
        ''' Internal logging method
        ''' </summary>
        ''' <param name="level">Log level</param>
        ''' <param name="message">Message to log</param>
        ''' <param name="ex">Optional exception</param>
        Private Shared Sub Log(level As LogLevel, message As String, Optional ex As Exception = Nothing)
            If level < _logLevel Then
                Return
            End If

            Dim timestamp As String = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
            Dim levelName As String = level.ToString().ToUpper()
            Dim logMessage As String = $"[{timestamp}] [{levelName}] {message}"

            If ex IsNot Nothing Then
                logMessage += $"{vbCrLf}Exception: {ex.Message}{vbCrLf}Stack Trace: {ex.StackTrace}"
            End If

            ' Console output
            Select Case level
                Case LogLevel.Debug
                    Console.ForegroundColor = ConsoleColor.Gray
                Case LogLevel.Info
                    Console.ForegroundColor = ConsoleColor.White
                Case LogLevel.Warning
                    Console.ForegroundColor = ConsoleColor.Yellow
                Case LogLevel.[Error]
                    Console.ForegroundColor = ConsoleColor.Red
                Case LogLevel.Fatal
                    Console.ForegroundColor = ConsoleColor.Magenta
            End Select

            Console.WriteLine(logMessage)
            Console.ResetColor()

            ' File output
            If _logToFile Then
                Try
                    File.AppendAllText(_logFilePath, logMessage + vbCrLf)
                Catch
                    ' Ignore file logging errors
                End Try
            End If
        End Sub

        ''' <summary>
        ''' Clears the log file
        ''' </summary>
        Public Shared Sub ClearLogFile()
            If File.Exists(_logFilePath) Then
                Try
                    File.Delete(_logFilePath)
                Catch
                    ' Ignore file deletion errors
                End Try
            End If
        End Sub

        ''' <summary>
        ''' Gets the current log file path
        ''' </summary>
        ''' <returns>Current log file path</returns>
        Public Shared Function GetLogFilePath() As String
            Return _logFilePath
        End Function
    End Class
End Namespace
