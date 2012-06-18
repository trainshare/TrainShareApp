using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace TrainShareApp.Extension
{
    public static class RestClientExtensions
    {
        public static Task<IRestResponse> ExecutTaskAsync(this IRestClient client, IRestRequest request)
        {
            var subject = new TaskCompletionSource<IRestResponse>();

            client.ExecuteAsync(
                request,
                response =>
                {
                    try
                    {
                        if (response.ErrorException == null) subject.SetResult(response);
                        else subject.SetException(response.ErrorException);
                    }
                    catch (Exception e)
                    {
                        subject.SetException(e);
                    }
                });

            return subject.Task;
        }

        public static Task<IRestResponse<T>> ExecutTaskAsync<T>(this IRestClient client, IRestRequest request)
            where T : new() 
        {
            var subject = new TaskCompletionSource<IRestResponse<T>>();

            client.ExecuteAsync<T>(
                request,
                response =>
                {
                    try
                    {
                        if (response.ErrorException == null) subject.SetResult(response);
                        else subject.SetException(response.ErrorException);
                    }
                    catch (Exception e)
                    {
                        subject.SetException(e);
                    }
                });

            return subject.Task;
        }

        public static IRestRequest WithFormat(this IRestRequest request, DataFormat format)
        {
            request.RequestFormat = format;
            return request;
        }

        public static IRestRequest WithCredentials(this IRestRequest request, ICredentials credentials)
        {
            request.Credentials = credentials;
            return request;
        }

        public static IRestRequest WithRootElement(this IRestRequest request, string root)
        {
            request.RootElement = root;
            return request;
        }
        public static IRestRequest AddJson(this IRestRequest request, JToken json)
        {
            request.RequestFormat = DataFormat.Json;
            request
                .AddParameter(
                    new Parameter
                    {
                        Name = "application/json; charset=utf-8",
                        Type = ParameterType.RequestBody,
                        Value = json.ToString()
                    });

            return request;
        }

        public static Task<IDictionary<string, string>> ParseQueryString(this Task<string> task)
        {
            return
                task
                    .ContinueWith(
                        t =>
                        t
                            .Result
                            .Split('&')
                            .Select(vp => Regex.Split(vp, "="))
                            .ToDictionary(
                                singlePair => singlePair[0],
                                singlePair => singlePair.Length == 2 ? singlePair[1] : string.Empty)
                        as IDictionary<string, string>);
        }
    }
}
