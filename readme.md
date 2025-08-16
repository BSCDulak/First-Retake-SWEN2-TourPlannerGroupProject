### Tourplanner


## Using
# Programming tools
visual studio 2022


# frameworks
WPF framework
Nunit framework
Microsoft.EntityFrameworkCore
(for migrations)
Npgsql.EntityFrameworkCore.PostgreSQL
(for postgres database things)
microsoft.extention
(so we can use appsettings.json)

# Collaboration tools
git


## Lessons learned:
add .gitignore first, clearing cache and fixing up the project was annoying enough for us to just setup a new git.
forgetting about intitializing the MainWindowViewModel is extremely painful because nothing works and you are looking for the bugs in the wrong place

## database
to be able to use dotnet commands dotnet tools need to be installed.
```shell
dotnet tool install --global dotnet-ef
```
to create migrations (when you have changed your model in some way and need to bring that change to the database) use powershell
```shell
dotnet ef migrations add someMigrationName --project TourPlanner
```
to apply migrations build the project and then use
```shell
dotnet ef database update --project TourPlanner
```
