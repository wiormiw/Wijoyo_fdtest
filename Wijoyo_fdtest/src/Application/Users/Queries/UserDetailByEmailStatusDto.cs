namespace Wijoyo_fdtest.Application.Users.Queries;

public class UserDetailByEmailStatusDto
{
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init;} = string.Empty;
    public string EmailStatus { get; init; } = string.Empty;
}
