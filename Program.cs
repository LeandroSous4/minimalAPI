var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (minimalAPI.DTO.LoginDTO loginDTO) =>
{
    if (loginDTO.Email == "admin" && loginDTO.Senha == "admin")
    {
        return Results.Ok("Login com sucesso.");
    }
    else
    {
        return Results.Unauthorized();
    }
});

app.Run();
