using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using exampleWebAPI.Models;

namespace exampleWebAPI.Context
{
    public class TokenContext
    {
        private readonly WeerstationContext _context;

        private const string Url = "http://iot.jorgvisch.nl";
        private const string LoginUrl = "/Account/Login";
        private const string ResetTokenUrl = "Manage/ResetTokenRequest";
        private const string TokenUrl = "/Token";

        public TokenContext()
        {
            _context = new WeerstationContext();
        }

        public async Task<bool> ResetTokenNow(User user)
        {
            user = Login(user).Result;

            var client = new HttpClient {BaseAddress = new Uri(Url)};
            var request = new HttpRequestMessage(HttpMethod.Post, ResetTokenUrl);


            var keyValues = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("resettoken", "Reset token"),
            };
            request.Content = new FormUrlEncodedContent(keyValues);
            request.Headers.Add("Cookie",
                user.Cookies.ElementAt(0).Name + "=" + user.Cookies.ElementAt(0).Value + ";" +
                user.Cookies.ElementAt(1).Name + "=" +
                user.Cookies.ElementAt(1).Value +
                ";");

            try
            {
                var response = await client.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(e.Message);
                return false;
            }
        }

        public async Task<User> Login(User user)
        {
            var cookies = new CookieContainer();
            var handler = new HttpClientHandler();
            var client = new HttpClient(handler) {BaseAddress = new Uri(Url)};
            handler.CookieContainer = cookies;
            var request = new HttpRequestMessage(HttpMethod.Post, LoginUrl);

            user = GetRequestVerificationToken(user);

            var keyValues = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("__RequestVerificationToken", user.RequestVerificationToken),
                new KeyValuePair<string, string>("email", user.Email),
                new KeyValuePair<string, string>("password", user.Password)
            };
            request.Content = new FormUrlEncodedContent(keyValues);

            request.Headers.Add("Cookie", user.Cookies.ElementAt(0).Name + "=" + user.Cookies.ElementAt(0).Value + ";");
            try
            {
                var response = await client.SendAsync(request);
                var responseCookies = cookies.GetCookies(new Uri(Url)).Cast<Cookie>();
                var enumerable = responseCookies as Cookie[] ?? responseCookies.ToArray();

                var secondCookie = new MyCookie
                {
                    Name = enumerable.First().Name,
                    Value = enumerable.First().Value
                };


                user.Cookies.Add(secondCookie);
                user.Token = new Token(response.Content.ReadAsStringAsync().Result);
                _context.User.Update(user);
                return user;
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(e.Message);
            }
            return user;
        }

        public User GetRequestVerificationToken(User user)
        {
            var request = (HttpWebRequest) WebRequest.Create(Url + LoginUrl);
            request.Method = "GET";
            request.ContentType = "application/json";

            try
            {
                request.CookieContainer = new CookieContainer();
                var webResponse = request.GetResponse();

                using (var webStream = webResponse.GetResponseStream())
                {
                    if (webStream == null) return user;

                    using (var responseReader = new StreamReader(webStream))
                    {
                        var response = responseReader.ReadToEnd();

                        var regex = new Regex("name=\"__RequestVerificationToken\" type=\"hidden\" value=\"(.*)\"");
                        var tokenVer = regex.Match(response).Groups[1];

                        user.RequestVerificationToken = tokenVer.ToString();

                        var firstCookie = new MyCookie
                        {
                            Name = request.CookieContainer.GetCookies(new Uri(Url))[0].Name,
                            Value = request.CookieContainer.GetCookies(new Uri(Url))[0].Value
                        };
                        _context.MyCookies.Update(firstCookie);
                        user.Cookies = new List<MyCookie> {firstCookie};
                        _context.User.Update(user);

                        return user;
                    }
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(e.Message);
                return new User("fout RT 147 catch ", e.ToString());
            }
        }

        public async Task<Token> GetToken(User user)
        {
            var client = new HttpClient {BaseAddress = new Uri(Url)};
            var request = new HttpRequestMessage(HttpMethod.Post, TokenUrl);

            var keyValues = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", user.Email),
                new KeyValuePair<string, string>("password", user.Password)
            };
            request.Content = new FormUrlEncodedContent(keyValues);
            try
            {
                var response = await client.SendAsync(request);
                return response.IsSuccessStatusCode ? new Token(response.Content.ReadAsStringAsync().Result) : null;
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(e.Message);
                return null;
            }
        }

        public bool CheckToken(User user, HttpContext httpContext)
        {
            if (user.Token != null && user.Token.IsValid()) return true;
            if (ResetTokenNow(user).Result)
                user.Token = GetToken(user).Result;
            return user.Token != null && user.Token.IsValid();
        }
    }
}