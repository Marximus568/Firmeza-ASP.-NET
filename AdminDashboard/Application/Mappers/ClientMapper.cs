using AdminDashboard.Domain.Entities;
using AdminDashboard.Infrastructure.Identity.Entities;

namespace AdminDashboard.Application.Mappers
{
    public static class ClientMapper
    {
        // Convert from Identity object to Domain entity
        public static Client ToDomain(ApplicationClientIdentity identityClient)
        {
            return new Client
            {
                Id = identityClient.Id.GetHashCode(), // Temporary int mapping if DB handles int IDs
                Name = identityClient.UserName ?? string.Empty,
                Email = identityClient.Email ?? string.Empty,
                role = identityClient is { } ? "Client" : string.Empty, // Default role if not set
                Phone = string.Empty,
                Address = string.Empty
            };
        }

        // Convert from Domain entity to Identity object
        public static ApplicationClientIdentity ToIdentity(Client client)
        {
            return new ApplicationClientIdentity
            {
                UserName = client.Name,
                Email = client.Email,
                FirstName = client.Name, // You can adjust this mapping if you split names
                LastName = string.Empty,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}