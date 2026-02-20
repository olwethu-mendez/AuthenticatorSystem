using BusinessLogicLayer.Infrastructure.Documentation.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Infrastructure.Documentation.Implementation
{
    public class ProfileDocumentation : IDocDefinition
    {
        public Dictionary<string, (string Summary, string Remarks, List<ResponseInfo> Responses)> GetDocs() => new()
        {
            ["create-profile"] = ("Create Profile",
                "A profile creation endpoint to complete registering a new user account\n" +
                "A user provides their first and last name, gender (and ideally, maybe a profile picture)", new() {
            new(200, "Refresh and Access tokens and their expiry date"), new(400, "Bad Request"), new(500, "Error")
        }),
            ["get-profile"] = ("Get Profile",
            "An endpoint used to get the profile of the logged in user.", new() {
            new(200, "A user Profile"), new(401, "Unauthorized"), new(500, "Error")
        }),
            ["update-profile"] = ("Update Profile Details",
            "An endpoint for updating the profile details of the logged user", new() {
            new(204, string.Empty), new(400, "Bad Request"), new(401, "Unauthorized")
        }),
            ["activation-status"] = ("Activate/Deactivate Account",
            "This endpoint can either deactivate or activate your account", new() {
            new(204, string.Empty), new(400, "Bad Request"), new(401, "Unauthorized")
        }),
            ["admin-deactivation"] = ("Admin Activate/Deactivate Account",
            "This endpoint alos either deactivate or activate your account but can be done with admin privileges and only the admin can undo it.", new() {
            new(200, "Activated/Deactivated"), new(400, "Bad Request"), new(401, "Unauthorized"), new(403, "Forbidden"), new(404, "Not Found"), new(500, "Server Error")
        })
        };
    }
}
