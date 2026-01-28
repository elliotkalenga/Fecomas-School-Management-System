<%@ Page Title="" Language="C#" MasterPageFile="~/POS/AdminMaster.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="SMSWEBAPP.POS.Dashboard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <style>
        @keyframes fadeIn {
            0% { opacity: 0; transform: scale(0.8); }
            100% { opacity: 1; transform: scale(1); }
        }

        .card {
            animation: fadeIn 1s ease-out;
        }

        /* Adjusted the container-fluid margin to avoid protruding outside the sidebar */
        .container-fluid {
            margin-left: 40px; /* Adjust this value based on sidebar width */
            padding-right: 40px; /* Adjust the padding as necessary */
        }
    </style>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    
    <div class="row mb-5"></div>
    <div class="container-fluid">
        <!-- Cards Section -->
        <div class="row mb-3">
            <!-- Enrolled Students Card -->
            <div class="col-12 col-md-3 mb-3">
                <asp:LinkButton ID="EnrolledStudentsButton" runat="server" CssClass="card" Style="background-color: #0097a7; text-align: center; height: 200px;">
                    <div class="card-header" Style="background-color: #006064;">
                        <i class="fas fa-users" Style="color: #fff; font-size: 24px;"></i>
                        <h5 class="card-title" Style="color: #fff;">Enrolled Students</h5>
                    </div>
                    <div class="card-body">
                        <div class="row">
                            <div class="col-12">
                                <h3 class="card-label" style="color:azure"><asp:Label ID="lblEnrolledStudents" runat="server" Text="0"></asp:Label></h3>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-12">
                                <span Style="color: #fff;"><asp:Label ID="LblTermEnrolled" runat="server" Text="0"></asp:Label></span>
                            </div>
                        </div>
                    </div>
                </asp:LinkButton>
            </div>

            <!-- Active Teachers Card -->
            <div class="col-12 col-md-3 mb-3">
                <asp:LinkButton ID="ActiveTeachersButton" runat="server" CssClass="card" Style="background-color: #1e88e5; text-align: center; height: 200px;">
                    <div class="card-header" Style="background-color: #0d47a1;">
                        <i class="fas fa-chalkboard-teacher" Style="color: #fff; font-size: 24px;"></i>
                        <h5 class="card-title" Style="color: #fff;">Active Teachers</h5>
                    </div>
                    <div class="card-body">
                        <div class="row">
                            <div class="col-12">
                                <h3 class="card-label" style="color:azure"><asp:Label ID="lblActiveTeachers" runat="server" Text="0"></asp:Label></h3>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-12">
                                <span Style="color: #fff;"><asp:Label ID="LblTermTeachers" runat="server" Text="0"></asp:Label></span>
                            </div>
                        </div>
                    </div>
                </asp:LinkButton>
            </div>

            <!-- Subjects Offered Card -->
<div class="col-12 col-md-3 mb-3">
    <asp:LinkButton ID="ExamsAdministeredButton" runat="server" CssClass="card" Style="background-color: #00897b; text-align: center; height: 200px;">
        <div class="card-header" Style="background-color: #004d40;">
            <i class="fas fa-file-alt" Style="color: #fff; font-size: 24px;"></i>
            <h5 class="card-title" Style="color: #fff;">Exams Administered</h5>
        </div>
        <div class="card-body">
            <div class="row">
                <div class="col-12">
                    <h3 class="card-label" Style="color: #fff;"><asp:Label ID="LblTermExams" runat="server" Text="0"></asp:Label></h3>
                </div>
            </div>
            <div class="row">
                <div class="col-12">
                    <span Style="color: #fff;"><asp:Label ID="lblExamResults" runat="server" Text="0"></asp:Label></span>
                </div>
            </div>
        </div>
    </asp:LinkButton>
</div>

            <!-- Graduated Students Card -->
            <div class="col-12 col-md-3 mb-3">
                <asp:LinkButton ID="GraduatedStudentsButton" runat="server" CssClass="card" Style="background-color: #0097a7; text-align: center; height: 200px;">
                    <div class="card-header" Style="background-color: #004d60;">
                        <i class="fas fa-graduation-cap" Style="color: #fff; font-size: 24px;"></i>
                        <h5 class="card-title" Style="color: #fff;">Graduated Students</h5>
                    </div>
                    <div class="card-body">
                        <div class="row">
                            <div class="col-12">
                                <h3 class="card-label" style="color:azure"><asp:Label ID="LblGraduatedStudents" runat="server" Text="0"></asp:Label></h3>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-12">
                                <span Style="color: #fff;">All Years</span>
                            </div>
                        </div>
                    </div>
                </asp:LinkButton>
            </div>

        </div>

        <!-- Charts Section -->
<!-- Existing charts row -->
<div class="row">
    <div class="col-12 col-md-6 mb-3">
        <h4 style="text-align: center;">Active Term Enrollment Chart</h4>
        <div style="height: 200px; width: 100%;">
            <canvas id="barChart"></canvas>
        </div>
    </div>
    <div class="col-12 col-md-6 mb-3">
        <h4 style="text-align: center;">Today's Students Attendance</h4>
        <div style="height: 200px; width: 100%;">
            <canvas id="pieChart"></canvas>
        </div>
    </div>
</div>

<!-- New row with license information labels -->
<div class="row mt-3">
    <div class="col-12 col-md-3 text-center">
        <strong>License Status: </strong>
        <asp:Label ID="lblLicenseStatus" runat="server" Text="Active"></asp:Label>
    </div>
    <div class="col-12 col-md-2 text-center">
        <strong>Licensed Days: </strong>
        <asp:Label ID="lblLicensedDays" runat="server" Text="0"></asp:Label>
    </div>
    <div class="col-12 col-md-2 text-center">
        <strong>Used Days: </strong>
        <asp:Label ID="lblUsedDays" runat="server" Text="0"></asp:Label>
    </div>
    <div class="col-12 col-md-2 text-center">
        <strong>Remaining Days: </strong>
        <asp:Label ID="lblRemainingDays" runat="server" Text="0"></asp:Label>
    </div>

    <div class="col-12 col-md-3 text-center">
        <strong>Expiry Date: </strong>
        <asp:Label ID="lblEndDate" runat="server" Text=""></asp:Label>
    </div>
</div>
    </div>

    <script>

        // Bar Chart - Enrollment
        var ctxBar = document.getElementById('barChart').getContext('2d');
        var barChart = new Chart(ctxBar, {
            type: 'bar',
            data: {
                labels: [<%= BarChartLabels %>],
                datasets: [{
                    label: 'Enrollment',
                    data: [<%= BarChartData %>],
                    backgroundColor: ['#5c6bc0', '#1e88e5', '#3949ab', '#26c6da'],
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { display: false }
                },
                scales: {
                    y: { beginAtZero: true }
                }
            }
        });

        // Pie Chart - Attendance
        var ctxPie = document.getElementById('pieChart').getContext('2d');
        var pieChart = new Chart(ctxPie, {
            type: 'pie',
            data: {
                labels: [<%= PieChartLabels %>],
                datasets: [{
                    label: 'Todays Attendance',
                    data: [<%= PieChartData %>],
                    backgroundColor: ['#5c6bc0', '#1e88e5'],
                    borderColor: ['#3949ab', '#0d47a1'],
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false
            }
        });
    </script>
</asp:Content>
