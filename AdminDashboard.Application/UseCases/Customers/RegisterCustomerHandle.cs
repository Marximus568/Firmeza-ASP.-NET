using AdminDashboard.Domain.Entities;
using AdminDashboardApplication.DTOs.CustomersEmail;
using AdminDashboardApplication.Interfaces;
using AdminDashboardApplication.Interfaces.Repository;

namespace AdminDashboardApplication.UseCases.Customers;

public class RegisterCustomerHandler
{
    private readonly IEmailService _emailService;
    private readonly ICustomerRepository _customerRepo;

    public RegisterCustomerHandler(IEmailService emailService, ICustomerRepository customerRepo)
    {
        _emailService = emailService;
        _customerRepo = customerRepo;
    }

    public async Task Execute(RegisterCustomerDto dto)
    {
        var customer = new Clients(
            dto.FirstName,
            dto.LastName,
            dto.Email,
            dto.Phone
        );

        await _customerRepo.AddCustomerAsync(customer);

        await _emailService.SendEmailAsync(
            dto.Email,
            "Welcome to Firmeza!",
            "Your registration was successful."
        );
    }
}
