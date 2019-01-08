using XML_Parser.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Hosting;
using System.Xml;
using System.Web.Script.Serialization;
using System.Net.Http;
using System.Threading.Tasks;

using System.Configuration;
using System.Web.Configuration;

namespace XML_Parser.Helpers
{
    public class XmlHelper
    {

        private static readonly HttpClient _Client = new HttpClient();
        private static JavaScriptSerializer _Serializer = new JavaScriptSerializer();


        public static async Task<SqlData> Run(string query)
        {
            string url = WebConfigurationManager.AppSettings["RestAPILink"];
            SqlData cust = new SqlData() { sqlQuery = query };
            var json = _Serializer.Serialize(cust);
            var response = await Request(HttpMethod.Post, url, json, new Dictionary<string, string>());
            string responseText = await response.Content.ReadAsStringAsync();
            SqlData sqlData = new JavaScriptSerializer().Deserialize<SqlData>(responseText);
            return sqlData;
        }

        //public static async Task GetSqlDetailsAsync(string sqlQuery)
        //{
        //    Run(sqlQuery);
        //    //SqlData.Add(sqlData);
        //    document.SqlData = SqlData;
        //}

        public static async Task<HttpResponseMessage> Request(HttpMethod pMethod, string pUrl, string pJsonContent, Dictionary<string, string> pHeaders)
        {
            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = pMethod;
            httpRequestMessage.RequestUri = new Uri(pUrl);
            foreach (var head in pHeaders)
            {
                httpRequestMessage.Headers.Add(head.Key, head.Value);
            }
            switch (pMethod.Method)
            {
                case "POST":
                    HttpContent httpContent = new StringContent(pJsonContent, Encoding.UTF8, "application/json");
                    httpRequestMessage.Content = httpContent;
                    break;

            }

            var result = _Client.SendAsync(httpRequestMessage).Result;
            return result;
        }
        public static async Task<Document> ParseXmlAsync(XmlDocument xmlFile, string fileName)
        {
            XmlNode root = xmlFile.DocumentElement;
            XmlNodeList nodes = root.ChildNodes;
            Document document = new Document();
            List<SqlData> SqlData = new List<SqlData>();

            document.Tags = new List<Tag>();

            document.GlobalResources = null;
            //Dictionary<string, Dictionary<string,string>> nodeMap = new Dictionary<string, Dictionary<string, string>>();
            //Dictionary<Dictionary<string, string>, Dictionary<string, string>> childNodeMap = new Dictionary<Dictionary<string, string>, Dictionary<string, string>>();
            if (fileName.Contains("global"))
            {
                document.IsGlobal = true;
                document.GlobalResources = new GlobalResources();
                document.GlobalResources.DbConfig = new List<DbConfig>();
                document.GlobalResources.WebServiceConfig = new List<WebServiceConfig>();
            }

            foreach (XmlNode node in nodes)
            {
                //nodeMap[node.Name] = new Dictionary<string, string>();
                Tag tag = new Tag();
                Console.WriteLine(node.Name);
                tag.tagName = node.Name;
                Console.WriteLine("----------------------");
                if (node.Attributes != null)
                {
                    XmlAttributeCollection attributes = node.Attributes;
                    tag.attributeKeyValue = new Dictionary<string, string>();
                    bool addNameToNode = false;
                    bool dbConfigCreated = false;
                    bool wsConfigCreated = false;
                    DbConfig dbConfig = null;
                    WebServiceConfig wsConfig = null;
                    var AttributeCount = attributes.Count;

                    var i = 0;
                    foreach (XmlAttribute attribute in attributes)
                    {
                        i++;
                        tag.attributeKeyValue.Add(attribute.Name, attribute.Value);

                        if (node.Name.Equals("flow") || node.Name.Equals("sub-flow"))
                        {
                            if (!addNameToNode && attribute.Name.Equals("name"))
                            {
                                tag.tagName = node.Name + " (" + attribute.Value + ")";
                                addNameToNode = true;
                            }
                        }
                        else if (fileName.Contains("global"))
                        {

                            //ADDITIONAL CONDITION FOR GLOBAL.XML FILE
                            if (!addNameToNode && attribute.Name.Equals("name"))
                            {
                                tag.tagName = node.Name + " (" + attribute.Value + ")";
                                addNameToNode = true;
                            }

                            if (node.Name.StartsWith("db:"))
                            {
                                if (!dbConfigCreated)
                                {
                                    dbConfig = new DbConfig();
                                    dbConfigCreated = true;
                                    dbConfig.DbType = node.Name;
                                }

                                if (attribute.Name.Equals("name"))
                                {
                                    dbConfig.Dbname = attribute.Value;
                                }
                                else if (attribute.Name.Equals("url"))
                                {
                                    dbConfig.DbUrl = attribute.Value;

                                }
                            }
                            if (node.Name.StartsWith("ws:"))
                            {
                                if (!wsConfigCreated)
                                {
                                    wsConfig = new WebServiceConfig();
                                    wsConfigCreated = true;
                                }

                                if (attribute.Name.Equals("name"))
                                {
                                    wsConfig.WsName = attribute.Value;
                                }
                                else if (attribute.Name.Equals("service"))
                                {
                                    wsConfig.WsService = attribute.Value;
                                }
                                else if (attribute.Name.Equals("port"))
                                {
                                    wsConfig.WsPort = attribute.Value;
                                }
                                else if (attribute.Name.Equals("wsdlLocation"))
                                {
                                    wsConfig.wsdlLocation = attribute.Value;
                                }
                                else if (attribute.Name.Equals("connectorConfig"))
                                {
                                    wsConfig.WsconnectorConfig = attribute.Value;
                                }
                            }
                        }
                        else if (!fileName.Contains("global") && !fileName.Contains("api"))
                        {
                            continue;
                        }
                        Console.WriteLine(attribute.Name + " -> " + attribute.Value);
                        //nodeMap[node.Name][attribute.Name] = attribute.Value;
                        if (i == AttributeCount && node.Name.StartsWith("db:"))
                        {
                            document.GlobalResources.DbConfig.Add(dbConfig);
                        }

                        if (i == AttributeCount && node.Name.StartsWith("ws:"))
                        {
                            document.GlobalResources.WebServiceConfig.Add(wsConfig);
                        }
                    }

                    if (node.HasChildNodes)
                    {
                        tag.hasChildTag = true;

                        tag.childTagList = new List<ChildTag>();

                        XmlNodeList childNodes = node.ChildNodes;

                        foreach (XmlNode childNode in childNodes)
                        {
                            ChildTag childTag = new ChildTag();
                            XmlAttributeCollection childAttributes = childNode.Attributes;
                            if (childAttributes != null)
                            {
                                Console.WriteLine("\n");

                                if (childAttributes.Count == 0)
                                {
                                    break;
                                }

                                childTag.childTagname = childNode.Name;

                                // Console.WriteLine("Child node name: " + childNode.Name);
                                //Console.WriteLine("+++++++++++++++");

                                childTag.childTagCount = childAttributes.Count;
                                //else
                                //{
                                childTag.childAttributeKeyValue = new Dictionary<string, string>();
                                foreach (XmlAttribute attribute in childAttributes)
                                {
                                    Console.WriteLine(attribute.Name + " -> " + attribute.Value);
                                    childTag.childAttributeKeyValue.Add(attribute.Name, attribute.Value);



                                    //ADDITIONAL CHILD NODE CONDITION FOR GLOBAL.XML FILE
                                    if (attribute.Name.Equals("name") && fileName.Contains("global"))
                                    {
                                        childTag.childTagname = childNode.Name + " (" + attribute.Value + ")";
                                    }

                                    if (childNode.Name.Equals("db:select"))
                                    {

                                        if (attribute.Name.Equals("config-ref") && attribute.Value.Equals("Oracle_Configuration_BANNER", StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            if (childNode.HasChildNodes)
                                            {
                                                if (!childNode.InnerText.Equals("#[payload]", StringComparison.InvariantCultureIgnoreCase))
                                                {
                                                    XmlNodeList childOfChildNodes = childNode.ChildNodes;
                                                    foreach (XmlNode childofChildNode in childOfChildNodes)
                                                    {
                                                        //Console.WriteLine(childofChildNode.InnerText);
                                                        //Console.WriteLine(childofChildNode.Name);
                                                        if (childofChildNode.Name.Equals("db:parameterized-query"))
                                                        {

                                                            var sqlQuery = childNode.InnerText;
                                                            SqlData sqlData = await Run(sqlQuery);
                                                            sqlData.databaseType = attribute.Value;
                                                            sqlData.operation = childNode.Name;
                                                            SqlData.Add(sqlData);
                                                            //Task.Run(() => Run(sqlQuery)).Wait();                                                                                                               
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (childNode.Name.Equals("db:stored-procedure") && attribute.Name.Equals("config-ref") && attribute.Value.Equals("Oracle_Configuration_BANNER", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        if (childNode.HasChildNodes && !childNode.InnerText.Equals("#[payload]", StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            XmlNodeList childOfChildNodes = childNode.ChildNodes;
                                            foreach (XmlNode childofChildNode in childOfChildNodes)
                                            {
                                                if (childofChildNode.Name.Equals("db:parameterized-query"))
                                                {
                                                    SqlData sqlData = new SqlData();
                                                    sqlData.isStoredProcedure = true;
                                                    sqlData.storedProcedure = childNode.InnerText;
                                                    sqlData.databaseType = attribute.Value;
                                                    sqlData.operation = childNode.Name;
                                                    SqlData.Add(sqlData);
                                                }
                                            }
                                        }
                                    }

                                    //CONDITION FOR CHILD NODES
                                    if (childNode.HasChildNodes)
                                    {
                                        childTag.hasGrandchildTag = true;

                                        childTag.grandchildTagList = new List<GrandchildTag>();

                                        XmlNodeList grandchildNodes = childNode.ChildNodes;

                                        foreach (XmlNode grandchildNode in grandchildNodes)
                                        {
                                            GrandchildTag grandchildTag = new GrandchildTag();
                                            XmlAttributeCollection grandchildAttributes = grandchildNode.Attributes;
                                            if (grandchildAttributes != null)
                                            {
                                                Console.WriteLine("\n");

                                                if (grandchildAttributes.Count == 0)
                                                {
                                                    break;
                                                }

                                                grandchildTag.grandchildTagname = grandchildNode.Name;

                                                // Console.WriteLine("Child node name: " + childNode.Name);
                                                //Console.WriteLine("+++++++++++++++");

                                                grandchildTag.grandchildTagCount = grandchildAttributes.Count;
                                                //else
                                                //{
                                                grandchildTag.grandchildAttributeKeyValue = new Dictionary<string, string>();
                                                foreach (XmlAttribute gcAttribute in grandchildAttributes)
                                                {
                                                    Console.WriteLine(gcAttribute.Name + " -> " + gcAttribute.Value);
                                                    grandchildTag.grandchildAttributeKeyValue.Add(gcAttribute.Name, gcAttribute.Value);

                                                }
                                            }
                                            childTag.grandchildTagList.Add(grandchildTag);
                                        }
                                    }

                                }
                                document.SqlData = SqlData;
                            }
                            tag.childTagList.Add(childTag);
                        }
                    }

                    Console.WriteLine("\n");
                    if (tag.attributeKeyValue.Count > 0)
                    {
                        document.Tags.Add(tag);
                    }
                }

            }
            return document;

        }
    }
}

