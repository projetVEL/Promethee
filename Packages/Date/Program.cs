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

                    /* In order to optimise state object pushing, we will push each StateObject only when we have to
                    * coding this way we will only push a new data, when it will be new. 
                    * To illustrate : "I will push the year when it will be different, so I will push the year every year"
                    * We take just one risk, our code will need reviews if we can travel in time
                    * because we add 1 month and 1 year each time we need to, so if we go back in time we will experiment some bugs
                    * By the way, because the first time our variable lastTest will be equal to DateTime.Now, we will push once every stateObject
                    * then our conditionning will do it */
                    DateTime LastTest = DateTime.Now.AddHours(JetLag);
                    var lastTime = DateTime.MinValue;
                    //As I said, we push manually
                    PackageHost.PushStateObject("Seconds", DateTime.Now.AddHours(JetLag).Second);
                    PackageHost.PushStateObject("Minutes", DateTime.Now.AddHours(JetLag).Minute);
                    PackageHost.PushStateObject("Hours", DateTime.Now.AddHours(JetLag).Hour);
                    PackageHost.PushStateObject("Day", DateTime.Now.AddHours(JetLag).Day);
                    PackageHost.PushStateObject("Month", DateTime.Now.AddHours(JetLag).Month);
                    PackageHost.PushStateObject("Year", DateTime.Now.AddHours(JetLag).Year);
                    PackageHost.PushStateObject("DayOfWeek", DateTime.Now.AddHours(JetLag).DayOfWeek.ToString());

                    while (PackageHost.IsRunning)//Could be could to use this condition because we can need the date or the time at every time
                    {
                        //We loop it every 0.001s to have the second with a 0.001s accuracy
                        if (DateTime.Now.Subtract(lastTime).TotalMilliseconds >= 1)
                        {
                            //And we start pushing every StateObject when they are different to the previous one
                            if (LastTest.Second != DateTime.Now.AddHours(JetLag).Second)
                            {
                                PackageHost.PushStateObject("Seconds", DateTime.Now.AddHours(JetLag).Second);
                            }
                            if (LastTest.Minute != DateTime.Now.AddHours(JetLag).Minute)
                            {
                                PackageHost.PushStateObject("Minutes", DateTime.Now.AddHours(JetLag).Minute);
                            }
                            if (LastTest.Hour != DateTime.Now.AddHours(JetLag).Hour)
                            {
                                PackageHost.PushStateObject("Hours", DateTime.Now.AddHours(JetLag).Hour);
                            }
                            if (LastTest.Day != DateTime.Now.AddHours(JetLag).Day)
                            {
                                PackageHost.PushStateObject("Day", DateTime.Now.AddHours(JetLag).Day);
                            }

                            if (LastTest.Month != DateTime.Now.AddHours(JetLag).Month)
                            {
                                PackageHost.PushStateObject("Month", DateTime.Now.AddHours(JetLag).Month);
                            }
                            if (LastTest.Year != DateTime.Now.AddHours(JetLag).Year)
                            {
                                PackageHost.PushStateObject("Year", DateTime.Now.AddHours(JetLag).Year);

                            }
                            if (LastTest.DayOfWeek != DateTime.Now.AddHours(JetLag).DayOfWeek)
                            {
                                PackageHost.PushStateObject("DayOfWeek", DateTime.Now.AddHours(JetLag).DayOfWeek.ToString());
                            }
                            LastTest = DateTime.Now;
                            lastTime = DateTime.Now;
                        }
                        Thread.Sleep(1);
                    }
                });
        }


        //If you go on a trip you can change the hour because of the JetLag, called it "Fuseau" because it seems to be a good word even if it is french
        [MessageCallback(Key = "Fuseau")]
        void changeUTC(int utc)
        {
            JetLag = utc;
        }
    }
}
