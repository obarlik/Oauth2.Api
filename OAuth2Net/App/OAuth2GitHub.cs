﻿using System;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace OAuth2Net
{
    public class OAuth2GitHub : OAuth2App
    {
        public string PersonId { get; protected set; }
        public string PersonName { get; protected set; }
        public string PersonEmail { get; protected set; }
        public string PersonPhotoUrl { get; protected set; }
        public string PersonProfileUrl { get; protected set; }
        public string PersonLocation { get; protected set; }
        public string PersonInfo { get; protected set; }

        public OAuth2GitHub(
            string client_id,
            string client_secret,
            string redirect_uri = null, 
            Action<OAuth2GitHub> success = null, 
            Action<OAuth2GitHub> failure = null) 
            : base(
                "https://github.com/login/oauth/authorize",
                "https://github.com/login/oauth/access_token", 
                client_id, 
                redirect_uri, 
                null,
                success_api =>
                {
                    var gitHubApi = success_api as OAuth2GitHub;

                    var cli = gitHubApi.NewClient();
                    var json = JObject.Parse(cli.DownloadString("https://api.github.com/user"));

                    gitHubApi.PersonId = json["id"].Value<string>();
                    gitHubApi.PersonName = json["name"].Value<string>() ?? json["login"].Value<string>();
                    gitHubApi.PersonEmail = json["email"].Value<string>();
                    gitHubApi.PersonPhotoUrl = json["avatar_url"].Value<string>();
                    gitHubApi.PersonProfileUrl = json["html_url"].Value<string>();
                    gitHubApi.PersonLocation = json["location"].Value<string>();
                    gitHubApi.PersonInfo = json["bio"].Value<string>();

                    success((OAuth2GitHub)success_api);
                },
                failure_api => failure((OAuth2GitHub)failure_api))
        {
            AuthorizationParams["client_secret"] = client_secret;
        }


        public WebClient NewClient(string agent = null)
        {
            var cli = new WebClient();

            cli.Headers["Accept"] = "application/vnd.github.machine-man-preview+json";
            cli.Headers["User-Agent"] = agent ?? "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.140 Safari/537.36 Edge/17.17134";
            cli.Headers["Authorization"] = $"{AccessTokenType} {AccessToken}";
            cli.Encoding = Encoding.UTF8;

            return cli;
        }

    }
}
