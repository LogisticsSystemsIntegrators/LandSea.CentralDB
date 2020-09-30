using Newtonsoft.Json.Linq;

namespace Demo.Library
{
    public class Authentication
    {
        public string AuthenticationToken(string authURL, string username, string password)
        {
            if (string.IsNullOrEmpty(authURL))
            {
                return "Empty authentication URL.";
            }
            if (string.IsNullOrEmpty(username))
            {
                return "Empty username.";
            }
            if (string.IsNullOrEmpty(password))
            {
                return "Empty password.";
            }

            string tokenResponseValue = string.Empty;
            string tokenvalue = string.Empty;

            System.Net.WebRequest req = System.Net.WebRequest.Create(authURL + "?username=" + username + "&password=" + password);
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "GET";

            string tokenResponse = string.Empty;
            System.Net.WebResponse resp = req.GetResponse();
            if (resp == null)
            {
                return "Authentication failed.";
            }
            using (System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream()))
            {
                tokenResponse = sr.ReadToEnd().Trim();
            }
            if (!string.IsNullOrEmpty(tokenResponse))
            {
                tokenResponseValue = "Data";
                //check if we can get the response property
                tokenvalue = JObject.Parse(tokenResponse)[tokenResponseValue].ToString();
            }

            return tokenvalue;
        }
    }
}