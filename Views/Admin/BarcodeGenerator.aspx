<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="BarcodeGenerator.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.BarcodeGenerator" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
        .card {
            width: 100%;
            max-width: 1100px;
            margin: 0 auto;
            box-shadow: 0px 4px 6px rgba(0, 0, 0, 0.1);
            border-radius: 8px;
            padding: 20px;
        }
        .table-responsive {
            width: 100%;
            overflow-x: auto;
        }
        .d-flex {
            display: flex;
        }
        .flex-wrap {
            flex-wrap: wrap;
        }
        .mr-2 {
            margin-right: 0.5rem;
        }
        .mb-2 {
            margin-bottom: 0.5rem;
        }
        .text-center {
            text-align: center;
        }
        .d-block {
            display: block;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mt-2">
        <div class="card">
            <div class="card-header d-flex justify-content-between align-items-center p-2">
                <h5>Barcode Generator</h5>
            </div>
            <div class="card-body">
                <div class="d-flex">
                    <div class="form-group mr-2">
                        <asp:Label ID="lblPrefix" runat="server" Text="Prefix:"></asp:Label>
                        <asp:TextBox ID="txtPrefix" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group mr-2">
                        <asp:Label ID="lblSuffix" runat="server" Text="Suffix:"></asp:Label>
                        <asp:TextBox ID="txtSuffix" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group mr-2">
                        <asp:Label ID="lblStart" runat="server" Text="Start Point:"></asp:Label>
                        <asp:TextBox ID="txtStart" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group mr-2">
                        <asp:Label ID="lblEnd" runat="server" Text="End Point:"></asp:Label>
                        <asp:TextBox ID="txtEnd" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group mr-2">
                        <asp:Button ID="btnGenerate" runat="server" Text="Generate Barcodes" OnClick="btnGenerate_Click" CssClass="btn btn-primary mt-4" />
                        <asp:Button ID="btnExportToPdf" runat="server" Text="Export to PDF" OnClick="btnExportToPdf_Click" CssClass="btn btn-secondary mt-4" />
                    </div>
                </div>
                <div class="table-responsive mt-4">
                    <asp:PlaceHolder ID="phBarcodes" runat="server"></asp:PlaceHolder>
                </div>

            </div>
        </div>
    </div>
</asp:Content>