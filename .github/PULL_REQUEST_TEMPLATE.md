## Description
Provide a concise summary of the changes introduced in this pull request and the rationale behind them.

## Related Issue
Fixes #(issue) or Relates to #(issue)

## Type of Change
Select all that apply:
- [ ] 🐛 Bug fix (non-breaking change fixing an issue)
- [ ] ✨ New feature (non-breaking change adding functionality)
- [ ] 💥 Breaking change (fix or feature causing existing functionality to change)
- [ ] 📝 Documentation update
- [ ] ♻️ Code refactoring (no functional changes)
- [ ] ⚡ Performance improvement
- [ ] 🔧 Build / CI / tooling update
- [ ] 🧪 Testing enhancement

## Implementation Details
Highlight significant design decisions, database schema modifications, or architectural changes:
- Detail 1
- Detail 2

## Testing Performed
Describe how you verified these changes:
- [ ] Built the solution successfully (`dotnet build NotifyX.slnx`)
- [ ] Verified API endpoints manually or via HTTP client (Swagger/cURL)
- [ ] Verified database migrations apply cleanly (`dotnet ef database update`)
- [ ] Verified background worker behavior (`OutboxBackgroundService`)
- [ ] Verified SignalR notifications delivery

## Contributor Checklist
Please review and check each box before submitting:
- [ ] My code adheres to the project's coding standards and naming conventions.
- [ ] I have performed a self-review of my own code.
- [ ] I have not committed any sensitive credentials, connection strings, or secrets.
- [ ] My branch follows the naming convention (`feature/*`, `bugfix/*`, `hotfix/*`, `docs/*`).
- [ ] My commit messages follow [Conventional Commits](https://www.conventionalcommits.org/).
- [ ] I have updated relevant documentation where necessary (README, XML comments).
- [ ] No compilation warnings or errors are introduced.
- [ ] I have tested changes in a local development environment.
