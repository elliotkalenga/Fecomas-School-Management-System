<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Master.Master"
    AutoEventWireup="true" CodeBehind="ReleaseExam.aspx.cs"
    Inherits="SMSWEBAPP.Views.Admin.ReleaseExam" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.2.3/dist/css/bootstrap.min.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:ScriptManager ID="ScriptManager1" runat="server" />

    <h4 class="mb-3">Release Exam</h4>

    <asp:Label ID="lblStatus" runat="server" CssClass="text-primary" />
    <asp:Label ID="lblSMSError" runat="server" CssClass="text-primary" />

    <div class="form-group">
        <label>Exam</label>
        <asp:DropDownList ID="ddlExam" runat="server" CssClass="form-control" />
    </div>

<div class="form-group">
    <label>Class</label>
    <asp:DropDownList ID="ddlClass" runat="server" CssClass="form-control">
        <asp:ListItem Text="-- Select Class --" Value="" />
    </asp:DropDownList>
</div>



<div class="form-group">
    <label>Status</label>
    <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control">
        <asp:ListItem Text="-- Select Release Status --" Value="" Disabled="true" Selected="true" />
        <asp:ListItem Text="Released" Value="Released" />
        <asp:ListItem Text="Not Released" Value="Not Released" />
    </asp:DropDownList>
</div>

<div class="form-group">
    <label>Do you want to send SMS to Parents?</label>
    <asp:DropDownList ID="ddlSMS" runat="server" CssClass="form-control">
        <asp:ListItem Text="-- Select Option --" Value="" Disabled="true" Selected="true" />
        <asp:ListItem Text="Yes" Value="Yes" />
        <asp:ListItem Text="No" Value="No" />
    </asp:DropDownList>
</div>


    <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-primary btn-block"
        OnClick="btnSubmit_Click" />

</asp:Content>
