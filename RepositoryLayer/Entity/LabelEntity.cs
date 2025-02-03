using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace RepositoryLayer.Entity
{
    public class LabelEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LabelId { get; set; }

        public string LabelName { get; set; }

        [ForeignKey("LabelNote")]
        public int NoteId { get; set; }

        [ForeignKey("LabelUser")]
        public int UserId { get; set; }
        [JsonIgnore]
        public virtual User LabelUser { get; set; }

        [JsonIgnore]
        public virtual NotesEntity LabelNote { get; set; }
    }
}
