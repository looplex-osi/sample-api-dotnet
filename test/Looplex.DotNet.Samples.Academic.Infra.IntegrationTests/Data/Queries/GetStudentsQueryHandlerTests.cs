using System.Dynamic;
using Looplex.DotNet.Samples.Academic.Domain.Commands;
using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;
using Looplex.DotNet.Samples.Academic.Domain.Queries;
using Looplex.DotNet.Samples.Academic.Infra.Data.CommandHandlers;
using Looplex.DotNet.Samples.Academic.Infra.Data.QuerieHandlers;
using Looplex.DotNet.Samples.WebAPI.IntegrationTests;
using Looplex.OpenForExtension.Abstractions.Contexts;
using NSubstitute;

namespace Looplex.DotNet.Samples.Academic.Infra.IntegrationTests.Data.Queries;

[TestClass]
public class GetStudentsQueryHandlerTests : IntegrationTestsBase
{
    [TestMethod]
    public async Task Handle_ValidRequest_GetStudentsFromDatabase()
    {
        // Arrange
        var createStudentCommandHandler = new CreateStudentCommandHandler(DatabaseContext);
        var context = Substitute.For<IContext>();
        dynamic state = new ExpandoObject();
        context.State.Returns(state);
        dynamic pagination = new ExpandoObject();
        pagination.Page = 1;
        pagination.PerPage = 2;
        state.Pagination = pagination;
        var getStudentsQueryHandler = new GetStudentsQueryHandler(DatabaseContext);
        var getStudentsQuery = new GetStudentsQuery
        {
            Context = context
        };
        var createStudentCommand = new CreateStudentCommand
        {   
            Student = new Student
            {
                RegistrationId = "test1",
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
        createStudentCommand = new CreateStudentCommand
        {   
            Student = new Student
            {
                RegistrationId = "test2",
                Projects = new List<Project>()
                {
                    new()
                    {
                        Name = "Project3",
                    },
                    new()
                    {
                        Name = "Project4",
                    },
                }
            }
        };
        await createStudentCommandHandler.Handle(createStudentCommand, CancellationToken.None);
        createStudentCommand = new CreateStudentCommand
        {   
            Student = new Student
            {
                RegistrationId = "test3",
                Projects = new List<Project>()
                {
                    new()
                    {
                        Name = "Project5",
                    },
                    new()
                    {
                        Name = "Project6",
                    },
                }
            }
        };
        await createStudentCommandHandler.Handle(createStudentCommand, CancellationToken.None);
        using var connection = DatabaseContext.CreateConnection();
        var count = await connection.QueryFirstOrDefaultAsync<int>("select count(1) from students");

        // Act
        var students = await getStudentsQueryHandler.Handle(getStudentsQuery, CancellationToken.None);

        // Assert
        Assert.AreEqual(count, students.TotalResults);
        Assert.AreEqual(1, students.StartIndex);
        Assert.AreEqual(2, students.ItemsPerPage);
        Assert.IsTrue(students.Resources.Count() >= 0);
        Assert.IsTrue(students.Resources.Count() <= 2);
        
        Assert.AreEqual("test3", ((Student)students.Resources[0]).RegistrationId);
        Assert.AreEqual("Project6", ((Student)students.Resources[0]).Projects[0].Name);
        Assert.AreEqual("Project5", ((Student)students.Resources[0]).Projects[1].Name);
        Assert.AreEqual("test2", ((Student)students.Resources[1]).RegistrationId);
        Assert.AreEqual("Project4", ((Student)students.Resources[1]).Projects[0].Name);
        Assert.AreEqual("Project3", ((Student)students.Resources[1]).Projects[1].Name);
    }
}