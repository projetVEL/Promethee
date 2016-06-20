using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Packages
{
    public class Program : PackageBase
    {

        int JetLag = 0;
        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {

            Task.Factory.StartNew(() =>
                {
                    var lastTime = DateTime.MinValue;
                    while (PackageHost.IsRunning)
                    {
                        if (DateTime.Now.Subtract(lastTime).TotalMilliseconds >= 2)
                        {
                            PackageHost.PushStateObject<DateStateOject>("Date", new DateStateOject()
                            {
                                dayOfTheWeek = DateTime.Now.AddHours(JetLag).DayOfWeek.ToString(),
                                day = DateTime.Now.AddHours(JetLag).Day,
                                month = DateTime.Now.AddHours(JetLag).Month,
                                year = DateTime.Now.AddHours(JetLag).Year,
                                hours = DateTime.Now.AddHours(JetLag).Hour,
                                minutes = DateTime.Now.AddHours(JetLag).Minute,
                                seconds = DateTime.Now.AddHours(JetLag).Second
                            });
                            lastTime = DateTime.Now;
                        }
                        Thread.Sleep(500);
                    }

                    PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);
                });
                }

        [StateObject]
        public class DateStateOject
        {
            public String dayOfTheWeek { get; set; }
            public int day { get; set; }
            public int month { get; set; }
            public int year { get; set; }
            public int hours { get; set; }
            public int minutes { get; set; }
            public int seconds { get; set; }

        }

        [MessageCallback(Key = "Fuseau")]
        void changeUTC(int utc)
        {
            JetLag = utc;
            PackageHost.WriteInfo(utc.ToString());
        }
    }
}
