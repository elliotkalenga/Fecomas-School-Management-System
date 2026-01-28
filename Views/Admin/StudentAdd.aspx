<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Master.Master" AutoEventWireup="true" CodeBehind="StudentAdd.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.StudentAdd" %>
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
    <script>
        $(document).ready(function () {
            // Disable autocomplete for all input fields with autocomplete="off"
            $('input[autocomplete="off"]').attr('autocomplete', 'new-password');
        });
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


        <div class="modal fade" id="ErrorModal" tabindex="-1" role="dialog" CssClass="ErrorMessage" aria-labelledby="ErrorModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" CssClass="ErrorMessage" id="ErrorModalLabel">Validation Error</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <asp:Label ID="ErrorMessage" runat="server" CssClass="ErrorMessage"></asp:Label>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>



<div class="form-row">
    <div class="form-group col-md-6">
        <label for="txtFirstName">First Name</label>
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-person"></i></span>
            </div>
            <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" placeholder="Enter First Name" AutoComplete="off" required></asp:TextBox>
        </div>
    </div>
    <div class="form-group col-md-6">
        <label for="txtLastName">Last Name</label>
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-person"></i></span>
            </div>
            <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" placeholder="Enter Last Name" AutoComplete="off" required></asp:TextBox>
        </div>
    </div>
    </div>
    <div class="form-row">
    <div class="form-group col-md-6">
        <label for="ddlGender">Gender</label>
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-gender-ambiguous"></i></span>
            </div>
            <asp:DropDownList ID="ddlGender" runat="server" CssClass="form-control" placeholder="Select Gender"></asp:DropDownList>
        </div>
    </div>
    <div class="form-group col-md-6">
    <label for="txtRegCode">Registration Code (Optional)</label>
    <div class="input-group">
        <div class="input-group-prepend">
            <span class="input-group-text"><i class="bi bi-key"></i></span>
        </div>
        <asp:TextBox ID="txtRegCode" runat="server" CssClass="form-control" placeholder="Enter Registration Code" AutoComplete="off"></asp:TextBox>
    </div>  </div>

</div>

    
    <div class="form-row">
    <div class="form-group col-md-4">
        <label for="txtGuardian">Guardian</label>
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-person-badge"></i></span>
            </div>
            <asp:TextBox ID="txtGuardian" runat="server" CssClass="form-control" placeholder="Enter Guardian" AutoComplete="off" required></asp:TextBox>
        </div>
    </div>
    <div class="form-group col-md-4">
        <label for="ddlStatus">Status</label>
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-info-circle"></i></span>
            </div>
            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control" placeholder="Select Status"></asp:DropDownList>
        </div>
    </div>
    <div class="form-group col-md-4">
        <label for="txtPhone">Phone</label>
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-telephone"></i></span>
            </div>
            <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" placeholder="Enter Phone" AutoComplete="off" required></asp:TextBox>
        </div>
    </div>
</div>
<div class="form-row">
    <div class="form-group col-md-6">
        <label for="txtAddress">Address</label>
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-house"></i></span>
            </div>
            <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" placeholder="Enter Address" AutoComplete="off"></asp:TextBox>
        </div>
    </div>
    <div class="form-group col-md-6">
        <label for="txtEmail">Email</label>
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-envelope"></i></span>
            </div>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Enter Email"></asp:TextBox>
        </div>
    </div>
</div>
<div class="form-row">
    <div class="form-group col-md-6">
        <label for="fuImage">Upload Image</label>
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-upload"></i></span>
            </div>
            <asp:FileUpload ID="fuImage" runat="server" CssClass="form-control" />
        </div>
    </div>
    <div class="form-group col-md-6">
        <label for="txtDob">Date of Birth</label>
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-calendar"></i></span>
            </div>
            <asp:TextBox ID="txtDob" runat="server" CssClass="form-control" placeholder="Enter Date of Birth" TextMode="Date" ></asp:TextBox>
        </div>
    </div>
</div>

    <div class="form-row"></div>
    <div class="form-group text-center">
        <asp:Button ID="btnSubmit" runat="server" type="submit" CssClass="btn btn-dark-blue btn-dark-blue:hover btn-block" Text="Submit" OnClick="btnSubmit_Click" />
    </div>
</asp:Content>


