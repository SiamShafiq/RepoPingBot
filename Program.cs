using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

var app = builder.Build();

app.MapPost("/webhook/github", async (HttpRequest request, IHttpClientFactory httpClientFactory, IConfiguration config) =>
{
    using var reader = new StreamReader(request.Body);
    var body = await reader.ReadToEndAsync();

    using var jsonDoc = JsonDocument.Parse(body);
    var root = jsonDoc.RootElement;

    var eventType = request.Headers["X-GitHub-Event"].ToString();

    string message = eventType switch
    {
        "push" => BuildPushMessage(root),
        "pull_request" => BuildPullRequestMessage(root),
        _ => $"GitHub event received: {eventType}"
    };

    var botToken = config["Telegram:BotToken"];
    var chatId = config["Telegram:ChatId"];

    if (string.IsNullOrWhiteSpace(botToken) || string.IsNullOrWhiteSpace(chatId))
    {
        return Results.Problem("Telegram configuration missing.");
    }

    var client = httpClientFactory.CreateClient();
    var telegramUrl = $"https://api.telegram.org/bot{botToken}/sendMessage";

    var payload = new
    {
        chat_id = chatId,
        text = message
    };

    var content = new StringContent(
        JsonSerializer.Serialize(payload),
        Encoding.UTF8,
        "application/json"
    );

    var response = await client.PostAsync(telegramUrl, content);

    return response.IsSuccessStatusCode
        ? Results.Ok(new { success = true, eventType })
        : Results.Problem("Failed to send Telegram message.");
});

app.Run();

static string BuildPushMessage(JsonElement root)
{
    var repo = root.GetProperty("repository").GetProperty("name").GetString() ?? "unknown repo";
    var pusher = root.GetProperty("pusher").GetProperty("name").GetString() ?? "someone";

    string commitMessage = "No commit message";
    if (root.TryGetProperty("head_commit", out var headCommit) &&
        headCommit.ValueKind != JsonValueKind.Null &&
        headCommit.TryGetProperty("message", out var msg))
    {
        commitMessage = msg.GetString() ?? commitMessage;
    }

    return $"[GitHub Push]\nRepo: {repo}\nBy: {pusher}\nCommit: {commitMessage}";
}

static string BuildPullRequestMessage(JsonElement root)
{
    var action = root.GetProperty("action").GetString() ?? "updated";
    var pr = root.GetProperty("pull_request");
    var title = pr.GetProperty("title").GetString() ?? "Untitled PR";
    var repo = root.GetProperty("repository").GetProperty("name").GetString() ?? "unknown repo";
    var user = pr.GetProperty("user").GetProperty("login").GetString() ?? "someone";

    return $"[Pull Request]\nRepo: {repo}\nAction: {action}\nBy: {user}\nTitle: {title}";
}