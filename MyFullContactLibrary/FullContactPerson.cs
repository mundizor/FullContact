using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json.Linq;

namespace MyFullContactLibrary
{

    public interface IFullContactApi
    {
        Task<FullContactPerson> LookupPersonByEmailAsync(string email);
    }

    public class FullContactPerson
    {
        public string likelihood;
        public List<String> socialProfiles;
        public List<String> contactInfo;

        public FullContactPerson(string inLikeliood, List<String> inSocialProfiles, List<String> inContactInfo)
        {
            likelihood = inLikeliood;
            socialProfiles = inSocialProfiles;
            contactInfo = inContactInfo;
        }

    }

    public class FullContactAPIClass : IFullContactApi
    {

        public Task<FullContactPerson> LookupPersonByEmailAsync(string email)
        {
            return LookupPerson(email);
        }

        private IEnumerable<JToken> AllChildren(JToken json)
        {
            foreach (var c in json.Children())
            {
                yield return c;
                foreach (var cc in AllChildren(c))
                {
                    yield return cc;
                }
            }
        }

        private string getSingleValueFromJson(JObject jObj, string name)
        {
            return (string)jObj.Descendants()
                .OfType<JProperty>()
                .Where(p => p.Name == name)
                .First()
                .Value;
        }

        private List<JProperty> getMultipleValues(JObject jObj, string name)
        {
            var resultObjects = AllChildren(jObj)
                .First(c => c.Type == JTokenType.Array && c.Path.Contains(name))
                .Children<JObject>();

            List<JProperty> tempList = new List<JProperty>();
            foreach (JObject result in resultObjects)
            {
                foreach (JProperty property in result.Properties())
                {

                    tempList.Add(property);

                }

            }

            return tempList;
        }

        private async Task<FullContactPerson> LookupPerson(string email)
        {

            string requestUrl = "https://api.fullcontact.com/v2/person.json?email=" + email + "&apiKey=xxxxxxxxxxxxxxxx";


            var json = "";
            try
            {
                json = new WebClient().DownloadString(requestUrl);
            }
            catch (WebException ex)
            {
                Console.WriteLine("WebClient().DownloadString(requestUrl) failed. ");
                Console.WriteLine("could not even get a response to parse to get the error code ");
                Console.WriteLine("response: " + json);
                Console.ReadKey();

                return null;
            }


            JObject jObj = JObject.Parse(json);

            //get status to see if we got a correct response.
            string status = getSingleValueFromJson(jObj, "status");

            //wrong return value. try again. 
            if (status != "200")
            {
                Console.WriteLine("error code: " + status);
                Console.WriteLine("try again");
                Console.ReadKey();

                return null;
            }

            //get likelihood from Jobject
            string LH = getSingleValueFromJson(jObj, "likelihood");

            List<String> SocialProfilesList = new List<String>();

            //add every social profiles type to the list
            foreach (JProperty property in getMultipleValues(jObj, "socialProfiles"))
            {
                if (property.Name != "type")
                    continue;

                SocialProfilesList.Add(property.ToString());

            }

            List<String> ContactInfoList = new List<String>();

            //add all contact info to the list.
            foreach (JProperty property in getMultipleValues(jObj, "contactInfo"))
            {

                ContactInfoList.Add(property.ToString());

            }

            return new FullContactPerson(LH, SocialProfilesList, ContactInfoList);
        }

    }


}

