' LicenseChain VB.NET SDK - Basic Usage Example
Imports LicenseChain.SDK

Module BasicUsageExample
    Sub Main()
        Console.WriteLine(" LicenseChain VB.NET SDK - Basic Usage Example")
        Console.WriteLine(New String("="c, 50))
        
        ' Initialize the client
        Dim config As New LicenseChainConfig With {
            .ApiKey = "your-api-key-here",
            .AppName = "MyVBApp",
            .Version = "1.0.0",
            .Debug = True
        }
        
        Dim client As New LicenseChainClient(config)
        
        ' Connect to LicenseChain
        Console.WriteLine(vbNewLine & " Connecting to LicenseChain...")
        Try
            client.Connect()
            Console.WriteLine(" Connected to LicenseChain successfully!")
        Catch ex As LicenseChainException
            Console.WriteLine(" Failed to connect: " & ex.Message)
            Return
        End Try
        
        ' Example 1: User Registration
        Console.WriteLine(vbNewLine & " Registering new user...")
        Try
            Dim user As User = client.Register("testuser", "password123", "test@example.com")
            Console.WriteLine(" User registered successfully!")
            Console.WriteLine("Session ID: " & user.SessionId)
        Catch ex As LicenseChainException
            Console.WriteLine(" Registration failed: " & ex.Message)
        End Try
        
        ' Example 2: License Validation
        Console.WriteLine(vbNewLine & " Validating license...")
        Try
            Dim license As License = client.ValidateLicense("LICENSE-KEY-HERE")
            Console.WriteLine(" License is valid!")
            Console.WriteLine("License Key: " & license.Key)
            Console.WriteLine("Status: " & license.Status)
        Catch ex As LicenseChainException
            Console.WriteLine(" License validation failed: " & ex.Message)
        End Try
        
        ' Cleanup
        Console.WriteLine(vbNewLine & " Cleaning up...")
        client.Logout()
        client.Disconnect()
        Console.WriteLine(" Cleanup completed!")
    End Sub
End Module
