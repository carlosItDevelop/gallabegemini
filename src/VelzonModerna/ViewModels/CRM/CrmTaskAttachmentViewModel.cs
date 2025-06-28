using System;
using System.ComponentModel.DataAnnotations;

namespace VelzonModerna.ViewModels.CRM
{
    public class CrmTaskAttachmentViewModel
    {
        public Guid Id { get; set; }

        [Required]
        public Guid CrmTaskId { get; set; }

        [Required(ErrorMessage = "O nome do arquivo é obrigatório.")]
        public string FileName { get; set; }

        [Required(ErrorMessage = "A URL do arquivo é obrigatória.")]
        public string FileUrl { get; set; }

        public int FileSize { get; set; } // Em bytes

        public string MimeType { get; set; }

        public string? UploadedBy { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}