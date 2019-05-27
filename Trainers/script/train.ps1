param([string] $run_id, [string] $brains_path, [string[]] $brains, [bool] $clean_board=$False)

$ml_agents_path = "C:\Users\juraj\Documents\_git\ml-agents"
$anaconda_env_path = "D:\Programs\Anaconda3\envs\ml-agents"
$tensorboard_port = 8008
$config_path = (Get-Location).tostring() + "\config"

Set-Location -Path $ml_agents_path

if ($clean_board)
	{
	$tensorboard = Get-NetTCPConnection -ErrorAction Ignore -LocalPort $tensorboard_port
	if ($tensorboard -ne $null)
	{
		Stop-Process -ErrorAction Ignore -Id $tensorboard.OwningProcess -Force
	}
	Remove-Item -Recurse -Force -ErrorAction Ignore "$ml_agents_path\summaries\*"
}

Invoke-expression "cmd /c start powershell -Command { $anaconda_env_path\Scripts\tensorboard --port=8008 --logdir=$ml_agents_path\summaries }"
Remove-Item -Recurse -Force -ErrorAction Ignore "$ml_agents_path\models\$run_id"
Invoke-Expression "$anaconda_env_path\Scripts\mlagents-learn $config_path\config.yaml --run-id=$run_id --train"

foreach ($brain in $brains)
{
	Remove-Item -Force -ErrorAction Ignore "$brains_path\$brain.nn*"
}

Start-Sleep -s 2

foreach ($brain in $brains)
{
	Copy-Item -Force "$ml_agents_path\models\$run_id-0\$brain.nn" -Destination "$brains_path\$brain.nn"
}

if (-Not $?)
{
	Write-Host ''
	Write-Host -NoNewLine 'Error occured...';
	$null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown');
}