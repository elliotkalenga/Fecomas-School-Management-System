<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="LibraryInventoryReport.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.LibraryInventoryReport" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://ajax.aspnetcdn.com/ajax/reportsrv2010/reportviewer.css" rel="stylesheet" />

    <style>

                .container {
    max-width: 100% !important;
}

.card {
    max-width: 100% !important;
}

.table-responsive {
    width: 100% !important;
}

#studentsTable {
    width: 100% !important;
}
        .container {
            max-width: 1200px;
            margin: 0 auto;
            padding: 15px;
        }

        .card {
            margin: 0 auto;
            max-width: 1000px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            border-radius: 8px;
            padding: 20px;
        }

        .btn-dark-blue {
            background-color: #001f3f !important;
            border-color: #001f3f !important;
            color: #ffffff !important;
        }

        .btn-dark-blue:hover {
            background-color: #001a33 !important;
            border-color: #001a33 !important;
        }

        .report-container {
            overflow-x: auto;
        }

        .report-button-row {
            display: flex;
            justify-content: space-between;
            flex-wrap: wrap;
            margin-bottom: 15px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <div class="container mt-2">
        <div class="card">
            <div class="card-header d-flex justify-content-between align-items-center">
                <h5 class="mb-0">Library Inventory Report</h5>
            </div>

            <div class="card-body">
                <div class="report-button-row mb-3">
                    <asp:LinkButton ID="btnLibraryInventory" runat="server" CssClass="btn btn-dark-blue" OnClick="btnLibraryInventory_Click">
                        <i class="fas fa-file-alt"></i> Generate Library Inventory Report
                    </asp:LinkButton>

                                        <asp:LinkButton ID="LinkButton1" runat="server" CssClass="btn btn-dark-blue" OnClick="btnBorrowed_Click">
                        <i class="fas fa-file-alt"></i> Borrowed Books
                    </asp:LinkButton>

                                                            <asp:LinkButton ID="LinkButton2" runat="server" CssClass="btn btn-dark-blue" OnClick="btnLMissing_Click">
                        <i class="fas fa-file-alt"></i> Missing Books
                    </asp:LinkButton>


                </div>

                <div class="report-container">
                    <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="100%" Height="500px" CssClass="table-responsive" />
                </div>
            </div>
        </div>
    </div>

    <!-- Scripts -->
    <script src="https://code.jquery.com/jquery-3.3.1.slim.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.7/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/js/bootstrap.min.js"></script>
    <script src="https://ajax.aspnetcdn.com/ajax/reportsrv2010/reportviewer.js"></script>

    <script>
        function shareOnWhatsApp() {
            var reportUrl = window.location.href;
            var message = "Check out the Library Inventory Report: " + reportUrl;
            var whatsappUrl = "https://wa.me/?text=" + encodeURIComponent(message);
            window.open(whatsappUrl, "_blank");
        }
    </script>
</asp:Content>
