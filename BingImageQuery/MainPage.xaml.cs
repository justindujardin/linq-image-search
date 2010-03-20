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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using djc.SilverShorts.Images;

namespace BingImageQuery
{
   public partial class MainPage : UserControl
   {
      /// <summary>
      /// Your Bing Application Id goes here
      /// </summary>
      private const string _bingAppId = "";

      /// <summary>
      /// Your Google Application Id goes here
      /// </summary>
      private const string _googleAppId = "";

      public MainPage()
      {
         InitializeComponent();
      }

      #region  UI Element Event handlers
      private void btnSearch_Click(object sender, RoutedEventArgs e)
      {
         if (!btnSearch.IsEnabled)
            return;

         // Disable the UI elements that can be interacted with while a query is happening
         btnSearch.IsEnabled = false;
         txtInput.IsEnabled = false;

         // Clear any thumbnails from the canvas grid
         pssCanvas.Children.Clear();

         // If a Bing app id is specified use it, otherwise do a Google query which does
         // not require an app id (though they do recommend the usage of one)
         if(!string.IsNullOrEmpty(_bingAppId))
            djc.SilverShorts.Images.Bing.ImageQuery.Search(_bingAppId, txtInput.Text, new SearchResultCallback(ImageQueryResults), 9, 3);
         else
            djc.SilverShorts.Images.Google.ImageQuery.Search(_googleAppId,txtInput.Text, new SearchResultCallback(ImageQueryResults), 9, 0);
      }
      private void txtInput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
      {
         // Enter triggers search
         if (e.Key == Key.Enter && btnSearch.IsEnabled)
         {
            btnSearch_Click(sender, e);
         }
      }
      #endregion

      #region Image Query Results Callback
      /// <summary>
      /// Handle results of an Image query, placing thumbnails on the Grid Canvas for viewing
      /// </summary>
      /// <param name="images">A List of djc.SilverShorts.Images.ImageResult objects 
      /// that describe the results of a Bing query</param>
      private void ImageQueryResults(List<ImageResult> images)
      {
         int gridCol = 0;
         int gridRow = 0;
         int count = 0;

         // Iterate over the results and create Image thumbnails for each
         foreach (ImageResult result in images)
         {
            Uri uriThumb = new Uri(result.Thumb.Url,UriKind.Absolute);

            // The constructor to BitmapImage is passed the URI of the 
            // thumbnail, and asynchronously downloads the image data
            Image newImage = new Image()
            {
               Source = new BitmapImage(uriThumb),
               Effect = new System.Windows.Media.Effects.DropShadowEffect() { BlurRadius = 3, Opacity = 0.75 },
               Stretch = System.Windows.Media.Stretch.None,
               Opacity = 0.9
            };

            // Add the image to the grid canvas
            pssCanvas.Children.Add(newImage);

            // Calculate which Row/Column it belongs in and set it
            gridCol = count % pssCanvas.ColumnDefinitions.Count;
            newImage.SetValue(Grid.ColumnProperty, gridCol);
            newImage.SetValue(Grid.RowProperty, gridRow);            
            
            // At the end of a column, hit the next row
            if (gridCol == (pssCanvas.ColumnDefinitions.Count - 1))
               gridRow++;
            count++;
         }

         // Restore UI state
         btnSearch.IsEnabled = true;
         txtInput.IsEnabled = true;
      }
      #endregion
   }
}
