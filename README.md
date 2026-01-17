# TravelTechApi

Backend API for travel tech application with standardized response structure.

## API Response Structure

This project has been set up with a standardized API response structure to ensure consistency for frontend developers.

### Response Format

**Success Response:**
```json
{
  "success": true,
  "message": "Operation completed successfully",
  "data": { ... },
  "errors": null,
  "timestamp": "2026-01-17T23:22:55Z"
}
```

**Error Response:**
```json
{
  "success": false,
  "message": "Error message",
  "data": null,
  "errors": [
    {
      "field": "email",
      "message": "Email is required",
      "code": "REQUIRED_FIELD"
    }
  ],
  "timestamp": "2026-01-17T23:22:55Z"
}
```

### Usage in Controllers

**Success with data:**
```csharp
return this.Success(data, "User retrieved successfully");
```

**Success without data:**
```csharp
return this.Success("Operation completed");
```

**Created response:**
```csharp
return this.Created(newUser, "User created successfully");
```

**Throw exception (automatically handled):**
```csharp
throw new NotFoundException("User not found");
throw new ValidationException("Validation failed", errors);
```

### Available Custom Exceptions

- `BadRequestException` (400)
- `NotFoundException` (404)
- `UnauthorizedException` (401)
- `ForbiddenException` (403)
- `ConflictException` (409)
- `ValidationException` (422)

### Test Endpoints

- `GET /api/ping` - Health check with standardized response
- `GET /api/ping/error` - Test exception handling
- `GET /api/example/*` - See `ExampleController.cs` for more examples
