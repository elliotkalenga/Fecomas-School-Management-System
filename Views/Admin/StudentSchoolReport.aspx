<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Master.Master" AutoEventWireup="true" CodeBehind="StudentSchoolReport.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.StudentSchoolReport" %>
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
        .auto-style1 {
            position: relative;
            width: 100%;
            -ms-flex: 0 0 33.333333%;
            flex: 0 0 33.333333%;
            max-width: 33.333333%;
            left: -1px;
            top: 0px;
            padding-left: 15px;
            padding-right: 15px;
        }
    </style>
    <div class="container">
        <!-- Message Labels Row -->
        <!-- ScriptManager (added back here) -->
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

        <!-- Row for Buttons and Textboxes -->
<div class="row">
        
                <div class="col-md-3">
        <asp:LinkButton ID="BtnContinuous" runat="server" CssClass="btn btn-dark-blue btn-dark-blue:hover" OnClick="btnContinuos_Click">
            <i class="fas fa-file-alt"></i>School Reports
        </asp:LinkButton>
    </div>

               <div class="col-md-3">
        <asp:LinkButton ID="BtnContAssessment" runat="server" CssClass="btn btn-dark-blue btn-dark-blue:hover" OnClick="BtnContAssessment_Click">
            <i class="fas fa-file-alt"></i>  Cont Assessments
        </asp:LinkButton>
    </div>

    <div class="col-md-3">
        <asp:LinkButton ID="btnGenerateReport" runat="server" CssClass="btn btn-dark-blue btn-dark-blue:hover" OnClick="btnGenerateReport_Click">
            <i class="fas fa-file-alt"></i> Academic Transcript
        </asp:LinkButton>
    </div>
    <div class="col-md-3">
        <asp:LinkButton ID="BtnResultSheet" runat="server" CssClass="btn btn-dark-blue btn-dark-blue:hover" OnClick="btnResultsheet_Click">
            <i class="fas fa-file-alt"></i>  Exam Results Sheet
        </asp:LinkButton>
    </div>


</div>


<asp:Panel ID="pnlWhatsAppButtons" runat="server" Visible="false" CssClass="row">
    <div class="col-md-3">
        <asp:LinkButton ID="btnShare" runat="server" CssClass="btn btn-success w-100" 
            OnClientClick="sendWhatsAppMessage('share'); return false;">
            <i class="fab fa-whatsapp"></i> Send 
        </asp:LinkButton>
    </div>

  <div class="col-md-3">
        <asp:LinkButton ID="LinkButton1" runat="server" CssClass="btn btn-success w-100" 
            OnClientClick="sendWhatsAppMessage('ContAssessment'); return false;">
            <i class="fab fa-whatsapp"></i> Send 
        </asp:LinkButton>
    </div>

    <div class="col-md-3">
        <asp:LinkButton ID="LinkButton2" runat="server" CssClass="btn btn-success w-100" 
            OnClientClick="sendWhatsAppMessage('transcript'); return false;">
            <i class="fab fa-whatsapp"></i> Send 
        </asp:LinkButton>
    </div>

    <div class="col-md-3">
        <asp:LinkButton ID="LinkButton3" runat="server" CssClass="btn btn-success w-100" 
            OnClientClick="sendWhatsAppMessage('examSheet'); return false;">
            <i class="fab fa-whatsapp"></i> Send 
        </asp:LinkButton>
    </div>
</asp:Panel>

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
</div><div id="loading-overlay" style="display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; background-color: rgba(0, 0, 0, 0.5); z-index: 1000;">
    <div style="position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%);">
        <i class="fas fa-spinner fa-spin fa-3x"></i> <span style="font-size: 24px; color: #ffffff;">Loading...</span>
    </div>
</div>

<div class="row">
    <div class="col-md-3">
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-currency-dollar"></i></span>
            </div>
            <asp:TextBox ID="txtStudentNo" Visible="true" runat="server" CssClass="form-control" placeholder="Enter Student No"></asp:TextBox>
        </div>
    </div>

    <div class="col-md-3">
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-currency-dollar"></i></span>
            </div>
            <asp:TextBox ID="txtExam" Visible="true" runat="server" CssClass="form-control" placeholder="Enter Exam Details"></asp:TextBox>
        </div>
    </div>

    <div class="col-md-2">
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-currency-dollar"></i></span>
            </div>
            <asp:TextBox ID="TxtExamId" Visible="true" runat="server" CssClass="form-control" placeholder="ExamId"></asp:TextBox>
        </div>
    </div>
        <div class="col-md-2">
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-currency-dollar"></i></span>
            </div>
            <asp:TextBox ID="txtExamName" Visible="true" runat="server" CssClass="form-control" placeholder="ExamName"></asp:TextBox>
        </div>
    </div>



    <div class="col-md-2">
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-currency-dollar"></i></span>
            </div>
            <asp:TextBox ID="txtClassId" Visible="true" runat="server" CssClass="form-control" placeholder="ClassId"></asp:TextBox>
        </div>
    </div>

    <div class="col-md-2">
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-currency-dollar"></i></span>
            </div>
            <asp:TextBox ID="txtTermId" Visible="true" runat="server" CssClass="form-control" placeholder="TermId"></asp:TextBox>
        </div>
    </div>
</div>

                                    <div class="form-row">
                                        <div class="form-group col-md-4">

                <div class="input-group">
                    <div class="input-group-prepend">
                        <span class="input-group-text"><i class="bi bi-currency-dollar"></i></span>
                    </div>
                    <asp:TextBox ID="txtClass" Visible="true" runat="server" CssClass="form-control" placeholder="Class" ></asp:TextBox>
                </div>
            </div>
        <div class="form-group col-md-4">
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="bi bi-person"></i></span>
                </div>
                <asp:TextBox ID="txtStudentName" runat="server" CssClass="form-control" placeholder="Enter First Name" AutoComplete="off" required></asp:TextBox>
            </div>
        </div>

                            <div class="form-group col-md-4">
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="bi bi-person"></i></span>
                </div>
                <asp:TextBox ID="txtBalance" runat="server" CssClass="form-control" placeholder="Balance" AutoComplete="off" required></asp:TextBox>
            </div>
        </div>

    
                            <div class="form-group col-md-4">
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="bi bi-person"></i></span>
                </div>
                <asp:TextBox ID="txtTerm" runat="server" CssClass="form-control" placeholder="Term" AutoComplete="off" required></asp:TextBox>
            </div>
        </div>


                            <div class="form-group col-md-4">
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="bi bi-person"></i></span>
                </div>
                <asp:TextBox ID="txtSchoolName" runat="server" CssClass="form-control" placeholder="SchoolName" AutoComplete="off" required></asp:TextBox>
            </div>
        </div>

    <div class="form-group col-md-4">
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-telephone"></i></span>
            </div>
            <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" placeholder="Enter Phone" AutoComplete="off" required></asp:TextBox>
        </div>
    </div>



    </div>

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-icons/1.4.0/font/bootstrap-icons.min.css">
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
        <link rel="stylesheet" href="https://cdn.datatables.net/1.12.1/css/jquery.dataTables.min.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css">
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script src="https://cdn.datatables.net/1.12.1/js/jquery.dataTables.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jspdf/2.4.0/jspdf.umd.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jspdf-autotable/3.5.23/jspdf.plugin.autotable.min.js"></script>

<script type="text/javascript">
    function sendWhatsAppMessage(type) {
        var pdfUrl = '<%= Session["ReportPDF"] ?? "" %>'; // Ensure it's not null
        if (!pdfUrl || pdfUrl.trim() === "") {
            alert("Please generate the report first.");
            return;
        }

        var phoneNumber = document.getElementById('<%= txtPhone.ClientID %>').value.trim();
        var student = document.getElementById('<%= txtStudentName.ClientID %>').value.trim();
        var schoolName = document.getElementById('<%= txtSchoolName.ClientID %>').value.trim();
        var term = document.getElementById('<%= txtTerm.ClientID %>').value.trim();
        var exam = document.getElementById('<%= txtExamName.ClientID %>').value.trim();
        var balance = document.getElementById('<%= txtBalance.ClientID %>')?.value.trim() || "";
        var className = document.getElementById('<%= txtClass.ClientID %>')?.value.trim() || "";

        if (!phoneNumber) {
            alert("Please provide a valid phone number.");
            return;
        }

        var message = "";
        switch (type) {
            case "share":
                message = `*NOTIFICATION OF ${exam} EXAMINATION RESULTS*\n\n` +
                    `*${schoolName}* would like to inform you that *${exam}* Exam Results have been released for *${student}*.\n\n` +
                    `*Click the link below to download the School Report details:*\n${pdfUrl}`;
                break;

            case "summative":
                message = `*NOTIFICATION OF ${exam} EXAMINATION RESULTS*\n` +
                    `*TERM ${term}*\n\n` +
                    `*${schoolName}* would like to inform you that *${exam}* Exam Results have been released for *${student}*.\n\n` +
                    `*Click the link below to download the School Report details:*\n${pdfUrl}`;
                break;

            case "ContAssessment":
                message = `*NOTIFICATION OF ${exam} CONTINUOS ASSESSMENT REPORT*\n` +
                    `*TERM ${term}*\n\n` +
                    `*${schoolName}* would like to inform you that *${exam}* Continuous Assessment Exam Results have been released for *${student}*.\n\n` +
                    `*Click the link below to download the School Report details:*\n${pdfUrl}`;
                break;

            case "transcript":
                message = `*ACADEMIC TRANSCRIPT FOR ${student}*\n\n` +
                    `*${schoolName}* presents a comprehensive Academic Transcript for *${student}* as proof of enrollment and assessment.\n\n` +
                    `*Click the link below to download the Academic Transcript details:*\n${pdfUrl}`;
                break;

            case "examSheet":
                message = `*RESULTS SHEETS FOR ${exam} EXAMINATION RESULTS*\n` +
                    `*TERM ${term}*\n\n` +
                    `Find the Results Sheet for *${exam}* Exam Results for *${className}* class.\n\n` +
                    `*Click the link below to download the Examination Results Sheet details:*\n${pdfUrl}`;
                break;

            default:
                alert("Invalid message type.");
                return;
        }

        var whatsappUrl = `https://wa.me/${phoneNumber}?text=${encodeURIComponent(message)}`;
        console.log("WhatsApp URL:", whatsappUrl);
        window.open(whatsappUrl, "_blank");
    }
    function showLoadingOverlay() {
        $('#loading-overlay').show();
    }

    function hideLoadingOverlay() {
        $('#loading-overlay').hide();
    }

    $(document).ready(function () {

        $('#<%= btnGenerateReport.ClientID %>').on('click', function () {
        showLoadingOverlay();
    });

    $('#<%= BtnContinuous.ClientID %>').on('click', function () {
        showLoadingOverlay();
    });

    $('#<%= BtnContAssessment.ClientID %>').on('click', function () {
        showLoadingOverlay();
    });

    $('#<%= BtnResultSheet.ClientID %>').on('click', function () {
        showLoadingOverlay();
    });

    // Hide the loading overlay when the page has finished loading
    $(window).on('load', function () {
        hideLoadingOverlay();
    });
});

</script>

</asp:Content>
