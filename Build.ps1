param(
	[string[]]$projects = @(
	    "Tesseract.Net20\Tesseract.Net20.csproj",
		"Tesseract.Net20\Tesseract.Net20.csproj", # 3.5
		"Tesseract.Net40\Tesseract.Net40.csproj",
		"Tesseract.Net45\Tesseract.Net45.csproj",
		"Tesseract.Net45\Tesseract.Net45.csproj"  # 4.5.1
	),
    [string[]]$platforms = @(
        "x86"
    ),
    [string[]]$targetFrameworks = @(
        "v2.0", 
        "v3.5", 
        "v4.0",
        "v4.5", 
        "v4.5.1"
    ),
    [string]$packageVersion = $null,
    [string]$config = "Release",
    [string]$target = "Rebuild",
    [string]$verbosity = "Minimal",

    [bool]$clean = $true
)

# Initialization
$rootFolder = Split-Path -parent $script:MyInvocation.MyCommand.Definition
. $rootFolder\myget.include.ps1

# Avoid clean?
if($clean) { MyGet-Build-Clean $rootFolder }

# Build folders
$outputFolder = Join-Path $rootFolder "bin"

# Myget
$packageVersion = MyGet-Package-Version $packageVersion
$nugetExe = MyGet-NugetExe-Path
$nuspec = Join-Path $rootFolder "Tesseract.nuspec"

# Type of framework to use
$useFramework = @(
    "v2.0",
    "v3.5",
    "v4.0",
    "v4.5",
    "v4.5.1"
)

$i = 0

$projects | ForEach-Object {

    $project = $_
    $targetFramework = $useFramework[$i]
  
    $platforms | ForEach-Object {
        $platform = $_

        MyGet-Build-Project -rootFolder $rootFolder `
			-project $project `
            -outputFolder $outputFolder `
            -config $config `
            -target $target `
            -targetFrameworks $targetFramework `
            -platform $platform `
            -MSBuildCustomProperties "/property:AllowUnsafeBlocks=true"
			
    }

    $i++
}

$platforms | ForEach-Object {
    $platform = $_
	
	$buildOutputFolder = Join-Path $outputFolder "$platform\$config"
	
    MyGet-Build-Nupkg -project $project `
        -rootFolder $rootFolder `
        -outputFolder $buildOutputFolder `
        -config $config `
        -version $packageVersion `
        -nuspec $nuspec `
        -platform $platform
}
