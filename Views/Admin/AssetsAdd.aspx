<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Master.Master" AutoEventWireup="true" CodeBehind="AssetsAdd.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.AssetsAdd" %>
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
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <!-- Success Modal -->
    <div class="modal fade" id="successModal" tabindex="-1" role="dialog" CssClass="successMessage" aria-labelledby="successModalLabel" aria-hidden="true">
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
            <label for="txtAssetCode">Asset Code</label>
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="bi bi-barcode"></i></span>
                </div>
                <asp:TextBox ID="txtAssetCode" runat="server" CssClass="form-control" placeholder="Enter Asset Code" required></asp:TextBox>
            </div>
        </div>
        <div class="form-group col-md-4">
            <label for="txtAssetName">Asset Name</label>
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="bi bi-tag"></i></span>
                </div>
                <asp:TextBox ID="txtAssetName" runat="server" CssClass="form-control" placeholder="Enter Asset Name" required></asp:TextBox>
            </div>
        </div>
        <div class="form-group col-md-4">
            <label for="txtAssetDescription">Asset Description</label>
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="bi bi-card-text"></i></span>
                </div>
                <asp:TextBox ID="txtAssetDescription" runat="server" CssClass="form-control" placeholder="Enter Asset Description" TextMode="MultiLine"></asp:TextBox>
            </div>
        </div>
    </div>

    <div class="form-row">
        <div class="form-group col-md-4">
            <label for="ddlAssetCategory">Asset Category</label>
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="bi bi-list-ul"></i></span>
                </div>
                <asp:DropDownList ID="ddlAssetCategory" runat="server" CssClass="form-control"></asp:DropDownList>
            </div>
        </div>
        <div class="form-group col-md-4">
            <label for="txtLifespan">Lifespan</label>
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="bi bi-hourglass-split"></i></span>
                </div>
                <asp:TextBox ID="txtLifespan" runat="server" CssClass="form-control" placeholder="Enter Lifespan"  required></asp:TextBox>
            </div>
        </div>
        <div class="form-group col-md-4">
            <label for="txtPurchasedDate">Purchased Date</label>
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="bi bi-calendar"></i></span>
                </div>
                <asp:TextBox ID="txtPurchasedDate" runat="server" CssClass="form-control" placeholder="Enter Purchased Date" TextMode="Date"  required></asp:TextBox>
            </div>
        </div>
    </div>

    <div class="form-row">
        <div class="form-group col-md-4">
            <label for="txtPurchasedCost">Purchased Cost</label>
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="bi bi-currency-dollar"></i></span>
                </div>
                <asp:TextBox ID="txtPurchasedCost" runat="server" CssClass="form-control" placeholder="Enter Purchased Cost"  required></asp:TextBox>
            </div>
        </div>
    </div>

    <div class="form-group text-center">
        <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-dark-blue btn-block" Text="Submit" OnClick="btnSubmit_Click" />
    </div>
</div></asp:Content>
