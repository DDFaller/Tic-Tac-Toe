namespace TicTacToe.Events
{
    public class GameBoardEventData
    {
        public int Position { get; private set; }
        public char ID { get; private set; }

        public GameBoardEventData( int position, char id )
        {
            Position = position;
            ID = id;
        }
    }
}