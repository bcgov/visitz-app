using Visitz.FontIcons;
using Visitz.Resources.Localization;
using VisitzModel.Models;
using VisitzModel.Models.InPersonVisits;

namespace Visitz.Views.Todo;

public class TodoVisitsDisplayItem(PersonVisit visit, CaseloadItem caseloadItem)
{
    public PersonVisit TodoItem { get; set; } = visit;
    public string CaseloadDisplayName { get; set; } = caseloadItem?.DisplayName;
    public bool IsOverdue { get; set; } = TodoVisitsDisplayItem.FindIfOverdue(visit);

    public static bool FindIfOverdue(PersonVisit visit)
    {
        return DateTimeOffset.Now.Date > visit.DueDate;
    }
}
