<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Master.Master" AutoEventWireup="true" CodeBehind="AssessmentScoreAdd.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.AssessmentScoreAdd" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        html, body {
            height: 100%;
            margin: 0;
        }

        .container {
            height: 100%;
            display: flex;
            flex-direction: column;
        }

        .card {
            flex: 1;
        }

        .card-body {
            flex: 1;
            display: flex;
            flex-direction: column;
            justify-content: space-between;
        }

        .btn-dark-blue {
            background-color: #00008B;
            color: white;
        }
                .select2 {
            width: 100% !important;
        }
        .card-header {
            background-color: #001f3f;
            font-size: smaller;
            padding: 0.25rem;
        }

        .btn-small {
            font-size: 0.50rem;
            padding: 0.20rem 0.5rem;
        }

        .successMessage {
            color: green;
            font-weight: bold;
        }

        .ErrorMessage {
            color: darkred;
            font-weight: bold;
        }
    </style>
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">
    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <div class="modal fade" id="successModal" tabindex="-1" role="dialog" aria-labelledby="successModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title successMessage" id="successModalLabel">Success</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div> 
                <div class="modal-body">
                    <asp:Label ID="lblMessage" runat="server" CssClass="successMessage"></asp:Label>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="errorModal" tabindex="-1" role="dialog" aria-labelledby="errorModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="errorModalLabel">Error</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Text=""></asp:Label>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

<div class="container mt-3">
    <div class="form-row">
        <div class="form-group col-12">
            <label for="ddlExam">Exam</label>
            <div class="input-group w-100">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="bi bi-journal-check"></i></span>
                </div>
                <asp:DropDownList ID="ddlExam" runat="server" CssClass="form-control flex-grow-1" OnSelectedIndexChanged="ddlExam_SelectedIndexChanged"></asp:DropDownList>
            </div>
        </div>

        <div class="form-group col-12">
            <label for="ddlStudent">Student</label>
            <div class="input-group w-100">
                <asp:DropDownList ID="ddlStudent" runat="server" CssClass="form-control flex-grow-1 select2"></asp:DropDownList>
            </div>
        </div>

        <div class="form-group col-12">
            <label for="txtScore">Score</label>
            <div class="input-group w-100">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="bi bi-graph-up"></i></span>
                </div>
                <asp:TextBox ID="txtScore" runat="server" CssClass="form-control flex-grow-1" placeholder="Enter Score"></asp:TextBox>
            </div>
        </div>
    </div>

    <div class="form-group text-center">
        <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-dark-blue btn-block" Text="Submit" OnClick="btnSubmit_Click" />
    </div>
</div>

    <script>
        $(document).ready(function () {
            $('.select2').select2({
                placeholder: 'Select a student',
                allowClear: true
            });

            $('#studentSearch').on('keyup', function () {
                var searchValue = $(this).val().toUpperCase();
                $('.select2 option').each(function () {
                    var text = $(this).text().toUpperCase();
                    if (text.indexOf(searchValue) > -1) {
                        $(this).show();
                    } else {
                        $(this).hide();
                    }
                });
            });
        });
    </script>
</asp:Content>