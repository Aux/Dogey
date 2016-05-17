using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Modules.Games.Types
{
    public class DraftColor
    {
        public static string[] Names = {
            "Red",
            "Blue",
            "Yellow",
            "Green",
            "Purple",
            "Orange",
            "Pink",
            "LightBlue",
            "Grey",
            "Teal"
        };
        public static Color[] Colors = {
            Color.Red,
            Color.DarkBlue,
            Color.DarkGold,
            Color.Green,
            Color.Purple,
            Color.Orange,
            Color.Magenta,
            Color.Blue,
            Color.LightGrey,
            Color.Teal
    };

        public static Color Red = Color.Red;
        public static Color Blue = Color.DarkBlue;
        public static Color Yellow = Color.DarkGold;
        public static Color Green = Color.Green;
        public static Color Purple = Color.Purple;
        public static Color Orange = Color.Orange;
        public static Color Pink = Color.Magenta;
        public static Color LightBlue = Color.Blue;
        public static Color Grey = Color.LightGrey;
        public static Color Teal = Color.Teal;
    }
}
