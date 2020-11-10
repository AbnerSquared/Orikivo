using Arcadia.Models;

namespace Arcadia
{
    public interface IViewer <in TModel>
        where TModel : IModel
    {
        // This is the 'main menu' of the viewer, mainly to show generalized information
        string ViewDefault(ArcadeUser invoker);

        // This is used to view specifics based on a query
        string View(ArcadeUser invoker, string query, int page);

        // For viewing the full details of a model
        string ViewSingle(ArcadeUser invoker, TModel model);

        // For previewing a model on a page
        string PreviewSingle(ArcadeUser invoker, TModel model);
    }
}
