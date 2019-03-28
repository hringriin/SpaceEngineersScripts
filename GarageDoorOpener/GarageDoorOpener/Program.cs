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
        /*
         * Example arguments
         * Note, that only TRANSCEIVERS need to actively put arguments into
         * their programmable blocks' calls.
         * 
         * Note the <LIM> in each example, which is a "limiter" or separator,
         * separating multiple arguments from each other.
         * 
         * Example 1:
         *      open<LIM>EXAMPLE_KEY<LIM>Garage_Door_1
         *      
         * Example 2:
         *      close<LIM>ANOTHER_EXAMPLE_KEY<LIM>RoofTop-Garage
         *
         * Example 3:
         *      addKey<LIM>MyKey
         */

        // BEGIN SCRIPT CONFIGURATION

        // if set to True, transceiver mode is enabled, else receiver mode is
        // used.
        private Boolean transmode = false;

        // ENCRYPTION! (sort of)
        // this list contains ALL "secret keys" that are allowed to open/close
        // a garage door. maintain this list by yourself by adding the keys via
        // the argument "addKey<LIM>YOURKEY"
        // the word <LIM> is mandatory, since it separates the key from the
        // argument!
        private List<String> keys;

        // SPLIT STRING
        // this is very important, because this separates the key from the
        // argument to be handed over to the script.
        private String splitString = "<LIM>";

        // ANTENNA
        // the antenna itself
        private IMyRadioAntenna antenna;
        // the antenna to be used with this script. Each script needs one
        // antenna. Enter the name of the antenna below.
        private String antennaName = "Antenna";

        // END SCRIPT CONFIGURATION

        //String[] myStorage;

        public Program()
        {
            if (this.transmode)
                Echo("Initialising GarageDoor opener as transceiver.");
            else
                Echo("Initialising GarageDoor opener as receiver.");

            antenna = (IMyRadioAntenna)GridTerminalSystem.GetBlockWithName(this.antennaName);
            keys = new List<String>();

            // BEGIN LIST OF KEYS
            //keys.Add("EXAMPLE_KEY");
            // END LIST OF KEYS

            /*
             * SAVING DOES NOT WORK.
             * 
            if (this.Storage.Length > 0)
            {
                Echo("Storage Length: " + this.Storage.Length);
                myStorage = this.Storage.Split(new String[] { "\n" }, StringSplitOptions.None);

                Echo("Storage:");
                foreach ( String s in myStorage)
                {
                    this.keys.Add(s);
                }
            }

            if (this.keys.Count > 0 )
            {
                Echo("Keys:");
                foreach ( String k in this.keys )
                {
                    Echo(k);
                }
            }
            */
        }

        public void Save()
        {
            /*
             * SAVING DOES NOT WORK
             * 
            Echo("NULLING");
            this.Storage = null;

            Echo("Saving ...");
            if ( this.keys.Count > 0 )
            {
                this.Storage += "\n";
            }

            foreach ( String s in this.keys )
            {
                this.Storage += s;
            }
            Echo("... done!");

            Echo(this.Storage);
            */
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (argument.Contains(this.splitString))
            {
                String[] splits = argument.Split(new String[] { this.splitString }, StringSplitOptions.None);

                switch (splits[0])
                {
                    case "addKey":
                        this.AddKeyToList(splits[1]);
                        break;

                    case "searchKey":
                        this.SearchKey(splits[1]);
                        break;

                    default:
                        Echo("Default");
                        Echo("Argument not understandable.");
                        Echo("The String has to consist of a key, an argument and a block group, separated only by <LIM>");
                        break;
                }

                if (this.transmode)
                {

                    if (!String.IsNullOrWhiteSpace(argument))
                    {
                        // yes, I know this is depricated. I just don't know it
                        // better, yet.
                        this.antenna.TransmitMessage(argument);
                    }
                    else
                    {
                        Echo("Cannot transmit message!");
                    }
                }
                else
                {
                    // the splits are in the following order:
                    // 0: argument (open, close, ...)
                    // 1: key
                    // 2: name of garage door blockgroup
                    if (!String.IsNullOrWhiteSpace(argument))
                    {
                        switch (splits[0])
                        {
                            case "open":
                                this.OpenDoors(splits[1], splits[2]);
                                break;

                            case "close":
                                this.CloseDoors(splits[1], splits[2]);
                                break;
                        }
                    }
                    else
                    {
                        Echo("Argument invalid, i.e. null or empty");
                    }
                }
            }
            else
            {
                switch (argument)
                {
                    case "flushKeys":
                        this.FlushKeys();
                        break;
                }
            }
        }

        private Boolean MatchKeys(String pKey)
        {
            if (this.keys.Contains(pKey))
                return true;
            else
            {
                Echo("Key does not match!");
                return false;
            }
        }

        private void OpenDoors(String pKey, String pGroup)
        {
            if (this.MatchKeys(pKey))
            {
                if (pGroup != null && !pGroup.Equals(""))
                {
                    IMyBlockGroup group = GridTerminalSystem.GetBlockGroupWithName(pGroup);
                    List<IMyAirtightHangarDoor> doors = new List<IMyAirtightHangarDoor>();
                    group.GetBlocksOfType<IMyAirtightHangarDoor>(doors);

                    foreach (IMyAirtightHangarDoor d in doors)
                    {
                        d.OpenDoor();
                    }
                }
            }
        }

        private void CloseDoors(String pKey, String pGroup)
        {
            if (this.MatchKeys(pKey))
            {
                if (pGroup != null && !pGroup.Equals(""))
                {
                    IMyBlockGroup group = GridTerminalSystem.GetBlockGroupWithName(pGroup);
                    List<IMyAirtightHangarDoor> doors = new List<IMyAirtightHangarDoor>();
                    group.GetBlocksOfType<IMyAirtightHangarDoor>(doors);

                    foreach (IMyAirtightHangarDoor d in doors)
                    {
                        d.CloseDoor();
                    }
                }
            }
        }

        private Boolean AddKeyToList(String pString)
        {
            this.keys.Add(pString);
            if (this.MatchKeys(pString))
            {
                Echo("Adding key");
            }
            else
            {
                Echo("NOT adding key");
            }
            return this.MatchKeys(pString);
        }

        private void FlushKeys()
        {
            Echo("Flushing keys");
            Storage = "";
            this.keys.Clear();
        }

        private Boolean SearchKey(String pString)
        {
            if (this.keys.Contains(pString))
            {
                Echo("Key found!");
                return true;
            }
            else
            {
                Echo("Key not found!");
                return false;
            }
        }
    }
}