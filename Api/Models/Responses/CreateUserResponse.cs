using Core.ValueObjects;

namespace Api.Models.Responses;

public record CreateUserResponse(Guid Id, Email Email);