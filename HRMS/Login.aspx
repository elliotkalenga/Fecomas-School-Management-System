<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SMSWEBAPP.HRMS.Login" %>



<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Fecomas School Information System</title>
    <link href="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.3/css/all.min.css">
    <style>
        body, html {
            height: 100%;
            margin: 0;
            background-color: #001f3f !important; /* Dark blue background */
            font-size: 100%; /* Default font size */
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            text-align: center;
        }
        #Mrow {
            width: 100%;
            display: flex;
            justify-content: center;
            align-items: center;
            flex-direction: column; /* Stack items vertically */
        }
        .login-form {
            background: rgba(255, 255, 255, 0.8);
            padding: 30px;
            border-radius: 10px;
            box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);
            width: 100%;
            max-width: 400px;
            position: relative;
            z-index: 1; /* Ensure the form is above the icon */
        }
        .btn-dark-blue {
            background-color: #001f3f !important; /* Adjust the hex color code as needed */
            border-color: #001f3f !important; /* Ensure the border color matches */
            color: #ffffff !important; /* Light text color for better visibility */
        }
        .btn-dark-blue:hover {
            background-color: #001a33 !important; /* Darker blue for hover effect */
            border-color: #001a33 !important; /* Ensure the border color matches */
        }
        .bg-dark-blue {
            background-color: #001f3f !important; /* Adjust the hex color code as needed */
        }
        @media (max-width: 768px) {
            #Mrow {
                height: auto;
                padding: 20px;
            }
            .login-form {
                width: 90%;
                margin-top: 0;
                font-size: 1.2em; /* Increase relative font size */
            }
            h2 {
                font-size: 1.5em; /* Increase relative heading size */
            }
            label {
                font-size: 1em; /* Increase relative label size */
            }
            .btn {
                font-size: 1.1em; /* Increase relative button size */
            }
        }
        @media (max-width: 480px) {
            #Mrow {
                padding: 10px;
            }
            .login-form {
                width: 90%;
                margin-top: 0;
            }
        }
        .center-links {
            display: flex;
            justify-content: center;
            align-items: center;
            gap: 20px; /* Adjust the gap between links */
        }
        .center-links .nav-link {
            color: #ffffff !important; /* Change link color to white */
            font-weight: bold; /* Make the links bold */
            text-decoration: none; /* Remove underline */
        }
        .center-links .nav-link:hover {
            color: #ffeb3b !important; /* Yellow color on hover */
        }
        .graduation-icon {
            font-size: 120px; /* Very large size for the icon */
            color: white; /* White color for the icon */
            margin-bottom: 20px;
        }
        .card-title {
            color: rgb(186, 94, 13); /* Change the title color */
        }
        .slogan {
            color: #ffffff;
            font-style: italic;
            margin-top: -10px; /* Adjust margin to suit your design */
        }
    </style>
</head>
<body>
<i class="fas fa-handshake graduation-icon"></i>
<h4 class="card-title">FECOMAS HUMAN RESOURCE MANAGEMENT SYSTEM</h4>
<div class="slogan">Work Smarter, Efficiency Assured</div>
    <div class="container-fluid">
        <div class="row" id="Mrow">
            <div class="col">
                <div class="login-form mx-auto">
                    <h2 class="text-center"><i class="fas fa-sign-in-alt"></i> User Login</h2>
                    <form id="form1" runat="server">
                        <div>
                            <asp:Label ID="lblError" runat="server" ForeColor="#CC3300" Text="Label" Visible="False" Font-Size="Medium"></asp:Label>
                        </div>
                        <div class="form-group">
                            <label for="username"><i class="fas fa-user"></i> Username</label>
                            <asp:TextBox runat="server" type="text" class="form-control" id="TxtUsername" placeholder="Enter username" required autocomplete="off" />
                        </div>
                        <div class="form-group">
                            <label for="password"><i class="fas fa-lock"></i> Password</label>
                            <asp:TextBox runat="server" ID="TxtPassword" type="password" class="form-control" placeholder="Enter password" required autocomplete="off"/>
                        </div>
                        <asp:Button runat="server" ID="BtnLogin" type="submit" class="btn btn-dark-blue btn-dark-blue:hover btn-block" Text="Login" OnClick="BtnLogin_Click"/>
                    </form>
                </div>
                <div class="center-links mt-3">
                </div>
            </div>
        </div>
    </div>
    <!-- Coming Soon Modal -->
<div id="comingSoonModal" class="modal fade" tabindex="-1" role="dialog">
    <div class="modal-dialog modal-dialog-centered" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Coming Soon!</h5>
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body">
                <p>Thank you for your interest! This system is launching soon. Please visit our website regularly for the latest updates and announcements.</p>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-primary" data-dismiss="modal">OK</button>
            </div>
        </div>
    </div>
</div>

<footer class="bg-dark-blue text-white py-3 mt-auto text-center">
    <div class="container">
        <div class="row">
            <div class="col-3.5">
                &copy; Fecomas Tech Solutions. All rights reserved  |
            </div>
            <div class="col-8.5 text-left">
                <i class="fas fa-envelope"></i> Email: info@fecomastechsolutions.com 
                <i class="fas fa-phone"></i> Tel: +265 993 189 671 
                <i class="fas fa-globe"></i> Website: www.fecomas.com
            </div>
        </div>
    </div>
</footer>

    <!-- Bootstrap JS and dependencies -->
    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.3/dist/umd/popper.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
</body>
</html>
