<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Master.Master" AutoEventWireup="true" CodeBehind="RequisitionReport.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.RequisitionReport" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- Bootstrap CSS -->
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css" rel="stylesheet" />
    <!-- ReportViewer CSS -->
    <link href="https://ajax.aspnetcdn.com/ajax/reportsrv2010/reportviewer.css" rel="stylesheet" />
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
        <div class="col-6">
        <asp:LinkButton ID="RequisitionSUmmaryREport" runat="server" CssClass="btn btn-dark-blue w-100" OnClick="btnRequisitionSummary_Click">
            <i class="fas fa-file-alt"></i> Requisition Details
        </asp:LinkButton>
    </div>

    <div class="col-6">
        <asp:LinkButton ID="btnRequisitionDetailedReport" runat="server" CssClass="btn btn-dark-blue w-100" OnClick="btnRequisitionDetailed_Click">
            <i class="fas fa-file-alt"></i>All Requisitions  Report
        </asp:LinkButton>
    </div>
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
<div class="row">
    <div class="col-md-12">
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-currency-dollar"></i></span>
            </div>
            <asp:TextBox ID="txtTerm" Visible="true" runat="server" CssClass="form-control" placeholder="Term"></asp:TextBox>
            <asp:TextBox ID="txtRequisitionId" Visible="true" runat="server" CssClass="form-control" placeholder="Term"></asp:TextBox>
        </div>
    </div>

</div>


   

    <!-- Bootstrap JS -->
    <script src="https://code.jquery.com/jquery-3.3.1.slim.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.7/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/js/bootstrap.min.js"></script>
    <!-- ReportViewer JS -->
    <script src="https://ajax.aspnetcdn.com/ajax/reportsrv2010/reportviewer.js"></script>
</asp:Content>
