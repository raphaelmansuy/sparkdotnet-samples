spark-submit \
--class org.apache.spark.deploy.dotnet.DotnetRunner \
--master local \
./bin/Debug/net5.0/microsoft-spark-2-4_2.11-1.1.1.jar \
dotnet ./bin/Debug/net5.0/streaming-simpletcpserver.dll 