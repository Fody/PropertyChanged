﻿param($installPath, $toolsPath, $package, $project)


function Update-FodyConfig($addinName, $project)
{
    $fodyWeaversPath = [System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($project.FullName), "FodyWeavers.xml")

    if (!(Test-Path ($fodyWeaversPath)))
    {
        return
    }   

    $xml = [xml](get-content $fodyWeaversPath)

    $weavers = $xml["Weavers"]
    $node = $weavers.SelectSingleNode($addinName)

    if ($node)
    {
        $weavers.RemoveChild($node)
    }

    $xml.Save($fodyWeaversPath)
}



Update-FodyConfig "PropertyChanged" $project