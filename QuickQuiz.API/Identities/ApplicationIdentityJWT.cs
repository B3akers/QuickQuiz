using System.Security.Principal;
using System.Text.Json.Serialization;

namespace QuickQuiz.API.Identities
{
    public class ApplicationIdentityJWT : IIdentity
    {
        public ApplicationIdentityJWT(string id, string name, bool twitch)
        { 
            Id = id;
            Name = name;
            Twitch = twitch;
        }

        [JsonIgnore]
        public virtual string AuthenticationType => "ApplicationIdentityJWT";

        [JsonIgnore]
        public virtual bool IsAuthenticated => true;

        public virtual string Id { get; }
        public virtual string Name { get; }
        public virtual bool Twitch { get; }
    }
}
