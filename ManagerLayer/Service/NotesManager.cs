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
    }
}
