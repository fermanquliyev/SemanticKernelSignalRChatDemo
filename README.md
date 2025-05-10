# SemanticKernelSignalRChatDemo

A demo project showcasing the integration of **Semantic Kernel** with **SignalR** to create a real-time chat application. This project also includes a plugin for ordering pizzas, demonstrating how to extend functionality using plugins.

## Table of Contents
- [Overview](#overview)
- [Features](#features)
- [Prerequisites](#prerequisites)
- [Setup](#setup)
- [Usage](#usage)
- [Project Structure](#project-structure)
- [Contributing](#contributing)
- [License](#license)

## Overview
This project demonstrates how to build a real-time chat application using **Semantic Kernel** (a framework for integrating AI capabilities) and **SignalR** (for real-time web functionality). It includes a chat service that maintains conversation history and a plugin (`OrderPizzaPlugin`) for handling pizza orders within the chat.

## Features
- Real-time chat functionality using SignalR.
- Integration with Semantic Kernel for AI-driven chat capabilities.
- A custom `OrderPizzaPlugin` to simulate ordering pizzas via chat.
- Persistent chat history using a `ChatHistory` service.
- Configurable settings via `appsettings.json`.

## Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download) (version 6.0 or later recommended).
- An IDE like Visual Studio or Visual Studio Code.
- (Optional) Access to an AI service compatible with Semantic Kernel (e.g., OpenAI API key).

## Setup
1. **Clone the Repository**:
   ```bash
   git clone https://github.com/fermanquliyev/SemanticKernelSignalRChatDemo.git
   cd SemanticKernelSignalRChatDemo
   ```

2. **Restore Dependencies**:
   ```bash
   dotnet restore
   ```

3. **Configure Settings**:
   - Open `appsettings.json` and update the necessary configurations (e.g., AI service credentials, SignalR settings).
   - Note: The project includes a warning about the `UseExceptionHandler` setting for development. For production, follow the recommended practices (e.g., use HTTPS redirection).

4. **Build and Run**:
   ```bash
   dotnet build
   dotnet run
   ```

## Usage
1. Start the application using the steps above.
2. Open a browser or a SignalR client to connect to the chat hub (`AIHub`).
3. Use the chat interface to send messages and interact with the AI.
4. To order a pizza, use commands like "add pizza to cart" within the chat. The `OrderPizzaPlugin` will handle the request and return order details.

### Example Commands
- Send a message: `Hello, how can I order a pizza?`
- Add to cart: `Add a pizza to cart`
- View cart: `Show my cart`

## Project Structure
```
SemanticKernelSignalRChatDemo/
├── Pages/                  # Razor Pages for the web interface
│   ├── Error.cshtml
│   ├── Index.cshtml
│   ├── Privacy.cshtml
│   ├── ViewImports.cshtml
│   └── ViewStart.cshtml
├── Properties/             # Launch settings
│   └── launchSettings.json
├── Services/               # Core services
│   ├── AIService.cs        # AI service using Semantic Kernel
│   ├── SignalRHubs/        # SignalR hubs
│   │   └── AIHub.cs        # Main chat hub
├── KernelPlugins/          # Semantic Kernel plugins
│   └── OrderPizzaPlugin.cs # Plugin for ordering pizzas
├── Program.cs              # Application entry point
├── appsettings.json        # Configuration settings
├── appsettings.Development.json  # Development-specific settings
├── libman.json             # Library manager configuration
├── wwwroot/                # Static files (CSS, JS, etc.)
├── Dockerfile              # Docker configuration
├── .dockerignore           # Docker ignore file
├── .gitignore              # Git ignore file
└── README.md               # This file
```

### Key Classes
- **AIService**: Manages the chat functionality, integrates with Semantic Kernel, and handles chat history.
- **AIHub**: The SignalR hub for real-time chat communication.
- **OrderPizzaPlugin**: A Semantic Kernel plugin for handling pizza orders, including adding items to a cart and calculating prices.
- **ChatHistory**: A static class for managing and retrieving chat history.

## Contributing
Contributions are welcome! Please follow these steps:
1. Fork the repository.
2. Create a new branch (`git checkout -b feature/your-feature`).
3. Make your changes and commit (`git commit -m "Add your feature"`).
4. Push to your branch (`git push origin feature/your-feature`).
5. Open a Pull Request.

## License
This project is licensed under the MIT License. See the [[LICENSE](https://github.com/fermanquliyev/SemanticKernelSignalRChatDemo/blob/master/LICENSE)] file for details.
