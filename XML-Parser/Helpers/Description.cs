using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace XML_Parser.Helpers
{
    public enum Description
    {
        [Description("Configuration file")]
        GlobalFile,
        [Description("Parent resource file - contains Database, Webservice and other information.")]
        ApiFile,
        [Description("Other configuration file")]
        OtherFiles
    }
}