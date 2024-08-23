using System.Dynamic;
using FluentAssertions;
using Looplex.DotNet.Core.Common.Exceptions;
using Looplex.DotNet.Core.Domain;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Schemas;
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
    }

    [TestMethod]
    public async Task GetAllAsync_ShouldReturnPaginatedCollection()
    {
        // Arrange
        _context.State.Pagination = new ExpandoObject();
        _context.State.Pagination.Page = 1;
        _context.State.Pagination.PerPage = 10;
        var existingStudent = new Student
        {
            Id = "student1"
        };
        _mediator.Send(Arg.Any<GetStudentsQuery>(), Arg.Any<CancellationToken>())
            .Returns(new PaginatedCollection()
            {
                Page = 1,
                PerPage = 10,
                Records = [existingStudent],
                TotalCount = 1,
            });
            
        // Act
        await _studentService.GetAllAsync(_context, _cancellationToken);

        // Assert
        var result = JsonConvert.DeserializeObject<PaginatedCollection>((string)_context.Result!)!;
        Assert.AreEqual(1, result.TotalCount);
        JsonConvert.DeserializeObject<Student>(result.Records[0].ToString()!).Should().BeEquivalentTo(existingStudent);
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
            Id = Guid.NewGuid().ToString()
        };
        _context.State.Id = existingStudent.Id;
        _mediator.Send(Arg.Is<GetStudentByIdQuery>(q => q.Id == existingStudent.Id ), Arg.Any<CancellationToken>())
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
        var id = Guid.NewGuid().ToString();
        var studentJson = $"{{ \"Id\": \"{id}\", \"RegistrationId\": \"TestStudent1\" }}";
        _context.State.Resource = studentJson;
        Schema.Add<Student>("{}");
        
        // Act
        await _studentService.CreateAsync(_context, _cancellationToken);

        // Assert
        Assert.AreEqual(id, _context.Result);
        await _mediator.Received(1)
            .Send(Arg.Is<CreateStudentCommand>(c => AssertThatStudentIsValid(c.Student, id)), Arg.Any<CancellationToken>());
    }

    private bool AssertThatStudentIsValid(Student student, string expectedId)
    {
        Assert.AreEqual(student.Id, expectedId);
        Assert.AreEqual(student.RegistrationId, "TestStudent1");
        return true;
    }

    [TestMethod]
    public void PatchAsync_ShouldThrowException_OperationFailed()
    {
        // TODO
    }
    
    [TestMethod]
    public void PatchAsync_ShouldApplyOperationsToStudent()
    {
        // TODO
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
            Id = Guid.NewGuid().ToString()
        };
        _context.State.Id = existingStudent.Id;
        _mediator.Send(Arg.Is<GetStudentByIdQuery>(q => q.Id == existingStudent.Id ), Arg.Any<CancellationToken>())
            .Returns(existingStudent);

        // Act
        await _studentService.DeleteAsync(_context, _cancellationToken);

        // Assert
        await _mediator.Received(1)
            .Send(Arg.Is<DeleteStudentCommand>(c => c.Id == existingStudent.Id), Arg.Any<CancellationToken>());
    }
}