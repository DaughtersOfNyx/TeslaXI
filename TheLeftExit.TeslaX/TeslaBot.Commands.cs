using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TheLeftExit.Growtopia.ObjectModel;

using TheLeftExit.Growtopia.Native;
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
            var p = Process.GetProcessesByName("Growtopia").First(); //Defines the Growtopia process
            IntPtr wh = p.MainWindowHandle;

            NetAvatar netAvatar = game.App.GameLogicComponent.NetAvatar;
            WorldTileMap worldTileMap = game.App.GameLogicComponent.World.WorldTileMap;

            MovementManager movementManager = new(100, 150);
            PunchManager punchManager = new();

            while (!token.IsCancellationRequested)
            {
                var info = BlockAhead(netAvatar, worldTileMap);
                if (info.Tile.IsEmpty)
                {
                    log("Finished: no blocks in range");
                    this.rowEnd(netAvatar.Position.X, netAvatar.Position.Y);
                    break;
                }
                Int32 distance = PunchingDistance(netAvatar.Position.X, info.X, netAvatar.FacingLeft);
                if(distance > Range)
                {
                    log("Finished: no blocks in range");
                    this.rowEnd(netAvatar.Position.X, netAvatar.Position.Y);
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
            Thread.Sleep(2000);
            Console.Clear();
            Console.WriteLine("Current position is: " + netAvatar.Position.X + " , " + netAvatar.Position.Y);
        }
        public void rowEnd(int x, int y)
        {
            var p = Process.GetProcessesByName("Growtopia").First(); //Defines the Growtopia process
            IntPtr wh = p.MainWindowHandle;
            if (1250 < x && x < 1400)
            {
                window.SendKey(JumpKey, true);
                window.SendKey(LeftKey, true);
                window.SendKey(PunchKey, false);
                Thread.Sleep(10);
                window.SendKey(LeftKey, false);
                Thread.Sleep(190);
                window.SendKey(JumpKey, false);
                Thread.Sleep(1000);
                WorldTile tile = this.GetTileAhead();
                this.Break(x => wh.SetWindowText(x),
                    x => tile.Foreground != 0 && tile.Foreground == x.Foreground || tile.Background != 0 && tile.Background == x.Background,
                    CancellationToken.None); // 5990
            }
            if (23 < x && x < 160)
            {
                window.SendKey(JumpKey, true);
                window.SendKey(RightKey, true);
                window.SendKey(PunchKey, false);
                Thread.Sleep(10);
                window.SendKey(RightKey, false);
                Thread.Sleep(190);
                window.SendKey(JumpKey, false);
                Thread.Sleep(1000);
                WorldTile tile = this.GetTileAhead();
                this.Break(x => wh.SetWindowText(x),
                    x => tile.Foreground != 0 && tile.Foreground == x.Foreground || tile.Background != 0 && tile.Background == x.Background,
                    CancellationToken.None); // 5990
            }
        }
    }
}
