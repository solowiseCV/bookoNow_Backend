//using BookNow.Application.Features.Appointments.Handler.Commands.CreateAppointmentHandler;
//using BookNow.Application.Features.Appointments.Request.Commands;
//using BookNow.Application.Interfaces.Authentication;
//using BookNow.Application.Interfaces.Persistence;
//using BookNow.Application.Interfaces.Services;
//using BookNow.Application.Models;
//using BookNow.Domain.Entities;
//using BookNow.Domain.Enums;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Threading;
//using System.Threading.Tasks;
//using Xunit;

//namespace BookNow.Test.Features.Appointments
//{
//    public class CreateAppointmentHandlerTests
//    {
//        [Fact]
//        public async Task Handle_WithMediaFiles_SavesFilesAndAddsAttachments()
//        {
//            // arrange
//            var userId = Guid.NewGuid();
//            var userProfile = new UserProfile(userId, UserRole.Client);

//            var unitOfWorkMock = new Mock<IUnitOfWork>();
//            var appointmentRepoMock = new Mock<IAppointmentRepository>();
//            unitOfWorkMock.SetupGet(u => u.UserProfiles).Returns(Mock.Of<IUserProfileRepository>(r => r.GetByIdentityIdAsync(userId, It.IsAny<CancellationToken>()) == Task.FromResult(userProfile)));
//            unitOfWorkMock.SetupGet(u => u.Appointments).Returns(appointmentRepoMock.Object);
//            unitOfWorkMock.SetupGet(u => u.Workshops).Returns(Mock.Of<IWorkshopRepository>());
//            unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

//            var currentUserMock = new Mock<ICurrentUserService>();
//            currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Client.ToString());
//            currentUserMock.SetupGet(c => c.UserId).Returns(userId.ToString());

//            var mediaServiceMock = new Mock<IMediaStorageService>();
//            mediaServiceMock.Setup(m => m.SaveAsync(It.IsAny<MediaFile>(), It.IsAny<CancellationToken>()))
//                .ReturnsAsync("/uploads/appointments/test.jpg");

//            var notificationServiceMock = new Mock<INotificationService>();

//            var handler = new CreateAppointmentCommandHandler(unitOfWorkMock.Object, currentUserMock.Object, mediaServiceMock.Object);

//            var mediaFile = new MediaFile("test.jpg", new byte[] {1,2,3}, "image/jpeg");

//            var command = new CreateAppointmentCommand(Guid.NewGuid(), DateTime.UtcNow.AddDays(1), "Issue", new List<MediaFile> { mediaFile });

//            // act
//            var result = await handler.Handle(command, CancellationToken.None);

//            // assert
//            Assert.NotEqual(Guid.Empty, result);
//            mediaServiceMock.Verify(m => m.SaveAsync(It.IsAny<MediaFile>(), It.IsAny<CancellationToken>()), Times.Once);
//            appointmentRepoMock.Verify(r => r.AddAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()), Times.Once);
//        }
//    }
//}
