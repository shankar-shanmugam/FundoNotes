using ManagerLayer.Interface;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerLayer.Service
{
    public class LabelManager : ILabelManager
    {
        private readonly ILabelRepository _repository;

        public LabelManager(ILabelRepository repository)
        {
            this._repository = repository;
        }
        public LabelEntity CreateLabel(int noteId, int userId, string name)=>
            _repository.CreateLabel(noteId, userId, name);

        public bool DeleteLabel(int labelId,int userId)=>
            _repository.DeleteLabel(labelId,userId);

        public LabelEntity EditLabel(int labelId, string labelName,int userId)=>
            _repository.EditLabel(labelId,labelName,userId);

        public LabelEntity RetrieveLabel(int labelId,int userId)=>
            _repository.RetrieveLabel(labelId, userId);
        
    }
}
