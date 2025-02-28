using Configuration;
using DbContext;
using DbRepos;
using Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
builder.Services.AddEndpointsApiExplorer();

#region adding support for several secret sources and database sources
//to use either user secrets or azure key vault depending on UseAzureKeyVault tag in appsettings.json
builder.Configuration.AddApplicationSecrets("../Configuration/Configuration.csproj");

//use multiple Database connections and their respective DbContexts
builder.Services.AddEncryptions(builder.Configuration);
builder.Services.AddDatabaseConnections(builder.Configuration);
builder.Services.AddDatabaseConnectionsDbContext();
#endregion

builder.Services.AddJwtTokenService(builder.Configuration);

builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

//Inject Custom logger
builder.Services.AddSingleton<ILoggerProvider, InMemoryLoggerProvider>();
builder.Services.AddScoped<AdminDbRepos>();
builder.Services.AddScoped<ZooDbRepos>();
builder.Services.AddScoped<AnimalDbRepos>();
builder.Services.AddScoped<EmployeeDbRepos>();
builder.Services.AddScoped<CreditCardDbRepos>();
builder.Services.AddScoped<LoginDbRepos>();
builder.Services.AddScoped<IAdminService, AdminServiceDb>();
builder.Services.AddScoped<IZooService, ZooServiceDb>();
builder.Services.AddScoped<ILoginService, LoginServiceDb>();
builder.Services.AddScoped<AttractionDbRepos>();
builder.Services.AddScoped<AddressDbRepos>();
builder.Services.AddScoped<CategoryDbRepos>();
builder.Services.AddScoped<CommentDbRepos>();
builder.Services.AddScoped<IAttractionService, AttractionServiceDb>();
builder.Services.AddScoped<IAddressService, AddressServiceDb>();
builder.Services.AddScoped<ICommentServiceDb, CommentServiceDb>();
builder.Services.AddScoped<ICategoryService, CategoryServiceDb>();



var app = builder.Build();

//Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// global cors policy - the call to UseCors() must be done here
app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) 
    .AllowCredentials()); 

app.UseAuthorization();
app.MapControllers();

app.Run();
