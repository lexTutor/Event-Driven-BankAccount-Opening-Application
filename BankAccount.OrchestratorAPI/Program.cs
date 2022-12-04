using BankAccount.OrchestratorAPI;
using BankAccount.Shared;
using BankAccount.Shared.OrchestorService;
using BankAccount.Shared.Utilities;
using System.Reflection;
using static BankAccount.Shared.Domain.RecordTypes;
using static BankAccount.Shared.Utilities.Enumeration;

var myAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables()
    .AddUserSecrets(Assembly.GetExecutingAssembly(), false);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSharedServices(builder.Configuration);
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddAutoMap();
builder.Services.AddLogging();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyOrigin();
                          policy.AllowAnyHeader();
                      });
});
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
if (app.Environment.IsDevelopment())
{
    app.EnsureDatabaseSetup();
}

app.UseCors(myAllowSpecificOrigins);

app.MapPost("/initiateWorkflow", async (InitiateWorkFlowPayload payload, IOrchestratorService orchestratorService, ILogger<Program> logger)
    =>
{
    try
    {
        var response = await orchestratorService.InitiateWorkFlow(payload);
        if (response.Successful)
        {
            return Results.Ok(response);
        }
        return Results.BadRequest(response);
    }
    catch (Exception ex)
    {
        logger.LogError(ex.Message, ex);
        return Results.Problem(statusCode: 500, detail: Constants.InternalServerErrorMessage);
    }
});

app.MapGet("/WorkFlows", () => WorkFlow.PotentialMember.ToEnumModelList());

app.Run();