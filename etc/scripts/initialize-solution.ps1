$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path

$jobs = @()

$jobs += Start-Job -Name "InstallLibs" -ScriptBlock {
    $ErrorActionPreference = "Stop"
    Set-Location (Join-Path $using:scriptRoot "../../")
    abp install-libs

    if ($LASTEXITCODE -ne 0) {
        throw "abp install-libs exited with code $LASTEXITCODE"
    }
}

$jobs += Start-Job -Name "DbMigrator" -ScriptBlock {
    $ErrorActionPreference = "Stop"
    Set-Location (Join-Path $using:scriptRoot "../../src/Andro.Backend.Reference.DbMigrator")
    dotnet run

    if ($LASTEXITCODE -ne 0) {
        throw "dotnet run (DbMigrator) exited with code $LASTEXITCODE"
    }
}

$jobs += Start-Job -Name "DevCert" -ScriptBlock {
    $ErrorActionPreference = "Stop"
    Set-Location (Join-Path $using:scriptRoot "../../src/Andro.Backend.Reference.Web")
    dotnet dev-certs https -v -ep openiddict.pfx -p 9bdd3596-d1df-4707-baec-1882f1d3fae2

    if ($LASTEXITCODE -ne 0) {
        throw "dotnet dev-certs exited with code $LASTEXITCODE"
    }
}

Wait-Job $jobs | Out-Null
$jobs | Receive-Job -ErrorAction SilentlyContinue

$failed = $jobs | Where-Object { $_.State -eq 'Failed' }
$hasError = $failed.Count -gt 0

if ($hasError) {
    foreach ($job in $failed) {
        [Console]::Error.WriteLine("Job '$($job.Name)' FAILED")
    }

    Remove-Job $jobs | Out-Null
    exit -1
}

Remove-Job $jobs | Out-Null
exit 0