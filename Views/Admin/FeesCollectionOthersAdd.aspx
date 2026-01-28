<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Master.Master" AutoEventWireup="true" CodeBehind="FeesCollectionOthersAdd.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.FeesCollectionOthersAdd" %>
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
<script>
    $(document).ready(function () {
    });

    function filterStudents() {
        var input, filter, ddlStudent, options, i, txtValue;
        input = document.getElementById("studentSearch");
        filter = input.value.toUpperCase();
        ddlStudent = document.getElementById("<%= ddlStudent.ClientID %>");
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

   <div class="form-row">
    <div class="form-group col-md-4">
        <label for="ddlStudent">Student</label>
                <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-search"></i></span>
            </div>
            <input type="text" id="studentSearch" class="form-control" onkeyup="filterStudents()" placeholder="Type to search student">
        </div>

        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-person"></i></span>
            </div>
            <asp:DropDownList ID="ddlStudent" runat="server" CssClass="form-control" placeholder="Select Student"></asp:DropDownList>
        </div>
    </div>


           <div class="form-group col-md-4">
        <label for="ddlPaidFor">Paid For</label>
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-credit-card"></i></span>
            </div>
            <asp:DropDownList ID="ddlPaidFor" runat="server" CssClass="form-control" placeholder="Select Payment Purpose">
            </asp:DropDownList>
        </div>
    </div>

    <div class="form-group col-md-4">
        <label for="ddlPaymentMethod">Payment Method</label>
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-credit-card"></i></span>
            </div>
            <asp:DropDownList ID="ddlPaymentMethod" runat="server" CssClass="form-control" placeholder="Select Payment Method">
            </asp:DropDownList>
        </div>
    </div>

           <div class="form-group col-md-4">
        <label for="txtDescription">Description</label>
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-currency-dollar"></i></span>
            </div>
            <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" placeholder="Enter Description"></asp:TextBox>
        </div>
    </div>

    <div class="form-group col-md-4">
        <label for="txtAmount">Amount</label>
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-currency-dollar"></i></span>
            </div>
            <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control" placeholder="Enter Amount" required></asp:TextBox>
        </div>
    </div>


</div>

<div class="form-row"></div>
<div class="form-group text-center">
    <asp:Button ID="btnSubmit" runat="server" type="submit" CssClass="btn btn-dark-blue btn-dark-blue:hover btn-block" Text="Submit" OnClick="btnSubmit_Click" />
</div>

</asp:Content>
