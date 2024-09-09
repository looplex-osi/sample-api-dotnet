using Looplex.DotNet.Core.Common.Exceptions;
using Looplex.DotNet.Samples.Academic.Domain.Commands;
using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;
using Looplex.DotNet.Samples.Academic.Domain.Queries;
using Looplex.DotNet.Samples.Academic.Infra.Data.CommandHandlers;
using Looplex.DotNet.Samples.Academic.Infra.Data.QuerieHandlers;
using Looplex.DotNet.Samples.WebAPI.IntegrationTests;

namespace Looplex.DotNet.Samples.Academic.Infra.IntegrationTests.Data.Queries;

[TestClass]
public class GetStudentByIdQueryHandlerTests : IntegrationTestsBase
{
    [TestMethod]
    public async Task Handle_ValidRequest_GetStudentFromDatabase()
    {
        // Arrange
        var getStudentByIdQueryHandler = new GetStudentByIdQueryHandler(DatabaseContext);
        var createStudentCommandHandler = new CreateStudentCommandHandler(DatabaseContext);
        var createStudentCommand = new CreateStudentCommand
        {   
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
        var getStudentByIdQueryHandler = new GetStudentByIdQueryHandler(DatabaseContext);
        var id = Guid.NewGuid();
        var getStudentByIdQuery = new GetStudentByIdQuery
        {
            UniqueId = id
        };

        using var connection = DatabaseContext.CreateConnection();
        
        // Act
        var action = () => getStudentByIdQueryHandler.Handle(getStudentByIdQuery, CancellationToken.None);

        // Assert
        var ex = await Assert.ThrowsExceptionAsync<EntityNotFoundException>(() => action());
        Assert.AreEqual($"The Student with id {id} was not found.", ex.Message);
    }
}