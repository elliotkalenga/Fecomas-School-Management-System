<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="SchemesApprove.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.SchemesApprove" %>
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

#studentModal .modal-dialog {
  max-width: 85vw;
  width: 80vw;
  height: 80vh;
  margin: 10vh auto;
}

#studentModal .modal-content {
  height: 100%;
}

#studentIframe {
  width: 100%;
  height: calc(85vh - 120px); /* adjust the height to fit the modal header and footer */
  border: none;
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
    padding: 10px; /* Add spacing around the content */
    box-sizing: border-box; /* Include padding in the width calculation */
}

.card {
    max-width: 1000px; /* Limit the card's width */
    margin: 0 auto; /* Center the card within the container */
    box-shadow: 0px 4px 6px rgba(0, 0, 0, 0.1); /* Optional: Add a subtle shadow for better aesthetics */
    border-radius: 8px; /* Optional: Add rounded corners */
    padding: 20px; /* Optional: Add internal padding for spacing */
}


        .text-success {
            color: green;
        }
    
    </style>
    <link rel="stylesheet" href="https://cdn.datatables.net/1.12.1/css/jquery.dataTables.min.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css">
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script src="https://cdn.datatables.net/1.12.1/js/jquery.dataTables.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jspdf/2.4.0/jspdf.umd.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jspdf-autotable/3.5.23/jspdf.plugin.autotable.min.js"></script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<div class="container mt-2">
    <div class="card">
<div class="card-header d-flex justify-content-between align-items-center p-2">
    <div class="d-flex flex-wrap align-items-center gap-3">
        <button type="button" class="btn btn-dark-blue mb-1" data-toggle="modal" data-target="#studentModal">
            <i class="fas fa-plus"></i> Add New Scheme Record
        </button>
    </div>
    <div class="d-flex flex-wrap align-items-center gap-3">
        <button type="button" class="btn btn-dark-blue mb-1" data-toggle="modal" data-target="#reportModal">
            <i class="fas fa-file-alt"></i> Generate Schemes Of Work Reports
        </button>
    </div>

    <div class="d-flex flex-wrap gap-2">
        <input type="text" id="searchInput" class="form-control" placeholder="Search for Scheme Record..." style="height: 30px;" onkeyup="searchTable()">
    </div>
</div>            <div class="table-responsive">
                <table id="studentsTable" class="table table-striped table-bordered" style="width:100%">
                    <thead>
                        <tr>
                            <th>Week & Dates</th>
                            <th>Subject</th>
                            <th>Topic</th>
                            <th>Lessons Count</th>
                            <th>Success Criteria</th>
                            <th>Planned Activities</th>
                            <th>Resources</th>
                            <th>OutCome</th>
                            <th>References</th>
                            <th>Remarks</th>
                            <th>Teacher</th>
                            <th>Status</th>
                            <th>Edit</th>
                            <th>Delete</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="RecordsRepeater" runat="server">
                            <ItemTemplate>
                                <tr class="student-row">
                                    <td><%# Eval("WeekNo") %> - <%# Eval("Term") %></td>
                                    <td><%# Eval("Subject") %></td>
                                    <td><%# Eval("Topic") %></td>
                                    <td><%# $"{Eval("Lessons")} Lessons" %></td>
<td><%# Regex.Replace(Eval("LearningObjectives").ToString(), @"[\n.]+", "<br>") %></td>
<td><%# Regex.Replace(Eval("LearningOutcome").ToString(), @"[\n.]+", "<br>") %></td>
<td><%# Regex.Replace(Eval("Resources").ToString(), @"[\n.]+", "<br>") %></td>
<td><%# Regex.Replace(Eval("SchemeEvaluation").ToString(), @"[\n.]+", "<br>") %></td>
<td><%# Regex.Replace(Eval("References").ToString(), @"[\n.]+", "<br>") %></td>
<td><%# Regex.Replace(Eval("Remarks").ToString(), @"[\n.]+", "<br>") %></td>

                                    <td><%# Eval("CreatedBy") %></td>
<td>
    <%# "<span style='color: " + 
        (Eval("CompleteStatus").ToString() == "Completed" ? "#008000" : 
         Eval("CompleteStatus").ToString() == "In Progress" ? "#FFA500" : "#FF0000") + "'>" + 
        "<i class='fa " + 
        (Eval("CompleteStatus").ToString() == "Completed" ? "fa-check-circle" : 
         Eval("CompleteStatus").ToString() == "In Progress" ? "fa-spinner fa-spin" : "fa-times-circle") + "'></i> " + 
        Eval("CompleteStatus") + 
        "</span>" %>
</td>                                    <td>
                                        <button type="button" class="btn btn-success btn-sm" data-toggle="modal" data-target="#studentModal" onclick="openEditModal('<%# Eval("SchemeId") %>')">
                                            <i class="fas fa-edit"></i> 
                                        </button></td><td>
                                        <button type="button" class="btn btn-danger btn-sm" onclick="confirmDelete('<%# Eval("SchemeId") %>')">
                                            <i class="fas fa-trash"></i> 
                                        </button>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        <tr id="noDataRow" style="display: none;">
                            <td colspan="7" class="text-center">No Matching Records</td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div id="pagination" visible="false" class="pagination-container d-flex align-content-center"></div>
        </div>
    </div>
</div>

    <!-- Report Modal -->
<div class="modal fade" id="reportModal" tabindex="-1" role="dialog" aria-labelledby="reportModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg" role="document" style="max-width: 90vw; width: 90vw; height: 90vh;">
        <div class="modal-content" style="height: 100%;">
            <div class="modal-header">
                <h5 class="modal-title" id="reportModalLabel">Schemes of Work Reports</h5>
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body" style="height: calc(100% - 50px); padding: 0;">
                <iframe id="reportIframe" src="Schemesofworkreport.aspx" frameborder="0" style="width: 100%; height: 100%; border: none;"></iframe>
            </div>
        </div>
    </div>
</div>
<!--  Modal -->
<div class="modal fade" id="studentModal" tabindex="-1" role="dialog" aria-labelledby="studentModalLabel" aria-hidden="true">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="studentModalLabel">Schemes of Work Management</h5>
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body">
                <iframe id="studentIframe" src="SchemesOfWorkAdd.aspx" frameborder="0"></iframe>
            </div>
        </div>
    </div>
</div>

<!-- Success Modal -->
<div class="modal fade" id="successModal" tabindex="-1" role="dialog" aria-labelledby="successModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-dialog2" role="document" style="max-width: 400px;">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="successModalLabel">Success</h5>
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body text-success" style="border: 2px solid green;">
                Scheme ecord deleted successfully!
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
            // Refresh the page when the modal is closed
            $('#reportModal').on('hidden.bs.modal', function () {
                location.reload();
            });

        });



    


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

            // Update pagination
            updatePagination();
        }

        function updatePagination() {
            var rows = $('.student-row:visible');
            var rowsCount = rows.length;
            var pageCount = Math.ceil(rowsCount / rowsPerPage);
            var numbers = $('#pagination');
            numbers.empty();

            for (var i = 1; i <= pageCount; i++) {
                numbers.append('<a href="#" class="page-link" data-page="' + i + '">' + i + '</a> ');
            }

            // Show the first set of rows and hide the rest
            showPage(1);

            // Pagination controls click event
            $('.page-link').click(function (e) {
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


    
            function confirmDelete(SchemeId) {
            if (confirm("Are you sure you want to delete this Schemes of work Record?")) {
                window.location.href = 'SchemesOfWorkAdd.aspx?SchemeId=' + SchemeId + '&mode=delete';
            }
        }

        function openEditModal(SchemeId, mode) {
            var iframe = document.getElementById('studentIframe');
            iframe.src = 'SchemesOfWorkAdd.aspx?SchemeId=' + SchemeId;
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

    </script>
</asp:Content>
