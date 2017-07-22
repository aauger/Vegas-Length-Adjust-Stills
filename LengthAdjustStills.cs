/*
LengthAdjustStills.cs - Adjust still length in VEGAS Pro
Copyright(C) 2017 Austin Auger

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.If not, see<http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ScriptPortal.Vegas;

public class EntryPoint
{
    public void FromVegas(Vegas vegas)
    {
        List<TrackEvent> events = GetAppropriateTrackEvents(vegas.Project);
        int seconds = int.Parse(GetUserInput("Length?", "10"));
        int milliCross = int.Parse(GetUserInput("Milliseconds transition?", "2000"));

        //set first length event
        events[0].Length = Timecode.FromSeconds(seconds);

        //set remaining events
        for (int i = 1; i < events.Count; i++)
        {
            events[i].Start = events[i - 1].End - Timecode.FromMilliseconds(milliCross);
            events[i].Length = Timecode.FromSeconds(seconds);
        }
     }

    public List<TrackEvent> GetAppropriateTrackEvents(Project proj)
    {
        List<TrackEvent> tevs = new List<TrackEvent>();

        foreach (Track track in proj.Tracks)
        {
            foreach (TrackEvent te in track.Events)
            {
                //only selected video w/o audio is affected
                if (te.Selected && te.IsVideo() && !te.IsAudio())
                    tevs.Add(te);
            }
        }

        return tevs;
    }

    public string GetUserInput(string promptText, string defaultText)
    {
        Form f = new Form
        {
            Size = new Size(200, 120),
            Text = promptText
        };

        TextBox textBox = new TextBox();
        textBox.Size = new Size(150, 50);
        textBox.Text = defaultText;
        textBox.Left = 20;
        textBox.Top = 20;

        Button okButton = new Button() { Text = "OK" };
        okButton.Left = 20;
        okButton.Top = 40;
        okButton.Click += (sender, e) => f.Close();

        f.Controls.Add(textBox);
        f.Controls.Add(okButton);
        f.ShowDialog();

        return textBox.Text;
    }
}
