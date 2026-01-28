<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Master.Master" AutoEventWireup="true" CodeBehind="BulkEnrollment.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.BulkEnrollment" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
.pagination-container {
    display: flex;
    flex-wrap: wrap; /* Allow pagination links to wrap to the next row */
    justify-content: center; /* Center the pagination */
    margin-top: 20px;
    position: relative;
    width: 100%;
    display: none !important; /* Completely hide the pagination controls */

}
.table {
    font-size: 12px; /* Adjust the font size as needed */
}

.table th, .table td {
    font-size: inherit; /* Ensure consistent font size in table headers and cells */
}


.page-link {
    margin: 5px; /* Add spacing between links */
    padding: 8px 12px;
    border: 1px solid #007bff;
    color: #007bff;
    text-decoration: none;
    border-radius: 3px;
    font-size: 14px;
    min-width: 40px; /* Ensure links are a consistent size */
    text-align: center;
}

.page-link:hover {
    background-color: #007bff;
    color: white;
}

.page-link.active {
    background-color: #007bff;
    color: white;
    border-color: #007bff;
}

        .modal-dialog {
            max-width: 90%;
            width: 80%;
            height: auto;
        }

        #studentIframe {
            width: 100%;
            height: 400px;
        }


        .table-responsive {
            overflow-x: auto;
        }

        .table-responsive {
    width: 100%; /* Ensure table doesn't exceed available space */
    overflow-x: auto; /* Enable horizontal scrolling if content overflows */
}

        .card {
            width: 100%;
        }
        .container {
    max-width: 1200px; /* Set a maximum width for the container */
    margin: 0 auto; /* Center the container horizontally */
    padding: 15px; /* Add spacing around the content */
    box-sizing: border-box; /* Include padding in the width calculation */
}

.card {
    max-width: 1000px; /* Limit the card's width */
    margin: 0 auto; /* Center the card within the container */
    box-shadow: 0px 4px 6px rgba(0, 0, 0, 0.1); /* Optional: Add a subtle shadow for better aesthetics */
    border-radius: 8px; /* Optional: Add rounded corners */
    padding: 20px; /* Optional: Add internal padding for spacing */
}


   .modal-dialog {
        max-width: 400px;
    }
        .text-success {
            color: green;
        }
    </style>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-icons/1.4.0/font/bootstrap-icons.min.css">
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
<script>
    $(document).ready(function () {
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

<div class="section-container">
    <div class="section-title"><h4>Source Section</h4></div>
    <div class="form-row">
        <!-- Source Class Dropdown -->
        <div class="form-group col-md-6">
            <label for="SourceClass">Source Class</label>
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="bi bi-mortarboard"></i></span>
                </div>
                <asp:DropDownList ID="ddlSourceClass" runat="server" CssClass="form-control" placeholder="Select Source Class"></asp:DropDownList>
            </div>
        </div>

        <!-- Source Term Dropdown -->
        <div class="form-group col-md-6">
            <label for="SourceTerm">Source Term</label>
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="bi bi-calendar"></i></span>
                </div>
                <asp:DropDownList ID="ddlSourceTerm" runat="server" CssClass="form-control" placeholder="Select Source Term"></asp:DropDownList>
            </div>
        </div>
    </div>
</div>

<!-- Destination Section -->
<div class="section-container">
    <div class="section-title"><h4>Destination Section</h4></div>
    <div class="form-row">
        <!-- Destination Class Dropdown -->
        <div class="form-group col-md-6">
            <label for="DestinationClass">Destination Class</label>
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="bi bi-mortarboard"></i></span>
                </div>
                <asp:DropDownList ID="ddlDestinationClass" runat="server" CssClass="form-control" placeholder="Select Destination Class"></asp:DropDownList>
            </div>
        </div>

        <!-- Destination Term Dropdown -->
        <div class="form-group col-md-6">
            <label for="DestinationTerm">Destination Term</label>
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="bi bi-calendar"></i></span>
                </div>
                <asp:DropDownList ID="ddlDestinationTerm" runat="server" CssClass="form-control" placeholder="Select Destination Term"></asp:DropDownList>
            </div>
        </div>
    </div>
</div>
    <div class="form-row"></div>
    <div class="form-group text-center">
        <asp:Button ID="btnSubmit" runat="server" type="submit" CssClass="btn btn-dark-blue btn-dark-blue:hover btn-block" Text="Submit" OnClick="btnSubmit_Click" />
    </div>
</asp:Content>
