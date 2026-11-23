namespace TodoListApp
{
    public interface ICommand
    {
        void Execute();
        void Unexecute();
    }
}
