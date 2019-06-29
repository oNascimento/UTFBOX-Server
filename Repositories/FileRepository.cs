using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UTFBox_Server.Models;

namespace UTFBox_Server.Repositories
{
    public class FileRepository
    {
        private List<User> _users;
        private List<Revision> _revisions;

        public FileRepository()
        {
            _users = new List<User>();
            _revisions = new List<Revision>();
        }

        public List<User> GetAllUsers()
        {
            return _users;
        }

        public List<Revision> GetAllRevisionsByUser(User user)
        {
            var revisions = new List<Revision>();

            foreach (var item in _revisions)
            {
                if(item.userName == user.Name || item.isPublic)
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

        public async Task AddToRepository(Revision revision)
        {
            if(_revisions.Where(r => r.fileName == revision.fileName && r.userName == revision.userName).Any()){
                var rev = _revisions.Where(r =>
                             r.fileName == revision.fileName && r.userName == revision.userName).FirstOrDefault();
                _revisions.Remove(rev);
                _revisions.Add(revision);
            }else
                _revisions.Add(revision);
        }

        public void AddUser(User user)
        {
            _users.Add(user);
        }

        public async Task RemoveOfRepository(Revision revision)
        {
            var rev = _revisions.Where(r => r.fileName == revision.fileName && r.userName == revision.userName).FirstOrDefault();
            _revisions.Remove(rev);
        }
    }
}