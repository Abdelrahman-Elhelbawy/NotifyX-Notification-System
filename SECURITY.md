# Security Policy

The NotifyX maintainers and community take the security of this project very seriously. We appreciate your efforts to responsibly disclose any vulnerabilities.

## Supported Versions

NotifyX is actively maintained on the latest preview/release branch. Currently, security updates and patches are provided for the following versions:

| Version | Supported          | Target Framework | Notes                     |
| :------ | :----------------- | :--------------- | :------------------------ |
| 1.0.x   | :white_check_mark: | .NET 11 (Preview) | Current active development |
| < 1.0   | :x:                | -                | Experimental builds       |

## Reporting a Vulnerability

**Please do not report security vulnerabilities via public GitHub issues, discussions, or pull requests.**

To report a vulnerability responsibly:

1. **GitHub Private Vulnerability Reporting (Preferred)**:
   Navigate to the repository's **Security** tab on GitHub:
   [NotifyX Security Advisories](https://github.com/Abdelrahman-Elhelbawy/NotifyX-Notification-System/security/advisories)
   Click **"Report a vulnerability"** to open a confidential advisory draft directly with the maintainers.

2. **Alternative Contact**:
   If private vulnerability reporting is unavailable, reach out privately to the repository owner via their verified GitHub profile:
   [Abdelrahman-Elhelbawy on GitHub](https://github.com/Abdelrahman-Elhelbawy).

### What to Include in Your Report

To help us triage and resolve the issue quickly, please include:
- A clear description of the vulnerability and its potential impact.
- Detailed step-by-step instructions to reproduce the issue (proof-of-concept script, sample HTTP request, or minimal reproduction repository).
- Affected components (endpoints, background services, database layers).
- Any proposed remediations or patches, if available.
- Relevant system configuration and environment details (.NET SDK version, OS).

## Response Process

When a security vulnerability is reported:
1. **Acknowledgment**: We aim to acknowledge receipt of the report within 48 to 72 hours.
2. **Investigation**: The issue will be evaluated and verified in a private development environment.
3. **Remediation**: If confirmed, a fix will be developed and tested privately.
4. **Advisory & Release**: A security release will be published alongside a GitHub Security Advisory crediting the reporter (unless anonymity is requested).

## Security Best Practices for Contributors

Contributors are required to follow these security practices when developing and testing:
- **Never commit secrets**: Do not commit real connection strings, database passwords, API keys, JWT secrets, or production URLs. Use .NET User Secrets (`dotnet user-secrets`) or `.env` files (excluded by `.gitignore`) for local configuration.
- **Input validation**: Ensure all API inputs are strictly validated using FluentValidation rules and model constraints.
- **Data sanitization**: Prevent injection attacks by relying on Entity Framework Core parameterized queries and avoiding raw string concatenation in SQL commands.
- **Rate limiting and abuse prevention**: Maintain and respect rate limiting policies on newly introduced endpoints.
- **Dependency management**: Keep NuGet dependencies updated and review dependency alerts promptly.
