$version = git tag --points-at;
Write-Host("Varsion {$version}")

$branch = git rev-parse --abbrev-ref HEAD
Write-Host("Branch {$branch}")

$latest_tag = git tag -l --merged master --sort='-*authordate' | Select-Object -Last 1
Write-Host("Latest tag {$latest_tag}")

$count = git rev-list --count master..$branch
Write-Host("Commit from master {$count}")


if($branch -contains '([\d].[\d].[\d])') {
  $bump = ($branch -split '([\d].[\d].[\d])')[1]
}

$version_parts = $latest_tag -split '([\d].[\d].[\d])'
$prefix = $version_parts[0]

$semver_parts = $version_parts[1] -split '\.'
$major = $semver_parts[0]
$minor = $semver_parts[1]
$patch = $semver_parts[2]

function Write-SemVer([int]$major, [int]$minor, [int]$patch, [string]$prefix, [string]$suffix) {
    $v = "$major.$minor.$patch"
    if (-not [string]::IsNullOrWhiteSpace($prefix)) {
        $v = "$prefix-$v"
    }
    if (-not [string]::IsNullOrWhiteSpace($suffix)) {
        $v = "$v-$suffix"
    }
    return $v
}

if ([string]::IsNullOrWhiteSpace($bump)) {
    if ([string]::IsNullOrWhiteSpace($version)) {
        switch ($branch) {
            'master' {
                $version = Write-Semver -major $major -minor ($minor + 1) -patch $patch
            }
            'hotfix' {
                $version = Write-Semver -major $major -minor $minor -patch ($patch + $count)
            }
            default {
                $version = Write-Semver -major $major -minor $minor -patch $patch -suffix "$branch.$count"
            }
        } 
    }
}
else {
  $version = $bump
}

Write-Host("New version {$version}")

Write-Host("##vso[task.setvariable variable=CAKEBOARD_VERSION]{$version}")
$env:CAKEBOARD_VERSION = $version
