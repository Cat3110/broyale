using System;

namespace SocketIO.Data.Responses
{
    public enum Status
    {
        Ok = 200,
        NotExist = 404,
        Error = 500
    }
    public interface Response
    {
        Status status { get; }
        string error { get; }
    }

    public class BaseResponse : Response
    {
        public Status status { get; private set; }
        public string error { get; private set; }

        public bool IsSuccess => status == Status.Ok;

        public override string ToString()
        {
            return $"Status:{status} Error:{error}";
        }
    }
    
    public class VerifyResponse : BaseResponse
    {
        public class Data
        {
            public bool is_new { get; set; }
            public User user { get; set; }
        }

        public Data data { get; set; }
    }
    
    public class CharactersResponse : BaseResponse
    {
        public class Data
        {
            public Character[] characters { get; set; }
        }

        public Data data { get; set; }
    }
    
    public class CreateGameResponse : BaseResponse
    {
        public class Data
        { 
            public Game game { get; set; }
            public int eta { get; set; }
        }

        public Data data { get; set; }
    }

    public class UpdateGamesListEvent
    {
        public Game[] games;
    }
    
    public class Game
    {
        public string id { get; set; }
        public string name { get; set; }
        
        public string owner { get; set; }
        
        public bool isStarted { get; set; }
        public bool isFinished { get; set; }
        
        public int waitingTime { get; set; }
        
        public DateTime createTime{ get; set; }
        public DateTime startTime{ get; set; }
        public DateTime endTime{ get; set; }
        public ServerInfo serverInfo { get; set; }
        public User[] players { get; set; }
    }
    
    public class StartGameResponse : BaseResponse
    {
        public class Data
        {
            public float time { get; set; }
            public string address { get; set; }
            public int port { get; set; }
        }

        public Data data { get; set; }
    }
    
    public class UpdateGamesListResponse : BaseResponse
    {
        public class Data
        {
            public Game[] games { get; set; }
        }
        public Data data { get; set; }
    }
    
    public class JoinGameResponse : BaseResponse
    {
        public class Data
        {
            public Game game { get; set; }
            public int eta { get; set; }
        }
        public Data data { get; set; }
    }
    
    public class User
    {
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string _id { get; set; }
        public string device_id { get; set; }

        public ulong soft_coins { get; set; }
        public ulong hard_coins { get; set; }
    }

    public class Character
    {
        public string sex { get; set; }
        public int head_type { get; set; }
        public int body_type { get; set; }
        public int pants_type { get; set; }
        public string head_color { get; set; }
        public string body_color { get; set; }
        public string pants_color { get; set; }
        public Skill_Set skill_set { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string _id { get; set; }
        public string user_id { get; set; }
    }
    
    public class Skill_Set
    {
        public string main_skill { get; set; }
        public string attack_skill { get; set; }
        public string defence_skill { get; set; }
        public string passive_skill { get; set; }
        public string utils_skill { get; set; }
    }
    
    public class ServerInfo
    {
        public string address { get; set; }
        public int port { get; set; }
    }
}
