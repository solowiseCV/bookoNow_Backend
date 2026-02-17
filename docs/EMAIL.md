# Email configuration

This project reads SMTP configuration from `EmailSettings` at runtime. For local development, use `dotnet user-secrets` (already configured for this project). For production, use environment variables or a secrets manager.

Recommended keys:
- `EmailSettings:SmtpServer` (e.g. smtp.sendgrid.net)
- `EmailSettings:Port` (e.g. 587)
- `EmailSettings:Username` (SMTP username or API key)
- `EmailSettings:Password` (SMTP password or API key)
- `EmailSettings:FromEmail` (sender address)
- `EmailSettings:FromName` (sender display name)

Local setup (already executed):

```bash
cd BookNow.Presentation
# initialize once per project
dotnet user-secrets init
# set values
dotnet user-secrets set "EmailSettings:SmtpServer" "smtp.gmail.com"
dotnet user-secrets set "EmailSettings:Port" "587"
dotnet user-secrets set "EmailSettings:Username" "your-email@gmail.com"
dotnet user-secrets set "EmailSettings:Password" "REPLACE_WITH_REAL_PASSWORD"
dotnet user-secrets set "EmailSettings:FromEmail" "noreply@yourapp.com"
dotnet user-secrets set "EmailSettings:FromName" "BookNow App"
```

CI / Production:
- Use environment variables or Azure Key Vault, AWS Secrets Manager, etc.
- In CI set `DOTNET_USER_SECRETS` or inject env vars before starting the app.

Security notes:
- Never commit real credentials to source control.
- Prefer managed secret stores (Azure Key Vault) for production.
