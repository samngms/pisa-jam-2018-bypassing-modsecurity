using pisa.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace pisa
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var username = ConfigurationManager.AppSettings["myname"];
            var password = ConfigurationManager.AppSettings["mypassword"];
            if ( username == tbUserName.Text && password == tbPassword.Text )
            {
                Session["user"] = username;
                txResult.Text = "Login Successful";
            }
            else
            {
                Session.Remove("user");
                txResult.Text = "Login Failed";
            }
        }
    }
}