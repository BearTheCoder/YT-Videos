using System;
using System.IO; //Used to convert PNG to Binary
using System.Threading.Tasks; //Used to Run Twitter Commands Async
using Tweetinvi; //Used to Auth and Tweet
using Tweetinvi.Models;
using Tweetinvi.Parameters; //Used to define pararmeter for image tweet.
namespace PhotoEveryDayApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string APIKey = "";
            string APISecret = "";
            string AccessToken = "";
            string AccessSecret = "";

            byte[] JPG2Binary = File.ReadAllBytes("image.jpg");

            string[] Quotes =
            {
                "Quote",
                "Quote",
            };

            TwitterClient UserClient = new TwitterClient(APIKey, APISecret, AccessToken, AccessSecret);

            IMedia Image_IMedia = await UserClient.Upload.UploadTweetImageAsync(JPG2Binary);

            Random RandNum = new Random();

            ITweet tweetWithImage = await UserClient.Tweets.PublishTweetAsync(new PublishTweetParameters(Quotes[RandNum.Next(10)] + " #Tag #Tag #Tag")
            {
                Medias = { Image_IMedia }
            });
        }
    }
}
