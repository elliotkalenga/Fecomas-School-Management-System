<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mt-5">
        <!-- Add Student Button -->
        <button type="button" class="btn btn-primary" data-toggle="modal" data-target="#studentModal">Add Student</button>

        <!-- Student Registration Modal -->
        <div class="modal fade" id="studentModal" tabindex="-1" role="dialog" aria-labelledby="studentModalLabel" aria-hidden="true">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="studentModalLabel">Student Registration</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <iframe id="studentIframe" src="StudentAdd.aspx" width="100%" height="400px" frameborder="0"></iframe>
                    </div>
                </div>
            </div>
        </div>


    </div>
</asp:Content>
