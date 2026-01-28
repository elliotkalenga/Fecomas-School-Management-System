<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Master.Master" AutoEventWireup="true" CodeBehind="BorrowBookAdd.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.BorrowBookAdd" %>
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
            background-color: #00008B; /* Very dark blue */
            font-size: smaller; /* Smaller font size */
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
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />

    <script>
        $(document).ready(function () {
            $('.select2').select2({
                placeholder: "Select an option",
                allowClear: true,
                width: '100%'
            });
        });

        function filterBooks() {
            var input, filter, ddlBook, options, i, txtValue;
            input = document.getElementById("bookSearch");
            filter = input.value.toUpperCase();
            ddlBook = document.getElementById("<%= ddlBook.ClientID %>");
            options = ddlBook.getElementsByTagName("option");

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
                option.text = "No book found";
                option.value = "";
                option.style.display = "";
                ddlBook.appendChild(option);
                ddlBook.selectedIndex = -1;
            } else {
                // Remove the "No book found" option if it exists
                var noBookOption = Array.from(options).find(option => option.text === "No book found");
                if (noBookOption) {
                    ddlBook.removeChild(noBookOption);
                }
            }
        }

        function filterMembers() {
            var input, filter, ddlMember, options, i, txtValue;
            input = document.getElementById("memberSearch");
            filter = input.value.toUpperCase();
            ddlMember = document.getElementById("<%= ddlMember.ClientID %>");
            options = ddlMember.getElementsByTagName("option");

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
                option.text = "No member found";
                option.value = "";
                option.style.display = "";
                ddlMember.appendChild(option);
                ddlMember.selectedIndex = -1;
            } else {
                // Remove the "No member found" option if it exists
                var noMemberOption = Array.from(options).find(option => option.text === "No member found");
                if (noMemberOption) {
                    ddlMember.removeChild(noMemberOption);
                }
            }
        }
    </script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <!-- Success Modal -->
    <div class="modal fade" id="successModal" tabindex="-1" role="dialog" aria-labelledby="successModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="successModalLabel">Success</h5>
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

    <!-- Book Search with Dropdown -->
    <div class="form-row">
        <div class="form-group col-md-4">
            <label for="bookSearch">Search a Book</label>
            <div class="input-group">
                <input type="text" id="bookSearch" class="form-control" onkeyup="filterBooks()" placeholder="Type to search a Book">
                <div class="input-group-append">
                    <asp:DropDownList ID="ddlBook" runat="server" CssClass="form-control select2" placeholder="Select Book"></asp:DropDownList>
                </div>
            </div>
        </div>

        <!-- Member Search with Dropdown -->
        <div class="form-group col-md-4">
            <label for="memberSearch">Search Library Member</label>
            <div class="input-group">
                <input type="text" id="memberSearch" class="form-control" onkeyup="filterMembers()" placeholder="search a Member">
                <div class="input-group-append">
                    <asp:DropDownList ID="ddlMember" runat="server" CssClass="form-control select2" placeholder="Select Library Member"></asp:DropDownList>
                </div>
            </div>
        </div>

        <!-- Number of Days -->
        <div class="form-group col-md-4">
            <label for="txtDays">Number of Days</label>
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="bi bi-calendar-day"></i></span>
                </div>
                <asp:TextBox ID="txtDays" runat="server" CssClass="form-control" required placeholder="Enter Number of Days"></asp:TextBox>
            </div>
        </div>
    </div>

    <div class="form-row"></div>
    <div class="form-group text-center">
        <asp:Button ID="btnSubmit" runat="server" type="submit" CssClass="btn btn-dark-blue btn-dark-blue:hover btn-block" Text="Submit" OnClick="btnSubmit_Click" />
    </div>

</asp:Content>
