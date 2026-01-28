using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;

namespace SMSWEBAPP
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            ScriptManager.ScriptResourceMapping.AddDefinition("jquery",
                new ScriptResourceDefinition
                {
                    Path = "~/Scripts/jquery-3.6.0.min.js", // Update the path to your jQuery file
                    DebugPath = "~/Scripts/jquery-3.6.0.js", // Update the path to your jQuery debug file
                    CdnPath = "https://ajax.aspnetcdn.com/ajax/jQuery/jquery-3.6.0.min.js", // Update the CDN path
                    CdnDebugPath = "https://ajax.aspnetcdn.com/ajax/jQuery/jquery-3.6.0.js" // Update the CDN debug path
                });
        }
    }
}

