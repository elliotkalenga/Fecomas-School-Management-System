<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Master.Master" AutoEventWireup="true" CodeBehind="RequisitionItemAdd.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.RequisitionItemAdd" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        html, body {
            height: 100%;
            margin: 0;
        }
                        .table {
            font-size: 12px;
        }
        .table th, .table td {
            font-size: inherit;
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
                #studentIframe {
            width: 100%;
            height: 250px;
        }

    </style>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-icons/1.4.0/font/bootstrap-icons.min.css">
        <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css">
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
<div class="row">
    <div class="form-group col-md-6">
        <label for="txtItem">Requisition Item</label>
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-ui-checks"></i></span>
            </div>
            <asp:TextBox ID="txtItem" runat="server" CssClass="form-control" 
                 placeholder="Enter Requisition Item"></asp:TextBox>
        </div>     </div>
    <div class="form-group col-md-5">
        <label for="txtAMount">Requisition Amount</label>
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-ui-checks"></i></span>
            </div>
            <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control" 
                 placeholder="Enter Requisition Amount"></asp:TextBox>
        </div>     </div>
 

        <!-- Requisition ID -->
    <div class="form-group col-md-1">
        <label for="txtRequisitionId"></label>
        <div class="input-group">
            <div class="input-group-prepend">
            </div>
            <asp:TextBox ID="txtRequisitionId"  ReadOnly="true"  required runat="server" CssClass="form-control" placeholder="Enter Requisition ID"></asp:TextBox>
        </div>
    </div>


</div>
</div>
</div>

        <div class="form-group text-center">
            <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-dark-blue btn-block" Text="Submit" OnClick="btnSubmit_Click" />
        </div>
   

                <div class="card-body">
                <div class="table-responsive">
                    <table id="studentsTable" class="table table-striped table-bordered" style="width:100%">
                        <thead>
                            <tr>
                                <th>Item #</th>
                                <th>Item</th>
                                <th>Amount</th>
                                <th>Approver Comment</th>
                                <th><button type="button" class="btn btn-dark-blue mb-1" data-toggle="modal" data-target="#studentModal">
    <i class="fas fa-book-medical"></i>Action
</button>
</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="RecordsRepeater" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("RequisitionItemId") %></td>
                                        <td><%# Eval("RequisitionItemName") %></td>
                                    <td><%# "MK " + string.Format("{0:N2}", Eval("Amount")) %></td>
                                        <td><%# Eval("Notes") %></td>
<td>
    <button type="button" class="btn btn-warning btn-sm" data-toggle="modal" data-target="#studentModal" onclick="openEditModal('<%# Eval("RequisitionItemId") %>')">
        <i class="fas fa-edit"></i>
    </button>
</td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
            </div>

        <div class="modal fade" id="studentModal" tabindex="-1" role="dialog" aria-labelledby="studentModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="studentModalLabel">Requisition Management</h5>
                    <button type="button" class="close" data-dismiss="modal" onclick="refreshPage()" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <iframe id="studentIframe" src="RequisitionEdit.aspx" frameborder="0"></iframe>
                </div>
            </div>
        </div>
    </div>

    <script>

         type="text/javascript">
            $(document).ready(function () {
                $('#successModal, #errorModal').on('hidden.bs.modal', function () {
                    __doPostBack('<%= RecordsRepeater.ClientID %>', '');
        });
    });
    
        function confirmDelete(RequisitionId) {
            if (confirm("Are you sure you want to delete this Item?")) {
                window.location.href = 'RequisitionItemAdd.aspx?RequisitionItemId=' + RequisitionId + '&mode=delete';
            }
        }


        function openEditModal(RequisitionId) {
            document.getElementById('studentIframe').src = 'RequisitionItemEdit.aspx?RequisitionItemId=' + RequisitionId;
        }



    </script>

</asp:Content>
