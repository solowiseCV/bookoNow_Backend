using BookNow.Application.Common.Exceptions;
using BookNow.Application.Features.Appointments.Handler.Commands.UpdateAppointmentHandler;
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
    public class UpdateAppointmentHandlerTests
    {
        [Fact]
        public async Task Handle_AdminUpdatesAppointment()
        {
            var appointment = new Appointment(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(2), "initial");
            var repoMock = new Mock<IAppointmentRepository>();
            repoMock.Setup(r => r.GetByIdAsync(appointment.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.Appointments).Returns(repoMock.Object);
            unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var currentUserMock = new Mock<ICurrentUserService>();
            currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Admin.ToString());

            var handler = new UpdateAppointmentCommandHandler(unitOfWorkMock.Object, currentUserMock.Object);

            await handler.Handle(new UpdateAppointmentCommand(appointment.Id, DateTime.UtcNow.AddDays(3), "updated"), CancellationToken.None);

            // verify the entity was updated and saved
            Assert.Equal(DateTime.UtcNow.AddDays(3).Date, appointment.AppointmentAt.Date);
            Assert.Equal("updated", appointment.IssueDescription);
            unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_NonAdminThrowsUnauthorized()
        {
            var repoMock = new Mock<IAppointmentRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.Appointments).Returns(repoMock.Object);

            var currentUserMock = new Mock<ICurrentUserService>();
            currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Client.ToString());

            var handler = new UpdateAppointmentCommandHandler(unitOfWorkMock.Object, currentUserMock.Object);

            await Assert.ThrowsAsync<ForbiddenAccessException>(async () =>
                await handler.Handle(new UpdateAppointmentCommand(Guid.NewGuid(), DateTime.UtcNow, "x"), CancellationToken.None));
        }
    }
}
