<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="ExchangeRateUpdater.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.ExchangeRateUpdater" Async="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        @keyframes fadeIn {
            0% { opacity: 0; transform: scale(0.8); }
            100% { opacity: 1; transform: scale(1); }
        }

        .card {
            animation: fadeIn 1s ease-out;
        }
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
        .container-fluid {
            margin-left: 40px;
            padding-right: 40px;
        }

        .btn {
            width: 100%;
            padding: 10px;
            background-color: #ff9800;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
        }

        .text-success {
            color: green;
        }

        .text-error {
            color: red;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row mb-5"></div>
    <div class="container-fluid">
        <div class="row mb-3">
            <div class="col-12 col-md-6 offset-md-3 mb-3">
                <div class="card shadow p-4" style="background-color: #fff; text-align: center;">
                    <div class="card-header mb-3" style="background-color: #007bff;">
                        <i class="fas fa-exchange-alt" style="color: #fff; font-size: 24px;"></i>
                        <h4 class="card-title mt-2" style="color: #fff;">Update Exchange Rates</h4>
                    </div>
                    <div class="card-body">
                        <div class="form-group mb-3">
                            <asp:Button ID="btnUpdateExchangeRates" runat="server" Text="Update Exchange Rates" OnClick="btnUpdateExchangeRates_Click" CssClass="btn" />
                        </div>

                        <asp:Label ID="lblStatus" runat="server" CssClass="mt-3 d-block text-success"></asp:Label>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>