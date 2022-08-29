using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using ModeratorBot.Services;

namespace ModeratorBot.Modules
{
    /// <summary>
    /// Command module for Music.
    /// </summary>
    public class Voice : ModuleBase<SocketCommandContext>
    {
        private Storage Storage { get; set; }
        private AudioService _audioService;
        private LogService Log { get; set; }

        ///
        public Voice(AudioService service)
        {
            _audioService = service;
        }
        ///
        [Command("join", RunMode = RunMode.Async)]
        [Summary("Joins the voice channel.")]
        public async Task JoinChannel()
        {
            await _audioService.JoinAudioAsync(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
        }

        ///
        [Command("leave")]
        [Alias("fuckoff", "fuck off")]
        [Summary("Kick the bot from the voice channel")]
        public async Task LeaveChannel(IVoiceChannel channel = null)
        {
            await _audioService.LeaveAudioAsync(Context.Guild);
        }
        ///
        [Command("play", RunMode = RunMode.Async)]
        [Remarks("play [index]")]
        [Summary("Plays a song by local path.")]
        public async Task PlayVoiceChannel([Remainder] string song)
        {
            await _audioService.SendAudioAsync(Context.Guild, Context.Channel, song);
        }

    }
}