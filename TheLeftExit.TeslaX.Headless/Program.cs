﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

using TheLeftExit.Growtopia;
using TheLeftExit.Growtopia.ObjectModel;
using TheLeftExit.Growtopia.Native;
using TheLeftExit.TeslaX;
using System.Collections.Generic;
using System.Drawing;

using System.Windows.Input;

namespace TheLeftExit.TeslaX.Headless
{
    class Program
    {
        String itempath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Growtopia", "game", "tiles_page1.rttex"); //Currently does nothing

        String pickup = Path.Combine("C:", "pickup", "tiles_page1.bmp"); //Currently does nothing

        public static void Main(string[] args)
        {
            Console.WriteLine("TeslaXI console demo. Initializing..."); //Self Explanatory
            var p = Process.GetProcessesByName("Growtopia").First(); //Defines the Growtopia process
            IntPtr wh = p.MainWindowHandle;
            GrowtopiaGame g = new GrowtopiaGame(p.Id, 0xA04130);
            TeslaBot bot = new TeslaBot(p.Id);
            Console.WriteLine("TeslaBot instantiated. Please check Growtopia's window title for instructions.");
            Console.WriteLine("To properly close the app, press Ctrl+C while the bot isn't running.");
            wh.SetWindowText("TeslaXI is running. Press Ctrl+X to start breaking the block in front of you.");


            while (true)
            {
                while (true)
                {
                    //bot.posit();
                    if (VK.Control.IsKeyDown() && VK.X.IsKeyDown()) //Registers ctrl + x for autobreak
                        break;
                    if(VK.Control.IsKeyDown() && VK.C.IsKeyDown())
                    {
                        wh.SetWindowText("Growtopia");
                        Process.GetCurrentProcess().Kill();
                    }
                    Thread.Sleep(10);
                }

                WorldTile tile;

                try
                {
                    tile = bot.GetTileAhead();
                }
                catch (ProcessMemoryException e) //Checks if an error catches ig? Maybe daddy leftexit had some issues
                {
                    wh.SetWindowText(e.Message);
                    continue;
                }

                wh.SetWindowText($"Breaking: {bot.WorldTileToString(tile)}");
                
                bot.Break(x => wh.SetWindowText(x),
                    x => tile.Foreground != 0 && tile.Foreground == x.Foreground || tile.Background != 0 && tile.Background == x.Background,
                    CancellationToken.None); // 5990
                
            }
        }
    }
}