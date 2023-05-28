using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace RobsDiscordBot.Modules.Utility
{
    public class ResponseHelper
	{
		public static async void CreateInteractionResponseAsync(InteractionContext ctx, InteractionResponseType responseType, string message, bool ephemeral = false)
		{
			if (ephemeral)
			{
                await ctx.CreateResponseAsync(responseType, new DiscordInteractionResponseBuilder().WithContent(message).AsEphemeral());
                return;
            }
            await ctx.CreateResponseAsync(responseType, new DiscordInteractionResponseBuilder().WithContent(message));

        }

        public static async void EditInteractionResponseAsync(InteractionContext ctx, string message)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(message));
        }
    }
}

