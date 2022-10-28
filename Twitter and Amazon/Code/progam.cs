/*
    Hi, Hello.

    A couple developer notes.
        1.) You might want to serialize "ProductsPurchased" to JSON and recall it if necessary.
            I don't know why I didn't save this info to JSON. Must have slipped my mind.
        2.) I used two JSON files because I don't use JSON that much and it was much easier and time
            effective to use two files instead of having to learn how to use one file.
        3.) I didn't set passwords to an environment variable because I didn't feel like it.
    I am very much a "if it works it works" type of programmer and this program works VERY WELL for what I needed.
    If you need it for you project you might have to heavily alter it because I don't exactly think of other developers
    when I program personal projects.
    
    Love you. Ok, Bye.
*/

using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using AE.Net.Mail;
namespace TwitterTest2
{
    class Program
    {
        // Settings Variables
        static readonly float AllowedTotal = 1000;
        static readonly int MaxPurchaseAmount = 57;
        static readonly int DaysAlloted = 13;
        static readonly int MinutesAlloted = 5;
        static readonly int EmailResponseTimeAlloted = 90;
        static async Task Main(string[] args)
        {
            // Init general variables
            string FilePath = "Users.json";
            List<IMessage> BitlyList = new List<IMessage>();
            int ProductsPurchased = 0;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance); //Needed to get AE.NET.MAIL to work with CORE 3.1
            Stopwatch HalfHourStopwatch = Stopwatch.StartNew();
            Stopwatch TwoWeekStopwach = Stopwatch.StartNew();

            // Start Main Loop
            while (true)
            {
                CheckForTimeout(TwoWeekStopwach, null);
                if (HalfHourStopwatch.Elapsed.TotalMinutes > MinutesAlloted)
                {
                    // Auth on Init Twitter client
                    TwitterClient UserClient = new TwitterClient(SensitiveInfo.API_Key, SensitiveInfo.API_Secret, SensitiveInfo.Access_Token, SensitiveInfo.Access_Secret);
                    var Messages = await UserClient.Messages.GetMessagesAsync();
                    // Preparing to check if user has participated before or not.
                    List<long> UserIDS = new List<long>();
                    if (File.Exists(FilePath))
                    {
                        // Deserialize User JSON file
                        UserIDS = DeserializerUserJSON(FilePath);
                    }
                    // Looping through each Twitter Message
                    foreach (IMessage IM in Messages)
                    {
                        string DeleteMessage = "true";
                        // Check to make sure message is only a link.
                        if (IM.Text.IndexOf("https://") == 0 && IM.Text.IndexOf(" ") == -1)
                        {
                            if (UserIDS.Contains(IM.SenderId))
                            {
                                // User already had successful purchase.
                                string Reply = "User already had a successful purchase twitter reply message...";
                                SendTwitterReply(Reply, UserClient, IM);
                            }
                            else
                            {
                                //User has not had successful purchase
                                IWebDriver WDriver = new ChromeDriver(@""); // INPUT PATH TO CHROMEDRIVE.EXE HERE
                                try
                                {
                                    bool Continue = AddToCartMacros(WDriver, IM, UserClient);
                                    if (Continue)
                                    {
                                        goto End;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    string SSPath = TakeScreenshot(WDriver);
                                    SendErrorEmail(SSPath);
                                    Console.WriteLine("Waiting for user intervention...");
                                    Console.WriteLine("Delete Twitter message?");
                                    DeleteMessage = Console.ReadLine();
                                    WDriver.Quit();
                                    goto End;
                                }

                                //Check for captcha
                                try
                                {
                                    //Works if Captcha is not present
                                    PlaceOrder(WDriver, UserClient, IM, FilePath, UserIDS, ProductsPurchased);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex);
                                    //Take screenshot, email to me, listen for response, input captcha, and continue to place order.
                                    string SSFilePath = TakeScreenshot(WDriver);
                                    string Captcha = SendCaptchaEmail(SSFilePath);
                                    //INPUT CAPTCHA MACROS HERE
                                    WDriver.FindElement(By.Name("password")).SendKeys(SensitiveInfo.PASSWORD);
                                    WDriver.FindElement(By.Name("guess")).SendKeys(Captcha);
                                    WDriver.FindElement(By.Id("signInSubmit")).SendKeys(Keys.Return);

                                    //With Captcha input, continue with order placement.
                                    PlaceOrder(WDriver, UserClient, IM, FilePath, UserIDS, ProductsPurchased);
                                }
                                WDriver.Quit();
                            }
                        }
                        else
                        {
                            // Twitter message is not a link...
                            string Reply = "Not a link twitter message goes here...";
                            SendTwitterReply(Reply, UserClient, IM);
                        }
                    End:
                        if (DeleteMessage == "true")
                        {
                            IM.DestroyAsync().Wait();
                        }
                    }
                    Thread.Sleep(5000); //Pause before next message...
                    Console.WriteLine("Restart...");
                    HalfHourStopwatch.Restart();
                }
            }
        }
        static void CheckForTimeout(Stopwatch LocalSW, IWebDriver LocalWDriver)
        {
            if (LocalSW.Elapsed.Days > DaysAlloted)
            {
                string Reason = "The application has detected that the time passed has exceeded time permitted.";
                SendAppQuitEmail(Reason);
                QuitApplication(LocalWDriver);
            }
        }
        static bool AddToCartMacros(IWebDriver LocalWDriver, IMessage LocalIM, TwitterClient LocalClient)
        {
            //Extract full url instead of compressed version.
            string UserURL = LocalIM.Entities.Urls[0].ExpandedURL;
            //Pre-determined set of macros to add product to cart, can error if a protection plan is offered.
            if (UserURL.StartsWith("https://www.amazon.com/"))
            {
                try
                {
                    //Check for spanish language
                    if (LocalIM.Entities.Urls[0].URL.Contains("/es/"))
                    {
                        int ASINIndex = UserURL.IndexOf("/dp/");
                        string ASIN = UserURL.Substring(ASINIndex + 4, 10);
                        Console.WriteLine($"ASIN NUMBER: {ASIN} ------------------------------------------");
                        LocalWDriver.Url = $"https://www.amazon.com/gp/aws/cart/add.html?AssociateTag=Associate+Tag&ASIN.1={ASIN}&Quantity.1=1&add=add";
                        LocalWDriver.FindElement(By.XPath("/html/body/div[1]/div[2]/div/form/div[5]/span[2]/span/input")).SendKeys(Keys.Return);
                        LocalWDriver.FindElement(By.Name("proceedToRetailCheckout")).SendKeys(Keys.Return);
                    }
                    else
                    {
                        //If it has "Buying Options" it needs to be pulled by URL no ASIN
                        LocalWDriver.Url = LocalIM.Entities.Urls[0].URL;
                        LocalWDriver.FindElement(By.Id("add-to-cart-button")).SendKeys(Keys.Return);
                        LocalWDriver.FindElement(By.Name("proceedToRetailCheckout")).SendKeys(Keys.Return);
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        Console.WriteLine("Checking for buying options...");
                        // Buying Options
                        LocalWDriver.FindElement(By.LinkText("See All Buying Options")).SendKeys(Keys.Return);
                        Thread.Sleep(2000);
                        LocalWDriver.FindElement(By.XPath("/html/body/div[1]/span/span/span/div/div/div[4]/div[1]/div[2]/div/div/div[2]/div/div/div[2]/span/span/span/span/input")).SendKeys(Keys.Return);
                        Thread.Sleep(1000);
                        LocalWDriver.FindElement(By.XPath("/html/body/div[1]/span/div/span/span/i")).Click();
                        Thread.Sleep(1000);
                        LocalWDriver.FindElement(By.Id("nav-cart-count")).Click();
                        Thread.Sleep(1000);
                        LocalWDriver.FindElement(By.Name("proceedToRetailCheckout")).SendKeys(Keys.Return);
                    }
                    catch (Exception ex2)
                    {
                        // Additional products offered
                        Console.WriteLine("Additional items offered...");
                        LocalWDriver.FindElement(By.Id("add-to-cart-button")).SendKeys(Keys.Return);
                        Thread.Sleep(3000);
                        //Proceed to checkout
                        LocalWDriver.FindElement(By.XPath("/html/body/div[5]/div[3]/div/div/div[1]/div[3]/div[1]/div[2]/div[3]/span/span/input")).SendKeys(Keys.Return);
                    }
                }
                LocalWDriver.FindElement(By.Name("email")).SendKeys(SensitiveInfo.EMAIL);
                LocalWDriver.FindElement(By.Name("email")).SendKeys(Keys.Return);
                LocalWDriver.FindElement(By.Name("password")).SendKeys(SensitiveInfo.PASSWORD);
                LocalWDriver.FindElement(By.Name("password")).SendKeys(Keys.Return);
                Thread.Sleep(10000);  // Let captcha or check out form load
                return false;
            }
            else if (LocalIM.Entities.Urls[0].ExpandedURL.Contains(".com"))
            {
                //Possible Malacious link - Block User
                string Reply = "Link other than amazon link twitter reply message...";
                SendTwitterReply(Reply, LocalClient, LocalIM);
                LocalWDriver.Quit();
                // Line below BLOCKS USER. Comment out if you don't want that to happen.
                LocalClient.Users.BlockUserAsync(LocalIM.SenderId);
                return true;
            }
            else
            {
                //No link detected
                string Reply = "No link detected twitter reply message...";
                SendTwitterReply(Reply, LocalClient, LocalIM);
                LocalWDriver.Quit();
                return true;
            }
        }
        static string TakeScreenshot(IWebDriver LocalWDriver)
        {
            LocalWDriver.Manage().Window.FullScreen();
            string SSFilePath = "SS.png";
            Screenshot SS = ((ITakesScreenshot)LocalWDriver).GetScreenshot();
            SS.SaveAsFile(SSFilePath, ScreenshotImageFormat.Png);
            return SSFilePath;
        }
        static void SendErrorEmail(string LocalSSFilePath)
        {
            using (SmtpClient GmailClient = new SmtpClient("smtp.gmail.com", 587))
            {
                System.Net.Mail.MailMessage MsgObj = ComposeEmailGeneralInfo(GmailClient);
                MsgObj.Subject = "Error or Protection plan?";
                MsgObj.Body = "The program just encountered an error, please ensure that it is because of a protection plan or something else.";
                System.Net.Mail.Attachment Image = new System.Net.Mail.Attachment(LocalSSFilePath);
                MsgObj.Attachments.Add(Image);
                GmailClient.Send(MsgObj);
            };
        }
        static string SendCaptchaEmail(string LocalSSFilePath)
        {
            string SSEmailKey = "";
            using (SmtpClient GmailClient = new SmtpClient("smtp.gmail.com", 587))
            {
                System.Net.Mail.MailMessage MsgObj = ComposeEmailGeneralInfo(GmailClient);
                SSEmailKey = "Captcha: " + DateTime.Now.ToString("HH_mm_ss");
                MsgObj.Subject = SSEmailKey;
                MsgObj.Body = "Solve this captcha. Please respond in the format of {Captcha: ######!} Where the '#s' are the actual captcha.";
                System.Net.Mail.Attachment Image = new System.Net.Mail.Attachment(LocalSSFilePath);
                MsgObj.Attachments.Add(Image);
                GmailClient.Send(MsgObj);
            };
            // Currently doesn't work and will cause an unbreakable loop if uncommented, need to reach the captcha screen in order to get this to work.
            return ListenForEmailReply(SSEmailKey);
        }
        static void SendConfirmationEmail(float LocalTotal, int LocalPurchases)
        {
            using (SmtpClient GmailClient = new SmtpClient("smtp.gmail.com", 587))
            {
                System.Net.Mail.MailMessage MsgObj = ComposeEmailGeneralInfo(GmailClient);
                MsgObj.Subject = "A successful purchase has just been made!";
                MsgObj.Body = $"So far there has been {LocalPurchases} purchases and a total of ${LocalTotal} spent.";
                GmailClient.Send(MsgObj);
            };
        }
        static void SendAppQuitEmail(string LocalReason)
        {
            using (SmtpClient GmailClient = new SmtpClient("smtp.gmail.com", 587))
            {
                System.Net.Mail.MailMessage MsgObj = ComposeEmailGeneralInfo(GmailClient);
                MsgObj.Subject = "The application has shutdown!";
                MsgObj.Body = $"The application has reached and endpoint. Reason: {LocalReason}";
                GmailClient.Send(MsgObj);
            };
        }
        static System.Net.Mail.MailMessage ComposeEmailGeneralInfo(SmtpClient LocalGmailClient)
        {
            LocalGmailClient.EnableSsl = true;
            LocalGmailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            LocalGmailClient.UseDefaultCredentials = false;
            LocalGmailClient.Credentials = new NetworkCredential(SensitiveInfo.EMAIL, SensitiveInfo.EMAILPASS);
            System.Net.Mail.MailMessage MsgObj = new System.Net.Mail.MailMessage();
            MsgObj.To.Add(SensitiveInfo.EMAIL);
            MsgObj.From = new MailAddress(SensitiveInfo.EMAIL);
            return MsgObj;
        }
        static string ListenForEmailReply(string LocalSSEmailKey)
        {
            Stopwatch EmailResponseSW = Stopwatch.StartNew();
            while (true)
            {
                if (EmailResponseSW.Elapsed.TotalSeconds >= EmailResponseTimeAlloted)
                {
                    ImapClient IC = new ImapClient("imap.gmail.com", SensitiveInfo.EMAIL, SensitiveInfo.EMAILPASS, AuthMethods.Login, 993, true);
                    var SelectedMailbox = IC.SelectMailbox("INBOX");
                    for (int i = SelectedMailbox.NumMsg - 1; i > SelectedMailbox.NumMsg - 10; i--)
                    {
                        var Email = IC.GetMessage(i);
                        if (Email.Subject == "Re: " + LocalSSEmailKey && Email.Body.Substring(0, 8) == "Captcha:")
                        {
                            int EndOfCaptcha = Email.Body.IndexOf('!');
                            return Email.Body[9..EndOfCaptcha];
                        }
                    }
                    EmailResponseSW.Restart();
                }
            }
        }
        static string FindTotal(IWebDriver LocalWDriver)
        {
            IWebElement TotalElement;
            string Total = "";
            for (int i = 15; i > 0; i--)
            {
                // "Total" element can be listed farther down depending on how much effects the total (shipping costs, discounts, etc), but will always be the last thing in the table.
                try
                {
                    // Try catch is needed because element may not exist, if it doesn't then it continues to next iteration.
                    TotalElement = LocalWDriver.FindElement(By.XPath($"/html/body/div[5]/div[1]/div/div/div[2]/div/div/div[2]/div/div[1]/div/div[2]/div/div/div/table/tbody/tr[{i}]/td[2]"));
                    if (TotalElement.Text.StartsWith('$'))
                    {
                        Total = TotalElement.Text.Substring(1);
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            return Total;
        }
        static void PlaceOrder(IWebDriver LocalWDriver, TwitterClient LocalTwitterClient, IMessage LocalIM, string LocalUserFilePath, List<long> LocalUserIDs, int LocalProductsPurchased)
        {
            // Finds the "Use Payment Method" button
            LocalWDriver.FindElement(By.XPath("//input[@aria-labelledby=\"orderSummaryPrimaryActionBtn-announce\"]")).SendKeys(Keys.Return);
            Thread.Sleep(10000); // Let "place order" form load
            string Total = FindTotal(LocalWDriver);
            float TotalSpent = 0f;
            if (File.Exists("TotalSpent.json"))
            {
                TotalSpent = DeserializeTotalSpentJSON();
            }
            if (float.Parse(Total) <= MaxPurchaseAmount)
            {
                if (TotalSpent < AllowedTotal)
                {
                    // Uncomment Lines below to place order
                    LocalWDriver.FindElement(By.Name("placeYourOrder1")).SendKeys(Keys.Return); // (CAUTION) THIS HITS THE PLACE ORDER BUTTON AND SPENDS MONEY!!!!
                    Thread.Sleep(5000); //Allow order to be place...
                    string Reply = "Confirmation twitter reply message...";
                    SendTwitterReply(Reply, LocalTwitterClient, LocalIM);
                    // Adds user to User JSON file to prevent another purchase
                    LocalUserIDs.Add(LocalIM.SenderId);
                    SerializeUserJSON(LocalUserFilePath, LocalUserIDs);
                    SerializeTotalSpentJSON(float.Parse(Total), LocalWDriver, LocalIM, LocalProductsPurchased);
                }
                else
                {
                    // This code should be unreachable, but is here just in case.
                    string Reply = "End of project twitter reply message...";
                    SendTwitterReply(Reply, LocalTwitterClient, LocalIM);
                    LocalIM.DestroyAsync().Wait();
                    string Reason = "The application has detected that the total amount spent is over $1000, through 'unreachable code'.";
                    SendAppQuitEmail(Reason);
                    QuitApplication(LocalWDriver);
                }
            }
            else
            {
                string Reply = "Product too expensive twitter repsonse message...";
                SendTwitterReply(Reply, LocalTwitterClient, LocalIM);
            }
        }
        static void SerializeUserJSON(string LocalFilePath, List<long> LocalUserIDs)
        {
            string JSONObj = JsonSerializer.Serialize(LocalUserIDs);
            File.WriteAllText(LocalFilePath, JSONObj);
        }
        static List<long> DeserializerUserJSON(string FilePathLocal)
        {
            string JSONDes = "";
            using (StreamReader r = new StreamReader(FilePathLocal))
            {
                JSONDes = r.ReadToEnd();
            }
            List<long> UsersDes = JsonSerializer.Deserialize<List<long>>(JSONDes);
            return UsersDes;
        }
        static float DeserializeTotalSpentJSON()
        {
            string TotalSpentFilePath = "TotalSpent.json";
            string TotalOverallJSONDes = "";
            using (StreamReader r = new StreamReader(TotalSpentFilePath))
            {
                TotalOverallJSONDes = r.ReadToEnd();
            }

            return JsonSerializer.Deserialize<float>(TotalOverallJSONDes);
        }
        static void SerializeTotalSpentJSON(float LocalTotal, IWebDriver LocalWDriver, IMessage LocalIM, int LocalProductsPurchased)
        {
            string TotalSpentFilePath = "TotalSpent.json";
            float NewOverallTotal = 0;
            if (File.Exists(TotalSpentFilePath))
            {
                float TotalOverall = DeserializeTotalSpentJSON();
                //Update total
                NewOverallTotal = LocalTotal + TotalOverall;
                //Serialize
                string JSONObj = JsonSerializer.Serialize(NewOverallTotal);
                File.WriteAllText(TotalSpentFilePath, JSONObj);
                if (NewOverallTotal > AllowedTotal)
                {
                    SendConfirmationEmail(NewOverallTotal, LocalProductsPurchased);
                    string Reason = "The application has detected that the total amount spent is over $1000.";
                    LocalIM.DestroyAsync().Wait();
                    SendAppQuitEmail(Reason);
                    QuitApplication(LocalWDriver);
                }
            }
            else
            {
                NewOverallTotal = LocalTotal;
                string JSONObj = JsonSerializer.Serialize(LocalTotal);
                File.WriteAllText(TotalSpentFilePath, JSONObj);
            }
            LocalProductsPurchased++;
            SendConfirmationEmail(NewOverallTotal, LocalProductsPurchased);
        }
        static void SendTwitterReply(string LocalReply, TwitterClient LocalClient, IMessage LocalMessage)
        {
            Task<IMessage> ReplyMessage = LocalClient.Messages.PublishMessageAsync(new PublishMessageParameters(LocalReply, LocalMessage.SenderId));
            ReplyMessage.Result.DestroyAsync();
        }
        static void QuitApplication(IWebDriver LocalWDriver)
        {
            if (LocalWDriver != null)
            {
                LocalWDriver.Quit();
            }
            File.Delete("Users.json");
            Environment.Exit(0);
        }
    }
}
