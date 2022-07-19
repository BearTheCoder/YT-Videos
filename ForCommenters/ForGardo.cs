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
            try
            {
                string APIKey = "Add Api Key";
                string APISecret = "Add Api Secret";
                string AccessToken = "Add Access Token";
                string AccessSecret = "Add Access Secret";
                int CountOfImages = 0; //Change number to count of images in folder.
                string[] Images = new string[CountOfImages];
                int Counter = 0;
                Random RandNum = new Random();

                foreach(string Image in Directory.EnumerateFiles(@"Add Folder Directory"))
                {
                    Console.WriteLine(Image);
                    Images[Counter] = Image;
                    Counter++;
                }

                Console.WriteLine(Images[RandNum.Next(0, Images.Length)]);
                byte[] JPG2Binary = File.ReadAllBytes(Images[RandNum.Next(0, Images.Length)]);

                TwitterClient UserClient = new TwitterClient(APIKey, APISecret, AccessToken, AccessSecret);

                IMedia Image_IMedia = await UserClient.Upload.UploadTweetImageAsync(JPG2Binary);

                ITweet tweetWithImage = await UserClient.Tweets.PublishTweetAsync(new PublishTweetParameters("Testing out a new error with the twitter API, please disregard. :)")
                {
                    Medias = { Image_IMedia }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }
    }
}
