<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%@ Register Assembly="DevExpress.Web.v13.2, Version=13.2.0.0, Culture=neutral, PublicKeyToken=79868b8147b5eae4" Namespace="DevExpress.Web.ASPxFileManager" TagPrefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <dx:ASPxFileManager ID="ASPxFileManager1" runat="server"
            OnFileUploading="ASPxFileManager1_FileUploading"
            OnFolderCreating="ASPxFileManager1_FolderCreating"
            OnItemDeleting="ASPxFileManager1_ItemDeleting"
            OnItemMoving="ASPxFileManager1_ItemMoving"
            OnItemRenaming="ASPxFileManager1_ItemRenaming"
            OnItemCopying="ASPxFileManager1_ItemCopying"
            CustomFileSystemProviderTypeName="LinqFileSystemProvider">
            <Settings RootFolder="a1" ThumbnailFolder="~\Thumb\" />
            <SettingsDataSource />
            <SettingsEditing AllowCreate="True" AllowDelete="True" AllowMove="True" AllowRename="True" AllowCopy="true" />
            <SettingsFileList View="Thumbnails"></SettingsFileList>
        </dx:ASPxFileManager>
    </form>
</body>
</html>
