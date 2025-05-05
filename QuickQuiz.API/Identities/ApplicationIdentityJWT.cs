using QuickQuiz.API.Interfaces;
using System.Security.Principal;
using System.Text.Json.Serialization;

namespace QuickQuiz.API.Identities
{
    public class ApplicationIdentityJWT : IIdentity
    {
        public ApplicationIdentityJWT(string id, string name, string authSource)
        {
            Id = id;
            Name = name;
            AuthSource = authSource;
        }

        [JsonIgnore]
        public virtual string AuthenticationType => "ApplicationIdentityJWT";

        [JsonIgnore]
        public virtual bool IsAuthenticated => true;

        public virtual string Id { get; }
        public virtual string Name { get; }
        public virtual string AuthSource { get; }
    }
}
