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
           Func<Column, Column> udfArray =
                    Udf<string, string[]>((str) => new string[] { str, $"{str} {str.Length}" });

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


            var linesWithTime = lines
                    .WithColumn("timestamp",CurrentTimestamp())
                    .WithColumn("DayOfTheWeek",DayOfYear(Col("timestamp")));



            var words = linesWithTime
                    .WithColumn("words",Split(Col("value")," "));

           var word = words.WithColumn("word",Explode(Col("words")));
           
            var windowedCounts = word
                .GroupBy(Window(Col("timestamp"), windowDuration, slideDuration),
                   Col("word"))
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
