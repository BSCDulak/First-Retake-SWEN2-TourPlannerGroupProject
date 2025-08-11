### Tourplanner


## Using
# Programming tools
visual studio 2022


# frameworks
WPF framework
Nunit framework
microsoft.entitiy.frameworkcore
(for migrations)
microsoft.extention.configuration
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
to create migrations use powershell
```shell
dotnet ef migrations add someMigrationName
```
to apply migrations build the project and then use
```shell
dotnet ef database update --project TourPlanner
```
