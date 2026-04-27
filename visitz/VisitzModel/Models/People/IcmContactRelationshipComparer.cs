namespace VisitzModel.Models.People;

public class IcmContactRelationshipComparer : IComparer<IcmContact>
{
    public int Compare(IcmContact? x, IcmContact? y)
    {
        if (x == null || y == null)
            return 0;
        if (x.SortPositionAsc == y.SortPositionAsc)
            return CompareAgeDesc(y.Age, x.Age);
        else
            return x.SortPositionAsc.CompareTo(y.SortPositionAsc);
    }

    private static int CompareAgeDesc(int? yAge, int? xAge)
    {
        return (yAge ?? int.MinValue).CompareTo(xAge ?? int.MinValue);
    }
}
