#!/usr/bin/env pwsh


dotnet tool uninstall grate -g
dotnet tool install grate -g --version 1.3.2
#dotnet tool install grate -g --version 1.4.0


grate `
--files=.\db `
--env=Local `
--connstring="Server=(localdb)\MSSQLLocalDB;Integrated Security=true;Database=grate_test" `
--version=1.0 `
--silent `
--drop