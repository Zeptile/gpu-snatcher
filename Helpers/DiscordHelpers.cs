using Discord;
using gpu_snatcher.Models;
using System;
using System.Collections.Generic;

namespace gpu_snatcher.Helpers
{
    public static class DiscordHelpers
    {
        public static IEnumerable<Embed> BuildEmbed(EndpointItem item)
        {
            var builder = new EmbedBuilder()
            {
                Title = item.Title,
                ThumbnailUrl = item.ImageUrl,
                Color = Color.Blue,
                Timestamp = DateTime.Now
            };

            builder.AddField("Available", $"{item.Available}");
            builder.AddField("Price", $"{item.Price}$");
            builder.AddField("Link", item.PageUrl);

            // (???) SendMessageAsync from Discord API takes IEnumerable Embed, but fails if there's more than one (???)
            return new List<Embed>() { builder.Build() };
        }
    }
}
