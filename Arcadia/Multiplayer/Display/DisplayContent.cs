using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arcadia.Multiplayer
{
    public class DisplayContent
    {
        public DisplayContent()
        {
            Components = new List<IComponent>();
        }

        public List<IComponent> Components { get; set; }

        // when this value is specified,
        // all components are ignored,
        // only drawing this
        public string ValueOverride { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(ValueOverride))
                return ValueOverride;

            var writer = new StringBuilder();

            if (Components.Count == 0)
            {
                return "Empty Content";
            }

            foreach(IComponent component in Components.OrderBy(x => x.Position))
            {
                // if this component is not active, skip over it and continue
                if (!component.Active)
                    continue;

                // refer to the buffer, and only if the buffer is null, can a default component draw be specified
                writer.AppendLine(string.IsNullOrWhiteSpace(component.Buffer) ? component.Draw() : component.Buffer);
            }

            return writer.ToString();
        }

        private int GetLowestPosition()
            => Components.OrderBy(x => x.Position).First().Position;

        private int GetHighestPosition()
            => Components.OrderByDescending(x => x.Position).First().Position;

        public IComponent GetComponent(string id)
        {
            foreach(IComponent component in Components)
            {
                if (component.Id == id)
                    return component;
            }

            return null;
        }

        public ComponentGroup GetGroup(string id)
        {
            if (GetComponent(id) is ComponentGroup group)
                return group;

            throw new InvalidCastException("Unable to cast IComponent as ComponentGroup");
        }

        // this brings the specific component to the top of the list
        public void MoveToTop(string id)
        {
            IComponent component = GetComponent(id);

            if (component == null)
                return;

            // in this case, you would have to set the index of this component
            component.Position = GetLowestPosition() - 1;
            // to the lowest available position
        }

        // this brings the specific component to the bottom of the list
        public void MoveToBottom(string id)
        {
            IComponent component = GetComponent(id);

            if (component == null)
                return;

            component.Position = GetHighestPosition() + 1;
        }

        // this sets the specific component to the specific position
        public void SetPosition(string id, int position)
        {
            IComponent component = GetComponent(id);

            if (component == null)
                return;

            component.Position = position;
        }

        // swaps the position of both components
        // if either of the components don't exist,
        // cancel this the method
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
    }
}
