### Tourplanner


## Using
# Programming tools
visual studio 2022


# frameworks
WPF framework
Nunit framework

# Collaboration tools
git


## Lessons learned:
add .gitignore first, clearing cache and fixing up the project was annoying enough for us to just setup a new git.
forgetting about intitializing the MainWindowViewModel is extremely painful because nothing works and you are looking for the bugs in the wrong place

## database

to create migrations use powershell

dotnet ef migrations add someMigrationName

to apply migrations build the project and then use

dotnet ef database update --project TourPlanner