﻿using System;
using System.Collections.Generic;
using System.IO;
using CreateAR.Commons.Unity.Async;
using CreateAR.Commons.Unity.DataStructures;

namespace CreateAR.Commons.Unity.Http
{
    /// <summary>
    /// Verbs.
    /// </summary>
    public enum HttpVerb
    {
        Get,
        Post,
        Put,
        Patch,
        Delete
    }

    /// <summary>
    /// Defines an HTTP service.
    /// </summary>
    public interface IHttpService
    {
        /// <summary>
        /// Services managed for used with this http service.
        /// </summary>
        HttpServiceManager Services { get; }

        /// <summary>
        /// Global timeout. If less than or equal to zero, timeout is disabled.
        /// </summary>
        long TimeoutMs { get; set; }

        /// <summary>
        /// Event called when a request is made.
        /// </summary>
        event Action<string, string, Dictionary<string, string>, object> OnRequest;
        
        /// <summary>
        /// Aborts all http requests.
        /// </summary>
        void Abort();

        /// <summary>
        /// Sends a GET request to an endpoint.
        /// </summary>
        /// <typeparam name="T">The type to expect from the endpoint.</typeparam>
        /// <param name="url">The URL to hit, including GET parameters.</param>
        /// <returns>An IAsyncScope to listen to.</returns>
        /// <exception cref="NullReferenceException"></exception>
        IAsyncToken<HttpResponse<T>> Get<T>(string url);
        
        /// <summary>
        /// Sends a POST request to an endpoint.
        /// </summary>
        /// <typeparam name="T">The type to expect from the endpoint.</typeparam>
        /// <param name="url">The URL to hit.</param>
        /// <param name="payload">The resource to send to this endpoint.</param>
        /// <returns>An IAsyncScope to listen to.</returns>
        /// <exception cref="NullReferenceException"></exception>
        IAsyncToken<HttpResponse<T>> Post<T>(
            string url,
            object payload);
        
        /// <summary>
        /// Sends a PUT request to an endpoint.
        /// </summary>
        /// <typeparam name="T">The type to expect from the endpoint.</typeparam>
        /// <param name="url">The URL to hit.</param>
        /// <param name="payload">The resource to send to this endpoint.</param>
        /// <returns>An IAsyncScope to listen to.</returns>
        /// <exception cref="NullReferenceException"></exception>
        IAsyncToken<HttpResponse<T>> Put<T>(
            string url,
            object payload);
        
        /// <summary>
        /// Sends a DELETE request to an endpoint.
        /// </summary>
        /// <typeparam name="T">The type to expect from the endpoint.</typeparam>
        /// <param name="url">The URL to hit.</param>
        /// <returns>An IAsyncScope to listen to.</returns>
        /// <exception cref="NullReferenceException"></exception>
        IAsyncToken<HttpResponse<T>> Delete<T>(
            string url);

        /// <summary>
        /// Sends a file through POST.
        /// </summary>
        /// <typeparam name="T">The type of response we expect.</typeparam>
        /// <param name="url">The url to send the request to.</param>
        /// <param name="fields">Optional fields that will _precede_ the file.</param>
        /// <param name="file">The file, which will be named "file".</param>
        /// <param name="offset">The offset in the byte array.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <returns></returns>
        IAsyncToken<HttpResponse<T>> PostFile<T>(
            string url,
            IEnumerable<Tuple<string, string>> fields,
            ref byte[] file,
            int offset,
            int count);

        /// <summary>
        /// Sends a file through POST.
        /// </summary>
        /// <typeparam name="T">The type of response we expect.</typeparam>
        /// <param name="url">The url to send the request to.</param>
        /// <param name="fields">Optional fields that will _precede_ the file.</param>
        /// <param name="file">The file, which will be named "file".</param>
        /// <returns></returns>
        IAsyncToken<HttpResponse<T>> PostFile<T>(
            string url,
            IEnumerable<Tuple<string, string>> fields,
            Stream file);

        /// <summary>
        /// Sends a file through PUT.
        /// </summary>
        /// <typeparam name="T">The type of response we expect.</typeparam>
        /// <param name="url">The url to send the request to.</param>
        /// <param name="fields">Optional fields that will _precede_ the file.</param>
        /// <param name="file">The file, which will be named "file".</param>
        /// <param name="offset">The offset in the byte array.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <returns></returns>
        IAsyncToken<HttpResponse<T>> PutFile<T>(
            string url,
            IEnumerable<Tuple<string, string>> fields,
            ref byte[] file,
            int offset,
            int count);

        /// <summary>
        /// Sends a file through PUT.
        /// </summary>
        /// <typeparam name="T">The type of response we expect.</typeparam>
        /// <param name="url">The url to send the request to.</param>
        /// <param name="fields">Optional fields that will _precede_ the file.</param>
        /// <param name="file">The file, which will be named "file".</param>
        /// <returns></returns>
        IAsyncToken<HttpResponse<T>> PutFile<T>(
            string url,
            IEnumerable<Tuple<string, string>> fields,
            Stream file);

        /// <summary>
        /// Downloads raw bytes.
        /// </summary>
        /// <param name="url">Url to download from.</param>
        /// <returns></returns>
        IAsyncToken<HttpResponse<byte[]>> Download(string url);
    }
}