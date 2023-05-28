using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Serilog;
using RobsDiscordBot.Modules.Utility;

namespace RobsDiscordBot.Modules
{
    public class SlashCommands : ApplicationCommandModule
    {

        [SlashCommand("how-to-use-message", "Displays a message on how to use the bot and create their category")]
        public async Task HowToUse(InteractionContext ctx, [Option("channel-to-send-message-in","The channel in which to send the how to use message in")] DiscordChannel? channelToSendMessage = null)
        {
            var howToUseMessageEmbed = new DiscordEmbedBuilder()
                .WithFooter("Created by https://twitter.com/kekwYuki | Discord: yuki.#0700")
                .WithColor(DiscordColor.Aquamarine)
                .WithTitle("How to use this bot!")
                .AddField("/create-category", "This command takes in a discord user, so you can mention yourself (Using your actual @, not a nickname) and it will create a category and channels for you!", true)
                .Build();
            Log.Information("Created Embed");
         
            ResponseHelper.CreateInteractionResponseAsync(ctx, InteractionResponseType.DeferredChannelMessageWithSource, "Please wait!");
            if(channelToSendMessage != null)
            {
                Log.Debug("Need to send to a specific channel: \"{Name}\"", channelToSendMessage.Name);
                ResponseHelper.EditInteractionResponseAsync(ctx, "Thanks for waiting!");
                await channelToSendMessage.SendMessageAsync(howToUseMessageEmbed);
                Log.Information("Sent the embed");
            }

            channelToSendMessage = ctx.Channel;
            Log.Information("Sending embed to channel the command was executed in: \"{Name}\"",channelToSendMessage.Name);
            ResponseHelper.EditInteractionResponseAsync(ctx, "Thanks for waiting!");
            await channelToSendMessage.SendMessageAsync(howToUseMessageEmbed);
            Log.Information("Sent the embed");
        }

        [SlashCommand("greet", "A command to greet a user!")]
        public async Task GreetingSlashCommand(InteractionContext ctx)
        {
           ResponseHelper.CreateInteractionResponseAsync(ctx, InteractionResponseType.ChannelMessageWithSource, "Hello! Welcome to Rob's Coaching Server!");
        }

        readonly List<DiscordChannelData> discordChannelNames = DiscordChannelData.DiscordChannelNames;

        [SlashCommand("create-category", "Creates a category and related channels for a specific user")]
        public async Task CreateUserCategorySlashCommandWithDiscordUser(InteractionContext ctx, [Option("user", "user to create the category name for")] DiscordUser user)
        {
            var dartEmoji = DiscordEmoji.FromName(ctx.Client ,name: ":dart:");
            var parentChannel = await ctx.Guild.CreateChannelCategoryAsync($"{dartEmoji} {user.Username}");
            var robbfpsRole = ctx.Guild.GetRole(1103223017619857408);
            if (robbfpsRole is null) return;

            var discordMemberToAddToChannel = await ctx.Guild.GetMemberAsync(user.Id);

            Log.Information("Adding user into the channel: {members}", user);

            await parentChannel.AddOverwriteAsync(ctx.Guild.EveryoneRole, Permissions.None, Permissions.AccessChannels);
            await parentChannel.AddOverwriteAsync(discordMemberToAddToChannel, Permissions.AccessChannels, Permissions.None);
            await parentChannel.AddOverwriteAsync(robbfpsRole, Permissions.AccessChannels, Permissions.None);
       
            ResponseHelper.CreateInteractionResponseAsync(ctx, InteractionResponseType.DeferredChannelMessageWithSource, "Please wait...", true);

            CreateChannels(discordChannelNames, ctx, parentChannel);

            ResponseHelper.EditInteractionResponseAsync(ctx, "Thanks for waiting!");
        }

        private void CreateChannels(List<DiscordChannelData> listOfDiscordChannelData, InteractionContext ctx, DiscordChannel parentChannel)
        {
            listOfDiscordChannelData.ForEach(async x =>
            {
                Log.Information("Creating {Channel}: {Name} under the {Parent} category", x.ChannelType, x.Name, parentChannel.Name);
                await ctx.Guild.CreateChannelAsync(x.Name, x.ChannelType, parent: parentChannel);
                Log.Information("Done!");
            });
        }

        [SlashCommand("delete-category-or-channel", "delete the category and channels within for a specific user")]
        public async Task DeleteUserCategoryCommand(InteractionContext ctx, [Option("delete", "ID for the category you want to delete (also deletes the channels within it!)")] DiscordChannel categoryToDelete)
        {
            ResponseHelper.CreateInteractionResponseAsync(ctx, InteractionResponseType.DeferredChannelMessageWithSource, "Please wait...", true);
            var executingMember = ctx.Member;
            var robbfpsRole = ctx.Guild.GetRole(1103223017619857408);
            var doesMemberHaveRole = executingMember.Roles.Any(r => r == robbfpsRole);

            if (!doesMemberHaveRole)
            {
                ResponseHelper.EditInteractionResponseAsync(ctx, "You do not have the role to execute this command. If you're supposed to have that role, please DM yuki or robbfps for help");
                return;
            }
            Log.Information("User \"{user}\" does have role to execute command", executingMember.Username);

            Log.Information("Channel is category: {count}", categoryToDelete.IsCategory);
            if(categoryToDelete.IsCategory)
            {
                Log.Information("Children count for category is: {count}", categoryToDelete.Children.Count);
                foreach (DiscordChannel channel in categoryToDelete.Children)
                {
                    Log.Information("Deleted: {channel}", channel.Name);
                    await channel.DeleteAsync();
                }
                await categoryToDelete.DeleteAsync();
            } else
            {
                Log.Information("Deleted: {}", categoryToDelete.Name);
                await categoryToDelete.DeleteAsync();
            }

            var embed = new DiscordEmbedBuilder()
            {
                Title = "Rob's Discord Bot - Delete Categories/Channels",
                Description = categoryToDelete.IsCategory ? $"The category: \"{categoryToDelete.Name}\" and the channels within it have been deleted!" : $"The channel: \"{categoryToDelete.Name}\" has been deleted!",
                Color = DiscordColor.Turquoise
            };

            await ctx.Channel.SendMessageAsync(embed);
            ResponseHelper.EditInteractionResponseAsync(ctx, "Thanks for waiting!");
        }
    }
}

