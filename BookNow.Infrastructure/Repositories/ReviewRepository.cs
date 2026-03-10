using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Entities;
using BookNow.Infrastructure.Data;

namespace BookNow.Infrastructure.Repositories;

public class ReviewRepository(BookNowDbContext context)
    : GenericRepository<Review>(context), IReviewRepository
{
}
