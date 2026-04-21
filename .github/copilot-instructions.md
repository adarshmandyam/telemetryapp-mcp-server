# Copilot Instructions

## Project Guidelines
- User's preferred terminal shell: powershell.exe and uses Microsoft Visual Studio Enterprise 2026 (18.4.3) as their IDE.
- Use MCP server type 'streaming-http' instead of legacy 'sse' for mcp-servers.json.
- When requesting an MCP server deployment diagram, include an ARR reverse proxy in front of IIS; show IIS hosting with Application Pool + ASP.NET Core Module (ANCM) and alternative Windows Service/Worker hosting; indicate ports and example URL http://localhost:5139/mcp.