using System.Collections.ObjectModel;
using System.Dynamic;
using FluentAssertions;
using Looplex.DotNet.Core.Common.Exceptions;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;
using Looplex.DotNet.Samples.Academic.Application.Abstractions.Services;
using Looplex.DotNet.Samples.Academic.Application.Services;
using Looplex.DotNet.Samples.Academic.Domain.Commands;
using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;
using Looplex.DotNet.Samples.Academic.Domain.Queries;
using Looplex.OpenForExtension.Abstractions.Contexts;
using MediatR;
using Newtonsoft.Json;
using NSubstitute;

namespace Looplex.DotNet.Samples.Academic.Application.UnitTests.Services;

[TestClass]
public class StudentServiceTests
{
    private IMediator _mediator = null!;
    private IStudentService _studentService = null!;
    private IContext _context = null!;
    private CancellationToken _cancellationToken;

    [TestInitialize]
    public void Setup()
    {
        _mediator = Substitute.For<IMediator>();
        _studentService = new StudentService(_mediator);
        _context = Substitute.For<IContext>();
        var state = new ExpandoObject();
        _context.State.Returns(state);
        var roles = new Dictionary<string, dynamic>();
        _context.Roles.Returns(roles);
        _cancellationToken = new CancellationToken();
        if (!Schemas.ContainsKey(typeof(Student)))
            Schemas.Add(typeof(Student), File.ReadAllText("Entities/Schemas/Student.1.0.schema.json"));
    }
    
    [TestMethod]
    public async Task GetAllAsync_ShouldReturnPaginatedCollection()
    {
        // Arrange
        _context.State.Pagination = new ExpandoObject();
        _context.State.Pagination.StartIndex = 1;
        _context.State.Pagination.ItemsPerPage = 10;
        var existingStudent = new Student
        {
            Id = null,
            UniqueId = Guid.NewGuid()
        };
        _mediator.Send(Arg.Any<GetStudentsQuery>(), Arg.Any<CancellationToken>())
            .Returns(new ListResponse()
            {
                StartIndex = 1,
                ItemsPerPage = 10,
                Resources = [existingStudent],
                TotalResults = 1,
            });
            
        // Act
        await _studentService.GetAllAsync(_context, _cancellationToken);

        // Assert
        var result = JsonConvert.DeserializeObject<ListResponse>((string)_context.Result!)!;
        Assert.AreEqual(1, result.TotalResults);
        JsonConvert.DeserializeObject<Student>(result.Resources[0].ToString()!).Should().BeEquivalentTo(existingStudent);
    }

    [TestMethod]
    public async Task GetByIdAsync_ShouldThrowEntityNotFoundException_WhenStudentDoesNotExist()
    {
        // Arrange
        _context.State.Id = Guid.NewGuid().ToString();

        // Act & Assert
        await Assert.ThrowsExceptionAsync<EntityNotFoundException>(() => _studentService.GetByIdAsync(_context, _cancellationToken));
    }

    [TestMethod]
    public async Task GetByIdAsync_ShouldReturnStudent_WhenStudentDoesExist()
    {
        // Arrange
        var existingStudent = new Student
        {
            Id = null,
            UniqueId = Guid.NewGuid()
        };
        _context.State.Id = existingStudent.UniqueId.ToString()!;
        _mediator.Send(Arg.Is<GetStudentByIdQuery>(q => q.UniqueId == existingStudent.UniqueId), Arg.Any<CancellationToken>())
            .Returns(existingStudent);

        // Act
        await _studentService.GetByIdAsync(_context, _cancellationToken);
            
        // Assert
        JsonConvert.DeserializeObject<Student>(_context.Result!.ToString()!).Should().BeEquivalentTo(existingStudent);
    }
    
    [TestMethod]
    public async Task CreateAsync_ShouldAddStudentToList()
    {
        // Arrange
        var studentJson = $"{{ \"registrationId\": \"TestStudent1\", \"userId\": 1 }}";
        _context.State.Resource = studentJson;
        
        // Act
        await _studentService.CreateAsync(_context, _cancellationToken);

        // Assert
        await _mediator.Received(1)
            .Send(Arg.Is<CreateStudentCommand>(c => AssertThatCreateStudentIsValid(c.Student)), Arg.Any<CancellationToken>());
    }

    private bool AssertThatCreateStudentIsValid(Student student)
    {
        Assert.AreEqual(student.RegistrationId, "TestStudent1");
        return true;
    }

    [TestMethod]
    public async Task PatchAsync_ShouldThrowException_OperationFailed()
    {
        // Arrange
        var existingUser = new Student()
        {
            Id = 1,
            UniqueId = Guid.NewGuid(),
            RegistrationId = "reg1"
        };
        _mediator.Send(Arg.Any<GetStudentByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(existingUser);
        _context.State.Operations = "[ { \"op\": \"add\", \"path\": \"InvalidPath\", \"value\": \"Updated Reg\" } ]";
        _context.State.Id = existingUser.UniqueId.ToString()!;
        _context.Roles["Student"] = existingUser;
        // Act
        var action = () => _studentService.PatchAsync(_context, _cancellationToken);

        // Assert
        var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(() => action());
        Assert.AreEqual("InvalidPath", ex.ParamName);
    }
    
    [TestMethod]
    public async Task PatchAsync_ShouldApplyOperationsToStudent()
    {
        // Arrange
        var existingUser = new Student()
        {
            Id = 1,
            UniqueId = Guid.NewGuid(),
            RegistrationId = "reg1",
            Projects = new ObservableCollection<Project>
            {
                new()
                {
                    Name = "Project1"
                },
                new()
                {
                    Name = "Project2"
                }
            },
            UserId = 1
        };
        _mediator.Send(Arg.Any<GetStudentByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(existingUser);
        _context.State.Operations = @"[ 
            { ""op"": ""add"", ""path"": ""RegistrationId"", ""value"": ""Updated Reg"" },
            { ""op"": ""add"", ""path"": ""Projects[Name eq \""Project1\""].Name"", ""value"": ""Project3"" },
            { ""op"": ""add"", ""path"": ""Projects"", ""value"": { ""Name"": ""ProjectNew"" } },
            { ""op"": ""remove"", ""path"": ""Projects[Name eq \""Project2\""]"" }
        ]";
        _context.State.Id = existingUser.UniqueId.ToString()!;

        // Act
        await _studentService.PatchAsync(_context, _cancellationToken);

        // Assert
        await _mediator.Received(1).Send(
            Arg.Is<UpdateStudentCommand>(c => AssertThatPatchStudentIsValid(c.Student)),
            Arg.Any<CancellationToken>());
    }

    private bool AssertThatPatchStudentIsValid(Student student)
    {
        Assert.AreEqual("Updated Reg", student.RegistrationId);
        Assert.AreEqual("Project3", student.Projects[0].Name);
        student.ChangedPropertyNotification.ChangedProperties.Should().BeEquivalentTo(["RegistrationId"]);
        student.Projects[0].ChangedPropertyNotification.ChangedProperties.Should().BeEquivalentTo(["Name"]);
        Assert.IsTrue(student.ChangedPropertyNotification.AddedItems.ContainsKey("Projects"));
        student.ChangedPropertyNotification.AddedItems["Projects"].Should().BeEquivalentTo([new Project() { Name = "ProjectNew" }]);
        Assert.IsTrue(student.ChangedPropertyNotification.RemovedItems.ContainsKey("Projects"));
        student.ChangedPropertyNotification.RemovedItems["Projects"].Should().BeEquivalentTo([new Project() { Name = "Project2" }]);
        return true;
    }
    
    [TestMethod]
    public async Task PatchAsync_EntityIsInvalidAfterPatch_ThrowsEntityInvalidException()
    {
        // Arrange
        var existingUser = new Student()
        {
            Id = 1,
            UniqueId = Guid.NewGuid(),
            RegistrationId = "reg1",
            Projects = new ObservableCollection<Project>
            {
                new()
                {
                    Name = "Project1"
                },
                new()
                {
                    Name = "Project2"
                }
            }
        };
        _mediator.Send(Arg.Any<GetStudentByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(existingUser);
        _context.State.Operations = @"[ 
            { ""op"": ""remove"", ""path"": ""RegistrationId"" },
            { ""op"": ""remove"", ""path"": ""Projects[Name eq \""Project2\""].Name"" }
        ]";
        _context.State.Id = existingUser.UniqueId.ToString()!;

        // Act
        var action = () => _studentService.PatchAsync(_context, _cancellationToken);

        // Assert
        var ex = await Assert.ThrowsExceptionAsync<EntityInvalidException>(async () => await action());
        Assert.AreEqual($"One or more validation errors occurred.", ex.Message);
        ex.ErrorMessages.Should().Contain(m => m.IndexOf("registrationId") > 0);
        ex.ErrorMessages.Should().Contain(m => m.IndexOf("projects[1].name") > 0);
        await _mediator.DidNotReceive().Send(
            Arg.Any<UpdateStudentCommand>(),
            Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task DeleteAsync_ShouldThrowEntityNotFoundException_WhenStudentDoesNotExist()
    {
        // Arrange
        _context.State.Id = Guid.NewGuid().ToString();

        // Act & Assert
        await Assert.ThrowsExceptionAsync<EntityNotFoundException>(() => _studentService.DeleteAsync(_context, _cancellationToken));
    }
    
    [TestMethod]
    public async Task DeleteAsync_ShouldRemoveStudentFromList_WhenStudentDoesExist()
    {
        // Arrange
        var existingStudent = new Student
        {
            Id = 1,
            UniqueId = Guid.NewGuid()
        };
        _context.State.Id = existingStudent.UniqueId.ToString()!;
        _mediator.Send(Arg.Is<GetStudentByIdQuery>(q => q.UniqueId == existingStudent.UniqueId), Arg.Any<CancellationToken>())
            .Returns(existingStudent);

        // Act
        await _studentService.DeleteAsync(_context, _cancellationToken);

        // Assert
        await _mediator.Received(1)
            .Send(Arg.Is<DeleteStudentCommand>(c => c.UniqueId == existingStudent.UniqueId), Arg.Any<CancellationToken>());
    }
}