using ALT.BoltHeight.Views;
using System.Windows.Controls;

namespace ALT.BoltHeight.Views
{
    public class MdDynamicUC
    {
        private const int ColumnCount = 4; // 열 설정
        private const int RowCount = 2; // 행 설정

        private UCSubModelView[] uCSubArray; 
        private Grid modelViewGrid;
        private static MdDynamicUC dynamicUC;
        public string[] ModelArray { get; set; }
        private MdDynamicUC() // 생성자
        {
            
        }
        
        private void UCDataSet() // 행 열 값에 맞춰 해당 수량만큼의 UC를 생성
        {
            if(uCSubArray == null)
            {
                uCSubArray = new UCSubModelView[ColumnCount * RowCount]; // 행 열에 맞춰서 UC 공간 생성
                for (int a = 0; a < ColumnCount * RowCount; a++)
                {
                    UCSubModelView uCSub = new UCSubModelView();
                    uCSubArray[a] = uCSub;
                }
            }
        }
        public static MdDynamicUC GetDynamicCs() // 생성자
        {
            if (dynamicUC == null)
                dynamicUC = new MdDynamicUC();
            return dynamicUC;
        }
        public Grid GridDraw() // 그리드에 UC를 그려 리턴
        {
            int count = 0;
            UCDataSet();
            if (modelViewGrid == null)
            {
                modelViewGrid = new Grid();
                for (int col = 0; col < ColumnCount; col++)
                {
                    ColumnDefinition colDef = new ColumnDefinition();
                    modelViewGrid.ColumnDefinitions.Add(colDef);
                }
                for (int row = 0; row < RowCount; row++)
                {
                    RowDefinition rowDef = new RowDefinition();
                    modelViewGrid.RowDefinitions.Add(rowDef);
                }
            }
            else
            {
                modelViewGrid.Children.Clear();
            }
            for (int col = 0; col < ColumnCount; col++)
            {
                for (int row = 0; row < RowCount; row++)
                {
                    if(ModelArray == null)
                    {
                        ModelArray = new string[ColumnCount * RowCount];
                        for (int ModelDefaultCount = 0; ModelDefaultCount < ColumnCount * RowCount; ModelDefaultCount++)
                        {
                            ModelArray[ModelDefaultCount] = "Default" + ModelDefaultCount.ToString();
                        }
                    }
                    uCSubArray[count].BtnModel.Content = ModelArray[count];
                    Grid.SetColumn(uCSubArray[count], col);
                    Grid.SetRow(uCSubArray[count], row);
                    modelViewGrid.Children.Add(uCSubArray[count]);
                    count++;
                }
            }
            return modelViewGrid;
        }
        public void UCModelNameChange(string btnModelTextName) // 선택한 모델의 이름을 변경
        {
            for(int i = 0; i<uCSubArray.Length; i++)
            {
                if(btnModelTextName == uCSubArray[i].BtnModel.Content.ToString())
                {
                    ModelArray[i] = uCSubArray[i].TxtModelname.Text;
                }
            }
        }
    }
}
