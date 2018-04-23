using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxFileManager;

public partial class _Default : System.Web.UI.Page 
{
    protected void ASPxFileManager1_ItemDeleting(object source, DevExpress.Web.ASPxFileManager.FileManagerItemDeleteEventArgs e) {
        ValidateSiteMode(e);
    }
    protected void ASPxFileManager1_ItemMoving(object source, DevExpress.Web.ASPxFileManager.FileManagerItemMoveEventArgs e) {
        ValidateSiteMode(e);
    }
    protected void ASPxFileManager1_ItemRenaming(object source, DevExpress.Web.ASPxFileManager.FileManagerItemRenameEventArgs e) {
        ValidateSiteMode(e);
    }
    protected void ASPxFileManager1_FolderCreating(object source, DevExpress.Web.ASPxFileManager.FileManagerFolderCreateEventArgs e) {
        ValidateSiteMode(e);
    }
    protected void ASPxFileManager1_FileUploading(object source, DevExpress.Web.ASPxFileManager.FileManagerFileUploadEventArgs e) {
        ValidateSiteMode(e);
    }
    protected void ValidateSiteMode(FileManagerActionEventArgsBase e) {
        e.Cancel = true;
        e.ErrorText = "Data modifications are not allowed in the example.";
    }
}
