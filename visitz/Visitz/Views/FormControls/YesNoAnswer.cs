namespace Visitz.Views.FormControls;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "ApiDesign",
    "SS039:An enum should specify a default value",
    Justification = "Only yes or no"
)]
public enum YesNoAnswer
{
    No = 0,
    Yes = 1,
}
