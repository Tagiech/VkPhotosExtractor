namespace VkPhotosExtractor.Application.Auth.Models;

public enum Sex
{
    Unknown = 0,
    Female = 1,
    Male = 2
}

public class UserInfo
{
    public Guid Id { get; private set; }
    public int ExternalId { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string PhotoUrl { get; private set; }
    public Sex Sex { get; private set; }
    public DateTime Birthday { get; private set; }

    public UserInfo(Guid id,
        int externalId,
        string firstName,
        string lastName,
        string photoUrl,
        Sex sex,
        DateTime birthday)
    {
        Id = id;
        ExternalId = externalId;
        FirstName = firstName;
        LastName = lastName;
        PhotoUrl = photoUrl;
        Sex = sex;
        Birthday = birthday;
    }
}