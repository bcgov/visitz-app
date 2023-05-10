using hestiapi.Models;

namespace hestia.Models.BOs
{
    public class CaseloadIncidentItem : CaseloadBaseItem
    {
        public string DateReported { get; set; }

        public CaseloadIncidentItem(CaseloadIncidentEntity caseloadEntity) : base(caseloadEntity)
        {
            DateReported = caseloadEntity.DateReported;
        }
    }
}
