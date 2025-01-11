# Secure Swagger API with Basic Authentication

This project demonstrates how to implement basic authentication for a Swagger API in ASP.NET Core. It includes user authentication, protected Swagger UI, and a sample WeatherForecast endpoint.

## ⚠️ Security Disclaimer

**This project is for learning/demonstration purposes only!**

The current implementation has several security concerns that make it unsuitable for production:
- Passwords are stored in plain text in appsettings.json
- Basic authentication sends credentials in easily decoded base64
- No password hashing or salting
- Configuration-based user storage

For production applications, always:
- Use secure password hashing (e.g., bcrypt)
- Store user credentials in a secure database
- Use HTTPS
- Implement proper token-based authentication
- Follow security best practices

## Project Structure
```
SecureSwaggerApi8/
├── Controllers/
│ ├── AuthController.cs # Handles authentication
│ └── WeatherForecastController.cs # Sample API endpoint
├── Models/
│ ├── SwaggerUser.cs # User model
│ └── WeatherForecast.cs # Sample data model
├── Program.cs # Application setup
└── appsettings.json # Configuration
```

## Endpoints
### Authentication

#### Login
GET /auth/login?username={username}&password={password}
- Authenticates user and redirects to Swagger UI
- Example: `https://localhost:7233/auth/login?username=admin&password=SecurePassword123!`
- Returns:
  - Success: Redirects to `/swagger/index.html`
  - Failure: 401 Unauthorized

#### Logout
GET /auth/logout
- Clears authentication cookies
- Returns: "Logged Out. Sign in again to access the swagger page"
### API Documentation

#### Swagger UI
GET /swagger/index.html
- Interactive API documentation interface
- Protected: Requires authentication
- Redirects to login if unauthorized

#### Swagger JSON
GET /swagger/v1/swagger.json
- OpenAPI specification document
- Protected: Requires authentication
- Used by Swagger UI for API documentation

### API Endpoints

#### Weather Forecast
GET /WeatherForecast
- Sample protected endpoint
- Requires authentication
- Returns: Array of weather forecast data
- Protected by [Authorize] attribute

## Configuration

### User Management

Users are configured in `appsettings.json`:
```json
{
  "SwaggerCredentials": {
    "Users": [
      {
        "Username": "admin",
        "Password": "SecurePassword123!"
      },
      {
        "Username": "user",
        "Password": "UserPass123!"
      }
    ]
  }
}
```

### Adding Users
To add a new user:
1. Open `appsettings.json`
2. Add a new user object to the `Users` array:
```json
{
  "Username": "newuser",
  "Password": "password123"
}
```

### Removing Users
To remove a user:
1. Open `appsettings.json`
2. Delete the corresponding user object from the `Users` array
3. Save the file (changes take effect on next application restart)

## Authentication Flow

1. User attempts to access protected resource (e.g., Swagger UI)
2. If not authenticated:
   - Redirected to login page
   - User provides credentials via login endpoint
   - On success, authentication cookie is set
   - Redirected back to requested resource
3. If authenticated:
   - Authentication cookie is included in subsequent requests
   - Access granted to protected resources
4. On logout:
   - Authentication cookie is cleared
   - User returns to unauthenticated state

## Development Setup

1. Clone the repository
2. Navigate to project directory
3. Restore dependencies:
```bash
dotnet restore
```
4. Update user credentials in `appsettings.json` if needed
5. Run the application:
```bash
dotnet run
```
6. Access the application:
   - Swagger UI: `https://localhost:7233/swagger/index.html`
   - Login URL: `https://localhost:7233/auth/login?username=admin&password=SecurePassword123!`

## Technologies Used

- ASP.NET Core 8.0
- Swagger/OpenAPI (Swashbuckle)
- Cookie Authentication
- Basic Authentication Handler
- C# 12

## Testing

You can test the API using:
- Swagger UI interface
- Direct URL access
- API testing tools (Postman, curl, etc.)

Example curl command:
bash
curl -X GET "https://localhost:7233/WeatherForecast" -H "Cookie: .AspNetCore.Cookies=<your-cookie-value>"
