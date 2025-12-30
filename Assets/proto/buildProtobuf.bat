mkdir output\csharp

protoc -I=. --csharp_out=output/csharp *.proto
protoc -I=. --cpp_out=../../DeltaNights-Server/GameEvent *.proto
