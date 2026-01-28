<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Master.Master" AutoEventWireup="true" CodeBehind="AttendanceReportAdd.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.AttendanceReportAdd" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- Bootstrap CSS -->
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css" rel="stylesheet" />
    <!-- jQuery UI CSS (For DatePicker) -->
    <link rel="stylesheet" href="https://code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css" />
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
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

        <!-- Row for Buttons -->
        <div class="row">
            <div class="col-md-6">
                <asp:LinkButton ID="btnGenerateAttendanceReport" runat="server" CssClass="btn btn-dark-blue w-100" OnClick="btnGenerateAttendanceReport_Click">
                    <i class="fas fa-file-alt"></i> Generate Attendance Report
                </asp:LinkButton>
            </div>
        </div>

        <!-- Filters Row -->
        <div class="row mt-3">
            <div class="form-group col-md-4">
                <label for="txtDob">Start</label>
                <div class="input-group">
                    <div class="input-group-prepend">
                        <span class="input-group-text"><i class="bi bi-calendar"></i></span>
                    </div>
                    <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" placeholder="Enter Date of Birth" TextMode="Date" required></asp:TextBox>
                </div>
            </div>

            <div class="form-group col-md-4">
                <label for="txtDob">End Date</label>
                <div class="input-group">
                    <div class="input-group-prepend">
                        <span class="input-group-text"><i class="bi bi-calendar"></i></span>
                    </div>
                    <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control" placeholder="Enter Date of Birth" TextMode="Date" required></asp:TextBox>
                </div>
            </div>
            <div class="col-md-4">
                <label>Term:</label>
                <asp:TextBox ID="txtTerm" runat="server" CssClass="form-control" placeholder="Enter Term"></asp:TextBox>
            </div>
        </div>

        <!-- ReportViewer -->
        <div class="row mt-3">
            <div class="col-md-12">
                <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="100%" Height="750px" CssClass="table-responsive" />
            </div>
        </div>

        <!-- Error Messages -->
        <div class="row mt-3">
            <div class="col-md-12">
                <asp:Label ID="lblMessage" runat="server" ForeColor="Red" CssClass="alert alert-warning d-none" />
            </div>
        </div>
    </div>

    <!-- Required Scripts -->
    <script src="https://code.jquery.com/jquery-3.3.1.min.js"></script> 
    <script src="https://code.jquery.com/ui/1.12.1/jquery-ui.min.js"></script> <!-- jQuery UI for DatePicker -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.7/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/js/bootstrap.min.js"></script>
    <!-- ReportViewer JS -->
    <script src="https://ajax.aspnetcdn.com/ajax/reportsrv2010/reportviewer.js"></script>

    <script>
        $(document).ready(function () {
            $(".datepicker").datepicker({
                dateFormat: "yy-mm-dd",
                changeMonth: true,
                changeYear: true
            });
        });
    </script>
</asp:Content>
