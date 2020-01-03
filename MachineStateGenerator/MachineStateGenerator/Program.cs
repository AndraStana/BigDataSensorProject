using System;
using System.Data;
using System.Data.SqlClient;

namespace MachineStateGenerator
{
    class Program
    {
        private static float temp_upperlimit = 4;
        private static float temp_lowerlimit = 0;

        private static float hum_upperlimit = 60;
        private static float hum_lowerlimit = 20;

        //temperature increases, humidity decreases

        //intervalul ideal pt temperatura : 1-7 grade
        private static int temperature_lower_possible = -1;
        private static int temperature_upper_possible = 5;

        //intervalul pentru umiditate ??
        private static int humidity_lower_possible = 18;
        private static int humidity_upper_possible = 62;

        static void Main(string[] args)
        {
            GenerateData();
        }


        public static void GenerateData()
        {
            long partId = 0;
            long batchId = 1;
            int toolId = 0;

            var currentDate = DateTime.Now;
            string productionTimestamp;
            double temperature;
            double humidity;

            Random r = new Random();

            //while (partId < 401)
            while (partId < 10)

            {
                partId++; //different for each item
                batchId = batchId % 200 == 0 ? batchId + 1 : batchId; //increases at every 200 items

                currentDate = currentDate.AddSeconds(19);
                productionTimestamp = GetTimestamp(currentDate); //3 per minute

                temperature = r.Next(temperature_lower_possible, temperature_upper_possible);
                humidity = r.Next(humidity_lower_possible, humidity_upper_possible);

                toolId = r.Next(1, 2);

                bool temp_error = temperature < temp_lowerlimit || temperature > temp_upperlimit;
                bool hum_error = humidity < hum_lowerlimit || humidity > hum_upperlimit;
                if (temp_error && hum_error)
                {
                    //log error code 3
                }
                else if (temp_error)
                {
                    //log error 1
                }
                else if (hum_error)
                {
                    //log error 2
                }


                var refrigeratorData = new RefrigeratorData()
                {
                    PartId = partId,
                    BatchId = batchId,
                    ProductionTimestamp = productionTimestamp,
                    ToolId = toolId,
                    Temperature = temperature,
                    Humidity = humidity,
                    TemperatureLowerLimit = temp_upperlimit,
                    TemperatureUpperLimit = temp_lowerlimit,
                    HumidityLowerLimit = hum_upperlimit,
                    HumidityUpperLimit = hum_lowerlimit
                };

                InsertRecordInDb(refrigeratorData);

            }
        }

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmss");
        }


        public static void InsertRecordInDb(RefrigeratorData refrigeratorData)
        {

            string connetionString = null;
            string sql = null;

            connetionString = "Data Source=DESKTOP-A0UAI1B;Initial Catalog=RefrigeratorDb; Trusted_Connection=True;";

            sql = "insert into Refrigerators ([PartId], [BatchId], [ProductionTimestamp]," +
                " [ToolId], [Temperature] ,[Humidity] , [TemperatureLowerLimit], [TemperatureUpperLimit], [HumidityLowerLimit], [HumidityUpperLimit])" +
                " values(@PartId, @BatchId, @ProductionTimestamp, @ToolId, @Temperature, @Humidity," +
                "@TemperatureLowerLimit, @TemperatureUpperLimit, @HumidityLowerLimit, @HumidityUpperLimit)";

            using (SqlConnection cnn = new SqlConnection(connetionString))
            {
                try
                {
                    cnn.Open();

                    using (SqlCommand cmd = new SqlCommand(sql, cnn))
                    {
                        cmd.Parameters.Add("@PartId", SqlDbType.BigInt).Value = refrigeratorData.PartId;
                        cmd.Parameters.Add("@BatchId", SqlDbType.BigInt).Value = refrigeratorData.BatchId;
                        cmd.Parameters.Add("@ProductionTimestamp", SqlDbType.NVarChar).Value = refrigeratorData.ProductionTimestamp;
                        cmd.Parameters.Add("@ToolId", SqlDbType.BigInt).Value = refrigeratorData.ToolId;
                        cmd.Parameters.Add("@Temperature", SqlDbType.Float).Value = refrigeratorData.Temperature;
                        cmd.Parameters.Add("@Humidity", SqlDbType.Float).Value = refrigeratorData.Humidity;
                        cmd.Parameters.Add("@TemperatureLowerLimit", SqlDbType.Float).Value = refrigeratorData.TemperatureLowerLimit;
                        cmd.Parameters.Add("@TemperatureUpperLimit", SqlDbType.Float).Value = refrigeratorData.TemperatureUpperLimit;
                        cmd.Parameters.Add("@HumidityLowerLimit", SqlDbType.Float).Value = refrigeratorData.HumidityLowerLimit;
                        cmd.Parameters.Add("@HumidityUpperLimit", SqlDbType.Float).Value = refrigeratorData.HumidityUpperLimit;
                        int rowsAdded = cmd.ExecuteNonQuery();

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR:" + ex.Message);
                }
            }

        }
    }

    public class RefrigeratorData
    {
        public long PartId { get; set; }
        public long BatchId { get; set; }

        public string ProductionTimestamp { get; set; }

        public long ToolId { get; set; }

        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double TemperatureLowerLimit { get; set; }
        public double TemperatureUpperLimit { get; set; }

        public double HumidityLowerLimit { get; set; }
        public double HumidityUpperLimit { get; set; }



    }

   


}
