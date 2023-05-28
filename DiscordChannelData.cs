using DSharpPlus;
using DSharpPlus.Entities;

namespace RobsDiscordBot
{
	public class DiscordChannelData
	{
        public static List<DiscordChannelData> DiscordChannelNames { get; } = new()
        {

            new DiscordChannelData { Name = "briefs-and-goals", ChannelType = ChannelType.Text},
            new DiscordChannelData { Name = "chat", ChannelType = ChannelType.Text},
            new DiscordChannelData { Name = "vod-upload", ChannelType = ChannelType.GuildForum},
            new DiscordChannelData { Name = "vc", ChannelType = ChannelType.Voice}
 
        };

        public string? Name { get; set; }
		public ChannelType ChannelType { get; set; }
		public DiscordChannel? Parent { get; set; }

	}


}

