# Tests #
there is the description of why write the tests and how we write them

## Goal ##
There are two goals that can be archived by implementing unit tests
1. Keep the solution stable all the time, whenever or whatever has been changed
2. Make the code self-documented or self-descriptive by setting meaningful  names to the tests

## Code conventions ##
The rule of thumb here is "easy to read". Feel free to propose better convention if you come up with

+ The test class should be created one-or-many per each class

*Example:*

GIVEN Actual class name  "RevenueCalculationService"

THEN The test class would be **RevenueCalculationService**Tests


If the actual class has a lot of things to implement we can split test class to many

*Example:*

GIVEN Actual class name  "RevenueCalculationService"

THEN The test class would be 
 
RevenueCalculationService**TradeCalcs**Tests
 
RevenueCalculationService**PositionCals**Tests

+ The test class should start with the name of the actual class and end with the word "Tests"

+ The instance of the class that we are going to test we call **SUT** - **S**ystem **U**nder **T**est

+ The SUT should be created for each test separately ( to archive one of the main principals - Isolation)

+ We use lower_case_underscore_separated_name_to_name_the_tests

+ Test methods should start with *should_* or *should_NOT_

*Examples:*
```
public void should_return_4_if_parameters_are_2_and_2
public void should_NOT_return_value_if_something_is_disabled
```
+ We use pattern AAA - **A**rrange **A**ct **A**ssert

*Example:*
```
public void should_return_4_if_parameters_are_2_and_2
{
	// Arrange
	var a = 2;
	var b = 2;

	// Act
	var result = _sut.Multiply(a,b);

	// assert
	result.Should().Be(4);
}
```

+ For assertions, we use the FluentAssertions framework (https://fluentassertions.com/)

+ For mockups, we use the NSubstitute framework (http://nsubstitute.github.io/)

## Unit Tests ##
###Principals###
1. Isolation - tests SHOULD **NOT** INFLUENCE ON EACH OTHER ANYHOW!!!
2. Test only one block - the only one piece of logic should be tested within one test, not more!
3. Immutability - test should return the same result no matter how many times it has been run

*Examples:*

There should be **3** test methods to test it!!!
```
pub void Cals()
{
	// 1 test
	if (Something1)
	{
	}
   	// 2 test
	if (Something2)
	{
	}

	// 3 test
	if (Something3)
	{
	}
}
```

### Purpose ###
By this type of tests, we want to test the only one small piece of business logic. **All the integrations** should be mocked up

*Examples:*
```
public int Cals()
{
	// _ service1.GetValue() - this implementation should be mocked up with the value that we expect
	var value = _ service1.GetValue();

	return value * 3;
}
```

## Integration tests ##
###Principals###
1. Isolation - tests SHOULD **NOT** INFLUENCE ON EACH OTHER ANYHOW!!!
2. Immutability - test should return the same result no matter how many times it has been run. (Sometimes it's not possible to archive by this type of tests)

### Purpose ###
By this type of tests, we want to test *integration* between classes, components or systems.

