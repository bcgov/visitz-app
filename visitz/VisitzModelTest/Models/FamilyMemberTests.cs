using VisitzModel.Models;

namespace VisitzModelTest.Models;

public class FamilyMemberTests
{
	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("    ")]
	[InlineData("this isn't a date")]
	[InlineData("2020-05-54")]
	[InlineData("2020-13-01")]
	[InlineData("-2020-05-01")]
	public void AgeNullWhenDateOfBirthIsInvalidValue(string dateOfBirth)
	{
		FamilyMember member = new()
		{
			DateOfBirth = dateOfBirth
		};

		Assert.Null(member.Age);
	}

	[Theory]
	[InlineData("1800-01-01")]
	[InlineData("1950-01-01")]
	[InlineData("2000-01-01")]
	[InlineData("2500-01-01")]
	public void AgeIsCorrectByDate(string dateOfBirth)
	{
		var dob = DateTime.Parse(dateOfBirth);
		var dateDiff = DateTime.Now - dob;
		var expectedAge = dateDiff.Days / 365;

		FamilyMember member = new()
		{
			DateOfBirth = dateOfBirth
		};

		Assert.Equal(expectedAge, member.Age);
	}
}
