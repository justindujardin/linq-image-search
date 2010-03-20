using System.Collections.Generic;

namespace djc.SilverShorts.Images
{
   /// <summary>
   /// Describe an ImageResult from an IImageQuery response
   /// </summary>
   public class ImageResult
   {
      public string Title;
      public string MediaUrl;
      public string Url;
      public string DisplayUrl;
      public int Width;
      public int Height;
      public class Thumbnail
      {
         public string Url;
         public int Width;
         public int Height;
      }
      public Thumbnail Thumb;
   }

   public delegate void SearchResultCallback(List<ImageResult> results);
   public interface IImageQuery
   {
      /// <summary>
      /// Format a Url string for a specific ImageQuery service
      /// </summary>
      /// <param name="search">The search terms to use in the query</param>
      /// <param name="appId">
      /// The specific query API's developer Id. 
      /// Some services require this and should throw an exception if it is empty.  
      /// Others may ignore it.
      /// </param>
      /// <param name="numImages">The number of results desired</param>
      /// <param name="useSafeSearch">Exclude explicit images. Whatever that means anymore.</param>
      /// <returns>Formatted URL to query the image search service</returns>
      string FormatQueryUrl(string search, string appId = "", int numImages = 10, bool useSafeSearch = true);

      /// <summary>
      /// Process image query results from formatted URL string query
      /// </summary>
      /// <param name="response">string response data</param>
      /// <param name="results">a compatible list container for the ImageResult's</param>
      void ProcessResultString(string response, IList<ImageResult> results);

   }
}
