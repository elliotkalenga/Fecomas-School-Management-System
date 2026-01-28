<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="InvoicesPrint.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.InvoicesPrint" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

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
                #studentIframe1 {
            width: 100%;
            height: 450px;
        }

   .modal-dialog {
        max-width: 400px;
    }
        .text-success {
            color: green;
        }

#studentModal2 .modal-dialog {
    max-width: 70%;    /* Set the width to 70% of the screen */
    height: 85vh;      /* Set the height to 85% of the viewport height */
    margin: 0 auto;    /* Center the modal horizontally */
}

#studentModal2 .modal-body {
    height: calc(85vh - 56px); /* Adjust the body height, subtracting the header height */
    overflow-y: auto;  /* Allows scrolling if content overflows */
}

#studentModal2 iframe {
    width: 100%;
    height: 100%;   /* Make the iframe take the full height of the modal body */
}


    </style>
    <link rel="stylesheet" href="https://cdn.datatables.net/1.12.1/css/jquery.dataTables.min.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css">
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script src="https://cdn.datatables.net/1.12.1/js/jquery.dataTables.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jspdf/2.4.0/jspdf.umd.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jspdf-autotable/3.5.23/jspdf.plugin.autotable.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/xlsx/0.18.5/xlsx.full.min.js"></script>

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-2">

        <div class="card">
            <div class="card-header d-flex justify-content-between align-items-center" style="padding: 5px;">
                <div class="d-flex flex-wrap align-items-center" style="margin: 0;">
                    <button type="button" class="btn btn-success mb-1 mr-1" onclick="exportTableToExcel('students.csv')">
                        <i class="fas fa-file-csv"></i> Export CSV
                    </button>
                    <button type="button" class="btn btn-danger mb-1 mr-1" onclick="exportTableToPDF('students.pdf')">
                        <i class="fas fa-file-pdf"></i> Export PDF
                    </button>
                            <button type="button" class="btn btn-dark-blue mb-1 d-flex align-items-center"
                data-toggle="modal" data-target="#reportModal1">
            <i class="fas fa-file-alt mr-1"></i> Generate Fees Balance Report
            <i class="fas fa-download ml-1"></i>
        </button>
                </div>
                <div class="d-flex" style="margin: 0;">
<input type="text" id="searchInput" onkeyup="searchTable()" placeholder="Search...">

                </div>
            </div>
            <div class="card-body">
<div class="table-responsive">
<table id="studentsTable" class="table table-striped table-bordered" style="width:100%">
    <thead>
        <tr>
            <th>StudentNo</th>
            <th>Student</th>
            <th>Class</th>
            <th>Stream</th>
            <th>Term</th>
            <th>Last Term Balance</th>
            <th>This Term Invoice</th>
            <th>TotalBill</th>
            <th>Paid</th>
            <th>Balance</th>
            <th>Print</th>
        </tr>
    </thead>
    <tbody>
        <asp:Repeater ID="CollectionsRepeater" runat="server">
            <ItemTemplate>
                <tr class="student-row">
                    <td><%# Eval("StudentNo") %></td>
                    <td><%# Eval("Student") %></td>
                    <td><%# Eval("ClassName") %></td>
                    <td><%# Eval("StreamName") %></td>
                    <td><%# Eval("Term") %></td>
                    <td><%# Eval("PreviousTermBalance") %></td>
                    <td><%# Eval("ThisTermTotal") %></td>
                    <td><%# Eval("TotalFees") %></td>
                    <td><%# Eval("TotalCollected") %></td>
                    <td><%# Eval("Balance") %></td>
                    <td>
<button type="button" class="btn btn-success btn-sm" data-toggle="modal" data-target="#studentModal2" onclick="openEditModal('<%# Eval("StudentNo") %>')">
    <i class="fab fa-whatsapp"></i>Send Invoice
</button>
                        </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
        <tr id="noDataRow" style="display: none;">
            <td colspan="9" class="text-center">No Matching Records</td>
        </tr>
    </tbody>
</table>
</div>
                <div id="pagination" visible="false" class="pagination-container d-flex align-content-center"  ></div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="studentModal" tabindex="-1" role="dialog" aria-labelledby="studentModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="studentModalLabel">Fees Collection Transactions </h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <iframe id="studentIframe" src="InvoicePrintAdd.aspx" frameborder="0"></iframe>
                </div>
            </div>
        </div>
    </div>
            <div class="modal fade" id="reportModal1" tabindex="-1" role="dialog" aria-labelledby="reportModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg" role="document" style="max-width: 90vw; width: 90vw; height: 90vh;">
        <div class="modal-content" style="height: 100%;">
            <div class="modal-header">
                <h5 class="modal-title" id="reportModalLabel1">Fees Balance Reports</h5>
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body" style="height: calc(100% - 50px); padding: 0;">
                <iframe id="reportIframe1" src="FeesBalancesReport.aspx" frameborder="0" style="width: 100%; height: 100%; border: none;"></iframe>
            </div>
        </div>
    </div>
</div>

<div class="modal fade" id="studentModal2" tabindex="-1" role="dialog" aria-labelledby="studentModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body">
                <iframe id="studentIframe2" src="InvoicePrintAdd.aspx" frameborder="0"></iframe>
            </div>
        </div>
    </div>
</div>


<div class="modal fade" id="successModal" tabindex="-1" role="dialog" aria-labelledby="successModalLabel" aria-hidden="true">
    <div class="modal-dialog" CSClass="modal-dialog2" role="document" style="max-width: 400px;">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="successModalLabel">Success</h5>
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body text-success" style="border: 2px solid green;">
                Student deleted successfully!
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>


    <script>


        $(document).ready(function () {
            var rowsPerPage = 10; // Number of records per page
            var rows = $('.student-row');
            var rowsCount = rows.length;
            var pageCount = Math.ceil(rowsCount / rowsPerPage); // Total number of pages
            var numbers = $('#pagination');

            // Generate pagination controls
            for (var i = 1; i <= pageCount; i++) {
                numbers.append('<a href="#" class="page-link" data-page="' + i + '">' + i + '</a>');
            }

            // Show the first set of rows and hide the rest
            showPage(1);

            // Pagination controls click event
            $('.page-link').click(function (e) {
                e.preventDefault();
                var page = $(this).data('page');
                showPage(page);
            });

            // Function to show rows of a particular page
            function showPage(page) {
                rows.hide();
                var start = (page - 1) * rowsPerPage;
                var end = start + rowsPerPage;
                rows.slice(start, end).show();

                // Highlight the current page link
                $('.page-link').removeClass('active');
                $('.page-link[data-page="' + page + '"]').addClass('active');
            }
        });


        $(document).ready(function () {
            // Refresh the page when the modal is closed
            $('#studentModal').on('hidden.bs.modal', function () {
                location.reload();
            });
        });



        function searchTable() {
            var input = document.getElementById("searchInput");
            var filter = input.value.toUpperCase();
            var table = document.getElementById("studentsTable");
            var tr = table.getElementsByClassName("student-row");
            var visibleCount = 0;

            // Filter rows
            for (let i = 0; i < tr.length; i++) {
                tr[i].style.display = "none";
                let td = tr[i].getElementsByTagName("td");

                for (let j = 0; j < td.length; j++) {
                    let txtValue = td[j].textContent || td[j].innerText;
                    if (txtValue.toUpperCase().indexOf(filter) > -1) {
                        tr[i].style.display = "";
                        visibleCount++;
                        break;
                    }
                }
            }

            // Show or hide "No Data Found"
            var noDataRow = document.getElementById("noDataRow");
            noDataRow.style.display = (visibleCount === 0) ? "" : "none";

            // ---------------------------
            // 🔹 UPDATE PAGINATION BELOW
            // ---------------------------

            var rows = $('.student-row:visible');
            var rowsCount = rows.length;
            var pageCount = Math.ceil(rowsCount / rowsPerPage);

            var numbers = $('#pagination');
            numbers.empty();

            // Create pagination buttons
            for (let i = 1; i <= pageCount; i++) {
                numbers.append(
                    '<a href="#" class="page-link" data-page="' + i + '">' + i + '</a> '
                );
            }

            // Show first page after filtering
            showPage(1);

            // Pagination button event
            $('.page-link').off('click').on('click', function (e) {
                e.preventDefault();
                var page = $(this).data('page');
                showPage(page);
            });

            function showPage(page) {
                rows.hide();
                var start = (page - 1) * rowsPerPage;
                var end = start + rowsPerPage;
                rows.slice(start, end).show();

                $('.page-link').removeClass('active');
                $('.page-link[data-page="' + page + '"]').addClass('active');
            }
        }


        $(document).ready(function () {
            var urlParams = new URLSearchParams(window.location.search);
            if (urlParams.get('deleteSuccess') === 'true') {
                $('#successModal').modal('show');
                // Remove the query parameter after displaying the modal
                history.replaceState(null, '', window.location.pathname);
            }
        });



        function confirmDelete(FeesCollectionId) {
            if (confirm("Are you sure you want to reverse this transaction?")) {
                window.location.href = 'FeesCollectionAdd.aspx?FeesCollectionId=' + FeesCollectionId + '&mode=delete';
            }
        }

        function openEditModal(FeesCollectionId, mode) {
            var iframe = document.getElementById('studentIframe2');
            iframe.src = 'InvoicePrintAdd.aspx?StudentNo=' + FeesCollectionId;
        }



        function exportTableToCSV(filename) {
            var csv = [];
            var rows = document.querySelectorAll("#studentsTable tr");

            for (var i = 0; i < rows.length; i++) {
                var row = [], cols = rows[i].querySelectorAll("td:not(:last-child), th:not(:last-child)");

                for (var j = 0; j < cols.length; j++) {
                    var cellValue = cols[j].innerText.trim();

                    // Wrap numbers like 20,000 in quotes so Excel reads them correctly
                    if (cellValue.includes(",")) {
                        cellValue = '"' + cellValue + '"';
                    }

                    row.push(cellValue);
                }

                csv.push(row.join(","));
            }

            downloadCSV(csv.join("\n"), filename);
        }


        function exportTableToExcel() {
            var table = document.getElementById("studentsTable");

            // Convert HTML table to Excel worksheet
            var wb = XLSX.utils.book_new();
            var ws = XLSX.utils.table_to_sheet(table);

            // 🔹 1. Make header row bold
            const range = XLSX.utils.decode_range(ws['!ref']);
            for (let C = range.s.c; C <= range.e.c; ++C) {
                let cellAddress = XLSX.utils.encode_cell({ r: 0, c: C });
                if (!ws[cellAddress]) continue;

                if (!ws[cellAddress].s) ws[cellAddress].s = {};
                ws[cellAddress].s.font = { bold: true };
            }

            // 🔹 2. Auto-fit column widths
            const colWidths = [];
            for (let C = range.s.c; C <= range.e.c; ++C) {
                let maxWidth = 10;
                for (let R = range.s.r; R <= range.e.r; ++R) {
                    let cellAddress = XLSX.utils.encode_cell({ r: R, c: C });
                    let cell = ws[cellAddress];
                    if (!cell) continue;
                    let text = String(cell.v);
                    maxWidth = Math.max(maxWidth, text.length);
                }
                colWidths.push({ wch: maxWidth + 2 });
            }
            ws['!cols'] = colWidths;

            // Add worksheet to Excel file
            XLSX.utils.book_append_sheet(wb, ws, "Report");

            // Save file
            XLSX.writeFile(wb, "FeesReport.xlsx");
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



    </script>
</asp:Content>

