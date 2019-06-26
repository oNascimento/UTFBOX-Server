namespace UTFBox_Server.Models
{
    public class User
    {
        public string Name;
        public string UserName;
        public string Password;
    
        public User(string name, string userName, string password)
        {
            Name = name;
            UserName = userName;
            Password = password;
        }
        
    }
}