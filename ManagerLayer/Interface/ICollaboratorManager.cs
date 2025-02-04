using RepositoryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerLayer.Interface
{
    public interface ICollaboratorManager
    {
      public  CollaboratorEntity CreateCollab(int noteId, string email);
       public bool DeleteCollab(int collabId);
      public  List<CollaboratorEntity> RetrieveCollab(int noteId);
    }
}
