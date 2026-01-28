<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RecoverLogin.aspx.cs" Inherits="SMSWEBAPP.Views.Students.RecoverLogin" %>


<!DOCTYPE html>
<html>
<head runat="server">
    <title>Recover Login Details</title>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" />

<style>

            .btn-dark-blue {
            background-color: #001f3f !important; /* Adjust the hex color code as needed */
            border-color: #001f3f !important; /* Ensure the border color matches */
            color: #ffffff !important; /* Light text color for better visibility */
        }

</style>
</head>
<body>
    <form id="form1" runat="server">
<div class="container mt-5">
    <h2 class="text-center">Recover Login Details</h2>

    <div class="row justify-content-center">
        <div class="col-md-6">
            <label>First Letter of First Name:</label>
            <asp:TextBox ID="txtFirstLetter" runat="server" MaxLength="1" CssClass="form-control" placeholder="E.g., J For John"></asp:TextBox>

            <label class="mt-2">First Letter of Last Name:</label>
            <asp:TextBox ID="txtLastLetter" runat="server" MaxLength="1" CssClass="form-control" placeholder="E.g., B for Banda"></asp:TextBox>

            <label class="mt-2">Phone Number:</label>
            <asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="form-control" placeholder="Enter Your Phone Number"></asp:TextBox>

            <div class="text-center mt-3">
                <asp:Button ID="btnRecover" runat="server" Text="Recover Login Details" CssClass="btn btn-dark-blue btn-block" OnClick="btnRecover_Click" />
            </div>
        </div>
    </div>

    <div class="row mt-4">
        <div class="col-md-8 offset-md-2">
            <asp:GridView ID="gvLoginDetails" runat="server" CssClass="table table-bordered table-striped" AutoGenerateColumns="False" Visible="false">
                <Columns>
                    <asp:BoundField DataField="FullName" HeaderText="Student Name" />
                    <asp:BoundField DataField="UserName" HeaderText="Username" />
                    <asp:BoundField DataField="Password" HeaderText="Password" />
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <asp:Label ID="lblMessage" runat="server" CssClass="text-danger mt-3" Visible="false"></asp:Label>
</div>
    </form>
</body>
</html>
