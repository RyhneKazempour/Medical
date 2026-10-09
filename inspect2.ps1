param([string]$path)
Add-Type -AssemblyName System.Reflection
$asm = [System.Reflection.Assembly]::LoadFile($path)
try {
    $types = $asm.GetTypes()
} catch {
    $types = $asm.GetExportedTypes()
}
foreach ($t in $types) {
    if ($t.Name -match "Pipeline|Resilience|StrategyOptions") {
        Write-Host "TYPE: $($t.FullName)"
    }
}
