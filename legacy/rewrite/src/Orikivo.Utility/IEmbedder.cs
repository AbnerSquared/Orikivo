using Discord;
using Orikivo.Utility;
using Orikivo.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    /// <summary>
    /// Defines the basic properties of an object's embeddable data.
    /// </summary>
    public interface IPanel<T>
    {
        T Source { get; }
        Embed Build();
    }

    // define how an embed is built.
    public class EmbedderOptions
    {

    }

    // embedders have toggles.
    // make Embeddable, which defines how 

    public class DblBotPanel : IPanel<IDblBot>
    {
        public DblBotPanel(IDblBot bot)
        {
            Source = bot;
            Embed = Embedder.DefaultEmbed;
        }
        public IDblBot Source { get; }
        private EmbedBuilder Embed { get; set; } // the base embed used.
        public Embed Build() { return Embed.Build(); }

        public static implicit operator DblBotPanel(DblBot b)
            => new DblBotPanel(b);
    }

    // in short, this defines how a data object looks for a specific 
    /// <summary>
    /// Represents a class that defines how a specific object is read.
    /// </summary>
    public class ObjectPanelReader
    {
        
    }
}
