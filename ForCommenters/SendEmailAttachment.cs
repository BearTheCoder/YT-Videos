// Pulled from Microsoft Docs https://docs.microsoft.com/en-us/dotnet/api/system.net.mail.attachment?view=net-5.0
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
namespace TestImgSend {
    class Program {
        static void Main(string[] args) {
            try {
                using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587)) {
                    string MyEmail = ""; // Put your email here
                    string Recipient = ""; // Put recipient email here
                    string MyPassword = ""; // Put password Here
                    string Attachment = @""; // Put File Location here - JPEG or PNG worked for me.
                    client.EnableSsl = true;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(MyEmail, MyPassword);
                    MailMessage message = new MailMessage(MyEmail, Recipient, "Email Subject", "Email Body");
                    Attachment Image = new Attachment( Attachment, MediaTypeNames.Image.Jpeg);
                    // Adding metadata. Probably not necessary, IDK didn't check
                    ContentDisposition Dispo = Image.ContentDisposition;
                    Dispo.CreationDate = File.GetCreationTime(Attachment);
                    Dispo.ModificationDate = File.GetLastWriteTime(Attachment);
                    Dispo.ReadDate = File.GetLastAccessTime(Attachment);
                    message.Attachments.Add(Image);
                    client.Send(message);
                    Console.WriteLine("Message sent successfully!");
                }
            }
            catch {
                Console.WriteLine("Message Unsuccessful");
            }
            Console.ReadLine();
        }
    }
}
