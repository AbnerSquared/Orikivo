using Orikivo.Desync;
using System.Collections.Generic;

namespace Orikivo
{
    // this class will handle how everything is displayed in an info service
    // this is used to allow multiple bots to use different help styles
    public abstract class InfoFormatter
    {
        public virtual List<GuideNode> OnLoadGuides()
            => new List<GuideNode>();

        public abstract string OnWriteMenu(InfoService service, BaseUser user = null);

        public virtual string OnReadModule(ModuleNode module)
            => module.ToString();

        public virtual string OnReadGroup(ModuleNode group)
            => group.ToString();

        public virtual string OnReadCommand(CommandNode command)
            => command.ToString();

        public virtual string OnReadOverload(OverloadNode overload)
            => overload.ToString();

        public virtual string OnReadParameter(ParameterNode parameter)
            => parameter.ToString();
    }
}
