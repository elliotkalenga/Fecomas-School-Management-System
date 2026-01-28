<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="InvoiceDetails.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.InvoiceDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .pagination-container {
            display: flex;
            flex-wrap: wrap;
            justify-content: center;
            margin-top: 20px;
            position: relative;
            width: 100%;
            display: none !important;
        }
        .table {
            font-size: 12px;
        }
        .table th, .table td {
            font-size: inherit;
        }
        .page-link {
            margin: 5px;
            padding: 8px 12px;
            border: 1px solid #007bff;
            color: #007bff;
            text-decoration: none;
            border-radius: 3px;
            font-size: 14px;
            min-width: 40px;
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
            width: 100%;
            overflow-x: auto;
        }
        .card {
            width: 100%;
        }
        .container {
            max-width: 1200px;
            margin: 0 auto;
            padding: 15px;
            box-sizing: border-box;
        }
        .card {
            max-width: 1000px;
            margin: 0 auto;
            box-shadow: 0px 4px 6px rgba(0, 0, 0, 0.1);
            border-radius: 8px;
            padding: 20px;
        }
        .modal-dialog {
            max-width: 400px;
        }
        .text-success {
            color: green;
        }

        .modal-dialog {
    max-width: 75% !important; /* Set modal width to 3/4 of the page */
    width: 75% !important;
    height: 90vh; /* Adjust height relative to viewport */
}

#studentIframe1,
#studentIframe2,
#studentIframe3 {
    width: 100%;
    height: 60vh; /* Adjust height relative to viewport */
}

    </style>
    <link rel="stylesheet" href="https://cdn.datatables.net/1.12.1/css/jquery.dataTables.min.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css">
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script src="https://cdn.datatables.net/1.12.1/js/jquery.dataTables.min.js"></script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mt-2">
        <div class="card">
<div class="card-header d-flex justify-content-between align-items-center p-2">
    <div class="d-flex flex-wrap align-items-center gap-2">
    </div>
    <div class="d-flex flex-wrap gap-2">
        
<div class="d-flex justify-content-between align-items-center w-100 mb-2">
    <button type="button" class="btn btn-dark-blue" data-toggle="modal" data-target="#studentModal">
        <i class="fas fa-plus-circle"></i> Add New Invoice
    </button>

    <div class="input-group" style="width: 300px;">
        <input type="text" id="searchInput" class="form-control" placeholder="Search for Invoice">
        <div class="input-group-append">
            <button type="button" class="btn btn-info" onclick="searchTable()">
                <i class="fas fa-search"></i> Search
            </button>
        </div>
    </div>
</div>
    </div>
</div>
            <div class="card-body">
                <div class="table-responsive">
                    <table id="studentsTable" class="table table-striped table-bordered" style="width:100%">
                        <thead>
                            <tr>
                                <th>Reports</th>
                                <th>InvoiceNumber #</th>
                                <th>Invoice Description</th>
                                <th>Amount</th>
                                <th>Term</th>
                                <th>School</th>
                                <th>Status</th>
                                <th>Edit</th>
                                <th>Details</th>
                                <th>Approve</th>

                                                               

                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="RecordsRepeater" runat="server">
                                <ItemTemplate>
                                    <tr>                            <td>
                                <button type="button" class="btn btn-dark-blue btn-sm" 
        data-toggle="modal" 
        data-target="#studentModal3" 
        onclick="openEditModal3('<%# Eval("Term") %>', '<%# Eval("InvoiceId") %>')">
    <i class="fas fa-print"></i> 
</button>                               </td>
                                        <td><%# Eval("InvoiceNumber") %></td>
                                        <td><%# Eval("InvoiceDescription") %></td>
                                        <td><%# Eval("Amount", "{0:N2}") %></td>
                                        <td><%# Eval("Term") %></td>
                                        <td><%# Eval("SchoolName") %></td>
<td>
    <span style='color: <%# Eval("Status").ToString() == "Pending" ? "chocolate" : 
                          Eval("Status").ToString() == "Approved" ? "darkgreen" : 
                          Eval("Status").ToString() == "Rejected" ? "red" : "red" %>;'>

        <i class='<%# Eval("Status").ToString() == "Pending" ? "fas fa-hourglass-half" : 
                    Eval("Status").ToString() == "Paid" ? "fas fa-check-circle" : 
                    Eval("Status").ToString() == "Rejected" ? "fas fa-times-circle" : "fas fa-info-circle" %>'></i>

        <%# Eval("Status") %>
    </span>
</td>
                                        <td>
                                                <button type="button" class="btn btn-warning btn-sm" data-toggle="modal" data-target="#studentModal" onclick="openEditModal('<%# Eval("InvoiceId") %>')">
        <i class="fas fa-edit"></i>
    </button>

                                        </td>
<td>
    <button type="button" class="btn btn-primary btn-sm" data-toggle="modal" data-target="#studentModal2" onclick="openDetailsModal('<%# Eval("InvoiceId") %>')">
        <i class="fas fa-info-circle"></i>Add Details
    </button></td><td>
    <button type="button" class="btn btn-success btn-sm" data-toggle="modal" data-target="#studentModal1" onclick="approveExpense('<%# Eval("InvoiceId") %>')">
        <i class="fas fa-check"></i> Pay
    </button>
</td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="studentModal3" tabindex="-1" role="dialog" aria-labelledby="studentModalLabel3" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="studentModalLabel3">Invoice Management</h5>
                    <button type="button" class="close" data-dismiss="modal" onclick="refreshPage()" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <iframe id="studentIframe3" src="InvoiceReport.aspx" frameborder="0"></iframe>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="studentModal" tabindex="-1" role="dialog" aria-labelledby="studentModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="studentModalLabel">Invoice Management</h5>
                    <button type="button" class="close" data-dismiss="modal" onclick="refreshPage()" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <iframe id="studentIframe" src="InvoiceDetailsAdd.aspx" frameborder="0"></iframe>
                </div>
            </div>
        </div>
    </div>

        <div class="modal fade" id="studentModal1" tabindex="-1" role="dialog" aria-labelledby="studentModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="studentModalLabel1">Invoice Payment Management</h5>
                    <button type="button" class="close" data-dismiss="modal" onclick="refreshPage()" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <iframe id="studentIframe1" src="PayInvoice.aspx" frameborder="0"></iframe>
                </div>
            </div>
        </div>
    </div>


            <div class="modal fade" id="studentModal2" tabindex="-1" role="dialog" aria-labelledby="studentModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="studentModalLabel2">Invoice Items Management</h5>
                    <button type="button" class="close" data-dismiss="modal" onclick="refreshPage()" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <iframe id="studentIframe2" src="InvoiceItems.aspx" frameborder="0"></iframe>
                </div>
            </div>
        </div>
    </div>


    <script>
        $(document).ready(function () {
            $('#studentModal').on('hidden.bs.modal', function () {
                location.reload();
            });
        });

        $(document).ready(function () {
            $('#studentModal3').on('hidden.bs.modal', function () {
                location.reload();
            });
        });

        $(document).ready(function () {
            $('#studentModal1').on('hidden.bs.modal', function () {
                location.reload();
            });
        });

        $(document).ready(function () {
            $('#studentModal2').on('hidden.bs.modal', function () {
                location.reload();
            });
        });

        function approveExpense(ExpenseId) {
            document.getElementById('studentIframe1').src = 'PayInvoice.aspx?InvoiceId=' + ExpenseId;
        }

        function openDetailsModal(ExpenseId) {
            document.getElementById('studentIframe2').src = 'InvoiceItems.aspx?InvoiceId=' + ExpenseId;
        }



        function openEditModal3(term, requisitionId) {
            document.getElementById('studentIframe3').src = 'InvoiceReport.aspx?Term=' + encodeURIComponent(term) + '&InvoiceId=' + encodeURIComponent(requisitionId);
        }
        function openEditModal(ExpenseId) {
            document.getElementById('studentIframe').src = 'InvoiceDetailsAdd.aspx?InvoiceId=' + ExpenseId;
        }


        function confirmDelete(ExpenseId) {
            if (confirm("Are you sure you want to delete this Book?")) {
                window.location.href = 'InvoicedetailsAdd.aspx?InvoiceId=' + ExpenseId + '&mode=delete';
            }
        }
        function exportTableToCSV(filename) {
            var csv = [];
            var rows = document.querySelectorAll("#studentsTable tr");

            for (var i = 0; i < rows.length; i++) {
                var row = [], cols = rows[i].querySelectorAll("td:not(:last-child), th:not(:last-child)");

                for (var j = 0; j < cols.length; j++)
                    row.push(cols[j].innerText);

                csv.push(row.join(","));
            }

            // Download CSV file
            downloadCSV(csv.join("\n"), filename);
        }

        function downloadCSV(csv, filename) {
            var csvFile;
            var downloadLink;

            // CSV file
            csvFile = new Blob([csv], { type: "text/csv" });

            // Download link
            downloadLink = document.createElement("a");

            // File name
            downloadLink.download = filename;

            // Create a link to the file
            downloadLink.href = window.URL.createObjectURL(csvFile);

            // Hide download link
            downloadLink.style.display = "none";

            // Add the link to DOM
            document.body.appendChild(downloadLink);

            // Click download link
            downloadLink.click();
        }

        function exportTableToPDF(filename) {
            const { jsPDF } = window.jspdf;
            const doc = new jsPDF();

            doc.autoTable({
                html: '#studentsTable',
                columnStyles: {
                    5: { cellWidth: 0 } // Hide the last column (Actions)
                },
                didParseCell: function (data) {
                    if (data.column.index === 5) {
                        data.cell.styles.cellWidth = 0;
                    }
                }
            });

            // Save the PDF
            doc.save(filename);
        }

        $(document).ready(function () {
            $("#searchInput").on("keyup", function () {
                var value = $(this).val().toLowerCase();
                $("#studentsTable tbody tr").filter(function () {
                    $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
                });
            });
        });

        $(document).ready(function () {
            $('#studentsTable').DataTable({
                "paging": true, // Enable pagination
                "searching": true, // Enable built-in search
                "info": true, // Enable table information display
                "lengthChange": false, // Disable changing the number of records per page
                "pageLength": 10, // Number of records per page
                "language": {
                    "emptyTable": "No Matching Records"
                }
            });
        });
    </script>
</asp:Content>
