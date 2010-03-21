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
