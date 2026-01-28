<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Master.Master" AutoEventWireup="true" CodeBehind="LessonReport.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.LessonReport" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <!-- Bootstrap CSS -->

    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css" rel="stylesheet" />
    <!-- ReportViewer CSS -->
    <link href="https://ajax.aspnetcdn.com/ajax/reportsrv2010/reportviewer.css" rel="stylesheet" />
    <!-- Select2 CSS -->
    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.3/css/all.min.css" />

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
        .select2 {
            width: 100% !important;
        }
    </style>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div class="container">
        <!-- Message Labels Row -->

        <!-- Row for Buttons and Textboxes -->
<div class="row">
    <div class="col-md-4">
        <label class="form-label">Lesson Plans Report Per Subject Allocation</label>
        <asp:DropDownList ID="ddlSubjects" runat="server" CssClass="form-control select2" AutoPostBack="true" OnSelectedIndexChanged="ddlSubjects_SelectedIndexChanged"></asp:DropDownList>
    </div>

    <div class="col-md-4">
        <label class="form-label">Lesson Plans Report Per Subject-WeekNo</label>
        <asp:DropDownList ID="ddlWeeks" runat="server" CssClass="form-control select2" AutoPostBack="true" OnSelectedIndexChanged="ddlWeeks_SelectedIndexChanged"></asp:DropDownList>
    </div>

    <div class="col-md-4">
        <label class="form-label">Single Lesson Plans Report</label>
        <asp:DropDownList ID="ddlClassPeriod" runat="server" CssClass="form-control select2" AutoPostBack="true" OnSelectedIndexChanged="ddlPeriod_SelectedIndexChanged"></asp:DropDownList>
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

    </div>
            <div class="row">
            <div class="col-md-4">
                <asp:TextBox ID="txtSubject" runat="server" CssClass="form-control mt-2" placeholder="Selected Subject"></asp:TextBox>
            </div>
            <div class="col-md-4">
                <asp:TextBox ID="txtWeek" runat="server" CssClass="form-control mt-2" placeholder="Selected Week"></asp:TextBox>
            </div>
            <div class="col-md-4">
                <asp:TextBox ID="txtClassPeriod" runat="server" CssClass="form-control mt-2" placeholder="Selected Teacher"></asp:TextBox>
            </div>
        </div>

    <!-- Bootstrap JS -->
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.7/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/js/bootstrap.min.js"></script>
    <!-- Select2 JS -->
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
    <!-- ReportViewer JS -->
    <script src="https://ajax.aspnetcdn.com/ajax/reportsrv2010/reportviewer.js"></script>

    <script>
        $(document).ready(function () {
            $('.select2').select2();

            $('#<%= ddlSubjects.ClientID %>').on('select2:select', function (e) {
                showLoadingOverlay();
            });

            $('#<%= ddlWeeks.ClientID %>').on('select2:select', function (e) {
                showLoadingOverlay();
            });

            $('#<%= ddlClassPeriod.ClientID %>').on('select2:select', function (e) {
                showLoadingOverlay();
            });
        });

        function showLoadingOverlay() {
            $('#loading-overlay').show();
        }

        function hideLoadingOverlay() {
            $('#loading-overlay').hide();
        }
    </script>
</asp:Content>