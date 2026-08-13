# LuffyAuth C# Example Project

Official C# example implementation for **LuffyAuth** (software licensing & authentication system). Includes both **Console** and **WinForms** application templates.

## 🔗 Official Links

- **Auth Dashboard**: [https://auth.luffycpp.in](https://auth.luffycpp.in)
- **Discord Community**: [https://discord.gg/mza6dZQ4h](https://discord.gg/mza6dZQ4h)
- **YouTube Channel**: [https://www.youtube.com/@Rapidcorpo](https://www.youtube.com/@Rapidcorpo)

---

## 🚀 Quick Setup

1. Open `Console/Program.cs` or `Form/Login.cs` in Visual Studio.
2. Configure your application credentials:

```csharp
public static api LuffyAuthApp = new api(
    name: "Testing",                  // Application Name
    ownerid: "xrfNty6okO",             // Account Owner ID
    version: "1.0",                   // Application Version
    path: null                        // Optional path
);
```

3. Call `init()` before executing any authentication actions:

```csharp
LuffyAuthApp.init();

if (!LuffyAuthApp.response.success)
{
    Console.WriteLine("Init Failed: " + LuffyAuthApp.response.message);
    return;
}
```

---

## 🛠️ Features Implemented

- **Authentication**: `login(username, password)`
- **Registration**: `register(username, password, license)`
- **License Validation**: `license(key)`
- **Account Upgrade**: `upgrade(username, license)`
- **User Variables & Webhooks**: Remote variable fetching and webhook dispatching
- **HWID & Blacklist Checks**: Built-in security & subscription verification

---

## 📜 License

Distributed under the MIT License. See `LICENSE` for details.
