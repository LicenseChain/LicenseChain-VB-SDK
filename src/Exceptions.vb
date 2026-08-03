Imports System

Namespace LicenseChain.VB

    ''' <summary>
    ''' Base exception for all LicenseChain errors
    ''' </summary>
    Public Class LicenseChainException
        Inherits Exception

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(message As String)
            MyBase.New(message)
        End Sub

        Public Sub New(message As String, innerException As Exception)
            MyBase.New(message, innerException)
        End Sub

    End Class

    ''' <summary>
    ''' Exception thrown when authentication fails
    ''' </summary>
    Public Class AuthenticationException
        Inherits LicenseChainException

        Public Sub New()
            MyBase.New("Authentication failed")
        End Sub

        Public Sub New(message As String)
            MyBase.New(message)
        End Sub

        Public Sub New(message As String, innerException As Exception)
            MyBase.New(message, innerException)
        End Sub

    End Class

    ''' <summary>
    ''' Exception thrown when request validation fails
    ''' </summary>
    Public Class ValidationException
        Inherits LicenseChainException

        Public Sub New()
            MyBase.New("Validation failed")
        End Sub

        Public Sub New(message As String)
            MyBase.New(message)
        End Sub

        Public Sub New(message As String, innerException As Exception)
            MyBase.New(message, innerException)
        End Sub

    End Class

    ''' <summary>
    ''' Exception thrown when a resource is not found
    ''' </summary>
    Public Class NotFoundException
        Inherits LicenseChainException

        Public Sub New()
            MyBase.New("Resource not found")
        End Sub

        Public Sub New(message As String)
            MyBase.New(message)
        End Sub

        Public Sub New(message As String, innerException As Exception)
            MyBase.New(message, innerException)
        End Sub

    End Class

    ''' <summary>
    ''' Exception thrown when rate limit is exceeded
    ''' </summary>
    Public Class RateLimitException
        Inherits LicenseChainException

        Public Sub New()
            MyBase.New("Rate limit exceeded")
        End Sub

        Public Sub New(message As String)
            MyBase.New(message)
        End Sub

        Public Sub New(message As String, innerException As Exception)
            MyBase.New(message, innerException)
        End Sub

    End Class

    ''' <summary>
    ''' Exception thrown when server returns an error
    ''' </summary>
    Public Class ServerException
        Inherits LicenseChainException

        Public Sub New()
            MyBase.New("Server error")
        End Sub

        Public Sub New(message As String)
            MyBase.New(message)
        End Sub

        Public Sub New(message As String, innerException As Exception)
            MyBase.New(message, innerException)
        End Sub

    End Class

    ''' <summary>
    ''' Exception thrown when network operations fail
    ''' </summary>
    Public Class NetworkException
        Inherits LicenseChainException

        Public Sub New()
            MyBase.New("Network error")
        End Sub

        Public Sub New(message As String)
            MyBase.New(message)
        End Sub

        Public Sub New(message As String, innerException As Exception)
            MyBase.New(message, innerException)
        End Sub

    End Class

End Namespace
