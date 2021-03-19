

run:
	DOTNET_CLI_TELEMETRY_OPTOUT=1 ASPNETCORE_ENVIRONMENT=Development 		     \
		dotnet watch --project GoodNight.Service.Api run --launch-profile Development

build:
	DOTNET_CLI_TELEMETRY_OPTOUT=1 ASPNETCORE_ENVIRONMENT=Production dotnet build

# run like: make test filter=SceneTest
test:
	DOTNET_CLI_TELEMETRY_OPTOUT=1 ASPNETCORE_ENVIRONMENT=Development          \
		dotnet test --nologo $(if $(filter),--filter FullyQualifiedName~$(filter),)


test-list:
	DOTNET_CLI_TELEMETRY_OPTOUT=1 ASPNETCORE_ENVIRONMENT=Development          \
		dotnet test --nologo -t -v q


clean:
	DOTNET_CLI_TELEMETRY_OPTOUT=1 ASPNETCORE_ENVIRONMENT=Development          \
		dotnet clean --nologo -v q


test-perf:
	DOTNET_CLI_TELEMETRY_OPTOUT=1 CORECLR_ENABLE_PROFILING=1 COMPlus_PerfMapEnabled=1 \
	   COMPlus_EnableEventLog=1 COMPlus_ZapDisable=1     \
	   dotnet test --nologo --no-build
