using Application.Common.Interfaces;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NetArchTest.Rules;
using Xunit;

namespace ProjectApproval.ArchitectureTests;

public class CleanArchitectureTests
{
    private static readonly System.Reflection.Assembly DomainAssembly = typeof(User).Assembly;
    private static readonly System.Reflection.Assembly ApplicationAssembly = typeof(ICurrentUserService).Assembly;
    private static readonly System.Reflection.Assembly InfrastructureAssembly = typeof(ProjectApprovalDbContext).Assembly;
    private static readonly System.Reflection.Assembly ApiAssembly = typeof(Program).Assembly;

    [Fact]
    public void Domain_Should_Not_HaveDependencyOn_Application_Infrastructure_Or_Api()
    {
        // Arrange
        var otherProjects = new[]
        {
            "Application",
            "Infrastructure",
            "ProjectApproval.Api"
        };

        // Act
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue("Domain layer must be completely independent from all outer layers");
    }

    [Fact]
    public void Application_Should_Not_HaveDependencyOn_Infrastructure_Or_Api()
    {
        // Arrange
        var prohibitedProjects = new[]
        {
            "Infrastructure",
            "ProjectApproval.Api"
        };

        // Act
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(prohibitedProjects)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue("Application layer must not depend on Infrastructure or Web Api");
    }

    [Fact]
    public void Infrastructure_Should_Not_HaveDependencyOn_Api()
    {
        // Act
        var result = Types.InAssembly(InfrastructureAssembly)
            .ShouldNot()
            .HaveDependencyOn("ProjectApproval.Api")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue("Infrastructure layer must not depend on Web Api");
    }

    [Fact]
    public void Handlers_Should_Have_NameEndingWithHandler()
    {
        // Act
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(IRequestHandler<,>))
            .Should()
            .HaveNameEndingWith("Handler")
            .Or()
            .HaveNameEndingWith("Handlers")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue("All MediatR request handlers must follow the *Handler naming convention");
    }

    [Fact]
    public void Controllers_Should_InheritFrom_ControllerBase()
    {
        // Act
        var result = Types.InAssembly(ApiAssembly)
            .That()
            .HaveNameEndingWith("Controller")
            .Should()
            .Inherit(typeof(ControllerBase))
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue("All controllers in Api assembly must inherit from ControllerBase");
    }

    [Fact]
    public void Controllers_Should_Have_NameEndingWithController()
    {
        // Act
        var result = Types.InAssembly(ApiAssembly)
            .That()
            .Inherit(typeof(ControllerBase))
            .Should()
            .HaveNameEndingWith("Controller")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue("All classes inheriting from ControllerBase must have the *Controller suffix");
    }
}
