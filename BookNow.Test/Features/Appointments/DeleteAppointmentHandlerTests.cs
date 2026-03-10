using BookNow.Application.Common.Exceptions;
using BookNow.Application.Features.Appointments.Handler.Commands.DeleteAppointmentHandler;
using BookNow.Application.Features.Appointments.Request.Commands;
using BookNow.Application.Interfaces.Authentication;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Entities;
using BookNow.Domain.Enums;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BookNow.Test.Features.Appointments
{
    public class DeleteAppointmentHandlerTests
    {
        [Fact]
        public async Task Handle_AdminDeletesAppointment()
        {
            var appointment = new Appointment(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(1), "issue");
            var repoMock = new Mock<IAppointmentRepository>();
            repoMock.Setup(r => r.GetByIdAsync(appointment.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.Appointments).Returns(repoMock.Object);
            unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var currentUserMock = new Mock<ICurrentUserService>();
            currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Admin.ToString());

            var handler = new DeleteAppointmentCommandHandler(unitOfWorkMock.Object, currentUserMock.Object);

            await handler.Handle(new DeleteAppointmentCommand(appointment.Id), CancellationToken.None);

            repoMock.Verify(r => r.Remove(appointment), Times.Once);
            unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_NonAdminThrowsUnauthorized()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var currentUserMock = new Mock<ICurrentUserService>();
            currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Mechanic.ToString());

            var handler = new DeleteAppointmentCommandHandler(unitOfWorkMock.Object, currentUserMock.Object);

            await Assert.ThrowsAsync<ForbiddenAccessException>(async () =>
                await handler.Handle(new DeleteAppointmentCommand(Guid.NewGuid()), CancellationToken.None));
        }
    }
}
