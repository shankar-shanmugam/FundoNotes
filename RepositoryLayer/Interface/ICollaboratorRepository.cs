using RepositoryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Interface
{
    public interface ICollaboratorRepository
    {
        public CollaboratorEntity CreateCollab(int notesId, string email);

        public List<CollaboratorEntity> RetrieveCollab(int noteId);

        public bool DeleteCollab(int collabId);
    }
}
