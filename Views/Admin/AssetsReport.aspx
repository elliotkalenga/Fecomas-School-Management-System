<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="AssetsReport.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.AssetsReport" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .container {
    max-width: 95% !important;
}

.card {
    max-width: 95% !important;
}
        .btn-dark-blue {
            background-color: #001f3f !important;
            border-color: #001f3f !important;
            color: #ffffff !important;
        }

        .btn-dark-blue:hover {
            background-color: #001a33 !important;
            border-color: #001a33 !important;
        }
        .btn-dark-blue {
    margin: 5px; /* adjust the value as needed */
}
    </style>
    <link rel="stylesheet" href="https://cdn.datatables.net/1.12.1/css/jquery.dataTables.min.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div class="container mt-5">
        <div class="card">
            <div class="card-header d-flex justify-content-between align-items-center p-2">
                <div class="d-flex flex-wrap align-items-center gap-2">
                    <asp:LinkButton ID="btnAssetInventory" runat="server" CssClass="btn btn-dark-blue" OnClick="btnAssetInventory_Click">
                        <i class="fas fa-file-alt"></i> Assets Inventory Report
                    </asp:LinkButton>
                    <asp:LinkButton ID="BtnAssetAllocation" runat="server" CssClass="btn btn-dark-blue" OnClick="btnAssetAllocation_Click">
                        <i class="fas fa-file-alt"></i> Assets Allocation Report
                    </asp:LinkButton>
                    <asp:LinkButton ID="BtnAllocationHistory" runat="server" CssClass="btn btn-dark-blue" OnClick="BtnAllocationHistory_Click">
                        <i class="fas fa-file-alt"></i> Assets Movement History Report
                    </asp:LinkButton>
                </div>
            </div>
            <div class="card-body">
                <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="100%" Height="750px" />
            </div>
        </div>
    </div>

    <script src="https://code.jquery.com/jquery-3.3.1.slim.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.7/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/js/bootstrap.min.js"></script>
    <script src="https://ajax.aspnetcdn.com/ajax/reportsrv2010/reportviewer.js"></script>
</asp:Content>