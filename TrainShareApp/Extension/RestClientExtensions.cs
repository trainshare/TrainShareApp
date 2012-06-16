﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace TrainShareApp.Extension
{
    public static class RestClientExtensions
    {
        public static IObservable<IRestResponse> ExecuteObservable(this IRestClient client, IRestRequest request)
        {
            var subject = new AsyncSubject<IRestResponse>();

            client.ExecuteAsync(
                request,
                response =>
                {
                    subject.OnNext(response);
                    subject.OnCompleted();
                });

            return subject;
        }

        public static IObservable<IRestResponse<T>> ExecuteObservable<T>(this IRestClient client, IRestRequest request)
            where T : new()
        {
            var subject = new AsyncSubject<IRestResponse<T>>();

            client.ExecuteAsync<T>(
                request,
                response =>
                {
                    try
                    {
                        if (response.ErrorException == null) subject.OnNext(response);
                        else subject.OnError(response.ErrorException);
                    }
                    catch (Exception e)
                    {
                        subject.OnError(e);
                    }
                    finally
                    {
                        subject.OnCompleted();
                    }
                });

            return subject;
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

        public static IObservable<IDictionary<string, string>> ParseQueryString(this IObservable<string> uri)
        {
            return
                uri
                    .SelectMany(address => address.Split('&'))
                    .Select(vp => Regex.Split(vp, "="))
                    .ToDictionary(
                        singlePair => singlePair[0],
                        singlePair => singlePair.Length == 2 ? singlePair[1] : string.Empty);
        }
    }
}
