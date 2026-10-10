param($path)
$asm = [System.Reflection.Assembly]::LoadFrom($path)
$exported = $asm.ExportedTypes
foreach ($t in $exported) {
    if ($t.Name -like '*Pipeline*' -or $t.Name -like '*Resilience*') {
        Write-Host "TYPE: $($t.FullName)"
        foreach ($m in $t.GetMethods([System.Reflection.BindingFlags]::Public -bor [System.Reflection.BindingFlags]::Static -bor [System.Reflection.BindingFlags]::Instance)) {
            if ($m.DeclaringType -eq $t) {
                Write-Host "  METHOD: $($m.Name)"
            }
        }
    }
}