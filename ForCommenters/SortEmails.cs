using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using AE.Net.Mail;

namespace SortEmails
{
  class Program
  {
    static string YourPassword;
    static string YourEmail;
    static string Objective;
    static ImapClient IC;
    static MailMessage[] EmailArray;
    static bool Continue = true;
    static void Main(string[] args)
    {
      Console.WriteLine("Loading...");
      YourPassword = "PASSWORD";
      YourEmail = "EMAIL";
      IC = new ImapClient("imap.gmail.com", YourEmail, YourPassword, AuthMethods.Login, 993, true);
      IC.SelectMailbox("[Gmail]/All Mail");
      EmailArray = IC.GetMessages(0, IC.GetMessageCount());
      Console.WriteLine("\n \n");

      Console.WriteLine("Hello, User!");
      Console.WriteLine("Total emails: " + EmailArray.Length + "\n");
      while (Continue)
      {
        Continue = RunObjective();
        Console.WriteLine("\n \n");
      }
      Console.WriteLine("Objective Finished");
      Console.ReadLine();
    }

    static bool RunObjective()
    {
      bool Continue = true;
      Console.WriteLine("What is the objective today? \n You can choose: \n D - Delete \n S - Sort \n R - ReadAll \n");
      Objective = Console.ReadLine();

      if (Objective != "break")
      {
        if (Objective.ToLower() == "r") //Read All
        {
          try
          {
            foreach (MailMessage email in EmailArray)
            {
              IC.GetMessage(email.Uid, false, true);
            }
          }
          catch (Exception ex)
          {
            Console.WriteLine(ex);
          }

        }

        else if (Objective.ToLower() == "d") //Deleting
        {
          Console.WriteLine("\n" + "How are we searching? By sender? Or by email contents?");
          Console.WriteLine("You can choose: \n S - Sender \n C - Contents \n");
          string DeleteObjective = Console.ReadLine();
          Console.WriteLine("\n" + "What are we deleting? \n");
          string DeleteKeyword = Console.ReadLine();

          if (DeleteObjective.ToLower() == "s")
          {
            int tally2 = 0;
            int tally1 = 0;
            Console.WriteLine("\n" + "Deleting Emails...");
            foreach (MailMessage email in EmailArray)
            {
              Console.WriteLine(tally1);
              tally1++;
              if (email.From != null)
              {

                if (email.From.ToString().ToUpper().Contains(DeleteKeyword.ToUpper()))
                {
                  Console.WriteLine("Deleting Email From: " + email.From.ToString());
                  tally2++;
                  IC.MoveMessage(email.Uid, "[Gmail]/Trash");

                }
              }
            }
            Console.WriteLine("Total Emails Deleted: " + tally2);
          }
          else if (DeleteObjective.ToLower() == "c")
          {
            foreach (MailMessage email in EmailArray)
            {
              if (email.Body.ToString().ToUpper().Trim(' ').Contains(DeleteKeyword))
              {
                IC.DeleteMessage(email);
                IC.MoveMessage(email.Uid, "[Gmail]/Trash");
              }
            }
          }

        }

        else if (Objective.ToLower() == "s") //Sorting
        {
          int tally1 = 0;
          Console.WriteLine("\n" + "What's the keyword we are sorting? \n");
          string Keyword = Console.ReadLine();
          Console.WriteLine("\n" + "Remember that all folders being sorted to must be under the 'Auto Sort' label.");
          Console.WriteLine("Which folder are we moving to? \n");
          string Folder = Console.ReadLine();
          Console.WriteLine("\n" + "Moving emails from " + Keyword.ToUpper() + " to folder 'Auto Sort/" + Folder + "'");
          try
          {
            foreach (MailMessage email in EmailArray)
            {
              if (email.From != null)
              {
                if (email.From.ToString().ToUpper().Contains(Keyword.ToUpper()))
                {
                  Console.WriteLine("Moving email: " + email.From);
                  IC.MoveMessage(email.Uid, "Auto Sort/" + Folder);
                  tally1++;
                }
              }
            }
            Console.WriteLine(tally1 + "messages moved to 'Auto Sort/" + Folder);
          }
          catch (Exception ex)
          {
            Console.WriteLine(ex);
            Console.WriteLine();
          }
        }
      }
      else
      {
        Continue = false;
      }
      return Continue;
    }
  }
}
