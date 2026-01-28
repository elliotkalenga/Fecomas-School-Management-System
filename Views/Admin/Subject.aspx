<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="Subject.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.Subject" %>
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
                    <button type="button" class="btn btn-dark-blue mb-1" data-toggle="modal" data-target="#studentModal">
                        <i class="fas fa-user-plus"></i> Add New Subject
                    </button>
                    <button type="button" class="btn btn-success mb-1" onclick="exportTableToCSV('students.csv')">
                        <i class="fas fa-file-csv"></i> Export CSV
                    </button>
                    <button type="button" class="btn btn-danger mb-1" onclick="exportTableToPDF('students.pdf')">
                        <i class="fas fa-file-pdf"></i> Export PDF
                    </button>
                </div>
                <div class="d-flex flex-wrap gap-2">
                    <input type="text" id="searchInput" class="form-control" placeholder="Search for Subjects..." style="height: 30px;">
                    <button type="button" class="btn btn-info" onclick="searchTable()">
                        <i class="fas fa-search"></i> Search
                    </button>
                </div>
            </div>
            <div class="card-body">
                <div class="table-responsive">
                    <table id="studentsTable" class="table table-striped table-bordered" style="width:100%">
                        <thead>
                            <tr>
                                <th>SubjectCode</th>
                                <th>SubjectName</th>
                                <th>Description</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="ScoresRepeater" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("SubjectCode") %></td>
                                        <td><%# Eval("SubjectName") %></td>
                                        <td><%# Eval("Description") %></td>
                                        <td>
                                            <button type="button" class="btn btn-success btn-sm" data-toggle="modal" data-target="#studentModal" onclick="openEditModal('<%# Eval("SubjectId") %>')">
                                                <i class="fas fa-edit"></i> 
                                            </button>
                                            <button type="button" class="btn btn-danger btn-sm" onclick="confirmDelete('<%# Eval("SubjectId") %>')">
                                                <i class="fas fa-trash"></i> 
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

    <!-- Exam Management Modal -->
    <div class="modal fade" id="studentModal" tabindex="-1" role="dialog" aria-labelledby="studentModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="studentModalLabel">Subjects Management</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <iframe id="studentIframe" src="SubjectAdd.aspx" frameborder="0"></iframe>
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
                    Subject deleted successfully!
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <script>
        $(document).ready(function () {
            // Initialize DataTable
            $('#studentsTable').DataTable({
                "paging": true, // Enable pagination
                "searching": true, // Enable built-in search
                "info": true, // Enable table information display
                "lengthChange": false, // Disable changing the number of records per page
                "pageLength": 10, // Number of records per page (set to 5 or whatever value you prefer)
                "language": {
                    "emptyTable": "No Matching Records"
                }
            });
        });

        // Search functionality
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

        // Export Table to CSV
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

        // Helper function to trigger CSV download
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

        // Export Table to PDF
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

        // Delete Confirmation
        function confirmDelete(SubjectId) {
            if (confirm("Are you sure you want to delete this Subject?")) {
                window.location.href = 'SubjectAdd.aspx?SubjectId=' + SubjectId + '&mode=delete';
            }
        }

        // Open Edit Modal
        function openEditModal(SubjectId) {
            var iframe = document.getElementById('studentIframe');
            iframe.src = 'SubjectAdd.aspx?SubjectId=' + SubjectId;
        }

        $(document).ready(function () {
            var urlParams = new URLSearchParams(window.location.search);
            if (urlParams.get('deleteSuccess') === 'true') {
                $('#successModal').modal('show');
                // Remove the query parameter after displaying the modal
                history.replaceState(null, '', window.location.pathname);
            }
        });
    </script>
</asp:Content>
