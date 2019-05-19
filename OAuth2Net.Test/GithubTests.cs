using NUnit.Framework;
using OAuth2Net;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace Tests
{
    public class GithubTests
    {
        HttpListener listener;
        OAuth2GitHub api;
        string result;


        [SetUp]
        public void Setup()
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:6625/auth/github/");

            api = new OAuth2GitHub(
                "4722fcf139915e3ff0f1",
                "afea9fcac75c2bae6ea2603e7871b5cacadabe44",
                "http://localhost:6625/auth/github/default.aspx",
                success: api => result = "success",
                failure: api => result = "failure");
        }


        [Test]
        public void Test1()
        {
            var authUrl = api.GetAuthorizationUrl();

            var cli = new WebClient();

            cli.Encoding = Encoding.UTF8;

            listener.Start();

            var context = listener.GetContext();


            OAuth2App.Callback(
                context.Request.QueryString["code"],
                context.Request.QueryString["error"],
                context.Request.QueryString["state"],
                context.Request.QueryString["error_description"],
                context.Request.QueryString["error_uri"]);
        }
    }
}