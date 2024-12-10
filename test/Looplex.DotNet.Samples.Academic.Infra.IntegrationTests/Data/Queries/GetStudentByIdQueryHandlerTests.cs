using Looplex.DotNet.Core.Common.Exceptions;
using Looplex.DotNet.Middlewares.ScimV2.Domain;
using Looplex.DotNet.Samples.Academic.Domain.Commands;
using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;
using Looplex.DotNet.Samples.Academic.Domain.Queries;
using Looplex.DotNet.Samples.Academic.Infra.Data.CommandHandlers;
using Looplex.DotNet.Samples.Academic.Infra.Data.QuerieHandlers;
using Looplex.DotNet.Samples.IntegrationTests;
using NSubstitute;

namespace Looplex.DotNet.Samples.Academic.Infra.IntegrationTests.Data.Queries;

[TestClass]
public class GetStudentByIdQueryHandlerTests : IntegrationTestsBase
{
    private IScimV2Context _context = null!;
    
    [TestInitialize]
    public void Setup()
    {
        _context = Substitute.For<IScimV2Context>();
        _context.GetSqlDatabaseService().Returns(SqlDatabaseService);
    }
    
    [TestMethod]
    public async Task Handle_ValidRequest_GetStudentFromDatabase()
    {
        // Arrange
        var getStudentByIdQueryHandler = new GetStudentByIdQueryHandler();
        var createStudentCommandHandler = new CreateStudentCommandHandler();
        var createStudentCommand = new CreateStudentCommand
        {   
            Context = _context,
            Student = new Student
            {
                RegistrationId = "test",
                Projects = new List<Project>()
                {
                    new()
                    {
                        Name = "Project1",
                    },
                    new()
                    {
                        Name = "Project2",
                    },
                }
            }
        };
        await createStudentCommandHandler.Handle(createStudentCommand, CancellationToken.None);
        var getStudentByIdQuery = new GetStudentByIdQuery
        {
            Context = _context,
            UniqueId = createStudentCommand.Student.UniqueId!.Value
        };
        
        // Act
        var student = await getStudentByIdQueryHandler.Handle(getStudentByIdQuery, CancellationToken.None);

        // Assert
        Assert.IsNotNull(student);
        Assert.IsNotNull(student.Id);
        Assert.IsNotNull(student.UniqueId);
        Assert.AreEqual("test", student.RegistrationId);
    }
    
    [TestMethod]
    public async Task Handle_StudentDoesntExist_EntityNotFoundExceptionIsThrown()
    {
        // Arrange
        var getStudentByIdQueryHandler = new GetStudentByIdQueryHandler();
        var id = Guid.NewGuid();
        var getStudentByIdQuery = new GetStudentByIdQuery
        {
            Context = _context,
            UniqueId = id
        };
        
        // Act
        var action = () => getStudentByIdQueryHandler.Handle(getStudentByIdQuery, CancellationToken.None);

        // Assert
        var ex = await Assert.ThrowsExceptionAsync<EntityNotFoundException>(() => action());
        Assert.AreEqual($"The Student with id {id} was not found.", ex.Message);
    }
}