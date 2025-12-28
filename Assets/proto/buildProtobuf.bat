mkdir output\csharp
mkdir output\cpp

protoc -I=. --csharp_out=output\csharp *.proto
protoc -I=. --cpp_out=output\cpp *.proto
