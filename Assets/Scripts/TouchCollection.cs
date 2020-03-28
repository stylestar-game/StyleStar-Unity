using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace StyleStar
{
    public static class TouchSettings
    {
        public static Axis WidthAxis { get; set; } = Axis.PosX;
        public static int CalMinX { get; set; } = 0;
        public static int CalMaxX { get; set; } = 1280;
        public static int AbsX { get; set; } = 1024;
        public static int CalMinY { get; set; } = 0;
        public static int CalMaxY { get; set; } = 720;
        public static int AbsY { get; set; } = 1024;

        public static void SetConfig(Dictionary<string, object> config)
        {
            foreach (var entry in config)
            {
                switch (entry.Key)
                {
                    case "WidthAxis":
                        WidthAxis = (Axis)Convert.ToInt32(entry.Value);
                        break;
                    case "CalMinX":
                        CalMinX = Convert.ToInt32(entry.Value);
                        break;
                    case "CalMaxX":
                        CalMaxX = Convert.ToInt32(entry.Value);
                        break;
                    case "AbsX":
                        AbsX = Convert.ToInt32(entry.Value);
                        break;
                    case "CalMinY":
                        CalMinY = Convert.ToInt32(entry.Value);
                        break;
                    case "CalMaxY":
                        CalMaxY = Convert.ToInt32(entry.Value);
                        break;
                    case "AbsY":
                        AbsY = Convert.ToInt32(entry.Value);
                        break;
                    default:
                        break;
                }
            }
        }

        public static Dictionary<string, object> GetConfig()
        {
            return new Dictionary<string, object>()
            {
                {"WidthAxis", (int)WidthAxis },
                {"CalMinX", CalMinX },
                {"CalMaxX", CalMaxX },
                {"AbsX", AbsX },
                {"CalMinY", CalMinY },
                {"CalMaxY", CalMaxY },
                {"AbsY", AbsY }
            };
        }

        public enum Axis
        {
            PosX,
            PosY,
            NegX,
            NegY
        }
    }

    public static class TouchCollection
    {
        public static ConcurrentDictionary<uint, TouchPoint> Points = new ConcurrentDictionary<uint, TouchPoint>();

        //public bool UpdateID(uint id, Point rawPt)
        //{
        //    var pt = Points.FirstOrDefault(x => x.Value.ID == id);
        //    pt.Value.RawX = rawPt.X;
        //    pt.Value.RawY = rawPt.Y;
        //    return true;
        //}

        public static bool RemoveID(uint id)
        {
            Points[id].Disable();
            TouchPoint temp;
            return Points.TryRemove(id, out temp);
        }

        public static bool CheckHit(Note note)
        {
            var noteMin = Globals.CalcTransX(note, Side.Left);
            var noteMax = Globals.CalcTransX(note, Side.Right);

            float diffMS = float.MaxValue;
            if (note.Type != NoteType.Shuffle)
            {
                var validPoints = Points.Where(x => x.Value.Valid && x.Value.MinX < noteMax && x.Value.MaxX > noteMin).ToList();
                if (validPoints.Count == 0)
                    return false;   // No need to modify hit result-- defaults to false

                validPoints.Sort((x, y) => Math.Abs(x.Value.Beat - note.BeatLocation).CompareTo(Math.Abs(y.Value.Beat - note.BeatLocation)));

                // Use the closest point and get the time difference
                //float diffMS = (float)(((note.BeatLocation - validPoints.First().Beat) * 60 / Globals.CurrentBpm));
                diffMS = (float)(Globals.GetSecAtBeat(note.BeatLocation) - Globals.GetSecAtBeat(validPoints.First().Value.Beat));
                if (diffMS > NoteTiming.Bad) // Too soon to hit, just leave
                    return false;

                // All other times are valid
                note.HitResult.WasHit = true;
                note.HitResult.Difference = diffMS;
            }
            else
            {
                var validPoints = Points.Where(x => x.Value.Valid && (float)(Globals.GetSecAtBeat(note.BeatLocation) - Globals.GetSecAtBeat(x.Value.UpdateBeat)) < NoteTiming.Shuffle && x.Value.VelX >= NoteTiming.ShuffleVelocityThreshold).ToList();
                if (validPoints.Count == 0)
                    return false;   // No need to modify hit result-- defaults to false

                // If any points exist, then we're fine
                note.HitResult.WasHit = true;
                note.HitResult.Difference = diffMS;
            }

            return true;
        }

        public static void Draw()
        {
            foreach (var touch in Points)
            {
                touch.Value.Draw();
            }
        }
    }

    public class TouchPoint
    {
        public int RawX { get; set; }  // Scaled value after calibration (0 to TouchSettings.AbsX)
        public int RawY { get; set; } // Scaled value after calibration (0 to TouchSettings.AbsX)
        public int VelX { get; private set; }
        public int VelY { get; private set; }
        public float X { get { return RawX * Globals.GradeZoneWidth / TouchSettings.AbsX - Globals.GradeZoneWidth / 2; } }
        public int RawWidth { get; set; } // 0-1023
        public int RawHeight { get; set; } // 0-1023
        public float Width { get { return RawWidth * Globals.GradeZoneWidth / TouchSettings.AbsX; } }
        public float MinX { get { return X - Width / 2; } }
        public float MaxX { get { return X + Width / 2; } }
        public double Beat { get; private set; }
        public double UpdateBeat { get; private set; }
        public uint ID { get; set; } // 32bit ID (from Windows Message, etc.)
        public bool Valid { get; set; }

        private GameObject footObject;
        private GameObject bgObject;
        //private QuadTexture footTexture;
        //private QuadTexture laneTexture;

        public TouchPoint(double beat, int rawX, int rawY)
        {
            Beat = beat;
            var xy = ConvertXY(rawX, rawY);
            RawX = xy.Item1;
            RawY = xy.Item2;
            Valid = true;
        }

        public void Reset(double beat, int rawX, int rawY, int rawWidth, int rawHeight)
        {
            Beat = beat;
            var xy = ConvertXY(rawX, rawY);
            RawX = xy.Item1;
            RawY = xy.Item2;
            RawWidth = rawWidth;
            RawHeight = rawHeight;
        }

        public void Update(double beat, int x, int y)
        {
            UpdateBeat = beat;
            var xy = ConvertXY(x, y);
            VelX = xy.Item1 - RawX;
            VelY = xy.Item2 - RawY;
            RawX = xy.Item1;
            RawY = xy.Item2;
        }

        private Tuple<int, int> ConvertXY(int rawX, int rawY)
        {
            int x = -1, y = -1;
            switch (TouchSettings.WidthAxis)
            {
                case TouchSettings.Axis.PosX:
                    x = (int)((float)(rawX - TouchSettings.CalMinX) / (float)(TouchSettings.CalMaxX - TouchSettings.CalMinX) * (float)TouchSettings.AbsX);
                    y = (int)((float)(rawY - TouchSettings.CalMinY) / (float)(TouchSettings.CalMaxY - TouchSettings.CalMinY) * (float)TouchSettings.AbsY);
                    break;
                case TouchSettings.Axis.PosY:
                    x = (int)((float)(rawY - TouchSettings.CalMinY) / (float)(TouchSettings.CalMaxY - TouchSettings.CalMinY) * (float)TouchSettings.AbsX);
                    y = TouchSettings.AbsX - (int)((float)(rawX - TouchSettings.CalMinX) / (float)(TouchSettings.CalMaxX - TouchSettings.CalMinX) * (float)TouchSettings.AbsY);
                    break;
                case TouchSettings.Axis.NegX:
                    x = TouchSettings.AbsY - (int)((float)(rawY - TouchSettings.CalMinY) / (float)(TouchSettings.CalMaxY - TouchSettings.CalMinY) * (float)TouchSettings.AbsX);
                    y = TouchSettings.AbsX - (int)((float)(rawX - TouchSettings.CalMinX) / (float)(TouchSettings.CalMaxX - TouchSettings.CalMinX) * (float)TouchSettings.AbsY);
                    break;
                case TouchSettings.Axis.NegY:
                    x = TouchSettings.AbsY - (int)((float)(rawY - TouchSettings.CalMinY) / (float)(TouchSettings.CalMaxY - TouchSettings.CalMinY) * (float)TouchSettings.AbsX);
                    y = (int)((float)(rawX - TouchSettings.CalMinX) / (float)(TouchSettings.CalMaxX - TouchSettings.CalMinX) * (float)TouchSettings.AbsY);
                    break;
                default:
                    break;
            }
            return new Tuple<int, int>(x, y);
        }

        public void Draw()
        {
            if (footObject == null)
                footObject = Pools.FootMarker.GetPooledObject();
            if (bgObject == null)
                bgObject = Pools.FootBg.GetPooledObject();

            footObject.transform.position = new Vector3(X, -0.001f, 0);
            bgObject.transform.position = new Vector3(X, -0.001f, 24.5f);

            if((footObject.transform.position.x > 0 && footObject.transform.localScale.x > 0) ||
                (footObject.transform.position.x < 0 && footObject.transform.localScale.x < 0))
            {
                footObject.transform.localScale = new Vector3(footObject.transform.localScale.x * -1.0f, footObject.transform.localScale.y);
            }

            footObject.SetActive(true);
            bgObject.SetActive(true);
        }

        public void Disable()
        {
            footObject.SetActive(false);
            bgObject.SetActive(false);
        }

        //public void Draw(Matrix view, Matrix projection)
        //{
        //    if (laneTexture == null)
        //        laneTexture = new QuadTexture(Globals.Textures["FootHold"]);
        //    laneTexture.SetVerts(MaxX, MinX, -(float)Globals.StepNoteHeightOffset, 300);
        //    laneTexture.Draw(view, projection);

        //    if (footTexture == null)
        //        footTexture = new QuadTexture(RawX <= TouchSettings.AbsX / 2 ? Globals.Textures["FootLeft"] : Globals.Textures["FootRight"]);
        //    footTexture.SetVerts(X + Globals.FootWidth / 2, X - Globals.FootWidth / 2, -(float)Globals.StepNoteHeightOffset, (float)Globals.StepNoteHeightOffset, -0.05f);

        //    footTexture.Draw(view, projection);
        //}
    }
}
