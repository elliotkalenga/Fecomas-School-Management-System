<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Master.Master" AutoEventWireup="true" Async="true" CodeBehind="InvoicePrintAdd.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.InvoicePrintAdd" %>
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
    </style>
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

<script>
    $(document).ready(function () {
    });

    function filterStudents() {
        var input, filter, ddlStudent, options, i, txtValue;
        input = document.getElementById("studentSearch");
        filter = input.value.toUpperCase();
        ddlStudent = document.getElementById("<%= txtStudentNo.ClientID %>");
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
    <div class="row">
        
                <div class="col-md-12">
        <asp:LinkButton ID="BtnGenerateInvoice" runat="server" CssClass="btn btn-dark-blue btn-dark-blue:hover" OnClick="btnGenerateInvoice_Click">
            <i class="fas fa-file-alt"></i>Generate Student Invoice
        </asp:LinkButton>
    </div>



</div>

<asp:Panel ID="pnlWhatsAppButtons" runat="server" Visible="false" CssClass="row">
    <div class="row w-100">
        <div class="col-6">
            <asp:LinkButton ID="btnShare" runat="server" CssClass="btn btn-success w-100" OnClientClick="shareOnWhatsApp(); return false;">
                <i class="fab fa-whatsapp"></i> Send Initial Invoice
            </asp:LinkButton>
        </div>

        <div class="col-6">
            <asp:LinkButton ID="LinkButton1" runat="server" CssClass="btn btn-success w-100" OnClientClick="ReminderOnWhatsApp(); return false;">
                <i class="fab fa-whatsapp"></i> Send Payment Reminder
            </asp:LinkButton>
        </div>
    </div>
</asp:Panel>
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
        <div class="col-md-12">
            <!-- Load Receipt Button at the top -->
            <div class="form-group text-center mb-3">
            </div>

            <!-- Report Viewer in the middle -->
            <div class="form-group">
                <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="110%" Height="700px" CssClass="table-responsive" />
            </div>

            <!-- FeesCollectionId Textbox at the bottom -->
                            <div class="form-row">
                                        <div class="form-group col-md-4">

                <div class="input-group">
                    <div class="input-group-prepend">
                        <span class="input-group-text"><i class="bi bi-currency-dollar"></i></span>
                    </div>
                    <asp:TextBox ID="txtStudentNo" Visible="true" runat="server" CssClass="form-control" placeholder="Enter Amount" ></asp:TextBox>
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
        </div>

            <!-- Submit Button -->
            <div class="form-group text-center">
            </div>
        </div>
    </div>
   <script type="text/javascript">
              function shareOnWhatsApp() {
                  var pdfUrl = '<%= Session["ReportPDF"] %>'; // Fetch the public URL
    var phoneNumber = document.getElementById('<%= txtPhone.ClientID %>').value; // Get phone number from the textbox
    var student = document.getElementById('<%= txtStudentName.ClientID %>').value;
    var schoolName = document.getElementById('<%= txtSchoolName.ClientID %>').value;
    var term = document.getElementById('<%= txtTerm.ClientID %>').value;

                  if (phoneNumber && pdfUrl && pdfUrl !== "undefined" && pdfUrl !== "") {
                      var message = encodeURIComponent(
                          "*SCHOOL FEES INVOICE NOTICE*\n" +
                          "*" + schoolName + "* would like to inform you that Term *" + term +
                          "* school fee invoice for *" + student + "* is ready for payment.\n" +
                          "*Click the link below to download the invoice details as well as payment details:*\n" + pdfUrl
                      );

                      var whatsappUrl = "https://wa.me/" + phoneNumber + "?text=" + message;
                      console.log("WhatsApp URL:", whatsappUrl); // Debugging - Check the generated URL
                      window.open(whatsappUrl, "_blank");
                  } else {
                      alert("Please provide a phone number and generate the report first.");
                  }
              }

              function ReminderOnWhatsApp() {
                  var pdfUrl = '<%= Session["ReportPDF"] %>'; // Fetch the public URL
    var phoneNumber = document.getElementById('<%= txtPhone.ClientID %>').value; 
    var student = document.getElementById('<%= txtStudentName.ClientID %>').value; 
    var schoolName = document.getElementById('<%= txtSchoolName.ClientID %>').value; 
    var term = document.getElementById('<%= txtTerm.ClientID %>').value; 
                  var balance = document.getElementById('<%= txtBalance.ClientID %>').value;

                  if (phoneNumber && pdfUrl && pdfUrl !== "undefined" && pdfUrl !== "") {
                      var message = encodeURIComponent(
                          "*GENTLE REMINDER ON PENDING FEES BALANCE*\n" +
                          "*" + schoolName + "* would like to remind you that your ward *" + student +
                          "* has a pending fees balance of *" + balance + "* for Term *" + term + "*.\n" +
                          "*Click the link below to download the invoice Payment Status Details:*\n" + pdfUrl
                      );

                      var whatsappUrl = "https://wa.me/" + phoneNumber + "?text=" + message;
                      console.log("WhatsApp URL:", whatsappUrl); // Debugging - Check the generated URL
                      window.open(whatsappUrl, "_blank");
                  } else {
                      alert("Please provide a phone number and generate the report first.");
                  }
              }


</script>


</asp:Content>
