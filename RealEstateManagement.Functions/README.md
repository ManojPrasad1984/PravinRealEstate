# RealEstateManagement Azure Functions API

This project adds HTTP-triggered Azure Function APIs for the existing real-estate solution.

## Endpoints

All routes are under `/api` by default in Azure Functions.

- `GET /api/assets` - list assets
- `POST /api/assets` - create asset
- `GET /api/tenants` - list tenants
- `POST /api/tenants` - create tenant
- `GET /api/owners` - list owners
- `POST /api/owners` - create owner

## Configuration

Set `EstateConnectionString` in local settings or application settings:

```json
"EstateConnectionString": "server=(LocalDB)\\MSSQLLocalDB;database=EstateDBExam;trusted_connection=true;trust server certificate=true"
```

The HTTP trigger authorization level is set to `Function`.
