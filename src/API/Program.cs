using API;
using Application.Implementation;
using Microsoft.Extensions.DependencyInjection;

var host = await HostCreator.CreateHost(args);

var consoleInvoker = host.Services.GetRequiredService<ConsoleInvoker>();
await consoleInvoker.Run();