using BookNow.Application.Common.Exceptions;
using BookNow.Application.Features.Appointments.Handler.Queries.GetAppointmentsByWorkshopHandler;
using BookNow.Application.Features.Appointments.Request.Queries;
using BookNow.Application.Interfaces.Authentication;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Entities;
using BookNow.Domain.Enums;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BookNow.Test.Features.Appointments
{
    public class GetAppointmentsByWorkshopHandlerTests
    {
        [Fact]
        public async Task Handle_AdminCanRetrieve()
        {
            var workshopId = Guid.NewGuid();
            var appointments = new List<Appointment> { new Appointment(Guid.NewGuid(), workshopId, DateTime.UtcNow.AddDays(1), "test") };

            var repoMock = new Mock<IAppointmentRepository>();
            repoMock.Setup(r => r.GetByWorkshopAsync(workshopId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointments);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.Appointments).Returns(repoMock.Object);

            var currentUserMock = new Mock<ICurrentUserService>();
            currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Admin.ToString());

            var mapperMock = new Mock<AutoMapper.IMapper>();
            mapperMock.Setup(m => m.Map<IReadOnlyList<BookNow.Application.DTOs.Appointment.AppointmentDto>>(appointments))
                      .Returns(new List<BookNow.Application.DTOs.Appointment.AppointmentDto>());

            var handler = new GetAppointmentsByWorkshopQueryHandler(unitOfWorkMock.Object, currentUserMock.Object, mapperMock.Object);

            var result = await handler.Handle(new GetAppointmentsByWorkshopQuery(workshopId), CancellationToken.None);

            Assert.NotNull(result);
            repoMock.Verify(r => r.GetByWorkshopAsync(workshopId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_NonOwnerThrowsUnauthorized()
        {
            var workshopId = Guid.NewGuid();

            var repoMock = new Mock<IAppointmentRepository>();
            var workshopRepoMock = new Mock<IWorkshopRepository>();

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.Appointments).Returns(repoMock.Object);
            unitOfWorkMock.SetupGet(u => u.Workshops).Returns(workshopRepoMock.Object);

            var currentUserMock = new Mock<ICurrentUserService>();
            currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Mechanic.ToString());
            currentUserMock.SetupGet(c => c.UserId).Returns(Guid.NewGuid().ToString());

            // profile lookup returns profile with different id
            var profile = new UserProfile(Guid.NewGuid(), UserRole.Mechanic);
            var userRepoMock = new Mock<IUserProfileRepository>();
            userRepoMock.Setup(r => r.GetByIdentityIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(profile);
            unitOfWorkMock.SetupGet(u => u.UserProfiles).Returns(userRepoMock.Object);

            // IsOwnedByMechanic returns false
            workshopRepoMock.Setup(w => w.IsOwnedByMechanicAsync(workshopId, profile.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var mapperMock = new Mock<AutoMapper.IMapper>();

            var handler = new GetAppointmentsByWorkshopQueryHandler(unitOfWorkMock.Object, currentUserMock.Object, mapperMock.Object);

            await Assert.ThrowsAsync<ForbiddenAccessException>(async () =>
                await handler.Handle(new GetAppointmentsByWorkshopQuery(workshopId), CancellationToken.None));
        }
    }
}
