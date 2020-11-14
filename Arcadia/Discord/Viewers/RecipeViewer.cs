namespace Arcadia.Services
{
    public class RecipeViewer : IViewer<Recipe>
    {
        /// <inheritdoc />
        public string ViewDefault(ArcadeUser invoker)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public string View(ArcadeUser invoker, string query, int page)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public string ViewSingle(ArcadeUser invoker, Recipe model)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public string PreviewSingle(ArcadeUser invoker, Recipe model)
        {
            throw new System.NotImplementedException();
        }
    }
}