using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralLabSolutions.Domain.Entities.Audit;
using GeneralLabSolutions.Domain.Entities.Base;

namespace GeneralLabSolutions.Domain.Entities.CRM
{
    public class CrmTaskAttachment : EntityAudit
    {
        public CrmTaskAttachment() { }

        public CrmTaskAttachment(Guid crmTaskId, string fileName, string fileUrl, int fileSize, string mimeType, string? uploadedBy)
        {
            CrmTaskId = crmTaskId;
            FileName = fileName;
            FileUrl = fileUrl;
            FileSize = fileSize;
            MimeType = mimeType;
            UploadedBy = uploadedBy;
        }

        public string FileName { get; private set; }
        public string FileUrl { get; private set; }
        public int FileSize { get; private set; } // Em bytes
        public string MimeType { get; private set; }
        public string? UploadedBy { get; private set; }

        // Relacionamento com CrmTask
        public Guid CrmTaskId { get; set; }
        public virtual CrmTask CrmTask { get; set; }
    }
}
