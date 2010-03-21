//-----------------------------------------------------------------------------
// Silverlight 3 Web Image Search Example
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using SilverShorts;

namespace WebImageSearch
{
   public partial class MainPage : UserControl
   {
      /// <summary>
      /// Your Bing Application Id goes here
      /// </summary>
      private const string _bingApiKey = "";
      /// <summary>
      /// Your Yahoo Api Key goes here
      /// </summary>
      private const string _yahooApiKey = "";


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

         // If we have a Bing api key use Bing, if not try Yahoo, then use Google
         if (!string.IsNullOrEmpty(_bingApiKey))
         {
            // Bing
            WebImageQuery query = new WebImageQuery(new BingImageQuery());
            query.QueryCompleted += ImageQueryCompleted;
            query.ApiKey = _bingApiKey;
            query.Search(txtInput.Text);
         }
         else if(!string.IsNullOrEmpty(_yahooApiKey))
         {
            // Yahoo
            WebImageQuery query = new WebImageQuery(new YahooImageQuery());
            query.QueryCompleted += ImageQueryCompleted;
            query.ApiKey = _yahooApiKey;
            query.Search(txtInput.Text);
         }
         else
         {
            // Google
            WebImageQuery query = new WebImageQuery(new GoogleImageQuery());
            query.QueryCompleted += ImageQueryCompleted;
            query.Search(txtInput.Text);
         }
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
      private void ImageQueryCompleted(List<ImageResult> images)
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
