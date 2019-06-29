using System.Collections.Generic;

namespace UTFBox_Server.Models
{
    public class User
    {
        public string Name;
        public string UserName;
        public string Password;
        public List<Revision> SharedFolderFiles;
    }
}