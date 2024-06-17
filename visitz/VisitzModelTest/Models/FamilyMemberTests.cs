using VisitzModel.Models;
using VisitzModel.Sorting;

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

	const string Mother = "Mother";
	const string Father = "Father";
	const string Uncle = "Uncle";
	const string Cousin = "Cousin";
	const string OlderAge = "2019-01-01 06:30:31";
	const string YoungerAge = "2021-01-01 06:30:31";
	const string YoungestAge = "2023-01-01 06:30:31";
	const string NoAge = "";

	const int ExpectedYoungerAgePosition = 0;
	const int ExpectedMotherPosition = 1;
	const int ExpectedFatherPosition = 2;
	const int ExpectedOlderAgePosition = 3;
	const int ExpectedYoungestAgePosition = 4;
	const int ExpectedUnclePosition = 5;
	const int ExpectedCousinPosition = 6;
	const int ExpectedNoAgePosition = 7;

	private static List<FamilyMember> MakeFamilyList()
	{
		return
		[
			new()
			{
				SubjectChild = true,
				DateOfBirth = YoungestAge,
			},
			new()
			{
				SubjectChild = true,
				DateOfBirth = OlderAge,
			},
			new()
			{
				KeyPlayer = "Y",
				SubjectChild = true,
				DateOfBirth = YoungerAge,
			},
			new()
			{
				Relationship = Mother,
				ParentCaregiver = true,
				DateOfBirth = OlderAge,
			},
			new()
			{
				DateOfBirth = NoAge,
			},
			new()
			{
				Relationship = Father,
				ParentCaregiver = true,
				DateOfBirth = YoungerAge,
			},
			new()
			{
				Relationship = Uncle,
				DateOfBirth = OlderAge,
			},
			new()
			{
				Relationship = Cousin,
				DateOfBirth = YoungerAge,
			}
		];
	}

	private static IOrderedEnumerable<FamilyMember> MakeOrderedFamilyEnumerable()
	{
		return MakeFamilyList().Order(new FamilyMemberComparer());
	}

	[Fact]
	public void KeyPlayerSortOrderIsCorrect()
	{
		var family = MakeOrderedFamilyEnumerable();

		Assert.True(family.ElementAt(ExpectedYoungerAgePosition).IsKeyPlayer);
	}

	[Fact]
	public void MotherSortOrderIsCorrect()
	{
		var family = MakeOrderedFamilyEnumerable();

		Assert.Equal(Mother, family.ElementAt(ExpectedMotherPosition).Relationship);
	}

	[Fact]
	public void FatherSortOrderIsCorrect()
	{
		var family = MakeOrderedFamilyEnumerable();

		Assert.Equal(Father, family.ElementAt(ExpectedFatherPosition).Relationship);
	}

	[Fact]
	public void UncleSortOrderIsCorrect()
	{
		var family = MakeOrderedFamilyEnumerable();

		Assert.Equal(Uncle, family.ElementAt(ExpectedUnclePosition).Relationship);
	}

	[Fact]
	public void CousinSortOrderIsCorrect()
	{
		var family = MakeOrderedFamilyEnumerable();

		Assert.Equal(Cousin, family.ElementAt(ExpectedCousinPosition).Relationship);
	}

	[Fact]
	public void ChildOlderSortOrderIsCorrect()
	{
		var family = MakeOrderedFamilyEnumerable();

		Assert.Equal(OlderAge, family.ElementAt(ExpectedOlderAgePosition).DateOfBirth);
	}

	[Fact]
	public void ChildYoungestSortOrderIsCorrect()
	{
		var family = MakeOrderedFamilyEnumerable();

		Assert.Equal(YoungestAge, family.ElementAt(ExpectedYoungestAgePosition).DateOfBirth);
	}

	[Fact]
	public void NoAgeSortOrderIsCorrect()
	{
		var family = MakeOrderedFamilyEnumerable();

		Assert.Empty(family.ElementAt(ExpectedNoAgePosition).DateOfBirth);
	}
}
