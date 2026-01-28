<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="TimeTable.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.TimeTable" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .timetable-container {
            margin-top: 30px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        th, td {
            text-align: center;
            vertical-align: middle;
        }
        th {
            background-color: #007bff;
            color: white;
        }
        .break {
            background-color: #f8d7da;
            font-weight: bold;
        }
    </style>

    <div class="container mt-4">
        <h2 class="text-center">Weekly Timetable</h2>
        <asp:GridView ID="gvWeeklyTimetable" runat="server" AutoGenerateColumns="true" CssClass="table table-bordered">
        </asp:GridView>
    </div>
</asp:Content>
