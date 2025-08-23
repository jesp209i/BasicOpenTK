// See https://aka.ms/new-console-template for more information
using Runner;

string title = "Hello Hest";
Console.Title = title;


using (Game game = new Game(title: title))
{
    game.Run();
}