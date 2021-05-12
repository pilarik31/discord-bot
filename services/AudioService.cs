using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;

namespace ModeratorBot.Services
{
    ///
    public class AudioService
    {
        private LogService _logService;

        private readonly ConcurrentDictionary<ulong, IAudioClient> _audioClients = new ConcurrentDictionary<ulong, IAudioClient>();

        ///
        public AudioService(LogService logService)
        {
            _logService = logService;
        }
        
        // Joins the voice channel of the target.
        // Adds a new client to the ConcurrentDictionary.
        ///
        public async Task JoinAudioAsync(IGuild guild, IVoiceChannel target)
        {
            var audioClient = await target.ConnectAsync();
            if (_audioClients.TryAdd(guild.Id, audioClient))
            {
                Console.WriteLine("connected");
            }
        }

        ///
        public async Task LeaveAudioAsync(IGuild guild)
        {
            IAudioClient client;
            if (_audioClients.TryRemove(guild.Id, out client))
            {
                await client.StopAsync();
                //await Log(LogSeverity.Info, $"Disconnected from voice on {guild.Name}.");
            }
        }

        ///
        public async Task SendAudioAsync(IGuild guild, IMessageChannel channel, string path)
        {
            string filePath = $"sounds/{path}.mp3";
            if (!File.Exists(filePath))
            {
                await channel.SendMessageAsync("File does not exist.");
                return;
            }
            IAudioClient client;
            if (_audioClients.TryGetValue(guild.Id, out client))
            {
                //await Log(LogSeverity.Debug, $"Starting playback of {path} in {guild.Name}");
                var output = CreateStream(filePath).StandardOutput.BaseStream;
                var stream = client.CreatePCMStream(AudioApplication.Music);
                await output.CopyToAsync(stream);
                await stream.FlushAsync().ConfigureAwait(false);
            }
    }


        ///
        private Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }
    }
}