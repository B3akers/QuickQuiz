using System.Security.Principal;

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

        public virtual string AuthenticationType => "ApplicationIdentityJWT";
        public virtual bool IsAuthenticated => true;
        public virtual string Id { get; }
        public virtual string Name { get; }
        public virtual bool Twitch { get; }
    }
}
