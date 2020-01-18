using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;

namespace StyleStar
{
    public class MusicManager
    {
        private int streamHandle = -1;

        public bool IsPlaying { get { return Bass.BASS_ChannelIsActive(streamHandle) == BASSActive.BASS_ACTIVE_PLAYING; } }
        public bool IsFinished { get { return GetCurrentSec() >= (SongLengthSec - (Offset / 1000)); } }
        public long SongLengthBytes { get; private set; }
        public double SongLengthSec { get; private set; }

        // Offset in milliseconds
        public double Offset { get; set; }

        public MusicManager()
        {
            bool success = Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            if (!success)
                throw new Exception("BASS Library failed to initialize.");
        }

        ~MusicManager()
        {
            Bass.BASS_Free();
        }

        public bool LoadSong(string filename, List<BpmChangeEvent> bpmChanges)
        {
            streamHandle = Bass.BASS_StreamCreateFile(filename, 0, 0, BASSFlag.BASS_DEFAULT | BASSFlag.BASS_STREAM_PRESCAN);
            Globals.BpmEvents = bpmChanges;
            Globals.BpmEvents = Globals.BpmEvents.OrderBy(x => x.StartBeat).ToList();
            for (int i = 0; i < Globals.BpmEvents.Count; i++)
            {
                if (i > 0)
                {
                    Globals.BpmEvents[i].StartSeconds = (Globals.BpmEvents[i].StartBeat - Globals.BpmEvents[i - 1].StartBeat) / Globals.BpmEvents[i - 1].BPM * 60 + Globals.BpmEvents[i - 1].StartSeconds;
                }
            }

            //SongBpm = bpm;
            // Globals.CurrentBpm = Globals.BpmEvents[0].BPM;
            SongLengthBytes = Bass.BASS_ChannelGetLength(streamHandle);
            SongLengthSec = Bass.BASS_ChannelBytes2Seconds(streamHandle, SongLengthBytes);
            return streamHandle == 0 ? false : true;
        }

        public void Play()
        {
            Bass.BASS_ChannelPlay(streamHandle, false);
        }

        public void Pause()
        {
            Bass.BASS_ChannelPause(streamHandle);
        }

        public double GetCurrentSec()
        {
            var pos = Bass.BASS_ChannelGetPosition(streamHandle);
            return Bass.BASS_ChannelBytes2Seconds(streamHandle, pos) - (Offset / 1000);
        }

        public double GetCurrentBeat()
        {
            if (Globals.BpmEvents == null)
                return -1;

            double sec = GetCurrentSec();
            var evt = Globals.BpmEvents.Where(x => sec >= x.StartSeconds).LastOrDefault();
            if (evt == null)
                evt = Globals.BpmEvents[0];
            return (evt.BPM * (sec - evt.StartSeconds) / 60) + evt.StartBeat;
        }
    }
}
