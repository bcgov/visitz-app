using VisitzModel.Models.People;
using VisitzModel.Models.SafetyAssess;

namespace VisitzModelTest.Models;

public class BindingsTests
{
    // SafetyAssessment uses manually-coded bindings. Eventually all manual binding properties should be migrated to
    // use generated ones instead.
    [Fact]
    public async Task ManualBindingsNotifyCorrectly()
    {
        var realm = await TestingUtilities.MakeRealm<BindingsTests>();

        SafetyAssessment sa = new();
        string expectedProperty = nameof(sa.CreatedDateBinding);
        List<string> notifiedProperties = [];
        sa.PropertyChanged += (sender, args) => notifiedProperties.Add(args.PropertyName ?? "");

        // Update any Binding property
        sa.CreatedDateBinding = DateTimeOffset.UtcNow;

        Assert.Contains(expectedProperty, notifiedProperties);
    }

    // ContactEducation uses generated bindings.
    [Fact]
    public async Task GeneratedBindingsNotifyCorrectly()
    {
        var realm = await TestingUtilities.MakeRealm<BindingsTests>();

        ContactEducation edu = new();
        string expectedProperty = nameof(edu.CreatedByNameBinding);
        List<string> notifiedProperties = [];
        edu.PropertyChanged += (sender, args) => notifiedProperties.Add(args.PropertyName ?? "");

        // Update any Binding property
        edu.CreatedByNameBinding = "name";

        Assert.Contains(expectedProperty, notifiedProperties);
    }
}
