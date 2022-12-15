using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AottgBotLib.Commands;
using AottgBotLib.Commands.Modules;
using AottgBotLib.Logic;
using Photon.Realtime;
using AottgBotLib.Handlers;

namespace AottgBotLib.Example
{
  public class Program
  {
    public static async Task Main(string[] args)
    {
      Log.LogMethod = (str, level) => { Console.WriteLine($"[{level}] {str}"); };


      //Edit this to change room name
      string roomName = "ExampleBot";
      //Amount of maximum players
      int maxPlayers = 2;

      //Initializing client
      var client = new HostBotClient("HostBot");

      client.GreetingMessage = "Welcome to test of a lobby bot that I creating in hopes of running a tournament";

      //Configurin chat commands for bot
      CommandHandler handler = client.UseCommands((config) =>
      {
        config.Prefix = "!"; //Sets prefix for triggering commands
      });
      handler.RegisterModule<RestartCommandModule>(); //Includes restart module

      //Creating handler for Chat RPC
      client.RPCHandler.AddChatCallback((id, sender, content) =>
      {
        sender = sender.RemoveAll();
        content = content.RemoveAll();
        Console.WriteLine($"[{id}] {sender}: {content}"); //In this case chat message just outputs into console
      });

      //Assigning game logic type
      client.LogicType = typeof(EmptyLogic);

      //Region should be assigned before connection
      client.Region = PhotonRegion.Europe;
      await client.ConnectToMasterAsync();

      //Message to ensure connection
      Console.WriteLine("Connected");

      IReadOnlyList<RoomInfo> list = client.RoomList;

      //Showcase that we actually can see rooms in lobby
      foreach (RoomInfo room in list)
      {
        Console.WriteLine(room.Name);
      }

      //Properties of room to create
      var roomCreation = new RoomCreationInfo()
      {
        Name = roomName,
        Daylight = Daylight.Day,
        Difficulity = MapDifficulity.Normal,
        Map = MapName.ForestIII,
        ServerTime = 99999
      };

      //Finally creating room
      bool connected = await client.CreateRoomAsync(roomCreation, maxPlayers);

      if (connected)
      {
        Console.WriteLine("Connected to created room.");
      }
      //Bot is running now. Feel free to join room and check it yourself.
      //Print exit to close the application
      while (true)
      {
        Console.Write("Enter command: ");
        string line = Console.ReadLine();

        if (line == "exit")
        {
          client.Disconnect();
          Environment.Exit(0);
          break;
        }
      }
    }
  }
}
