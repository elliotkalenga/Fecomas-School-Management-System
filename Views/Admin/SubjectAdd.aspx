<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Master.Master" AutoEventWireup="true" CodeBehind="SubjectAdd.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.SubjectAdd" %>
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
            background-color: #001f3f;
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

    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <div class="modal fade" id="successModal" tabindex="-1" role="dialog" aria-labelledby="successModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title successMessage" id="successModalLabel">Success</h5>
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
                    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Text=""></asp:Label>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

<div class="container mt-3">
    <div class="form-row">
        <div class="form-group col-md-4">
            <label for="txtSubjectCode">Subject Code</label>
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="fas fa-code"></i></span> <!-- Icon for Subject Code -->
                </div>
                <asp:TextBox ID="txtSubjectCode" runat="server" CssClass="form-control" placeholder="Enter Subject Code" required></asp:TextBox>
            </div>
        </div>
        <div class="form-group col-md-4">
            <label for="txtSubjectName">Subject Name</label>
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="fas fa-book"></i></span> <!-- Icon for Subject Name -->
                </div>
                <asp:TextBox ID="txtSubjectName" runat="server" CssClass="form-control" placeholder="Enter Subject Name" required></asp:TextBox>
            </div>
        </div>
        <div class="form-group col-md-4">
            <label for="txtDescription">Subject Description</label>
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="fas fa-info-circle"></i></span> <!-- Icon for Description -->
                </div>
                <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" placeholder="Enter Description" required></asp:TextBox>
            </div>
        </div>
    </div>

    <div class="form-group text-center">
        <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-dark-blue btn-block" Text="Submit" OnClick="btnSubmit_Click" />
    </div>
</div>
        <script>
            $(document).ready(function () {
            });

            function filterStudents() {
                var input, filter, ddlStudent, options, i, txtValue;
                input = document.getElementById("studentSearch");
                filter = input.value.toUpperCase();
                ddlStudent = document.getElementById("<%= txtDescription.ClientID %>");
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
                    option.text = "No student found";
                    option.value = "";
                    option.style.display = "";
                    ddlStudent.appendChild(option);
                    ddlStudent.selectedIndex = -1;
                } else {
                    // Remove the "No student found" option if it exists
                    var noStudentOption = Array.from(options).find(option => option.text === "No student found");
                    if (noStudentOption) {
                        ddlStudent.removeChild(noStudentOption);
                    }
                }
            }
        </script>

</asp:Content>
