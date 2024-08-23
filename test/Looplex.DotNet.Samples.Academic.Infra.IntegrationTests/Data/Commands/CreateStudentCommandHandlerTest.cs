using Looplex.DotNet.Samples.Academic.Domain.Commands;
using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;
using Looplex.DotNet.Samples.Academic.Domain.Queries;
using Looplex.DotNet.Samples.Academic.Infra.Data.Commands;
using Looplex.DotNet.Samples.Academic.Infra.Data.Queries;
using Looplex.DotNet.Samples.WebAPI.IntegrationTests;

namespace Looplex.DotNet.Samples.Academic.Infra.IntegrationTests.Data.Commands;

[TestClass]
public class CreateStudentCommandHandlerTest : IntegrationTestsBase
{
    [TestMethod]
    public async Task Handle_ValidRequest_InsertsStudentIntoDatabase()
    {
        // Arrange
        var createStudentCommandHandler = new CreateStudentCommandHandler(DatabaseContext);
        var id = Guid.NewGuid().ToString();
        var createStudentCommand = new CreateStudentCommand
        {   
            Student = new Student
            {
                RegistrationId = Guid.NewGuid().ToString(),
                Id = id
            }
        };

        // Act
        await createStudentCommandHandler.Handle(createStudentCommand, CancellationToken.None);

        // Assert
        var getStudentByIdQueryHandler = new GetStudentByIdQueryHandler(DatabaseContext);
        var getStudentByIdQuery = new GetStudentByIdQuery
        {
            Id = id
        };
        var student = await getStudentByIdQueryHandler.Handle(getStudentByIdQuery, CancellationToken.None);
        Assert.IsNotNull(student);
        Assert.AreEqual(createStudentCommand.Student.RegistrationId, student.RegistrationId);
        Assert.AreEqual(createStudentCommand.Student.UserId, student.UserId);
    }
}