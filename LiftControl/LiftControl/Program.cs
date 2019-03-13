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
        // This script will alter some settings on pistons, i.e. MaxHeight and
        // MaxImpulse(Non)Axis.
        //
        // The Vanilla pistons work fine, mods which use the *IMyExtendedPistonBase*
        // Class should do the same.
        //
        // END INFORMATION


        // CONFIG //////////////////////////////////////////////////////////////////

        // This script requires a BlockGroup of pistons, just enter group names you
        // gave them below accordingly.

        // Enter the names of the piston and the lcd groups into the following
        // variables.
        private String pistonGroupName = "Piston_ServiceBay_Lift_1";

        // END CONFIG //////////////////////////////////////////////////////////////



        // list of all pistons
        private List<IMyExtendedPistonBase> pistons;

        // Constructor
        public Program()
        {
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

        // altering pistons' settings
        public void Main(string argument, UpdateType updateSource)
        {
            switch (argument)
            {
                case "incMaxHeight":
                    foreach (IMyExtendedPistonBase p in pistons)
                    {
                        if (p.MaxLimit - p.MinLimit + 0.25f >= 3f)
                            p.MaxLimit = p.MinLimit + 3f;
                        else
                            p.MaxLimit += 0.25f;
                    }
                    break;

                case "decMaxHeight":
                    foreach (IMyExtendedPistonBase p in pistons)
                    {
                        if (p.MaxLimit - p.MinLimit - 0.25f <= 0f)
                            p.MaxLimit = p.MinLimit;
                        else
                            p.MaxLimit -= 0.25f;
                    }
                    break;

                case "incMaxImpulseAxis":
                    foreach (IMyExtendedPistonBase p in pistons)
                    {
                        if (p.GetValueFloat("MaxImpulseAxis") + 500f >= 75000f)
                            p.SetValueFloat("MaxImpulseAxis", 75000f);
                        else
                            p.SetValueFloat("MaxImpulseAxis", p.GetValueFloat("MaxImpulseAxis") + 500f);
                    }
                    break;

                case "decMaxImpulseAxis":
                    foreach (IMyExtendedPistonBase p in pistons)
                    {
                        if (p.GetValueFloat("MaxImpulseAxis") - 500f <= 1000f)
                            p.SetValueFloat("MaxImpulseAxis", 500f);
                        else
                            p.SetValueFloat("MaxImpulseAxis", p.GetValueFloat("MaxImpulseAxis") - 500f);
                    }
                    break;
            }
        }

    }
}