using Core.ValueObjects;

namespace Api.Models.Requests;

public record CreateUserRequest(string Name, Email Email, Password Password);