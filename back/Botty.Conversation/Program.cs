var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/healthcheck", () => {
    return "I'm alive!";
});

app.MapPost("/message", (IncomingMessage message) => {
    var response = $"Hello {message.UserId}! Nice to meet you!";
    return response;
});

app.Run();

record IncomingMessage(string UserId, string Message) {}
