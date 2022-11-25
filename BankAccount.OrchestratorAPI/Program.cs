using BankAccount.OrchestratorAPI.OrchestorService;
using BankAccount.Shared;
using BankAccount.Shared.Utilities;
using static BankAccount.Shared.Domain.RecordTypes;
using static BankAccount.Shared.Utilities.Enumeration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSharedServices();
builder.Services.AddScoped<IOrchestratorService, OrchestratorService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.MapPost("/initiateWorkflow", (InitiateWorkFlowPayload payload, IOrchestratorService orchestratorService)
    => orchestratorService.InitiateWorkFlow(payload));

app.MapGet("/WorkFlows", () => WorkFlow.PotentialMember.ToEnumModelList());

app.Run();