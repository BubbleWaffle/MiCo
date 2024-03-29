﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MiCo.Models
{
    public class Threads
    {
        [Key] public int id { get; set; }

        [ForeignKey("author")]
        public int id_author { get; set; }
        public virtual Users author { get; set; } = null!;

        public int? id_reply { get; set; }
        public virtual Threads? reply { get; set; }

        public int? id_OG_thread { get; set; }
        public virtual Threads? OG_thread { get; set; }

        [Required]
        public string title { get; set; } = null!;

        [Required]
        public string description { get; set; } = null!;

        [Required]
        public DateTimeOffset creation_date { get; set; }

        public bool deleted { get; set; } = false;

        public virtual List<Images>? thread_images { get; set; }

        public virtual List<ThreadTags>? thread_tags { get; set; }
    }
}
