<%@ Page Title="Student Attendance Report" Language="C#" MasterPageFile="~/Views/Admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="StudentAttendanceReport.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.StudentAttendanceReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css" />
    <style>
        .table-responsive {
            width: 100%;
            overflow-x: auto;
        }
        .table {
            font-size: 12px;
        }
        .card {
            max-width: 1000px;
            margin: 0 auto;
            box-shadow: 0px 4px 6px rgba(0, 0, 0, 0.1);
            border-radius: 8px;
            padding: 20px;
        }
        .pagination li a {
            cursor: pointer;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mt-3">
        <div class="card">
            <div class="card-header d-flex justify-content-between align-items-center">
                <h5 class="mb-0">Student Attendance Report</h5>
                <div class="d-flex gap-2">
                    <button type="button" class="btn btn-success btn-sm" onclick="exportTableToCSV('attendance.csv')">
                        <i class="fas fa-file-csv"></i> Export CSV
                    </button>
                    <button type="button" class="btn btn-danger btn-sm" onclick="exportTableToPDF('attendance.pdf')">
                        <i class="fas fa-file-pdf"></i> Export PDF
                    </button>
                </div>
            </div>
            <div class="card-body">
                <input type="text" id="searchBox" placeholder="Search..." class="form-control mb-3" />
                <div class="table-responsive">
                    <asp:GridView ID="GridViewAttendance" runat="server"
                        CssClass="table table-bordered table-striped attendance-table"
                        AutoGenerateColumns="true"
                        GridLines="None"
                        HeaderStyle-CssClass="table-dark" />
                </div>
                <nav>
                    <ul class="pagination justify-content-center" id="pagination"></ul>
                </nav>
            </div>
        </div>
    </div>

    <script>
        const rowsPerPage = 10;
        let currentPage = 1;

        function paginateTable() {
            const table = document.querySelector('.attendance-table');
            const rows = table.querySelectorAll('tbody tr');
            const filter = document.getElementById('searchBox').value.toLowerCase();

            let filteredRows = Array.from(rows).filter(row =>
                row.innerText.toLowerCase().includes(filter)
            );

            const totalPages = Math.ceil(filteredRows.length / rowsPerPage);
            const pagination = document.getElementById('pagination');
            pagination.innerHTML = '';

            // Hide all rows
            rows.forEach(row => row.style.display = 'none');

            // Show only current page rows from filtered set
            const start = (currentPage - 1) * rowsPerPage;
            const end = start + rowsPerPage;
            for (let i = start; i < end && i < filteredRows.length; i++) {
                filteredRows[i].style.display = '';
            }

            // Generate pagination buttons
            for (let i = 1; i <= totalPages; i++) {
                const li = document.createElement('li');
                li.className = 'page-item' + (i === currentPage ? ' active' : '');
                li.innerHTML = `<a class="page-link">${i}</a>`;
                li.addEventListener('click', function (e) {
                    e.preventDefault();
                    currentPage = i;
                    paginateTable();
                });
                pagination.appendChild(li);
            }
        }

        document.addEventListener('DOMContentLoaded', function () {
            paginateTable();
            document.getElementById('searchBox').addEventListener('input', function () {
                currentPage = 1;
                paginateTable();
            });
        });

        function exportTableToCSV(filename) {
            var csv = [];
            var rows = document.querySelectorAll(".attendance-table tr");
            for (var i = 0; i < rows.length; i++) {
                var row = [], cols = rows[i].querySelectorAll("td, th");
                for (var j = 0; j < cols.length; j++)
                    row.push('"' + cols[j].innerText.replace(/"/g, '""') + '"');
                csv.push(row.join(","));
            }
            var csvFile = new Blob([csv.join("\n")], { type: "text/csv" });
            var downloadLink = document.createElement("a");
            downloadLink.download = filename;
            downloadLink.href = window.URL.createObjectURL(csvFile);
            downloadLink.style.display = "none";
            document.body.appendChild(downloadLink);
            downloadLink.click();
        }

        function exportTableToPDF(filename) {
            alert("PDF export requires jsPDF and autoTable plugins. Consider using advanced export tools.");
        }
    </script>
</asp:Content>
