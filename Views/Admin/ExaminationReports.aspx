<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="ExaminationReports.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.ExaminationReports" %>
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

        #studentIframe1 {
            width: 100%;
            height: 450px;
        }
        #studentIframe {
            width: 100%;
            height: 450px;
        }


                #studentIframe2 {
            width: 100%;
            height: 450px;
        }

                                #studentIframe4 {
            width: 100%;
            height: 450px;
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

#studentModal .modal-dialog {
    max-width: 50% !important;
    width: 50% !important;
}

#studentModal .modal-dialog {
    max-width: 70% !important;
    width: 70% !important;
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
<div class="container">
<div class="row text-center">
    <!-- Row 1 -->
    <div class="col-md-4 mb-3">
        <button type="button" class="btn btn-dark-blue w-100" data-toggle="modal" data-target="#reportModal">
            <i class="fas fa-file-alt"></i> Continuous Assessment Report
        </button>
    </div>
    <div class="col-md-4 mb-3">
        <button type="button" class="btn btn-dark-blue w-100" data-toggle="modal" data-target="#reportModal3">
            <i class="fas fa-file-alt"></i> MID/MOCK School Reports
        </button>
    </div>    
    <div class="col-md-4 mb-3">
        <button type="button" class="btn btn-dark-blue w-100" data-toggle="modal" data-target="#reportModal4">
            <i class="fas fa-file-alt"></i> MID/MOCK Results Sheets
        </button>
    </div>

    <!-- Row 2 -->
    <div class="col-md-4 mb-3">
        <button type="button" class="btn btn-dark-blue w-100" data-toggle="modal" data-target="#reportModal2">
            <i class="fas fa-file-alt"></i> End Of Term School Reports
        </button>
    </div>
    <div class="col-md-4 mb-3">
        <button type="button" class="btn btn-dark-blue w-100" data-toggle="modal" data-target="#reportModal1">
            <i class="fas fa-file-alt"></i> End Of Term Result Sheets
        </button>
    </div>

        <div class="col-md-4 mb-3">
<button type="button" class="btn btn-dark-blue w-100"
        data-toggle="modal"
        data-target="#ExamanalysisModal">

            <i class="fas fa-file-alt"></i> Exam Analysis Summary
        </button>
    </div>
    <!-- Row 3 -->
<div class="d-flex justify-content-center align-items-center mt-3 w-100 gap-2">
    <input type="text"
           id="searchInput"
           class="form-control"
           placeholder="Search for Exam"
           style="height: 30px; max-width: 350px;">

<%--    <a href="AdminAssessmentscores.aspx"
       class="btn btn-primary btn-sm">
        <i class="fas fa-edit"></i> Correct Scores
    </a>--%>
</div>

</div></div>

        </div>
        <div class="card-body">
            <div class="table-responsive">
                <table id="studentsTable" class="table table-striped table-bordered" style="width:100%">
                    <thead>
                        <tr>
                            <th>Exam Title</th>
                            <th>Release Status</th>
                            <th>Released Date</th>
                            <th>Release By</th>
                            <th>Release Exam</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="ScoresRepeater" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td><%# Eval("AssessmentId") %></td>
<td>
    <%# Eval("ReleaseStatus").ToString() == "Released" ? 
        "<span class='badge badge-success'><i class='fas fa-check-circle'></i> Released</span>" : 
        "<span class='badge badge-danger'><i class='fas fa-times-circle'></i> Not Released</span>" %>
</td>                                    <td><%# Eval("ReleasedTime") %></td>
                                    <td><%# Eval("ReleasedBy") %></td>
                                    <td>
<button type="button" class="btn btn-success btn-sm" data-toggle="modal" data-target="#"
        onclick="openEditModal('<%# Eval("AssessmentId") %>')">
    <i class="fas fa-paper-plane"></i> Release Results
</button>                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
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
                <h5 class="modal-title" id="reportModalLabel">Continuous Assessments Reports</h5>
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body" style="height: calc(100% - 50px); padding: 0;">
                <iframe id="reportIframe" src="ContAssessmentReport.aspx" frameborder="0" style="width: 100%; height: 100%; border: none;"></iframe>
            </div>
        </div>
    </div>
</div>

    <div class="modal fade" id="ExamanalysisModal" tabindex="-1" role="dialog" aria-labelledby="reportExamanalysisModal" aria-hidden="true">
    <div class="modal-dialog modal-lg" role="document" style="max-width: 90vw; width: 90vw; height: 90vh;">
        <div class="modal-content" style="height: 100%;">
            <div class="modal-header">
                <h5 class="modal-title" id="reportExamanalysisModal">Exam Analysis Summary Reports</h5>
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body" style="height: calc(100% - 50px); padding: 0;">
                <iframe id="reportIframeAnalysis" src="ExamAnalysisReports.aspx" frameborder="0" style="width: 100%; height: 100%; border: none;"></iframe>
            </div>
        </div>
    </div>
</div>
    <div class="modal fade" id="reportModal4" tabindex="-1" role="dialog" aria-labelledby="reportModalLabel4" aria-hidden="true">
    <div class="modal-dialog modal-lg" role="document" style="max-width: 90vw; width: 90vw; height: 90vh;">
        <div class="modal-content" style="height: 100%;">
            <div class="modal-header">
                <h5 class="modal-title" id="reportModalLabel4">MID/MOCK RESULTS SHEETS</h5>
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body" style="height: calc(100% - 50px); padding: 0;">
                <iframe id="reportIframe4" src="ResultSherrtsOthers.aspx" frameborder="0" style="width: 100%; height: 100%; border: none;"></iframe>
            </div>
        </div>
    </div>
</div>

    <div class="modal fade" id="reportModal1" tabindex="-1" role="dialog" aria-labelledby="reportModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg" role="document" style="max-width: 90vw; width: 90vw; height: 90vh;">
        <div class="modal-content" style="height: 100%;">
            <div class="modal-header">
                <h5 class="modal-title" id="reportModalLabel1">End of Term Result Sheets Reports</h5>
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body" style="height: calc(100% - 50px); padding: 0;">
                <iframe id="reportIframe1" src="ResultSheetsEOTReports.aspx" frameborder="0" style="width: 100%; height: 100%; border: none;"></iframe>
            </div>
        </div>
    </div>
</div>

    <div class="modal fade" id="reportModal2" tabindex="-1" role="dialog" aria-labelledby="reportModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg" role="document" style="max-width: 90vw; width: 90vw; height: 90vh;">
        <div class="modal-content" style="height: 100%;">
            <div class="modal-header">
                <h5 class="modal-title" id="reportModalLabel2">End of Term School Reports</h5>
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body" style="height: calc(100% - 50px); padding: 0;">
                <iframe id="reportIframe2" src="AssessmentScoresEOTReports.aspx" frameborder="0" style="width: 100%; height: 100%; border: none;"></iframe>
            </div>
        </div>
    </div>
</div>

        <div class="modal fade" id="reportModal3" tabindex="-1" role="dialog" aria-labelledby="reportModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg" role="document" style="max-width: 90vw; width: 90vw; height: 90vh;">
        <div class="modal-content" style="height: 100%;">
            <div class="modal-header">
                <h5 class="modal-title" id="reportModalLabel3">MID/MOCK School Reports Reports</h5>
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body" style="height: calc(100% - 50px); padding: 0;">
                <iframe id="reportIframe3" src="AssessmentSchoolReportsOthers.aspx" frameborder="0" style="width: 100%; height: 100%; border: none;"></iframe>
            </div>
        </div>
    </div>
</div>

<!-- Exam Management Modal -->
<div class="modal fade" id="studentModal" tabindex="-1" role="dialog" aria-labelledby="studentModalLabel" aria-hidden="true">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="studentModalLabel">Release Examinations</h5>
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body">
                <iframe id="studentIframe" src="ReleaseExam.aspx" frameborder="0"></iframe>
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
        /* ===========================
           PAGINATION SETUP
        ============================ */
        var rowsPerPage = 10;
        var rows = $('#studentsTable tbody tr');
        var rowsCount = rows.length;
        var pageCount = Math.ceil(rowsCount / rowsPerPage);
        var numbers = $('#pagination');

        // Generate pagination links
        for (var i = 1; i <= pageCount; i++) {
            numbers.append('<a href="#" class="page-link" data-page="' + i + '">' + i + '</a>');
        }

        showPage(1);

        $(document).on('click', '.page-link', function (e) {
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

        /* ===========================
           SEARCH FILTER
        ============================ */
        $('#searchInput').on('input', function () {
            var searchTerm = $(this).val().toLowerCase();
            rows.each(function () {
                var row = $(this);
                var text = row.text().toLowerCase();
                row.toggle(text.includes(searchTerm));
            });

            // Update pagination dynamically
            rows = $('#studentsTable tbody tr:visible');
            rowsCount = rows.length;
            pageCount = Math.ceil(rowsCount / rowsPerPage);
            numbers.empty();
            for (var i = 1; i <= pageCount; i++) {
                numbers.append('<a href="#" class="page-link" data-page="' + i + '">' + i + '</a>');
            }
            showPage(1);
        });
        /* ===========================
        REFRESH PAGE ON ANY MODAL CLOSE
     =========================== */
        $('.modal').on('hidden.bs.modal', function () {
            location.reload();
        });
        /* ===========================
           DELETE SUCCESS ALERT
        ============================ */
        var urlParams = new URLSearchParams(window.location.search);
        if (urlParams.get('deleteSuccess') === 'true') {
            $('#successModal').modal('show');
            history.replaceState(null, '', window.location.pathname);
        }
        /* ===========================
           SEARCH TABLE (manual trigger)
        ============================ */
        window.searchTable = function () {
            var input = document.getElementById("searchInput");
            var filter = input.value.toUpperCase();
            var table = document.getElementById("studentsTable");
            var tr = table.getElementsByTagName("tr");
            var visibleCount = 0;

            for (var i = 1; i < tr.length; i++) {
                tr[i].style.display = "none";
                var td = tr[i].getElementsByTagName("td");
                for (var j = 0; j < td.length; j++) {
                    if (td[j]) {
                        var txtValue = td[j].textContent || td[j].innerText;
                        if (txtValue.toUpperCase().indexOf(filter) > -1) {
                            tr[i].style.display = "";
                            visibleCount++;
                            break;
                        }
                    }
                }
            }

            var noDataRow = document.getElementById("noDataRow");
            noDataRow.style.display = visibleCount === 0 ? "" : "none";
        };
    });
    /* ===========================
       MODAL HANDLERS (global)
    ============================ */
    function openAddModal() {
        var iframe = document.getElementById('studentIframe');
        iframe.src = 'AssessmentScoreAdd.aspx';
        $('#studentModal').modal('show');
    }

    function openEditModal(ScoreId) {
        var iframe = document.getElementById('studentIframe');
        iframe.src = 'ReleaseExam.aspx?mode=edit&AssessmentId=' + encodeURIComponent(ScoreId);
        $('#studentModal').modal('show');
    }

    function confirmDelete(ScoreId) {
        if (confirm("Are you sure you want to delete this Score?")) {
            window.location.href = 'AssessmentScoreAdd.aspx?ScoreId=' + encodeURIComponent(ScoreId) + '&mode=delete';
        }
    }

    /* ===========================
       EXPORT FUNCTIONS
    ============================ */
    function exportTableToCSV(filename) {
        var csv = [];
        var rows = document.querySelectorAll("#studentsTable tr");

        for (var i = 0; i < rows.length; i++) {
            var row = [], cols = rows[i].querySelectorAll("td:not(:last-child), th:not(:last-child)");
            for (var j = 0; j < cols.length; j++) {
                row.push('"' + cols[j].innerText.replace(/"/g, '""') + '"');
            }
            csv.push(row.join(","));
        }

        downloadCSV(csv.join("\n"), filename);
    }

    function downloadCSV(csv, filename) {
        var csvFile = new Blob([csv], { type: "text/csv" });
        var downloadLink = document.createElement("a");
        downloadLink.download = filename;
        downloadLink.href = window.URL.createObjectURL(csvFile);
        downloadLink.style.display = "none";
        document.body.appendChild(downloadLink);
        downloadLink.click();
    }

    function exportTableToPDF(filename) {
        const { jsPDF } = window.jspdf;
        const doc = new jsPDF();
        doc.autoTable({
            html: '#studentsTable',
            columnStyles: { 5: { cellWidth: 0 } },
            didParseCell: function (data) {
                if (data.column.index === 5) {
                    data.cell.styles.cellWidth = 0;
                }
            }
        });
        doc.save(filename);
    }
</script>
</asp:Content>
