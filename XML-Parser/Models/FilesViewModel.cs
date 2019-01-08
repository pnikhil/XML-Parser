
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using XML_Parser.Helpers;
namespace XML_Parser.Models
{
    public class FilesViewModel
    {
        public ViewDataUploadFilesResult[] Files { get; set; }
    }
}