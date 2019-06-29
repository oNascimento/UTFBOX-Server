using System.Collections.Generic;
using System.IO;
using System.Linq;
using UTFBox_Server.Models;

namespace UTFBox_Server.Repositories
{
    public class FileRepository
    {
        private List<User> _users;
        private List<Revision> _revisions;

        public List<User> GetAllUsers()
        {
            return _users;
        }

        public List<Revision> GetAllRevisionsByUser(User user)
        {
            var revisions = new List<Revision>();

            foreach (var item in _revisions)
            {
                if(item.userName == user.Name)
                    revisions.Add(item);
            }

            user = _users.Where(u => u.UserName == user.UserName).FirstOrDefault();
            revisions.AddRange(user.SharedFolderFiles);

            return revisions;
        }

        public void ShareRevision(User receiver, Revision revision){
            if(receiver.SharedFolderFiles.Equals(null))
                receiver.SharedFolderFiles = new List<Revision>();
            
            receiver = _users.Where(u => u.UserName == receiver.UserName).FirstOrDefault();
            revision = _revisions.Where( r => r.fileName == revision.fileName).FirstOrDefault();

            receiver.SharedFolderFiles.Add(revision);
        }
    }
}