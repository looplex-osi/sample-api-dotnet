using System.Dynamic;
using Looplex.DotNet.Samples.Academic.Domain.Queries;
using Looplex.DotNet.Samples.Academic.Infra.Data.Queries;
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

        using var connection = DatabaseContext.CreateConnection();
        var count = await connection.QueryFirstOrDefaultAsync<int>("select count(1) from students");

        // Act
        var students = await getStudentsQueryHandler.Handle(getStudentsQuery, CancellationToken.None);

        // Assert
        Assert.AreEqual(count, students.TotalCount);
        Assert.AreEqual(1, students.Page);
        Assert.AreEqual(2, students.PerPage);
        Assert.IsTrue(students.Records.Count() >= 0);
        Assert.IsTrue(students.Records.Count() <= 2);
    }
}