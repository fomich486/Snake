namespace NetworkExtension
{
    public class Command
    {
        public int Id { get; set; }

        public Command(ServerCommands _id)
        {
            Id = (int)_id;
        }


    }
}