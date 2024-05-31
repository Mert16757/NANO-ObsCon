del "E:\CAPTURAS NINA\FWHM\FWHM.old"
echo OFF
FOR /F "tokens=2 " %%g IN ('siril --version') do (SET version=%%g)
set ext=fits
set Folder="E:\CAPTURAS NINA\FWHM"
(
echo requires %version%
echo setext %ext%
echo cd %Folder%
echo load FWHM
echo findstar
echo close
) | "C:\Program Files\SiriL\bin\siril-cli.exe" -s - 2>&1 | findstr (FWHM >%Folder%\fwhm.txt
