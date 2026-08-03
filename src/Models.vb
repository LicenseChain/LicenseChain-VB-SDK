Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace LicenseChain.VB

    ''' <summary>
    ''' Base model class for all LicenseChain entities
    ''' </summary>
    Public MustInherit Class BaseModel
        Protected _data As Dictionary(Of String, Object)

        Public Sub New()
            _data = New Dictionary(Of String, Object)()
        End Sub

        Public Sub New(data As Dictionary(Of String, Object))
            _data = If(data, New Dictionary(Of String, Object)())
        End Sub

        Default Public Property Item(key As String) As Object
            Get
                Return If(_data.ContainsKey(key), _data(key), Nothing)
            End Get
            Set(value As Object)
                _data(key) = value
            End Set
        End Property

        Public Function ToDictionary() As Dictionary(Of String, Object)
            Return New Dictionary(Of String, Object)(_data)
        End Function

        Public Overrides Function ToString() As String
            Return JsonConvert.SerializeObject(_data, Formatting.Indented)
        End Function

    End Class

    ''' <summary>
    ''' User model
    ''' </summary>
    Public Class User
        Inherits BaseModel

        Public ReadOnly Property Id As String
            Get
                Return If(Me("id"), String.Empty)
            End Get
        End Property

        Public ReadOnly Property Email As String
            Get
                Return If(Me("email"), String.Empty)
            End Get
        End Property

        Public ReadOnly Property Name As String
            Get
                Return If(Me("name"), String.Empty)
            End Get
        End Property

        Public ReadOnly Property Company As String
            Get
                Return If(Me("company"), String.Empty)
            End Get
        End Property

        Public ReadOnly Property CreatedAt As DateTime?
            Get
                Dim value = Me("created_at")
                If value IsNot Nothing AndAlso DateTime.TryParse(value.ToString(), Nothing) Then
                    Return DateTime.Parse(value.ToString())
                End If
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property UpdatedAt As DateTime?
            Get
                Dim value = Me("updated_at")
                If value IsNot Nothing AndAlso DateTime.TryParse(value.ToString(), Nothing) Then
                    Return DateTime.Parse(value.ToString())
                End If
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property EmailVerified As Boolean
            Get
                Return If(Me("email_verified"), False)
            End Get
        End Property

        Public ReadOnly Property IsActive As Boolean
            Get
                Return If(Me("status"), "active") = "active"
            End Get
        End Property

    End Class

    ''' <summary>
    ''' Application model
    ''' </summary>
    Public Class Application
        Inherits BaseModel

        Public ReadOnly Property Id As String
            Get
                Return If(Me("id"), String.Empty)
            End Get
        End Property

        Public ReadOnly Property Name As String
            Get
                Return If(Me("name"), String.Empty)
            End Get
        End Property

        Public ReadOnly Property Description As String
            Get
                Return If(Me("description"), String.Empty)
            End Get
        End Property

        Public ReadOnly Property ApiKey As String
            Get
                Return If(Me("api_key"), String.Empty)
            End Get
        End Property

        Public ReadOnly Property WebhookUrl As String
            Get
                Return If(Me("webhook_url"), String.Empty)
            End Get
        End Property

        Public ReadOnly Property AllowedOrigins As String()
            Get
                Dim value = Me("allowed_origins")
                If value IsNot Nothing AndAlso TypeOf value Is String() Then
                    Return DirectCast(value, String())
                End If
                Return New String() {}
            End Get
        End Property

        Public ReadOnly Property CreatedAt As DateTime?
            Get
                Dim value = Me("created_at")
                If value IsNot Nothing AndAlso DateTime.TryParse(value.ToString(), Nothing) Then
                    Return DateTime.Parse(value.ToString())
                End If
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property UpdatedAt As DateTime?
            Get
                Dim value = Me("updated_at")
                If value IsNot Nothing AndAlso DateTime.TryParse(value.ToString(), Nothing) Then
                    Return DateTime.Parse(value.ToString())
                End If
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property IsActive As Boolean
            Get
                Return If(Me("status"), "active") = "active"
            End Get
        End Property

        Public ReadOnly Property LicenseCount As Integer
            Get
                Return If(Me("license_count"), 0)
            End Get
        End Property

    End Class

    ''' <summary>
    ''' License model
    ''' </summary>
    Public Class License
        Inherits BaseModel

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(data As Dictionary(Of String, Object))
            MyBase.New(data)
        End Sub

        Public ReadOnly Property Id As String
            Get
                Return If(Me("id"), String.Empty)
            End Get
        End Property

        Public ReadOnly Property Key As String
            Get
                Return If(Me("key"), String.Empty)
            End Get
        End Property

        Public ReadOnly Property AppId As String
            Get
                Return If(Me("app_id"), String.Empty)
            End Get
        End Property

        Public ReadOnly Property UserId As String
            Get
                Return If(Me("user_id"), String.Empty)
            End Get
        End Property

        Public ReadOnly Property UserEmail As String
            Get
                Return If(Me("user_email"), String.Empty)
            End Get
        End Property

        Public ReadOnly Property UserName As String
            Get
                Return If(Me("user_name"), String.Empty)
            End Get
        End Property

        Public ReadOnly Property Status As String
            Get
                Return If(Me("status"), String.Empty)
            End Get
        End Property

        Public ReadOnly Property ExpiresAt As DateTime?
            Get
                Dim value = Me("expires_at")
                If value IsNot Nothing AndAlso DateTime.TryParse(value.ToString(), Nothing) Then
                    Return DateTime.Parse(value.ToString())
                End If
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property CreatedAt As DateTime?
            Get
                Dim value = Me("created_at")
                If value IsNot Nothing AndAlso DateTime.TryParse(value.ToString(), Nothing) Then
                    Return DateTime.Parse(value.ToString())
                End If
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property UpdatedAt As DateTime?
            Get
                Dim value = Me("updated_at")
                If value IsNot Nothing AndAlso DateTime.TryParse(value.ToString(), Nothing) Then
                    Return DateTime.Parse(value.ToString())
                End If
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property Metadata As Dictionary(Of String, Object)
            Get
                Dim value = Me("metadata")
                If value IsNot Nothing AndAlso TypeOf value Is Dictionary(Of String, Object) Then
                    Return DirectCast(value, Dictionary(Of String, Object))
                End If
                Return New Dictionary(Of String, Object)()
            End Get
        End Property

        Public ReadOnly Property Features As String()
            Get
                Dim value = Me("features")
                If value IsNot Nothing AndAlso TypeOf value Is String() Then
                    Return DirectCast(value, String())
                End If
                Return New String() {}
            End Get
        End Property

        Public ReadOnly Property UsageCount As Integer
            Get
                Return If(Me("usage_count"), 0)
            End Get
        End Property

        Public ReadOnly Property IsActive As Boolean
            Get
                Return If(Me("status"), "active") = "active"
            End Get
        End Property

        Public ReadOnly Property IsExpired As Boolean
            Get
                If ExpiresAt.HasValue Then
                    Return ExpiresAt.Value < DateTime.Now
                End If
                Return False
            End Get
        End Property

        Public ReadOnly Property IsRevoked As Boolean
            Get
                Return If(Me("status"), "revoked") = "revoked"
            End Get
        End Property

        Public ReadOnly Property DaysUntilExpiration As Integer?
            Get
                If ExpiresAt.HasValue Then
                    Return CInt(Math.Ceiling((ExpiresAt.Value - DateTime.Now).TotalDays))
                End If
                Return Nothing
            End Get
        End Property

    End Class

    ''' <summary>
    ''' Webhook model
    ''' </summary>
    Public Class Webhook
        Inherits BaseModel

        Public ReadOnly Property Id As String
            Get
                Return If(Me("id"), String.Empty)
            End Get
        End Property

        Public ReadOnly Property AppId As String
            Get
                Return If(Me("app_id"), String.Empty)
            End Get
        End Property

        Public ReadOnly Property Url As String
            Get
                Return If(Me("url"), String.Empty)
            End Get
        End Property

        Public ReadOnly Property Events As String()
            Get
                Dim value = Me("events")
                If value IsNot Nothing AndAlso TypeOf value Is String() Then
                    Return DirectCast(value, String())
                End If
                Return New String() {}
            End Get
        End Property

        Public ReadOnly Property Secret As String
            Get
                Return If(Me("secret"), String.Empty)
            End Get
        End Property

        Public ReadOnly Property IsActive As Boolean
            Get
                Return If(Me("status"), "active") = "active"
            End Get
        End Property

        Public ReadOnly Property CreatedAt As DateTime?
            Get
                Dim value = Me("created_at")
                If value IsNot Nothing AndAlso DateTime.TryParse(value.ToString(), Nothing) Then
                    Return DateTime.Parse(value.ToString())
                End If
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property UpdatedAt As DateTime?
            Get
                Dim value = Me("updated_at")
                If value IsNot Nothing AndAlso DateTime.TryParse(value.ToString(), Nothing) Then
                    Return DateTime.Parse(value.ToString())
                End If
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property LastTriggeredAt As DateTime?
            Get
                Dim value = Me("last_triggered_at")
                If value IsNot Nothing AndAlso DateTime.TryParse(value.ToString(), Nothing) Then
                    Return DateTime.Parse(value.ToString())
                End If
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property FailureCount As Integer
            Get
                Return If(Me("failure_count"), 0)
            End Get
        End Property

    End Class

    ''' <summary>
    ''' Analytics model
    ''' </summary>
    Public Class Analytics
        Inherits BaseModel

        Public ReadOnly Property TotalLicenses As Integer
            Get
                Return If(Me("total_licenses"), 0)
            End Get
        End Property

        Public ReadOnly Property ActiveLicenses As Integer
            Get
                Return If(Me("active_licenses"), 0)
            End Get
        End Property

        Public ReadOnly Property ExpiredLicenses As Integer
            Get
                Return If(Me("expired_licenses"), 0)
            End Get
        End Property

        Public ReadOnly Property RevokedLicenses As Integer
            Get
                Return If(Me("revoked_licenses"), 0)
            End Get
        End Property

        Public ReadOnly Property ValidationsToday As Integer
            Get
                Return If(Me("validations_today"), 0)
            End Get
        End Property

        Public ReadOnly Property ValidationsThisWeek As Integer
            Get
                Return If(Me("validations_this_week"), 0)
            End Get
        End Property

        Public ReadOnly Property ValidationsThisMonth As Integer
            Get
                Return If(Me("validations_this_month"), 0)
            End Get
        End Property

        Public ReadOnly Property TopFeatures As String()
            Get
                Dim value = Me("top_features")
                If value IsNot Nothing AndAlso TypeOf value Is String() Then
                    Return DirectCast(value, String())
                End If
                Return New String() {}
            End Get
        End Property

    End Class

    ''' <summary>
    ''' Paginated response model
    ''' </summary>
    Public Class PaginatedResponse(Of T)
        Inherits BaseModel

        Public ReadOnly Property Data As T()
            Get
                Dim value = Me("data")
                If value IsNot Nothing AndAlso TypeOf value Is T() Then
                    Return DirectCast(value, T())
                End If
                Return New T() {}
            End Get
        End Property

        Public ReadOnly Property Page As Integer
            Get
                Return If(Me("page"), 1)
            End Get
        End Property

        Public ReadOnly Property Limit As Integer
            Get
                Return If(Me("limit"), 20)
            End Get
        End Property

        Public ReadOnly Property Total As Integer
            Get
                Return If(Me("total"), 0)
            End Get
        End Property

        Public ReadOnly Property TotalPages As Integer
            Get
                Return If(Me("total_pages"), 1)
            End Get
        End Property

        Public ReadOnly Property HasNextPage As Boolean
            Get
                Return Page < TotalPages
            End Get
        End Property

        Public ReadOnly Property HasPreviousPage As Boolean
            Get
                Return Page > 1
            End Get
        End Property

        Public ReadOnly Property NextPage As Integer?
            Get
                Return If(HasNextPage, Page + 1, Nothing)
            End Get
        End Property

        Public ReadOnly Property PreviousPage As Integer?
            Get
                Return If(HasPreviousPage, Page - 1, Nothing)
            End Get
        End Property

    End Class

End Namespace
