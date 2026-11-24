mkdir ~\bin\SeikoHelper\python
cp .\python\*.py ~\bin\SeikoHelper\python
dotnet publish SeikoHelper.csproj -r win-x64 -p:PublishSingleFile=true -p:SelfContained=true -o ~\bin\SeikoHelper
cd CreateEntryList

.\build.ps1
cd ..
