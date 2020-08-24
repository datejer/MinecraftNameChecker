using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace NameChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime drop;
            string toDrop;
            
            Console.Title = "Minecraft Name Checker by ejer";
            Console.WriteLine("Please enter name:");
            string name = Console.ReadLine();
            
            HttpClient client = new HttpClient();

            try
            {
                JObject previousOwner = JObject.Parse(client.GetStringAsync($"https://api.mojang.com/users/profiles/minecraft/{name}?at={(DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 3196800)}").Result);
                
                JArray previousOwnerNames = JArray.Parse(client.GetStringAsync($"https://api.mojang.com/user/profiles/{previousOwner["id"]}/names").Result);

                List<int> index = new List<int>();

                for (int i = 0; i < previousOwnerNames.Count; i++)
                {
                    string previousName = (string)previousOwnerNames[i]["name"];
                    if (name.ToLower() == previousName.ToLower()) index.Add(i);
                }

                int last = index[index.Count - 1] + 1;

                drop = DateTimeOffset.FromUnixTimeMilliseconds((long)previousOwnerNames[last]["changedToAt"] + 3196800000).ToLocalTime().DateTime;

                while (true)
                {
                    toDrop = getHumanTimeFormatFromMilliseconds(DateTimeOffset
                        .FromUnixTimeMilliseconds((long) previousOwnerNames[last]["changedToAt"] + 3196800000 -
                                                  DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()).ToUnixTimeMilliseconds());
                    update(name, drop, toDrop);
                    Thread.Sleep(1000);
                }
            }
            catch
            {
                Console.ReadKey();
                return;
            }
        }

        public static void update(string name, DateTime drop, string toDrop)
        {
            Console.CursorVisible = false;
            Console.Clear();
            Console.Title = $"{name} - {toDrop}";
            Console.WriteLine($"    {name}");
            Console.WriteLine($"Time of Availability: {drop}");
            Console.WriteLine($"Time Remaining: {toDrop}");
        }

        public static string getHumanTimeFormatFromMilliseconds(long millisecondS)
        {
            string message = "";
            long milliseconds = millisecondS;
            
            if (milliseconds >= 1000)
            {
                int seconds = (int) (milliseconds / 1000) % 60;
                int minutes = (int) ((milliseconds / (1000 * 60)) % 60);
                int hours = (int) ((milliseconds / (1000 * 60 * 60)) % 24);
                int days = (int) (milliseconds / (1000 * 60 * 60 * 24));
                
                if ((days == 0) && (hours != 0))
                {
                    message = $"{hours}h {minutes}m {seconds}s";
                }
                else if ((hours == 0) && (minutes != 0))
                {
                    message = $"{minutes}m {seconds}s";
                }
                else if ((days == 0) && (hours == 0) && (minutes == 0))
                {
                    message = $"{seconds}s";
                }
                else
                {
                    message = $"{days}d {hours}h {minutes}m {seconds}s";
                }
            }
            else
            {
                message = "Less than a second.";
            }

            return message;
        }
    }
}