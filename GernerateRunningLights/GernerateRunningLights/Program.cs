// @Author: el_barzh

using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Linq;
using System;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        /**
         * This programm / script will accept a BlockGroup of interior Lights
         * and make a beautiful running light of them.
         * 
         * You need to modify your blink length and interval by yourself! The
         * script only regulates the offset!
         */

        // the lights
        private List<IMyInteriorLight> lights;

        /**
         * Have a look at the ReadMe!
         *
         * The main entry point of the script, invoked every time
         * one of the programmable block's Run actions are invoked,
         * or the script updates itself. The updateSource argument
         * describes where the update came from. Be aware that the
         * updateSource is a  bitfield  and might contain more than 
         * one update type.
         * 
         * @Param argument: the BlockGroup to be handled by the script
         */
        public void Main(string argument, UpdateType updateSource)
        {
            // check, if the argument is valid, i.e. it is a string
            if (argument.Equals("") || argument == null)
            {
                Echo("\""
                    + argument
                    + "\" is no valid parameter. Enter a BlockGroup name which contains only interior lights!");
            }
            else
            {
                // get the blockgroup itself. this is not a real list that we
                // can use
                IMyBlockGroup lightList = GridTerminalSystem.GetBlockGroupWithName(argument);

                // temporary list, because the BlockGroup itself outputs a list
                // of the wrong data type
                List<IMyTerminalBlock> tmpLights = new List<IMyTerminalBlock>();
                lightList.GetBlocksOfType<IMyInteriorLight>(tmpLights);

                // finally add all the lights to the list, i.e. get the list of
                // TerminalBlocks and cast them to InteriorLights
                this.lights = tmpLights.Cast<IMyInteriorLight>().ToList();

                // this string will take care of the leading zeroes, if
                // necessary
                String digits = "D" + this.lights.Count.ToString().Length;

                // now we will iterate through all the lights, to make sure
                // they are named properly
                foreach (IMyInteriorLight l in this.lights)
                {
                    // the *result* string ... for later
                    String tmp = l.DisplayNameText;

                    // the substring (match) to look for, in this case we
                    // search the last part of the string for numbers
                    String match = System.Text.RegularExpressions.Regex.Match(l.DisplayNameText, @"\d+$").Value;

                    // if there is no number, this is our only exception,
                    // assume it is the first light and has no number
                    if ( match.Equals("") )
                    {
                        match = "1";
                        tmp = tmp.Insert(tmp.Length, " " + match);
                    }

                    // substituion string, the match will be replaced with this
                    String tmpSub = Convert.ToInt32(match).ToString(digits);

                    // actual replacing of the match with the substitution
                    // string
                    tmp = System.Text.RegularExpressions.Regex.Replace(
                        tmp,
                        match,
                        tmpSub);

                    // set the custom name, which will be visible in the SE
                    // interface
                    l.CustomName = tmp;


                    // just some output to see the script did anything ... and
                    // for debugging purposes *cough*
                    Echo("Orig:     " + l.DisplayNameText);
                    Echo("Match:    " + match);
                    Echo("Sub:      " + tmpSub);
                    Echo("Result:   " + tmp + "\n");
                }

                // sorting the list by the display name. setting the display
                // name is forbidden, changing the customname will change this
                // as well, though
                this.lights = this.lights.OrderBy(l => l.DisplayNameText).ToList();

                // call for the actual method, that handles the blink offset
                this.runningLight();
            }
        }

        /**
         * iterates through the lights and increments their blink offset
         * // TODO modify the main methods's argument to be able to set this
         * // while executing the programme
         */
        private void runningLight()
        {
            // the initial offset
            float offset = 0;
            
            // for each interior light increment the offset by, except if
            // incrementing would reach to a higher value than 100
            foreach ( IMyInteriorLight l in this.lights )
            {
                l.BlinkOffset = offset;

                if ((offset + 2) <= (float)100)
                    offset += 2;
                else
                    offset = 0;
            }
        }
    }
}