using System;
using System.Linq;
using System.Text;

namespace ALT.DSCamera.Camera3D
{
    public partial class Md3DFrameGrabber
    {
        string DSMaxParamAsStringBuffer = string.Empty;

        private void DSSetFeature(string featureName, string featureValue)
        {
            cam3D.FrameGrabber.OwnedGenTLAccess.SetFeature(featureName, featureValue);
        }
        private string DSGetFeature(string featureName)
        {
            try
            {
                return cam3D.FrameGrabber.OwnedGenTLAccess.GetFeature(featureName);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        internal void AddDefaultPropertyFromDSMax(string propertyName, string value)
        {
            if (string.IsNullOrEmpty(DSMaxParamAsStringBuffer))
                DSMaxParamAsStringBuffer = cam3D.OwnedCustomPropertiesParams.CustomPropsAsString;

            var text = DSMaxParamAsStringBuffer;

            text.Concat("\n\r");
            text.Concat(propertyName);
            text.Concat("\t");
            text.Concat(value);

            cam3D.OwnedCustomPropertiesParams.CustomPropsAsString = text;
        }

        internal void SetDefaultPropertyFromDSMax(string propertyName, string value)
        {
            if (string.IsNullOrEmpty(DSMaxParamAsStringBuffer))
                DSMaxParamAsStringBuffer = cam3D.OwnedCustomPropertiesParams.CustomPropsAsString;

            var text = DSMaxParamAsStringBuffer;

            var textList = text.Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
            var oldValue = textList.Where(x => x.Contains(propertyName)).First().Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries).Last();
            string readText = textList.Where(x => x.Contains(propertyName)).First().Replace(oldValue, value);

            StringBuilder builder = new StringBuilder();

            foreach (var item in textList)
            {
                if (item.Contains(propertyName))
                    builder.Append(readText);
                else
                    builder.Append(item);

                builder.Append("\n\r");
            }

            DSMaxParamAsStringBuffer = builder.ToString();
        }
        internal string GetDSMaxSystemProperty(string propertyName)
        {
            return cam3D.FrameGrabber.OwnedGenTLAccess.GetFeature($"SY_{propertyName}");
        }
        internal string GetDSMaxInterfaceProperty(string propertyName)
        {
            return cam3D.FrameGrabber.OwnedGenTLAccess.GetFeature($"IF_{propertyName}");
        }
        internal string GetDSMaxRemoteDeviceProperty(string propertyName)
        {
            return cam3D.FrameGrabber.OwnedGenTLAccess.GetFeature($"RD_{propertyName}");
        }
        internal string GetDSMaxLocalDeviceProperty(string propertyName)
        {
            return cam3D.FrameGrabber.OwnedGenTLAccess.GetFeature($"LD_{propertyName}");
        }
        internal string GetDSMaxStreamProperty(string propertyName)
        {
            return cam3D.FrameGrabber.OwnedGenTLAccess.GetFeature($"DS_{propertyName}");
        }
        internal void SetDSMaxSystemProperty(string propertyName, string value)
        {
            cam3D.FrameGrabber.OwnedGenTLAccess.SetFeature($"SY_{propertyName}", value);
        }
        internal void SetDSMaxInterfaceProperty(string propertyName, string value)
        {
            cam3D.FrameGrabber.OwnedGenTLAccess.SetFeature($"IF_{propertyName}", value);
        }
        internal void SetDSMaxRemoteDeviceProperty(string propertyName, string value)
        {
            cam3D.FrameGrabber.OwnedGenTLAccess.SetFeature($"RD_{propertyName}", value);
        }
        internal void SetDSMaxLocalDeviceProperty(string propertyName, string value)
        {
            cam3D.FrameGrabber.OwnedGenTLAccess.SetFeature($"LD_{propertyName}", value);
        }
        internal void SetDSMaxStreamProperty(string propertyName, string value)
        {
            cam3D.FrameGrabber.OwnedGenTLAccess.SetFeature($"DS_{propertyName}", value);
        }
    }
}
