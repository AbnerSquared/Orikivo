using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Arcadia.Multiplayer
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class DisplayContent : List<IComponent>
    {
        public DisplayContent(string separator = "\n", string valueOverride = null)
        {
            ValueOverride = valueOverride;
            Separator = separator;
        }

        public string ValueOverride { get; set; }

        public string Separator { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(ValueOverride))
                return ValueOverride;

            var writer = new StringBuilder();

            if (Count == 0)
            {
                return "Empty Content";
            }

            int i = 0;
            foreach(IComponent component in this.OrderBy(x => x.Position))
            {
                // if this component is not active, skip over it and continue
                if (!component.Active)
                    continue;

                if (i > 0)
                    writer.Append(Separator);

                writer.Append(string.IsNullOrWhiteSpace(component.Buffer) ? component.Draw() : component.Buffer);
                i++;
            }

            return writer.ToString();
        }

        private int GetLowestPosition()
            => this.OrderBy(x => x.Position).First().Position;

        private int GetHighestPosition()
            => this.OrderByDescending(x => x.Position).First().Position;

        public IComponent GetComponent(string id)
        {
            return this.FirstOrDefault(x => x.Id == id);
        }

        public ComponentGroup GetGroup(string id)
        {
            if (GetComponent(id) is ComponentGroup group)
                return group;

            throw new InvalidCastException("Unable to cast IComponent as ComponentGroup");
        }

        public void MoveToTop(string id)
        {
            IComponent component = GetComponent(id);

            if (component == null)
                return;

            component.Position = GetLowestPosition() - 1;
        }

        public void MoveToBottom(string id)
        {
            IComponent component = GetComponent(id);

            if (component == null)
                return;

            component.Position = GetHighestPosition() + 1;
        }

        public void SetPosition(string id, int position)
        {
            IComponent component = GetComponent(id);

            if (component == null)
                return;

            component.Position = position;
        }

        public void Swap(string oldId, string newId)
        {
            IComponent a = GetComponent(oldId);
            IComponent b = GetComponent(newId);

            if (a == null || b == null)
                return;

            int oldPos = a.Position;
            a.Position = b.Position;
            b.Position = oldPos;
        }

        private string DebuggerDisplay => $"Components: {Count}{(!string.IsNullOrWhiteSpace(ValueOverride) ? $", Value Override: {ValueOverride}" : "")}";
    }
}
