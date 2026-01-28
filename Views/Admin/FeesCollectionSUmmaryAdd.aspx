<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Master.Master" AutoEventWireup="true" CodeBehind="FeesCollectionSUmmaryAdd.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.FeesCollectionSUmmaryAdd" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- Bootstrap CSS -->
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css" rel="stylesheet" />
    <!-- ReportViewer CSS -->
    <link href="https://ajax.aspnetcdn.com/ajax/reportsrv2010/reportviewer.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        /* Custom dark blue button color */ 
        .btn-dark-blue {
            background-color: #001f3f !important;
            border-color: #001f3f !important; 
            color: #ffffff !important;
        }
        .btn-dark-blue:hover { 
            background-color: #001a33 !important;
            border-color: #001a33 !important;
        }
    </style>
    <div class="container">
        <!-- Message Labels Row -->
        <!-- ScriptManager (added back here) -->
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

        <!-- Row for Buttons and Textboxes -->
<div class="row">
        <div class="col-md-3">
        <asp:LinkButton ID="btnSummaryPerCategory" runat="server" CssClass="btn btn-dark-blue btn-dark-blue:hover" OnClick="btnGenerateReport_Click">
            <i class="fas fa-file-alt"></i> Fees Collection Per Category
        </asp:LinkButton>
    </div>

            <div class="col-md-3">
        <asp:LinkButton ID="BtnCollectionSummary" runat="server" CssClass="btn btn-dark-blue btn-dark-blue:hover" OnClick="btnCollectionSummary_Click">
            <i class="fas fa-file-alt"></i>Fees  Collection Summary
        </asp:LinkButton>
    </div>

        <div class="col-md-3">
        <asp:LinkButton ID="BtnAllTransactions" runat="server" CssClass="btn btn-dark-blue btn-dark-blue:hover" OnClick="btnResultsheet_Click">
            <i class="fas fa-file-alt"></i> Fees Collection Transactions
        </asp:LinkButton>
    </div>


            <div class="col-md-3">
        <asp:LinkButton ID="BtnInvoiceStatus" runat="server" CssClass="btn btn-dark-blue btn-dark-blue:hover" OnClick="btnInvoicesStatus_Click">
            <i class="fas fa-file-alt"></i> All Invoices Status
        </asp:LinkButton>
    </div>

</div>

<!-- Row for ReportViewer Controls -->
<div class="row mt-3">
    <!-- Column for ReportViewer taking 75% width -->
    <div class="col-md-12">
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="113%" Height="750px" CssClass="table-responsive" />
    </div>
    <!-- Column for buttons and textboxes taking 25% width -->
    <div class="col-md-3">
        <div class="row mt-3">
            <div class="col-md-12">
                <asp:Label ID="lblMessage" runat="server" ForeColor="Red" CssClass="alert alert-warning d-none" />
            </div>
        </div>
        <div class="row mt-3">
            <div class="col-md-12">
                <asp:Label ID="lblMessage2" runat="server" ForeColor="Red" CssClass="alert alert-warning d-none" />
            </div>
        </div>
        <div class="row mt-3">
            <div class="col-md-12">
                <asp:Label ID="lblMessage3" runat="server" ForeColor="Red" CssClass="alert alert-warning d-none" />
            </div>
        </div>

    </div>
</div>
        <div id="loading-overlay" style="display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; background-color: rgba(0, 0, 0, 0.5); z-index: 1000;">
    <div style="position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%);">
        <i class="fas fa-spinner fa-spin fa-3x"></i> <span style="font-size: 24px; color: #ffffff;">Loading...</span>
    </div>
</div>

<div class="row">
    <div class="col-md-12">
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-currency-dollar"></i></span>
            </div>
            <asp:TextBox ID="txtTerm" Visible="true" runat="server" CssClass="form-control" placeholder="Term"></asp:TextBox>
        </div>
    </div>

</div>


    </div>

    <!-- Bootstrap JS -->
    <script src="https://code.jquery.com/jquery-3.3.1.slim.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.7/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/js/bootstrap.min.js"></script>
    <!-- ReportViewer JS -->
    <script src="https://ajax.aspnetcdn.com/ajax/reportsrv2010/reportviewer.js"></script>


<script type="text/javascript">

        function showLoadingOverlay() {
            $('#loading-overlay').show();
        }

        function hideLoadingOverlay() {
            $('#loading-overlay').hide();
        }

    $(document).ready(function () {

        $('#<%= btnSummaryPerCategory.ClientID %>').on('click', function () {
        showLoadingOverlay();
    });

    $('#<%= BtnAllTransactions.ClientID %>').on('click', function () {
        showLoadingOverlay();
    });

    $('#<%= BtnCollectionSummary.ClientID %>').on('click', function () {
        showLoadingOverlay();
    });

    $('#<%= BtnInvoiceStatus.ClientID %>').on('click', function () {
        showLoadingOverlay();
    });
});

    $(window).on('load', function () {
        hideLoadingOverlay();
    });

</script>
</asp:Content>
