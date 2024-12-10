using Looplex.DotNet.Core.Application.ExtensionMethods;
using Looplex.DotNet.Core.Common.Exceptions;
using Looplex.DotNet.Middlewares.ScimV2.Domain;
using Looplex.DotNet.Samples.Academic.Domain.Commands;
using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;
using Looplex.DotNet.Samples.Academic.Domain.Queries;
using Looplex.DotNet.Samples.Academic.Infra.Data.CommandHandlers;
using Looplex.DotNet.Samples.Academic.Infra.Data.QuerieHandlers;
using Looplex.DotNet.Samples.IntegrationTests;
using NSubstitute;

namespace Looplex.DotNet.Samples.Academic.Infra.IntegrationTests.Data.Commands;

[TestClass]
public class UpdateStudentCommandHandlerTest : IntegrationTestsBase
{
    private IScimV2Context _context = null!;
    
    [TestInitialize]
    public void Setup()
    {
        _context = Substitute.For<IScimV2Context>();
        _context.GetSqlDatabaseService().Returns(SqlDatabaseService);
    }

    [TestMethod]
    public async Task Handle_ValidRequest_UpdatesStudentFromDatabase()
    {
        // Arrange
        var updateStudentCommandHandler = new UpdateStudentCommandHandler();
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
        var updateStudentCommand = new UpdateStudentCommand
        {
            Context =_context,
            Student = createStudentCommand.Student.WithObservableProxy()
        };
        updateStudentCommand.Student.RegistrationId = "updated reg";

        // Act
        await updateStudentCommandHandler.Handle(updateStudentCommand, CancellationToken.None);

        // Assert
        var getStudentByIdQueryHandler = new GetStudentByIdQueryHandler();
        var getStudentByIdQuery = new GetStudentByIdQuery
        {
            Context =_context,
            UniqueId = createStudentCommand.Student.UniqueId!.Value
        };
        var student = await getStudentByIdQueryHandler.Handle(getStudentByIdQuery, CancellationToken.None);
        Assert.AreEqual("updated reg", student.RegistrationId);
    }
    
    [TestMethod]
    public async Task Handle_StudentDoesntExist_EntityNotFoundExceptionIsThrown()
    {
        // Arrange
        var updateStudentCommandHandler = new UpdateStudentCommandHandler();
        var updateStudentCommand = new UpdateStudentCommand
        {
            Context =_context,
            Student = new Student
            {
                UniqueId = Guid.NewGuid()
            }
        };
        updateStudentCommand.Student.ChangedPropertyNotification.ChangedProperties.Add("RegistrationId");

        // Act
        var action = () => updateStudentCommandHandler.Handle(updateStudentCommand, CancellationToken.None);

        // Assert
        var ex = await Assert.ThrowsExceptionAsync<EntityNotFoundException>(() => action());
        Assert.AreEqual($"The Student with id {updateStudentCommand.Student.UniqueId} was not found.", ex.Message);
    }
}