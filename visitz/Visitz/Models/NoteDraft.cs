using System;
using Realms;

namespace Visitz.Models
{
	public partial class NoteDraft : IRealmObject
    {
        [PrimaryKey]
        public string CaseIncidentAndCreatedDateID { get; set; }

        public string Draft { get; set; }

        public static string MakeId(string caseIncidentNumber, string createdDate)
        {
            return $"{caseIncidentNumber}-{createdDate}";
        }
    }
}

