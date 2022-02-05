using System.Collections.ObjectModel;

namespace ALT.CVL.Interfaces
{
    public interface IRecipe
    {
        ObservableCollection<MdRecipeInfo> OCRecipe { get; set; }
    }
}
