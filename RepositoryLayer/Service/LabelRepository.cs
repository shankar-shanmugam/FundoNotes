using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepositoryLayer.Service
{
    public class LabelRepository : ILabelRepository
    {
        private readonly FundoDBContext _dbContext;

        public LabelRepository(FundoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public LabelEntity CreateLabel(int noteId, int userId, string name)
        {
            try
            {
                var notes = _dbContext.Notestable.FirstOrDefault(notes => notes.NotesId == noteId);


                if (notes == null)
                {
                    return null;
                }
                LabelEntity label = new LabelEntity()
                {
                    LabelId = noteId,
                    UserId = userId,
                    LabelName = name
                };
                _dbContext.Labeltable.Add(label);
                _dbContext.SaveChanges();
                return label;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public LabelEntity RetrieveLabel(int labelId,int userId)
        {
            try
            {
                var result = _dbContext.Labeltable.FirstOrDefault(x => x.LabelId == labelId && x.UserId==userId);
                if (result != null)
                {
                    return result;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public bool DeleteLabel(int labelId,int userId)
        {
            try
            {
                var result = _dbContext.Labeltable.FirstOrDefault(x => x.LabelId == labelId && x.UserId==userId);

                _dbContext.Labeltable.Remove(result);

                _dbContext.SaveChanges();
                return true;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public LabelEntity EditLabel(int labelId, string labelName,int userId)
        {
            try
            {
                var labelEntity = _dbContext.Labeltable.FirstOrDefault(e => e.LabelId ==labelId && e.UserId==userId );

                if (labelEntity != null)
                {
                    labelEntity.LabelName = labelName;

                   _dbContext.SaveChanges();
                    return labelEntity;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }

}

