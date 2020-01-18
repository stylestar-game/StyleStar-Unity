using UnityEngine;

namespace StyleStar
{
    public class BeatMarker
    {
        public bool IsLoaded { get; private set; }
        public double BeatLocation { get; private set; }

        private GameObject noteObject;

        //private Model model;
        //private Matrix world;
        //private NoteTextureBase noteTexture;


        public BeatMarker(double beatLoc)
        {
            BeatLocation = beatLoc;
        }

        public void Draw(double currentBeat)
        {
            if (noteObject == null)
            {
                noteObject = Pools.BeatMarker.GetPooledObject();
                noteObject.transform.position = new Vector3(0, 0, -5);
                noteObject.SetActive(true);
            }

            var curDist = (float)Globals.GetDistAtBeat(currentBeat);
            var beatLoc = (float)Globals.GetDistAtBeat(BeatLocation);

            noteObject.transform.position = noteObject.transform.position.ModZ(beatLoc - curDist - Globals.BeatMarkerHeightOffset);
        }

        //public void PreloadTexture(UserSettings settings)
        //{
        //    if (noteTexture == null)
        //        noteTexture = new BeatMarkerTexture(this);
        //}

        //public void Draw(double currentBeat, Matrix view, Matrix projection)
        //{
        //    if (noteTexture == null)
        //        noteTexture = new BeatMarkerTexture(this);
        //    noteTexture.Draw(currentBeat, view, projection);
        //}
    }
}
