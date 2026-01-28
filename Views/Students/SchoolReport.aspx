<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Students/StudentMasterPage.Master" AutoEventWireup="true" CodeBehind="SchoolReport.aspx.cs" Inherits="SMSWEBAPP.Views.Students.SchoolReport" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style>
        html, body {
            height: 100vh;
            margin: 0;
            padding: 0;
            overflow: hidden; /* Prevents any scroll gaps */
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

        .container {
            width: 100%;
            height: 100%;
            max-width: none; /* Removes max-width restriction */
            padding: 0;
            display: flex;
            flex-direction: column;
        }

        .card {
            flex: 1; /* Allows it to expand fully within the container */
            width: 100%;
            height: 100%;
            margin: 0;
            border-radius: 0;
        }

        .table-responsive {
            width: 100% !important;
            overflow-x: auto; /* Enable horizontal scrolling if needed */
        }

        .table {
            width: 100% !important;
            table-layout: auto; /* Ensures columns adjust dynamically */
        }

        .modal-dialog {
            max-width: 95vw; /* Changed from 90% to 95vw */
            width: 95vw; /* Changed from 80% to 95vw */
            height: 95vh; /* Added to set the height to 95% of the viewport height */
        }

        .modal-body iframe {
            width: 100%;
            height: 100%;
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

        #studentModal2 .modal-dialog {
            max-width: 95vw; /* Changed from 80% to 95vw */
            height: 95vh; /* Set the height to 95% of the viewport height */
            margin: 0 auto; /* Center the modal horizontally */
        }

        #studentModal2 .modal-body {
            height: calc(85vh - 56px); /* Adjust the body height, subtracting the header height */
            overflow-y: auto; /* Allows scrolling if content overflows */
        }

        #studentModal2 iframe {
            width: 100%;
            height: calc(95vh - 56px); /* Adjust the iframe height to fill the modal body */
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
            <div class="card-header d-flex justify-content-between align-items-center" style="padding: 5px;">
                <div class="d-flex flex-wrap align-items-center" style="margin: 0;">
                    <button type="button" class="btn btn-dark-blue mb-1 mr-1" onclick="window.location.href="#">
    <i class="fas fa-file-alt"></i>
 Examination Results Reports For  <asp:Label ID="lblUser" runat="server" CssClass="ml-2 font-weight-bold"></asp:Label>

</button>


                </div>
    <div class="d-flex flex-wrap gap-2">
        <div class="input-group" style="height: 30px;">
            <input type="text" id="searchInput" class="form-control" placeholder="Type any key word to Search...">
        </div>
    </div>
            </div>

            <div class="card-body">

                 <div class="table-responsive">
     <table id="studentsTable" class="table table-striped table-bordered" style="width:100%">
         <thead>
             <tr>
<th>Download School Reports</th>
<th>StudentNo</th>
<th>Student</th>
<th>Class</th>
<th>Term</th>
<th>ExamCode</th>
<th>Exam</th>
             </tr>
         </thead>
         <tbody>
             <asp:Repeater ID="StudentsRepeater" runat="server">
                 <ItemTemplate>
                     <tr class="student-row">
                                                                 <td>
                                            <button type="button" class="btn btn-dark-blue btn-sm" 
                                                    data-toggle="modal" 
                                                    data-target="#studentModal2" 
                                                    onclick="openEditModal('<%# Eval("StudentNo") %>', '<%# Eval("Exam") %>', '<%# Eval("ExamId") %>', '<%# Eval("TermId") %>', '<%# Eval("ClassId") %>')">
                                                <i class="fas fa-download"></i>

                                            </button>
                                        </td>
                                        <td><%# Eval("StudentNo") %></td>
                                        <td><%# Eval("Student") %></td>
                                        <td><%# Eval("ClassName") %></td>
                                        <td><%# Eval("Term") %></td>
                                        <td><%# Eval("ExamCode") %></td>
<td><%# Eval("Exam") %></td>                     </tr>
                 </ItemTemplate>
             </asp:Repeater>
             <tr id="noDataRow" style="display: none;">
                 <td colspan="7" class="text-center">No Matching Records</td>
             </tr>
         </tbody>
     </table>

                 </div>
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

<div class="modal fade" id="studentModal2" tabindex="-1" role="dialog" aria-labelledby="studentModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg" role="document">
        <div class="modal-content">
            <!-- Modal Header -->
            <div class="modal-header d-flex justify-content-center" style="background-color: #001f3f; color: white;">
                <h5 class="modal-title text-center" id="studentModalLabel">Examination Results Reports Generation</h5>
                <button type="button" class="close" data-dismiss="modal" aria-label="Close" style="color: white; position: absolute; right: 15px;">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <!-- Modal Body -->
            <div class="modal-body">
<iframe id="studentIframe2" src="<%= ResolveUrl("~/Views/Admin/StudentSchoolReport.aspx") %>" 
        frameborder="0" style="width: 100%; height: 700px;"></iframe>
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
            var rowsPerPage = 5; // Number of records per page
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
                window.location.href = 'StudentSchoolReport.aspx?StudentNo=' + FeesCollectionId + '&mode=delete';
            }
        }

        function openEditModal(FeesCollectionId, mode, ExamId, TermId, ClassId) {
            var iframe = document.getElementById('studentIframe2');
            var basePath = '<%= ResolveUrl("~/Views/Admin/StudentSchoolReport.aspx") %>';
            iframe.src = basePath + '?StudentNo=' + FeesCollectionId +
                '&Exam=' + mode +
                '&ExamId=' + ExamId +
                '&TermId=' + TermId +
                '&ClassId=' + ClassId;
        }



        $(document).ready(function () {
            $("#searchInput").on("keyup", function () {
                var value = $(this).val().toLowerCase();
                $("#studentsTable tbody tr").filter(function () {
                    $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
                });
            });
        });


    </script>
</asp:Content>