using hestiapi.Models;

namespace hestia.Models.BOs
{
    public class CaseloadCaseItem : CaseloadBaseItem
    {
        public string CreatedDate { get; set; }
        public string KeyPlayerCellPhone { get; set; }
        public string KeyPlayerEmail { get; set; }
        public string KeyPlayerHomePhone { get; set; }

        public CaseloadCaseItem(CaseloadCaseEntity caseloadCase) : base(caseloadCase)
        {
            CreatedDate = caseloadCase.CreatedDate;
            KeyPlayerCellPhone = caseloadCase.KeyPlayerCellPhone;
            KeyPlayerEmail = caseloadCase.KeyPlayerEmail;
            KeyPlayerHomePhone = caseloadCase.KeyPlayerHomePhone;
        }
    }
}
