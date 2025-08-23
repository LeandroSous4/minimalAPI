using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimalAPI.Dominio.DTO;
using minimalAPI.Dominio.Entidades;
using minimalAPI.Dominio.Enum;
using minimalAPI.Dominio.Interfaces;
using minimalAPI.Dominio.ModelViews;
using minimalAPI.Dominio.Servicos;
using minimalAPI.Infra.DB;

#region Builder
var builder = WebApplication.CreateBuilder(args);
var key = builder.Configuration["Jwt:Key"]!;

builder.Services.AddAuthentication(Options =>
{
    Options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    Options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(Option =>
{
    Option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();
builder.Services.AddScoped<IAdminitradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(Options =>
{
    Options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT:"
    });

    Options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference{
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<DbContexto>(Options =>
{
    Options.UseSqlServer(builder.Configuration.GetConnectionString("sqlServer"));
});

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Administradores
string GerarTokenJwt(Administrador administrador)
{
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    var claims = new List<Claim>()
    {
        new("Email", administrador.Email),
        new("Perfil", administrador.Perfil),
        new(ClaimTypes.Role, administrador.Perfil)
    };
    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: credential
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

app.MapPost("/login", ([FromBody] minimalAPI.DTO.LoginDTO loginDTO, IAdminitradorServico adminitradorServico) =>
{
    var adm = adminitradorServico.Login(loginDTO);
    if (adm != null)
    {
        string token = GerarTokenJwt(adm);
        return Results.Ok(new AdmLogado
        {
            Email = adm.Email,
            Perfil = adm.Perfil,
            Token = token
        });
    }
    else
    {
        return Results.Unauthorized();
    }
}).AllowAnonymous().WithTags("Administrador");

app.MapPost("/Administradores", ([FromBody] AdministradorDTO administradorDTO, IAdminitradorServico adminitradorServico) =>
{
    var Validacao = new Erros
    {
        Mensagens = []
    };

    if (string.IsNullOrEmpty(administradorDTO.Email)) Validacao.Mensagens.Add("Email é obrigatório.");
    if (string.IsNullOrEmpty(administradorDTO.Senha)) Validacao.Mensagens.Add("Senha é obrigatória.");
    if (administradorDTO.Perfil == null) Validacao.Mensagens.Add("Perfil é obrigatório.");

    if (Validacao.Mensagens.Count > 0) return Results.BadRequest(Validacao);

    var admin = new Administrador
    {
        Email = administradorDTO.Email,
        Senha = administradorDTO.Senha,
        Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
    };

    adminitradorServico.Incluir(admin);

    return Results.Created($"/administrador/{admin.id}", new AdministradorModel
        {
            id = admin.id,
            Email = admin.Email,
            Perfil = admin.Perfil
        });
}).RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"})
.WithTags("Administrador");

app.MapGet("/Administradores", ([FromQuery] int? pagina, IAdminitradorServico adminitradorServico) =>
{
    var adms = new List<AdministradorModel>();
    var administradores = adminitradorServico.Todos(pagina);
    foreach (var adm in administradores)
    {
        adms.Add(new AdministradorModel
        {
            id = adm.id,
            Email = adm.Email,
            Perfil = adm.Perfil
        });
    }
    return Results.Ok(adms);
}).RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"})
.WithTags("Administrador");

app.MapGet("/Administradores/{id}", ([FromRoute] int id, IAdminitradorServico adminitradorServico) =>
{
    var admin = adminitradorServico.BuscaPorId(id);

    if (admin == null) return Results.NotFound();

    return Results.Ok(new AdministradorModel
        {
            id = admin.id,
            Email = admin.Email,
            Perfil = admin.Perfil
        });
}).RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"})
.WithTags("Administrador");

#endregion

#region Veiculos
Erros validaDTO(VeiculoDTO veiculoDTO)
{
    var Validacao = new Erros
    {
        Mensagens = new List<string>()
    };

    if (string.IsNullOrEmpty(veiculoDTO.Nome))
        Validacao.Mensagens.Add("Nome é obrigatório.");

    if (string.IsNullOrEmpty(veiculoDTO.Marca))
        Validacao.Mensagens.Add("Marca é obrigatório.");

    if (veiculoDTO.Ano < 1950)
        Validacao.Mensagens.Add("Ano é obrigatório e deve ser maior que 1950.");

    return Validacao;
}

app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{
    var Validacao = validaDTO(veiculoDTO);
    if (Validacao.Mensagens.Count > 0) return Results.BadRequest(Validacao);
    
    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };
    veiculoServico.Incluir(veiculo);

    return Results.Created($"/veiculo/{veiculo.id}", veiculo);
}).RequireAuthorization(new AuthorizeAttribute {Roles = "Adm, Editor"})
.WithTags("Veiculos");

app.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) =>
{
    var veiculos = veiculoServico.Todos(pagina);

    return Results.Ok(veiculos);
}).RequireAuthorization(new AuthorizeAttribute {Roles = "Adm, Editor"})
.WithTags("Veiculos");

app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);

    if (veiculo == null) return Results.NotFound();

    return Results.Ok(veiculo);
}).RequireAuthorization().WithTags("Veiculos");

app.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);

    if (veiculo == null) return Results.NotFound();

    var Validacao = validaDTO(veiculoDTO);
    if (Validacao.Mensagens.Count > 0) return Results.BadRequest(Validacao);
    
    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;

    veiculoServico.Atualizar(veiculo);

    return Results.Ok(veiculo);
}).RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"})
.WithTags("Veiculos");

app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);

    if (veiculo == null) return Results.NotFound();

    veiculoServico.Apagar(veiculo);

    return Results.NoContent();
}).RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"})
.WithTags("Veiculos");

#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
#endregion