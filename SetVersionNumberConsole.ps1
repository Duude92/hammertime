param (
    [string]$ver
)

if (-not $ver) {
    Write-Host "Error: Version number is required." -ForegroundColor Red
    exit 1
}
$rep1 = '[assembly: AssemblyVersion("' + $ver + '")]'
$rep2 = '[assembly: AssemblyFileVersion("' + $ver + '")]'
foreach ($file in Get-ChildItem -Filter AssemblyInfo.cs -Recurse) {
    $content = (Get-Content -Encoding UTF8 $file.PSPath)
    $content = Foreach-Object {$content -replace '\[assembly: AssemblyVersion\("[^"]*"\)\]', $rep1}
    $content = Foreach-Object {$content -replace '\[assembly: AssemblyFileVersion\("[^"]*"\)\]', $rep2}
    [IO.File]::WriteAllText($file.FullName, ($content -join "`r`n"))
}
Return 'Done'