param($installPath, $toolsPath, $package, $project)


function RemoveForceProjectLevelHack($project)
{
    Write-Host "RemoveForceProjectLevelHack" 
	Foreach ($item in $project.ProjectItems) 
	{
		if ($item.Name -eq "Fody_ToBeDeleted.txt")
		{
			$item.Delete()
		}
	}
}

function Fix-ReferencesCopyLocal($package, $project)
{
    Write-Host "Fix-ReferencesCopyLocal $($package.Id)"
    $asms = $package.AssemblyReferences | %{$_.Name}

    foreach ($reference in $project.Object.References)
    {
        if ($asms -contains $reference.Name + ".dll")
        {
            if($reference.CopyLocal -eq $true)
            {
                $reference.CopyLocal = $false;
            }
        }
    }
}

function Set-NugetPackageRefAsDevelopmentDependency($package, $project)
{
	Write-Host "Set-NugetPackageRefAsDevelopmentDependency" 
    $packagesconfigPath = [System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($project.FullName), "packages.config")
	$packagesconfig = [xml](get-content $packagesconfigPath)
	$packagenode = $packagesconfig.SelectSingleNode("//package[@id=`'$($package.id)`']")
	$packagenode.SetAttribute('developmentDependency','true')
	$packagesconfig.Save($packagesconfigPath)
}

RemoveForceProjectLevelHack $project

Fix-ReferencesCopyLocal $package $project

Set-NugetPackageRefAsDevelopmentDependency $package $project