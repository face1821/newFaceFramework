set LUBAN_DLL=%~dp0..\Luban\Luban.dll
set CONF_ROOT=%~dp0

dotnet %LUBAN_DLL% ^
    -t client ^
    -c cs-simple-json ^
    -d json ^
    --conf %CONF_ROOT%luban.conf ^
    -x outputCodeDir=%~dp0..\..\..\Assets\scripts\data\static ^
    -x outputDataDir=%~dp0..\..\..\Assets\resource\jsondata

pause
