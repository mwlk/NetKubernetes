using NetKubernetes.DTO.Users;

namespace NetKubernetes.Repository.Interfaces;

public interface IUsersRepository
{
    Task<UserResponseDto> GetUser();

    Task<UserResponseDto> Login(LoginRequestDto request);

    Task<UserResponseDto> Post(UserPostDto request);
}
