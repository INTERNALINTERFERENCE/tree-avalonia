using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;

namespace tree.Services.Tree;

public class TreeNode
    : ReactiveObject
{
    protected TreeNode( string part )
        : this( part, -1, null )
    {

    }

    private TreeNode(
        string part,
        int level,
        TreeNode? parent )
    {
        Part = part;
        Level = level;
        Parent = parent;
        Root = parent?.Root ?? this;
    }

    #region IO

    private bool _updateEnqueued;
    public int Level { get; }
    public TreeNode? Parent { get; }
    public TreeNode Root { get; }
    [Reactive] public string Part { get; set; }
    [Reactive] public int MessagesCount { get; set; }
    [Reactive] public int TopicsCount { get; set; }
    [Reactive] public int UiHistoryCounter { get; set; } = 0;

    private bool _isExpanded;
    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            this.RaiseAndSetIfChanged(ref _isExpanded, value);
            Root.EnqueueUpdate();
        }
    }

    public List<TreeNode> VisibleChildren { get; set; } = new();
    
    public readonly SortedDictionary<string, TreeNode> TreeChildren = new();

    #endregion

    public bool Add( string[] subTopic )
        => Add( subTopic, Level + 1 );

    public void ForceResync()
        => Root.Update();
    
    public void EnqueueUpdate()
    {
        if ( _updateEnqueued )
            return;

        _updateEnqueued = true;

        Dispatcher.UIThread.Post( Update, DispatcherPriority.Background );
    }
    
    private bool Add( IReadOnlyList<string> topic, int level )
    {
        // тут нужно добавлять элемент в историю, предпалагалось, что Add будет иметь еще один параметр T, который ляжет в историю
        if ( level >= topic.Count )
            return false; 

        var part = topic[ level ];

        if (!TreeChildren.TryGetValue(part, out var tree))
        {
            TopicsCount++;
            TreeChildren.Add(part, tree = new TreeNode( part, level, this ));
        }
        
        MessagesCount++;
        tree.Add( topic, level + 1 );

        return true;
    }

    private void AppendItems()
    {
        _updateEnqueued = false;

        var flatTreeList = new List<TreeNode>();

        AppendItems( flatTreeList, this );

        VisibleChildren = flatTreeList;
    }

    private static void AppendItems(
        ICollection<TreeNode> flatTreeList,
        TreeNode treeNode )
    {
        flatTreeList.Add( treeNode );

        if ( !treeNode.IsExpanded )
            return;

        foreach ( var ch in treeNode.TreeChildren )
            AppendItems( flatTreeList, ch.Value );
    }

    private void Update()
    {
        AppendItems();
        this.RaisePropertyChanged( nameof( VisibleChildren ) );
    }
}