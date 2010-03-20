﻿using System;
using System.Collections.Generic;
using System.Net;

namespace SilverShorts
{
   public class WebImageQuery
   {
      /// <summary>
      /// Construct a WebImageQuery using the specified IImageQuery provider 
      /// </summary>
      public WebImageQuery(IImageQuery provider) { _queryProvider = provider; }
      private WebImageQuery() { }

      private IImageQuery _queryProvider;

      /// <summary>
      /// Specifies the API specific key (if any) for the search provider
      /// </summary>
      private string _apiKey;
      public string ApiKey
      {
         get { return _apiKey; }
         set { _apiKey = value; }
      }

      /// <summary>
      /// Delegate for Completed query event handlers
      /// </summary>
      /// <param name="results"></param>
      public delegate void QueryCompletedEventHandler(List<ImageResult> results);
      
      /// <summary>
      /// Fired when the asynchronous web image query is complete
      /// </summary>
      public event QueryCompletedEventHandler QueryCompleted;

      /// <summary>
      /// 
      /// </summary>
      /// <param name="search"></param>
      /// <param name="callback"></param>
      /// <param name="numImages"></param>
      /// <param name="offset"></param>
      /// <param name="useSafeSearch"></param>
      public void Search(string search, int numImages = 10, int offset = 0, bool useSafeSearch = true)
      {
         if (_queryProvider == null)
            throw new ArgumentNullException("_queryProvider");

         if (_queryProvider.RequiresApiKey && string.IsNullOrEmpty(_apiKey))
            throw new ArgumentNullException("ApiKey", "This search provider requires a valid key");

         string requestString = _queryProvider.FormatQueryUrl(search, _apiKey,numImages, offset, useSafeSearch);

         WebClient client = new WebClient();
         client.DownloadStringCompleted += _downloadStringCompleted;
         client.DownloadStringAsync(new Uri(requestString, UriKind.Absolute));
      }

      /// <summary>
      /// WebClient DownloadStringAsync Event handler
      /// </summary>
      /// <param name="e">e.UserState as SearchResultCallback</param>
      private void _downloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
      {
         if (_queryProvider == null)
            throw new ArgumentNullException("_queryProvider");

         if (e.Error != null)
            return;

         List<ImageResult> results = new List<ImageResult>();
         try
         {
            _queryProvider.ProcessResultString(e.Result.ToString(), results);
         }
         catch (System.Exception ex)
         {
            throw new Exception("Failed Process Image Query Result", ex);
         }

         // QueryCompleted event
         if (QueryCompleted != null)
            QueryCompleted(results);
      }
   }
}