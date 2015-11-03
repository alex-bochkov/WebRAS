
Partial Class ib
    Inherits System.Web.UI.Page

    Private Sub ib_Load(sender As Object, e As EventArgs) Handles Me.Load

        If Not IsPostBack Then

            ShowInfoBaseForm()

        End If


    End Sub


    Sub ShowInfoBaseForm()

        Dim InfoBaseStringID As String = Request.QueryString.Item("InfoBaseID")
        Dim ClusterID As String = Request.QueryString.Item("ClusterID")
        Dim ServerID As String = Request.QueryString.Item("ServerID")


        If Not String.IsNullOrEmpty(InfoBaseStringID) _
            And Not String.IsNullOrEmpty(ClusterID) _
            And Not String.IsNullOrEmpty(ServerID) Then

            Dim ServersList As List(Of RAS.Service1C) = HttpContext.Current.Cache.Get("ServersList")

            For Each Server1C In ServersList
                If Server1C.ID.ToString = ServerID Then

                    Dim StrUserName = UserName.Text
                    Dim StrPassword = Password.Text

                    If String.IsNullOrEmpty(StrUserName) Then
                        CredMngr.GetUserNameForInfobase(Request, ClusterID, InfoBaseStringID, StrUserName, StrPassword)
                    End If

                    Dim Rez = RAS.GetInfoBaseFullInfo(Server1C, ClusterID, InfoBaseStringID, StrUserName, StrPassword, RememberMe.Checked)

                    If RememberMe.Checked And Rez.Success Then
                        CredMngr.SaveUserNameForInfobase(Request, Response, ClusterID, InfoBaseStringID, StrUserName, StrPassword)
                    End If

                    If Rez.Success Then

                        AuthForm.Visible = False
                        ObjectForm.Visible = True

                        LabelUserName.Text = "Авторизованы под пользователем: " + StrUserName

                        InfoBaseName.Text = Rez.InfoBase.Name
                        InfoBaseDescr.Text = Rez.InfoBase.Descr
                        InfoBaseID.Text = Rez.InfoBase.StringID
                        InfoBaseDbServerName.Text = Rez.InfoBase.DbServerName
                        InfoBaseDbPassword.Text = Rez.InfoBase.DbPassword

                        InfoBaseDbms.Text = Rez.InfoBase.Dbms
                        InfoBaseDatabaseName.Text = Rez.InfoBase.DbmsDB
                        InfoBaseDbUser.Text = Rez.InfoBase.DbUser

                        If Rez.InfoBase.DeniedFrom.Year > 1 Then
                            InfoBaseDeniedFrom.Text = Rez.InfoBase.DeniedFrom.ToString("yyyy-MM-dd HH:mm:ss")
                        End If
                        If Rez.InfoBase.DeniedTo.Year > 1 Then
                            InfoBaseDeniedTo.Text = Rez.InfoBase.DeniedTo.ToString("yyyy-MM-dd HH:mm:ss")
                        End If

                        InfoBaseDeniedParameter.Text = Rez.InfoBase.DeniedParameter
                        InfoBaseDeniedMessage.Text = Rez.InfoBase.DeniedMessage
                        InfoBasePermissionCode.Text = Rez.InfoBase.PermissionCode
                        InfoBaseDbUser.Text = Rez.InfoBase.DbUser
                        InfoBaseDateOffset.Text = Rez.InfoBase.DateOffset
                        InfoBaseLocale.Text = Rez.InfoBase.Locale

                        CheckBoxLicenseDistributionAllowed.Checked = Rez.InfoBase.LicenseDistributionAllowed
                        CheckBoxSessionsDenied.Checked = Rez.InfoBase.SessionsDenied
                        CheckBoxScheduledJobsDenied.Checked = Rez.InfoBase.ScheduledJobsDenied

                    ElseIf Rez.ErrorCode = ErrorCodes.AuthError Then

                        AuthForm.Visible = True
                        ObjectForm.Visible = False

                        LabelErrorText.Text = Rez.Description

                    Else

                    End If

                End If

            Next

        End If

    End Sub

    Private Sub ButtonLogin_Click(sender As Object, e As EventArgs) Handles ButtonLogin.Click

        ShowInfoBaseForm()

    End Sub

End Class
