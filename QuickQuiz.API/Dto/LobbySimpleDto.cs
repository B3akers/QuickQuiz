namespace QuickQuiz.API.Dto
{
    public class LobbySimpleDto
    {
        public string Id { get; set; }
        public PlayerDto Owner { get; set; }
        public string ActiveGameId { get; set; }
        public int PlayersCount { get; set; }
    }
}
