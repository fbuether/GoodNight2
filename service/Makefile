

run:
	DOTNET_CLI_TELEMETRY_OPTOUT=1 ASPNETCORE_ENVIRONMENT=Development 		     \
		dotnet run --project GoodNight.Service.Api --launch-profile Development

build:
	DOTNET_CLI_TELEMETRY_OPTOUT=1 ASPNETCORE_ENVIRONMENT=Production dotnet build

test:
	DOTNET_CLI_TELEMETRY_OPTOUT=1 ASPNETCORE_ENVIRONMENT=Development          \
		dotnet test --nologo

test-list:
	DOTNET_CLI_TELEMETRY_OPTOUT=1 ASPNETCORE_ENVIRONMENT=Development          \
		dotnet test --nologo -t -v q


clean:
	DOTNET_CLI_TELEMETRY_OPTOUT=1 ASPNETCORE_ENVIRONMENT=Development          \
		dotnet clean --nologo -v q
