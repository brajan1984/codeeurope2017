using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Prometheus;
using System.Reflection;

namespace PlanetExpressMetrics
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("PlanetExpress app start.");

            var metricServer = new MetricServer(port: 1234);
            metricServer.Start();


            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    new Delivery().Process();
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                }
                
            });

            var done = false;
            while (!done)
            {
                Task.Factory.StartNew(() =>
                {
                    Console.WriteLine();
                    Console.WriteLine("Good news everyone, [ESC] to stop.");

                    done = (Console.ReadKey().Key == ConsoleKey.Escape);
                }).Wait();
            }

            metricServer.Stop();

            Console.WriteLine("PlanetExpress app end");
        }
    }

    public class Delivery
    {
        private static readonly IEnumerable<string> Employees = new[] {"fry", "leela", "bender"};

        public void Process()
        {
            var rnd = new Random();
            var employee = Employees.ElementAt(rnd.Next(0, Employees.Count()));
            var deliveryTime = TimeSpan.FromHours(rnd.Next(12, 12000));
            
            Console.WriteLine($"Delivery made by {employee} in {deliveryTime.TotalDays} days.");

            Metrics.DeliveryCounter.Labels(employee).Inc();
            Metrics.DeliveryDuration.Labels(employee).Observe(deliveryTime.TotalDays);
        }
    }

    public static class Metrics
    {
        public static Summary DeliveryDuration = Prometheus.Metrics.CreateSummary("delivery_duration_days",
            "Delivery duration in days", "employee");

        public static Counter DeliveryCounter =
            Prometheus.Metrics.CreateCounter("delivery_count", "Number of deliveries made", "employee");
    }
}
