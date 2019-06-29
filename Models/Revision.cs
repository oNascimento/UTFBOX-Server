using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace UTFBox_Server.Models
{
    public class Revision
    {
        public string fileName;
        public string userName;
        public byte[] fileData = null;
        public bool isPublic = false;
        public DateTime LastModificationDate;

        
        public static string GetFileServerPath(Revision revision){
            const string _repository = "./Repository";
            string path;

            if(revision.isPublic)
                path = Path.Combine(_repository, revision.fileName);
            else
                path = Path.Combine(_repository, revision.userName, revision.fileName);

            return path;
        }
    }
}