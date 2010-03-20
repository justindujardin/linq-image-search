//-----------------------------------------------------------------------------
// Silverlight 3 Google Image Search Example
//-----------------------------------------------------------------------------
// Copyright (c) 2010 Justin DuJardin
//
// The MIT License
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using Newtonsoft.Json.Linq;


namespace djc.SilverShorts.Images.Google
{
   static class ImageQuery
   {
      #region Public Search Interface
      static public void Search(string appId, string search, SearchResultCallback callback, int numImages = 10, int offsetIndex = 0, bool useSafeSearch = true)
      {
         // Use the WebClient class to perform an asynchronous URL based query against Bing
         WebClient client = new WebClient();
         client.DownloadStringCompleted += _downloadStringCompleted;

         string requestString = "http://ajax.googleapis.com/ajax/services/search/images?v=1.0"
             // Query 
             + "&q=" + search
             // Application Key
             + (String.IsNullOrEmpty(appId) ? "" : "&key=" + appId)
             // Safe search
             + (useSafeSearch ? "&safe=m" : "")
             // Offset results
             + "&start=" + Convert.ToString(offsetIndex);

         // Create a URI from the request string and fire off the query
         // Note that we're passing the callback delegate as user data
         Uri uri = new Uri(requestString, UriKind.Absolute);
         client.DownloadStringAsync(uri, callback);
      }
      #endregion

      #region JSON to Object LINQ
      /// <summary>
      /// Process a google image search JSON response
      /// </summary>
      /// <param name="response">string JSON data</param>
      /// <param name="callback">The user callback to invoke on completion</param>
      static private void _processJsonResults(string response, SearchResultCallback callback)
      {
         if (callback == null) 
            throw new ArgumentNullException("callback");

         List<ImageResult> results = null;
         try
         {
            // Parse the JSON string
            JObject json = JObject.Parse(response);

            // Check response validity, throw exception with details if error
            if ((int)json["responseStatus"] != 200)
               throw new Exception((string)json["responseDetails"]);

            // Parse out results using LINQ
            var imageResults =
               from ir in json["responseData"]["results"].Children()
               where ((string)ir["GsearchResultClass"]).Equals("GimageSearch")
               select new ImageResult()
               {
                  Title       = (string)ir["title"],
                  MediaUrl    = (string)ir["unescapedUrl"],
                  Url         = (string)ir["url"],
                  DisplayUrl  = (string)ir["visibleUrl"],
                  Width       = Int32.Parse((string)ir["width"]),
                  Height      = Int32.Parse((string)ir["height"]),
                  Thumb = new ImageResult.Thumbnail() 
                  {
                     Url      = (string)ir["tbUrl"],
                     Width    = Int32.Parse((string)ir["tbWidth"]),
                     Height   = Int32.Parse((string)ir["tbHeight"]),
                  }
               };

            results = imageResults.ToList();
         }
         catch (System.Exception ex)
         {
            throw new Exception("Failed LINQ query against Google JSON result", ex);
         }

         if (results == null)
            results = new List<ImageResult>();
         // invoke search callback
         callback(results);
      }
      /// <summary>
      /// Event handler for Google Image query result JSON string
      /// </summary>
      /// <param name="e">e.UserState as SearchResultCallback</param>
      static private void _downloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
      {
         SearchResultCallback callback = e.UserState as SearchResultCallback;
         if (e.Error != null || callback == null)
            return;

         _processJsonResults(e.Result.ToString(), callback);
      }
      #endregion
   }
}
