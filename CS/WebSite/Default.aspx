<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%@ Register Assembly="DevExpress.Web.v11.1" Namespace="DevExpress.Web.ASPxFileManager"
    TagPrefix="dx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <dx:ASPxFileManager ID="ASPxFileManager1" runat="server" 
            onfileuploading="ASPxFileManager1_FileUploading" 
            onfoldercreating="ASPxFileManager1_FolderCreating" 
            onitemdeleting="ASPxFileManager1_ItemDeleting" 
            onitemmoving="ASPxFileManager1_ItemMoving"
            onitemrenaming="ASPxFileManager1_ItemRenaming" 
        CustomFileSystemProviderTypeName="LinqFileSystemProvider" >
            <Settings RootFolder="a1" ThumbnailFolder="~\Thumb\" />
            <SettingsDataSource  />
            <SettingsEditing AllowCreate="True" AllowDelete="True" AllowMove="True" AllowRename="True" />
        </dx:ASPxFileManager>
    </form>
</body>
</html>
