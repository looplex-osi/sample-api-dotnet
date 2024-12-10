using Looplex.DotNet.Core.Common.Exceptions;
using Looplex.DotNet.Middlewares.ScimV2.Domain;
using Looplex.DotNet.Samples.Academic.Domain.Commands;
using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;
using Looplex.DotNet.Samples.Academic.Infra.Data.CommandHandlers;
using Looplex.DotNet.Samples.IntegrationTests;
using NSubstitute;

namespace Looplex.DotNet.Samples.Academic.Infra.IntegrationTests.Data.Commands;

[TestClass]
public class DeleteStudentCommandHandlerTest : IntegrationTestsBase
{
    private IScimV2Context _context = null!;
    
    [TestInitialize]
    public void Setup()
    {
        _context = Substitute.For<IScimV2Context>();
        _context.GetSqlDatabaseService().Returns(SqlDatabaseService);
    }

    [TestMethod]
    public async Task Handle_ValidRequest_DeletesStudentFromDatabase()
    {
        // Arrange
        var deleteStudentCommandHandler = new DeleteStudentCommandHandler();
        var createStudentCommandHandler = new CreateStudentCommandHandler();
        var createStudentCommand = new CreateStudentCommand
        {
            Context =_context,
            Student = new Student
            {
                RegistrationId = Guid.NewGuid().ToString(),
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

        var deleteStudentCommand = new DeleteStudentCommand
        {
            Context =_context,
            UniqueId = createStudentCommand.Student.UniqueId!.Value
        };
        
        // Act
        await deleteStudentCommandHandler.Handle(deleteStudentCommand, CancellationToken.None);

        // Assert
        var count = await SqlDatabaseService.QueryFirstOrDefaultAsync<int>($"select top 1 1 from students where uuid = @UniqueId", new { deleteStudentCommand.UniqueId });
        Assert.AreEqual(0, count);
    }
    
    [TestMethod]
    public async Task Handle_StudentDoesntExist_EntityNotFoundExceptionIsThrown()
    {
        // Arrange
        var deleteStudentCommandHandler = new DeleteStudentCommandHandler();
        var id = Guid.NewGuid();
        var deleteStudentCommand = new DeleteStudentCommand
        {
            Context =_context,
            UniqueId = id
        };
        
        // Act
        var action = () => deleteStudentCommandHandler.Handle(deleteStudentCommand, CancellationToken.None);

        // Assert
        var ex = await Assert.ThrowsExceptionAsync<EntityNotFoundException>(() => action());
        Assert.AreEqual($"The Student with id {id} was not found.", ex.Message);
    }
}