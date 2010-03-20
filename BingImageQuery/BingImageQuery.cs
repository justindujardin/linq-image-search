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
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using djc.SilverShorts.Images;

namespace djc.SilverShorts.Images.Bing
{

   static class ImageQuery
   {
      #region Public Search Interface
      static public void Search(string appId, string search, SearchResultCallback callback, int numImages = 10, int offsetIndex = 0, bool useSafeSearch = true)
      {
         if (string.IsNullOrEmpty(appId))
            throw new ArgumentNullException("appId", "Bing Image search requires an application key.  See: http://www.bing.com/developer/");

         string requestString = "http://api.bing.net/xml.aspx?"
             + "AppId=" + appId
             + "&Query=" + search
             + "&Sources=Image"
             + "&Version=2.0"
             + "&Market=en-us"
             + (useSafeSearch ? "&Adult=Moderate" : "")
             + "&Image.Count=" + numImages.ToString()
             + "&Image.Offset=" + offsetIndex.ToString();

         // Use the WebClient class to perform an asynchronous URL based query against Bing
         Uri uri = new Uri(requestString, UriKind.Absolute);
         WebClient client = new WebClient();
         client.DownloadStringCompleted += _downloadStringCompleted;
         // Note that we're passing the callback delegate as user data
         client.DownloadStringAsync(uri, callback);
      }
      #endregion

      #region Event handlers
      /// <summary>
      /// WebClient DownloadStringAsync Event handler
      /// </summary>
      /// <param name="e">e.UserState as SearchResultCallback</param>
      static private void _downloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
      {
         SearchResultCallback callback = e.UserState as SearchResultCallback;
         if (e.Error != null || callback == null)
            return;
         
         _processXmlResults(e.Result.ToString(), callback);
      }
      #endregion

      #region Xml to Object LINQ
      /// <summary>
      /// Process an Xml string into custom class data
      /// </summary>
      /// <param name="xml">string Xml data</param>
      /// <param name="callback">The user callback to invoke on completion</param>
      static private void _processXmlResults(string xml, SearchResultCallback callback)
      {
         if (callback == null)
            return;

         List<ImageResult> results = null;
         try
         {
            // Parse the XML response text 
            XDocument doc = XDocument.Parse(xml);

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
            results = imageResults.ToList();
         }
         catch (System.Exception ex)
         {
            throw new XmlException("Failed LINQ query against BING XML result", ex);
         }

         if (results == null)
            results = new List<ImageResult>();
         // invoke search callback
         callback(results);
      }
      #endregion
   }
}