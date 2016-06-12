<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="POAMA.Portal.Main" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server"><meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <title>POAMA forecast</title>
    <link href="http://fonts.googleapis.com/css?family=Open+Sans:400italic,600italic,700italic,400,600,700" rel="stylesheet" type="text/css" />
    <meta name="viewport" content="width=device-width,initial-scale=1,maximum-scale=10.0,minimum-scale=0.25,user-scalable=yes" />
    <style>
        h1, p, input {font-family: 'Open Sans', arial;}
        body {margin: 0px;
          background-color:rgba(77,65,69,.2);
          line-height: 1.5;
          font-family: 'Open Sans', arial;
        }  

        img {
            height: auto;
        }          
        header {
          background-image: url('Background_Birchip.jpg');
          background-repeat: no-repeat;
          background-position: center center;
          background-size: cover;
          margin: 0;
          padding:0px;
          padding-top:60px;
          padding-bottom:60px;
          text-align:center;
          color:rgb(255,255,255);
          margin: 0px auto;
        }
        p {margin: 20px;
        }

        label {
            display: inline-block;
            width:400px;
            text-align: left;
        }

        .header__container {
            display: block;
            align-items: center;
            justify-content: center;
            margin: 0px auto;
            padding: 0px 30px;
   
    
    
        }

        @media (min-width:800px) {
            .header__container {
                display: flex;
            }
        }


        .header__container  h1 { 
            margin: 0px 30px 0px 24px;
        }

        @media (min-width:800px) {
            .header__container  h1 {
                margin: 0px 24px;
                padding: 0px;
            }
        }

        input[type="submit"] {
            margin-bottom: 52px;
        }

        footer {
            display: block;
            color: #ffffff;
            font-size: 12px;
            padding: 12px 0px 12px 0px;
            position: static;
            bottom: 0;
            width: 100%;
            background-color:rgb(41,42,47);
        }



        @media (min-width:550px) {
            footer {
                position: fixed;
            }
        }

        @media (min-width:940px) {
            footer {
                display: table;
            }
        }

        footer ul {
            display: table-cell;
            vertical-align: middle;
            padding: 0px 0px 0px 12px;
            margin: 0px;
        }

        footer ul li {
            display: inline-block;
            margin-right: 6px;
        }

        footer ul li a {
            color: #ffffff;
            font-size: 12px;
        }

        footer ul li a:hover, footer ul li a:focus {
            text-decoration: none;
        }

        .footer__logos {
            float: none;
    
            vertical-align: middle;
        }

        @media (min-width:940px) {
            .footer__logos {
                float: right;
                display: table-cell;
            }
        }



        footer img {
            padding-left: 30px;
            padding-top: 15px;
            vertical-align: middle;
        }

        @media (min-width:940px) {
            footer img {
               padding-right: 30px;
               padding-top: 0px;
               padding-left: 0px;
            }
        }


    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <header><h1>Calibrated POAMA seasonal climate forecasts</h1></header>
        <p>
            This site generates POAMA seasonal climate forecasts downscaled and calibrated to 
            an individual SILO station. An ensemble of 33 different forecasts is provided, 
            with daily data extending out 9 months from the start date. The user can request 
            just rainfall data, which takes a few seconds, or radn, maxt, mint and rainfall data, 
            which takes 4 times as long. The output is produced in a format suitable for input 
            into many agricultural programs.
        </p>
        <p>
            <asp:Label ID="stationNumberLabel"
                       runat="server">
                Station number:
            </asp:Label> 

            <asp:TextBox ID="stationNumber" 
                         runat="server" 
                         AutoPostBack="True">
            </asp:TextBox>
            <asp:RequiredFieldValidator ID="stationNumberValidator" runat="server" ControlToValidate="stationNumber" ErrorMessage="Please enter a station number." ForeColor="Red" SetFocusOnError="True"></asp:RequiredFieldValidator>
        </p>
        <p>
            <asp:Label ID="label2"
                       runat="server">
                Start date (yyyy/mm/dd):
            </asp:Label> 
            <asp:TextBox ID="date" 
                         runat="server" 
                         AutoPostBack="True">
            </asp:TextBox>
            <asp:CustomValidator ID="dateValidator" runat="server" ControlToValidate="date" ErrorMessage="Invalid date" ForeColor="Red" OnServerValidate="OnDateValidation" SetFocusOnError="True" ValidateEmptyText="True"></asp:CustomValidator>
        </p>
        <p>
            <asp:RadioButton ID="rainOnly" 
                          runat="server" 
                          Text="rainfall forecast only"
                          CssClass="radiobutton"
                          AutoPostBack="True" GroupName="a" Checked="True">
            </asp:RadioButton>
        </p>
        <p>
            <asp:RadioButton ID="allData" 
                          runat="server" 
                          Text="radn, maxt, mint & rainfall forecast"
                          CssClass="radiobutton"
                          AutoPostBack="True" GroupName="a">
            </asp:RadioButton>
        </p>
        <p>
            <asp:Label ID="passwordLabel"
                       runat="server">
                Enter password:
            </asp:Label> 
            <asp:TextBox ID="password" 
                         runat ="server" TextMode="Password" />
            <asp:CustomValidator ID="passwordValidator" runat="server" ControlToValidate="password" ErrorMessage="Invalid password" ForeColor="Red" OnServerValidate="OnPasswordValidation" SetFocusOnError="True" ValidateEmptyText="True"></asp:CustomValidator>
        </p>
        <p>
            <asp:Label ID="Label1"
                       runat="server">
                By downloading this data the user agrees that it will be used for research and development 
                purposes only, that the data will not be used for any commercial purpose, and that the 
                data will not be passed onto any third party.
            </asp:Label> 
        </p>
        <p>
            <asp:Button ID="generateButton" 
                        runat ="server" 
                        onclick="OnGenerateClick" 
                        style="font-size:16pt;"
                        Text="Agree and generate data" />
        </p>
    </div>
    </form>

    <footer>

        <ul><li>&copy; CSIRO and GRDC 2016</li><li><a href="http://www.csiro.au/en/About/Footer/Legal-notice"> Legal notice and disclaimer</a></li><li><a href="http://www.csiro.au/en/About/Access-to-information/Privacy"> Your privacy</a></li><li><a href="http://www.csiro.au/en/About/Footer/Accessibility"> Accessibility</a></li><li><a href="http://www.csiro.au/en/Contact"> Contact us</a></li></ul>
        <div class="footer__logos">
            <img src="grdc--gov_inverted.png" alt="Grains research and development corporation" width="220px" />
            <img src="csiro-logo--white.png" alt="CSIRO logo" width="110px" />
        </div>
    </footer>
</body>
</html>
