using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TheLeftExit.Growtopia.ObjectModel;

using static TheLeftExit.Growtopia.GameWindow;

namespace TheLeftExit.TeslaX
{
    public partial class TeslaBot
    {
        public WorldTile GetTileAhead() => BlockAhead(game.App.GameLogicComponent.NetAvatar, game.App.GameLogicComponent.World.WorldTileMap).Tile;

        public void Debug(Action<String> log, CancellationToken token)
        {
            NetAvatar netAvatar = game.App.GameLogicComponent.NetAvatar;
            WorldTileMap worldTileMap = game.App.GameLogicComponent.World.WorldTileMap;
            
            while (!token.IsCancellationRequested)
            {
                Thread.Sleep(10);
                var info = BlockAhead(netAvatar, worldTileMap);
                if (info.Tile.IsEmpty)
                {
                    log("Detected: nothing.");
                    continue;
                }
                Int32 distance = PunchingDistance(netAvatar.Position.X, info.X, netAvatar.FacingLeft);
                log($"Detected: {WorldTileToString(info.Tile)} (distance: {distance}).");
            }
        }

        public void Break(Action<String> log, Func<WorldTile, bool> condition, CancellationToken token)
        {
            NetAvatar netAvatar = game.App.GameLogicComponent.NetAvatar;
            WorldTileMap worldTileMap = game.App.GameLogicComponent.World.WorldTileMap;

            MovementManager movementManager = new(100, 150);
            PunchManager punchManager = new();

            while (!token.IsCancellationRequested)
            {
                var info = BlockAhead(netAvatar, worldTileMap);
                if (info.Tile.IsEmpty)
                {
                    //finished(netAvatar.Position.X, netAvatar.Position.Y);
                    log("Finished: no blocks in range");
                    break;
                }
                Int32 distance = PunchingDistance(netAvatar.Position.X, info.X, netAvatar.FacingLeft);
                if(distance > Range)
                {
                    //finished(netAvatar.Position.X, netAvatar.Position.Y);
                    log("Finished: no blocks in range");
                    break;
                }
                if (!condition(info.Tile))
                {
                    log("Finished: target does not match the condition.");
                    break;
                }

                bool? toMove = movementManager.Update(distance > TargetDistance);
                if (toMove.HasValue)
                    window.SendKey(netAvatar.FacingLeft ? LeftKey : RightKey, toMove.Value);
                bool? toPunch = punchManager.Update();
                if (toPunch.HasValue)
                    window.SendKey(PunchKey, toPunch.Value);
            }

            window.SendKey(LeftKey, false);
            window.SendKey(RightKey, false);
            window.SendKey(PunchKey, false);
        }
        public void posit()
        {
            NetAvatar netAvatar = game.App.GameLogicComponent.NetAvatar;
            Thread.Sleep(500);
            int xpos = netAvatar.Position.X / 32;
            int ypos = netAvatar.Position.Y / 32;
            Console.Clear();
            Console.WriteLine("Current position is: " + netAvatar.Position.X + ", " + netAvatar.Position.Y);
            Console.WriteLine("Current grid position is: " + xpos + ", " + ypos);
        }

        public void finished(int x, int y)
        {
            var p = Process.GetProcessesByName("Growtopia").First(); //Defines the Growtopia process
            IntPtr wh = p.MainWindowHandle;
            WorldCamera worldcamera = game.App.GameLogicComponent.WorldRenderer.WorldCamera;
            NetAvatar netAvatar = game.App.GameLogicComponent.NetAvatar;
            if (0 < x && x < 160)
            {
            }
            if (3030 < x && x < 3180)
            {
            }
        }
    }
}
