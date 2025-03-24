using Shouldly;
using VideoScreenshot.Domain.Results;
using VideoScreenshot.Tests.Fakes;

namespace VideoScreenshot.Tests.UnitTests.Domain.Results;

public sealed class OperationResultTest
{
    [Fact]
    public void Sucess_Given_OperationWasSuccessful_When_CreatingOperationResult_Then_ResultIsSuccessful()
    {
        // Arrange
        var message = FakerBr.Faker.Random.Words(5);
        
        // Act
        var operationResult = OperationResult.Success(message);
        
        // Assert
        operationResult.Succeeded.ShouldBeTrue();
        operationResult.Message.ShouldBe(message);
    }
    
    [Fact]
    public void Fail_Given_OperationWasFailure_When_CreatingOperationResult_Then_ResultIsSuccessfulFalse()
    {
        // Arrange
        var message = FakerBr.Faker.Random.Words(5);
        
        // Act
        var operationResult = OperationResult.Fail(message);
        
        // Assert
        operationResult.Succeeded.ShouldBeFalse();
        operationResult.Message.ShouldBe(message);
    }
    
    [Fact]
    public void Default_Given_DefaultSuccessResult_ShouldBeSuccessful()
    {
        // Arrange
        // Act
        var operationResult = OperationResult.DefaultSuccessResult;
        
        // Assert
        operationResult.Succeeded.ShouldBeTrue();
        operationResult.Message.ShouldBeNull();
    }
    
    [Fact]
    public void Success_Given_OperationWasSuccessful_When_CreatingOperationResultWithGeneric_Then_ResultIsSuccessful()
    {
        // Arrange
        var message = FakerBr.Faker.Random.Words(5);
        var value = FakerBr.Faker.Random.Int();
        
        // Act
        var operationResult = OperationResult<int>.Success(value, message);
        
        // Assert
        operationResult.Succeeded.ShouldBeTrue();
        operationResult.Message.ShouldBe(message);
        operationResult.Value.ShouldBe(value);
    }
    
    [Fact]
    public void Fail_Given_OperationWasFailure_When_CreatingOperationResultWithGeneric_Then_ResultIsSuccessfulFalse()
    {
        // Arrange
        var message = FakerBr.Faker.Random.Words(5);
        
        // Act
        var operationResult = OperationResult<int>.Fail(message);
        
        // Assert
        operationResult.Succeeded.ShouldBeFalse();
        operationResult.Message.ShouldBe(message);
        operationResult.Value.ShouldBe(default);
    }
}