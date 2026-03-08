ASPNETCORE_ENVIRONMENT=Development

# Last Migrations
dotnet ef migrations add AddAppointmentIdServiceRecord --project VoroBidsCrm.Infrastructure --startup-project VoroBidsCrm.API --output-dir Migrations

# Remove Last Migrations
dotnet ef migrations remove --project VoroBidsCrm.Infrastructure --startup-project VoroBidsCrm.API

# Update
dotnet ef database update --project VoroBidsCrm.Infrastructure --startup-project VoroBidsCrm.API

# Watch
dotnet watch --project VoroBidsCrm.API --urls http://0.0.0.0:5000