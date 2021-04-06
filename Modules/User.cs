using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;

namespace ModeratorBot.Modules
{
    /// <summary>
    /// Command module for common users.
    /// </summary>
    public class User : ModuleBase<SocketCommandContext>
    {
        private Storage _storage;

        /// <summary>
        /// Constructor.
        /// </summary>
        public User() {
            _storage = new Storage();
        }

        /// <summary>
        /// Returns info of the specified user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Command("userinfo")]
        [Summary("Returns info about the current user, or the user parameter, if one passed.")]
        [Alias("user", "whois")]
        public async Task UserInfoAsync(
		    [Summary("The (optional) user to get info from")]
		    SocketUser user = null)
	    {
		    var userInfo = user ?? Context.Client.CurrentUser;
		    await ReplyAsync($"{userInfo.Username}#{userInfo.Discriminator}");
	    }

        ///
        [Command("meme",RunMode = RunMode.Async)]
        [Alias("reddit")]
        [Summary("Random meme.")]
        public async Task Meme(string subreddit = null)
        {
            var client = new HttpClient();
            var result = await client.GetStringAsync($"https://reddit.com/r/{subreddit ?? "memes"}/random.json?limit=1");

            if (!result.StartsWith("["))
            {
                await Context.Channel.SendMessageAsync("This subreddit doesn't exist!");
                return;
            }

            JArray jArray = JArray.Parse(result);
            JObject jObject = JObject.Parse(jArray[0]["data"]["children"][0]["data"].ToString());

            if (jObject["over_18"].ToString() == "True" && !(Context.Channel as ITextChannel).IsNsfw)
            {
                await ReplyAsync("This subreddit contains NSFW content, while this is SFW channel.");
                return;
            }

            EmbedBuilder builder = new EmbedBuilder()
                .WithImageUrl(jObject["url"].ToString())
                .WithColor(new Color(33, 176, 252))
                .WithTitle(jObject["title"].ToString())
                .WithUrl("https://reddit.com" + jObject["permalink".ToString()])
                .WithFooter($"💬 {jObject["num_comments"]} ⬆️ {jObject["ups"]}");

            await ReplyAsync(embed: builder.Build());
        }
    }
}