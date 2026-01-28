<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Master.Master" AutoEventWireup="true" CodeBehind="BudgetReports.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.BudgetReports" %>
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
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

        <!-- Row for Buttons and Textboxes -->
        <div class="row">
            <div class="col-12">
                <asp:LinkButton ID="btnBudgetMonitoring" runat="server" CssClass="btn btn-dark-blue w-100" OnClick="btnBudgetMonitoring_Click">
                    <i class="fas fa-file-alt"></i> Budget Monitoring Report
                </asp:LinkButton>
            </div>
        </div>

    </div>

    <!-- Row for ReportViewer Controls -->
    <div class="row mt-3">
        <div class="col-md-12">
            <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="113%" Height="750px" CssClass="table-responsive" />
        </div>
    </div>
            <!-- TextBox for User to Enter Budget ID -->
        <div class="row mt-3">
            <div class="col-md-12">
                <div class="input-group">
                    <div class="input-group-prepend">
                        <span class="input-group-text"><i class="bi bi-currency-dollar"></i></span>
                    </div>
                    <asp:TextBox ID="txtBudgetId" runat="server" CssClass="form-control" placeholder="Enter Budget ID" />
                </div>
            </div>
        </div>

        <!-- TextBox for User to Enter Phone Number -->
        <div class="row mt-3">
            <div class="col-md-12">
                <div class="input-group">
                    <div class="input-group-prepend">
                        <span class="input-group-text"><i class="bi bi-telephone"></i></span>
                    </div>
                    <asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="form-control" placeholder="Enter phone number" />
                </div>
            </div>
        </div>

    <asp:Button ID="btnShare" runat="server" Text="Share via WhatsApp" CssClass="btn btn-success" OnClientClick="shareOnWhatsApp(); return false;" />

    <script type="text/javascript">
        function shareOnWhatsApp() {
            var pdfUrl = '<%= Session["ReportPDF"] %>'; // Fetch the public URL
            var phoneNumber = document.getElementById('<%= txtPhoneNumber.ClientID %>').value; // Get phone number from the textbox

            if (phoneNumber && pdfUrl && pdfUrl !== "undefined" && pdfUrl !== "") {
                var message = encodeURIComponent("Here is the budget monitoring report:\n" + pdfUrl);
                var whatsappUrl = "https://wa.me/" + phoneNumber + "?text=" + message;

                console.log("WhatsApp URL:", whatsappUrl); // Debugging - Check the generated URL
                window.open(whatsappUrl, "_blank");
            } else {
                alert("Please provide a phone number and generate the report first.");
            }
        }
    </script>

    <!-- Bootstrap JS -->
    <script src="https://code.jquery.com/jquery-3.3.1.slim.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.7/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/js/bootstrap.min.js"></script>
    <!-- ReportViewer JS -->
    <script src="https://ajax.aspnetcdn.com/ajax/reportsrv2010/reportviewer.js"></script>

</asp:Content>
