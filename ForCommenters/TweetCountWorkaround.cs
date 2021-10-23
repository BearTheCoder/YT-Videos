/*
 Instructions:
    1. Download the Tweetinvi NuGet from >Project >Manage Nuget Packages.
    2. Input your API key, API Secret key, and Bearer Token.
    3. Set your query term.
    4. Set the number of days back you want to go as a negative number. (You could change "AddDays" to something else if required)
    5. Run it. This will take a long time if your query is broad.

    This is an extremely crude way of getting around Tweetinvi's flaws, but hey, it works.
*/
using System;
using Tweetinvi;
using Tweetinvi.Iterators;
using Tweetinvi.Models.V2;
using System.Threading.Tasks;
using Tweetinvi.Core.Iterators;
using Tweetinvi.Parameters.V2;
namespace TwitterTestForCommenter {
    class Program {
        private static readonly string APIKey = ""; // Step 2
        private static readonly string APISecret = ""; // Step 2
        private static readonly string BearerToken = ""; // Step 2
        private static TwitterClient UserClient;
        static void Main(string[] args) {
            UserClient = new TwitterClient(APIKey, APISecret, BearerToken);
            SearchTweetsV2Parameters Test = new SearchTweetsV2Parameters("#Sample"); // Step 3
            Test.StartTime = DateTime.Now.AddDays(-1.0); // Step 4
            Test.EndTime = DateTime.Now;
            ITwitterRequestIterator<SearchTweetsV2Response, string> ListOfTweets = UserClient.SearchV2.GetSearchTweetsV2Iterator(Test);
            SearchTweets(ListOfTweets);
            Console.ReadLine();
        }
        private static async Task<IteratorPageResult<SearchTweetsV2Response, string>> SearchTweets(ITwitterRequestIterator<SearchTweetsV2Response, string> LocalListOfTweets) {
            int Count = 0;
            while (!LocalListOfTweets.Completed) {
                var searchPage = await LocalListOfTweets.NextPageAsync();
                SearchTweetsV2Response searchResponse = searchPage.Content;
                TweetV2[] tweets = searchResponse.Tweets;
                Count += tweets.Length;
            }
            Console.WriteLine(Count);
            return null;
        }
    }
}
