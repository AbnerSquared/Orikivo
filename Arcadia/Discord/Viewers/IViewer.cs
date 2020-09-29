using Arcadia.Models;

namespace Arcadia.Modules
{
    public interface IViewer <TModel> where TModel : IModel
    {
        // This is the 'main menu' of the viewer, mainly to show generalized information
        string ViewDefault();

        // This is used to view specifics based on a query
        string View(string query, int page);

        // For viewing the full details of a model
        string ViewSingle(TModel model);

        // For previewing a model on a page
        string PreviewSingle(TModel model);
    }
}
