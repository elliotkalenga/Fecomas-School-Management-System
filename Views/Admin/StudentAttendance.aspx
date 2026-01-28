<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="StudentAttendance.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.StudentAttendance" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Student Attendance</title>

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-icons/1.4.0/font/bootstrap-icons.min.css">
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>

    <style>
        html, body {
            height: 100%;
            margin: 0;
            font-family: Arial, sans-serif;
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            background-color: #001f3f;
        }



        .card {
            width: 100%;
            max-width: 500px;
            margin: 20px;
            padding: 20px;
            text-align: center;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
            border-radius: 10px;
            background-color: #ffffff;
        }

        .card-header {
            background-color: #001f3f;
            color: white;
            padding: 15px;
            font-size: 1.5rem;
        }

        .btn-primary {
            background-color: #00008B;
            font-size: 1.2rem;
            padding: 15px 30px;
            border-radius: 10px;
            width: 100%;
            color: white;
            transition: background-color 0.3s ease;
        }

        .btn-primary:hover {
            background-color: #001f3f;
        }

        .form-control {
            font-size: 1.25rem;
            padding: 10px;
            text-align: center;
            border-radius: 10px;
            margin-top: 20px;
            width: 100%;
        }

        .form-control::placeholder {
            color: #999;
        }

        .digital-clock {
            font-size: 3rem;
            font-weight: bold;
            color: #00008B;
            margin-bottom: 20px;
        }

        .successMessage, .ErrorMessage {
            font-weight: bold;
            margin-top: 20px;
        }

        .successMessage {
            color: green;
        }

        .ErrorMessage {
            color: darkred;
        }
    </style>
</head>

<body>

<form id="form1" runat="server">
    <div class="container">

<div class="form-group" style="max-width: 500px; margin: 20px auto 0 auto;">
    <asp:DropDownList ID="ddlWeek" runat="server" CssClass="form-control text-center">
        <asp:ListItem Text="Select Week" Value="" Selected="True" Disabled="True" />
        <asp:ListItem>WK1</asp:ListItem>
        <asp:ListItem>WK2</asp:ListItem>
        <asp:ListItem>WK3</asp:ListItem>
        <asp:ListItem>WK4</asp:ListItem>
        <asp:ListItem>WK5</asp:ListItem>
        <asp:ListItem>WK6</asp:ListItem>
        <asp:ListItem>WK7</asp:ListItem>
        <asp:ListItem>WK8</asp:ListItem>
        <asp:ListItem>WK9</asp:ListItem>
        <asp:ListItem>WK10</asp:ListItem>
        <asp:ListItem>WK11</asp:ListItem>
        <asp:ListItem>WK12</asp:ListItem>
        <asp:ListItem>WK13</asp:ListItem>
        <asp:ListItem>WK14</asp:ListItem>
        <asp:ListItem>WK15</asp:ListItem>
    </asp:DropDownList>
</div>


        <!-- Card starts here -->
        <div class="card">
            <h5 class="card-header">Student Attendance</h5>
            <div class="card-body">
                <!-- Digital Clock -->
<div id="clock" class="digital-clock"></div>
<div id="date" class="digital-clock" style="font-size: 1.5rem; color: gray;"></div>

                <!-- QR Reader -->
                <div id="reader"></div>
                <asp:Label ID="lblMessage" runat="server" ForeColor="Red" Font-Bold="True" Visible="false"></asp:Label>

                <!-- Textbox to display scanned barcode -->
                <asp:TextBox ID="txtBarcode" runat="server" CssClass="form-control text-center mt-3"
                    placeholder="Scan Barcode" AutoPostBack="true" OnTextChanged="SubmitBarcode"></asp:TextBox>
            </div>
        </div>

    </div>
</form>
<div class="container">
    <div style="max-width: 500px; margin: 0 auto;">
        <a href="<%= ResolveUrl("~/Views/Admin/Dashboard.aspx") %>" class="btn btn-outline-warning mt-3 w-100">Back to Dashboard</a>
    </div>
</div>

    <script src="https://unpkg.com/html5-qrcode"></script>
    <script>
        // Function to update the digital clock

        function updateClock() {
            var now = new Date();
            var hours = now.getHours().toString().padStart(2, '0');
            var minutes = now.getMinutes().toString().padStart(2, '0');
            var seconds = now.getSeconds().toString().padStart(2, '0');
            var timeString = hours + ":" + minutes + ":" + seconds;

            var options = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
            var dateString = now.toLocaleDateString(undefined, options);

            document.getElementById('clock').textContent = timeString;
            document.getElementById('date').textContent = dateString;
        }

        // Call immediately and then set interval
        updateClock();
        setInterval(updateClock, 1000);


        // Update the clock every second

        // Barcode scanner function
        function startScanner() {
            let scanner = new Html5Qrcode("reader");
            scanner.start(
                { facingMode: "environment" }, // Use the back camera
                { fps: 10, qrbox: 250 },
                (decodedText) => {
                    document.getElementById("txtBarcode").value = decodedText;
                    // Trigger the server-side function after scanning
                    __doPostBack('btnSubmit', ''); // Trigger postback to call SubmitBarcode
                },
                (errorMessage) => {
                    console.log(errorMessage); // Handle errors
                }
            );
        }
    </script>


</body>
</html>
