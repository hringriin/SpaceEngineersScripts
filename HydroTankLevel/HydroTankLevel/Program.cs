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
        private List<IMyGasTank> tanks;
        private List<IMyTextPanel> lcds;
        private IMyTextPanel lcd;

        private String tankGrpName = "Hydrogen-Tanks";
        private String lcdGrpName =  "Hydrogen-Tanks-LCD";
        private String lcdSumName = "Hydrogen-Tanks-LCD-SUM";

        public Program()
        {
            // run itself every 100 ticks
            Runtime.UpdateFrequency = UpdateFrequency.Update100;

            this.tanks = new List<IMyGasTank>();
            GridTerminalSystem.GetBlockGroupWithName(this.tankGrpName).GetBlocksOfType<IMyGasTank>(this.tanks);

            this.lcds = new List<IMyTextPanel>();
            GridTerminalSystem.GetBlockGroupWithName(this.lcdGrpName).GetBlocksOfType<IMyTextPanel>(this.lcds);

            this.lcd = (IMyTextPanel) GridTerminalSystem.GetBlockWithName("Hydrogen-Tanks-LCD-SUM");
        }

        public void Main(string argument, UpdateType updateSource)
        {
            this.CalcCapacity();
            this.CalcRatio();

            // the following is an example output

            /*
             * Tank #123
             * =========
             * 
             * Level:
             *     1234567 m^3 (123.4%)
             * Capacity:
             *     1234567 m^3
             *     
             *     
             * Tank #456
             * =========
             * 
             * Level:
             *     1234567 m^3 (123.4%)
             * Capacity:
             *     1234567 m^3
             */
            int k = 0;
            for ( int i = 0; i < this.lcds.Count; ++i )
            {
                IMyTextPanel d = this.lcds.ElementAt(i);
                String output = "";

                if (k < this.tanks.Count)
                {
                    output += this.WriteOutput(this.tanks.ElementAt(k));
                }
                else
                {
                    break;
                }

                ++k;

                if (k < this.tanks.Count)
                {
                    output += this.WriteOutput(this.tanks.ElementAt(k));
                }
                else
                {
                    break;
                }

                ++k;

                d.SetValueBool("ShowTextOnScreen", true);
                d.FontColor = new Color(51, 65, 60);
                d.FontSize = 1.15f;
                d.Font = "Monospace";

                d.WritePublicText(output);
            }

            String output2 = "";

            output2 += this.lcdSumName;
            // underline this shit
            output2 += "\n"
                + new string('=', this.lcdSumName.Length)
                + "\n\nCommulative values\n\n";
            output2 += "Level:\n";
            output2 += "    "
                + Math.Round((this.CalcCapacity() * this.CalcRatio()) / 1000, 1).ToString("0000.0")
                + "m^3 ("
                + Math.Round(this.CalcRatio() * 100, 1).ToString("000.0")
                + "%)\nCapacity\n";
            output2 += "    "
                + Math.Round(this.CalcCapacity() / 1000, 1).ToString("0000.0")
                + "m^3\n\n";

            this.lcd.SetValueBool("ShowTextOnScreen", true);
            this.lcd.FontColor = new Color(51, 65, 60);
            this.lcd.FontSize = 1.15f;
            this.lcd.Font = "Monospace";

            this.lcd.WritePublicText(output2);
        }

        private String WriteOutput(IMyGasTank pTank)
        {
            String output = "";

            output += pTank.DisplayNameText;
            // underline this shit
            output += "\n"
                + new string('=', pTank.DisplayNameText.Length)
                + "\n\n";
            output += "Level:\n";
            output += "    "
                + Math.Round((pTank.Capacity * pTank.FilledRatio) / 1000, 1).ToString("0000.0")
                + "m^3 ("
                + Math.Round(pTank.FilledRatio * 100, 1).ToString("000.0")
                + "%)\nCapacity\n";
            output += "    "
                + Math.Round(pTank.Capacity / 1000, 1).ToString("0000.0")
                + "m^3\n\n";

            return output;
        }

        private float CalcCapacity()
        {
            float cap = 0f;

            foreach ( IMyGasTank t in this.tanks)
            {
                
                cap += t.Capacity;
            }

            return cap;
        }

        private double CalcRatio()
        {
            double rat = 0d;

            foreach ( IMyGasTank t in this.tanks)
            {
                rat += t.FilledRatio;
            }

            return (rat / this.tanks.Count);
        }
    }
}