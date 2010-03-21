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
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SilverShorts
{
   /// <summary>
   /// Implement a Microsoft Bing IImageQuery provider
   /// </summary>
   public class BingImageQuery : IImageQuery
   {
      /// <summary>
      /// Bing Image Query requires a valid ApiKey
      /// See: http://www.bing.com/developer/
      /// </summary>
      public bool RequiresApiKey { get { return true; } }

      public string FormatQueryUrl(string search, string appId, int numImages, int offset, bool useSafeSearch)
      {
         return "http://api.bing.net/xml.aspx?"
             + "AppId=" + appId
             + "&Query=" + search
             + "&Sources=Image"
             + "&Version=2.0"
             + "&Market=en-us"
             + (useSafeSearch ? "&Adult=Moderate" : "")
             + "&Image.Count=" + numImages.ToString()
             + "&Image.Offset=" + offset.ToString();
      }

      public void ProcessResultString(string response, List<ImageResult> results)
      {
         // Parse the XML response text 
         XDocument doc = XDocument.Parse(response);

         // Elements in the response all conform to this schema and have a namespace prefix of mms:
         // For our LINQ query to work properly, we must use mmsNs + ElementName
         XNamespace mmsNs = XNamespace.Get("http://schemas.microsoft.com/LiveSearch/2008/04/XML/multimedia");

         // Build a LINQ query to parse the XML data into our custom ImageResult objects
         var imageResults =
            from ir in doc.Descendants()
            where ir.Name.Equals(mmsNs + "ImageResult")
            select new ImageResult()
            {
               Title = ir.Element(mmsNs + "Title").Value,
               MediaUrl = ir.Element(mmsNs + "MediaUrl").Value,
               Url = ir.Element(mmsNs + "Url").Value,
               DisplayUrl = ir.Element(mmsNs + "DisplayUrl").Value,
               Width = Int32.Parse(ir.Element(mmsNs + "Width").Value),
               Height = Int32.Parse(ir.Element(mmsNs + "Height").Value),

               Thumb =
                  (from th in ir.Descendants()
                   where th.Name.Equals(mmsNs + "Thumbnail")
                   select new ImageResult.Thumbnail()
                   {
                      Url = th.Element(mmsNs + "Url").Value,
                      Width = Int32.Parse(th.Element(mmsNs + "Width").Value),
                      Height = Int32.Parse(th.Element(mmsNs + "Height").Value),
                   }).Single(),
            };


         // Execute the LINQ query and stuff the results into our list 
         results.AddRange(imageResults);
      }
   }
}