<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Master.Master" AutoEventWireup="true" CodeBehind="SendAdmissionLetter.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.SendAdmissionLetter" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        html, body {
            height: 100%;
            margin: 0;
        }

        .container {
            height: 100%;
            display: flex;
            flex-direction: column;
        }

        .card {
            flex: 1;
        }

        .card-body {
            flex: 1;
            display: flex;
            flex-direction: column;
            justify-content: space-between;
        }

        .btn-dark-blue {
            background-color: #00008B;
            color: white;
        }

        .card-header {
            background-color: #001f3f; /* Very dark blue */
            font-size: smaller;
            padding: 0.25rem;
        }

        .btn-small {
            font-size: 0.50rem;
            padding: 0.20rem 0.5rem;
        }

        .successMessage {
            color: green;
            font-weight: bold;
        }

        .ErrorMessage {
            color: darkred;
            font-weight: bold;
        }

    </style>
        <link rel="stylesheet" href="https://cdn.datatables.net/1.12.1/css/jquery.dataTables.min.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css">
        <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-icons/1.4.0/font/bootstrap-icons.min.css">
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-icons/1.4.0/font/bootstrap-icons.min.css">
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
<div class="row">
    <div class="col-md-6">
        <asp:LinkButton ID="btnBudgetMonitoring" runat="server" CssClass="btn btn-dark-blue w-100" OnClick="btnBudgetMonitoring_Click">
            <i class="fas fa-file-contract"></i> Generate Admission Letter
        </asp:LinkButton>
    </div>
    <div class="col-md-6">
        <asp:LinkButton ID="btnShare" runat="server" CssClass="btn btn-success w-100" OnClientClick="shareOnWhatsApp(); return false;">
            <i class="fab fa-whatsapp"></i> Send Admission Letter Via WhatsApp
        </asp:LinkButton>
    </div>
</div>

    <div class="form-row">
        <div class="form-group col-md-3">
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="bi bi-person"></i></span>
                </div>
                <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" placeholder="Enter First Name" AutoComplete="off" required></asp:TextBox>
            </div>
        </div>
        <div class="form-group col-md-3">
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="bi bi-person"></i></span>
                </div>
                <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" placeholder="Enter Last Name" AutoComplete="off" required></asp:TextBox>
            </div>
        </div>
    

    <div class="form-group col-md-3">
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-telephone"></i></span>
            </div>
            <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" placeholder="Enter Phone" AutoComplete="off" required></asp:TextBox>
        </div>
    </div>
    <div class="form-group col-md-3">
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-telephone"></i></span>
            </div>
            <asp:TextBox ID="txtCandidateId" runat="server" CssClass="form-control" placeholder="Enter Phone" AutoComplete="off" required></asp:TextBox>
        </div>
    </div>

        </div>

        <div class="row mt-3">
        <div class="col-md-12">
            <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="113%" Height="750px" CssClass="table-responsive" />
        </div>
    </div>


    <script type="text/javascript">
        function shareOnWhatsApp() {
            var pdfUrl = '<%= Session["ReportPDF"] %>'; // Fetch the public URL
            var phoneNumber = document.getElementById('<%= txtPhone.ClientID %>').value; // Get phone number from the textbox
            var SchoolName = document.getElementById('<%= txtFirstName.ClientID %>').value; // Get phone number from the textbox

            if (phoneNumber && pdfUrl && pdfUrl !== "undefined" && pdfUrl !== "") {
                var message = encodeURIComponent("*ADMISSION OFFER LETTER* \n"
                    + "We are pleased to inform you that you have been successfully admitted to *" +SchoolName+ "* for the 2025/2026 academic year, starting September 8, 2025, following your entrance exam performance\n"
                    + "*Click the link below to download your offer letter for more details* \n"
                    + pdfUrl);
                var whatsappUrl = "https://wa.me/" + phoneNumber + "?text=" + message;

                console.log("WhatsApp URL:", whatsappUrl); // Debugging - Check the generated URL
                window.open(whatsappUrl, "_blank");
            } else {
                alert("Please provide a phone number and generate the report first.");
            }
        }
    </script>

</asp:Content>
