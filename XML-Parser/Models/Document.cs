using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XML_Parser.Models
{
    public class Document
    {
        public List<Tag> Tags { get; set; }
        public bool IsGlobal { get; set; }

        public bool IsOthers { get; set; }

        public String Description { get; set; }

        public GlobalResources GlobalResources { get; set; }

        public List<SqlData> SqlData { get; set; }
    }

    public class Tag
    {
        public string tagName { get; set; }
        public Dictionary<string, string> attributeKeyValue { get; set; }
        public bool hasChildTag { get; set; }

        public List<ChildTag> childTagList { get; set; }

    }

    public class ChildTag
    {
        public string childTagname { get; set; }

        public int childTagCount { get; set; }

        public bool hasGrandchildTag { get; set; }
        public Dictionary<string, string> childAttributeKeyValue { get; set; }

        public List<GrandchildTag> grandchildTagList { get; set; }
    }

    public class GrandchildTag
    {
        public string grandchildTagname { get; set; }
        public int grandchildTagCount { get; set; }
        public Dictionary<string,string> grandchildAttributeKeyValue { get; set; }
    }

    public class GlobalResources
    {
        public List<DbConfig> DbConfig { get; set; }
        public List<WebServiceConfig> WebServiceConfig { get; set; }
    }

    public class DbConfig
    {
        public string Dbname { get; set; }

        [DisplayFormat(NullDisplayText = "", ApplyFormatInEditMode = true)]
        public string DbUrl { get; set; }
        public string DbType { get; set; }
    }

    public class WebServiceConfig
    {
        public string WsName { get; set; }
        public string WsService { get; set; }
        public string WsPort { get; set; }
        public string wsdlLocation { get; set; }
        public string WsconnectorConfig { get; set; }
    }

    public class SqlData
    {
        public string sqlQuery { get; set; }

        public Boolean isStoredProcedure { get; set; }

        public string databaseType { get; set; }
        public int numberOfTables { get; set; }
        public int numberOfColumns { get; set; }
        public List<string> tableNames { get; set; }
        public List<string> columnNames { get; set; }
        public string storedProcedure {get;set;}
        public string operation { get; set; }

    }
}