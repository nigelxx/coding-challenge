using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel.Syndication;
using System.Xml;

namespace News_Console
{
    class Program
    {
        static public string underline = "-----------------------------------------";

        static void Main(string[] args)
        {
            // Render introduction text and menu options
            IntroText();

            bool quit = false;
            while (!quit)
            {
                string userInput;
                userInput = Console.ReadLine().ToUpper();
                if (userInput == "Q")
                {
                    quit = true;
                }
                else if (userInput == "CLS")
                {
                    // allow user to clear screen. Re-render introduction text
                    Console.Clear();
                    IntroText();
                }
                else
                {
                    // Numeric user input 1-5 
                    if (userInput == "1" || userInput == "2" || userInput == "3" || userInput == "4" || userInput == "5")
                    {
                        // Clear any previos news articles. Re-render introduction text
                        Console.Clear();
                        IntroText();

                        // Process the user input of the symbol 
                        GetRSSFeed(userInput);
                    }
                    else
                    {
                        // Invalid input. Re-render introduction text
                        Console.Clear();
                        IntroText();
                        Console.WriteLine("Enter a number between 1 and 5");
                    }
                }

            }

        } // end Main 


        // Introduction Text / Program Description / User Menu Options 
        static public void IntroText()
        {
            Console.WriteLine(underline);
            Console.WriteLine("| NEWS CONSOLE v.1.0                    |");
            Console.WriteLine("| (c)2022 Nigel Martin                  |");
            Console.WriteLine("| RSS News Source: express.co.uk        |");
            Console.WriteLine(underline);
            Console.WriteLine("");

            Console.WriteLine("Q   = Quit");
            Console.WriteLine("CLS = Clear Screen");
            Console.WriteLine("");

            // Show News selections
            Console.WriteLine("Enter a number for the following News");
            Console.WriteLine("1) UK News");
            Console.WriteLine("2) World News");
            Console.WriteLine("3) Weather");
            Console.WriteLine("4) Science");
            Console.WriteLine("5) Tech");
            Console.WriteLine("");
        }

        // Accept numeric input, return a specific RSS url
        static public string FeedByInteger(string FeedNumber)
        {
            string strSymbol = string.Empty;
            switch (FeedNumber)
            {
                case "1":
                    strSymbol = "https://www.express.co.uk/posts/rss/1/uk";
                    break;
                case "2":
                    strSymbol = "https://www.express.co.uk/posts/rss/78/world";
                    break;
                case "3":
                    strSymbol = "https://www.express.co.uk/posts/rss/153/weather";
                    break;
                case "4":
                    strSymbol = "https://www.express.co.uk/posts/rss/151/science";
                    break;
                case "5":
                    strSymbol = "https://www.express.co.uk/posts/rss/59/tech";
                    break;
                default:
                    // otherwise default to 
                    strSymbol = "https://www.express.co.uk/posts/rss/1/uk";
                    break;
            }

            return (strSymbol);
        }

        // Goto express.co.uk and retrieve RSS news feed
        // Render a formatted output to console. 

        static public void GetRSSFeed(string CategoryId)
        {
            // Initialise RSS feed object 
            SyndicationFeed feed = null;

            // Look up RSS feed URL
            string FeedUrl = FeedByInteger(CategoryId);

            try
            {
                using (var reader = XmlReader.Create(FeedUrl))
                {
                    feed = SyndicationFeed.Load(reader);
                }
            }
            catch
            {
                Console.WriteLine("GetRSSFeed Error");
            }

            if (feed != null)
            {
                // Create string builder to build the output
                StringBuilder sb = new StringBuilder();

                // 1st underline 
                sb.Append(underline + "\r\n\r\n");

                // Loop through all RSS items 
                foreach (var element in feed.Items)
                {
                    // integers for finding index of particlar strings
                    int splitIdx = 0;
                    int RssSummaryLength = 0;
                    int ImgIdx = 0;

                    // clear previous variables 
                    string NewsDesc = string.Empty;
                    string ImgTag = string.Empty;
                    string ImgUrl = string.Empty;

                    // Get the RSS variables.
                    // Additional variables are available as required, for example Link and image tag
                    string NewsTitle = element.Title.Text;
                    string Link = element.Id.ToString();
                    string RssSummary = element.Summary.Text;
                    string strPubDate = element.PublishDate.ToString("dd-MM-yyyy HH:mm");
                    strPubDate = strPubDate.Replace(" +01:00", string.Empty);
                    DateTime PubDate = Convert.ToDateTime(strPubDate);

                    // Process the RssSummary value. Strip the html breaks, href and link from RssSummary string
                    RssSummary = RssSummary.Replace("<br /><br />", string.Empty);
                    splitIdx = RssSummary.IndexOf("</a>");
                    RssSummaryLength = RssSummary.Length;

                    // Find the News description without the image tag
                    NewsDesc = RssSummary.Substring(splitIdx + 4);

                    // Optional image tag variables
                    ImgTag = RssSummary.Substring(0, splitIdx);
                    ImgIdx = ImgTag.IndexOf(">");
                    ImgUrl = ImgTag.Substring(ImgIdx + 1);
                    ImgUrl = ImgUrl.Replace("<img src=\"", string.Empty).Replace("\"/>", string.Empty);

                    // Append the string values from the RSS feed to the string builder object
                    // sb.Append("*** " + NewsTitle + " ***\n" + strPubDate + " " + Link + "\r\n\r\n" + underline + "\r\n\r\n");

                    // Optional Output which includes the full NewsDesc
                    sb.Append("*** " + NewsTitle + " ***\n" + NewsDesc + "\r\n\r\nDate: " + strPubDate + "\nURL: " + Link + "\r\n\r\n" + underline + "\r\n\r\n");

                } // end for each loop

                // Finally output the entire string to the console.
                Console.WriteLine(sb.ToString());
            }
            else
            {
                // RSS feed is null. Write error to console. 
                Console.WriteLine("RSS Feed is null\n" + FeedUrl);
            }
        } // end of GetRSSFeed function 


    } // end class
}     // end namespace
