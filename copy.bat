copy "Source\KerbalScienceInnovation\obj\Release\KerbalScienceInnovation.dll"  "GameData\KSI\Plugins" /Y

(robocopy "GameData\KSI" "..\KSP\GameData\KSI"   /E /V)
(robocopy "GameData\ContractPacks" "..\KSP\GameData\ContractPacks"   /E /V) 

(robocopy "GameData\KSI" "..\KSP en\GameData\KSI"   /E /V) 
(robocopy "GameData\ContractPacks" "..\KSP\GameData\ContractPacks"   /E /V) 

(robocopy "GameData\KSI" "..\KSP es\GameData\KSI"   /E /V) 
(robocopy "GameData\ContractPacks" "..\KSP\GameData\ContractPacks"   /E /V) 

(robocopy "GameData\KSI" "..\KSP ru\GameData\KSI"   /E /V) 
(robocopy "GameData\ContractPacks" "..\KSP\GameData\ContractPacks"   /E /V) 

(robocopy "GameData\KSI" "..\KSP jp\GameData\KSI"   /E /V) 
(robocopy "GameData\ContractPacks" "..\KSP\GameData\ContractPacks"   /E /V) 

(robocopy "GameData\KSI" "..\KSP zh\GameData\KSI"   /E /V)
(robocopy "GameData\ContractPacks" "..\KSP\GameData\ContractPacks"   /E /V)

pause
