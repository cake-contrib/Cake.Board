$branch = git rev-parse --abbrev-ref HEAD
$latest_tag = git tag -l --merged master --sort='-*authordate' | Select-Object -Last 1

$semver_parts = $latest_tag -split '([\d].[\d].[\d])'
major=${semver_parts[0]}
minor=${semver_parts[1]}
patch=${semver_parts[2]}
