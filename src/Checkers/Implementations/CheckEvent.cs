using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using HtmlAgilityPack;
using TicketNotifier.Checkers.Interfaces;
using TicketNotifier.Entities;
using TicketNotifier.Repositories.Interfaces;

namespace TicketNotifier.Checkers.Implementations
{
    public class CheckEvent : ICheckEvent
    {
        private readonly IUserRepository _userRepository;
        private const string DateTimeFormat = "ddd, dd/MM/yy"; 

        public CheckEvent(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Run()
        {
            var allUsers = await _userRepository.GetAllUsers();
            var userEvents = allUsers.SelectMany(user => user.Events).ToList();

            foreach (var userEvent in userEvents)
            {
                var htmlDecode = await GetDecodedHtml(userEvent);
                var doc = CreateHtmlDocument();
                doc.LoadHtml(htmlDecode);
                var htmlNode = GetUserStageFromNode(doc, userEvent);

                if (htmlNode == null) continue;
                    
                var dateTime = GetDateTimeFromNode(htmlNode);
                var exactTime = DateTime.TryParseExact(dateTime, DateTimeFormat, 
                    new CultureInfo("tr-TR"), DateTimeStyles.None, out var eventDateTime);

                if (!exactTime) continue;
                    
                if (CompareEventAndUserDateTime(eventDateTime, userEvent))
                {
                    Console.WriteLine("send mail");
                }
            }

            Console.WriteLine("test");
        }

        private static HtmlDocument CreateHtmlDocument()
        {
            return new HtmlDocument();
        }

        private static async Task<string> GetDecodedHtml(Event userEvent)
        {
            var text = await GetHtmlFromPlaceLink(userEvent);
            var decodeV1 = DecodeText(text);
            var decodedHtml = DecodeText(decodeV1);
            return decodedHtml;
        }

        private static string DecodeText(string text)
        {
            return System.Web.HttpUtility.HtmlDecode(text);
        }

        private static async Task<string> GetHtmlFromPlaceLink(Event userEvent)
        {
            return await userEvent.PlaceLink.GetStringAsync();
        }

        private static HtmlNode GetUserStageFromNode(HtmlDocument doc, Event userEvent)
        {
            return doc.DocumentNode.Descendants("a").FirstOrDefault(x =>
                x.InnerHtml.Contains(userEvent.Stage, StringComparison.InvariantCultureIgnoreCase));
        }

        private static string GetDateTimeFromNode(HtmlNode htmlNode)
        {
            var parentNode =
                htmlNode.ParentNode.ParentNode.ParentNode.ParentNode.NextSibling.NextSibling.ChildNodes
                    .FirstOrDefault(x => x.Name.Contains("div", StringComparison.InvariantCultureIgnoreCase));
            var dateTime = parentNode?.FirstChild.NextSibling.InnerHtml;
            return dateTime;
        }

        private static bool CompareEventAndUserDateTime(DateTime eventDateTime, Event userEvent)
        {
            return eventDateTime > userEvent.StartDate && eventDateTime < userEvent.EndDate;
        }
    }
}