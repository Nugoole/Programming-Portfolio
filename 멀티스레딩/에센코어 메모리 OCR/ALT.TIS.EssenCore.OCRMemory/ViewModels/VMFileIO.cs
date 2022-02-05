using ALT.CVL;
using ALT.CVL.Interfaces;
using ALT.Log.Interface;
using ALT.Log.Model;
using ALT.TIS.EssenCore.OCRMemory.Interfaces;
using ALT.TIS.EssenCore.OCRMemory.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ALT.TIS.EssenCore.OCRMemory.ViewModels
{
    public class VMFileIO : ViewModelBase
    {
        #region Constructor
        public VMFileIO()
        {
            InitVMFileIO();
            InitCommand();
        }
        private void InitVMFileIO() // 시작 초기화
        {
            RecipeData = MdRecipeData.Getinstance();
            ConfigData = MdConfigData.Getinstance().ConfigParam;
            if (ConfigData.RecipePath == AppDomain.CurrentDomain.BaseDirectory)
            {
                MdFileIO.Getinstance().AddFolder($@"{AppDomain.CurrentDomain.BaseDirectory}", "Model");
                ConfigData.RecipePath = $@"{AppDomain.CurrentDomain.BaseDirectory}\Model";
            }
            
            RefreshOCRecipeList(ConfigData.RecipePath);
        }
        private void InitCommand()
        {
            CmdRecipeSave = new RelayCommand<MdRecipeInfo>(RecipeSave);
            CmdRecipeLoad = new RelayCommand<MdRecipeInfo>(RecipeLoad);
            CmdConfigSave = new RelayCommand(ConfigSave);
            CmdRecipeSelectingUP = new RelayCommand(RecipeSelectingUP);
            CmdRecipeSelectingDown = new RelayCommand(RecipeSelectingDown);
            CmdAddRecipeDlgOpen = new RelayCommand(AddRecipeDlgOpen);
            CmdAddRecipe = new RelayCommand<string>(AddRecipe);
            CmdAddRecipeCancel = new RelayCommand(AddRecipeCancel);
            CmdDeleteRecipe = new RelayCommand(DeleteRecipe);
            CmdRecipePathSetDlgOpen = new RelayCommand(RecipePathSetDlgOpen);
            CmdRecipeFolderOpen = new RelayCommand<string>(RecipeFolderOpen);
            CmdNGImgPathSetDlgOpen = new RelayCommand(NGImgPathSetDlgOpen);
            CmdNGImgFolderOpen = new RelayCommand<string>(NGImgFolderOpen);
        }
        #endregion

        #region Variables
        private Window WdRecipeAddView;
        private readonly string recipeConfigFileName = "RecipeInfo";
        private ILogger FileLogger => LoggerDictionary.Instance.GetLogger(Log.Enum.LogUsage.FileIO);
        #endregion

        #region Properties
        public RelayCommand<MdRecipeInfo> CmdRecipeSave { get; private set; } // 레시피 저장
        public RelayCommand<MdRecipeInfo> CmdRecipeLoad { get; private set; } // 레시피 불러오기
        public RelayCommand CmdConfigSave { get; private set; } // 설정 저장
        public RelayCommand CmdRecipeSelectingUP { get; private set; } // List에서 선택한 Index Up
        public RelayCommand CmdRecipeSelectingDown { get; private set; } // List에서 선택한 Index Down
        public RelayCommand CmdAddRecipeDlgOpen { get; private set; } // 레시피 추가할 창 열기
        public RelayCommand<string> CmdAddRecipe { get; private set; } // 레시피 추가
        public RelayCommand CmdAddRecipeCancel { get; private set; } // 레시피 추가 취소
        public RelayCommand CmdDeleteRecipe { get; private set; } // 레시피 삭제
        public RelayCommand CmdRecipePathSetDlgOpen { get; private set; } // 모델 저장 경로 설정창 열기
        public RelayCommand<string> CmdRecipeFolderOpen { get; private set; } // 모델 폴더 열기
        public RelayCommand CmdNGImgPathSetDlgOpen { get; private set; } // 불량이미지 저장 경로 설정창 열기
        public RelayCommand<string> CmdNGImgFolderOpen { get; private set; } // 불량이미지 폴더 열기

        private int recipeListSelectedIndex;
        public int RecipeListSelectedIndex
        {
            get => recipeListSelectedIndex;
            set
            {
                if (value <= 0)
                    //recipeListSelectedIndex = RecipeData.OCRecipe.Count - 1;
                    recipeListSelectedIndex = 0;
                else if (value >= RecipeData.OCRecipe.Count)
                    recipeListSelectedIndex = RecipeData.OCRecipe.Count - 1;
                //recipeListSelectedIndex = 0;
                else
                    recipeListSelectedIndex = value;
                RaisePropertyChanged();
            }
        }
        public string CurrentJobName { get; set; }
        public IRecipe RecipeData { get; set; }
        public IConfigParam ConfigData { get; set; }
        #endregion

        #region Functions
        private void RefreshOCRecipeList(string filePath) // ObservableCollection Refresh Func 완료 , 뷰모델용함수
        {
            MdRecipeData.Getinstance().OCRecipe.Clear();

            //DirectoryInfo info = new DirectoryInfo(filePath);
            var info = MdFileIO.Getinstance().GetDirectoryInfo(filePath);

            foreach (var directoryName in info.GetDirectories().Select(x => x.Name))
            {
                var mdRecipeInfo = MdFileIO.JsonFileLoad<MdRecipeInfo>($@"{filePath}\{directoryName}", recipeConfigFileName);

                if (mdRecipeInfo != null)
                    MdRecipeData.Getinstance().OCRecipe.Add(mdRecipeInfo);
            }
            FileLogger.Write(Log.Enum.LogLevel.Info, "Recipe FileList Check");
        }

        private void RecipeSave(MdRecipeInfo recipeInfo)
        {
            for (int i = 0; i < ConfigData.CamCount; i++)
            {
                var camName = ConfigData.OCCameraData[i].CamName;
                MdFileIO.Getinstance().JsonFileSave(MdTotalParamData.Getinstance(),
                    $@"{ConfigData.RecipePath}\{recipeInfo.ModelName}\{camName}", $"{camName}");

                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<string>($@"{ConfigData.RecipePath}\{ recipeInfo.ModelName}\{camName}",camName);
                FileLogger.Write(Log.Enum.LogLevel.Info, $"{camName} Recipe Save");
            }
            
        }
        private void RecipeLoad(MdRecipeInfo recipeInfo)
        {
            for (int i = 0; i < ConfigData.CamCount; i++)
            {
                var camName = ConfigData.OCCameraData[i].CamName;
                var mdTotalParamInfo = MdFileIO.JsonFileLoad<MdTotalParamInfo>($@"{ConfigData.RecipePath}\{recipeInfo.ModelName}\{camName}",
                    $"{camName}");

                if (mdTotalParamInfo != null)
                    MdTotalParamData.Getinstance().OCTotalParamInfo[i] = mdTotalParamInfo;


                MdFileIO.Getinstance().ToolFileListSet(MdTotalParamData.Getinstance().OCTotalParamInfo[i].ToolParamInfo,
                    $@"{ConfigData.RecipePath}\{ recipeInfo.ModelName}\{camName}");
            }
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<ObservableCollection<MdTotalParamInfo>, IVMTool>(MdTotalParamData.Getinstance().OCTotalParamInfo);
            FileLogger.Write(Log.Enum.LogLevel.Info, $"Recipe Load");
        }

        private void ConfigSave() // Config 저장 완료
        {
            MdFileIO.Getinstance().JsonFileSave(ConfigData, MdConfigData.Getinstance().configFilePath, MdConfigData.Getinstance().configFileName);
            FileLogger.Write(Log.Enum.LogLevel.Info, $"Config Save");
        }

        private void RecipeSelectingUP() // Recipe Selecting Index Up 완료
        {
            RecipeListSelectedIndex--;
        }

        private void RecipeSelectingDown() // Recipe Selecting Index Down 완료
        {
            RecipeListSelectedIndex++;
        }

        private void AddRecipeDlgOpen() // Recipe 추가 창 오픈 완료
        {
            WdRecipeAddView = new WdRecipeAdd();
            WdRecipeAddView.ShowDialog();
        }
        private void AddRecipe(string modelName) // Recipe 추가 완료
        {
            MdFileIO.Getinstance().AddFolder(ConfigData.RecipePath, modelName);
            MdFileIO.Getinstance().JsonFileSave(new MdRecipeInfo { ModelName = modelName, CreationTime = DateTime.Now }, $@"{ConfigData.RecipePath}\{modelName}", recipeConfigFileName);
            RefreshOCRecipeList(ConfigData.RecipePath);
            WdRecipeAddView.DialogResult = true;
            FileLogger.Write(Log.Enum.LogLevel.Info, $"Recipe : {modelName} Add");
        }
        private void AddRecipeCancel() // Recipe 추가 취소 완료
        {
            WdRecipeAddView.DialogResult = false;
        }
        private void DeleteRecipe() // Recipe 삭제 완료
        {
            MdFileIO.Getinstance().DeleteFolder(ConfigData.RecipePath, RecipeData.OCRecipe[RecipeListSelectedIndex].ModelName);
            RefreshOCRecipeList(ConfigData.RecipePath);
        }
        private void RecipePathSetDlgOpen() // 모델 경로 설정 완료
        {
            string path = MdFileIO.Getinstance().DlgFolderPathSet();
            if (path != null)
            {
                ConfigData.RecipePath = path;
                FileLogger.Write(Log.Enum.LogLevel.Info, $"Path : Recipe Set");
            }
                
        }
        private void RecipeFolderOpen(string folderPath) // 모델 폴더 열기 완료
        {
            MdFileIO.Getinstance().DirectoryOpen(folderPath);
        }


        private void NGImgPathSetDlgOpen() // NGImg 경로 설정 완료
        {
            string path = MdFileIO.Getinstance().DlgFolderPathSet();
            if (path != null)
            {
                ConfigData.NGImgPath = path;
                FileLogger.Write(Log.Enum.LogLevel.Info, $"Path : NGImg Set");
            }
                
        }
        private void NGImgFolderOpen(string folderPath) // NG Img 폴더 열기 완료
        {
            MdFileIO.Getinstance().DirectoryOpen(folderPath);
        }
        #endregion
    }
}
