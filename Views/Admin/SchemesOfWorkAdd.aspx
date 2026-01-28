<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Master.Master" AutoEventWireup="true" CodeBehind="SchemesOfWorkAdd.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.SchemesOfWorkAdd" %>
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
    </style>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-icons/1.4.0/font/bootstrap-icons.min.css">
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <!-- Success Modal -->
    <div class="modal fade" id="successModal" tabindex="-1" role="dialog" CssClass="successMessage" aria-labelledby="successModalLabel" aria-hidden="true">
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
        <div class="form-group col-md-4">
            <label for="ddlSubject">Subject</label>
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="bi bi-book"></i></span>
                </div>
                <asp:DropDownList ID="ddlSubject" runat="server" CssClass="form-control"></asp:DropDownList>
            </div>
        </div>

        <div class="form-group col-md-4">
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
        <div class="form-group col-md-4">
            <label for="txtLessons">Number of Lessons Plans</label>
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="bi bi-bookmark"></i></span>
                </div>
                <asp:TextBox ID="txtLessons" runat="server" CssClass="form-control" placeholder="Enter Number of Lesson Plans" required></asp:TextBox>
            </div>
        </div>
    </div>

    <div class="form-row">
        <div class="form-group col-md-12">
            <label for="txtTopic">Topic</label>
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="bi bi-card-text"></i></span>
                </div>
                <asp:TextBox ID="txtTopic" runat="server" CssClass="form-control" placeholder="Enter Topic" TextMode="MultiLine" Rows="1" required></asp:TextBox>
            </div>
        </div>
    </div>

    <div class="form-row">
        <div class="form-group col-md-12">
            <label for="txtLearningObjectives">Success Criteria</label>
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="bi bi-bullseye"></i></span>
                </div>
                <asp:TextBox ID="txtLearningObjectives" runat="server" CssClass="form-control" placeholder="Enter the Success Criteria" TextMode="MultiLine" Rows="5" required></asp:TextBox>
            </div>
        </div>
        <div class="form-group col-md-12">
            <label for="txtLearningOutcome">Planned Activities</label>
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="bi bi-check-circle"></i></span>
                </div>
                <asp:TextBox ID="txtLearningOutcome" runat="server" CssClass="form-control" placeholder="Enter Planned Activities" TextMode="MultiLine" Rows="5" required></asp:TextBox>
            </div>
        </div>
        <div class="form-group col-md-12">
            <label for="txtResources">Learning and Assessment Resources</label>
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="bi bi-folder"></i></span>
                </div>
                <asp:TextBox ID="txtResources" runat="server" CssClass="form-control" placeholder="Enter Learning and Asessment Resources" TextMode="MultiLine" Rows="5"></asp:TextBox>
            </div>
        </div>      
        <div class="form-group col-md-12">
            <label for="txtSchemeEvaluation">Outcome</label>
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="bi bi-clipboard-check"></i></span>
                </div>
                <asp:TextBox ID="txtSchemeEvaluation" runat="server" CssClass="form-control" placeholder="Enter Schemes of work Outcome" Rows="5" TextMode="MultiLine"></asp:TextBox>
            </div>
        </div>
    </div>

    <div class="form-row">
        <div class="form-group col-md-6">
            <label for="txtReferences">References</label>
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="bi bi-book-half"></i></span>
                </div>
                <asp:TextBox ID="txtReferences" runat="server" CssClass="form-control" placeholder="Enter References" TextMode="MultiLine" Rows="3"></asp:TextBox>
            </div>
        </div>
        <div class="form-group col-md-6">
            <label for="txtRemarks">Remarks</label>
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="bi bi-chat-left-text"></i></span>
                </div>
                <asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control" placeholder="Enter Remarks" TextMode="MultiLine" Rows="3"></asp:TextBox>
            </div>
        </div>
    </div>

    <div class="form-row">
        <div class="form-group col-md-4">
            <label for="ddlCompleteStatus">Complete Status</label>
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="bi bi-check2-circle"></i></span>
                </div>
                <asp:DropDownList ID="ddlCompleteStatus" runat="server" CssClass="form-control">
                    <asp:ListItem Value="Pending" Text="Pending"></asp:ListItem>
                    <asp:ListItem Value="In Progress" Text="In Progress"></asp:ListItem>
                    <asp:ListItem Value="Completed" Text="Completed"></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="form-group col-md-4">
            <label for="ddlTerm">Term</label>
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="bi bi-calendar"></i></span>
                </div>
                <asp:DropDownList ID="ddlTerm" runat="server" CssClass="form-control"></asp:DropDownList>
            </div>
        </div>
        <div class="form-group col-md-4">
            <label for="txtCompletedDate">Completed Date</label>
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="bi bi-calendar"></i></span>
                </div>
                <asp:TextBox ID="txtCompletedDate" runat="server" CssClass="form-control" placeholder="Enter Completed Date" TextMode="Date" required></asp:TextBox>
            </div>
        </div>
    </div>

    <div class="form-group text-center">
        <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-dark-blue btn-block" Text="Submit" OnClick="btnSubmit_Click" />
    </div>
</div></asp:Content>
