<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="AttendanceReport.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.AttendanceReport" %>

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

        .table-responsive {
            width: 100%;
            overflow-x: auto;
        }

        .table {
            font-size: 13px;
        }

        h2 {
            font-size: 20px;
            font-weight: bold;
        }

        button {
            font-size: 13px;
            padding: 6px 12px;
        }
    </style>

    <!-- jsPDF and AutoTable -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jspdf/2.5.1/jspdf.umd.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jspdf-autotable/3.5.25/jspdf.plugin.autotable.min.js"></script>

    <!-- SheetJS for Excel export -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/xlsx/0.18.5/xlsx.full.min.js"></script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container mt-2">
    <div class="card p-3">
        <h2 class="mb-3">Attendance Report</h2>

        <!-- Single Row for Term, Class, and Export Buttons -->
        <div class="row mb-3 align-items-end">
            <!-- Term Dropdown -->
            <div class="col-md-4">
                <label for="ddlTerm">Select Term</label>
                <asp:DropDownList ID="ddlTerm" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlTerm_SelectedIndexChanged" />
            </div>

            <!-- Class Dropdown -->
            <div class="col-md-4">
                <label for="ddlClass">Select Class</label>
                <asp:DropDownList ID="ddlClass" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlClass_SelectedIndexChanged" />
            </div>

            <!-- Export Buttons -->
            <div class="col-md-4 d-flex gap-2">
                <button type="button" class="btn btn-success" onclick="exportToExcel()">Export to Excel</button>
                <button type="button" class="btn btn-danger" onclick="exportToPDF()">Export to PDF</button>
            </div>
        </div>

        <!-- Attendance Table -->
        <div class="table-responsive">
            <asp:GridView ID="gvAttendanceReport" runat="server" CssClass="table table-striped table-bordered" AutoGenerateColumns="True" />
        </div>
    </div>
</div>

    <script>
        function exportToExcel() {
            var table = document.querySelector("#<%= gvAttendanceReport.ClientID %>");
            var wb = XLSX.utils.table_to_book(table, { sheet: "Attendance Report" });
            XLSX.writeFile(wb, "Attendance_Report.xlsx");
        }

        function exportToPDF() {
            const { jsPDF } = window.jspdf;
            var doc = new jsPDF();
            doc.text("Attendance Report", 14, 10);
            doc.autoTable({
                html: "#<%= gvAttendanceReport.ClientID %>",
                startY: 20,
                styles: { fontSize: 8 },
                headStyles: { fillColor: [0, 123, 255] }
            });
            doc.save("Attendance_Report.pdf");
        }
    </script>
</asp:Content>
