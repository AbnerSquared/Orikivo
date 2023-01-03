using Orikivo.Text;
using Orikivo.Text.Pagination;

namespace Arcadia
{
    public class TextBodyOptions
    {
        public Language Language { get; set; } = Language.English;

        public bool ShowTips { get; set; } = true;

        public int? CharacterLimit { get; set; }

        public int? PageCharacterLimit { get; set; } = 2000;
        
        public GroupSplitOptions SplitOptions = GroupSplitOptions.Element;

        public int? MaxSectionsPerPage { get; set; }

        public bool ShowHeaderOnPartialSection { get; set; }

        public SectionOverflowMode SectionOptions { get; set; }
    }
}
