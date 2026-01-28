<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="StudentReg.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.StudentReg" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        <style>
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
</style>

   
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <script>
        document.getElementById('<%= fuImage.ClientID %>').addEventListener('change', function (e) {
            const file = e.target.files[0];
            if (file) {
                const reader = new FileReader();
                reader.onload = function (event) {
                    document.getElementById('<%= imgPreview.ClientID %>').src = event.target.result;
                };
                reader.readAsDataURL(file);
            }
        });

        function openCamera() {
            $('#cameraModal').modal('show');
            const constraints = { video: true };
            navigator.mediaDevices.getUserMedia(constraints)
                .then((stream) => {
                    const video = document.getElementById('cameraVideo');
                    video.srcObject = stream;
                    video.play();

                    document.getElementById('captureButton').onclick = () => {
                        const canvas = document.createElement('canvas');
                        canvas.width = video.videoWidth;
                        canvas.height = video.videoHeight;
                        canvas.getContext('2d').drawImage(video, 0, 0);
                        const dataUrl = canvas.toDataURL('image/png');
                        document.getElementById('<%= imgPreview.ClientID %>').src = dataUrl;
                        document.getElementById('capturedImage').value = dataUrl; // Store base64 string in hidden field
                        stream.getTracks().forEach(track => track.stop());
                        $('#cameraModal').modal('hide');
                    };
                })
                .catch((error) => {
                    console.error('Error accessing media devices.', error);
                });
        }
    </script></asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mt-5">
        <div class="card">
            <div class="card-header text-dark-blue">
                <h6>Student Registration</h6>
            </div>
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

<div class="modal fade" id="successModal" tabindex="-1" role="dialog" CssClass="successMessage" aria-labelledby="successModalLabel" aria-hidden="true">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" CssClass="successMessage" id="successModalLabel">Success</h5>
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



            <div class="card-body">
                <div class="form-row">
                    <div class="form-group col-md-3">
                        <label for="txtFirstName">First Name</label>
                        <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" placeholder="Enter First Name" required></asp:TextBox>
                    </div>
                    <div class="form-group col-md-3">
                        <label for="txtLastName">Last Name</label>
                        <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" placeholder="Enter Last Name" required></asp:TextBox>
                    </div>
                      <div class="form-group col-md-3">
                        <label for="txtRegCode">Reg Code (Optional)</label>
                        <asp:TextBox ID="txtRegCode" runat="server" CssClass="form-control" placeholder="Enter Reg Code if available " ></asp:TextBox>
                    </div>
                    <div class="form-group col-md-3">
                        <label for="ddlGender">Gender</label>
                        <asp:DropDownList ID="ddlGender" runat="server" CssClass="form-control" placeholder="Select Gender"></asp:DropDownList>
                    </div>
                    <div class="form-group col-md-3">
                        <label for="ddlStatus">Status</label>
                        <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control" placeholder="Select Status" ></asp:DropDownList>
                    </div>
                </div>
                <div class="form-row">
                    <div class="form-group col-md-3">
                        <label for="txtGuardian">Guardian</label>
                        <asp:TextBox ID="txtGuardian" runat="server" CssClass="form-control" placeholder="Enter Guardian" required></asp:TextBox>
                    </div>
                    <div class="form-group col-md-3">
                        <label for="txtPhone">Phone</label>
                        <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" placeholder="Enter Phone" required></asp:TextBox>
                    </div>
                    <div class="form-group col-md-3">
                        <label for="txtAddress">Address</label>
                        <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" placeholder="Enter Address"></asp:TextBox>
                    </div>
                      <div class="form-group col-md-3">
                        <label for="txtEmail">Email</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Enter Email"></asp:TextBox>
                    </div>

                </div>
                <div class="form-row">
                    <div class="form-group col-md-3">
                        <label for="txtUserName">Username</label>
                        <asp:TextBox ID="txtUserName" runat="server" CssClass="form-control" placeholder="Enter Username" required></asp:TextBox>
                    </div>
                    <div class="form-group col-md-3">
                        <label for="txtPassword">Password</label>
                        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Enter Password"></asp:TextBox>
                    </div>
                    <div class="form-group col-md-3">
                        <label for="fuImage">Upload Image</label>
                        <asp:FileUpload ID="fuImage" runat="server" CssClass="form-control" />
                        </div>
                    <asp:Image ID="imgPreview" runat="server" CssClass="img-thumbnail mt-2" Width="100px" Height="80px" />
                    <button type="button" class="btn btn-dark-blue mt-2 btn-small" onclick="openCamera()">Capture Image</button>

                    </div>

                </div>
                <div class="form-row">
                </div>
                <div class="form-group text-center">
                    <asp:Button ID="btnSubmit" runat="server"  type="submit" CssClass="btn btn-dark-blue btn-dark-blue:hover btn-block" Text="Submit" OnClick="btnSubmit_Click" />
                </div>

            </div>
        </div>
   

    <!-- Camera Modal -->
    <div class="modal fade" id="cameraModal" tabindex="-1" role="dialog" aria-labelledby="cameraModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="cameraModalLabel">Capture Image</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <video id="cameraVideo" width="100%" autoplay></video>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-primary" id="captureButton">Capture</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
