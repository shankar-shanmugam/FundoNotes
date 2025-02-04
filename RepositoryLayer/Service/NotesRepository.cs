using CloudinaryDotNet;
using CommonLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace RepositoryLayer.Service
{
    public class NotesRepository : INotesRepository
    {
        private readonly FundoDBContext _dbContext;
        private readonly IConfiguration configuration;

        public NotesRepository(FundoDBContext dbContext,IConfiguration configuration)
        {
            _dbContext = dbContext;
            this.configuration = configuration;
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
                LastUpdatedAt = DateTime.Now,
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
            catch (Exception ex)
            {
                throw ex;
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
                    notes.LastUpdatedAt = DateTime.Now;
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
            catch (Exception ex)
            { 
                throw ex; 
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
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool IsPinUnPinNotes(int userId, int noteId)
        {
            try
            {
                var note = _dbContext.Notestable.SingleOrDefault(n => n.NotesId == noteId && n.Id == userId);

                if (note == null)
                    return false; 

                note.IsPin = !note.IsPin;
                note.LastUpdatedAt = DateTime.Now;
                _dbContext.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                throw ex;
            }

        }

        public bool ToggleArchive(int userId, int noteId)
        {
            try
            {
                var note = _dbContext.Notestable.FirstOrDefault(n => n.NotesId == noteId && n.Id == userId);

                if (note == null)
                    return false;

                note.IsArchive = !note.IsArchive;
                note.LastUpdatedAt = DateTime.Now;
                _dbContext.SaveChanges();
                return true;
            }
            catch( Exception ex)
            {
                throw ex;
            }
        }

        public bool ToggleTrash(int userId, int noteId)
        {
            try
            {
                var note = _dbContext.Notestable.FirstOrDefault(n => n.NotesId == noteId && n.Id == userId);

                if (note == null)
                    return false;

                note.IsTrash = !note.IsTrash;
                note.LastUpdatedAt = DateTime.Now;
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ImageNotes(string image, int noteId, int userId)
        {
            try
            {
                var note = _dbContext.Notestable.FirstOrDefault(n => n.NotesId == noteId && n.Id == userId);
                if (note == null)
                    return null;

                note.Image = image;
                note.LastUpdatedAt = DateTime.Now;
                _dbContext.SaveChanges();
                return note.Image;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //public string ImageNotes(IFormFile image,int noteId,int userId)
        //{
        //    try
        //    {
        //        var result = _dbContext.Notestable.FirstOrDefault(x => x.NotesId == noteId && x.Id == userId);
        //        if (result != null)
        //        {
        //            Account account = new Account(
        //             configuration["CloudinarySettings:CloudName"],
        //             configuration["CloudinarySettings:ApiKey"],
        //             configuration["CloudinarySettings:ApiSecret"]

        //                );
        //            Cloudinary cloudinary = new Cloudinary(account);
        //            var uploadParams = new CloudinaryDotNet.Actions.ImageUploadParams()
        //            {
        //                File = new FileDescription(image.FileName, image.OpenReadStream()),
        //                PublicId=result.Title
        //            };
        //            var uploadResult = cloudinary.Upload(uploadParams);
        //            if (uploadResult != null)
        //            {
        //                result.LastUpdatedAt = DateTime.Now;
        //                result.Image = uploadResult.Url.ToString();
        //                _dbContext.SaveChanges();
        //                return "Image Uploaded Successfully";
        //            }
        //            else throw new Exception($" Image not upload for the note id:{noteId} ");
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public string BackgroundColor(string color,int noteId,int userId)
        {
            try
            {
                var note = _dbContext.Notestable.FirstOrDefault(n => n.NotesId == noteId && n.Id == userId);
                if (note == null)
                    return null;

                note.Color = color;
                note.LastUpdatedAt = DateTime.Now;
                _dbContext.SaveChanges();
                return note.Color;

            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public string AddRemainder(DateTime remainder,int noteId,int userId)
        {
            try
            {
                var note = _dbContext.Notestable.FirstOrDefault(n => n.NotesId == noteId && n.Id == userId);
                if (note == null) return null;
                note.Remainder = remainder;
                note.LastUpdatedAt = DateTime.Now;
                _dbContext.SaveChanges();
                return remainder.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
