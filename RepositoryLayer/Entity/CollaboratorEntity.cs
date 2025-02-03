using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RepositoryLayer.Entity
{
    public class CollaboratorEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  
        public int CollaboratorId { get; set; }

        public string Email { get; set; }

        [ForeignKey("CollaboratorNotes")]
        public int NotesId { get; set; }

        [ForeignKey("CollaboratorUser")]
        public int UserId { get; set; }

        public virtual User CollaboratorUser { get; set; }

        public virtual NotesEntity CollaboratorNotes { get; set; }
    }
}
