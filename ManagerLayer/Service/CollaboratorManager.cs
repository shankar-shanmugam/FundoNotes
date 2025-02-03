using ManagerLayer.Interface;
using RepositoryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerLayer.Service
{
    public class CollaboratorManager : ICollaboratorManager
    {
        private readonly ICollaboratorManager collaboratorManager;

        public CollaboratorManager(ICollaboratorManager collaboratorManager)
        {
            this.collaboratorManager = collaboratorManager;
        }
        public CollaboratorEntity CreateCollab(int noteId, string email)=>
        
           collaboratorManager.CreateCollab(noteId, email);
        

        public bool DeleteCollab(int collabId, int userId)=>
        
            collaboratorManager.DeleteCollab(collabId, userId);
        

        public List<CollaboratorEntity> RetrieveCollab(int noteId)=>
        
            collaboratorManager.RetrieveCollab(noteId);
        
    }
}
