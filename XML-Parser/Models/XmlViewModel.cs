namespace XML_Parser.Models
{
    public class XmlViewModel
    {
        public string name { get; set; }
        public Document parsedXml { get; set; }

        public bool isEmptyParam { get; set; }
    }
}