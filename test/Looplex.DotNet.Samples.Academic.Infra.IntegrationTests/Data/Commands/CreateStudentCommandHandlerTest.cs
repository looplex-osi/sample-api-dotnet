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
public class CreateStudentCommandHandlerTest : IntegrationTestsBase
{
    private IScimV2Context _context = null!;
    
    [TestInitialize]
    public void Setup()
    {
        _context = Substitute.For<IScimV2Context>();
        _context.GetSqlDatabaseService().Returns(SqlDatabaseService);
    }

    [TestMethod]
    public async Task Handle_ValidRequest_InsertsStudentIntoDatabase()
    {
        // Arrange
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

        // Act
        await createStudentCommandHandler.Handle(createStudentCommand, CancellationToken.None);

        // Assert
        var getStudentByIdQueryHandler = new GetStudentByIdQueryHandler();
        var getStudentByIdQuery = new GetStudentByIdQuery
        {
            Context =_context,
            UniqueId = createStudentCommand.Student.UniqueId!.Value
        };
        var student = await getStudentByIdQueryHandler.Handle(getStudentByIdQuery, CancellationToken.None);
        Assert.IsNotNull(student);
        Assert.AreEqual(createStudentCommand.Student.RegistrationId, student.RegistrationId);
        Assert.AreEqual(createStudentCommand.Student.UserId, student.UserId);
        Assert.AreEqual(2, student.Projects.Count);
        Assert.AreEqual("Project1", student.Projects[0].Name);
        Assert.AreEqual("Project2", student.Projects[1].Name);
    }
}