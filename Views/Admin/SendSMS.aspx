<%@ Page Language="C#" Async="true" MasterPageFile="~/Views/Admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="SendSMS.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.SendSMS" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
                .container {
    max-width: 100% !important;
}

.card {
    max-width: 100% !important;
}

.table-responsive {
    width: 100% !important;
}

#studentsTable {
    width: 100% !important;
}
        .container {
            max-width: 800px;
            margin: 30px auto;
            padding: 20px;
        }
        .card {
            background-color: #fff;
            border-radius: 8px;
            padding: 20px;
            box-shadow: 0px 4px 6px rgba(0, 0, 0, 0.1);
        }
        input[type="text"], textarea {
            width: 100%;
            padding: 8px;
            margin-bottom: 10px;
            border: 1px solid #ccc;
            border-radius: 5px;
        }
        button, .btn, input[type="submit"] {
            width: 100%;
            padding: 10px;
            background-color: #007bff;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
        }
        .text-success {
            color: green;
        }
        .text-error {
            color: red;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mt-5">
        <div class="card shadow p-4">
            <h3 class="mb-4">Send SMS</h3>

            <div class="form-group">
                <asp:TextBox ID="txtPhoneNumber" runat="server" TextMode="MultiLine" CssClass="form-control" Rows="3" Placeholder="Enter phone numbers (comma, semicolon, or new line separated)"></asp:TextBox>
            </div>

            <div class="form-group">
                <asp:TextBox ID="txtMessage" runat="server" TextMode="MultiLine" CssClass="form-control" Placeholder="Enter your message" Rows="4"></asp:TextBox>
            </div>

            <div class="form-group">
                <asp:Button ID="btnSendSms" runat="server" Text="Send SMS" OnClick="btnSendSms_Click" CssClass="btn btn-primary btn-block" />
            </div>

            <asp:Label ID="lblStatus" runat="server" CssClass="mt-3 d-block text-success"></asp:Label>
        </div>
    </div>

     <!-- 🆕 New Section: Send Custom SMS to All Parents -->
        <div class="card shadow p-4 mt-5">
            <h3 class="mb-4">Send Custom SMS to All Parents</h3>

            <div class="form-group">
                <asp:TextBox ID="txtAllParentsMessage" runat="server" TextMode="MultiLine" CssClass="form-control" Rows="4" Placeholder="Enter your message to all parents"></asp:TextBox>
            </div>

            <div class="form-group">
                <asp:Button ID="btnSendAllParents" runat="server" Text="Send to All Parents" OnClick="btnSendAllParents_Click" CssClass="btn btn-success btn-block" />
            </div>

            <asp:Label ID="lblAllParentsStatus" runat="server" CssClass="mt-3 d-block text-success"></asp:Label>
        </div>
</asp:Content>
