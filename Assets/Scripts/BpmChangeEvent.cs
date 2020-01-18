namespace StyleStar
{
    public class BpmChangeEvent
    {
        public double BPM { get; set; }
        public double StartBeat { get; set; }
        public double StartSeconds { get; set; }

        public BpmChangeEvent(double bpm, double beat)
        {
            BPM = bpm;
            StartBeat = beat;
        }
    }
}
