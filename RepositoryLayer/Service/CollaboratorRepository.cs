﻿using Microsoft.Extensions.Logging;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepositoryLayer.Service
{
    public class CollaboratorRepository : ICollaboratorRepository
    {
        private readonly FundoDBContext _dBContext;
        private readonly ILogger<CollaboratorRepository> _logger;

        public CollaboratorRepository(FundoDBContext dbContext, ILogger<CollaboratorRepository> logger)
        {
            _dBContext = dbContext;
            _logger = logger;
        }

        public CollaboratorEntity CreateCollab(int notesId, string email)
        {
            try
            {
                var noteResult = _dBContext.Notestable.FirstOrDefault(x => x.NotesId == notesId);
                var emailResult = _dBContext.User.FirstOrDefault(x => x.Email == email);

                if (noteResult == null || emailResult == null)
                    return null;

                //  Check  collaboration already exists
                var existingCollab = _dBContext.Collaboratortable
                    .FirstOrDefault(c => c.NotesId == notesId && c.UserId == emailResult.Id);

                if (existingCollab != null)
                    return existingCollab; //  Return existing collaborator (Prevent duplicates)

                var collaboratorEntity = new CollaboratorEntity
                {
                    Email = emailResult.Email,
                    NotesId = noteResult.NotesId,
                    UserId = emailResult.Id
                };

                _dBContext.Add(collaboratorEntity);
                _dBContext.SaveChanges();
                return collaboratorEntity;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CreateCollab: {ex.Message}");
                throw;
            }
        }

        //  Retrieve Collaborators
        public List<CollaboratorEntity> RetrieveCollab(int noteId)
        {
            try
            {
                return _dBContext.Collaboratortable
                    .Where(x => x.NotesId == noteId)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in RetrieveCollab: {ex.Message}");
                throw;
            }
        }

        public bool DeleteCollab(int collabId)
        {
            try
            {
                var result = _dBContext.Collaboratortable.FirstOrDefault(x => x.CollaboratorId == collabId);

                if (result == null)
                    return false; 

                _dBContext.Collaboratortable.Remove(result);
                _dBContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in DeleteCollab: {ex.Message}");
                throw;
            }
        }
    }

}
