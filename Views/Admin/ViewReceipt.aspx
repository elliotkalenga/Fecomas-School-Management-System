<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewReceipt.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.ViewReceipt" %>


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
        function printReceipt() {
            window.print();
        }
    </script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <div class="container">
        <div class="row justify-content-center">
            <div class="col-12">
                <!-- Report Viewer in the middle -->
                <div class="report-container" style="width: 100vw; height: 80vh; overflow-x: auto;">
                    <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="100%" Height="100%" CssClass="report-viewer" />
                </div>

                <!-- Print Button -->
                <div class="form-group text-center mt-4">
                    <button class="btn btn-dark-blue btn-lg" onclick="printReceipt()">Print Receipt</button>
                </div>
            </div>
        </div>
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
                AutoPostBack="true" />
        </div>
    </div>
</asp:Content>
