<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Changepassword.aspx.cs" Inherits="SMSWEBAPP.Views.Students.Changepassword" %>


<!DOCTYPE html>
<html lang="en">
<head>
    <style>
                    .btn-dark-blue {
            background-color: #001f3f !important; /* Adjust the hex color code as needed */
            border-color: #001f3f !important; /* Ensure the border color matches */
            color: #ffffff !important; /* Light text color for better visibility */
        }

    </style>
    
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Change Password</title>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">
</head>
<body>
    <form id="form1" runat="server">
<div class="container mt-4">
    <h4 class="text-center">Change Password</h4>
    <div class="card p-4">
        <asp:Label ID="lblMessage" runat="server" CssClass="text-danger" EnableViewState="false"></asp:Label>

        <div class="form-group">
            <label>Current Password</label>
            <asp:TextBox ID="txtCurrentPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
        </div>

        <div class="form-group">
            <label>New Password</label>
            <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
        </div>

        <div class="form-group">
            <label>Confirm New Password</label>
            <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
        </div>

        <asp:Button ID="btnChangePassword" runat="server" Text="Change Password" CssClass="btn btn-dark-blue btn-block"
            OnClick="btnChangePassword_Click" />
    </div>
</div>
</form>
</body>
</html>
