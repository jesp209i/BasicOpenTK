// See https://aka.ms/new-console-template for more information
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Runner;

string title = "Hello Hest";
int windowsWidth = 1280;
int windowsHeight = 756;

Console.Title = title;

var options = new GameOptions();

using (Game game = new Game
    (GameWindowSettings.Default, 
    new NativeWindowSettings {
        Title = title,
        ClientSize = new Vector2i(windowsWidth, windowsHeight),
        WindowBorder = OpenTK.Windowing.Common.WindowBorder.Resizable,
        StartVisible = false,
        StartFocused = true,
        API = ContextAPI.OpenGL,
        Profile = ContextProfile.Core,
        APIVersion = new Version(3, 3)
    }, 
    options))
{
    game.Run();
}