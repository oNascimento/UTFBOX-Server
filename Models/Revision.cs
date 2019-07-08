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
        public string fileData;
        public bool isPublic;
        public string LastModificationDate;


        public static string GetFileServerPath(Revision revision){
            const string _repository = @"C:\Repository";
            string path;

            if(revision.isPublic)
                path = _repository;
            else
                path = Path.Combine(_repository, revision.userName);

            // Verifica se o diretório Existe
            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = Path.Combine(path, revision.fileName);

            return path;
        }
    }
}