using Microsoft.SemanticKernel;
using OpenAI;
using SemanticKernelSignalRChatDemo.KernelPlugins;
using SemanticKernelSignalRChatDemo.Services;
using SemanticKernelSignalRChatDemo.SignalRHubs;
using System.ClientModel;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services
    .AddKernel()
    .AddOpenAIChatCompletion(
    modelId: configuration["OpenRouter:Model"] ?? throw new ArgumentNullException("OpenRouter:Model"),
    openAIClient: new OpenAIClient(
        credential: new ApiKeyCredential(configuration["OpenRouter:ApiKey"] ?? throw new ArgumentNullException("OpenRouter:ApiKey")),
        options: new OpenAIClientOptions
        {
            Endpoint = new Uri(configuration["OpenRouter:Endpoint"] ?? throw new ArgumentNullException("OpenRouter:Endpoint"))
        })
    )
    .Plugins.AddFromType<OrderPizzaPlugin>("OrderPizza");

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy.AllowAnyMethod()
                                       .AllowAnyHeader()
                                       .AllowCredentials()
                                       .SetIsOriginAllowed(s => true)));

builder.Services.AddSignalR();

builder.Services.AddSingleton<AIService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.UseCors();

app.MapHub<AIHub>("/ai");

app.MapPost("/chat", async (AIService aiService, ChatRequestVM chatRequest, CancellationToken cancellationToken)
    => await aiService.GetMessageStreamAsync(chatRequest.Prompt, chatRequest.ConnectionId, cancellationToken));

app.Run();

public record ChatRequestVM(string Prompt, string ConnectionId);
