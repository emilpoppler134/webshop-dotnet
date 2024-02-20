# Webshop in .NET

## Installation
Ensure you have .NET Core SDK installed on your machine.

1. Clone the repository:
```console
git clone https://github.com/yourusername/yourproject.git
```

2. Navigate into the project directory
```bash
cd yourproject
```

3. Install dependencies:
```bash
dotnet restore
```

## Configuration
### Stripe Configuration
To use Stripe for payment processing, you need to configure your Stripe API keys. If you don't have a Stripe account, you can sign up here.

1. Log in to your Stripe dashboard.
2. Go to Developers > API keys.
3. Copy your Secret key.
4. In your ASP.NET Core project, create a file called "appsettings.local.json".
5. Add the following code
```json
{
  "Stripe": {
    "SecretKey": "your_secret_key"
  }
}
```

### SQLite Configuration
SQLite is used as the database for this project. No additional configuration is required for SQLite.

## Usage
Start the project
```bash
dotnet run
```
Navigate to https://localhost:5029 in your web browser to access the application.
Note that the port might be different on your computer.
