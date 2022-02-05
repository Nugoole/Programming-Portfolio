using System;

namespace ALT.CVL
{
    public class MdRecipeInfo
    {
        #region Constructor
        public MdRecipeInfo()
        {

        }
        #endregion

        #region Variables

        #endregion

        #region Properties
        public string ModelName { get; set; }
        public DateTime CreationTime { get; set; }
        public string ModelImagePath { get; }
        #endregion
    }
}
