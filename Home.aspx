<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="SMSWEBAPP.Views.Home" %>

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
        }
        .navbar-nav .nav-link {
            color: #ffffff !important; /* Light color for better visibility */
        }
        .navbar-nav .nav-link:hover {
            color: #ffeb3b !important; /* Yellow color on hover for more emphasis */
        }
        #Mrow {
            background-image: url(../Assets/Images/Stdbg.jpg);
            background-size: cover;
            height: 100vh;
            position: relative;
        }
        /* Custom dark blue background color */
        .bg-dark-blue {
            background-color: #001f3f !important; /* Adjust the hex color code as needed */
        }
        .centered-text {
            background-color: rgba(0, 0, 0, 0.5);
            color: white;
            font-size: 2rem;
            font-weight: bold;
            position: absolute;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            text-align: center;
            padding: 20px;
            border-radius: 10px;
        }
        @media (max-width: 768px) {
            #Mrow {
                height: auto;
                padding: 20px;
            }
            .centered-text {
                font-size: 1.5rem;
                padding: 15px;
            }
        }
        @media (max-width: 480px) {
            #Mrow {
                padding: 10px;
            }
            .centered-text {
                font-size: 1rem;
                padding: 10px;
            }
        }
    </style>
</head>
<body>
    <!-- Navigation Bar -->
    <nav class="navbar navbar-expand-lg navbar-dark bg-dark-blue">
        <a class="navbar-brand" href="Index.aspx"><i class="fas fa-home"></i> Fecomas SIS</a>
        <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
            <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="navbarNav">
            <ul class="navbar-nav ml-auto">
                <li class="nav-item">
                    <a class="nav-link" href="Views/Students/Login.aspx"><i class="fas fa-user"></i> Student Login</a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" href="Views/Admin/UserLogin.aspx"><i class="fas fa-user-cog"></i> User Login</a>
                </li>
            </ul>
        </div>
    </nav>

    <div class="container-fluid">
        <div class="row" id="Mrow">
            <div class="col">
                <div class="centered-text">
                    Fecomas School Information System
                </div>
            </div>
        </div>
    </div>

    <!-- Bootstrap JS and dependencies -->
    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.3/dist/umd/popper.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
</body>
</html>
