using System.Numerics;
namespace JumpKingMacro
{
    internal class JumpArray
    {
        // X = direction | 1-4 (Evens = left, Odds = right), Y = hold time in millisecs
        // Hold time ranges from 20 to 600
        internal static Vector2[] J_Array = new Vector2[]
        {
            //Screen 1 (Start - Old Guy)
            new Vector2(0, 700), //Jump to left ledge
            new Vector2(0, 20), //Get more space to jump
            new Vector2(1, 700), //Jump to top ledge
            new Vector2(1, 700), //Jump to next screen

            //Screen 2 (Red Crown Woods - Start)
            new Vector2(1, 60), //Line up wall jump
            new Vector2(1, 700), //Wall jump off left ledge
            new Vector2(0, 700), //Jump on top of square block
            new Vector2(0, 300), //Jump to top left block
            new Vector2(2, 200), //Zero out against wall
            new Vector2(1, 50), //Line up nest jump
            new Vector2(1, 700), //Jump to next screen

            //Screen 3 (Lower small trunk jumps)
            new Vector2(1, 50), //Get to right edge of ledge
            new Vector2(1, 150), //Jump gap to next wood ledge
            new Vector2(1, 50), //Get closer to right edge
            new Vector2(1, 400), //Jump to next platform
            new Vector2(0, 700), //Big jump to next platform
            new Vector2(3, 800), //Zero out against lip
            new Vector2(1, 700), //Wall bounce off lip to next platform
            new Vector2(0, 410), //Jump to crow
            new Vector2(2, 150), //Zero out against wall
            new Vector2(1, 700), //Jump to next screen

            //Screen 4
            new Vector2(0, 75), //Get closer to edge
            new Vector2(0, 700), //Jump to next high platform
            new Vector2(1, 200), //Jump gap
            new Vector2(1, 125), //Get closer to edge.
            new Vector2(1, 400), //Jump to next platform (right)
            new Vector2(0, 470), //Jump to next platform (left)
            new Vector2(1, 595), //Jumpt to next screen

            //Screen 5 (Red Leaves - 3 Block Top)
            new Vector2(0, 425), //Jump gap to the left
            new Vector2(1, 700), //Big jump to higher platform on right
            new Vector2(1, 450), //Jump to far right platform
            new Vector2(3, 250), //Zero out against wall
            new Vector2(0, 510), //Jump to first of three red platforms
            new Vector2(0, 700), //Bonk head on top to get to second
            new Vector2(2, 225), //Get closer to left edge
            new Vector2(0, 700), //Bonk head on top to get to third
            new Vector2(2, 350), //Get closer to left edge
            new Vector2(0, 170), //Small jump to crow
            new Vector2(1, 700), //Jump to next screen

            //Screen 6 (Collosal Drain - Start)
            new Vector2(1, 110), //Line up next jump with edge
            new Vector2(0, 470), //Jump to next platform (left)
            new Vector2(0, 400), //Jump to next platform (left)
            new Vector2(2, 400), //Zero out against wall
            new Vector2(0, 700), //Wall jump to next platform
            new Vector2(3, 450), //Walk off edge to right
            new Vector2(2, 400), //Zero out against wall on left
            new Vector2(3, 1000), //Walk to far edge on right
            new Vector2(1, 325), //Jump the gap
            new Vector2(3, 1500), //Walk to the end
            new Vector2(1, 700), //Jump to next screen (wall jump)

            //Screen 7 (First Sloped Jumps)
            new Vector2(2, 300), //Zero out against wall to left
            new Vector2(3, 400), //Walk to edge
            new Vector2(1, 700), //Wall jump to top platform
            new Vector2(2, 1350), //Walk to left edge
            new Vector2(0, 100), //Jump gap to lower pipe
            new Vector2(2, 500), //Walk to left edge of pipe
            new Vector2(0, 450), //Jump up sloped edge
            new Vector2(2, 150), //Zero out against left wall
            new Vector2(1, 520), //Jump to next pipe
            new Vector2(1, 500), //Jump to next screen (wall jump)

            //Screen 8 (Big Pipe)
            new Vector2(2, 300), //Zero out against left wall
            new Vector2(3, 500), //Walk to edge
            new Vector2(1, 700), //Jump to top platform
            new Vector2(3, 750), //Walk to right edge
            new Vector2(1, 500), //Jump gap
            new Vector2(3, 150), //Zero out against right wall
            new Vector2(0, 700), //Jump gap to the big pipe
            new Vector2(2, 2500), //Walk to left entrance
            new Vector2(1, 700), //Wall jump up to next platform
            new Vector2(0, 700), //Jump to next screen (wall jump)

            //Screen 9 (Hanging Jail Cells)
            new Vector2(1, 700), //Wall jump to next pipe
            new Vector2(2, 100), //Zero out against left wall
            new Vector2(1, 500), //Jump over middle pipe
            new Vector2(0, 150), //Jump to lower platform
            new Vector2(2, 250), //Zero out against pipe
            new Vector2(3, 350), //Walk to right edge
            new Vector2(1, 150), //Jump gap to first cage
            new Vector2(3, 250), //Walk to right edge
            new Vector2(1, 150), //Jump gap to second cage
            new Vector2(3, 250), //Walk to right edge
            new Vector2(1, 150), //Jump gap to third cage (crow)
            new Vector2(3, 200), //Walk to right edge
            new Vector2(1, 700), //Wall jump to top platform
            new Vector2(0, 450), //Jump gap to left platform
            new Vector2(1, 700), //Jump to next screen

            //Screen 10 (Entrance to Library)
            new Vector2(3, 1000), //Zero out against right wall
            new Vector2(2, 700), //Walk to left edge
            new Vector2(0, 700), //Jump gap to higher platform (left)
            new Vector2(2, 500), //Walk to left edge
            new Vector2(0, 400), //Jump gap to higher platform (left)
            new Vector2(1, 450), //Jump gap to the entrance of keep
            new Vector2(3, 800), //Walk over the edge into the keep
            new Vector2(2, 450), //Zero out against left edge
            new Vector2(1, 400), //Jump on table under carrots
            new Vector2(3, 900), //Walk off table - Zero out right table
            new Vector2(0, 700), //Wall jump of table to next screen

            //Screen 11 (False King's Keep)
            new Vector2(2, 250), //Get closer to left edge
            new Vector2(0, 700), //Wall jump of left to axe platform
            new Vector2(2, 250), //Get closer to left edge
            new Vector2(0, 500), //Wall jump of left to above platform
            new Vector2(1, 200), //Jump to far right platform
            new Vector2(3, 200), //Zero out agaist right wall
            new Vector2(2, 200), //Get closer to left edge
            new Vector2(0, 700), //Wall jump (left) to top platform
            new Vector2(0, 700), //Big jump across gap to next screen

            //Screen 12 (Chandalier - Roof Entrance)
            new Vector2(3, 200), //Get closer to right edge
            new Vector2(1, 700), //Big jump to far right platform
            new Vector2(3, 400), //Zero out against right wall
            new Vector2(0, 525), //Jump to top of chandalier
            new Vector2(2, 150),//Move more left to clear jump
            new Vector2(1, 700), //Jump to next screen

            //Screen 13 (Painting Room)
            new Vector2(0, 700), //Jump gap to left platform
            new Vector2(2, 600), //Walk to the left edge (outside)
            new Vector2(0, 425), //Jump to outside block
            new Vector2(1, 500), //Jump back inside up top
            new Vector2(3, 200), //Get closer to edge
            new Vector2(1, 700), //Jump gap to top block
            new Vector2(3, 250), //Zero out agaisnt wall
            new Vector2(2, 200), //get closer to edge for jump
            new Vector2(0, 700), //Jump to next screen

            //Screen 14 (Top of Keep)
            new Vector2(2, 450), //Zero out against wall
            new Vector2(1, 700), //Jump to first platform
            new Vector2(0, 400), //Jump to second
            new Vector2(0, 700), //Jump to next screen
            
            //Screen 15 (Bargainburg)
            new Vector2(2, 350), //Zero out against salesman
            new Vector2(3, 480), //Walk to edege
            new Vector2(1, 700), //Jump gap
            new Vector2(1, 500), //Jump inside house
            new Vector2(3, 500), //Zero out with wall
            new Vector2(2, 575), //Line up to edge
            new Vector2(0, 700), //Roof Pit
            new Vector2(3, 250), //Zero out with wall
            new Vector2(0, 700), //Jump to next screen

            //Screen 16 (Transalvania - Bulletin Board)
            new Vector2(1, 480), //Jump gap
            new Vector2(3, 365), //Walk to edge
            new Vector2(1, 150), //Jump to middle small ledge
            new Vector2(1, 100), //Jump to outside small ledge
            new Vector2(1, 700), //Wall bounce to top
            new Vector2(0, 450), //Jump tp top ledge
            new Vector2(1, 700), //Jump to next screen

            //Screen 17 - Birds and Hard Tower
            new Vector2(0, 450), //Jump to middle house ledge
            new Vector2(2, 200), //Zero out against wall (left)
            new Vector2(1, 500), //Jump to far left platform
            new Vector2(3, 200), //Zero out against wall (right)
            new Vector2(2, 125), //Get Closer To Edge
            new Vector2(0, 500), //Jump into hard house
            new Vector2(3, 200), //Zero out against wall (right)
            new Vector2(2, 250), //Walk to edge (left)
            new Vector2(0, 275), //Jump the middle gap
            new Vector2(2, 800), //Walk to zero out against wall (left)
            new Vector2(0, 700), //Wall bounce (left) to next platform
            new Vector2(2, 250), //Walk down to slightly lower platform
            new Vector2(3, 250), //Zero out (right)
            new Vector2(0, 700), //Wall bounce (left) to next platform
            new Vector2(2, 250), //Walk down to slightly lower platform
            new Vector2(3, 250), //Zero out (right)
            new Vector2(0, 700), //Wall bounce (left) to next screen

            //Screen 18 (hidden room ledege - stairs)
            new Vector2(3, 150), //Zero to right
            new Vector2(0, 500), //Wall bounce (left) to higher platform
            new Vector2(3, 300), //Zero out (right)
            new Vector2(1, 700), //Wall jump (right) to higher platform (left)
            new Vector2(1, 200), //Jump to lower platform
            new Vector2(3, 350), //Walk to edge
            new Vector2(1, 100), //Clear gap to lower ledge
            new Vector2(3, 1000), //Walk down stairs to lower gap
            new Vector2(2, 150), //Zero out against bottow ledge (right)
            new Vector2(1, 150), //Jump small lower gap
            new Vector2(3, 150), //Zero out against wall (right)
            new Vector2(1, 700), //Wall bounce to higher platform
            new Vector2(0, 500), //Jump to next higher platform
            new Vector2(2, 500), //Zero out against wall (right)
            new Vector2(1, 350), //Walk towards edge (left)
            new Vector2(1, 450), //Jump to next higher platform
            new Vector2(1, 400), //Wall bounce (right) next higher platform
            new Vector2(1, 700), //Wall bounce (right) to next screen

            //Screen 19 (UNTESTED)
            new Vector2(0, 450), //Jump to next higher platform
            new Vector2(0, 500), //Jump to next higher platform
            new Vector2(1, 700), //Jump to next higher platform
            new Vector2(3, 200), //Jump to next higher platform
            new Vector2(1, 700), //Jump to next higher platform

            //Screen 20 (UNTESTED
            new Vector2(1, 200), //Jump to next higher platform
            new Vector2(1, 500), //Jump to next higher platform
            new Vector2(3, 700), //Jump to next higher platform
            new Vector2(1, 500), //Jump to next higher platform
            new Vector2(1, 700), //Jump to next higher platform

            //I gave up on the move array.
        };  
    }
}

