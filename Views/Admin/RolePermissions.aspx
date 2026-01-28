<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="RolePermissions.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.RolePermissions" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .dark-blue-header {
            background-color: #0a1e2b; /* Very Dark Blue for the card header */
            color: white;
        }
        .container-centered {
            padding-left: 5%;
            padding-right: 3%;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-centered mt-5">
        <div class="card shadow-lg">
            <div class="card-header dark-blue-header">
                <h4 class="mb-0">Manage Role Permissions</h4>
            </div>
            <div class="card-body">
                <div class="row mb-3">
                    <!-- DropDownList for Roles in the first column (8 columns wide) -->
                    <div class="col-md-8">
                        <label for="ddlRoles" class="form-label">Select Role:</label>
                        <asp:DropDownList ID="ddlRoles" runat="server" CssClass="form-control"  AutoPostBack="true" OnSelectedIndexChanged="ddlRoles_SelectedIndexChanged"></asp:DropDownList>
                    </div>
                    <!-- Submit button in the second column (4 columns wide) -->
                    <div class="col-md-4 text-end">
                        <asp:Button ID="btnSave" runat="server" Text="Save Permissions" CssClass="btn btn-dark-blue" OnClick="btnSave_Click" />
                    </div>
                </div>

                <div class="mb-3">
                    <label class="form-label">Permissions:</label>
                    <asp:Repeater ID="rptModules" runat="server" OnItemDataBound="rptModules_ItemDataBound">
                        <HeaderTemplate>
                            <div class="container">
                                <div class="row">
                        </HeaderTemplate>

                        <ItemTemplate>
                            <%# Container.ItemIndex % 3 == 0 ? "</div><div class='row'>" : "" %>
                            <div class="col-md-4">
                                <div class="card mb-3">
                                    <div class="card-header bg-secondary text-white">
                                        <h5 class="mb-0"><%# Eval("ModuleName") %></h5>
                                    </div>
                                    <div class="card-body">
                                        <asp:CheckBoxList ID="cblModulePermissions" runat="server" CssClass="form-check"></asp:CheckBoxList>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>

                        <FooterTemplate>
                                </div> <%-- Closing row --%>
                            </div> <%-- Closing container --%>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>

            </div>
        </div>
    </div>
</asp:Content>
