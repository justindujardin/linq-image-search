//-----------------------------------------------------------------------------
// Silverlight 3 Google Image Search Example
//-----------------------------------------------------------------------------
// Copyright (c) 2010 DuJardin Consulting, LLC
// For more information see http://www.github.com/justindujardin/SilverShorts/
//
// This file is licensed under the terms of the MIT license, which is included
// in the LICENSE.markdown file at the root directory repository.
//
// It may also be found on the web : http://www.opensource.org/licenses/mit-license.php
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace SilverShorts
{
   /// <summary>
   /// Implement a Google IImageQuery provider using AJAX api and JSON.Net
   /// See: http://code.google.com/apis/ajaxsearch/documentation/reference.html#_intro_fonje
   /// </summary>
   public class GoogleImageQuery : IImageQuery
   {
      /// <summary>
      /// Google Image Query does not require (but strongly recommends) a valid ApiKey
      /// </summary>
      public bool RequiresApiKey { get { return false; } }

      public string FormatQueryUrl(string search, string appId, int numImages, int offset, bool useSafeSearch)
      {
         return "http://ajax.googleapis.com/ajax/services/search/images?v=1.0"
             + "&q=" + search
             + "&key=" + appId
             + (useSafeSearch ? "&safe=m" : "")
             + "&start=" + Convert.ToString(offset);
      }

      public void ProcessResultString(string response, List<ImageResult> results)
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
               Title = (string)ir["title"],
               MediaUrl = (string)ir["unescapedUrl"],
               Url = (string)ir["url"],
               DisplayUrl = (string)ir["visibleUrl"],
               Width = Int32.Parse((string)ir["width"]),
               Height = Int32.Parse((string)ir["height"]),
               Thumb = new ImageResult.Thumbnail()
               {
                  Url = (string)ir["tbUrl"],
                  Width = Int32.Parse((string)ir["tbWidth"]),
                  Height = Int32.Parse((string)ir["tbHeight"]),
               }
            };

         // Execute the LINQ query and stuff the results into our list 
         results.AddRange(imageResults);
      }
   }
}
