using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLayer.Models
{
    public class CollaborationMessage
    {
        public string NoteTitle { get; set; }
        public string CollaboratorEmail { get; set; }
        public string OwnerName { get; set; }
        public int NoteId { get; set; }
    
    }
}
