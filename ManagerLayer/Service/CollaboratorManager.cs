using ManagerLayer.Interface;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerLayer.Service
{
    public class CollaboratorManager : ICollaboratorManager
    {
        private readonly ICollaboratorRepository collaboratorRepository;

        public CollaboratorManager(ICollaboratorRepository collaboratorRepository)
        {
            this.collaboratorRepository = collaboratorRepository;
        }
        public CollaboratorEntity CreateCollab(int noteId, string email)=>
        
           collaboratorRepository.CreateCollab(noteId, email);
        

        public bool DeleteCollab(int collabId)=>
        
            collaboratorRepository.DeleteCollab(collabId);
        

        public List<CollaboratorEntity> RetrieveCollab(int noteId)=>
        
            collaboratorRepository.RetrieveCollab(noteId);
        
    }
}
