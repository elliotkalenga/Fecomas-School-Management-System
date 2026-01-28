<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Master.Master" AutoEventWireup="true" CodeBehind="PrintReceipt.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.PrintReceipt" %>
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
            font-size: smaller; /* Smaller font size */
            padding: 0.25rem; /* Reduced padding */
        }

        .form-control::placeholder {
            color: #6c757d; /* Optional, adjust placeholder color */
        }

        .card-header {
            background-color: #001f3f; /* Very dark blue */
            font-size: smaller; /* Smaller font size */
            padding: 0.25rem; /* Reduced padding */
        }

        .btn-small {
            font-size: 0.50rem; /* Adjust font size */
            padding: 0.20rem 0.5rem; /* Adjust padding for smaller button */
        }

        .successMessage {
            color: green;
            font-weight: bold;
        }

        .ErrorMessage {
            color: darkred;
            font-weight: bold;
        }
        @media (max-width: 768px) {
    .table-responsive > div {
        overflow-x: auto;
        -webkit-overflow-scrolling: touch;
    }

    .btn-block {
        width: 100% !important;
    }

    .form-control {
        width: 100% !important;
    }
}

    </style>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-icons/1.4.0/font/bootstrap-icons.min.css">
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
<script>
    function printOnlyReport() {
        var reportArea = document.querySelector('.report-container');
        if (!reportArea) {
            alert("Report not found.");
            return;
        }

        var printWindow = window.open('', '_blank');
        printWindow.document.write('<html><head><title>Print Receipt</title>');
        printWindow.document.write('<link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">');
        printWindow.document.write('</head><body>');
        printWindow.document.write(reportArea.innerHTML);
        printWindow.document.write('</body></html>');
        printWindow.document.close();

        printWindow.onload = function () {
            printWindow.focus();
            printWindow.print();
            printWindow.close();
        };
    }

    $(document).ready(function () {
    });

    function filterStudents() {
        var input, filter, ddlStudent, options, i, txtValue;
        input = document.getElementById("studentSearch");
        filter = input.value.toUpperCase();
        ddlStudent = document.getElementById("<%= txtFeesCollectionId.ClientID %>");
        options = ddlStudent.getElementsByTagName("option");

        var found = false;
        for (i = 0; i < options.length; i++) {
            txtValue = options[i].textContent || options[i].innerText;
            if (txtValue.toUpperCase().indexOf(filter) > -1) {
                options[i].style.display = "";
                found = true;
            } else {
                options[i].style.display = "none";
            }
        }

        if (!found) {
            var option = document.createElement("option");
            option.text = "No Transaction found";
            option.value = "";
            option.style.display = "";
            ddlStudent.appendChild(option);
            ddlStudent.selectedIndex = -1;
        } else {
            // Remove the "No student found" option if it exists
            var noStudentOption = Array.from(options).find(option => option.text === "No Transaction found");
            if (noStudentOption) {
                ddlStudent.removeChild(noStudentOption);
            }
        }
    }
</script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
<div class="text-center my-3">
    <asp:Button ID="btnPrintPdf" runat="server" Text="Print Receipt" CssClass="btn btn-dark-blue" OnClick="btnPrintPdf_Click" />
</div>

    <div class="modal fade" id="successModal" tabindex="-1" role="dialog" CssClass="successMessage" aria-labelledby="successModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" CssClass="successMessage" id="successModalLabel">Success</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <asp:Label ID="lblMessage" runat="server" CssClass="successMessage"></asp:Label>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <!-- Error Modal -->
    <div class="modal fade" id="errorModal" tabindex="-1" role="dialog" aria-labelledby="errorModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="errorModalLabel">Error</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <asp:Label ID="ErrorMessage" runat="server" ForeColor="Red" Text=""></asp:Label>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

<div class="container">
    <div class="row justify-content-center">
        <div class="col-12">

            <!-- Report Viewer in the middle -->
<div class="report-container" style="width: 100vw; height: 80vh; overflow-x: auto;">
    <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="100%" Height="100%" CssClass="report-viewer" />
</div>

            <!-- FeesCollectionId Textbox at the bottom (hidden) -->
            <div class="form-group">
                <div class="input-group">
                    <div class="input-group-prepend">
                        <span class="input-group-text">
                            <i class="bi bi-currency-dollar"></i>
                        </span>
                    </div>
                    <asp:TextBox ID="txtFeesCollectionId" Visible="false" runat="server"
                        CssClass="form-control"
                        placeholder="Enter Amount"
                        OnTextChanged="txtFeesCollectionId_TextChanged"
                        AutoPostBack="true" />
                </div>
            </div>
        </div>
    </div>
</div>
    <style>
    .spinner-overlay {
        position: fixed;
        top: 0;
        left: 0;
        height: 100vh;
        width: 100vw;
        background-color: rgba(255, 255, 255, 0.8);
        display: flex;
        justify-content: center;
        align-items: center;
        z-index: 9999;
        flex-direction: column;
    }

    .spinner-border {
        width: 3rem;
        height: 3rem;
        border-width: 0.3em;
    }

    .spinner-text {
        margin-top: 1rem;
        font-weight: bold;
        color: #001f3f; /* Match your theme */
    }
</style>

<div id="loadingSpinner" class="spinner-overlay">
    <div class="spinner-border text-primary" role="status"></div>
    <div class="spinner-text">Loading receipt, please wait...</div>
</div>

<script>
    window.onload = function () {
        setTimeout(function () {
            var spinner = document.getElementById("loadingSpinner");
            if (spinner) {
                spinner.style.display = "none";
            }
        }, 5000); // 5 seconds
    };
</script>



    <div id="htmlReceiptContainer" runat="server" visible="false" class="mt-4">
    <div class="card">
        <div class="card-header text-white bg-primary">
            Receipt
        </div>
        <div class="card-body" id="receiptContent">
            <!-- Populated from code-behind -->
        </div>
    </div>
    <div class="text-center mt-2">
        <button class="btn btn-dark-blue" onclick="window.print();">Print</button>
    </div>
</div>

</asp:Content>
