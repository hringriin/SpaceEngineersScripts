/*
 *   R e a d m e
 *   -----------
 * 
 *   This script will take a BlockGroup of Lights and will change their offset
 *   to create a running light, as you would expect it in a space ship.
 *   
 *   The thing with the lights itself is, they need to be in order. The script
 *   will sort them for you, but they need to have a number according to the
 *   sequence they are ought to light up.
 *   
 *   If you place 10 lights one after the other, the lights will probably be in
 *   order, make sure of that by showing them und the HUD (use with antenna)
 *   and rename them if needed.
 *   
 *   After naming them correctly (it sould not be necessary, if built in order
 *   in the first place), group them and take this group name as argument for
 *   the script and you're good to go.
 *   
 *   The script orders the lights itself and will make sure the numbers contain
 *   leading zeroes, if necessary.
 *   
 *   If **ONE** light has no number at all, it is fine as well.
 *   
 *   The script assumes, lights are named by default, so names like
 *      "Interior Light"
 *      "Interior Light 2"
 *      "Interior Light 3"
 *      "Interior Light 4"
 *      "Interior Light 13"
 *   are completely ok!
 *   
 */