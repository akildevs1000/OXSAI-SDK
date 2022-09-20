using FCardProtocolAPI;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration.AddJsonFile("DeviceType.json", true);
builder.Configuration.AddJsonFile("Devices.json", true);

builder.Services.AddControllers();
builder.Services.AddScoped<FCardProtocolAPI.Command.IDoor8900HCommand, FCardProtocolAPI.Command.Door8900HCommand>();
builder.Services.AddScoped<FCardProtocolAPI.Command.IFingerprintCommand, FCardProtocolAPI.Command.FingerprintCommand>();
builder.Services.AddScoped<FCardProtocolAPI.Command.IFcardCommandResult, FCardProtocolAPI.Command.FcardCommandResult>();
builder.Services.AddScoped<FCardProtocolAPI.Command.IFcardCommandParameter, FCardProtocolAPI.Command.FcardCommandParameter>();

var app = builder.Build();

DefaultFilesOptions defaultFilesOptions = new();
defaultFilesOptions.DefaultFileNames.Clear();
defaultFilesOptions.DefaultFileNames.Add("help.html");
app.UseDefaultFiles(defaultFilesOptions);
app.UseStaticFiles();
app.UseRouting();


#region ∆Ù”√webSocket
var webSocketOptions = new WebSocketOptions()
{
    KeepAliveInterval = TimeSpan.FromSeconds(10),
};

app.UseWebSockets(webSocketOptions);
await FCardProtocolAPI.Command.CommandAllocator.Init(builder.Configuration);
#endregion
app.UseAuthorization();

app.MapControllers();

app.Run();
