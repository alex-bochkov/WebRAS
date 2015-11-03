
Imports Newtonsoft.Json

Partial Class _Default
    Inherits System.Web.UI.Page

    Private Sub _Default_Load(sender As Object, e As EventArgs) Handles Me.Load

        If Not IsPostBack Then

            ShowTree()

        Else

            If CurrentOperation.Attributes.Count > 0 Then

                Dim Action = CurrentOperation.Attributes.Item("Action")
                Dim ClusterID = CurrentOperation.Attributes.Item("ClusterID")

                DoTheAction(Action, ClusterID)

            End If

        End If

        Labellogin.Text = "Пользователь: " + User.Identity.Name
        DivWarning.InnerText = ""

    End Sub

    Sub ShowTree()

        MainOperations.Visible = True

        Dim TreeNodeMain = New TreeNode
        TreeNodeMain.Value = "MainPage"
        TreeNodeMain.Text = "Начальная страница"

        TreeView.Nodes.Add(TreeNodeMain)

        Dim ServersList As List(Of RAS.Service1C) = RAS.GetClusterList(GetServerListFromCookie())
        'Dim ServersList As List(Of RAS.Service1C) = GerServerListFromCookie()

        For Each Server1C As RAS.Service1C In ServersList

            Dim TreeNodeServer = New TreeNode
            TreeNodeServer.Value = Server1C.ID
            TreeNodeServer.Text = Server1C.Descr + ":" + Server1C.Port.ToString

            If Server1C.Connected Then
                For Each Cl In Server1C.Clusters

                    Dim TreeNode = New TreeNode
                    TreeNode.Value = Cl.StringID
                    TreeNode.Text = Cl.Name + ":" + Cl.Port.ToString ' + "<br/> <a href=""about:blank"">New</a>"

                    TreeNode.ChildNodes.Add(New TreeNode With {.Text = "Databases", .Value = "Databases"})
                    TreeNode.ChildNodes.Add(New TreeNode With {.Text = "Sessions", .Value = "Sessions"})
                    TreeNode.ChildNodes.Add(New TreeNode With {.Text = "Connections", .Value = "Connections"})
                    TreeNode.ChildNodes.Add(New TreeNode With {.Text = "Processes", .Value = "Processes"})

                    TreeNodeServer.ChildNodes.Add(TreeNode)

                Next
            Else
                TreeNodeServer.Text = TreeNodeServer.Text + " / " + Server1C.ConnectionError
            End If

            TreeView.Nodes.Add(TreeNodeServer)

        Next

        If ServersList.Count > 1 Then
            'Покажем сводные списки данных

            Dim TreeNodeTotals = New TreeNode
            TreeNodeTotals.Text = "Сводные данные"
            TreeNodeTotals.ChildNodes.Add(New TreeNode With {.Text = "All databases", .Value = "AllDatabases"})
            TreeNodeTotals.ChildNodes.Add(New TreeNode With {.Text = "All sessions", .Value = "AllSessions"})
            TreeNodeTotals.ChildNodes.Add(New TreeNode With {.Text = "All connections", .Value = "AllConnections"})
            TreeNodeTotals.ChildNodes.Add(New TreeNode With {.Text = "All processes", .Value = "AllProcesses"})

            TreeView.Nodes.Add(TreeNodeTotals)

        End If


        ''Раздел с настройками
        'Dim TreeNodeSetting = New TreeNode
        'TreeNodeSetting.Text = "Setting"

        'TreeNodeSetting.ChildNodes.Add(New TreeNode With {.Text = "Saved credentials", .Value = "ShowSavedUsers"})

        'TreeView.Nodes.Add(TreeNodeSetting)


        SaveCache(ServersList)


    End Sub

    Sub SaveCache(ServersList As List(Of Service1C))

        HttpContext.Current.Cache.Remove("ServersList")

        HttpContext.Current.Cache.Add("ServersList", ServersList, Nothing, Cache.NoAbsoluteExpiration, New TimeSpan(0, 15, 0), CacheItemPriority.Normal, Nothing)

    End Sub

    Sub ShowDatabases(ClusterID As String)

        DataTable.Rows.Clear()

        Dim FirstCellHeader = New TableHeaderCell With {.Text = ""}
        FirstCellHeader.Attributes.Add("data-sorter", "false")

        Dim Row = New TableHeaderRow
        Row.Cells.Add(FirstCellHeader)
        Row.Cells.Add(New TableHeaderCell With {.Text = "Имя ИБ"})
        Row.Cells.Add(New TableHeaderCell With {.Text = "Описание"})
        Row.Cells.Add(New TableHeaderCell With {.Text = "УИД"})
        Row.TableSection = TableRowSection.TableHeader
        DataTable.Rows.Add(Row)

        Dim RowNumber = 1

        Dim ServersList As List(Of RAS.Service1C) = HttpContext.Current.Cache.Get("ServersList")

        For Each Server1C In ServersList
            For Each Cl In Server1C.Clusters
                If ClusterID = "" Or Cl.StringID = ClusterID Then

                    Cl.FillInfobasesList(Server1C.GetConnection())

                    For Each IB In Cl.Infobases

                        Dim Link As LinkButton = New LinkButton
                        Link.Text = "..."
                        Link.ID = "OpenIBForm_" + RowNumber.ToString
                        Link.Attributes.Add("href", "#")

                        Dim PostBackUrl = Link.ResolveClientUrl("~/ib.aspx?ServerID=" + Server1C.ID.ToString + "&ClusterID=" + Cl.StringID + "&InfoBaseID=" + IB.StringID)

                        Link.OnClientClick = "window.open('" + PostBackUrl + "','name','width=500,height=800')"
                        'Как-то не очень красиво открывается диалог..
                        'Link.OnClientClick = "OpenInfoBaseForm('" + PostBackUrl + "')"


                        Dim FirstCell = New TableCell
                        FirstCell.Controls.Add(Link)

                        If CredMngr.CredentialsForObjectExists(Request, IB.StringID) Then

                            Dim CredPic As Image = New Image
                            CredPic.ImageUrl = CredPic.ResolveClientUrl("~/Content/cred.png")
                            FirstCell.Controls.Add(CredPic)

                        End If

                        Dim RowIB = New TableRow
                        RowIB.Cells.Add(FirstCell)
                        RowIB.Cells.Add(New TableCell With {.Text = IB.Name})
                        RowIB.Cells.Add(New TableCell With {.Text = IB.Descr})
                        RowIB.Cells.Add(New TableCell With {.Text = IB.StringID})
                        DataTable.Rows.Add(RowIB)

                        RowNumber = RowNumber + 1

                    Next



                End If
            Next
        Next

    End Sub

    Sub ShowSessions(ClusterID As String)

        DataTable.Rows.Clear()

        Dim Row = New TableHeaderRow
        Row.Cells.Add(New TableHeaderCell With {.Text = ""})
        Row.Cells.Add(New TableHeaderCell With {.Text = "ИБ"})
        Row.Cells.Add(New TableHeaderCell With {.Text = "Имя пользователя"})
        Row.Cells.Add(New TableHeaderCell With {.Text = "Имя приложения"})
        Row.Cells.Add(New TableHeaderCell With {.Text = "Начало работы"})
        Row.Cells.Add(New TableHeaderCell With {.Text = "Последняя активность"})
        Row.Cells.Add(New TableHeaderCell With {.Text = "УИД ИБ"})
        Row.Cells.Add(New TableHeaderCell With {.Text = "Лицензия"})
        Row.TableSection = TableRowSection.TableHeader
        DataTable.Rows.Add(Row)

        Dim RowNumber = 1

        Dim ServersList As List(Of RAS.Service1C) = HttpContext.Current.Cache.Get("ServersList")

        For Each Server1C In ServersList
            For Each Cl In Server1C.Clusters
                If ClusterID = "" Or Cl.StringID = ClusterID Then


                    Cl.FillUserSessions(Server1C.GetConnection())



                    For Each S In Cl.UserSessions

                        Dim ButtonDelete As Button = New Button()
                        ButtonDelete.Text = "Kill"
                        ButtonDelete.ID = "KillSession_" + RowNumber.ToString
                        'ButtonDelete.ControlStyle.CssClass = "button"
                        ButtonDelete.Attributes.Add("SessionID", S.ID.toString)
                        ButtonDelete.Attributes.Add("ClusterID", Cl.StringID)
                        AddHandler ButtonDelete.Click, AddressOf ButtonKillSession_Click

                        Dim FirstCell = New TableCell
                        FirstCell.Controls.Add(ButtonDelete)


                        Dim RowIB = New TableRow
                        RowIB.Cells.Add(FirstCell)
                        RowIB.Cells.Add(New TableCell With {.Text = S.InfoBaseName})
                        RowIB.Cells.Add(New TableCell With {.Text = S.UserName})
                        RowIB.Cells.Add(New TableCell With {.Text = S.AppName})
                        RowIB.Cells.Add(New TableCell With {.Text = S.StartedAt})
                        RowIB.Cells.Add(New TableCell With {.Text = S.LastActiveAt})
                        RowIB.Cells.Add(New TableCell With {.Text = S.InfoBaseID})
                        RowIB.Cells.Add(New TableCell With {.Text = S.LicensePresentation})
                        'RowIB.ID = S.InfoBaseID

                        DataTable.Rows.Add(RowIB)

                        RowNumber = RowNumber + 1

                    Next



                End If
            Next
        Next

    End Sub

    Sub ShowConnections(ClusterID As String)

        DataTable.Rows.Clear()

        Dim Row = New TableHeaderRow
        Row.Cells.Add(New TableHeaderCell With {.Text = ""})
        Row.Cells.Add(New TableHeaderCell With {.Text = "InfobaseName"})
        Row.Cells.Add(New TableHeaderCell With {.Text = "ConnId"})
        Row.Cells.Add(New TableHeaderCell With {.Text = "SessionNumber"})
        Row.Cells.Add(New TableHeaderCell With {.Text = "Application"})
        Row.Cells.Add(New TableHeaderCell With {.Text = "Host"})
        Row.Cells.Add(New TableHeaderCell With {.Text = "BlockedByLs"})
        Row.Cells.Add(New TableHeaderCell With {.Text = "ConnectedAt"})
        Row.TableSection = TableRowSection.TableHeader
        DataTable.Rows.Add(Row)

        Dim RowNumber = 1

        Dim ServersList As List(Of RAS.Service1C) = HttpContext.Current.Cache.Get("ServersList")

        For Each Server1C In ServersList
            For Each Cl In Server1C.Clusters
                If ClusterID = "" Or Cl.StringID = ClusterID Then


                    Cl.FillConnections(Server1C.GetConnection())



                    For Each S In Cl.Connections

                        Dim ButtonDelete As Button = New Button()
                        ButtonDelete.Text = "Kill"
                        ButtonDelete.ID = "KillConnection_" + RowNumber.ToString
                        ButtonDelete.Attributes.Add("ConnectionID", S.ID.toString)
                        ButtonDelete.Attributes.Add("ClusterID", Cl.StringID)
                        AddHandler ButtonDelete.Click, AddressOf ButtonKillConnection_Click
                        Dim FirstCell = New TableCell
                        FirstCell.Controls.Add(ButtonDelete)

                        Dim RowIB = New TableRow
                        RowIB.Cells.Add(FirstCell)
                        RowIB.Cells.Add(New TableCell With {.Text = S.InfoBaseName})
                        RowIB.Cells.Add(New TableCell With {.Text = S.ConnId.ToString})
                        RowIB.Cells.Add(New TableCell With {.Text = S.SessionNumber.ToString})
                        RowIB.Cells.Add(New TableCell With {.Text = S.Application})
                        RowIB.Cells.Add(New TableCell With {.Text = S.Host})
                        RowIB.Cells.Add(New TableCell With {.Text = S.BlockedByLs.ToString})
                        RowIB.Cells.Add(New TableCell With {.Text = S.ConnectedAt.ToString})

                        DataTable.Rows.Add(RowIB)

                        RowNumber = RowNumber + 1

                    Next



                End If
            Next
        Next

    End Sub

    Sub ShowProcesses(ClusterID As String)

        DataTable.Rows.Clear()

        Dim Row = New TableHeaderRow
        Row.Cells.Add(New TableHeaderCell With {.Text = ""})
        Row.Cells.Add(New TableHeaderCell With {.Text = "PID"})
        Row.Cells.Add(New TableHeaderCell With {.Text = "Main port"})
        Row.Cells.Add(New TableHeaderCell With {.Text = "Memory, Kb"})
        Row.Cells.Add(New TableHeaderCell With {.Text = "Enabled"})
        Row.Cells.Add(New TableHeaderCell With {.Text = "Use"})
        Row.Cells.Add(New TableHeaderCell With {.Text = "Running"})
        Row.Cells.Add(New TableHeaderCell With {.Text = "Started at"})
        Row.Cells.Add(New TableHeaderCell With {.Text = "License"})

        Row.TableSection = TableRowSection.TableHeader
        DataTable.Rows.Add(Row)

        ' Dim RowNumber = 1

        Dim ServersList As List(Of RAS.Service1C) = HttpContext.Current.Cache.Get("ServersList")

        For Each Server1C In ServersList
            For Each Cl In Server1C.Clusters
                If ClusterID = "" Or Cl.StringID = ClusterID Then

                    Cl.FillProcessesData(Server1C.GetConnection())

                    For Each S In Cl.Processes

                        Dim RowIB = New TableRow
                        RowIB.Cells.Add(New TableCell With {.Text = ""})
                        RowIB.Cells.Add(New TableCell With {.Text = S.PID})
                        RowIB.Cells.Add(New TableCell With {.Text = S.MainPort.ToString})
                        RowIB.Cells.Add(New TableCell With {.Text = S.Memory.ToString})
                        RowIB.Cells.Add(New TableCell With {.Text = S.isEnable.ToString})
                        RowIB.Cells.Add(New TableCell With {.Text = S.Use.ToString})
                        RowIB.Cells.Add(New TableCell With {.Text = S.Running.ToString})
                        RowIB.Cells.Add(New TableCell With {.Text = S.StartedAt.ToString})
                        RowIB.Cells.Add(New TableCell With {.Text = S.LicensePresentation})

                        DataTable.Rows.Add(RowIB)

                    Next



                End If
            Next
        Next

    End Sub

    Protected Sub ButtonKillSession_Click(sender As Object, e As System.EventArgs)

        DivWarning.InnerText = ""

        Dim SessionID As String = sender.Attributes("SessionID")
        Dim ClusterID As String = sender.Attributes("ClusterID")

        Dim ServersList As List(Of RAS.Service1C) = HttpContext.Current.Cache.Get("ServersList")

        For Each Server1C In ServersList
            For Each Cl In Server1C.Clusters
                If Cl.StringID = ClusterID Then

                    Try
                        Cl.KillSession(Server1C.GetConnection(), SessionID)
                    Catch ex As Exception
                        DivWarning.InnerText = ex.Message
                    End Try

                End If
            Next
        Next

        SaveCache(ServersList)

        DoTheAction("Sessions", ClusterID)

    End Sub

    Protected Sub ButtonKillConnection_Click(sender As Object, e As System.EventArgs)

        DivWarning.InnerText = ""

        Dim ConnectionID As String = sender.Attributes("ConnectionID")
        Dim ClusterID As String = sender.Attributes("ClusterID")

        Dim ServersList As List(Of RAS.Service1C) = HttpContext.Current.Cache.Get("ServersList")

        For Each Server1C In ServersList
            For Each Cl In Server1C.Clusters
                If Cl.StringID = ClusterID Then

                    Try
                        Cl.KillConnection(Server1C.GetConnection(), ConnectionID)
                    Catch ex As Exception
                        DivWarning.InnerText = ex.Message
                    End Try

                End If
            Next
        Next

        SaveCache(ServersList)

        DoTheAction("Connections", ClusterID)

    End Sub



    Protected Sub TreeView_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView.SelectedNodeChanged

        Dim Node = DirectCast(sender, System.Web.UI.WebControls.TreeView).SelectedNode

        If Node.Depth = 0 Then
            If Node.Value = "MainPage" Then
                DoTheAction("MainPage", Node.Value)
            Else
                DoTheAction("ServerPage", Node.Value)
            End If
            ' ElseIf Node.Depth = 1 Then
            '    DoTheAction("ClusterPage", Node.Value)
        Else
            DoTheAction(Node.Value, Node.Parent.Value)
        End If

    End Sub

    Sub DoTheAction(Action As String, ClusterId As String)

        If String.IsNullOrEmpty(Action) Then
            Exit Sub
        End If

        MainOperations.Visible = False
        ServerOperations.Visible = False
        ClusterOperations.Visible = False
        DatabasesOperations.Visible = False
        SessionOperations.Visible = False
        ConnectionsOperations.Visible = False
        ProcessesOperations.Visible = False

        DataTable.Rows.Clear()

        If Action = "Databases" Then

            DatabasesOperations.Visible = True

            ShowDatabases(ClusterId)

        ElseIf Action = "Sessions" Then

            SessionOperations.Visible = True

            RefreshSessionsList.Attributes.Clear()
            RefreshSessionsList.Attributes.Add("ClusterID", ClusterId)

            ShowSessions(ClusterId)

        ElseIf Action = "Connections" Then

            ConnectionsOperations.Visible = True

            RefreshConnectionsList.Attributes.Clear()
            RefreshConnectionsList.Attributes.Add("ClusterID", ClusterId)

            ShowConnections(ClusterId)

        ElseIf Action = "Processes" Then

            ProcessesOperations.Visible = True

            RefreshProcessesList.Attributes.Clear()
            RefreshProcessesList.Attributes.Add("ClusterID", ClusterId)

            ShowProcesses(ClusterId)

        ElseIf Action = "MainPage" Then

            MainOperations.Visible = True

        ElseIf Action = "ClusterPage" Then

            ClusterOperations.Visible = True

        ElseIf Action = "ServerPage" Then

            ServerOperations.Visible = True

            DeleteThisServer.Attributes.Clear()
            DeleteThisServer.Attributes.Add("ServerID", ClusterId)

        ElseIf Action = "AllDatabases" Then
            ShowDatabases("")
        ElseIf Action = "AllSessions" Then
            ShowSessions("")
        ElseIf Action = "AllConnections" Then
            ShowConnections("")
        ElseIf Action = "AllProcesses" Then
            ShowProcesses("")
        End If

        CurrentOperation.Attributes.Clear()
        CurrentOperation.Attributes.Add("Action", Action)
        CurrentOperation.Attributes.Add("ClusterID", ClusterId)

    End Sub

    Function GetServerListFromCookie() As List(Of RAS.Service1C)

        Dim List = New List(Of RAS.Service1C)

        Try

            Dim UID = DirectCast(User.Identity, System.Security.Principal.WindowsIdentity).User.Value

            Dim FilePath = HttpContext.Current.Server.MapPath("~/App_Data/" + UID + ".json")

            Dim UserData = My.Computer.FileSystem.ReadAllText(FilePath)

            List = JsonConvert.DeserializeObject(Of List(Of RAS.Service1C))(UserData)

        Catch ex As Exception

        End Try

        Return List

    End Function

    Sub SaveServerListToCookie(ServerList As List(Of RAS.Service1C))

        Dim UID = DirectCast(User.Identity, System.Security.Principal.WindowsIdentity).User.Value

        Dim FilePath = HttpContext.Current.Server.MapPath("~/App_Data/" + UID + ".json")

        Dim JSONSetting = JsonConvert.SerializeObject(ServerList)

        My.Computer.FileSystem.WriteAllText(FilePath, JSONSetting, False)

    End Sub

    Function AddNewServerToList(ServerName As String, ServerPort As Integer, Descr As String) As List(Of RAS.Service1C)

        Dim List = New List(Of RAS.Service1C)

        Dim ServersList As List(Of RAS.Service1C) = HttpContext.Current.Cache.Get("ServersList")

        Dim MaxID = 0

        For Each Server1C In ServersList

            Server1C.Clusters.Clear()
            Server1C.ConnectionError = ""

            MaxID = Math.Max(Server1C.ID, MaxID)

            List.Add(Server1C)

        Next


        Dim Server = New RAS.Service1C(MaxID + 1, ServerName, ServerPort)
        Server.Descr = Descr
        Server.Username = ""
        Server.Password = ""

        List.Add(Server)

        Return List

    End Function

    Private Sub AddNewServer_Click(sender As Object, e As EventArgs) Handles AddNewServer.Click

        Dim List = AddNewServerToList("localhost", 1545, "Local server")

        SaveServerListToCookie(List)

        Response.Redirect(Request.Url.AbsoluteUri)

    End Sub

    Private Sub DeleteThisServer_Click(sender As Object, e As EventArgs) Handles DeleteThisServer.Click

        Try

            Dim ServerID = Integer.Parse(DeleteThisServer.Attributes.Item("ServerID"))

            Dim ServersList = GetServerListFromCookie()

            For Each Server1C In ServersList
                If Server1C.ID = ServerID Then
                    ServersList.Remove(Server1C)
                    Exit For
                End If
            Next

            SaveServerListToCookie(ServersList)

            Response.Redirect(Request.Url.AbsoluteUri)

        Catch ex As Exception

        End Try

    End Sub

    Private Sub RefreshSessionsList_Click(sender As Object, e As EventArgs) Handles RefreshSessionsList.Click

        Dim ClusterID = RefreshSessionsList.Attributes.Item("ClusterID")

        ShowSessions(ClusterID)

    End Sub

    Private Sub SetServerListByDefault_Click(sender As Object, e As EventArgs) Handles SetServerListByDefault.Click

        SaveCache(New List(Of Service1C))

        Dim List = New List(Of RAS.Service1C)

        If My.Computer.Name = "ALEX-PC" Then

            List.Add(New RAS.Service1C(1, "localhost", 1545) With {.Descr = "Local server"})
            List.Add(New RAS.Service1C(2, "localhost", 1545) With {.Descr = "Local server"})

        ElseIf My.Computer.Name = "SRV1007" Then

            List.Add(New RAS.Service1C(1, "srv1007", 1745) With {.Descr = "srv1007:1745"})
            List.Add(New RAS.Service1C(2, "srv1007", 2545) With {.Descr = "srv1007:2545"})
            List.Add(New RAS.Service1C(3, "srv1007", 1545) With {.Descr = "srv1007:1545"})
            ' List.Add(New RAS.Service1C(4, "msc-zvg-1c-01v", 1745) With {.Descr = "Тестовый кластер"})
            List.Add(New RAS.Service1C(5, "msc-zvg-1c-01v", 1945) With {.Descr = "Тестовый кластер"})
            List.Add(New RAS.Service1C(6, "msc-zvg-1c-01v", 2145) With {.Descr = "Тестовый кластер"})
            List.Add(New RAS.Service1C(7, "msc-zvg-1c-01v", 2245) With {.Descr = "Тестовый кластер"})
            List.Add(New RAS.Service1C(8, "msc-zvg-1c-01v", 2345) With {.Descr = "Тестовый кластер"})
        Else

            List.Add(New RAS.Service1C(1, "localhost", 1545) With {.Descr = "Local server"})

        End If

        SaveServerListToCookie(List)

        Response.Redirect(Request.Url.AbsoluteUri)

    End Sub

    Private Sub ResetAllConnections_Click(sender As Object, e As EventArgs) Handles ResetAllConnections.Click

    End Sub

    Private Sub RefreshConnectionsList_Click(sender As Object, e As EventArgs) Handles RefreshConnectionsList.Click

        Dim ClusterID = RefreshConnectionsList.Attributes.Item("ClusterID")

        ShowConnections(ClusterID)

    End Sub

    Private Sub RefreshProcessesList_Click(sender As Object, e As EventArgs) Handles RefreshProcessesList.Click

        Dim ClusterID = RefreshProcessesList.Attributes.Item("ClusterID")

        ShowProcesses(ClusterID)

    End Sub
End Class


