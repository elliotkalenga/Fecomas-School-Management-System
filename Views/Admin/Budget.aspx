<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="Budget.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.Budget" %>
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
            height: 500px;
        }

#studentModal3 .modal-dialog {
    max-width: 99vw; /* 75% of viewport width */
    height: 90vh; /* 75% of viewport height */
}

#studentModal3 .modal-content {
    height: 100%;
}

#studentIframe3 {
    width: 100%;
    height: 100%;
}

                #studentIframe2 {
            width: 100%;
            height: 500px;
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
        <!-- Add content here if needed -->
    </div>
        <div class="d-flex flex-wrap gap-2">
        <div class="input-group" style="height: 30px;">
            <input type="text" id="searchInput" class="form-control" placeholder="Search for Budget...">
            <button type="button" class="btn btn-info" onclick="searchTable()">
                <i class="fas fa-search"></i> Search
            </button>
        </div>
    </div>


        <div class="d-flex flex-wrap gap-2">
        <div class="input-group" style="height: 30px;">
            <input type="text" id="searchInput2" class="form-control" placeholder="Search for BudgetItem...">
            <button type="button" class="btn btn-info" onclick="searchTable()">
                <i class="fas fa-search"></i> Search
            </button>
        </div>
    </div>
</div>
<div class="card-body">
    <div>                        <button type="button" class="btn btn-dark-blue mb-1" data-toggle="modal" data-target="#studentModal">
                            <i class="fas fa-book-medical"></i> Add New Budget
                        </button></div>
    <div class="table-responsive">
        <table id="studentsTable" class="table table-striped table-bordered" style="width:100%">
            <thead>
                <tr>
                    <th>Reports</th>
                    <th>#</th>
                    <th>Budget Name</th>
                    <th>Term</th>
                    <th>Total Income</th>
                    <th>Budgeted Amount</th>
                    <th>Contingency Rate</th>
                    <th>Contingency Amount</th>
                    <th>BudgetedPlusContingency</th>
                    <th>Spent</th>
                    <th>Variance</th>
                    <th>Budget Status</th>
                    <th>Active Status</th>

                    <th>
                        Edit
                    </th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater ID="RecordsRepeater" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <button type="button" class="btn btn-dark-blue btn-sm" 
                                        data-toggle="modal" 
                                        data-target="#studentModal3" 
                                        onclick="openEditModal3('<%# Eval("BudgetId") %>')">
                                    <i class="fas fa-print"></i> 
                                </button>
                            </td>
                            <td><%# Eval("BudgetId") %></td>
                            <td><%# Eval("BudgetName") %></td>
                            <td><%# Eval("Term") %></td>
                            <td><%#$"K{Eval("TotalIncome")}"  %></td>
                            <td><%#$"K{Eval("Amount")}"  %></td>
                            <td><%#$"{Eval("ContingencyPercent")}%"  %></td>
                            <td><%#$"K{Eval("Contingency")}"  %></td>
                            <td><%# $"K{Eval("AmountPlusContingency")}" %></td>
                            <td><%# $"K{ Eval("Spent")}" %></td>
                            <td><%# $"K{Eval("Variance")}"  %></td>
<td>
    <span style="color: <%# Eval("BudgetStatus").ToString() == "Within Budget" ? "green" : 
                          Eval("BudgetStatus").ToString() == "Over Spending" ? "red" : 
                          Eval("BudgetStatus").ToString() == "On Budget" ? "blue" : "black" %>;">
        <i class='<%# Eval("BudgetStatus").ToString() == "Within Budget" ? "fas fa-check-circle" : 
                    Eval("BudgetStatus").ToString() == "Over Spending" ? "fas fa-exclamation-circle" : 
                    Eval("BudgetStatus").ToString() == "On Budget" ? "fas fa-dot-circle" : "fas fa-info-circle" %>'></i>
        <%# Eval("BudgetStatus") %>
    </span>
</td>                            <td>
                                <span style="color: <%# Eval("Status").ToString() == "Inactive" ? "chocolate" : 
                                              Eval("Status").ToString() == "Active" ? "darkgreen" : 
                                              Eval("Status").ToString() == "Rejected" ? "red" : "black" %>;">
                                    <i class='<%# Eval("Status").ToString() == "Inactive" ? "fas fa-hourglass-half" : 
                                                Eval("Status").ToString() == "Active" ? "fas fa-check-circle" : 
                                                Eval("Status").ToString() == "Rejected" ? "fas fa-times-circle" : "fas fa-info-circle" %>'></i>
                                    <%# Eval("Status") %>
                                </span>
                            </td>
                            <td>
                                <button type="button" class="btn btn-success btn-sm" data-toggle="modal" data-target="#studentModal" onclick="openEditModal2('<%# Eval("BudgetId") %>')">
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

<div class="card">
    <div>                            <button type="button" class="btn btn-dark-blue mb-1" data-toggle="modal" data-target="#studentModal2">
    <i class="fas fa-book-medical"></i> Prepare Budget Items
</button>
</div>
    <div class="card-body">
        <div class="table-responsive">
            <table id="studentsTable2" class="table table-striped table-bordered" style="width:100%">
                <thead>
                    <tr>
                        <th>ItemNo</th>
                        <th>Item</th>
                        <th>Budget</th>
                        <th>Budgeted Amount</th>
                        <th>Contingency</th>
                        <th>BudgetedPlusContingency</th>
                        <th>Spent</th>
                        <th>Variance</th>
                        <th>Budget Status</th>
                        
                        <th>
                            Edit
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="RecordsRepeater2" runat="server">
                        <ItemTemplate>
                            <tr>

                                <td><%# Eval("BudgetItemId") %></td>
                                <td><%# Eval("ItemName") %></td>
                                <td><%# Eval("BudgetName") %></td>
                                <td><%# $"K{Eval("Amount")}" %></td>
                                <td><%#$"K{Eval("Contingency")}" %></td>
                                <td><%# $"K{Eval("AmountPlusContingency")}" %></td>
                                <td><%# $"K{Eval("Spent")}" %></td>
                                <td><%# $"K{Eval("Variance")}" %></td>
<td>
    <span style="color: <%# Eval("BudgetStatus").ToString() == "Within Budget" ? "green" : 
                          Eval("BudgetStatus").ToString() == "Over Spending" ? "red" : 
                          Eval("BudgetStatus").ToString() == "On Budget" ? "blue" : "black" %>;">
        <i class='<%# Eval("BudgetStatus").ToString() == "Within Budget" ? "fas fa-check-circle" : 
                    Eval("BudgetStatus").ToString() == "Over Spending" ? "fas fa-exclamation-circle" : 
                    Eval("BudgetStatus").ToString() == "On Budget" ? "fas fa-dot-circle" : "fas fa-info-circle" %>'></i>
        <%# Eval("BudgetStatus") %>
    </span>
</td>                                <td>
                                    <button type="button" class="btn btn-success btn-sm" data-toggle="modal" data-target="#studentModal2" onclick="openEditModal('<%# Eval("BudgetItemId") %>')">
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
</div>
    </div>

         
    </div>





    <div class="modal fade" id="studentModal3" tabindex="-1" role="dialog" aria-labelledby="studentModalLabel3" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="studentModalLabel3">Budgeting Reports</h5>
                    <button type="button" class="close" data-dismiss="modal" onclick="refreshPage()" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <iframe id="studentIframe3" src="BudgetReports.aspx" frameborder="0"></iframe>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="studentModal" tabindex="-1" role="dialog" aria-labelledby="studentModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="studentModalLabel">Budget Management</h5>
                    <button type="button" class="close" data-dismiss="modal" onclick="refreshPage()" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <iframe id="studentIframe" src="BudgetAdd.aspx" frameborder="0"></iframe>
                </div>
            </div>
        </div>
    </div>

        <div class="modal fade" id="studentModal2" tabindex="-1" role="dialog" aria-labelledby="studentModalLabel2" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="studentModalLabel2">Budget Items Preparation</h5>
                    <button type="button" class="close" data-dismiss="modal" onclick="refreshPage()" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <iframe id="studentIframe2" src="BudgetItemsAdd.aspx" frameborder="0"></iframe>
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
            $('#studentModal2').on('hidden.bs.modal', function () {
                location.reload();
            });
        });




        function openEditModal2(BudgetId) {
            document.getElementById('studentIframe').src = 'BudgetAdd.aspx?BudgetId=' + BudgetId;
        }

        function confirmDelete2(BudgetId) {
            if (confirm("Are you sure you want to delete this Budget?")) {
                window.location.href = 'BudgetAdd.aspx?BudgetId=' + BudgetId + '&mode=delete';
            }
        }



        function openEditModal3(BudgetId) {
            document.getElementById('studentIframe3').src = 'BudgetReports.aspx?BudgetId=' + BudgetId;
        }
        function openEditModal(BudgetId) {
            document.getElementById('studentIframe2').src = 'BudgetItemsAdd.aspx?BudgetItemId=' + BudgetId;
        }

        function confirmDelete(BudgetId) {
            if (confirm("Are you sure you want to delete this BudgetItem?")) {
                window.location.href = 'BudgetItemsAdd.aspx?BudgetItemId=' + BudgetId + '&mode=delete';
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

        function searchTable() {
            var input, filter, table, tr, td, i, j, txtValue, visibleCount;
            input = document.getElementById("searchInput");
            filter = input.value.toUpperCase();
            table = document.getElementById("studentsTable");
            tr = table.getElementsByTagName("tr");
            visibleCount = 0;

            for (i = 1; i < tr.length; i++) {
                tr[i].style.display = "none";
                td = tr[i].getElementsByTagName("td");
                for (j = 0; j < td.length; j++) {
                    if (td[j]) {
                        txtValue = td[j].textContent || td[j].innerText;
                        if (txtValue.toUpperCase().indexOf(filter) > -1) {
                            tr[i].style.display = "";
                            visibleCount++;
                            break;
                        }
                    }
                }
            }

            var noDataRow = document.getElementById("noDataRow");
            if (visibleCount === 0) {
                noDataRow.style.display = "";
            } else {
                noDataRow.style.display = "none";
            }
        }

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








        function exportTableToCSV(filename) {
            var csv = [];
            var rows = document.querySelectorAll("#studentsTable2 tr");

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
                html: '#studentsTable2',
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
            $("#searchInput2").on("keyup", function () {
                var value = $(this).val().toLowerCase();
                $("#studentsTable2 tbody tr").filter(function () {
                    $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
                });
            });
        });


        $(document).ready(function () {
            $('#studentsTable2').DataTable({
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
