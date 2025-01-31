using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLayer.Models
{
    public class NotesModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Color { get; set; }

        public DateTime Remainder { get; set; }

        public string Image { get; set; }

        public bool IsArchive { get; set; }

        public bool IsTrash { get; set; }

        public bool IsPin { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
