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
         * HOW TO USE?
         *
         * This script will provide the solution to opening a hangar door
         * remotely from any vehicle. You need the following preparation of a)
         * your vehicle and b) your base/building/vehicle with the door to be
         * opened/closed:
         *
         *      a): SENDING (TRANSCEIVER)
         *          The sender needs a programmable block and a radio antenna.
         *          - Assign the programmable block to the radio antenna via the
         *            Control Panel.
         *          - Rename the antenna / programmable block to whatever you like.
         *          - Load this script into the programmable block and set
         *            "transmode = true" in the configuration area below.
         *          - If you renamed your antenna, also correct the name of
         *            antennaName = "Antenna" in the configuration area below.
         *
         *      b): RECEIVING (RECEIVER)
         *          The receiver needs a programmable block and a radio antenna.
         *          - Assign the programmable block to the radio antenna via
         *            the Control Panel.
         *          - Rename the antenna / programmable block to whatever you like.
         *          - Load this script into the programmable block.
         *          - If you renamed your antenna, also correct the name of
         *            antennaName = "Antenna" in the configuration area below.
         *
         *      For "authentication", i.e. the transceiver is allowed to enter
         *      the base, you need to store keys in the receiver script.
         *      How to do that? Go to the programmable block in the Control
         *      Panel and enter the following:
         *
         *          addKey<LIM>MyPersonalKey
         *
         *      This will add a key "MyPersonalKey" to the system. Do not care
         *      about the "<LIM>", due to its purpose of a devider; internal
         *      stuff, just be sure to add it. I've not tested it, but the key
         *      is allowed to contain whitespaces (space key), I do not
         *      recommend it, though.
         *
         *      Now, you'll need to set up your quick bar.
         *
         *      In your vehicle (TRANSCEIVER), open the Toolbar config
         *      (default: press 'g') and drag the programmable block to the
         *      quickbar and chose "run". There should now be a popup, asking
         *      for an argument. There are two sensible arguments. There are
         *      more possible, but only two need to be quick-bar-ed:
         *
         *          Open the door:
         *              open<LIM>MyPersonalKey<LIM>HangarDoorOne
         *
         *          Close the door:
         *              close<LIM>MyPersonalKey<LIM>HangarDoorOne
         *
         *      This will cause the programm to ask the other side to
         *      open/close "HangarDoorOne" (a block group of hangar doors),
         *      providing the key "MyPersonalKey".
         *
         *      Please note, that every input is case-sensitive, so be
         *      attentive.
         *
         *      Now you can close the Toolbar config and press the
         *      quick-bar-buttons you just assigned and the door 'should'
         *      open/close. Due to the surrounding circumstances, only
         *      opening/closing one door at a time will work, pressing two
         *      buttons at once, e.g. for two different hangar door (groups)
         *      will result in opening/closing only one of those doors. Just
         *      keep in mind, that the script might not be as fast as your
         *      fingers :-)
         *
         * Other possible arguments (with examples) are:
         *
         *      Open a door:
         *          open<LIM>[KEY]<LIM>[Hangar-Door_BlockGroup_Name]
         *
         *      Close a door:
         *          close<LIM>[KEY]<LIM>[Hangar-Door_BlockGroup_Name]
         *
         *      Add a key:
         *          addKey<LIM>[KEY]
         *
         *      Remove a key:
         *          removeKey<LIM>[KEY]
         *
         *      Remove ALL keys:
         *          flushKeys
         *
         *      Search for a key, i.e. is it in the system?
         *          searchKey<LIM>[KEY]
         *
         *
         *
         * IMPORTANT NOTICE:
         *      All of the commands mentioned above and below will only work
         *      for the programmable block you're sitting in front of and
         *      therefor only for the grid you're working with.
         *
         *
         *
         *
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

        public Program()
        {
            if (this.transmode)
                Echo("\nInitialising GarageDoor opener as transceiver.");
            else
                Echo("\nInitialising GarageDoor opener as receiver.");

            antenna = (IMyRadioAntenna)GridTerminalSystem.GetBlockWithName(this.antennaName);
            keys = new List<String>();

            String[] storedData = Storage.Split(';');

            if ( storedData.Length > 0 )
            {
                Echo("\nRecovering stored Data");
                foreach ( String s in storedData)
                {
                    this.keys.Add(s);
                }
            }
        }

        public void Save()
        {
            Echo("\nClearing Storage");
            this.Storage = "";

            Echo("\nSaving Data");
            if ( this.keys.Count > 0 )
            {
                foreach ( String s in this.keys )
                {
                    if (this.Storage.Length == 0)
                        this.Storage = s;
                    else
                        this.Storage = String.Join(";", this.Storage, s);
                }
            }
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

                    case "removeKey":
                        this.RemoveKeyFromList(splits[1]);
                        break;

                    case "searchKey":
                        this.MatchKeys(splits[1]);
                        break;

                    default:
                        Echo("\nArgument not understandable.");
                        Echo("\nThe String has to consist of a key, an argument and a block group, separated only by <LIM>");
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
                        Echo("\nCannot transmit message!");
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
                        Echo("\nArgument invalid, i.e. null or empty");
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
            {
                Echo("\nKey match!");
                return true;
            }
            else
            {
                Echo("\nKey does not match!");
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

        private Boolean RemoveKeyFromList(String pKey)
        {
            if ( this.MatchKeys(pKey))
            {
                Echo("\nRemoving key");
                this.keys.Remove(pKey);
            }
            else
            {
                Echo("\nNOT removing key");
            }
            return this.MatchKeys(pKey);
        }

        private Boolean AddKeyToList(String pKey)
        {
            if (this.MatchKeys(pKey))
            {
                Echo("\nNOT adding key");
            }
            else
            {
                Echo("\nAdding key");
                this.keys.Add(pKey);
            }
            return this.MatchKeys(pKey);
        }

        private void FlushKeys()
        {
            Echo("\nFlushing keys");
            Storage = "";
            this.keys.Clear();
        }
    }
}
