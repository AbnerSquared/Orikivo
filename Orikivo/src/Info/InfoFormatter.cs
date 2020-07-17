using System.Collections.Generic;

namespace Orikivo
{
    // this class will handle how everything is displayed in an info service
    // this is used to allow multiple bots to use different help styles
    public abstract class InfoFormatter
    {
        public abstract List<GuideNode> OnGetGuides();
        public abstract string OnDrawMainPanel(InfoService service);
    }
}
