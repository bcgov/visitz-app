using VisitzModel.Models.SafetyAssess;

namespace VisitzModelTest.Models.SafetyAssess;

public class SafetyAssessmentTests
{
    static List<SafetyAssessment> Incident10Assessments =>
        [new() { Id = "1", IncidentNumber = "10" }, new() { Id = "2", IncidentNumber = "10" }];

    static List<SafetyAssessment> Incident20Assessments => [new() { Id = "3", IncidentNumber = "20" }];

    [Fact]
    public async Task AssessmentsSynchronizeInsertionCorrectly()
    {
        var realm = await TestingUtilities.MakeRealm<SafetyAssessmentTests>();

        await SafetyAssessment.SynchronizeAsync(realm, "10", Incident10Assessments);
        await SafetyAssessment.SynchronizeAsync(realm, "20", Incident20Assessments);

        Assert.Equal(3, realm.All<SafetyAssessment>().Count());
    }

    [Fact]
    public async Task AssessmentsSynchronizeDeletionCorrectly()
    {
        var realm = await TestingUtilities.MakeRealm<SafetyAssessmentTests>();

        // Load initial set
        await SafetyAssessment.SynchronizeAsync(realm, "10", Incident10Assessments);
        await SafetyAssessment.SynchronizeAsync(realm, "20", Incident20Assessments);

        // Sync again with one less to delete it from the realm
        await SafetyAssessment.SynchronizeAsync(realm, "10", Incident10Assessments.Skip(1));

        Assert.Equal(2, realm.All<SafetyAssessment>().Count());
    }
}
