Imports com._1c.v8.ibis


Public Module RAS

    Enum ErrorCodes
        AuthError
    End Enum

    Class Process
        Public ID As java.util.UUID
        Public isEnable As Boolean = False
        Public Use As Boolean = False
        Public Running As Boolean = False
        Public StartedAt As DateTime
        Public PID As String
        Public MainPort As Integer
        Public Memory As Integer
        Public LicensePresentation As String
    End Class

    Class Connection
        Public ID As java.util.UUID
        Public InfobaseID As java.util.UUID
        Public InfoBaseName As String
        Public WorkingProcessId As java.util.UUID
        Public ConnId As Integer
        Public SessionNumber As Integer
        Public Application As String
        Public Host As String
        Public BlockedByLs As Integer
        Public ConnectedAt As DateTime
    End Class

    Class UserSession
        Public UserName As String
        Public StartedAt As DateTime
        Public LastActiveAt As DateTime
        Public ID As java.util.UUID
        Public StringID As String
        Public InfoBaseID As String
        Public InfoBaseName As String
        Public AppName As String
        Public LicensePresentation As String
    End Class

    Structure RequestResult
        Dim Success As Boolean
        Dim Description As String
        Dim ErrorCode As ErrorCodes
        Dim InfoBase As InfoBase
    End Structure

    Class InfoBase

        Public Name As String
        Public ID As java.util.UUID
        Public StringID As String
        Public Descr As String = ""
        Public DbPassword As String = ""
        Public DbServerName As String = ""
        Public Dbms As String = ""
        Public DbmsDB As String = ""
        Public DbUser As String = ""

        Public DateOffset As Integer
        Public LicenseDistributionAllowed As Boolean = False
        Public Locale As String = ""
        Public ScheduledJobsDenied As Boolean = False
        Public SessionsDenied As Boolean = False


        Public DeniedFrom As DateTime
        Public DeniedTo As DateTime
        Public DeniedMessage As String = ""
        Public DeniedParameter As String = ""
        Public PermissionCode As String = ""

    End Class

    Class Cluster

        Public Name As String
        Public Port As Integer
        Public UserName As String = ""
        Public Password As String = ""
        Public ID As java.util.UUID
        Public StringID As String
        Public Infobases As List(Of InfoBase)
        Public UserSessions As List(Of UserSession)
        Public Processes As List(Of Process)
        Public Connections As List(Of Connection)

        Public Sub New()
            Infobases = New List(Of InfoBase)
            UserSessions = New List(Of UserSession)
            Processes = New List(Of Process)
            Connections = New List(Of Connection)
        End Sub

        Private Sub FillUserSessionsInternal(Connection As admin.IAgentAdminConnection)

            'если список ИБ не заполним - инициализируем его.
            'это нужно для подстановки имени ИБ по ГУИД
            If Infobases.Count = 0 Then
                FillInfobasesListInternal(Connection)
            End If

            UserSessions.Clear()

            Dim Sessions = Connection.getSessions(ID)

            For Each Session As admin.ISessionInfo In Sessions.toArray

                Dim S = New UserSession
                S.UserName = Session.getUserName()

                S.StartedAt = Date.Parse(Session.getStartedAt().toGMTString())
                If S.StartedAt.Year = 1969 Then
                    S.StartedAt = New DateTime
                End If

                S.LastActiveAt = Date.Parse(Session.getLastActiveAt().toGMTString())
                If S.LastActiveAt.Year = 1969 Then
                    S.LastActiveAt = New DateTime
                End If

                'S.getStartedAt = Session.getStartedAt().toString
                S.InfoBaseID = Session.getInfoBaseId.toString
                S.ID = Session.getSid
                S.StringID = S.ID.toString
                S.AppName = Session.getAppId

                For Each IB In Infobases
                    If IB.StringID = S.InfoBaseID Then
                        S.InfoBaseName = IB.Name
                        Exit For
                    End If
                Next

                Dim Licenses = Session.getLicenses()

                For Each License As admin.ILicenseInfo In Licenses.toArray
                    S.LicensePresentation = S.LicensePresentation + IIf(String.IsNullOrEmpty(S.LicensePresentation), "", "; ") + License.getFullPresentation
                Next

                UserSessions.Add(S)

            Next

        End Sub

        Private Sub FillConnectionsInternal(Connection As admin.IAgentAdminConnection)

            'если список ИБ не заполним - инициализируем его.
            'это нужно для подстановки имени ИБ по ГУИД
            If Infobases.Count = 0 Then
                FillInfobasesListInternal(Connection)
            End If

            Connections.Clear()

            Dim ClusterConnections = Connection.getConnectionsShort(ID)

            For Each Conn As admin.IInfoBaseConnectionShort In ClusterConnections.toArray

                Dim S = New Connection
                S.Application = Conn.getApplication()

                S.ConnectedAt = Date.Parse(Conn.getConnectedAt().toGMTString())
                If S.ConnectedAt.Year = 1969 Then
                    S.ConnectedAt = New DateTime
                End If
                S.BlockedByLs = Conn.getBlockedByLs()
                S.ConnId = Conn.getConnId()
                S.Host = Conn.getHost()
                S.ID = Conn.getInfoBaseConnectionId()
                S.InfobaseID = Conn.getInfoBaseId()
                S.SessionNumber = Conn.getSessionNumber()
                S.WorkingProcessId = Conn.getWorkingProcessId()

                For Each IB In Infobases
                    If IB.StringID = S.InfobaseID.toString Then
                        S.InfoBaseName = IB.Name
                        Exit For
                    End If
                Next

                Connections.Add(S)

            Next

        End Sub

        Private Sub FillProcessesDataInternal(Connection As admin.IAgentAdminConnection)

            ''если список ИБ не заполним - инициализируем его.
            ''это нужно для подстановки имени ИБ по ГУИД
            'If Infobases.Count = 0 Then
            '    FillInfobasesListInternal(Connection)
            'End If

            Processes.Clear()

            Dim ProcessesList = Connection.getWorkingProcesses(ID)

            For Each ProcessItem As admin.IWorkingProcessInfo In ProcessesList.toArray

                Dim S = New Process
                S.ID = ProcessItem.getWorkingProcessId()
                S.PID = ProcessItem.getPid
                S.isEnable = ProcessItem.isEnable()
                S.Use = (ProcessItem.getUse() = 1)
                S.Running = (ProcessItem.getRunning() = 1)

                S.StartedAt = Date.Parse(ProcessItem.getStartedAt().toGMTString())
                If S.StartedAt.Year = 1969 Then
                    S.StartedAt = New DateTime
                End If

                S.MainPort = ProcessItem.getMainPort()
                S.Memory = ProcessItem.getMemorySize()

                Dim Licenses = ProcessItem.getLicense()

                For Each License As admin.ILicenseInfo In Licenses.toArray
                    S.LicensePresentation = S.LicensePresentation + IIf(String.IsNullOrEmpty(S.LicensePresentation), "", "; ") + License.getFullPresentation
                Next

                Processes.Add(S)

            Next

        End Sub

        Private Sub FillInfobasesListInternal(Connection As admin.IAgentAdminConnection)

            Infobases.Clear()

            Dim bases = Connection.getInfoBasesShort(ID)

            For Each Base As admin.IInfoBaseInfoShort In bases.toArray

                Dim IB = New InfoBase
                IB.Name = Base.getName
                IB.ID = Base.getInfoBaseId
                IB.StringID = IB.ID.toString
                IB.Descr = Base.getDescr

                Infobases.Add(IB)

            Next

        End Sub

        Public Function FillInfobasesList(Connection As admin.IAgentAdminConnection) As Boolean

            Connection.authenticate(ID, UserName, Password)

            FillInfobasesListInternal(Connection)

            Return True

        End Function

        Public Function FillConnections(Connection As admin.IAgentAdminConnection) As Boolean

            Connection.authenticate(ID, UserName, Password)

            FillConnectionsInternal(Connection)

            Return True

        End Function

        Public Function FillUserSessions(Connection As admin.IAgentAdminConnection) As Boolean

            Connection.authenticate(ID, UserName, Password)

            FillUserSessionsInternal(Connection)

            Return True

        End Function

        Public Function FillProcessesData(Connection As admin.IAgentAdminConnection) As Boolean

            Connection.authenticate(ID, UserName, Password)

            FillProcessesDataInternal(Connection)

            Return True

        End Function

        Public Function KillSession(Connection As admin.IAgentAdminConnection, SessionID As String) As Boolean

            Connection.authenticate(ID, UserName, Password)

            For Each US In UserSessions
                If US.StringID = SessionID Then

                    Connection.terminateSession(ID, US.ID)

                End If

            Next

            FillUserSessionsInternal(Connection)

            Return True

        End Function

        Public Function KillConnection(Connection As admin.IAgentAdminConnection, ConnectionID As String) As Boolean

            Connection.authenticate(ID, UserName, Password)

            For Each Conn In Connections
                If Conn.ID.toString = ConnectionID Then

                    Connection.disconnect(ID, Conn.WorkingProcessId, Conn.ID)

                End If

            Next

            FillConnectionsInternal(Connection)

            Return True

        End Function

        Public Function GetInfoBaseInfo(Connection As admin.IAgentAdminConnection, InfoBaseID As String,
                                        Optional UserNameIB As String = "", Optional PasswordIB As String = "") As InfoBase

            Connection.authenticate(ID, UserName, Password)

            Dim Rez As InfoBase = New InfoBase

            Connection.addAuthentication(ID, UserNameIB, PasswordIB)

            For Each IB In Infobases

                If IB.StringID = InfoBaseID Then

                    Dim IBInfo = Connection.getInfoBaseInfo(ID, IB.ID)

                    IB.DbPassword = IBInfo.getDbPassword()
                    IB.DbServerName = IBInfo.getDbServerName()

                    IB.Dbms = IBInfo.getDbms()
                    IB.DbmsDB = IBInfo.getDbName()
                    IB.DbUser = IBInfo.getDbUser()

                    IB.DeniedFrom = Date.Parse(IBInfo.getDeniedFrom().toGMTString())
                    If IB.DeniedFrom.Year = 1969 Then
                        IB.DeniedFrom = New DateTime
                    End If
                    IB.DeniedTo = Date.Parse(IBInfo.getDeniedTo().toGMTString())
                    If IB.DeniedTo.Year = 1969 Then
                        IB.DeniedTo = New DateTime
                    End If

                    IB.DeniedMessage = IBInfo.getDeniedMessage
                    IB.DeniedParameter = IBInfo.getDeniedParameter
                    IB.PermissionCode = IBInfo.getPermissionCode

                    IB.DateOffset = IBInfo.getDateOffset
                    IB.LicenseDistributionAllowed = (IBInfo.getLicenseDistributionAllowed = 1)
                    IB.Locale = IBInfo.getLocale
                    IB.ScheduledJobsDenied = IBInfo.isScheduledJobsDenied
                    IB.SessionsDenied = IBInfo.isSessionsDenied


                    Return IB

                End If

            Next

            Return Rez

        End Function



    End Class



    Class Service1C

        Public ID As Integer
        Public Name As String
        Public Port As Integer
        Public Descr As String

        Public Connected As Boolean = False
        Public ConnectionError As String = ""

        Public Username As String
        Public Password As String

        Public Clusters As List(Of Cluster)


        Public Sub New(iID As Integer, sName As String, iPort As Integer)

            Clusters = New List(Of Cluster)

            ID = iID
            Name = sName
            Port = iPort

        End Sub

        Public Function GetConnection() As admin.IAgentAdminConnection

            Dim conn = New admin.client.AgentAdminConnector(60)

            Dim connection = conn.connect(Name, Port)
            If Not String.IsNullOrEmpty(Username) Then
                connection.authenticateAgent(Username, Password)
            End If

            Return connection

        End Function

        Public Function FillUserSessions(ClusterID As String) As List(Of UserSession)

            Dim Rez As List(Of UserSession) = New List(Of UserSession)

            For Each Cluster In Clusters

                If Cluster.StringID = ClusterID Then

                    Cluster.FillUserSessions(GetConnection())

                    Return Cluster.UserSessions

                End If

            Next

            Return Rez

        End Function

        Public Function GetInfoBaseInfo(ClusterID As String, InfoBaseID As String,
                                        UserNameIB As String, PasswordIB As String) As InfoBase

            Dim Rez As InfoBase = New InfoBase

            For Each Cluster In Clusters

                If Cluster.StringID = ClusterID Then

                    Rez = Cluster.GetInfoBaseInfo(GetConnection(), InfoBaseID, UserNameIB, PasswordIB)

                    Return Rez

                End If

            Next

            Return Rez

        End Function

    End Class

    Public Function GetClusterList(ServersOptions As List(Of Service1C)) As List(Of Service1C)

        Dim Rez = New List(Of Service1C)

        'Dim ServersOptions = CredMngr.GetServersList()

        For Each ServerOption In ServersOptions

            Try

                Dim Srv = New Service1C(ServerOption.ID, ServerOption.Name, ServerOption.Port)
                Srv.Username = ServerOption.Username
                Srv.Password = ServerOption.Password
                Srv.Descr = ServerOption.Descr

                Try
                    Dim connection = Srv.GetConnection()

                    Dim Clusters = connection.getClusters()

                    For Each Cluster As admin.ClusterInfo In Clusters.toArray
                        Try
                            Dim Cl = New Cluster
                            Cl.Name = Cluster.getName()
                            Cl.Port = Cluster.getMainPort()
                            Cl.ID = Cluster.getClusterId()
                            Cl.StringID = Cl.ID.toString

                            Srv.Clusters.Add(Cl)
                        Catch ex As Exception
                        End Try
                    Next

                    Srv.Connected = True

                Catch ex As Exception

                    Srv.ConnectionError = ex.Message

                End Try

                Rez.Add(Srv)

            Catch ex As Exception

            End Try

        Next

        Return Rez


    End Function

    Public Function GetInfoBaseFullInfo(Server As Service1C, ClusterID As String, InfoBaseID As String,
                                        ByRef UserName As String, Password As String, SaveIt As Boolean) As RequestResult

        Dim Rez = New RequestResult
        Rez.Success = False

        Try

            Rez.InfoBase = Server.GetInfoBaseInfo(ClusterID, InfoBaseID, UserName, Password)
            Rez.Success = True

        Catch ex As Exception

            Rez.ErrorCode = ErrorCodes.AuthError
            Rez.Description = ex.Message

        End Try

        Return Rez


    End Function

End Module
