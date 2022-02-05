using ALT.CVL.Common.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL
{
    /// <summary>
    /// 연결된 조명컨트롤러에 대한 컨트롤러 사전입니다.
    /// </summary>
    public static class LightControllerDictionary
    {
        internal static readonly Dictionary<string, ILightController> innerDictionary = new Dictionary<string, ILightController>();

        /// <summary>
        /// 연결되어 있는 컨트롤러 리스트 입니다.
        /// </summary>
        public static IReadOnlyDictionary<string, ILightController> Controllers => innerDictionary;
    }
}
