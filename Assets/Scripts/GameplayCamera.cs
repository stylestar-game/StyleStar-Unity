using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using StyleStar;

public class GameplayCamera : MonoBehaviour
{

    List<GameObject> activeNotes;
    public float CullLocation;

    MusicManager musicManager;

    NoteCollection currentSongNotes;
    SongMetadata currentSongMeta;

    // Initialize things
    private void Awake()
    {
        musicManager = new MusicManager();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Load list of songs
        SongSelection.ImportSongs(Defines.SongFolder);
        var meta = SongSelection.Songlist[7];
        currentSongNotes = new NoteCollection(meta);
        currentSongMeta = currentSongNotes.ParseFile();

        musicManager.LoadSong(currentSongMeta.FilePath + currentSongMeta.SongFilename, currentSongMeta.BpmEvents);
        musicManager.Offset = currentSongMeta.PlaybackOffset * 1000;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.J))
        //    activeNotes.ForEach(x => x.transform.position -= new Vector3(0, 0, 0.5f));

        //var inactive = activeNotes.Where(x => x.transform.position.z < CullLocation);
        //for (int i = 0; i < inactive.Count(); i++)
        //{
        //    inactive.ElementAt(i).SetActive(false);
        //    activeNotes.Remove(inactive.ElementAt(i));
        //}

        if(Input.GetKeyDown(KeyCode.Return))
        {
            if (musicManager.IsPlaying)
                musicManager.Pause();
            else
                musicManager.Play();
        }

        // Draw notes
        if(currentSongNotes != null)
        {
            var currentBeat = musicManager.GetCurrentBeat();
            var motions = currentSongNotes.Motions.Where(p => p.BeatLocation > currentBeat - 6 && p.BeatLocation < currentBeat + 16);
            var holds = currentSongNotes.Holds.Where(p => p.StartNote.BeatLocation > currentBeat - 16 && p.StartNote.BeatLocation < currentBeat + 16);
            var notes = currentSongNotes.Steps.Where(p => p.BeatLocation > currentBeat - 6 && p.BeatLocation < currentBeat + 16);
            var marks = currentSongNotes.Markers.Where(p => p.BeatLocation > currentBeat - 6 && p.BeatLocation < currentBeat + 16);

            foreach (var mark in marks)
                mark.Draw(currentBeat);

            foreach (var motion in motions)
                motion.Draw(currentBeat);

            foreach (var hold in holds)
                hold.Draw(currentBeat);

            foreach (var note in notes)
                note.Draw(currentBeat);
        }
    }
}
