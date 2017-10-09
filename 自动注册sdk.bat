cd .\sdk
copy .\*.dll %windir%\system32
regsvr32 %windir%\system32\zkemkeeper.dll