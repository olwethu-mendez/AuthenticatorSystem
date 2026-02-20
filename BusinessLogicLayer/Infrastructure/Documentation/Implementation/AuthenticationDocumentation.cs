using BusinessLogicLayer.Infrastructure.Documentation.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Infrastructure.Documentation.Implementation
{
    public class AuthenticationDocumentation : IDocDefinition
    {
        public Dictionary<string, (string Summary, string Remarks, List<ResponseInfo> Responses)> GetDocs() => new()
        {
            ["register"] = ("Register",
                "A user registration endpoint to create a new user account\n" +
                "A phone number & country code are not required but a country code is if a phone number is provided\n " +
                "A phone number must be exactly 9 digits long\n " +
                "A country code must be in the format +[1-3 digits] (e.g., +1, +27, +266)", new() {
            new(201, "Refresh and Access tokens and their expiry date"), new(400, "Bad Request"), new(500, "Error")
        }),
            ["login"] = ("Login", "A sign in endpoint\n" +
            "A phone number or email can both work with a valid password to login", new() {
            new(200, "Refresh and Access tokens and their expiry date"), new(401, "Unauthorized"), new(500, "Error")
        }),
            ["refresh-token"] = ("Refresh Token", "An endpoint used to request a new access token.\n" +
            "The most recent access token is passed with the refresh token on the body\n" +
            "(though I later want to test if it can't just be passed from the header like normal)\n" +
            "and based on the two tokens, a new access token should be generated (unless it hadn't expired or blacklisted)", new() {
            new(200, "Refresh and Access tokens and their expiry date"), new(401, "Unauthorized")
        }),
            ["logout"] = ("Logout", "A sign out endpoint\n" +
            "A user is unauthenticated by blacklisting access token and revoking refresh token pending next login", new() {
            new(204, string.Empty), new(401, "Unauthorized")
        })
        };
    }
}
