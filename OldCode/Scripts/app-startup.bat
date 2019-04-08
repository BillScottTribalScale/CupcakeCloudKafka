:: start up all the shit
:: first docker-compose
::  ss64.com/nt/color.html

echo standing up kafka in docker
CD C:\jh
cd ./kafka-waffle-stack
start cmd /k "title Docker Kafka & color e0 & docker-compose up"
pause
cd ../payday-services
:: CalculationEngine
echo Startup CalculationEngine
cd ./CalculationEngine/CalculationEngine.Api
start cmd /k "title Calc engine & color 04 & dotnet run"
pause
cd ../..
:: Aggregator
echo Startup Aggregator
cd ./Aggregator/Aggregator.Api
start cmd /k " title aggregator & color b0 & dotnet run"
pause
cd ../..
:: GenerateOutput
echo Startup GenerateOutput
cd ./GenerateOutput/GenerateOutput.Api
start cmd /k "title generate output & color 4a & dotnet run"
pause
cd ../..
:: FileProcessor
echo Startup FileProcessor
cd ./FileProcessor/FileProcessor.Api
start cmd /k "title file processor & color 5a & dotnet run"
pause
cd ../..
:: KafkaListener
echo Startup KafkaListener
cd ./KafkaListener/KafkaListener.Api
start cmd /k " title kafka listener & color 79 & dotnet run"
