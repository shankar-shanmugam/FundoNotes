using CommonLayer.Models;
using RepositoryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerLayer.Interface
{
    public interface INotesManager
    {
        public NotesEntity CreateNotes(NotesModel model, int userId);

        public List<NotesEntity> GetAllNotes(int userId);

        public NotesEntity GetNotes(int userId, int noteId);
        public NotesEntity UpdateNotes(int userId, int noteId, NotesModel notesModel);

        public bool DeleteNote(int userId, int noteId);
    }
}
