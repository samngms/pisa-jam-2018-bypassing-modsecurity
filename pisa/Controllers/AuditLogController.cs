using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Net.Http.Headers;
using System.Web;

namespace pisa.Controllers
{
    public class AuditLogController : ApiController
    {
        public HttpResponseMessage GetLog()
        {
            var session = HttpContext.Current.Session;
            if ( null == session || null == session["user"] ) throw new HttpResponseException(HttpStatusCode.Unauthorized);

            var resp = new HttpResponseMessage();
            var rx = new Regex("^--[0-9a-fA-F]+-([A-Z])--$");
            var logLocation = ConfigurationManager.AppSettings["auditLog"];
            string[] lines = ReadAllLines(logLocation);
            int lastIdx = lines.Length;
            for (int i=lines.Length-1; i>=0; i--)
            {
                var s = lines[i].Trim();
                var matcher = rx.Match(s);
                if ( matcher.Success )
                {
                    if ( "H" == matcher.Groups[1].Value )
                    {
                        resp.Content = new StringContent(toHtml(lines, i + 1, lastIdx));
                        resp.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                        return resp;
                    }
                    else
                    {
                        lastIdx = i;
                    }
                }
            }

            resp.Content = new StringContent(toHtml(new string[0], 0, 0));
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return resp;
        }

        // File.ReadAllLines() can't read a file used by another process, looks like a bug in .NET
        private string[] ReadAllLines(String path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(stream))
            {
                List<string> list = new List<string>();
                while (!reader.EndOfStream)
                {
                    list.Add(reader.ReadLine());
                }

                return list.ToArray();
            }
        }

        private string toHtml(string[] list, int start, int end)
        {
            int size = end - start;
            StringBuilder buf = new StringBuilder();
            buf.Append(@"<!DOCTYPE html>
<html>
<head>
<style>
body {
    font-family: monospace;
}
</style>
</head>
<body>");
            for (int i=0; i<size; i++)
            {
                buf.Append("<p>").Append(HttpUtility.HtmlEncode(list[i + start])).Append("</p>\n");
            }
            buf.Append("</body>\n</html>\n");
            return buf.ToString();
        }
    }
}
