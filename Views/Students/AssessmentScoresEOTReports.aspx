<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Master.Master" AutoEventWireup="true" CodeBehind="AssessmentScoresEOTReports.aspx.cs" Inherits="SMSWEBAPP.Views.Students.AssessmentScoresEOTReports" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <!-- Bootstrap CSS -->
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css" rel="stylesheet" />
    <!-- ReportViewer CSS -->
    <link href="https://ajax.aspnetcdn.com/ajax/reportsrv2010/reportviewer.css" rel="stylesheet" />
    <!-- Select2 CSS -->
    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />
    <!-- Font Awesome -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.3/css/all.min.css" />

    <style>
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
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <div class="container">
        <!-- Dropdowns Row -->
        <div class="row">
            <div class="col-md-4">
                <label class="form-label">End of Term School Report JCE</label>
                <asp:DropDownList ID="ddlAssessment" runat="server" CssClass="form-control select2" AutoPostBack="true" OnSelectedIndexChanged="ddlAssessment_SelectedIndexChanged"></asp:DropDownList>
            </div>
            <div class="col-md-4">
                <label class="form-label">End of Term School Report MSCE</label>
                <asp:DropDownList ID="ddlAssessmentStudent" runat="server" CssClass="form-control select2" AutoPostBack="true" OnSelectedIndexChanged="ddlAssessmentStudent_SelectedIndexChanged"></asp:DropDownList>
            </div>
            <div class="col-md-4">
                <label class="form-label">End of Term School Report PRIMARY</label>
                <asp:DropDownList ID="ddlAggregate" runat="server" CssClass="form-control select2" AutoPostBack="true" OnSelectedIndexChanged="ddlAggregate_SelectedIndexChanged"></asp:DropDownList>
            </div>
        </div>


        <!-- ReportViewer -->
        <div class="row mt-3">
            <div class="col-md-12">
                <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="100%" Height="750px" CssClass="table-responsive" />
            </div>
        </div>

        <!-- Labels for Messages -->
        <div class="row mt-3">
            <div class="col-md-4">
                <asp:Label ID="lblMessage" runat="server" ForeColor="Red" CssClass="alert alert-warning d-none" />
            </div>
            <div class="col-md-4">
                <asp:Label ID="lblMessage2" runat="server" ForeColor="Red" CssClass="alert alert-warning d-none" />
            </div>
            <div class="col-md-4">
                <asp:Label ID="lblMessage3" runat="server" ForeColor="Red" CssClass="alert alert-warning d-none" />
            </div>
        </div>

        <!-- Student Information Inputs -->
        <div class="row mt-3">
            <div class="col-md-3">
                <asp:TextBox ID="txtStudent" runat="server" CssClass="form-control" placeholder="Student Name"></asp:TextBox>
            </div>
            <div class="col-md-3">
                <asp:TextBox ID="txtStudentName" runat="server" CssClass="form-control" placeholder="Student Full Name"></asp:TextBox>
            </div>
            <div class="col-md-3">
                <asp:TextBox ID="txtSchool" runat="server" CssClass="form-control" placeholder="School Name"></asp:TextBox>
            </div>
            <div class="col-md-3">
                <asp:TextBox ID="txtClass" runat="server" CssClass="form-control" placeholder="Class"></asp:TextBox>
            </div>
            <div class="col-md-3 mt-2">
                <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" placeholder="Phone Number"></asp:TextBox>
            </div>
            <div class="col-md-3 mt-2">
                <asp:TextBox ID="txtAssessment" runat="server" CssClass="form-control" placeholder="Assessment"></asp:TextBox>
            </div>
            <div class="col-md-3 mt-2">
                <asp:TextBox ID="txtAggregate" runat="server" CssClass="form-control" placeholder="Aggregate"></asp:TextBox>
            </div>
            <div class="col-md-3 mt-2">
                <asp:TextBox ID="txtAssessmentStudent" runat="server" CssClass="form-control" placeholder="Assessment Student"></asp:TextBox>
            </div>
        </div>

        <!-- Loading Overlay -->
        <div id="loading-overlay" style="display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; background-color: rgba(0, 0, 0, 0.5); z-index: 1000;">
            <div style="position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%);">
                <i class="fas fa-spinner fa-spin fa-3x"></i> <span style="font-size: 24px; color: #ffffff;">Loading...</span>
            </div>
        </div>
    </div>

    <!-- Scripts -->
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.7/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/js/bootstrap.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
    <script src="https://ajax.aspnetcdn.com/ajax/reportsrv2010/reportviewer.js"></script>

    <script>
        $(document).ready(function () {
            $('.select2').select2();

            $('#<%= ddlAssessment.ClientID %>, #<%= ddlAssessmentStudent.ClientID %>, #<%= ddlAggregate.ClientID %>').on('select2:select', function () {
                showLoadingOverlay();
            });
        });

        function showLoadingOverlay() {
            $('#loading-overlay').show();
        }

        function hideLoadingOverlay() {
            $('#loading-overlay').hide();
        }

        function sendWhatsAppMessage(type) {
            var pdfUrl = '<%= Session["ReportPDF"] ?? "" %>';
            if (!pdfUrl || pdfUrl.trim() === "") {
                alert("Please generate the report first.");
                return;
            }

            var phoneNumber = document.getElementById('<%= txtPhone.ClientID %>').value.trim();
            var student = document.getElementById('<%= txtStudent.ClientID %>').value.trim();
            var schoolName = document.getElementById('<%= txtSchool.ClientID %>').value.trim();
            var className = document.getElementById('<%= txtClass.ClientID %>').value.trim();
            var assessment = document.getElementById('<%= txtAssessment.ClientID %>').value.trim();

            if (!phoneNumber) {
                alert("Please provide a valid phone number.");
                return;
            }

            var message = "";
            if (type === "share") {
                message = `*Notification of ${assessment} RESULTS*\n\n` +
                    `*${schoolName}* would like to inform you that *${assessment}* Results have been released for *${student}* - ${className}.\n\n` +
                    `*Click the link below to download the School Report:*\n${pdfUrl}`;
            } else {
                alert("Invalid message type.");
                return;
            }

            var whatsappUrl = `https://wa.me/${phoneNumber}?text=${encodeURIComponent(message)}`;
            window.open(whatsappUrl, "_blank");
        }
    </script>
</asp:Content>
