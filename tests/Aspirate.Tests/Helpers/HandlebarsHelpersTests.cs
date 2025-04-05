using Xunit;
using HandlebarsDotNet;
using Aspirate.Shared.Helpers;
using Aspirate.Shared.Models.Aspirate;
using FluentAssertions;

namespace Aspirate.Tests.Helpers;

public class HandlebarsHelpersTests
{
    public HandlebarsHelpersTests()
    {
        // Ensure the helper is registered for testing purposes
        // Note: In a real scenario, this registration happens once at startup.
        // Re-registering here ensures tests are isolated if run in parallel or out of order.
        try
        {
            Handlebars.RegisterHelper("IsProjectName", HandlebarsHelpers.IsProjectNameHelper);
        }
        catch (HandlebarsException ex) when (ex.Message.Contains("already registered"))
        {
            // Ignore if already registered by a previous test run
        }
    }

    [Theory]
    // The Name property is always stored lowercase via SetName. Helper uses Ordinal comparison.
    [InlineData("MyProject", "myproject", "Rendered")] // Should match: context name becomes "myproject", checks against "myproject"
    [InlineData("MyProject", "MyProject", "")] // Should NOT match: context name becomes "myproject", checks against "MyProject"
    [InlineData("MyProject", "anotherproject", "")] // Should NOT match: different string
    [InlineData("MyProject", "", "")] // Empty input
    public void IsProjectNameHelper_ShouldRenderBlock_WhenNameMatches(string resourceName, string projectToCheck, string expectedOutput)
    {
        // Arrange
        var template = Handlebars.Compile($"{{{{#IsProjectName \"{projectToCheck}\"}}}}Rendered{{{{/IsProjectName}}}}");
        var data = new KubernetesDeploymentData().SetName(resourceName);

        // Act
        var result = template(data);

        // Assert
        result.Should().Be(expectedOutput);
    }

    [Theory]
    // The Name property is always stored lowercase via SetName. Helper uses Ordinal comparison.
    [InlineData("MyProject", "MyProject", "ElseRendered")] // Should NOT match: context name becomes "myproject", checks against "MyProject"
    [InlineData("MyProject", "anotherproject", "ElseRendered")] // Should NOT match: different string
    [InlineData("MyProject", "", "ElseRendered")] // Should NOT match: empty input
    public void IsProjectNameHelper_ShouldRenderInverse_WhenNameDoesNotMatch(string resourceName, string projectToCheck, string expectedOutput)
    {
        // Arrange
        var template = Handlebars.Compile($"{{{{#IsProjectName \"{projectToCheck}\"}}}}Rendered{{{{else}}}}ElseRendered{{{{/IsProjectName}}}}");
        var data = new KubernetesDeploymentData().SetName(resourceName);

        // Act
        var result = template(data);

        // Assert
        result.Should().Be(expectedOutput);
    }

    [Theory]
    [InlineData("SomeProject")]
    [InlineData("")]
    public void IsProjectNameHelper_ShouldRenderInverse_WhenContextNameIsNull(string projectToCheck)
    {
        // Arrange
        var template = Handlebars.Compile($"{{{{#IsProjectName \"{projectToCheck}\"}}}}Rendered{{{{else}}}}ElseRendered{{{{/IsProjectName}}}}");
        var data = new KubernetesDeploymentData(); // Name is null

        // Act
        var result = template(data);

        // Assert
        result.Should().Be("ElseRendered");
    }

    [Fact]
    public void IsProjectNameHelper_ShouldRenderInverse_WhenContextIsNotCorrectType()
    {
        // Arrange
        var template = Handlebars.Compile("{{#IsProjectName \"AnyProject\"}}Rendered{{else}}ElseRendered{{/IsProjectName}}");
        var data = new { SomeOtherProperty = "value" }; // Incorrect context type

        // Act
        var result = template(data);

        // Assert
        result.Should().Be("ElseRendered");
    }

    [Fact]
    public void IsProjectNameHelper_ShouldRenderInverse_WhenArgumentCountIsNotOne()
    {
        // Arrange
        var template = Handlebars.Compile("{{#IsProjectName \"Arg1\" \"Arg2\"}}Rendered{{else}}ElseRendered{{/IsProjectName}}");
        var data = new KubernetesDeploymentData().SetName("Arg1");

        // Act
        var result = template(data);

        // Assert
        result.Should().Be("ElseRendered");
    }

    [Fact]
    public void IsProjectNameHelper_ShouldRenderInverse_WhenArgumentTypeIsNotString()
    {
        // Arrange
        var template = Handlebars.Compile("{{#IsProjectName 123}}Rendered{{else}}ElseRendered{{/IsProjectName}}");
        var data = new KubernetesDeploymentData().SetName("SomeName");

        // Act
        var result = template(data);

        // Assert
        result.Should().Be("ElseRendered");
    }
}
