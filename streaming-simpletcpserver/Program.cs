using System;
using Microsoft.Spark.Sql;
using Microsoft.Spark.Sql.Streaming;
using static Microsoft.Spark.Sql.Functions;

namespace streaming
{

  class Program
  {
    static void Main(string[] args)
    {
      var hostname = "localhost";
      var port = 65001;


      var windowDuration = "30 seconds";
      var slideDuration = "10 seconds";

      SparkSession spark = SparkSession
           .Builder()
           .AppName("StructuredNetworkWordCountWindowed")
           .GetOrCreate();


      spark.SparkContext.SetLogLevel("warn");


      DataFrame lines = spark
          .ReadStream()
          .Format("socket")
          .Option("host", hostname)
          .Option("port", port)
          .Load();


      var df = lines
             .WithColumn("tab", Split(Col("value"), ";"))
             .WithColumn("date", ToDate(Column("tab").GetItem(0)))
             .WithColumn("var", Column("tab").GetItem(1))
             .WithColumn("consumption", Column("tab").GetItem(2));

      var windowedCounts = df
          .GroupBy(Window(Col("date"), windowDuration, slideDuration),
             Col("var"))
          .Count()
          .OrderBy(Desc("window"));


      var query = windowedCounts
            .WriteStream()
            .OutputMode("complete")
            .Format("console")
            .Option("truncate", false)
            .OutputMode(OutputMode.Complete)
            .Start();

      query.AwaitTermination();


    }
  }
}
