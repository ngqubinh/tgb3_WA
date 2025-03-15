# tgb3_WA
# How to run this project.
  * Prerequisites: The system must have MySQL Server, .NET 8 installed, and Entity Framework tools available.
  1. Clone the repository from https://github.com/ngqubinh/tgb3_WA
  2. Navigate to the directory where the repository is stored in the terminal.
  3. Run "dotnet ef migrations add InitMigration -o Data/Infrastructure --project ./Infrastructure --startup-project ./ShelkovyPut_Main" to initialize the init migration.
  4. Run "dotnet ef database update --project ./Infrastructure --startup-project ./ShelkovyPut_Main" to initial database.
  5. Run "dotnet run --project ./ShelkovyPut_Main" to run the project.

# Details of the project(diagrams, entities and images): https://docs.google.com/document/d/1Nghykyf2N23s0RCecqoiZ3c-ZcHbiiw94-xTnJSPGR0/edit?usp=sharing
