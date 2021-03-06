﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CreateAR.Commons.Unity.Http
{
    /// <summary>
    /// A utility object for building URLs.
    /// </summary>
    public class UrlFormatter
    {
        /// <summary>
        /// Gets and sets the Base URL.
        /// </summary>
        public string BaseUrl = "localhost";

        /// <summary>
        /// The protocol.
        /// </summary>
        public string Protocol = "http";

        /// <summary>
        /// Port number.
        /// </summary>
        public int Port = 80;

        /// <summary>
        /// Api version. Eg - 'v1'.
        /// </summary>
        public string Version = "";

        /// <summary>
        /// Everything after the port number, if anything.
        /// </summary>
        public string PostHostname = "";

        /// <summary>
        /// A set of replacements. For each (A, B), we replace "{A}" with "B".
        /// 
        /// Eg. -
        /// 
        /// baseUrl.Url("/users/{userId}");
        /// </summary>
        public Dictionary<string, string> Replacements { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public UrlFormatter()
        {
            Replacements = new Dictionary<string, string>();
        }

        /// <summary>
        /// Replacements.
        /// </summary>
        /// <param name="endpoint">The endpoint to make into a URL.</param>
        /// <param name="replacements">Replacements to override.</param>
        /// <returns></returns>
        public virtual string Url(string endpoint, Dictionary<string, string> replacements)
        {
            return Url(endpoint, Version, Port, Protocol, replacements);
        }

        /// <summary>
        /// Creates a url from the endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint to make into a URL.</param>
        /// <returns>The formatted URL.</returns>
        public virtual string Url(string endpoint)
        {
            return Url(endpoint, Version, Port, Protocol);
        }

        /// <summary>
        /// Creates a url from the endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint to make into a URL.</param>
        /// <param name="version">A version taht overrides the default.</param>
        /// <returns>The formatted URL.</returns>
        public virtual string Url(string endpoint, string version)
        {
            return Url(endpoint, version, Port, Protocol);
        }

        /// <summary>
        /// Creates a url from the endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint to make into a URL.</param>
        /// <param name="version">A version taht overrides the default.</param>
        /// <param name="port">A port that overrides the default..</param>
        /// <returns>The formatted URL.</returns>
        public virtual string Url(string endpoint, string version, int port)
        {
            return Url(endpoint, version, port, Protocol);
        }
        
        /// <summary>
        /// Creates a url from the endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint to make into a URL.</param>
        /// <param name="version">A version that overrides the default.</param>
        /// <param name="port">A port that overrides the default.</param>
        /// <param name="protocol">A protocol that overrides the default.</param>
        /// <param name="replacements">String replacements.</param>
        /// <returns>The formatted URL.</returns>
        public virtual string Url(
            string endpoint,
            string version,
            int port,
            string protocol,
            Dictionary<string, string> replacements = null)
        {
            if (string.IsNullOrEmpty(version))
            {
                if (!string.IsNullOrEmpty(PostHostname))
                {
                    return string.Format("{0}{1}:{2}/{3}/{4}",
                        FormatProtocol(protocol),
                        FormatBaseUrl(),
                        FormatPort(port),
                        FormatPostHostname(PostHostname),
                        FormatEndpoint(endpoint, replacements));
                }

                return string.Format("{0}{1}:{2}/{3}",
                    FormatProtocol(protocol),
                    FormatBaseUrl(),
                    FormatPort(port),
                    FormatEndpoint(endpoint, replacements));
            }

            if (!string.IsNullOrEmpty(PostHostname))
            {
                return string.Format("{0}{1}:{2}/{3}/{4}/{5}",
                    FormatProtocol(protocol),
                    FormatBaseUrl(),
                    FormatPort(port),
                    FormatPostHostname(PostHostname),
                    FormatVersion(version),
                    FormatEndpoint(endpoint, replacements));
            }

            return string.Format("{0}{1}:{2}/{3}/{4}",
                FormatProtocol(protocol),
                FormatBaseUrl(),
                FormatPort(port),
                FormatVersion(version),
                FormatEndpoint(endpoint, replacements));
        }

        /// <summary>
        /// Parses url and fills out information.
        /// </summary>
        /// <param name="url">The URL to parse.</param>
        public virtual bool FromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }
            
            var regex = new Regex(@"^(\w+://)?([a-zA-Z0-9_\-\.]+)(:\d+)?(/v[0-9_\.\-]+/?)?(/[a-zA-Z0-9-\._~/]+)?$");
            var match = regex.Match(url);
            if (match.Success)
            {
                var groups = match.Groups;
                Protocol = groups[1].Success ? groups[1].Value : "https";
                BaseUrl = groups[2].Value;
                int.TryParse(
                    groups[3].Success
                        ? groups[3].Value.Substring(1)
                        : "80",
                    out Port);
                Version = groups[4].Success ? groups[4].Value : "";
                PostHostname = groups[5].Success ? groups[5].Value.Trim('/') : "";

                return true;
            }

            return false;
        }
        
        /// <summary>
        /// Correctly formats a protocol.
        /// </summary>
        /// <returns></returns>
        private string FormatProtocol(string protocol)
        {
            protocol = string.IsNullOrEmpty(protocol)
                ? "http://"
                : protocol;

            if (!protocol.EndsWith("://"))
            {
                protocol += "://";
            }

            return protocol;
        }

        /// <summary>
        /// Correctly formats the base url.
        /// </summary>
        /// <returns></returns>
        private string FormatBaseUrl()
        {
            var baseUrl = string.IsNullOrEmpty(BaseUrl)
                ? "localhost"
                : BaseUrl;

            // trim trailing slashes
            baseUrl = baseUrl.Trim('/');

            // trim protocol
            var index = baseUrl.IndexOf("://", StringComparison.Ordinal);
            if (-1 != index)
            {
                baseUrl = baseUrl.Substring(index + 3);
            }

            // trim endpoints off
            index = baseUrl.IndexOf("/", StringComparison.Ordinal);
            while (-1 != index)
            {
                baseUrl = baseUrl.Substring(0, index);

                index = baseUrl.IndexOf("/", StringComparison.Ordinal);
            }

            return baseUrl;
        }

        /// <summary>
        /// Correctly formats the port.
        /// </summary>
        /// <returns></returns>
        private int FormatPort(int port)
        {
            return port > 0
                ? port
                : 80;
        }

        /// <summary>
        /// Correctly formats the version.
        /// </summary>
        /// <returns></returns>
        private string FormatVersion(string version)
        {
            return version.Trim('/');
        }

        /// <summary>
        /// Correctly formats the endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="replacements">String replacements.</param>
        /// <returns></returns>
        private string FormatEndpoint(string endpoint, Dictionary<string, string> replacements)
        {
            if (string.IsNullOrEmpty(endpoint))
            {
                return "";
            }

            endpoint = endpoint.Trim('/');

            if (null != replacements)
            {
                endpoint = Replace(endpoint, replacements);
            }

            // replacements
            endpoint = Replace(endpoint, Replacements);

            return endpoint;
        }

        /// <summary>
        /// Formats portion of URL.
        /// </summary>
        /// <param name="postHostname">The portion after the hostname.</param>
        /// <returns></returns>
        private string FormatPostHostname(string postHostname)
        {
            if (string.IsNullOrEmpty(postHostname))
            {
                return string.Empty;
            }

            return postHostname.Trim('/');
        }

        /// <summary>
        /// Performs string replacement.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="replacements">Replacements.</param>
        /// <returns></returns>
        private string Replace(string endpoint, Dictionary<string, string> replacements)
        {
            foreach (var replacement in replacements)
            {
                endpoint = endpoint.Replace(
                    "{" + replacement.Key + "}",
                    replacement.Value);
            }

            return endpoint;
        }
    }
}