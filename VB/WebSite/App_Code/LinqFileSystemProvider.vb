Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports DevExpress.Web.ASPxFileManager
Imports System.IO

Public Class LinqFileSystemProvider
    Inherits FileSystemProviderBase
    Private Const DbRootItemId As Integer = 1
    Private dataContext_Renamed As DbFileSystemDataContext
    Private folderCache_Renamed As Dictionary(Of Integer, DbFileSystemItem)
    Private rootFolderDisplayName_Renamed As String

    Public Sub New(ByVal rootFolder As String)
        MyBase.New(rootFolder)
        Me.dataContext_Renamed = New DbFileSystemDataContext()
        RefreshFolderCache()
    End Sub

    Public ReadOnly Property DataContext() As DbFileSystemDataContext
        Get
            Return dataContext_Renamed
        End Get
    End Property

    ' Used to decrease the number of recursive LINQ to SQL queries made to a database
    Public ReadOnly Property FolderCache() As Dictionary(Of Integer, DbFileSystemItem)
        Get
            Return folderCache_Renamed
        End Get
    End Property


    Public Overrides ReadOnly Property RootFolderDisplayName() As String
        Get
            Return rootFolderDisplayName_Renamed
        End Get
    End Property

    Public Overrides Function GetFiles(ByVal folder As FileManagerFolder) As IEnumerable(Of FileManagerFile)
        Dim dbFolderItem As DbFileSystemItem = FindDbFolderItem(folder)
        Return _
         From dbItem In DataContext.DbFileSystemItems _
         Where (Not dbItem.IsFolder) AndAlso dbItem.ParentId = dbFolderItem.Id _
         Select New FileManagerFile(Me, folder, dbItem.Name)
    End Function
    Public Overrides Function GetFolders(ByVal parentFolder As FileManagerFolder) As IEnumerable(Of FileManagerFolder)
        Dim dbFolderItem As DbFileSystemItem = FindDbFolderItem(parentFolder)
        Return _
         From dbItem In FolderCache.Values _
         Where dbItem.IsFolder AndAlso dbItem.ParentId = dbFolderItem.Id _
         Select New FileManagerFolder(Me, parentFolder, dbItem.Name)
    End Function
    Public Overloads Overrides Function Exists(ByVal file As FileManagerFile) As Boolean
        Return FindDbFileItem(file) IsNot Nothing
    End Function
    Public Overloads Overrides Function Exists(ByVal folder As FileManagerFolder) As Boolean
        Return FindDbFolderItem(folder) IsNot Nothing
    End Function
    Public Overrides Function ReadFile(ByVal file As FileManagerFile) As System.IO.Stream
        Return New MemoryStream(FindDbFileItem(file).Data.ToArray())
    End Function
    Public Overrides Function GetLastWriteTime(ByVal file As FileManagerFile) As DateTime
        Dim dbFileItem As DbFileSystemItem = FindDbFileItem(file)
        Return dbFileItem.LastWriteTime.GetValueOrDefault(DateTime.Now)
    End Function

    ' File/folder management operations
    Public Overrides Sub CreateFolder(ByVal parent As FileManagerFolder, ByVal name As String)
        Dim dbItem As DbFileSystemItem = FindDbFolderItem(parent)
        DataContext.DbFileSystemItems.InsertOnSubmit(New DbFileSystemItem() With {.IsFolder = True, .LastWriteTime = DateTime.Now, .Name = name, .ParentId = dbItem.Id})
        SubmitChanges()
    End Sub
    Public Overrides Sub DeleteFile(ByVal file As FileManagerFile)
        Dim dbItem As DbFileSystemItem = FindDbFileItem(file)
        DataContext.DbFileSystemItems.DeleteOnSubmit(dbItem)
        SubmitChanges()
    End Sub
    Public Overrides Sub DeleteFolder(ByVal folder As FileManagerFolder)
        Dim dbItem As DbFileSystemItem = FindDbFolderItem(folder)
        DataContext.DbFileSystemItems.DeleteOnSubmit(dbItem)
        SubmitChanges()
    End Sub
    Public Overrides Sub MoveFile(ByVal file As FileManagerFile, ByVal newParentFolder As FileManagerFolder)
        Dim dbItem As DbFileSystemItem = FindDbFileItem(file)
        dbItem.ParentId = FindDbFolderItem(newParentFolder).Id
        SubmitChanges()
    End Sub
    Public Overrides Sub MoveFolder(ByVal folder As FileManagerFolder, ByVal newParentFolder As FileManagerFolder)
        Dim dbItem As DbFileSystemItem = FindDbFolderItem(folder)
        dbItem.ParentId = FindDbFolderItem(newParentFolder).Id
        SubmitChanges()
    End Sub
    Public Overrides Sub RenameFile(ByVal file As FileManagerFile, ByVal name As String)
        Dim dbItem As DbFileSystemItem = FindDbFileItem(file)
        dbItem.Name = name
        SubmitChanges()
    End Sub
    Public Overrides Sub RenameFolder(ByVal folder As FileManagerFolder, ByVal name As String)
        Dim dbItem As DbFileSystemItem = FindDbFolderItem(folder)
        dbItem.Name = name
        SubmitChanges()
    End Sub
    Public Overrides Sub UploadFile(ByVal folder As FileManagerFolder, ByVal fileName As String, ByVal content As Stream)
        Dim dbItem As DbFileSystemItem = FindDbFolderItem(folder)
        DataContext.DbFileSystemItems.InsertOnSubmit(New DbFileSystemItem() With {.Name = fileName, .Data = ReadAllBytes(content), .ParentId = dbItem.Id, .LastWriteTime = DateTime.Now, .IsFolder = False})
        SubmitChanges()
    End Sub
    Protected Sub SubmitChanges()
        DataContext.SubmitChanges()
        RefreshFolderCache()
    End Sub
    Protected Function FindDbFileItem(ByVal file As FileManagerFile) As DbFileSystemItem
        Dim dbFolderItem As DbFileSystemItem = FindDbFolderItem(file.Folder)
        If dbFolderItem Is Nothing Then
            Return Nothing
        End If
        Return ( _
          From dbItem In DataContext.DbFileSystemItems _
          Where dbItem.ParentId = dbFolderItem.Id AndAlso (Not dbItem.IsFolder) AndAlso dbItem.Name = file.Name _
          Select dbItem).FirstOrDefault()
    End Function
    Protected Function FindDbFolderItem(ByVal folder As FileManagerFolder) As DbFileSystemItem
        Return ( _
          From dbItem In FolderCache.Values _
          Where dbItem.IsFolder AndAlso GetRelativeName(dbItem) = folder.RelativeName _
          Select dbItem).FirstOrDefault()
    End Function

    ' Returns the path to a specified folder relative to a root folder
    Protected Function GetRelativeName(ByVal dbFolder As DbFileSystemItem) As String
        If dbFolder.Id = DbRootItemId Then
            Return String.Empty
        End If
        If dbFolder.ParentId = DbRootItemId Then
            Return dbFolder.Name
        End If
        If (Not FolderCache.ContainsKey(dbFolder.ParentId)) Then
            Return Nothing
        End If
        Dim name As String = GetRelativeName(FolderCache(dbFolder.ParentId))
        Return If(name Is Nothing, Nothing, Path.Combine(name, dbFolder.Name))
    End Function

    ' Caches folder names in memory and obtains a root folder's name
    Protected Sub RefreshFolderCache()
        Me.folderCache_Renamed = ( _
          From dbItem In DataContext.DbFileSystemItems _
          Where dbItem.IsFolder _
          Select dbItem).ToDictionary(Function(i) i.Id)

        Me.rootFolderDisplayName_Renamed = ( _
          From dbItem In FolderCache.Values _
          Where dbItem.Id = DbRootItemId _
          Select dbItem.Name).First()
    End Sub
    Protected Shared Function ReadAllBytes(ByVal stream As Stream) As Byte()
        Dim buffer(16 * 1024 - 1) As Byte
        Dim readCount As Integer
        Using ms As New MemoryStream()
            readCount = stream.Read(buffer, 0, buffer.Length)
            Do While readCount > 0
                ms.Write(buffer, 0, readCount)
                readCount = stream.Read(buffer, 0, buffer.Length)
            Loop
            Return ms.ToArray()
        End Using
    End Function
End Class
