public partial class CardScript
{
    enum status 
    {
        // regular size every cards on hand
        standard,   

        // bigger main focus on this card 
        selected,

        // same as selected but flipped to god skills and description
        revealed,

        // smaller and darker while any 1 is selected left 2 card is away focus
        unfocused
    }
}
