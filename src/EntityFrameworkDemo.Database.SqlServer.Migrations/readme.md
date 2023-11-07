# EntityFrameworkDemo - Migrations

This project contains the migrations of the EF Core Context for the EntityFrameworkDemo. The migrations are therefore part of an external project and not part of the business logic or the main project.
[See EFCore - Using a Separate Migrations Project](https://learn.microsoft.com/ef/core/managing-schemas/migrations/projects?WT.mc_id=DT-MVP-5001507)

The advantage of external migrations is that they can be easily integrated into different DevOps scenarios, e.g. through appropriate bundle mechanisms.
Furthermore, migrations themselves have no effect on the business project - so there is no reason for them to be part of the business project.

The EFCore default setup is simple, but again, not necessarily suitable for the real world.

## Add a migration

Use always unique names with underlines, e.g. `Name_Here`.

```shell
dotnet ef migrations add Name_Here --project BenjaminAbt.EntityFrameworkDemo.Database.SqlServer.Migrations --startup-project BenjaminAbt.EntityFrameworkDemo.Database.SqlServer.Migrations
```

## Export the script

The [EFCore script export](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli&WT.mc_id=DT-MVP-5001507#idempotent-sql-scripts) is especially suitable for local development. The script can be imported simply by drag-and-drop e.g. in SQL Magement Studio, examined and then executed.
It includes all migrations through the `--idempotent` parameter.

```shell
dotnet ef migrations script --output ./.localdev/schema.sql --idempotent --project EntityFrameworkDemo.Database.SqlServer.Migrations/EntityFrameworkDemo.Database.SqlServer.Migrations.csproj --context DemoSqlServerDbContext
```

In contrast to the CLI command `dotnet ef database update` changes are therefore very transparent and no settings, like connection strings, are necessary in the migration project.

The disadvantage of the script in the context of DevOps is that additional tooling must be used, at least for SQL Server, which currently only exists in a stable variant for Windows (SQLDacPac Deployments).
Accordingly, a Windows Runner has to be used in Azure DevOps or with GitHub Actions, for example.

For DevOps scenarios, it is therefore better to use the bundle variant.

## Export the bundle

[EFCore Bundles](https://learn.microsoft.com/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli&WT.mc_id=DT-MVP-5001507#bundles) are a relatively new feature in EFCore and allow the creation of executable files that already have the tooling and all migrations integrated.

```shell
dotnet ef migrations bundle --output ./.localdev/schema-update.exe --idempotent --project EntityFrameworkDemo.Database.SqlServer.Migrations/EntityFrameworkDemo.Database.SqlServer.Migrations.csproj --context DemoSqlServerDbContext
```

This exe file can then be executed in the release process of the DevOps pipeline. 

```yaml
- task: AzureCLI@2
  displayName: Azure SQL Schema Deployment
  inputs:
    azureSubscription: ${{ parameters.azureConnectionName }}
    scriptType: pscore
    scriptLocation: inlineScript
    inlineScript: |
        ${{ parameters.sqlServerDatabaseBundlePath}}\schema-update.exe `
        --connection 'Server=${{ variables.SqlServer }};Database=${{ variables.SqlDatabase }};Authentication=Active Directory Default'
```

Note: if the target is an [Azure SQL database with Managed Identity](https://learn.microsoft.com/azure/active-directory/managed-identities-azure-resources/tutorial-windows-vm-access-sql?WT.mc_id=DT-MVP-5001507), then the execution must be done via the Azure CLI, not via PowerShell, Bash or the standard shell!
Otherwise the necessary authorization is missing.

Unfortunately, Bundles is also insufficiently documented in EFCore: it is very simple and easy to create and run the exe files for Windows; however, to date there is no official documentation for Bundles on Linux.
I have unfortunately not found a single simple working sample / tutorial / forum post for Linux until today. Super sad.