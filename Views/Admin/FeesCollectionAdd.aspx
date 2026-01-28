<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Master.Master" AutoEventWireup="true" CodeBehind="FeesCollectionAdd.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.FeesCollectionAdd" Async="true" %>
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

                .select2 {
            width: 100% !important;
        }

    </style>
    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-icons/1.4.0/font/bootstrap-icons.min.css">
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
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
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
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
            <div>
                            <asp:Label ID="lblMessage" runat="server" visible="false" CssClass="successMessage"></asp:Label>

            </div>
            <div class="form-row">
                <div class="form-group col-md-4">
                    <label for="ddlStudent">Student</label>
                      <div class="input-group">
                        </div>
                          <asp:DropDownList ID="ddlStudent" runat="server" AutoPostBack="true"  CssClass="form-control select2" placeholder="Select Student" OnSelectedIndexChanged="ddlStudent_SelectedIndexChanged"></asp:DropDownList>
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
                    <label for="txtAmount">Amount</label>
                    <div class="input-group">
                        <div class="input-group-prepend">
                            <span class="input-group-text"><i class="bi bi-currency-dollar"></i></span>
                        </div>
                        <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control" placeholder="Enter Amount" required></asp:TextBox>
                    </div>
                </div>

<div class="form-group col-md-4" runat="server" id="divDateDeposited" visible="true">
        <label for="txtDate">Date Deposited (Optional)</label>
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-calendar"></i></span>
            </div>
            <asp:TextBox ID="txtDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
        </div>
    </div>
 
            
             <div class="form-group col-md-4" runat="server" id="divReference" visible="true">
        <label for="txtReference">Reference</label>
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-tag"></i></span>
            </div>
            <asp:TextBox ID="txtReference" runat="server" CssClass="form-control" placeholder="Enter Reference"></asp:TextBox>
        </div></div>

            <div class="form-row"></div>
            <div class="form-group text-center">
                <asp:Button ID="btnSubmit" runat="server" type="submit" CssClass="btn btn-dark-blue btn-dark-blue:hover btn-block" Text="Submit" OnClientClick="javascript:showProgress();" OnClick="btnSubmit_Click" />
                <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="0">
                    <ProgressTemplate>
                        <div class="progress">
                            <div class="progress-bar progress-bar-striped active" role="progressbar" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width: 100%">
                                Processing...
                            </div>
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <script type="text/javascript">
        function showProgress() {
            var updateProgress = $get('<%= UpdateProgress1.ClientID %>');
            updateProgress.style.display = 'block';
        }

        $(document).ready(function () {
            $('.select2').select2({
                placeholder: 'Select Student',
                allowClear: true
            });
        });



    </script>
</asp:Content>