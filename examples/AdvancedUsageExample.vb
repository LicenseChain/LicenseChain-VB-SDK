' LicenseChain VB.NET SDK - Advanced Usage Example
Imports LicenseChain.SDK
Imports System
Imports System.Threading.Tasks

Module AdvancedUsageExample
    Async Function Main() As Task
        Console.WriteLine("🚀 LicenseChain VB.NET SDK - Advanced Usage Example")
        Console.WriteLine(New String("="c, 60))

        ' Initialize the client with advanced configuration
        Dim config As New LicenseChainConfig With {
            .ApiKey = "your-api-key-here",
            .BaseUrl = "https://api.licensechain.app",
            .Timeout = 30000,
            .Retries = 3,
            .EnableLogging = True
        }

        Dim client As New LicenseChainClient(config)

        Try
            ' 1. Advanced License Management
            Console.WriteLine(vbCrLf & "1. Advanced License Management")
            Console.WriteLine(New String("-"c, 30))

            ' Create multiple licenses
            Dim licenses As New List(Of License)
            For i As Integer = 1 To 3
                Dim license As License = Await client.CreateLicenseAsync(
                    $"user{i}",
                    $"product{i}",
                    New Dictionary(Of String, Object) From {
                        {"features", New String() {"premium", "api_access", "support"}},
                        {"maxUsers", 50},
                        {"expirationDays", 365}
                    }
                )
                licenses.Add(license)
                Console.WriteLine($"✅ Created license {i}: {license.LicenseKey}")
            Next

            ' 2. Batch License Validation
            Console.WriteLine(vbCrLf & "2. Batch License Validation")
            Console.WriteLine(New String("-"c, 30))

            For Each license In licenses
                Dim isValid As Boolean = Await client.ValidateLicenseAsync(license.LicenseKey)
                Console.WriteLine($"License {license.LicenseKey}: {(If(isValid, "✅ Valid", "❌ Invalid"))}")
            Next

            ' 3. License Analytics
            Console.WriteLine(vbCrLf & "3. License Analytics")
            Console.WriteLine(New String("-"c, 30))

            Dim analytics As LicenseAnalytics = Await client.GetLicenseAnalyticsAsync()
            Console.WriteLine($"Total Licenses: {analytics.TotalLicenses}")
            Console.WriteLine($"Active Licenses: {analytics.ActiveLicenses}")
            Console.WriteLine($"Expired Licenses: {analytics.ExpiredLicenses}")
            Console.WriteLine($"Revenue: ${analytics.Revenue:F2}")

            ' 4. User Management
            Console.WriteLine(vbCrLf & "4. User Management")
            Console.WriteLine(New String("-"c, 30))

            ' Create user
            Dim user As User = Await client.CreateUserAsync(New UserRegistration With {
                .Username = "advanced_user",
                .Email = "advanced@example.com",
                .Password = "secure_password123",
                .Metadata = New Dictionary(Of String, Object) From {
                    {"company", "Advanced Corp"},
                    {"plan", "enterprise"}
                }
            })
            Console.WriteLine($"✅ Created user: {user.Username}")

            ' Update user
            user.Metadata("plan") = "premium"
            Dim updatedUser As User = Await client.UpdateUserAsync(user.Id, user)
            Console.WriteLine($"✅ Updated user plan to: {updatedUser.Metadata("plan")}")

            ' 5. Webhook Handling
            Console.WriteLine(vbCrLf & "5. Webhook Handling")
            Console.WriteLine(New String("-"c, 30))

            Dim webhookHandler As New WebhookHandler("your-webhook-secret")
            
            ' Simulate webhook events
            Dim webhookEvents As New List(Of WebhookEvent) From {
                New WebhookEvent With {.Type = "license.created", .Data = licenses(0)},
                New WebhookEvent With {.Type = "user.registered", .Data = user},
                New WebhookEvent With {.Type = "license.updated", .Data = licenses(1)}
            }

            For Each webhookEvent In webhookEvents
                Dim isValidSignature As Boolean = webhookHandler.VerifySignature(
                    Utils.ToJson(webhookEvent.Data),
                    "simulated-signature"
                )
                Console.WriteLine($"Webhook {webhookEvent.Type}: {(If(isValidSignature, "✅ Valid", "❌ Invalid"))}")
                Await webhookHandler.ProcessEventAsync(webhookEvent.Type, webhookEvent.Data)
            Next

            ' 6. Error Handling and Retry Logic
            Console.WriteLine(vbCrLf & "6. Error Handling and Retry Logic")
            Console.WriteLine(New String("-"c, 30))

            Try
                ' Simulate a failed operation with retry
                Await Utils.RetryAsync(
                    Async Function() As Task
                        ' This would normally be a real API call
                        Throw New Exception("Simulated network error")
                    End Function,
                    3, ' max retries
                    1000 ' base delay
                )
            Catch ex As Exception
                Console.WriteLine($"❌ Operation failed after retries: {ex.Message}")
            End Try

            ' 7. Data Export
            Console.WriteLine(vbCrLf & "7. Data Export")
            Console.WriteLine(New String("-"c, 30))

            ' Export licenses to JSON
            Dim exportData As New With {
                .ExportDate = DateTime.UtcNow,
                .Licenses = licenses,
                .Users = New List(Of User) From {user},
                .Analytics = analytics
            }

            Dim jsonExport As String = Utils.ToJson(exportData)
            Console.WriteLine($"✅ Exported {licenses.Count} licenses and {1} user")
            Console.WriteLine($"Export size: {jsonExport.Length} characters")

            ' 8. Performance Monitoring
            Console.WriteLine(vbCrLf & "8. Performance Monitoring")
            Console.WriteLine(New String("-"c, 30))

            Dim stopwatch As New System.Diagnostics.Stopwatch()
            
            ' Measure API call performance
            stopwatch.Start()
            Dim performanceTest As License = Await client.CreateLicenseAsync("perf_test", "perf_product")
            stopwatch.Stop()
            
            Console.WriteLine($"✅ License creation took: {stopwatch.ElapsedMilliseconds}ms")

            ' 9. Cleanup
            Console.WriteLine(vbCrLf & "9. Cleanup")
            Console.WriteLine(New String("-"c, 30))

            ' Revoke test licenses
            For Each license In licenses
                Dim revoked As Boolean = Await client.RevokeLicenseAsync(license.Id)
                Console.WriteLine($"License {license.LicenseKey}: {(If(revoked, "✅ Revoked", "❌ Failed to revoke"))}")
            Next

            ' Delete test user
            Dim deleted As Boolean = Await client.DeleteUserAsync(user.Id)
            Console.WriteLine($"User {user.Username}: {(If(deleted, "✅ Deleted", "❌ Failed to delete"))}")

            Console.WriteLine(vbCrLf & "🎉 Advanced usage example completed successfully!")

        Catch ex As Exception
            Console.WriteLine($"❌ Error: {ex.Message}")
            Console.WriteLine($"Stack trace: {ex.StackTrace}")
        Finally
            ' Cleanup resources
            client?.Dispose()
        End Try

        Console.WriteLine(vbCrLf & "Press any key to exit...")
        Console.ReadKey()
    End Function

    ' Helper class for webhook events
    Public Class WebhookEvent
        Public Property Type As String
        Public Property Data As Object
    End Class
End Module
