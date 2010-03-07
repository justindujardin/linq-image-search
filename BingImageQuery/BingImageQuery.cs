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
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;

namespace djc
{
   namespace SilverShorts
   {
      namespace Bing
      {
         public class ImageResult
         {
            public string Title;
            public string MediaUrl;
            public string Url;
            public string DisplayUrl;
            public int Width;
            public int Height;
            public int FileSize;
            public class Thumb
            {
               public string Url;
               public string ContentType;
               public int Width;
               public int Height;
               public int FileSize;
            }
            public Thumb Thumbnail;
         }

         public class ImageQuery
         {
            #region Public Interface
            public delegate void SearchResultCallback(List<ImageResult> results);
            public void Search(string appId, string search, SearchResultCallback callback, int numImages = 10, int offsetIndex = 0, bool useSafeSearch = true)
            {
               WebClient client = new WebClient();
               client.DownloadStringCompleted += _downloadStringCompleted;

               string requestString = "http://api.bing.net/xml.aspx?"

                   // Common request fields (required)
                   + "AppId=" + appId
                   + "&Query=" + search
                   + "&Sources=Image"

                   // Common request fields (optional)
                   + "&Version=2.0"
                   + "&Market=en-us"
                   + (useSafeSearch ? "&Adult=Moderate" : "")

                   // Image-specific request fields (optional)
                   + "&Image.Count=" + Convert.ToString(numImages)
                   + "&Image.Offset=" + Convert.ToString(offsetIndex);

               Uri uri = new Uri(requestString, UriKind.Absolute);
               client.DownloadStringAsync(new Uri(requestString), callback);
            }
            #endregion

            #region XML to Object LINQ
            private void _downloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
            {
               SearchResultCallback callback = e.UserState as SearchResultCallback;
               if (e.Error != null || callback == null)
                  return;

               List<ImageResult> results = new List<ImageResult>();
               string xmlText = e.Result.ToString();
               try
               {
                  XDocument doc = XDocument.Parse(xmlText);
                  XNamespace mmsNs = XNamespace.Get("http://schemas.microsoft.com/LiveSearch/2008/04/XML/multimedia");
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
                        FileSize = Int32.Parse(ir.Element(mmsNs + "FileSize").Value),

                        Thumbnail =
                           (from th in ir.Descendants()
                            where th.Name.Equals(mmsNs + "Thumbnail")
                            select new ImageResult.Thumb()
                            {
                               Url = th.Element(mmsNs + "Url").Value,
                               ContentType = th.Element(mmsNs + "ContentType").Value,
                               Width = Int32.Parse(th.Element(mmsNs + "Width").Value),
                               Height = Int32.Parse(th.Element(mmsNs + "Height").Value),
                               FileSize = Int32.Parse(th.Element(mmsNs + "FileSize").Value)
                            }).Single(),
                     };

                  foreach (ImageResult imgResult in imageResults)
                     results.Add(imgResult);
               }
               catch (System.Exception ex)
               {
                  throw new XmlException("Failed LINQ query against BING XML result", ex);
               }

               // invoke search callback
               callback(results);
            }
            #endregion
         }
      }
   }
}
