using QuickQuiz.API.WebSockets.Data;
using System.Text.Json.Serialization;

namespace QuickQuiz.API.WebSockets.Packets
{
    [JsonDerivedType(typeof(QuestionReportRequestPacket), typeDiscriminator: "reportQuestion")]
    public class QuestionReportRequestPacket : BasePacketRequest
    {
        public string QuestionId { get; set; }
        public int Reason { get; set; }
    }
}
