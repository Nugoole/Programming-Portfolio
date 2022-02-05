using ALT.DSCamera.Interface;
using ALT.DSCamera.Tool;
using Cognex.VisionPro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ALT.DSCamera
{
    public class _3DParamContainer : I3DCamParams, I3DRegionParams, I3DCrossSectionRegion, IAppliable<_3DParamContainer>, INotifyPropertyChanged
    {
        #region PropertyChangedConfig
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Field
        private static _3DParamContainer instance;

        private double sensitivity;
        private string laserMode;
        private int scanLength;
        private double zDetectionBase;
        private double zDetectionHeigt;
        private string acquireDirection;
        private string encoderDirection;
        private bool useEncoder;
        private bool highDynamicRange;
        private double xScale;
        private int encoderRes;
        private double stepsPerLine;
        private double distancePerCycle;
        private double firstPixelLocation;
        private double exposure;
        private List<RegionParams> heightCalcRegions;
        private List<List<RegionParams>> tiltCalcRegions;


        private bool useHardwareTrigger;
        private double motionSpeed;
        private int laserPower;
        private Dictionary<string, CrossSectionSaveLoadParam> crossSectionRegions;
        #endregion

        [JsonIgnore]
        public static _3DParamContainer Instance
        {
            get
            {
                if (instance is null)
                    instance = new _3DParamContainer();

                return instance;
            }
        }




        public double Exposure
        {
            get => exposure; set
            {
                exposure = value;
                RaisePropertyChanged();
            }

        }

        public double Sensitivity
        {
            get
            { return sensitivity; }
            set
            {
                if (value >= 0 && value <= 1)
                {
                    sensitivity = value;

                }
                RaisePropertyChanged();
            }

        }
        public string LaserMode
        {
            get
            {
                return laserMode;
            }
            set
            {
                if (Enum.GetNames(typeof(CogAcqLaserModeConstants)).Contains(value))
                {
                    laserMode = value;
                }
                RaisePropertyChanged();
            }
        }
        public int ScanLength
        {
            get
            {
                return scanLength;
            }
            set
            {
                if (value > 0)
                {
                    scanLength = value;
                }
                RaisePropertyChanged();
            }
        }
        public double ZDetectionBase
        {
            get
            {
                return zDetectionBase;
            }
            set
            {
                zDetectionBase = value;

                RaisePropertyChanged();
            }
        }
        public double ZDetectionHeight
        {
            get
            {
                return zDetectionHeigt;
            }
            set
            {
                zDetectionHeigt = value;

                RaisePropertyChanged();
            }
        }
        public string AcquireDirection
        {
            get
            {
                return acquireDirection;
            }
            set
            {
                if (Enum.GetNames(typeof(CogProfileCameraDirectionConstants)).Contains(value))
                {
                    acquireDirection = value;
                }
                RaisePropertyChanged();
            }
        }
        public string EncoderDirection
        {
            get
            {
                return encoderDirection;
            }
            set
            {
                if (Enum.GetNames(typeof(CogProfileCameraDirectionConstants)).Contains(value))
                {
                    encoderDirection = value;
                }
                RaisePropertyChanged();
            }
        }
        public bool UseEncoder
        {
            get
            {
                return useEncoder;
            }
            set
            {
                useEncoder = value;
                RaisePropertyChanged();
            }
        }
        public bool UseHardwareTrigger
        {
            get => useHardwareTrigger; set
            {
                useHardwareTrigger = value;
                RaisePropertyChanged();
            }
        }
        public bool HighDynamicRange
        {
            get
            {
                return highDynamicRange;
            }
            set
            {
                highDynamicRange = value;
                RaisePropertyChanged();
            }
        }
        public double XScale
        {
            get
            {
                return xScale;
            }
            set
            {
                xScale = value;
                RaisePropertyChanged();
            }
        }

        public double MotionSpeed
        {
            get => motionSpeed; set
            {
                motionSpeed = value;
                RaisePropertyChanged();
            }
        }
        public int EncoderRes
        {
            get
            {
                return encoderRes;
            }
            set
            {
                encoderRes = value;
                RaisePropertyChanged();
            }
        }
        public double StepsPerLine
        {
            get
            {
                return stepsPerLine;
            }
            set
            {
                stepsPerLine = value;
                RaisePropertyChanged();
            }
        }
        public double DistancePerCycle
        {
            get
            {
                return distancePerCycle;
            }
            set
            {
                distancePerCycle = value;
                RaisePropertyChanged();
            }
        }
        public double FirstPixelLocation
        {
            get
            {
                return firstPixelLocation;
            }
            set
            {
                firstPixelLocation = value;
                RaisePropertyChanged();
            }
        }

        public List<RegionParams> HeightCalcRegions
        {
            get => heightCalcRegions; set
            {
                heightCalcRegions = value;
                RaisePropertyChanged();
            }
        }
        public List<List<RegionParams>> TiltCalcRegions
        {
            get => tiltCalcRegions; set
            {
                tiltCalcRegions = value;
                RaisePropertyChanged();
            }
        }

        public int LaserPower
        {
            get => laserPower; set
            {
                laserPower = value;
                RaisePropertyChanged();
            }
        }

        public Dictionary<string, CrossSectionSaveLoadParam> CrossSectionRegions
        {
            get => crossSectionRegions;
            set
            {
                crossSectionRegions = value;
                RaisePropertyChanged();
            }
        }

        public void ApplyParams(I3DCamParams paramObject)
        {
            GetType().GetProperties().ToList().ForEach(x =>
            {
                if (x.CanWrite)
                    x.SetValue(this, paramObject.GetType().GetProperty(x.Name)?.GetValue(paramObject));
            });
        }

        public void ApplyParams(I3DHeightCalcRegion paramObject)
        {
            GetType().GetProperties().ToList().ForEach(x =>
            {
                if (x.CanWrite)
                    x.SetValue(this, paramObject.GetType().GetProperty(x.Name)?.GetValue(paramObject));
            });
        }

        public void ApplyParams(I3DTiltCalcRegion paramObject)
        {
            GetType().GetProperties().ToList().ForEach(x =>
            {
                if (x.CanWrite)
                    x.SetValue(this, paramObject.GetType().GetProperty(x.Name)?.GetValue(paramObject));
            });
        }

        public void ApplyParams(I3DCrossSectionRegion paramObject)
        {
            GetType().GetProperties().ToList().ForEach(x =>
            {
                if (x.CanWrite)
                    x.SetValue(this, paramObject.GetType().GetProperty(x.Name)?.GetValue(paramObject));
            });
        }

        public void ApplyParams(_3DParamContainer paramObject)
        {
            GetType().GetProperties().ToList().ForEach(x =>
            {
                if (x.CanWrite)
                    x.SetValue(this, paramObject.GetType().GetProperty(x.Name)?.GetValue(paramObject));
            });
        }



        private _3DParamContainer()
        {
            heightCalcRegions = new List<RegionParams>();
            tiltCalcRegions = new List<List<RegionParams>>();
        }
    }
}
