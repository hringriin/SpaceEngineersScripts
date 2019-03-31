using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        private List<IMyInteriorLight> lights;
        private List<Color> colors;

        // Gruvbox Colorscheme
        // https://github.com/morhetz/gruvbox#palette

        private Color black = new Color(16, 16, 16);
        private Color darkred = new Color(80, 14, 11);
        private Color darkgreen = new Color(60, 59, 10);
        private Color darkyellow = new Color(84, 60, 13);
        private Color darkblue = new Color(27, 52, 53);
        private Color darkpurple = new Color(69, 38, 53);
        private Color darkaqua = new Color(41, 62, 42);
        private Color gray = new Color(66, 60, 52);
        private Color darkgray = new Color(57, 51, 45);
        private Color red = new Color(98, 92, 20);
        private Color green = new Color(72, 73, 15);
        private Color yellow = new Color(98, 74, 18);
        private Color blue = new Color(51, 65, 60);
        private Color purple = new Color(83, 53, 61);
        private Color aqua = new Color(56, 75, 49);
        private Color white = new Color(92, 86, 70);


        public Program()
        {
            this.lights = new List<IMyInteriorLight>();
            this.colors = new List<Color>();

            this.colors.Add(this.black);
            this.colors.Add(this.darkred);
            this.colors.Add(this.darkgreen);
            this.colors.Add(this.darkyellow);
            this.colors.Add(this.darkblue);
            this.colors.Add(this.darkpurple);
            this.colors.Add(this.darkaqua);
            this.colors.Add(this.gray);
            this.colors.Add(this.darkgray);
            this.colors.Add(this.red);
            this.colors.Add(this.green);
            this.colors.Add(this.yellow);
            this.colors.Add(this.blue);
            this.colors.Add(this.purple);
            this.colors.Add(this.aqua);
            this.colors.Add(this.white);
        }

        public void Save()
        {
            //TODO
        }

        public void Main(string argument, UpdateType updateSource)
        {
            String[] splits = argument.Split(new String[] { "<LIM>" }, StringSplitOptions.None);
            GridTerminalSystem.GetBlockGroupWithName(splits[0]).GetBlocksOfType<IMyInteriorLight>(this.lights);

            foreach ( IMyInteriorLight l in this.lights )
            {
                switch (splits[1])
                {
                    case "black":
                    case "0":
                        l.Color = this.colors.ElementAt(0);
                        break;

                    case "darkred":
                    case "1":
                        l.Color = this.colors.ElementAt(1);
                        l.Color = this.darkred;
                        break;

                    case "darkgreen":
                    case "2":
                        l.Color = this.colors.ElementAt(2);
                        l.Color = this.darkgreen;
                        break;

                    case "darkyellow":
                    case "3":
                        l.Color = this.colors.ElementAt(3);
                        l.Color = this.darkyellow;
                        break;

                    case "darkblue":
                    case "4":
                        l.Color = this.colors.ElementAt(4);
                        l.Color = this.darkblue;
                        break;

                    case "darkpurple":
                    case "5":
                        l.Color = this.colors.ElementAt(5);
                        l.Color = this.darkpurple;
                        break;

                    case "darkaqua":
                    case "6":
                        l.Color = this.colors.ElementAt(6);
                        l.Color = this.darkaqua;
                        break;

                    case "gray":
                    case "7":
                        l.Color = this.colors.ElementAt(7);
                        l.Color = this.gray;
                        break;

                    case "darkgray":
                    case "8":
                        l.Color = this.colors.ElementAt(8);
                        l.Color = this.darkgray;
                        break;

                    case "red":
                    case "9":
                        l.Color = this.colors.ElementAt(9);
                        l.Color = this.red;
                        break;

                    case "green":
                    case "10":
                        l.Color = this.colors.ElementAt(10);
                        l.Color = this.green;
                        break;

                    case "yellow":
                    case "11":
                        l.Color = this.colors.ElementAt(11);
                        l.Color = this.yellow;
                        break;

                    case "blue":
                    case "12":
                        l.Color = this.colors.ElementAt(12);
                        l.Color = this.blue;
                        break;

                    case "purple":
                    case "13":
                        l.Color = this.colors.ElementAt(13);
                        l.Color = this.purple;
                        break;

                    case "aqua":
                    case "14":
                        l.Color = this.colors.ElementAt(14);
                        l.Color = this.aqua;
                        break;

                    case "white":
                    case "15":
                        l.Color = this.colors.ElementAt(15);
                        break;
                }

                l.Falloff = 2f;
                l.Intensity = 2f;
                l.Radius = 20f;
            }
        }
    }
}