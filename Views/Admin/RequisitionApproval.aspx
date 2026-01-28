<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Master.Master" AutoEventWireup="true" CodeBehind="RequisitionApproval.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.RequisitionApproval" %>
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
    <!-- Purpose -->
    <div class="form-group col-md-4">
        <label for="txtPurpose">Purpose</label>
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-ui-checks"></i></span>
            </div>
            <asp:TextBox ID="txtPurpose" runat="server" CssClass="form-control" 
                ReadOnly="true" placeholder="Enter Requisition Purpose"></asp:TextBox>
        </div>
    </div>

    <!-- Requisition Category -->
    <div class="form-group col-md-4">
        <label for="ddlCategory">Requisition Category</label>
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-cash-stack"></i></span>
            </div>
            <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-control" Enabled="false">
            </asp:DropDownList>
        </div>
    </div>

    <!-- Cost Center (Budget) -->
    <div class="form-group col-md-4">
        <label for="ddlBudget">Cost Center (Budget)</label>
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-cash-stack"></i></span>
            </div>
            <asp:DropDownList ID="ddlBudget" runat="server" CssClass="form-control" Enabled="false">
            </asp:DropDownList>
        </div>
    </div>
</div>
<div class="form-row">

        <!-- Requisition ID -->
    <div class="form-group col-md-2">
        <label for="txtRequisitionId">Requisition ID</label>
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-hash"></i></span>
            </div>
            <asp:TextBox ID="txtRequisitionId"  ReadOnly="true"  required runat="server" CssClass="form-control" placeholder="Enter Requisition ID"></asp:TextBox>
        </div>
    </div>


        <div class="form-group col-md-4">
        <label for="ddlStatus">Requisition Status</label>
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-cash-stack"></i></span>
            </div>
            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control">
            </asp:DropDownList>
        </div>
    </div>

    <!-- Item Description (Wider field) -->
    <div class="form-group col-md-6">
        <label for="txtDescription">Approver Notes</label>
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-book"></i></span>
            </div>
            <asp:TextBox 
                ID="txtDescription" 
                runat="server" 
                CssClass="form-control" 
                TextMode="MultiLine" 
                Rows="2" 
                placeholder="Enter Enter Approver's Notes">
            </asp:TextBox>
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
                                <th>#</th>
                                <th>Item</th>
                                <th>Amount</th>
                                <th>Approver Comment</th>
                                <th>Action</th>
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
    <button type="button" class="btn btn-success btn-sm" data-toggle="modal" data-target="#studentModal" onclick="approveRequisition('<%# Eval("RequisitionItemId") %>')">
        <i class="fas fa-check"></i> Approver Comment
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
                    <h5 class="modal-title" id="studentModalLabel">Requisition Approval Management</h5>
                    <button type="button" class="close" data-dismiss="modal" onclick="refreshPage()" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <iframe id="studentIframe" src="RequisitionItemApprove.aspx" frameborder="0"></iframe>
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
    


        function approveRequisition(RequisitionId) {
            document.getElementById('studentIframe').src = 'RequisitionItemApprove.aspx?RequisitionItemId=' + RequisitionId;
        }



    </script>

</asp:Content>
