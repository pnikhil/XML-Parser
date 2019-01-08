using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using XML_Parser.Constants;
namespace XML_Parser.Helpers
{
    public class FilesHelper
    {
       
        String DeleteURL = null;
        String DeleteType = null;
        String StorageRoot = null;
        String UrlBase = null;
        String tempPath = null;
        String serverMapPath = null;

        public FilesHelper(String DeleteURL, String DeleteType, String StorageRoot, String UrlBase, String tempPath, String serverMapPath)
        {
            this.DeleteURL = DeleteURL;
            this.DeleteType = DeleteType;
            this.StorageRoot = StorageRoot;
            this.UrlBase = UrlBase;
            this.tempPath = tempPath;
            this.serverMapPath = serverMapPath;
        }

        public bool CreateSampleFile(String file)
        {
            var a = "someString";
            if (a == null)
            {

            }
            String fullPath = Path.Combine(StorageRoot, file);
            System.Diagnostics.Debug.WriteLine(fullPath);
            System.Diagnostics.Debug.WriteLine(System.IO.File.Exists(fullPath));

            if (!File.Exists(fullPath))
            {

                File.Create(fullPath).Dispose(); ;
                TextWriter tw = new StreamWriter(fullPath);
                tw.WriteLine("The very first line!");
                tw.Close();
                return true;
            }
            return false;
        }

        public String DeleteFile(String file)
        {
            System.Diagnostics.Debug.WriteLine("DeleteFile");
            System.Diagnostics.Debug.WriteLine(file);
 
            String fullPath = Path.Combine(StorageRoot, file);
            System.Diagnostics.Debug.WriteLine(fullPath);
            System.Diagnostics.Debug.WriteLine(System.IO.File.Exists(fullPath));

            if (System.IO.File.Exists(fullPath))
            {
                
                System.IO.File.Delete(fullPath);
                String succesMessage = "Ok";
                return succesMessage;
            }
            String failMessage = "Error Delete";
            return failMessage;
        }

        
        public JsonFiles GetFileList()
        {

            var r = new List<ViewDataUploadFilesResult>();
       
            String fullPath = Path.Combine(StorageRoot);
            if (Directory.Exists(fullPath))
            {
                DirectoryInfo dir = new DirectoryInfo(fullPath);
                foreach (FileInfo file in dir.GetFiles())
                {
                    int SizeInt = unchecked((int)file.Length);
                    r.Add(UploadResult(file.Name,SizeInt,file.FullName));
                }

            }
            JsonFiles files = new JsonFiles(r);

            return files;
        }

        public void UploadAndShowResults(HttpContextBase ContentBase, List<ViewDataUploadFilesResult> resultList)
        {
            var httpRequest = ContentBase.Request;
            System.Diagnostics.Debug.WriteLine(Directory.Exists(tempPath));

            String fullPath = Path.Combine(StorageRoot);
            Directory.CreateDirectory(fullPath);
            // Create new folder for thumbs
           

            foreach (String inputTagName in httpRequest.Files)
            {

                var headers = httpRequest.Headers;

                var file = httpRequest.Files[inputTagName];
                System.Diagnostics.Debug.WriteLine(file.FileName);

                if (string.IsNullOrEmpty(headers["X-File-Name"]))
                {

                    UploadWholeFile(ContentBase, resultList);
                }
               
            }
        }


        private void UploadWholeFile(HttpContextBase requestContext, List<ViewDataUploadFilesResult> statuses)
        {

            var request = requestContext.Request;
            for (int i = 0; i < request.Files.Count; i++)
            {
                var file = request.Files[i];
                String pathOnServer = Path.Combine(StorageRoot);
                var fullPath = Path.Combine(pathOnServer, Path.GetFileName(file.FileName));
                file.SaveAs(fullPath);

                bool dataPresent = File.ReadAllText(fullPath).Contains("version=\"1.1\"") || File.ReadAllText(fullPath).Contains("version=\'1.1\'");
                if (dataPresent)
                {
                    string data = File.ReadAllText(fullPath)
                        .Replace("1.1", "1.0");
                    byte[] bytes = Encoding.ASCII.GetBytes(data);
                    File.WriteAllBytes(fullPath, bytes);
                }

                statuses.Add(UploadResult(file.FileName, file.ContentLength, file.FileName));
            }
        }


        public ViewDataUploadFilesResult UploadResult(String FileName,int fileSize,String FileFullPath)
        {

            String fileDescription = GetFileDescription(FileName);
            String getType = System.Web.MimeMapping.GetMimeMapping(FileFullPath);
            var result = new ViewDataUploadFilesResult()
            {
                name = FileName,
                size = fileSize,
                type = getType,
                url = UrlBase + FileName,
                deleteUrl = DeleteURL + FileName,
                //thumbnailUrl = CheckThumb(getType, FileName),
                deleteType = DeleteType,
                description = fileDescription
            };
            return result;
        }

        public string GetFileDescription(string fileName)
        {
            if (fileName.Contains("global"))
            {
                return Constants.Constants.GlobalFileDescription;
            }
            else if (fileName.Contains("api"))
            {
                return Constants.Constants.ApiFileDescription;
            }
            else
            {
                return Constants.Constants.OtherFileDescription;
            }
            
        }
    }
    public class ViewDataUploadFilesResult
    {
        public string name { get; set; }
        public int size { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public string deleteUrl { get; set; }
        public string deleteType { get; set; }
        public string description { get; set; }
    }
    public class JsonFiles
    {
        public ViewDataUploadFilesResult[] files;
        public string TempFolder { get; set; }
        public JsonFiles(List<ViewDataUploadFilesResult> filesList)
        {
            files = new ViewDataUploadFilesResult[filesList.Count];
            for (int i = 0; i < filesList.Count; i++)
            {
                files[i] = filesList.ElementAt(i);
            }

        }
    }
}

    