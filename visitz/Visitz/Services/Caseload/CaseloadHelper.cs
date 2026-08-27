using System.Net;
using VisitzApi.ErrorHandling;
using VisitzApi.Models.Base;
using VisitzApi.Models.Caseload;

namespace Visitz.Services.Caseload;

internal static class CaseloadHelper
{
    static bool HasCode<T>(SectionJson<T> section, params HttpStatusCode[] codes)
        where T : AssignableRecordJson
    {
        foreach (var code in codes)
            if (section.Status == (int)code)
                return true;

        return false;
    }

    public static bool CanSynchronize<T>(SectionJson<T> section, List<Exception> invalidOps)
        where T : AssignableRecordJson
    {
        if (HasCode(section, HttpStatusCode.OK, HttpStatusCode.NoContent))
            return true;
        else if (HasCode(section, HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden))
        {
            invalidOps.Add(new VisitzApiException(HttpStatusCode.Forbidden, section.GetFullDisplayError()));
        }
        else
            invalidOps.Add(new InvalidOperationException(section.GetFullDisplayError()));

        return false;
    }
}
