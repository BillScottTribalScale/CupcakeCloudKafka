# create the testresults folder
$TestResultFolder = (Get-Location).Path + '\TestResults\'
New-Item -Path $TestResultFolder -ItemType Directory -Force

# create the initial file that coverlet needs for merging
$CoverletOutput = $TestResultFolder + 'coverage.json'
Set-Content -Path $CoverletOutput -Value "{}"
$cmd = "dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=json /p:MergeWith=$CoverletOutput /p:CoverletOutput=$CoverletOutput PayDayAll.sln  --logger=trx -r $TestResultFolder"
Invoke-Expression $cmd

#Delete Test Result Folder 
Remove-Item $TestResultFolder -Force -Recurse