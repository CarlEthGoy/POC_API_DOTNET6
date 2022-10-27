using API.BLL;
using API.Cryptography;
using API.Database;
using API.Helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using Neo4j.Driver;

var builder = WebApplication.CreateBuilder(args);

#region Swagger && Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
  //c.SwaggerDoc("v2", new OpenApiInfo { Title = "API", Version = "v2" });

  c.AddSecurityDefinition("oauth2", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
  {
    Description = "Oath2.0 which uses AuthorizationCode flow",
    Name = "oauth2.0",
    Type = Microsoft.OpenApi.Models.SecuritySchemeType.OAuth2,
    Flows = new Microsoft.OpenApi.Models.OpenApiOAuthFlows
    {
      AuthorizationCode = new Microsoft.OpenApi.Models.OpenApiOAuthFlow
      {
        AuthorizationUrl = new Uri(builder.Configuration["SwaggerAzureAd:AuthorizationUrl"]),
        TokenUrl = new Uri(builder.Configuration["SwaggerAzureAd:TokenUrl"]),
        Scopes = new Dictionary<string, string>
          {
            {builder.Configuration["SwaggerAzureAd:Scope"],"Access Api as User" }
          }
      }
    }
  });
  c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement {
    {
      new OpenApiSecurityScheme
      {
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
      },
      new[] { builder.Configuration["SwaggerAzureAd:Scope"] }
    }
  });
});
#endregion

#region VERSIONNING
builder.Services.AddApiVersioning(setup =>
{
  setup.DefaultApiVersion = new ApiVersion(1, 0);
  setup.AssumeDefaultVersionWhenUnspecified = true;
  setup.ReportApiVersions = true;
  setup.ApiVersionReader = new UrlSegmentApiVersionReader();
});

builder.Services.AddVersionedApiExplorer(options =>
{
  options.GroupNameFormat = "'v'VVV";
  options.SubstituteApiVersionInUrl = true;
  options.AssumeDefaultVersionWhenUnspecified = true;
});
#endregion

#region Dependancy injection
builder.Services.AddSingleton(GraphDatabase.Driver(builder.Configuration["Neo4JSettings:Connection"], AuthTokens.Basic(builder.Configuration["Neo4JSettings:User"], builder.Configuration["Neo4JSettings:Password"])));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVaultRepository, VaultRepository>();
builder.Services.AddScoped<IPasswordRepository, PasswordRepository>();
builder.Services.AddScoped<ICryptographyUtil, CryptographyUtil>();
builder.Services.AddScoped<IAuthorizationHelper, AuthorizationHelper>();
builder.Services.AddScoped<IBLLUser, BLLUser>();
builder.Services.AddScoped<IBLLVault, BLLVault>();
builder.Services.AddScoped<IBLLPassword, BLLPassword>();
builder.Services.AddScoped<IBLLAuthentication, BLLAuthentication>();
builder.Services.AddHttpContextAccessor();
#endregion

builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI(c =>
  {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    //c.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
    c.OAuthClientId(builder.Configuration["SwaggerAzureAd:ClientId"]);
    c.OAuthUsePkce();
    //c.OAuthScopeSeparator(" ");
  });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();