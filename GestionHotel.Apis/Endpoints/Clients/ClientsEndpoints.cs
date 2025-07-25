﻿using GestionHotel.Apis.DTOs;
using GestionHotel.Application.UseCases.Clients;
using Microsoft.AspNetCore.Mvc;
using MiniValidation;

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
            if (!MiniValidator.TryValidate(request, out var errors))
                return Results.ValidationProblem(errors);

            var clientId = useCase.Execute(request.Name, request.Email, request.Password);
            return Results.Ok(clientId);
        })
        .WithName("CreateClient")
        .WithOpenApi();

        group.MapPost("/login", async (
            [FromBody] LoginClientRequest request,
            [FromServices] LoginClient useCase) =>
        {
            try
            {
                var token = useCase.Execute(request.Email, request.Password);
                return Results.Ok(new { Token = token });
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        })
        .WithName("LoginClient")
        .WithOpenApi();
    }
}