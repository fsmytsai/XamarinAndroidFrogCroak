namespace FrogCroak.Models
{
    public class ChatMessage
    {
        public int _id;
        public string message;
        public int from;
        public int type;

        public ChatMessage(int _id, string message, int from, int type)
        {
            this._id = _id;
            this.message = message;
            this.from = from;
            this.type = type;
        }
    }
}