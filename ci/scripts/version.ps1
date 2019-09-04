$version = git tag --points-at;
$branch = git rev-parse --abbrev-ref HEAD
$latest_tag = git tag -l --merged master --sort='-*authordate' | Select-Object -Last 1
$count = git rev-list --count ^master..$branch

$bump = $branch -split '([\d].[\d].[\d])'

$version_parts = $latest_tag -split '([\d].[\d].[\d])'
$prefix = $version_parts[0]

$semver_parts = $version_parts[1] -split '\.'
$major = $semver_parts[0]
$minor = $semver_parts[1]
$patch = $semver_parts[2]

function Write-SemVer([int]$major, [int]$minor, [int]$patch, [string]$prefix, [string]$suffix) {
    $v = "$major.$minor.$patch"
    if (!([string]::IsNullOrWhiteSpace($prefix))) {
        $v = "$prefix-$v"
    }
    if (!([string]::IsNullOrWhiteSpace($suffix))) {
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
                $version = Write-Semver -major $major -minor $minor -patch ($patch) -suffix "$branch.$count"
            }
        } 
    }
}
else {
  $version = $bump
}

return $version
