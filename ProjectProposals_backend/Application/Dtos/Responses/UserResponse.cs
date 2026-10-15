using System;

namespace Application.Dtos.Responses
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public GenericResponse Role { get; set; } = new();
    }

    [Obsolete("Use UserResponse instead.")]
    public class UsersResponse : UserResponse
    {
    }
}
