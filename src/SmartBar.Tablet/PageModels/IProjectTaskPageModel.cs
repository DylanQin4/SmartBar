using CommunityToolkit.Mvvm.Input;
using SmartBar.Tablet.Models;

namespace SmartBar.Tablet.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}