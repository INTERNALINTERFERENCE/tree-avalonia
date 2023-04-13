using System.Timers;

namespace tree.Services.Tree;

public class TreeRoot
    : TreeNode
{
    public TreeRoot()
        : base( "" )
    {
        var timer = new Timer( 1000 );
        timer.Elapsed += OnTimerElapsed;
        timer.Start();
    }

    private void OnTimerElapsed( object? sender, ElapsedEventArgs e )
    {
        Root.EnqueueUpdate();
    }
}