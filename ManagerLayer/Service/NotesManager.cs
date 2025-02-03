using CommonLayer.Models;
using ManagerLayer.Interface;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerLayer.Service
{
    public class NotesManager : INotesManager
    {
        private readonly INotesRepository _notesRepository;

        public NotesManager(INotesRepository notesRepository)
        {
            this._notesRepository = notesRepository;   
        }

        public NotesEntity CreateNotes(NotesModel model, int userId)=>
        
            _notesRepository.CreateNotes(model, userId);

        public List<NotesEntity> GetAllNotes(int userId)=>

            _notesRepository.GetAllNotes(userId);

        public NotesEntity GetNotes(int userId, int noteId)=>
        
            _notesRepository.GetNotes(userId, noteId);

        public NotesEntity UpdateNotes(int userId, int noteId, NotesModel notesModel)=>

            _notesRepository.UpdateNotes(userId,noteId,notesModel);

        public bool DeleteNote(int userId, int noteId)=>

            _notesRepository.DeleteNote(userId,noteId);

        public bool IsPinUnPinNotes(int userId, int noteId)=>

            _notesRepository.IsPinUnPinNotes((int)userId, noteId);

        public bool ToggleTrash(int userId, int noteId)=>

            _notesRepository.ToggleTrash(userId,noteId);
       

        public bool ToggleArchive(int userId, int noteId)=>
            _notesRepository.ToggleArchive(userId,noteId);

        public string AddRemainder(DateTime remainder, int noteId, int userId)=>
            _notesRepository.AddRemainder(remainder,noteId,userId);
       

        public string BackgroundColor(string color, int noteId, int userId)=>
            _notesRepository.BackgroundColor(color,noteId,userId);
       
        public string ImageNotes(string image, int noteId, int userId)=>
            _notesRepository.ImageNotes(image,noteId,userId);
        
    }
}
