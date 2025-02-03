using RepositoryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Interface
{
    public interface ILabelRepository
    {
        public LabelEntity CreateLabel(int noteId, int userId, string name);

        public LabelEntity EditLabel(int labelId, string labelName,int userid);

        public bool DeleteLabel(int labelId,int userId);

        public LabelEntity RetrieveLabel(int labelId,int userId);
    }
}
