<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SMSWEBAPP.CMS.Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Church Management System - Login</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.3/css/all.min.css" />

    <style>
        body, html {
            height: 100%;
            margin: 0;
            font-family: 'Segoe UI', sans-serif;
            background: linear-gradient(to right, rgba(0, 31, 63, 0.8), rgba(0, 51, 102, 0.8)), 
                        url('ftslogo.png') no-repeat center center fixed;
            background-size: cover;
            display: flex;
            justify-content: center;
            align-items: center;
        }

        .login-wrapper {
            max-width: 420px;
            width: 100%;
            padding: 30px;
            background: rgba(255, 255, 255, 0.15);
            backdrop-filter: blur(10px);
            border-radius: 20px;
            box-shadow: 0 10px 25px rgba(0, 0, 0, 0.3);
            text-align: center;
        }

        .login-wrapper .logo {
            font-size: 80px;
            color: white;
        }

        .login-wrapper h4 {
            color: #ffa500;
            font-weight: bold;
            margin-top: 15px;
        }

        .slogan {
            color: #f0f0f0;
            font-style: italic;
            margin-bottom: 25px;
        }

        .form-group .input-group-text {
            background-color: #001f3f;
            color: white;
        }

        .form-control {
            border-radius: 0.5rem !important;
        }

        .btn-dark-blue {
            background-color: #001f3f !important;
            border: none;
            color: white;
            font-weight: bold;
            transition: 0.3s;
        }

        .btn-dark-blue:hover {
            background-color: #004080 !important;
        }

        .center-links {
            margin-top: 20px;
            color: #fff;
            font-size: 0.95em;
        }

        .center-links a {
            color: #fff !important;
            text-decoration: none;
        }

        .center-links a:hover {
            color: #ffeb3b !important;
        }

        .modal-content iframe {
            border-radius: 10px;
        }

        .main-icon {
            font-size: 100px;
            color: white;
            background-color: rgba(255, 255, 255, 0.2);
            border-radius: 50%;
            padding: 25px;
            margin-bottom: 20px;
        }

        @media (max-width: 480px) {
            .login-wrapper {
                margin: 10px;
                padding: 20px;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-wrapper">
            <!-- Church-themed icon inside a circle -->
            <i class="fas fa-church main-icon"></i>

            <h4>Church Management System</h4>
            <div class="slogan">Empowering Ministries with Smart Tools</div>

            <asp:Label ID="lblError" runat="server" ForeColor="#ff4c4c" Text="" Visible="False" Font-Size="Medium"></asp:Label>

            <div class="form-group">
                <div class="input-group">
                    <div class="input-group-prepend">
                        <span class="input-group-text"><i class="fas fa-user"></i></span>
                    </div>
                    <asp:TextBox runat="server" type="text" class="form-control" ID="TxtUsername" placeholder="Enter username" required autocomplete="off" />
                </div>
            </div>

            <div class="form-group">
                <div class="input-group">
                    <div class="input-group-prepend">
                        <span class="input-group-text"><i class="fas fa-lock"></i></span>
                    </div>
                    <asp:TextBox runat="server" ID="TxtPassword" type="password" class="form-control" placeholder="Enter password" required autocomplete="off"/>
                </div>
            </div>

            <asp:Button runat="server" ID="BtnLogin" type="submit" class="btn btn-dark-blue btn-block" Text="Login" OnClick="BtnLogin_Click" />

            <div class="center-links mt-3">
                <a href="#" data-toggle="modal" data-target="#recoverModal"><i class="fas fa-key"></i> Forgot Password</a>
            </div>

            <div class="center-links mt-4">
                <a href="https://fecomas.com">&copy; Fecomas Tech Solutions <span id="current-year"></span></a>
                <br />
                <a href="https://wa.me/265993189671" target="_blank">
                    <i class="fab fa-whatsapp" style="font-size: 20px; color: #25D366;"></i> WhatsApp Us for Quick Support
                </a>
            </div>
        </div>
    </form>

    <script>
        document.getElementById("current-year").innerText = new Date().getFullYear();
    </script>

    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.3/dist/umd/popper.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
</body>
</html>
