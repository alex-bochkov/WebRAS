Imports Newtonsoft.Json

Public Module CredMngr

    Structure Credential
        ' Dim ClusterID As String
        ' Dim InfoBaseID As String
        Dim ObjectID As String
        Dim Username As String
        Dim Password As String
    End Structure

    Private Function GetSavedCredential(Request As HttpRequest, ObjectID As String) As Credential

        Dim Cred = New Credential
        Cred.Username = ""
        Cred.Password = ""
        Cred.ObjectID = ObjectID

        Try

            Dim CredentialString As HttpCookie = Request.Cookies.Get("Credential_" + ObjectID)

            Dim RezultStringJSON As FormsAuthenticationTicket = FormsAuthentication.Decrypt(CredentialString.Value)

            Cred = JsonConvert.DeserializeObject(Of Credential)(RezultStringJSON.UserData)

        Catch ex As Exception

        End Try

        Return Cred

    End Function

    Public Function CredentialsForObjectExists(Request As HttpRequest, ObjectID As String) As Boolean

        Try

            Dim CredentialString As HttpCookie = Request.Cookies.Get("Credential_" + ObjectID)

            Dim RezultStringJSON As FormsAuthenticationTicket = FormsAuthentication.Decrypt(CredentialString.Value)

            Dim Cred = JsonConvert.DeserializeObject(Of Credential)(RezultStringJSON.UserData)

            Return True

        Catch ex As Exception

        End Try

        Return False

    End Function



    Public Function GetUserNameForInfobase(Request As HttpRequest, ClusterID As String, InfoBaseID As String,
                                           ByRef UserName As String, ByRef Password As String) As Boolean

        Try

            Dim Cred = GetSavedCredential(Request, InfoBaseID)

            If Not String.IsNullOrEmpty(Cred.Username) Then
                UserName = Cred.Username
                Password = Cred.Password
            End If

        Catch ex As Exception

            Dim a = ex.Message

            Return False

        End Try

        Return True

    End Function

    Public Function SaveUserNameForInfobase(Request As HttpRequest, Response As HttpResponse, ClusterID As String,
                                            InfoBaseID As String, UserName As String, Password As String) As Boolean

        Try

            Dim Cred = GetSavedCredential(Request, InfoBaseID)

            Cred.Username = UserName
            Cred.Password = Password

            Dim JSONSetting = JsonConvert.SerializeObject(Cred)

            Dim ticket As FormsAuthenticationTicket = New FormsAuthenticationTicket(1, "Credentials", DateTime.Now, DateTime.MaxValue, True, JSONSetting)

            Dim cookie As HttpCookie = New HttpCookie("Credential_" + Cred.ObjectID, FormsAuthentication.Encrypt(ticket))

            Response.Cookies.Add(cookie)

        Catch ex As Exception

            Dim a = ex.Message

            Return False

        End Try

        Return True

    End Function


End Module
