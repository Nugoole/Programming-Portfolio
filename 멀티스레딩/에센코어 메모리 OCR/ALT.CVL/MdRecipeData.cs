using ALT.CVL.Interfaces;
using System;
using System.Collections.ObjectModel;

namespace ALT.CVL
{
    public class MdRecipeData : IRecipe
    {
        #region Singleton Constructor
        private static readonly Lazy<MdRecipeData> instance = new Lazy<MdRecipeData>(() => new MdRecipeData());
        private MdRecipeData()
        {
            OCRecipe = new ObservableCollection<MdRecipeInfo>();
        }
        public static MdRecipeData Getinstance() => instance.Value;
        #endregion

        #region Variables

        #endregion

        #region Properties
        public ObservableCollection<MdRecipeInfo> OCRecipe { get; set; }
        #endregion

        #region Functions

        #endregion
    }
    
}
