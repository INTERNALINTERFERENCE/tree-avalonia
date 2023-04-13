using System.Collections.Generic;
using System.Reactive;
using ReactiveUI;
using tree.Services.Tree;

namespace tree.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public TreeRoot TreeRoot { get; }

        public MainWindowViewModel()
        {
            var root = new TreeRoot { IsExpanded = false };
            root.ForceResync();

            var arr = new[]
            {
                "svc/system/license/state",
                "svc/system/license/beacon",
                "sys/system/archive/config"
            };

            foreach (var str in arr)
            {
                root.Add(str.Split('/'));
            }

            TreeRoot = root;
        }

        public void ChangeNodeExpandedValueCommand()
        {
            SelectedItem!.IsExpanded
                = !SelectedItem.IsExpanded;
        }
        
        public TreeNode? SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                if ( value != null )
                    ((IReactiveObject)this).RaisePropertyChanged();
            }
        }

        private TreeNode? _selectedItem;
    }
}