# You can always find an updated version @ https://raw.github.com/peters/myget/master/myget.include.ps1

# Miscellaneous

function MyGet-Write-Diagnostic {
    param([string]$message)

    Write-Host
    Write-Host $message -ForegroundColor Green
    Write-Host
}

function MyGet-Die {
    param(
        [string]$message,
        [object[]]$output,
        [int]$exitCode = 1
    )

    if ($output) {
		Write-Output $output
		$message += ". See output above."
	}

	Write-Error "$message exitCode: $exitCode"
	exit $exitCode

}

function MyGet-Create-Folder {
    param(
        [string]$folder = $(throw "-folder is required.")
    )
     
    if(-not (Test-Path $folder)) {
        [System.IO.Directory]::CreateDirectory($folder)
    }
    
}

function MyGet-Grep {
    param(
        [string]$folder = $(throw "-folder is required."),
        [string]$pattern = $(throw "-pattern is required."),
        [bool]$recursive = $true
    )

    if($recursive) {
        return Get-ChildItem $folder -Recurse | Where-Object { $_.FullName -match $pattern } 
    }

    return Get-ChildItem $folder | Where-Object { $_.FullName -match $pattern } 
}

function MyGet-BuildRunner {

    $buildRunner = ""

    if(Test-Path env:BuildRunner) {
        $buildRunner = Get-Content env:BuildRunner
    }

    return $buildRunner.tolower()

}

function MyGet-Package-Version {
    param(
        $packageVersion = $(throw "-packageVersion is required.")
    )

    if(Test-Path env:PackageVersion) { 
        $packageVersion = Get-Content env:PackageVersion 
    }

    if($packageVersion -eq "") {
        MyGet-Die "Invalid package version"
    }

    return $packageVersion
}

function MyGet-NugetExe-Path {

    if(Test-Path env:NuGet) { 
        return Get-Content env:NuGet 
    }
    
    return "nuget.exe"
}

function MyGet-NunitExe-Path {
    
    if(Test-Path env:Nunit) { 
       return Get-Content env:NunitPath
    }
    
    return "nunit-console.exe"
}

function MyGet-XunitExe-Path {
    param(
        [ValidateSet("x86", "x64", "AnyCpu")]
        [string]$platform,
        [ValidateSet("v2.0","v3.5", "v4.0", "v4.5", "v4.5.1")]
        [string]$targetFramework
    )

    MyGet-Die "Not implemented. Please contribute a PR @ https://www.github/peters/myget"
}

function MyGet-Normalize-Paths {
    param(
        [string]$basePath,
        [string[]]$paths
    )

    if($paths -isnot [System.Array]) {
        return @()
    }

    $i = 0
    $paths | ForEach-Object {
        $paths[$i] = [System.IO.Path]::Combine($basePath, $paths[$i])
        $i++;
    }

    return $paths

}

# Build

function MyGet-Build-Success {

    MyGet-Write-Diagnostic "Build: Success"

    exit 0

}

function MyGet-Build-Clean {
	param(
	    [string]$rootFolder = $(throw "-rootFolder is required."),
        [string]$folders = "bin,obj"
    )

    MyGet-Write-Diagnostic "Build: Clean"

    Get-ChildItem $rootFolder -Include $folders -Recurse | ForEach-Object {
       Remove-Item $_.fullname -Force -Recurse 
    }

}

function MyGet-Build-Bootstrap {
    param(
        [string]$project = $(throw "-solutionFile is required.")
    )

    MyGet-Write-Diagnostic "Build: Bootstrap"

    $solutionFolder = [System.IO.Path]::GetDirectoryName($project)
    $nugetExe = MyGet-NugetExe-Path

    . $nugetExe config -Set Verbosity=quiet

    if($project -match ".sln$") {
        . $nugetExe restore $project -NonInteractive
    }

    MyGet-Grep $rootFolder -recursive $true -pattern ".packages.config$" | ForEach-Object {
        . $nugetExe restore $_.FullName -NonInteractive -SolutionDirectory $solutionFolder
    }

}

function MyGet-Build-Nupkg {
    # http://docs.nuget.org/docs/reference/command-line-reference#Pack_Command

    param(
        [string]$rootFolder = $(throw "-rootFolder is required."),
        [string]$outputFolder = $(throw "-outputFolder is required."),

        [ValidatePattern(".(sln|csproj)$")]
        [string]$project = $(throw "-project is required."),

        [string]$nuspec = "",

        [string]$config = $(throw "-config is required."),
        
        [ValidatePattern("^([0-9]+)\.([0-9]+)\.([0-9]+)(?:-([0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*))?(?:\+[0-9A-Za-z-]+)?$")]
        [string]$version,

        [ValidateSet("x86", "x64", "AnyCpu")]
        [string]$platform = $(throw "-platform is required."),

        [string]$nugetProperties = ""
    )
    
    if(-not (Test-Path $project)) {
        MyGet-Die "Could not find project: $project"
    }

    if($nuspec -eq "" -or (-not (Test-Path $nuspec))) {
        $nuspec = [System.IO.Path]::Combine($rootFolder, $project) -ireplace ".(sln|csproj)$", ".nuspec"
    }

    if(-not (Test-Path $nuspec)) {
        MyGet-Die "Could not find nuspec: $nuspec"
    }

    $projectName = [System.IO.Path]::GetFileName($project) -ireplace ".(sln|csproj)$", ""

    # Nuget
    $nugetCurrentFolder = [System.IO.Path]::GetDirectoryName($nuspec)
    $nugetExe = MyGet-NugetExe-Path
    $nugetProperties = @(
        "Configuration=$config",
        "Platform=$platform",
        "OutputFolder=$outputFolder",
        "NuspecFolder=$nugetCurrentFolder",
        "$nugetProperties"
    ) -join ";"

    MyGet-Write-Diagnostic "Nupkg: $projectName ($platform / $config)"
    
    . $nugetExe pack $nuspec -OutputDirectory $outputFolder -Symbols -NonInteractive `
        -Properties "$nugetProperties" -Version $version
    
    if($LASTEXITCODE -ne 0) {
        MyGet-Die "Build failed: $projectName" -exitCode $LASTEXITCODE
    }
    
    # Support multiple build runners
    switch -Exact (MyGet-BuildRunner) {
        "myget" {
                
            $mygetBuildFolder = Join-Path $rootFolder "Build"

            MyGet-Create-Folder $mygetBuildFolder

            MyGet-Grep $outputFolder -recursive $false -pattern ".nupkg$" | ForEach-Object {
                $filename = $_.Name
                $fullpath = $_.FullName
		
		cp $fullpath $mygetBuildFolder\$filename
            }

        }
    }

}

function MyGet-Build-Project {
    param(
        [string]$rootFolder = $(throw "-rootFolder is required."),
        [string]$outputFolder = $(throw "-outputFolder is required."),

        [ValidatePattern(".(sln|csproj)$")]
        [string]$project = $(throw "-project is required."),

        [string]$config = $(throw "-config is required."),

        [ValidateSet("rebuild", "build")]
        [string]$target = $(throw "-target is required."),

        [ValidatePattern("^([0-9]+)\.([0-9]+)\.([0-9]+)(?:-([0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*))?(?:\+[0-9A-Za-z-]+)?$")]
        [string]$version,

        [ValidateSet("v1.1", "v2.0", "v3.5", "v4.0", "v4.5", "v4.5.1")]
        [string[]]$targetFrameworks = $(throw "-targetFrameworks is required."),

        [ValidateSet("x86", "x64", "AnyCpu")]
        [string]$platform = $(throw "-platform is required."),

        [ValidateSet("quiet", "minimal", "normal", "detailed", "diagnostic")]
        [string]$verbosity = "minimal",

        [string]$MSBuildCustomProperties = ""
    )

    $nugetExe = MyGet-NugetExe-Path

    $projectOutputPath = Join-Path $outputFolder "$version\$platform\$config"
    $projectPath = [System.IO.Path]::Combine($rootFolder, $project)
    $projectName = [System.IO.Path]::GetFileName($projectPath) -ireplace ".(sln|csproj)$", ""

    MyGet-Create-Folder $outputFolder

    if(-Not (Test-Path $projectPath)) {
        MyGet-Die "Could not find project: $projectPath"
    }

    MyGet-Build-Bootstrap $projectPath

    $targetFrameworks | ForEach-Object {
        
        $targetFramework = $_
        $buildOutputFolder = Join-Path $projectOutputPath "$targetFramework"

        MyGet-Create-Folder $buildOutputFolder

        MyGet-Write-Diagnostic "Build: $projectName ($platform / $config - $targetFramework)"

        # By default copy build output to final output path
        $msbuildOutputFilename = Join-Path $buildOutputFolder "msbuild.log"
        switch -Exact (MyGet-BuildRunner) {
            "myget" {
                
                # Otherwise copy to root folder so that we can see the
                # actual build failure in MyGet web interface
                $msbuildOutputFilename = Join-Path $rootFolder "msbuild.log"

            }
        }

        # YOLO
        $msbuildPlatform = $platform
        if($msbuildPlatform -eq "AnyCpu") {
            $msbuildPlatform = "Any CPU"
        }

        # http://msdn.microsoft.com/en-us/library/vstudio/ms164311.aspx
        & "$(Get-Content env:windir)\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" `
            $projectPath `
            /target:$target `
            /property:Configuration=$config `
            /property:OutputPath=$buildOutputFolder `
            /property:TargetFrameworkVersion=$targetFramework `
            /property:Platform=$msbuildPlatform `
            /maxcpucount `
            /verbosity:$verbosity `
            /fileLogger `
            /fileLoggerParameters:LogFile=$msbuildOutputFilename `
            /nodeReuse:false `
            /nologo `
            $MSBuildCustomProperties `
        
        if($LASTEXITCODE -ne 0) {
            MyGet-Die "Build failed: $projectName ($Config - $targetFramework)" -exitCode $LASTEXITCODE
        }

    }

}

function MyGet-Build-Solution {
    param(
        [string]$rootFolder = $(throw "-rootFolder is required."),
        [string]$outputFolder = $(throw "-outputFolder is required."),

        [ValidatePattern(".sln$")]
        [string]$sln = $(throw "-sln is required."),

        [ValidatePattern("^([0-9]+)\.([0-9]+)\.([0-9]+)(?:-([0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*))?(?:\+[0-9A-Za-z-]+)?$")]
        [string]$version,

        [string]$config = $(throw "-config is required."),
        [string]$target = $(throw "-target is required."),

        [string]$verbosity = "quiet",

        [string[]]$projects = @(),
        [string[]]$targetFrameworks = $(throw "-targetFrameworks is required."),
        [string[]]$platforms = $(throw "-platforms is required."),

        [string[]]$excludeNupkgProjects = @(),

        [string]$nuspec = $null,

        [string]$MSBuildCustomProperties = ""
    )

    if(-not (Test-Path $sln)) {
        MyGet-Die "Could not find solution: $sln"
    }

    $nugetExe = MyGet-NugetExe-Path
    $projectName = [System.IO.Path]::GetFileName($sln) -ireplace ".sln$", ""
    
    $excludeNupkgProjects = MyGet-Normalize-Paths $rootFolder, $excludeNupkgProjects

    # Building a solution
    if($projects.Count -eq 0) {
        $projects = @($sln)
    # Building projects within a solution
    } else {
        $projects = MyGet-Normalize-Paths $rootFolder, $projects
    }
 
    $projects | ForEach-Object {

        $project = $_

        $platforms | ForEach-Object {

            $platform = $_
            $finalBuildOutputFolder = Join-Path $outputFolder "$version\$platform\$config"
        
            MyGet-Build-Project -rootFolder $solutionFolder -project $project -outputFolder $outputFolder `
                -target $target -config $config -targetFrameworks $targetFrameworks `
                -version $version -platform $platform -verbosity $verbosity
    
            if(-not ($excludeNupkgProjects -contains $project)) {
                MyGet-Build-Nupkg -rootFolder $rootFolder -project $project -nuspec $nuspec -outputFolder $finalBuildOutputFolder `
                    -config $config -version $version -platform $platform
            }

        }
        
    }
}

# Test runners

function MyGet-TestRunner-Nunit {
    MyGet-Die "Not implemented. Please contribute a PR @ https://www.github/peters/myget"
}

function MyGet-TestRunner-Xunit {
    MyGet-Die "Not implemented. Please contribute a PR @ https://www.github/peters/myget"
}
