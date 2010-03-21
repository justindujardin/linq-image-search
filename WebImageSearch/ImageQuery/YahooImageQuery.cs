//-----------------------------------------------------------------------------
// Silverlight 3 Bing Image Search Example
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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using SilverShorts;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.Collections.Generic;

namespace SilverShorts
{
   /// <summary>
   /// Implement a Yahoo Image Search IImageQuery provider
   /// </summary>
   public class YahooImageQuery : IImageQuery
   {
      /// <summary>
      /// Yahoo Image Query requires a valid ApiKey
      /// See: https://developer.apps.yahoo.com/projects
      /// </summary>
      public bool RequiresApiKey { get { return true; } }

      public string FormatQueryUrl(string search, string apiKey, int numImages, int offset, bool useSafeSearch)
      {
         return "http://search.yahooapis.com/ImageSearchService/V1/imageSearch?"
          + "appid=" + apiKey
          + "&query=" + search
          + "&results=" + numImages
          + "&start=" + offset
          + (useSafeSearch ? "" : "&adult_ok=1");
      }

      public void ProcessResultString(string response, List<ImageResult> results)
      {
         // Parse the XML response text 
         XDocument doc = XDocument.Parse(response);
         XNamespace yahNs = "urn:yahoo:srchmi";
         
         // Build a LINQ query to parse the XML data into our custom ImageResult objects
         var imageResults =
            from ir in doc.Descendants()
            where ir.Name.Equals(yahNs + "Result")
            select new ImageResult()
            {
               Title = ir.Element(yahNs + "Title").Value,
               MediaUrl = ir.Element(yahNs + "ClickUrl").Value,
               Url = ir.Element(yahNs + "Url").Value,
               DisplayUrl = ir.Element(yahNs + "RefererUrl").Value,
               Width = Int32.Parse(ir.Element(yahNs + "Width").Value),
               Height = Int32.Parse(ir.Element(yahNs + "Height").Value),

               Thumb =
                  (from th in ir.Descendants()
                   where th.Name.Equals(yahNs + "Thumbnail")
                   select new ImageResult.Thumbnail()
                   {
                      Url = th.Element(yahNs + "Url").Value,
                      Width = Int32.Parse(th.Element(yahNs + "Width").Value),
                      Height = Int32.Parse(th.Element(yahNs + "Height").Value),
                   }).Single(),
            };


         // Execute the LINQ query and stuff the results into our list 
         results.AddRange(imageResults);
      }
   }
}
