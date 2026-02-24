using FastEndpoints;
using FastEndpoints.Swagger;
using TwitchAPI.Configuration;
using TwitchAPI.Constants;
using TwitchAPI.Services;
using ErrorResponse = TwitchAPI.Models.Responses.ErrorResponse;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<TwitchSettings>(
    builder.Configuration.GetSection(TwitchSettings.SectionName));

builder.Services.AddHttpClient<ITwitchService, TwitchService>();

builder.Services
    .AddFastEndpoints()
    .SwaggerDocument();

var app = builder.Build();

app.UseFastEndpoints(c =>
{
    c.Errors.ResponseBuilder = (failures, ctx, statusCode) =>
        ErrorResponse.From(ErrorMessages.InvalidOrMissingId);
});

app.UseSwaggerGen();

app.Run();