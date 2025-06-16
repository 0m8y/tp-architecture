using GestionHotel.Application.UseCases.Clients;
using GestionHotel.Apis.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GestionHotel.Apis.Endpoints.Clients;

public static class ClientsEndpoints
{
    public static void MapClientsEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/v1/clients")
            .WithTags("Clients");

        group.MapPost("/", async (
            [FromBody] CreateClientRequest request,
            [FromServices] CreateClient useCase) =>
        {
            var clientId = useCase.Execute(request.Name, request.Email, request.Password);
            return Results.Ok(clientId);
        })
        .WithName("CreateClient")
        .WithOpenApi();
    }
}