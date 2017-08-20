copy "Source\KerbalScienceInnovation\obj\Release\KerbalScienceInnovation.dll"  "GameData\KSI\Plugins" /Y

(robocopy "GameData\KSI" "..\KSP\GameData\KSI"  /MIR /V)
(robocopy "GameData\ContractPacks" "..\KSP\GameData\ContractPacks" /MIR /V) 

(robocopy "GameData\KSI" "..\KSP en\GameData\KSI"  /MIR /V) 
(robocopy "GameData\ContractPacks" "..\KSP\GameData\ContractPacks"  /MIR /V) 

(robocopy "GameData\KSI" "..\KSP es\GameData\KSI"   /MIR /V) 
(robocopy "GameData\ContractPacks" "..\KSP\GameData\ContractPacks"  /MIR /V) 

(robocopy "GameData\KSI" "..\KSP ru\GameData\KSI"   /MIR /V) 
(robocopy "GameData\ContractPacks" "..\KSP\GameData\ContractPacks"  /MIR /V) 

(robocopy "GameData\KSI" "..\KSP jp\GameData\KSI"   /MIR /V) 
(robocopy "GameData\ContractPacks" "..\KSP\GameData\ContractPacks"  /MIR /V) 

(robocopy "GameData\KSI" "..\KSP zh\GameData\KSI"   /MIR /V)
(robocopy "GameData\ContractPacks" "..\KSP\GameData\ContractPacks"  /MIR /V)

pause
