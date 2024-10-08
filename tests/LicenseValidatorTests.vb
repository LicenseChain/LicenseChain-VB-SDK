Imports NUnit.Framework

<TestFixture>
Public Class LicenseValidatorTests
    <Test>
    Public Async Function TestValidateLicense() As Task
        Dim validator As New LicenseValidator("test_api_key")
        Dim result As Boolean = Await validator.ValidateLicense("test_license_key")
        Assert.IsTrue(result)
    End Function
End Class
