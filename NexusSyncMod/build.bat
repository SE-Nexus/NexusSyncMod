@echo off
if "%~1"=="" goto error

set output_folder=%appdata%\SpaceEngineers\Mods\%~1
echo output: %output_folder%
if not exist "%output_folder%" (
	md "%output_folder%"
)
if exist Data (
	robocopy Data "%output_folder%/Data" /s /purge /njh /njs /np
)
if exist Models (
	robocopy Models "%output_folder%/Models" /s /purge /njh /njs /np
)
if exist Texures (
	robocopy Textures "%output_folder%/Textures" /s /purge /njh /njs /np
)
if exist Audio (
	robocopy Audio "%output_folder%/Audio" /s /purge /njh /njs /np
)
if exist metadata.mod (
	xcopy metadata.mod "%output_folder%" /d /y
)
if exist modinfo.sbmi (
	xcopy modinfo.sbmi "%output_folder%" /d /y
)
goto end

:error
echo Invalid parameter

:end