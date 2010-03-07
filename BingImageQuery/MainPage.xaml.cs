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
using djc.SilverShorts.Bing;

namespace BingImageQuery
{
   public partial class MainPage : UserControl
   {
      /// <summary>
      /// Your Bing Application Id goes here
      /// </summary>
      private const string _appId = "KJH45KJHF89U0W98FO8318HJF8YAQ089F3O2HJRF";

      public MainPage()
      {
         InitializeComponent();
      }

      #region Bing Image Search
      private void btnGO_Click(object sender, RoutedEventArgs e)
      {
         if (!btnGO.IsEnabled)
            return;

         btnGO.IsEnabled = false;
         textInput.IsEnabled = false;
         pssCanvas.Children.Clear();

         ImageQuery imageSearch = new ImageQuery();
         imageSearch.Search(_appId, textInput.Text, new ImageQuery.SearchResultCallback(BingSearchResults));
      }
      private void textInput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
      {
         // Enter triggers search
         if (e.Key == Key.Enter && btnGO.IsEnabled)
         {
            btnGO_Click(sender, e);
         }
      }


      private void BingSearchResults(List<ImageResult> images)
      {
         int gridCol = 0;
         int gridRow = 0;
         int count = 0; // When !% 3 increment the row

         foreach (ImageResult result in images)
         {
            Uri uriThumb = new Uri(result.Thumbnail.Url,UriKind.Absolute);

            Image newImage = new Image()
            {
               Source = new BitmapImage(uriThumb),
               Effect = new System.Windows.Media.Effects.DropShadowEffect() { BlurRadius = 3, Opacity = 0.75 },
               Stretch = System.Windows.Media.Stretch.None,
               Opacity = 0.9
            };

            // Download the image
            pssCanvas.Children.Add(newImage);

            gridCol = count % pssCanvas.ColumnDefinitions.Count;
            newImage.SetValue(Grid.ColumnProperty, gridCol);
            newImage.SetValue(Grid.RowProperty, gridRow);            
            
            if (gridCol == 2)
               gridRow++;
            count++;
         }

         // Restore UI state
         btnGO.IsEnabled = true;
         textInput.IsEnabled = true;
      }
      #endregion
   }
}
