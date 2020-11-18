using System;
using Arcadia.Multiplayer;

namespace Arcadia
{
    public class GameViewer : IViewer<GameInfo>
    {
        public string ViewDefault(ArcadeUser invoker)
        {
            throw new NotImplementedException();
        }

        public string View(ArcadeUser invoker, string query, int page)
        {
            throw new NotImplementedException();
        }

        public string ViewSingle(ArcadeUser invoker, GameInfo game)
        {
            throw new NotImplementedException();
        }

        public string PreviewSingle(ArcadeUser invoker, GameInfo game)
        {
            throw new NotImplementedException();
        }
    }
}