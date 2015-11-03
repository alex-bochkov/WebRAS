<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ib.aspx.vb" Inherits="ib" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Параметры ИБ</title>
    <style>
        .textlabel {
            display: inline-block;
            width: 145px;
        }

        .form-control {
            width: 250px;
        }

        p {
            margin-top: 4px;
            margin-bottom: 4px;
        }
    </style>
</head>
<body>


    <form id="form1" runat="server">
        <div id="AuthForm" runat="server">
            <fieldset>

                <!-- Form Name -->
                <legend>Авторизация</legend>

                <div class="alert alert-danger" role="alert">
                    <span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span>
                    <span class="sr-only"></span>
                    Error:
                                        <asp:Label ID="LabelErrorText" runat="server" Text=""></asp:Label>
                </div>

                <!-- Text input-->
                <div class="form-group">
                    <label class="col-md-3 control-label" for="textinput">Имя пользователя</label>
                    <div class="col-md-9">
                        <asp:TextBox ID="UserName" runat="server" placeholder="" class="form-control" type="text"></asp:TextBox>
                    </div>
                </div>

                <!-- Text input-->
                <div class="form-group">
                    <label class="col-md-3 control-label" for="textinput">Пароль</label>
                    <div class="col-md-9">
                        <asp:TextBox ID="Password" runat="server" placeholder="" class="form-control" type="text"></asp:TextBox>
                    </div>
                </div>

                <!-- Prepended checkbox -->
                <div class="form-group">
                    <label class="col-md-3 control-label" for="prependedcheckbox"></label>
                    <div class="col-md-9">
                        <div class="input-group">
                            <asp:CheckBox ID="RememberMe" runat="server" Text="Сохранить учетные данные в cookies" />
                        </div>
                    </div>
                </div>

                <!-- Button -->
                <div class="form-group">
                    <label class="col-md-3 control-label" for="singlebutton"></label>
                    <div class="col-md-9">
                        <asp:Button ID="ButtonLogin" runat="server" Text="Авторизоваться" name="singlebutton" class="btn btn-primary" />
                    </div>
                </div>

            </fieldset>
        </div>
        <div id="ObjectForm" runat="server">

            <fieldset>
                <!-- Form Name -->
                <legend>Параметры информационной базы</legend>

                <asp:Label ID="LabelUserName" runat="server" Text="" />

                <!-- GUID-->
                <p>
                    <label class="textlabel" for="InfoBaseID">GUID</label>
                    <asp:TextBox ID="InfoBaseID" runat="server" ReadOnly="True" class="form-control" type="text"></asp:TextBox>
                </p>
                <!-- Имя базы-->
                <p>
                    <label class="textlabel" for="InfoBaseName">Имя базы</label>
                    <asp:TextBox ID="InfoBaseName" runat="server" class="form-control" type="text"></asp:TextBox>
                </p>

                <!-- Описание-->
                <p>
                    <label class="textlabel" for="InfoBaseDescr">Описание</label>
                    <asp:TextBox ID="InfoBaseDescr" runat="server" class="form-control" type="text"></asp:TextBox>
                </p>

                <fieldset>
                    <legend>Подключение к СУБД:</legend>

                    <!-- Тип СУБД -->
                    <p>
                        <label class="textlabel" for="InfoBaseDbms">Тип СУБД:</label>
                        <asp:TextBox ID="InfoBaseDbms" runat="server" class="form-control" type="text"></asp:TextBox>
                    </p>
                    
                    <!-- Сервер СУБД -->
                    <p>
                        <label class="textlabel" for="InfoBaseDbServerName">Сервер СУБД:</label>
                        <asp:TextBox ID="InfoBaseDbServerName" runat="server" class="form-control" type="text"></asp:TextBox>
                    </p>

                    <!-- Имя БД -->
                    <p>
                        <label class="textlabel" for="InfoBaseDbName">Имя базы:</label>
                        <asp:TextBox ID="InfoBaseDatabaseName" runat="server" class="form-control" type="text"></asp:TextBox>
                    </p>

                    <!-- Пользователь СУБД-->
                    <p>
                        <label class="textlabel" for="InfoBaseDbUser">Пользователь:</label>
                        <asp:TextBox ID="InfoBaseDbUser" runat="server" class="form-control" type="text"></asp:TextBox>
                    </p>

                    <!-- Пароль СУБД:-->
                    <p>
                        <label class="textlabel" for="InfoBaseDbPassword">Пароль:</label>
                        <asp:TextBox ID="InfoBaseDbPassword" runat="server" class="form-control" type="text"></asp:TextBox>
                    </p>

                    <!-- Смещение дат -->
                    <p>
                        <label class="textlabel" for="InfoBaseDateOffset">Смещение дат:</label>
                        <asp:TextBox ID="InfoBaseDateOffset" runat="server" class="form-control" type="text"></asp:TextBox>
                    </p>

                    <!-- Locale -->
                    <p>
                        <label class="textlabel" for="InfoBaseLocale">Locale:</label>
                        <asp:TextBox ID="InfoBaseLocale" runat="server" class="form-control" type="text"></asp:TextBox>
                    </p>

                    

                </fieldset>

                <fieldset>
                    <legend>Блокировка установки соединений</legend>
                    <!-- Блокировка соединений-->
                    <p>
                        <asp:CheckBox ID="CheckBoxSessionsDenied" runat="server" Text="Блокировать установку соединений" />
                    </p>
                    <!-- Начало блокировки -->
                    <p>
                        <label class="textlabel" for="InfoBaseDeniedFrom">Начало:</label>
                        <asp:TextBox ID="InfoBaseDeniedFrom" runat="server" class="form-control" type="text" placeholder="yyyy-MM-dd HH:mm:ss"></asp:TextBox><br />

                        <!--Temporary disabled <a href="#" onclick="SetBlockTime('InfoBaseDeniedFrom', 0); return false;">Now</a>
                        <a href="#" onclick="SetBlockTime('InfoBaseDeniedFrom', 5); return false;">Now + 5 min</a>
                        <a href="#" onclick="SetBlockTime('InfoBaseDeniedFrom', 10); return false;">Now + 10 min</a>
                        <a href="#" onclick="SetBlockTime('InfoBaseDeniedFrom', 20); return false;">Now + 20 min</a>
                        <a href="#" onclick="SetBlockTime('InfoBaseDeniedFrom', 30); return false;">Now + 30 min</a>-->


                    </p>

                    <!-- Окончание блокировки -->
                    <p>
                        <label class="textlabel" for="InfoBaseDeniedTo">Окончание:</label>
                        <asp:TextBox ID="InfoBaseDeniedTo" runat="server" class="form-control" type="text" placeholder="yyyy-MM-dd HH:mm:ss"></asp:TextBox><br />

                        <!--Temporary disabled <a href="#" onclick="SetBlockTime('InfoBaseDeniedTo', 0); return false;">Now</a>
                        <a href="#" onclick="SetBlockTime('InfoBaseDeniedTo', 5); return false;">Now + 5 min</a>
                        <a href="#" onclick="SetBlockTime('InfoBaseDeniedTo', 10); return false;">Now + 10 min</a>
                        <a href="#" onclick="SetBlockTime('InfoBaseDeniedTo', 20); return false;">Now + 20 min</a>
                        <a href="#" onclick="SetBlockTime('InfoBaseDeniedTo', 30); return false;">Now + 30 min</a>
                        <a href="#" onclick="SetBlockTime('InfoBaseDeniedTo', 60); return false;">Now + 60 min</a>-->

                    </p>

                    <!-- Сообщение блокировки -->
                    <p>
                        <label class="textlabel" for="InfoBaseDeniedMessage">Сообщение:</label>
                        <asp:TextBox ID="InfoBaseDeniedMessage" runat="server" class="form-control" type="text"></asp:TextBox>
                    </p>

                    <!-- Код разрешения-->
                    <p>
                        <label class="textlabel" for="InfoBasePermissionCode">Код разрешения:</label>
                        <asp:TextBox ID="InfoBasePermissionCode" runat="server" class="form-control" type="text"></asp:TextBox>
                    </p>
                    <!-- Параметр блокировки-->
                    <p>
                        <label class="textlabel" for="InfoBaseDeniedParameter">Параметр:</label>
                        <asp:TextBox ID="InfoBaseDeniedParameter" runat="server" class="form-control" type="text"></asp:TextBox>
                    </p>

                </fieldset>

                <!-- Разрешена выдача лицензий сервером-->
                <p>
                    <asp:CheckBox ID="CheckBoxLicenseDistributionAllowed" runat="server" Text="Разрешена выдача лицензий сервером" />
                </p>

                <!-- Блокировка заданий-->
                <p>
                    <asp:CheckBox ID="CheckBoxScheduledJobsDenied" runat="server" Text="Блокировка заданий" />
                </p>
                <!-- Button (Double) -->
                <div class="form-group">
                    <div class="col-md-9">
                        <button id="button1id" name="button1id" class="btn btn-success">Сохранить изменения</button>
                        <button id="button2id" name="button2id" class="btn btn-danger" onclick="window.close();">Закрыть форму</button>
                    </div>
                </div>

            </fieldset>

        </div>
    </form>

    <script src="scripts/jquery-1.9.1.min.js"></script>


    <script>

        /*var OldValueInfoBaseName = "";

        $('#InfoBaseName').bind('input', function () {
            $(this).next().stop(true, true).fadeIn(0).html('').fadeOut(2000);

            var InfoBaseDbName = document.getElementById("InfoBaseDatabaseName");

            if (InfoBaseDbName.value == OldValueInfoBaseName || InfoBaseDbName.value == "") {
                InfoBaseDbName.value = $(this).val();
            }

            OldValueInfoBaseName = $(this).val();
            
        });*/

        Date.prototype.ShortForm = function () {
            var yyyy = this.getFullYear().toString();
            var mm = (this.getMonth() + 1).toString(); // getMonth() is zero-based
            var dd = this.getDate().toString();
            var hh = this.getHours().toString();
            var min = this.getMinutes().toString();
            return yyyy + "-" + (mm[1] ? mm : "0" + mm[0]) + "-" + (dd[1] ? dd : "0" + dd[0]) + " "
                + (hh[1] ? hh : "0" + hh[0]) + ":" + (min[1] ? min : "0" + min[0]) + ":00";
        };

        /*$("InfoBaseName").on("input", function () {
            alert("Input changed");
        });/*/


        function SetBlockTime(ElementName, Offset) {

            var TextBox = document.getElementById(ElementName);

            var d = new Date();
            d.setMinutes(d.getMinutes() + Offset);

            TextBox.value = d.ShortForm();
        }

    </script>
</body>
</html>
