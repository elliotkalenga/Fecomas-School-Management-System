<%@ Page Title="Change Password" Language="C#" MasterPageFile="~/Views/Admin/AdminMaster.Master"
    AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.ChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Change Password</title>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">

    <style>
        .btn-dark-blue {
            background-color: #001f3f !important; /* Fecomas Tech's Dark Blue */
            border-color: #001f3f !important;
            color: #ffffff !important;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mt-5">
        <h4 class="text-center">Change Password</h4>
        <div class="card p-4">
            <asp:Label ID="lblMessage" runat="server" CssClass="text-danger" EnableViewState="false"></asp:Label>

            <div class="form-group">
                <label>Current Password</label>
                <asp:TextBox ID="txtCurrentPassword" runat="server" CssClass="form-control" TextMode="Password" required></asp:TextBox>
            </div>

            <div class="form-group">
                <label>New Password</label>
                <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control" TextMode="Password" required></asp:TextBox>
            </div>

            <div class="form-group">
                <label>Confirm New Password</label>
                <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password" required></asp:TextBox>
            </div>

            <asp:Button ID="btnChangePassword" runat="server" Text="Change Password" CssClass="btn btn-dark-blue btn-block"
                OnClick="btnChangePassword_Click" />
        </div>
    </div>
</asp:Content>
