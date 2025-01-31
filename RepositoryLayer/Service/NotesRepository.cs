using CommonLayer.Models;
using Microsoft.Extensions.Configuration;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepositoryLayer.Service
{
    public class NotesRepository : INotesRepository
    {
        private readonly FundoDBContext _dbContext;

        public NotesRepository(FundoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public NotesEntity CreateNotes(NotesModel model, int userId)
        {
            NotesEntity notes = new NotesEntity()
            {
                Id = userId,
                Color = model.Color,
                CreatedAt = model.CreatedAt,
                Description = model.Description,
                Image = model.Image,
                IsArchive = model.IsArchive,
                IsPin = model.IsPin,
                IsTrash = model.IsTrash,
                LastUpdatedAt = model.LastUpdatedAt,
                Remainder = model.Remainder,
                Title = model.Title,
            };
            _dbContext.Notestable.Add(notes);
            _dbContext.SaveChanges();
            return notes;

        }
        public List<NotesEntity> GetAllNotes(int userId)
        {
            try
            {
                var notes = _dbContext.Notestable.Where(notes => notes.Id == userId).ToList();
                // Check if the list is empty
                if (!notes.Any())
                {
                    return null;
                }

                return notes;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public NotesEntity GetNotes(int userId, int noteId)
        {
            try
            {
                var notes = _dbContext.Notestable.FirstOrDefault(n => n.NotesId == noteId && n.Id == userId);

                if (notes != null)
                {
                    return notes;
                }
                else
                {
                   return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public NotesEntity UpdateNotes(int userId, int noteId,NotesModel notesModel)
        {
            try
            {
                var notes = _dbContext.Notestable.FirstOrDefault(n => n.NotesId == noteId && n.Id == userId);
                if (notes != null)
                {
                    notes.Remainder = notesModel.Remainder;
                    notes.Title = notesModel.Title;
                    notes.LastUpdatedAt = notesModel.LastUpdatedAt;
                    notes.Color = notesModel.Color;
                    notes.Description = notesModel.Description;
                    notes.Image = notesModel.Image;
                    notes.IsArchive = notesModel.IsArchive;
                    notes.IsPin = notesModel.IsPin;
                    notes.IsTrash = notesModel.IsTrash;
                    notes.CreatedAt = notesModel.CreatedAt;

                //    _dbContext.Update(notes);  No need because entity is already tracked ,saveChanges will update it in DB
                    _dbContext.SaveChanges();

                    return notes;
                }
                else
                {
                    return null ;
                }
            }
            catch (Exception)
            { 
                throw; 
            }

        }
        public bool DeleteNote(int userId, int noteId)
        {
            try
            {
                var notes = _dbContext.Notestable.FirstOrDefault(n => n.NotesId == noteId && n.Id == userId);
                if(notes != null)
                {
                    _dbContext.Notestable.Remove(notes);
                    _dbContext.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
