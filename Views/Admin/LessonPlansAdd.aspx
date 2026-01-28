<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Master.Master" AutoEventWireup="true" CodeBehind="LessonPlansAdd.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.LessonPlansAdd" %>

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

        .card-header {
            background-color: #00008B; /* Very dark blue */
            font-size: smaller; /* Smaller font size */
        }

        .form-control::placeholder {
            color: #6c757d; /* Optional, adjust placeholder color */
        }

        .card-header {
            background-color: #001f3f; /* Very dark blue */
            font-size: smaller; /* Smaller font size */
            padding: 0.25rem; /* Reduced padding */
        }

        .btn-small {
            font-size: 0.50rem; /* Adjust font size */
            padding: 0.20rem 0.5rem; /* Adjust padding for smaller button */
        }

        .successMessage {
            color: green;
            font-weight: bold;
        }

        .ErrorMessage {
            color: darkred;
            font-weight: bold;
        }

        .select2 {
            width: 100% !important;
        }
    </style>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-icons/1.4.0/font/bootstrap-icons.min.css">
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">
    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
    <script>
        $(document).ready(function () {
            $('.select2').select2();
        });
    </script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <!-- Success Modal -->
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
    <asp:Label ID="lblerror" runat="server" ForeColor="Red" Text=""></asp:Label>

    <!-- Error Modal -->
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

    <div class="container">
        <div class="form-row">
            <div class="form-group col-md-12">
                <label for="ddlScheme">Scheme</label>
                <div class="input-group">
                    <asp:DropDownList ID="ddlScheme" runat="server" CssClass="form-control select2" required>
                        <asp:ListItem Value="" Text="Select Scheme"></asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
        </div>

<div class="form-row">
    <div class="form-group col-md-3">
        <label for="txtDuration">Duration (in minutes)</label>
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-clock"></i></span>
            </div>
            <asp:TextBox ID="txtDuration" runat="server" CssClass="form-control" placeholder="Enter Duration" required></asp:TextBox>
        </div>
    </div>
    <div class="form-group col-md-3">
        <label for="ddlWeekNo">Week No</label>
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-calendar-week"></i></span>
            </div>
            <asp:DropDownList ID="ddlWeekNo" runat="server" CssClass="form-control">
                <asp:ListItem Value="" Text="--Select Week--" Selected="True"></asp:ListItem>
                <asp:ListItem Value="Week 1" Text="Week 1"></asp:ListItem>
                <asp:ListItem Value="Week 2" Text="Week 2"></asp:ListItem>
                <asp:ListItem Value="Week 3" Text="Week 3"></asp:ListItem>
                <asp:ListItem Value="Week 4" Text="Week 4"></asp:ListItem>
                <asp:ListItem Value="Week 5" Text="Week 5"></asp:ListItem>
                <asp:ListItem Value="Week 6" Text="Week 6"></asp:ListItem>
                <asp:ListItem Value="Week 7" Text="Week 7"></asp:ListItem>
                <asp:ListItem Value="Week 8" Text="Week 8"></asp:ListItem>
                <asp:ListItem Value="Week 9" Text="Week 9"></asp:ListItem>
                <asp:ListItem Value="Week 10" Text="Week 10"></asp:ListItem>
                <asp:ListItem Value="Week 11" Text="Week 11"></asp:ListItem>
                <asp:ListItem Value="Week 12" Text="Week 12"></asp:ListItem>
                <asp:ListItem Value="Week 13" Text="Week 13"></asp:ListItem>
                <asp:ListItem Value="Week 14" Text="Week 14"></asp:ListItem>
                <asp:ListItem Value="Week 15" Text="Week 15"></asp:ListItem>
            </asp:DropDownList>
            <asp:RequiredFieldValidator ID="rfvWeekNo" runat="server" 
                ControlToValidate="ddlWeekNo" 
                InitialValue="" 
                ErrorMessage="Please select a week" 
                ForeColor="Red" 
                Display="Dynamic" />
        </div>
    </div>
    <div class="form-group col-md-6">
        <label for="txtLessonTopic">Lesson Topic</label>
        <div class="input-group">
            <div class="input-group-prepend">
                <span class="input-group-text"><i class="bi bi-card-text"></i></span>
            </div>
            <asp:TextBox ID="txtLessonTopic" runat="server" CssClass="form-control" placeholder="Enter Lesson Topic" required></asp:TextBox>
        </div>
    </div>
</div>
        <div class="form-row">
            <div class="form-group col-md-12">
                <label for="txtLessonObjectives">Lesson Objectives</label>
                <div class="input-group">
                    <div class="input-group-prepend">
                        <span class="input-group-text"><i class="bi bi-bullseye"></i></span>
                    </div>
                    <asp:TextBox ID="txtLessonObjectives" runat="server" CssClass="form-control" placeholder="Enter Lesson Objectives" TextMode="MultiLine" Rows="5" required></asp:TextBox>
                </div>
            </div>
        </div>

        <div class="form-row">
            <div class="form-group col-md-12">
                <label for="txtTeachingMethods">Teaching Methods</label>
                <div class="input-group">
                    <div class="input-group-prepend">
                        <span class="input-group-text"><i class="bi bi-person"></i></span>
                    </div>
                    <asp:TextBox ID="txtTeachingMethods" runat="server" CssClass="form-control" placeholder="Enter Teaching Methods" TextMode="MultiLine" Rows="5" required></asp:TextBox>
                </div>
            </div>
        </div>

        <div class="form-row">
            <div class="form-group col-md-12">
                <label for="txtIntroduction">Introduction</label>
                <div class="input-group">
                    <div class="input-group-prepend">
                        <span class="input-group-text"><i class="bi bi-info-circle"></i></span>
                    </div>
                    <asp:TextBox ID="txtIntroduction" runat="server" CssClass="form-control" placeholder="Enter Introduction" TextMode="MultiLine" Rows="5" required></asp:TextBox>
                </div>
            </div>
        </div>

        <div class="form-row">
            <div class="form-group col-md-12">
                <label for="txtPlannedActivities">Planned Activities</label>
                <div class="input-group">
                    <div class="input-group-prepend">
                        <span class="input-group-text"><i class="bi bi-list-task"></i></span>
                    </div>
                    <asp:TextBox ID="txtPlannedActivities" runat="server" CssClass="form-control" placeholder="Enter Planned Activities" TextMode="MultiLine" Rows="5" required></asp:TextBox>
                </div>
            </div>
        </div>

        <div class="form-row">
            <div class="form-group col-md-12">
                <label for="txtResources">Resources</label>
                <div class="input-group">
                    <div class="input-group-prepend">
                        <span class="input-group-text"><i class="bi bi-folder"></i></span>
                    </div>
                    <asp:TextBox ID="txtResources" runat="server" CssClass="form-control" placeholder="Enter Resources" TextMode="MultiLine" Rows="5"></asp:TextBox>
                </div>
            </div>
        </div>

        <div class="form-row">
            <div class="form-group col-md-12">
                <label for="txtAssessmentCriteria">Assessment Criteria</label>
                <div class="input-group">
                    <div class="input-group-prepend">
                        <span class="input-group-text"><i class="bi bi-clipboard-check"></i></span>
                    </div>
                    <asp:TextBox ID="txtAssessmentCriteria" runat="server" CssClass="form-control" placeholder="Enter Assessment Criteria" TextMode="MultiLine" Rows="5"></asp:TextBox>
                </div>
            </div>
        </div>

        <div class="form-row">
            <div class="form-group col-md-12">
                <label for="txtLessonOutcome">Lesson Outcome</label>
                <div class="input-group">
                    <div class="input-group-prepend">
                        <span class="input-group-text"><i class="bi bi-check-circle"></i></span>
                    </div>
                    <asp:TextBox ID="txtLessonOutcome" runat="server" CssClass="form-control" placeholder="Enter Lesson Outcome" TextMode="MultiLine" Rows="5"></asp:TextBox>
                </div>
            </div>
        </div>

        <div class="form-row">
            <div class="form-group col-md-4">
                <label for="txtDeliveryTime">Delivery Time</label>
                <div class="input-group">
                    <div class="input-group-prepend">
                        <span class="input-group-text"><i class="bi bi-calendar"></i></span>
                    </div>
                    <asp:TextBox ID="txtDeliveryTime" runat="server" CssClass="form-control" placeholder="Enter Delivery Time" TextMode="DateTimeLocal"></asp:TextBox>
                </div>
            </div>

            <div class="form-group col-md-4">
                <label for="ddlCheckStatus">Check Status</label>
                <div class="input-group">
                    <div class="input-group-prepend">
                        <span class="input-group-text"><i class="bi bi-check2-circle"></i></span>
                    </div>
                    <asp:DropDownList ID="ddlCheckStatus" runat="server" CssClass="form-control">
                        <asp:ListItem Value="Not Checked" Text="Not Checked"></asp:ListItem>
                        <asp:ListItem Value="Checked" Text="Checked"></asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>

            <div class="form-group col-md-4">
                <label for="txtCheckdDate">Checked Date</label>
                <div class="input-group">
                    <div class="input-group-prepend">
                        <span class="input-group-text"><i class="bi bi-calendar"></i></span>
                    </div>
                    <asp:TextBox ID="txtCheckdDate" runat="server" CssClass="form-control" placeholder="Enter Check Date" TextMode="DateTimeLocal"></asp:TextBox>
                </div>
            </div>
        </div>

        <div class="form-row">
            <div class="form-group col-md-12">
                <label for="txtLessonEvaluation">Lesson Evaluation</label>
                <div class="input-group">
                    <div class="input-group-prepend">
                        <span class="input-group-text"><i class="bi bi-clipboard-check"></i></span>
                    </div>
                    <asp:TextBox ID="txtLessonEvaluation" runat="server" CssClass="form-control" placeholder="Enter Lesson Evaluation" TextMode="MultiLine" Rows="5"></asp:TextBox>
                </div>
            </div>
        </div>

        <div class="form-group text-center">
            <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-dark-blue btn-block" Text="Submit" OnClick="btnSubmit_Click" />
        </div>
    </div>
</asp:Content>