# RepoPing 🚀

GitHub → Telegram Developer Notifications

A lightweight .NET web service that listens to GitHub webhook events and sends real-time notifications to a Telegram bot.

---

## ✨ Features

* Receives GitHub webhook events (push, pull requests)
* Parses JSON payloads using C#
* Sends formatted notifications via Telegram Bot API
* Simple, fast, and focused on developer workflow automation

---

## 🛠️ Tech Stack

* C# / .NET (ASP.NET Core Web API)
* REST APIs
* GitHub Webhooks
* Telegram Bot API
* ngrok (for local tunneling)

---

## 📦 How It Works

```text
GitHub → Webhook → .NET API → Telegram Bot → You
```

1. GitHub sends an event (push, PR)
2. .NET API receives and processes the payload
3. App formats a message
4. Telegram bot sends a notification

---

## 🚀 Getting Started

### 1. Clone the repo

```bash
git clone https://github.com/YOUR_USERNAME/repoping.git
cd repoping
```

---

### 2. Configure Telegram

Create a bot using **BotFather** on Telegram and get your bot token.

Send your bot a message, then retrieve your chat ID using:

```text
https://api.telegram.org/bot<YOUR_BOT_TOKEN>/getUpdates
```

---

### 3. Add configuration

Update `appsettings.json`:

```json
{
  "Telegram": {
    "BotToken": "YOUR_BOT_TOKEN",
    "ChatId": "YOUR_CHAT_ID"
  }
}
```

---

### 4. Run the app

```bash
dotnet run
```

---

### 5. Expose with ngrok

```bash
ngrok http 5000
```

Copy the generated HTTPS URL.

---

### 6. Set up GitHub Webhook

In your repository:

* Go to **Settings → Webhooks → Add webhook**
* Payload URL:

```
https://your-ngrok-url/webhook/github
```

* Content type:

```
application/json
```

* Select events:

  * Push
  * Pull requests

---

## 🧪 Example Notification

```text
[GitHub Push]
Repo: repoping
By: siamshafiq
Commit: added webhook handler
```

---

## 💡 Why I Built This

I wanted to understand how developer tools integrate across platforms like GitHub and messaging systems. This project simulates a lightweight internal tool that improves visibility into team activity.

---

## 🔧 Future Improvements

* Store event history and expose via API
* Add filtering by repository or event type
* Improve message formatting
* Add a simple frontend dashboard

---

## 📄 License

MIT
