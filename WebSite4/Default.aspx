<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Remote Administration Server client</title>
    <link rel="shortcut icon" href="favicon.ico" />
    <!--Bootstrap-->
    <link href="Content/bootstrap.min.css" rel="stylesheet"/>
    <!--Тема для таблицы-->
    <link href="Content/theme.default.css" rel="stylesheet"/>
    <link href="Scripts/jquery-ui/jquery-ui.css" rel="stylesheet" />


    <style>
        .table > tbody > tr > td, .table > tbody > tr > th, .table > tfoot > tr > td, .table > tfoot > tr > th, .table > thead > tr > td, .table > thead > tr > th {
            padding: 1px;
        }
    </style>
</head>
<body>
    <div id="BoxForm"></div>


    <form id="form1" runat="server">
        <div style="float: left; width: 20%;">
            <asp:Label ID="Labellogin" runat="server" Text="Label"></asp:Label>
            <asp:TreeView ID="TreeView" runat="server" ImageSet="Simple">
                <HoverNodeStyle Font-Underline="True" ForeColor="#5555DD" />
                <NodeStyle Font-Names="Tahoma" Font-Size="10pt" ForeColor="Black" HorizontalPadding="0px" NodeSpacing="0px" VerticalPadding="0px" />
                <ParentNodeStyle Font-Bold="False" />
                <SelectedNodeStyle Font-Underline="True" ForeColor="#5555DD" HorizontalPadding="0px" VerticalPadding="0px" />
            </asp:TreeView>
        </div>
        <div style="float: left; width: 80%;">
            <div id="DivWarning" runat="server">
            </div>
            <asp:TextBox ID="CurrentOperation" runat="server" Style="display: none" Visible="False"></asp:TextBox>
            <div id="MainOperations" runat="server" Visible="False">
                <asp:Button ID="AddNewServer" runat="server" Text="Добавить существующий сервер" />
                <asp:Button ID="SetServerListByDefault" runat="server" Text=" ! установить список серверов по умолчанию ! " />
            </div>
            <div id="ServerOperations" runat="server" Visible="False">
                <asp:Button ID="AddNewCluster" runat="server" Text="Создать новый кластер" />
                <asp:Button ID="DeleteThisServer" runat="server" Text="Удалить сервер из списка" />
            </div>
             <div id="ClusterOperations" runat="server" Visible="False">
                <asp:Button ID="AddAnything" runat="server" Text="Создать что-нибудь внутри кластера" />
            </div>
            <div id="DatabasesOperations" runat="server" Visible="False">
                <asp:Button ID="AddNewDatabase" runat="server" Text="Создать новую базу (не работает)" />
            </div>
            <div id="SessionOperations" runat="server" Visible="False">
                <asp:Button ID="RefreshSessionsList" runat="server" Text="Обновить список" />
                <asp:Button ID="ResetAllSessions" runat="server" Text="Отключить всех (не работает)" />
            </div>
             <div id="ConnectionsOperations" runat="server" Visible="False">
                <asp:Button ID="RefreshConnectionsList" runat="server" Text="Обновить список" />
                <asp:Button ID="ResetAllConnections" runat="server" Text="Отключить всех (не работает)" />
            </div>
            <div id="ProcessesOperations" runat="server" Visible="False">
                <asp:Button ID="RefreshProcessesList" runat="server" Text="Обновить список" />
            </div>
            <asp:Table ID="DataTable" runat="server" class="table tablesorter tablesorter-default"></asp:Table>            

        </div>
    </form>

    <script src="scripts/jquery-1.9.1.min.js"></script>
    <script src="scripts/bootstrap.min.js"></script>

    <script src="Scripts/jquery-ui/jquery-ui.js"></script>


    <script type="text/javascript" src="scripts/jquery.tablesorter.js"></script>

    <script>
        $(document).ready(function () {
            $(function () {
                $("#DataTable").tablesorter();
            });
        });

        function OpenInfoBaseForm(Str) {
            var div = $(document.getElementById("BoxForm"));
            div.html('<iframe id="myAdhocIframeWindow" frameborder="0" width="100%" height="100%" src="' + Str + '" />');
            div.dialog();
        }
    </script>



</body>
</html>
