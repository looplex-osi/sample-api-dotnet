using Looplex.DotNet.Core.Application.Abstractions.Commands;
using Looplex.DotNet.Core.Application.Abstractions.DataAccess;
using Looplex.DotNet.Samples.Academic.Domain.Commands;

namespace Looplex.DotNet.Samples.Academic.Infra.Data.Commands
{
    public class UpdateStudentCommandHandler(IDatabaseContext context) : ICommandHandler<UpdateStudentCommand>
    {
        public async Task Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // TODO

            throw new NotImplementedException();
        }
    }
}
