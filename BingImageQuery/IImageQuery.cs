using System.Collections.Generic;

namespace SilverShorts
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

   public interface IImageQuery
   {
      /// <summary>
      /// Determines whether this provider requires a valid API key
      /// </summary>
      bool RequiresApiKey { get; }

      /// <summary>
      /// Format a Url string for a specific ImageQuery service
      /// </summary>
      /// <param name="search">The search terms to use in the query</param>
      /// <param name="apiKey">The specific query ApiKey</param>
      /// <param name="numImages">The number of results desired</param>
      /// <param name="offset">The desired result index offset</param>
      /// <param name="useSafeSearch">Exclude explicit images. Whatever that means anymore.</param>
      /// <returns>Formatted URL to query the image search service</returns>
      string FormatQueryUrl(string search, string apiKey, int numImages, int offset, bool useSafeSearch);

      /// <summary>
      /// Process image query results from formatted URL string query
      /// </summary>
      /// <param name="response">string response data</param>
      /// <param name="results">a compatible list container for the ImageResult's</param>
      void ProcessResultString(string response, List<ImageResult> results);

   }
}
