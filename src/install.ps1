# Ensures that the content files 'copy to output' property is set to always.
# Source derived from System.Data.SQLite project, http://system.data.sqlite.org/index.html/artifact/847a3997768174776a4e183769b94634dd1b5822

param($installPath, $toolsPath, $package, $project)

$platformNames = "x86", "x64"
$fileNames = "libtesseract304.dll", "liblept172.dll"
$propertyName = "CopyToOutputDirectory"

foreach($platformName in $platformNames) {
  $folder = $project.ProjectItems.Item($platformName)

  if ($folder -eq $null) {
    continue
  }

  foreach($fileName in $fileNames) {
	  $item = $folder.ProjectItems.Item($fileName)

	  if ($item -eq $null) {
		continue
	  }

	  $property = $item.Properties.Item($propertyName)

	  if ($property -eq $null) {
		continue
	  }

	  $property.Value = 1
  }
}