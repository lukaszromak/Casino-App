using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using PZ_PROJEKT.Data;
using PZ_PROJEKT.Models;
using System;
using System.Security.Claims;
using System.Text.Json;
using System.Web;

namespace PZ_PROJEKT.Controllers
{
    public class AuthController : Controller
    {
        private static bool debugxd = false;
        private IHttpClientFactory _clientFactory;
        private readonly IConfiguration _config;
        private string SteamApiKey;
		private readonly PZ_PROJEKTContext _context;

		public AuthController(IHttpClientFactory clientFactory, IConfiguration configuration, PZ_PROJEKTContext context)
        {
            _clientFactory = clientFactory;
            _config = configuration;
            SteamApiKey = _config["SteamApiKey"];
            _context = context;
        }

        public IActionResult Index()
        {
            return Redirect("https://steamcommunity.com/openid/login?openid.ns=http://specs.openid.net/auth/2.0&openid.claimed_id=http://specs.openid.net/auth/2.0/identifier_select&openid.identity=http://specs.openid.net/auth/2.0/identifier_select&openid.return_to=https://localhost:7253/Auth/Callback&openid.realm=https://localhost:7253&openid.mode=checkid_setup");
        }

        public async Task<IActionResult> Callback(
            [FromQuery(Name = "openid.ns")] string ns,
            [FromQuery(Name = "openid.mode")] string mode,
            [FromQuery(Name = "openid.op_endpoint")] string opEndpoint,
            [FromQuery(Name = "openid.claimed_id")] string claimedId,
            [FromQuery(Name = "openid.identity")] string identity,
            [FromQuery(Name = "openid.return_to")] string returnTo,
            [FromQuery(Name = "openid.response_nonce")] string responseNonce,
            [FromQuery(Name = "openid.assoc_handle")] string assocHandle,
            [FromQuery(Name = "openid.signed")] string signed,
            [FromQuery(Name = "openid.sig")] string sig
            )
        {
            if (debugxd)
            {
                User user = _context.User.FirstOrDefault(user => user.SteamId == "69");

                if (user == null)
                {
                    User newUser = new User();
                    newUser.SteamId = "69";
                    newUser.Balance = 1_000_000;
                    _context.User.Add(newUser);
                    await _context.SaveChangesAsync();
                }
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "lukasz"),
                    new Claim("Id", "69"),
                    new Claim("AvatarUrl", "spucha")
                };
                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuthentication");
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(
                    "CookieAuthentication",
                    new ClaimsPrincipal(claimsIdentity),
                authProperties);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                var uriBuilder = new UriBuilder("https://steamcommunity.com/openid/login");
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);

                query["openid.ns"] = ns;
                query["openid.mode"] = "check_authentication";
                query["openid.op_endpoint"] = opEndpoint;
                query["openid.claimed_id"] = claimedId;
                query["openid.identity"] = identity;
                query["openid.return_to"] = returnTo;
                query["openid.response_nonce"] = responseNonce;
                query["openid.assoc_handle"] = assocHandle;
                query["openid.signed"] = signed;
                query["openid.sig"] = sig;

                uriBuilder.Query = query.ToString();
                var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);
                var client = _clientFactory.CreateClient();
                var response = await client.SendAsync(request);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (!responseBody.Contains("is_valid:true"))
                {
                    return RedirectToAction("AccessDenied", "Auth");
                }

                string steamId = ParseSteamId(claimedId);

                string userInfoUrl = String.Format("http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={0}&steamids={1}", SteamApiKey, steamId);
                var userInfoRequest = new HttpRequestMessage(HttpMethod.Get, userInfoUrl);
                var userInfoResponse = await client.SendAsync(userInfoRequest);
                string userInfoResponseBody = await userInfoResponse.Content.ReadAsStringAsync();
                string userAvatarUrl = ParseJsonProperty(userInfoResponseBody, "avatarfull");
                string userName = ParseJsonProperty(userInfoResponseBody, "personaname");

                User user = _context.User.FirstOrDefault(user => user.SteamId == steamId);

                if(user == null)
                {
                    User newUser = new User();
                    newUser.SteamId = steamId;
                    newUser.Balance = 0;
                    _context.User.Add(newUser);
                    await _context.SaveChangesAsync();
                }

                var claims = new List<Claim>
                {
                new Claim(ClaimTypes.Name, userName),
                new Claim("Id", steamId),
                new Claim("AvatarUrl", userAvatarUrl),
                new Claim(ClaimTypes.Role, "USER")
                };

                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuthentication");
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(
                    "CookieAuthentication",
                    new ClaimsPrincipal(claimsIdentity),
                authProperties);

                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuthentication");
            return RedirectToAction("Index", "Home");
        }

        public string ParseSteamId(string steamId)
        {
            var tmp = steamId.Split("/");
            return tmp[tmp.Length - 1];
        }

        public static string ParseJsonProperty(string jsonString, string propertyName)
        {
            Console.WriteLine(jsonString);
            using (JsonDocument document = JsonDocument.Parse(jsonString))
            {
                JsonElement root = document.RootElement;
                if (root.TryGetProperty("response", out JsonElement responseElement) &&
                    responseElement.TryGetProperty("players", out JsonElement playersElement) &&
                    playersElement.ValueKind == JsonValueKind.Array &&
                    playersElement.GetArrayLength() > 0)
                {
                    JsonElement playerElement = playersElement[0];
                    if (playerElement.TryGetProperty(propertyName, out JsonElement propertyElement))
                    {
                        return propertyElement.ToString();
                    }
                }
            }
            return null; // or throw an exception, or return a default value
        }
    }
}
