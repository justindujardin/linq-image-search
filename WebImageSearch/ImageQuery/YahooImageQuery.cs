//-----------------------------------------------------------------------------
// Silverlight 3 Yahoo Image Search Example
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
using System.Xml.Linq;

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
