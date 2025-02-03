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

        public bool IsPinUnPinNotes(int userId, int noteId);
        bool ToggleTrash(int userId, int noteId);
        bool ToggleArchive(int userId, int noteId);

        public string AddRemainder(DateTime remainder, int noteId, int userId);

        public string BackgroundColor(string color, int noteId, int userId);

        public string ImageNotes(string image, int noteId, int userId);
    }
}
