namespace BookNow.Application.Models
{
  
    public sealed record MediaFile(string FileName, byte[] Content, string ContentType);
}