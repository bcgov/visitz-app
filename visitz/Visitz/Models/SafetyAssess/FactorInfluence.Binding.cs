/*
	Partial class implementation of a Realm + compiled bindings workaround.

	https://github.com/realm/realm-dotnet/issues/2270#issuecomment-786720318
 */

using VisitzModel.Extensions;

namespace Visitz.Models.SafetyAssess;

public partial class FactorInfluence
{
    private const string Binding = "Binding";

    partial void OnPropertyChanged(string propertyName)
    {
        if (!propertyName.EndsWith(Binding))
            RaisePropertyChanged($"{propertyName}{Binding}");
    }

    public bool AgeUptoFiveBinding
	{
		get => IsValid ? AgeUptoFive : default;
		set => this.Commit(() => AgeUptoFive = value);
	}
	
	public bool MedicalMentalDisorderBinding
	{
		get => IsValid ? MedicalMentalDisorder : default;
		set => this.Commit(() => MedicalMentalDisorder = value);
	}
	
	public bool NotReadilyAccessibleBinding
	{
		get => IsValid ? NotReadilyAccessible : default;
		set => this.Commit(() => NotReadilyAccessible = value);
	}
	
	public bool DiminishedMentalBinding
	{
		get => IsValid ? DiminishedMental : default;
		set => this.Commit(() => DiminishedMental = value);
	}
	
	public bool DiminishedPhysicalBinding
	{
		get => IsValid ? DiminishedPhysical : default;
		set => this.Commit(() => DiminishedPhysical = value);
	}
}
