using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents a generic player structure for a <see cref="GameBase{T}"/>.
    /// </summary>
    public abstract class PlayerBase
    {
        /// <summary>
        /// Gets the <see cref="Player"/> that is bound to this <see cref="PlayerBase"/>.
        /// </summary>
        public Player Connection { get; internal set; } // Receiver or Recipient

        public abstract PlayerBase GetDefault();

        /// <summary>
        /// Returns a collection of all specified <see cref="GameProperty"/> values on this <see cref="PlayerBase"/>.
        /// </summary>
        public List<GameProperty> ExportProperties()
        {
            return
                GetType()
                    .GetProperties()
                    .Where(x => x.GetCustomAttribute<PropertyAttribute>() != null)
                    .Select(x => GameProperty.Create(x.GetCustomAttribute<PropertyAttribute>()?.Id, x.GetValue(this), true))
                    .ToList();
        }

        public GameProperty FindProperty(string id)
        {
            PropertyInfo info = GetType().GetProperties().FirstOrDefault(x => x.GetCustomAttribute<PropertyAttribute>()?.Id == id);

            if (info == null)
                return null;

            return GameProperty.Create(info.GetCustomAttribute<PropertyAttribute>()?.Id, info.GetValue(this), true);
        }

        public void SetProperty(string id, object value)
        {

        }

        /// <summary>
        /// Resets this <see cref="PlayerBase"/> to its initial state.
        /// </summary>
        public void Reset()
        {
            foreach (PropertyInfo property in GetType().GetProperties())
            {
                property.SetValue(this, property.GetValue(GetDefault()));
            }
        }
    }
}