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
        // This file contains your actual script.
        //
        // You can either keep all your code here, or you can create separate
        // code files to make your program easier to navigate while coding.
        //
        // In order to add a new utility class, right-click on your project, 
        // select 'New' then 'Add Item...'. Now find the 'Space Engineers'
        // category under 'Visual C# Items' on the left hand side, and select
        // 'Utility Class' in the main area. Name it in the box below, and
        // press OK. This utility class will be merged in with your code when
        // deploying your final script.
        //
        // You can also simply create a new utility class manually, you don't
        // have to use the template if you don't want to. Just do so the first
        // time to see what a utility class looks like.

        // INFORMATION
        //
        // It is not important which display you use. I recommend using a "LCD Panel",
        // so no wide or text panel. I recommend using the "Monospace" font with a font
        // size of 1.0, this will make use of the whole screen.
        //
        // The Vanilla pistons work fine, mods which use the *IMyExtendedPistonBase*
        // Class should do the same.
        //
        // END INFORMATION


        // CONFIG //////////////////////////////////////////////////////////////////

        // This script requires a BlockGroup of pistons and a BlockGroup of LCD
        // Displays, just enter group names you gave them below accordingly.

        // Enter the names of the piston and the lcd groups into the following
        // variables.
        private String pistonGroupName = "Piston_ServiceBay_Lift_1";
        private String lcdGroupName = "LCD_ServiceBay_Lift_1";

        // The replacement string is the (Sub-)String of the pistons' group to replace
        // with nothing. Whatever you put in this variable, it will be replaced with an
        // empty string "", **NOT NULL**!
        private String replacementString = "Piston_ServiceBay_";

        // END CONFIG //////////////////////////////////////////////////////////////



        // list of all pistons
        private List<IMyExtendedPistonBase> pistons;

        // list of all lcd/text panels
        private List<IMyTextPanel> panels;

        public Program()
        {
            // It's recommended to set RuntimeInfo.UpdateFrequency
            // here, which will allow your script to run itself without a
            // timer block.

            // Update (re-run program) every 100 ticks
            Runtime.UpdateFrequency = UpdateFrequency.Update10;

            // the pistons. all of them for lift 1
            //
            // get the block group (the pistons) in the first place. because the SE API
            // is huge dump of donkey shite, you get a self-made class which contains a
            // list itself.
            IMyBlockGroup pistonList = GridTerminalSystem.GetBlockGroupWithName(this.pistonGroupName);

            // a temporary list containing the blocks as IMyTerminalBlock, sadly not as
            // IMyExtendedPistonBase
            List<IMyTerminalBlock> tmpPistons = new List<IMyTerminalBlock>();
            pistonList.GetBlocks(tmpPistons);

            // finally cast the temporary list to the type I want
            pistons = tmpPistons.Cast<IMyExtendedPistonBase>().ToList();



            // the lcd panels. same as above for the pistons
            IMyBlockGroup panelList = GridTerminalSystem.GetBlockGroupWithName(this.lcdGroupName);

            // a temporary list
            List<IMyTerminalBlock> tmpPanels = new List<IMyTerminalBlock>();
            panelList.GetBlocks(tmpPanels);

            // finally cast the temporary list to the type I want
            panels = tmpPanels.Cast<IMyTextPanel>().ToList();
        }

        public void Save()
        {
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means.
            //
            // This method is optional and can be removed if not
            // needed.
        }

        // writing output from pistons to lcds
        public void Main(string argument, UpdateType updateSource)
        {
            // array for output strings.
            List<String> output = new List<String>();

            foreach (IMyExtendedPistonBase p in pistons)
            {
                // needed for clearing the string.
                String tmp = null;

                // regex foo ... i have not figured out how to 'use' or 'import' the
                // regex to avoid writing that whole System.Text-shit all the time.
                String liftName = System.Text.RegularExpressions.Regex.Replace(
                        p.CustomName,
                        this.replacementString,
                        "");
                liftName = System.Text.RegularExpressions.Regex.Replace(
                        liftName,
                        "_",
                        " ");

                liftName = liftName.Insert(liftName.Length - 3, ", Piston");

                // name of the piston of the lift
                tmp += "    " + liftName + "\n";
                tmp += "    =================\n\n";

                // current state (on or off)
                tmp += "Power State:\n    "
                    + (p.Enabled ? "Enabled" : "Disabled");

                // retracting or extending?
                tmp += "\n    ";
                tmp += p.Status;
                tmp += "\n";

                // current / maximum height of the piston
                tmp += "\n";
                tmp += "Height:\n    "
                    + Math.Round(p.CurrentPosition - p.MinLimit, 2).ToString("0.00")
                    + " m  of  "
                    + Math.Round(p.MaxLimit - p.MinLimit, 2).ToString("0.00")
                    + " m";

                // Velocity of the piston
                tmp += "\n\n";
                tmp += "Velocity:\n    "
                    + Math.Round(p.Velocity, 3).ToString("0.000")
                    + " m/s";
                tmp += "\n";

                // force (power) of the piston in axial direction
                tmp += "\n";
                tmp += "Max Force Axis:\n    "
                    + (Math.Round(p.GetValueFloat("MaxImpulseAxis"), 3) / 1000).ToString("0.0")
                    + " kN";

                // force (power) of the piston in non-axial direction
                tmp += "\n";
                tmp += "Max Force NonAxis:\n    "
                    + (Math.Round(p.GetValueFloat("MaxImpulseNonAxis"), 3) / 1000).ToString("0.0")
                    + " kN";

                output.Add(tmp);
            }

            // write each string of the output List to a display
            foreach (IMyTextPanel t in panels)
            {
                t.WritePublicText(output.ElementAt(0));
                output.RemoveAt(0);
            }

            // If the String 'working' is shown in the Programmable Block's Console
            // output, the script runs fine.
            Echo("Working!");

            foreach (IMyExtendedPistonBase p in pistons)
            {
                Echo(p.CustomName);
            }

            foreach (IMyTextPanel t in panels)
            {
                Echo(t.CustomName);
            }
        }
    }
}