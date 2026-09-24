param(
    # only run the specified target framework
    [string]$Framework
)

$ErrorActionPreference = 'Stop'

# TUnit test projects are executables. They are run with 'dotnet run' for each target framework.
# Only run the frameworks that contain tests: the SmokeTest tests are compiled for .NET Framework only.
$projects = @(
    @{ Path = 'Tests\Tests.csproj' },
    @{ Path = 'PropertyChanged.Fody.Analyzer.Tests\PropertyChanged.Fody.Analyzer.Tests.csproj' },
    @{ Path = 'SmokeTest\SmokeTest.csproj'; Frameworks = @('net48') }
)

$failed = @()
foreach ($project in $projects) {
    $path = $project.Path
    $frameworks = (dotnet msbuild $path -getProperty:TargetFrameworks -p:Configuration=Release).Trim().Split(';')
    if ($frameworks.Count -eq 1 -and $frameworks[0] -eq '') {
        $frameworks = @((dotnet msbuild $path -getProperty:TargetFramework -p:Configuration=Release).Trim())
    }
    if ($project.Frameworks) {
        $frameworks = $frameworks | Where-Object { $_ -in $project.Frameworks }
    }
    if ($Framework) {
        $frameworks = $frameworks | Where-Object { $_ -eq $Framework }
    }
    foreach ($targetFramework in $frameworks) {
        Write-Host "Running $path ($targetFramework)"
        dotnet run --project $path -c Release -f $targetFramework --no-build
        if ($LASTEXITCODE -ne 0) {
            $failed += "$path ($targetFramework)"
        }
    }
}

if ($failed.Count -gt 0) {
    Write-Host "Failed test runs:`n$($failed -join "`n")"
    exit 1
}
