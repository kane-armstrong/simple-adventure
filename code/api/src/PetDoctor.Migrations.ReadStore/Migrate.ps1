param ([Parameter(Mandatory=$true)]$migrationName)
dotnet ef migrations add $migrationName -c PetDoctorContext -o Migrations