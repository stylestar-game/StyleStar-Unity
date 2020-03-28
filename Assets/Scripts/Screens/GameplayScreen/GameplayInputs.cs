using StyleStar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayInputs : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Prevent inputs while the game is transitioning between screens
        if (GameState.TransitionState == TransitionState.EnteringLoadingScreen ||
            GameState.TransitionState == TransitionState.LeavingLoadScreen)
            return;

        // Normal Controls
        if (Input.GetButtonDown("Down"))
            GameState.ScrollSpeed = GameState.ScrollSpeed > 1.0 ? GameState.ScrollSpeed - 0.5 : 1.0;
        if (Input.GetButtonDown("Up"))
            GameState.ScrollSpeed += 0.5;
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (Globals.MusicManager.IsPlaying)
                Globals.MusicManager.Pause();
            else
                Globals.MusicManager.Play();
        }
        if(Input.GetButtonDown("Left"))
        {
            GameState.TransitionState = TransitionState.EnteringLoadingScreen;
            GameState.Destination = Mode.Results;
            Globals.CurrentNoteCollection.SongEnd = SongEndReason.Forfeit;
        }

        // Get times
        var currentBeat = Globals.MusicManager.GetCurrentBeat();
        var currentTime = Globals.GetSecAtBeat(currentBeat);

        // Build touch collection
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                var touch = Input.GetTouch(i);

                if (touch.fingerId == 0)    // Ignore fingerId = 0 because this is treated as a mouse click
                    continue;

                Debug.Log("Point: " + touch.fingerId.ToString() + "\tX: " + touch.position.x.ToString() + "\tY: " + touch.position.y.ToString());

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        // Add to touch collection
                        TouchCollection.Points[(uint)touch.fingerId] = new TouchPoint(currentBeat, (int)touch.position.x, (int)touch.position.y);
                        break;
                    case TouchPhase.Moved:
                        // Edit touch collection
                        TouchCollection.Points[(uint)touch.fingerId].Update(currentBeat, (int)touch.position.x, (int)touch.position.y);
                        break;
                    case TouchPhase.Stationary:
                        break;
                    case TouchPhase.Ended:
                        // Remove from touch collection
                        TouchCollection.RemoveID((uint)touch.fingerId);
                        break;
                    case TouchPhase.Canceled:
                        break;
                    default:
                        break;
                }
            }
        }

        // Process inputs
        var songNotes = Globals.CurrentNoteCollection;
        var stepList = songNotes.Steps.Where(x => Math.Abs(x.BeatLocation - currentBeat) < 2 && !x.HitResult.WasHit).ToList();
        stepList.Sort((x, y) => Math.Abs(x.BeatLocation - currentBeat).CompareTo(Math.Abs(y.BeatLocation - currentBeat)));

        var holdList = songNotes.Holds.Where(x =>
            Math.Abs(x.StartNote.BeatLocation - currentBeat) < 64 || (x.StartNote.BeatLocation < currentBeat && x.Notes.Last().BeatLocation > currentBeat)).ToList();

        var motionList = songNotes.Motions.Where(x => Math.Abs(x.BeatLocation - currentBeat) < 2 && !x.HitResult.WasHit).ToList();
        motionList.Sort((x, y) => Math.Abs(x.BeatLocation - currentBeat).CompareTo(Math.Abs(y.BeatLocation - currentBeat)));

        // Check if we've hit any steps
        foreach (var step in stepList)
        {
            // First check to see if they've passed the miss mark
            //var stepTimeMS = ((step.BeatLocation - currentBeat) * 60 / Globals.CurrentBpm);
            var stepTimeMS = Globals.GetSecAtBeat(step.BeatLocation) - currentTime;
            if (stepTimeMS < -NoteTiming.Bad)
            {
                step.HitResult.WasHit = true;   // Let everyone else know this note has been resolved
                step.HitResult.Difference = Timing.MissFlag;
                songNotes.AddToScore(NoteType.Step, step.HitResult.Difference);
            }
            else if (GameState.CurrentSettings.AutoSetting == Settings.AutoMode.Auto)
            {
                if (Math.Abs(stepTimeMS) < NoteTiming.AutoTolerance)
                {
                    step.HitResult.WasHit = true;
                    step.HitResult.Difference = 0;
                    //gradeCollection.Set(gameTime, step);
                    songNotes.AddToScore(NoteType.Step, step.HitResult.Difference);
                }
            }
            else if (TouchCollection.Points.Count > 0)
            {

                if (TouchCollection.CheckHit(step))
                {
                    //gradeCollection.Set(gameTime, step);
                    songNotes.AddToScore(NoteType.Step, step.HitResult.Difference);
                }
            }
        }

        // Check if we've hit or are still hitting any holds
        foreach (var hold in holdList)
        {
            // Check start note if necessary
            if (!hold.StartNote.HitResult.WasHit)
            {
                //var stepTimeMS = ((hold.StartNote.BeatLocation - currentBeat) * 60 / Globals.CurrentBpm);
                var stepTimeMS = Globals.GetSecAtBeat(hold.StartNote.BeatLocation) - currentTime;
                if (stepTimeMS < -NoteTiming.Bad)
                {
                    hold.StartNote.HitResult.WasHit = true; // Let everyone else know this note has been resolved
                    hold.StartNote.HitResult.Difference = Timing.MissFlag;
                    songNotes.AddToScore(NoteType.Hold, hold.StartNote.HitResult.Difference);
                }
                else if (GameState.CurrentSettings.AutoSetting == Settings.AutoMode.Auto)
                {
                    if (Math.Abs(stepTimeMS) < NoteTiming.AutoTolerance)
                    {
                        hold.StartNote.HitResult.WasHit = true;
                        hold.StartNote.HitResult.Difference = 0;
                        //gradeCollection.Set(gameTime, hold.StartNote);
                        songNotes.AddToScore(NoteType.Hold, hold.StartNote.HitResult.Difference);
                    }
                }
                else if (TouchCollection.Points.Count > 0)
                {
                    if (TouchCollection.CheckHit(hold.StartNote))
                    {
                        //gradeCollection.Set(gameTime, hold.StartNote);
                        songNotes.AddToScore(NoteType.Hold, hold.StartNote.HitResult.Difference);
                    }
                }
            }

            // Check any shuffles separately
            var shuffles = hold.Notes.Where(x => x.Type == NoteType.Shuffle && x.HitResult.WasHit == false);
            foreach (var shuffle in shuffles)
            {
                // Check window around shuffle and see if the foot is moving in the correct direction
                var stepTimeMS = Globals.GetSecAtBeat(shuffle.BeatLocation) - currentTime;
                if (stepTimeMS < -NoteTiming.Bad)
                {
                    shuffle.HitResult.WasHit = true; // Let everyone else know this note has been resolved
                    shuffle.HitResult.Difference = Timing.MissFlag;
                    songNotes.AddToScore(NoteType.Hold, shuffle.HitResult.Difference);
                }
                else if (GameState.CurrentSettings.AutoSetting == Settings.AutoMode.Auto)
                {
                    if (Math.Abs(stepTimeMS) < NoteTiming.AutoTolerance)
                    {
                        shuffle.HitResult.WasHit = true;
                        shuffle.HitResult.Difference = 0;
                        songNotes.AddToScore(NoteType.Hold, shuffle.HitResult.Difference);
                    }
                }
                else if (TouchCollection.Points.Count > 0)
                {
                    if (TouchCollection.CheckHit(shuffle))
                    {
                        songNotes.AddToScore(NoteType.Hold, shuffle.HitResult.Difference);
                    }
                }
            }

            // Let the note figure out itself whether it's being held and scoring
            var holdResult = hold.CheckHold(currentBeat);
            if (holdResult == HitState.Hit)
                songNotes.AddToScore(NoteType.Hold, NoteTiming.Perfect);
            else if (holdResult == HitState.Miss)
                songNotes.AddToScore(NoteType.Hold, Timing.MissFlag);
        }

        // Check if we've hit any motions
        foreach (var motion in motionList)
        {
            //var motionTimeMS = ((motion.BeatLocation - currentBeat) * 60 / Globals.CurrentBpm);
            var motionTimeMS = Globals.GetSecAtBeat(motion.BeatLocation) - currentTime;
            if (motionTimeMS < -MotionTiming.Miss)
            {
                motion.HitResult.WasHit = true;
                motion.HitResult.Difference = Timing.MissFlag;
                songNotes.AddToScore(NoteType.Motion, motion.HitResult.Difference);
            }
            //else if (GameState.CurrentSettings.AutoSetting == Settings.AutoMode.Auto ||
            //    (GameState.CurrentSettings.AutoSetting == Settings.AutoMode.AutoDown && motion.Motion == Motion.Down))
            else if (GameState.CurrentSettings.AutoSetting == Settings.AutoMode.Auto )
            {
                if (Math.Abs(motionTimeMS) < NoteTiming.AutoTolerance)
                {
                    motion.HitResult.WasHit = true;
                    motion.HitResult.Difference = 0;
                    //gradeCollection.Set(gameTime, motion);
                    songNotes.AddToScore(NoteType.Motion, motion.HitResult.Difference);
                }
            }
            //else if (motion.Motion == Motion.Down && !double.IsNaN(motionCollection.DownBeat))
            //{
            //    if (motionCollection.CheckHit(motion))
            //    {
            //        gradeCollection.Set(gameTime, motion);
            //        songNotes.AddToScore(NoteType.Motion, motion.HitResult.Difference);
            //    }
            //}
            //else if (motion.Motion == Motion.Up && motionTimeMS < MotionTiming.JumpPerfectCheck)
            //{
            //    // If there's no feet on the pad within the perfect window, it counts
            //    if (touchCollection.Points.Count == 0)
            //    {
            //        motion.HitResult.WasHit = true;
            //        motion.HitResult.Difference = (float)motionTimeMS;
            //        gradeCollection.Set(gameTime, motion);
            //        songNotes.AddToScore(NoteType.Motion, motion.HitResult.Difference);
            //    }
            //}
        }
    }
}
